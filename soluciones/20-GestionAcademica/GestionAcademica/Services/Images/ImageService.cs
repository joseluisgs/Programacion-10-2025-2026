using System.IO;
using System.Windows.Media.Imaging;
using CSharpFunctionalExtensions;
using GestionAcademica.Errors.Common;
using GestionAcademica.Errors.Images;
using Serilog;

namespace GestionAcademica.Services.Images;

/// <summary>
/// Servicio para la gestión de imágenes de personas.
/// Maneja guardado, eliminación, validación y previsualización de imágenes.
/// </summary>
public class ImageService : IImageService
{
    private readonly ILogger _logger = Log.ForContext<ImageService>();
    private readonly string _imagesDirectory;
    private readonly string[] _allowedExtensions;

    private const long DefaultMaxSizeInBytes = 5_242_880; // 5 MB
    private const int DefaultMaxWidth = 4096;
    private const int DefaultMaxHeight = 4096;

    public ImageService(string imagesDirectory, string[] allowedExtensions)
    {
        _imagesDirectory = imagesDirectory;
        _allowedExtensions = allowedExtensions;
        
        if (!Directory.Exists(_imagesDirectory))
        {
            Directory.CreateDirectory(_imagesDirectory);
        }
    }

    public Result<string, DomainError> SaveImage(string sourcePath)
    {
        _logger.Information("Guardando imagen desde {SourcePath}", sourcePath);

        if (!File.Exists(sourcePath))
            return Result.Failure<string, DomainError>(ImageErrors.NotFound(sourcePath));

        if (!IsValidImage(sourcePath))
            return Result.Failure<string, DomainError>(ImageErrors.InvalidFormat(Path.GetExtension(sourcePath)));

        var sizeValidation = ValidateImageSize(sourcePath);
        if (sizeValidation.IsFailure)
            return Result.Failure<string, DomainError>(sizeValidation.Error);

        var mimeResult = ValidateMimeType(sourcePath);
        if (mimeResult.IsFailure)
            return Result.Failure<string, DomainError>(mimeResult.Error);

        var dimensionsValidation = ValidateImageDimensions(sourcePath);
        if (dimensionsValidation.IsFailure)
            return Result.Failure<string, DomainError>(dimensionsValidation.Error);

        try
        {
            var extension = Path.GetExtension(sourcePath).ToLower();
            var sanitizedFileName = SanitizeFileName($"{Guid.NewGuid()}{extension}");
            var destinationPath = Path.Combine(_imagesDirectory, sanitizedFileName);

            File.Copy(sourcePath, destinationPath, true);
            _logger.Information("Imagen guardada exitosamente como {FileName}", sanitizedFileName);
            
            return Result.Success<string, DomainError>(sanitizedFileName);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al guardar la imagen");
            return Result.Failure<string, DomainError>(ImageErrors.SaveError(ex.Message));
        }
    }

    public Result<bool, DomainError> DeleteImage(string fileName)
    {
        _logger.Information("Eliminando imagen {FileName}", fileName);
        
        var path = Path.Combine(_imagesDirectory, fileName);
        
        if (!File.Exists(path))
            return Result.Failure<bool, DomainError>(ImageErrors.NotFound(fileName));

        try
        {
            File.Delete(path);
            _logger.Information("Imagen {FileName} eliminada correctamente", fileName);
            return Result.Success<bool, DomainError>(true);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al eliminar la imagen {FileName}", fileName);
            return Result.Failure<bool, DomainError>(ImageErrors.DeleteError(ex.Message));
        }
    }

    public Result<bool, DomainError> UpdateImage(string sourcePath, string existingFileName)
    {
        _logger.Information("Actualizando imagen {ExistingFileName} con contenido de {SourcePath}", existingFileName, sourcePath);

        if (!File.Exists(sourcePath))
            return Result.Failure<bool, DomainError>(ImageErrors.NotFound(sourcePath));

        var destinationPath = Path.Combine(_imagesDirectory, existingFileName);
        if (!File.Exists(destinationPath))
            return Result.Failure<bool, DomainError>(ImageErrors.NotFound(existingFileName));

        if (!IsValidImage(sourcePath))
            return Result.Failure<bool, DomainError>(ImageErrors.InvalidFormat(Path.GetExtension(sourcePath)));

        var sizeValidation = ValidateImageSize(sourcePath);
        if (sizeValidation.IsFailure)
            return Result.Failure<bool, DomainError>(sizeValidation.Error);

        var dimensionsValidation = ValidateImageDimensions(sourcePath);
        if (dimensionsValidation.IsFailure)
            return Result.Failure<bool, DomainError>(dimensionsValidation.Error);

        try
        {
            File.Copy(sourcePath, destinationPath, true);
            _logger.Information("Contenido de imagen {ExistingFileName} actualizado correctamente", existingFileName);
            return Result.Success<bool, DomainError>(true);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al actualizar la imagen {ExistingFileName}", existingFileName);
            return Result.Failure<bool, DomainError>(ImageErrors.SaveError(ex.Message));
        }
    }

    public bool IsValidImage(string sourcePath)
    {
        var extension = Path.GetExtension(sourcePath).ToLower();
        return _allowedExtensions.Contains(extension);
    }

    public Result<bool, DomainError> ValidateImageSize(string sourcePath, long maxSizeInBytes = DefaultMaxSizeInBytes)
    {
        _logger.Debug("Validando tamaño de imagen: {Path}", sourcePath);

        if (!File.Exists(sourcePath))
            return Result.Failure<bool, DomainError>(ImageErrors.NotFound(sourcePath));

        try
        {
            var fileInfo = new FileInfo(sourcePath);

            if (fileInfo.Length > maxSizeInBytes)
            {
                return Result.Failure<bool, DomainError>(
                    ImageErrors.FileSizeTooLarge(Path.GetFileName(sourcePath), fileInfo.Length, maxSizeInBytes));
            }

            _logger.Debug("Tamaño válido: {Size} bytes", fileInfo.Length);
            return Result.Success<bool, DomainError>(true);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al validar tamaño de imagen");
            return Result.Failure<bool, DomainError>(ImageErrors.ValidationError(ex.Message));
        }
    }

    public Result<bool, DomainError> ValidateImageDimensions(string sourcePath, int maxWidth = DefaultMaxWidth, int maxHeight = DefaultMaxHeight)
    {
        _logger.Debug("Validando dimensiones de imagen: {Path}", sourcePath);

        if (!File.Exists(sourcePath))
            return Result.Failure<bool, DomainError>(ImageErrors.NotFound(sourcePath));

        try
        {
            var dims = GetImageDimensions(sourcePath);
            // If dimensions cannot be determined (file too small, unknown format, etc.),
            // allow the image (lenient validation).
            if (dims.Width <= 0 || dims.Height <= 0)
            {
                _logger.Debug("Dimensiones no determinadas para {SourcePath}; se asumen válidas", sourcePath);
                return Result.Success<bool, DomainError>(true);
            }

            if (dims.Width > maxWidth || dims.Height > maxHeight)
            {
                return Result.Failure<bool, DomainError>(
                    ImageErrors.DimensionsTooLarge(
                        Path.GetFileName(sourcePath),
                        dims.Width, dims.Height,
                        maxWidth, maxHeight));
            }

            _logger.Debug("Dimensiones válidas: {Width}x{Height}", dims.Width, dims.Height);
            return Result.Success<bool, DomainError>(true);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al validar dimensiones de imagen");
            return Result.Failure<bool, DomainError>(ImageErrors.ValidationError(ex.Message));
        }
    }

    /// <summary>
    /// Crea una previsualización de la imagen antes de guardarla.
    /// Retorna un BitmapImage que puede usarse en WPF.
    /// </summary>
    public Result<BitmapImage, DomainError> CreatePreview(string sourcePath, int maxWidth = 300, int maxHeight = 300)
    {
        if (!File.Exists(sourcePath))
            return Result.Failure<BitmapImage, DomainError>(ImageErrors.NotFound(sourcePath));

        try
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(sourcePath, UriKind.Absolute);
            bitmap.DecodePixelWidth = maxWidth;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();

            return Result.Success<BitmapImage, DomainError>(bitmap);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al crear previsualización de {SourcePath}", sourcePath);
            return Result.Failure<BitmapImage, DomainError>(
                ImageErrors.SaveError($"No se pudo crear la previsualización: {ex.Message}"));
        }
    }

    /// <summary>
    /// Sanitiza el nombre de un archivo eliminando caracteres peligrosos.
    /// </summary>
    internal string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = string.Join("_", fileName.Split(invalidChars));

        if (sanitized.Length > 200)
            sanitized = sanitized.Substring(0, 200);

        sanitized = sanitized
            .Replace(" ", "_")
            .Replace("..", ".")
            .Replace("--", "-");

        return sanitized;
    }

    /// <summary>
    /// Valida el tipo MIME real del archivo leyendo sus magic numbers.
    /// </summary>
    private Result<string, DomainError> ValidateMimeType(string sourcePath)
    {
        if (!File.Exists(sourcePath))
            return Result.Failure<string, DomainError>(ImageErrors.NotFound(sourcePath));

        try
        {
            using var fs = new FileStream(sourcePath, FileMode.Open, FileAccess.Read);
            var buffer = new byte[8];
            var bytesRead = 0;
            while (bytesRead < buffer.Length)
            {
                var read = fs.Read(buffer, bytesRead, buffer.Length - bytesRead);
                if (read == 0) break;
                bytesRead += read;
            }

            var mimeType = GetMimeTypeFromBytes(buffer);

            var allowedMimeTypes = new[]
            {
                "image/png",
                "image/jpeg",
                "image/bmp",
                "image/gif"
            };

            if (!allowedMimeTypes.Contains(mimeType))
            {
                return Result.Failure<string, DomainError>(
                    ImageErrors.InvalidFormat($"Tipo MIME no permitido: {mimeType}. Solo se permiten PNG, JPEG, BMP y GIF."));
            }

            return Result.Success<string, DomainError>(mimeType);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al validar tipo MIME de {SourcePath}", sourcePath);
            return Result.Failure<string, DomainError>(
                ImageErrors.SaveError($"Error al validar el tipo de archivo: {ex.Message}"));
        }
    }

    /// <summary>
    /// Determina el tipo MIME a partir de los magic numbers del archivo.
    /// </summary>
    private static string GetMimeTypeFromBytes(byte[] buffer)
    {
        // PNG: 89 50 4E 47
        if (buffer.Length >= 4 && buffer[0] == 0x89 && buffer[1] == 0x50 &&
            buffer[2] == 0x4E && buffer[3] == 0x47)
            return "image/png";

        // JPEG: FF D8 FF
        if (buffer.Length >= 3 && buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF)
            return "image/jpeg";

        // BMP: 42 4D
        if (buffer.Length >= 2 && buffer[0] == 0x42 && buffer[1] == 0x4D)
            return "image/bmp";

        // GIF: 47 49 46 38
        if (buffer.Length >= 4 && buffer[0] == 0x47 && buffer[1] == 0x49 &&
            buffer[2] == 0x46 && buffer[3] == 0x38)
            return "image/gif";

        return "unknown";
    }

    /// <summary>
    /// Reads image dimensions from the file header without external dependencies.
    /// Supports PNG, BMP, GIF, JPEG/JPG formats.
    /// Returns (0, 0) if the format is not recognized or the file is too small.
    /// </summary>
    private static (int Width, int Height) GetImageDimensions(string path)
    {
        var ext = Path.GetExtension(path).ToLower();
        using var stream = File.OpenRead(path);

        return ext switch
        {
            ".png"          => ReadPngDimensions(stream),
            ".bmp"          => ReadBmpDimensions(stream),
            ".gif"          => ReadGifDimensions(stream),
            ".jpg" or ".jpeg" => ReadJpegDimensions(stream),
            _               => (0, 0)
        };
    }

    private static (int Width, int Height) ReadPngDimensions(Stream stream)
    {
        if (stream.Length < 24) return (0, 0);
        stream.Seek(16, SeekOrigin.Begin);
        var buf = new byte[8];
        stream.ReadExactly(buf, 0, 8);
        int width  = (buf[0] << 24) | (buf[1] << 16) | (buf[2] << 8) | buf[3];
        int height = (buf[4] << 24) | (buf[5] << 16) | (buf[6] << 8) | buf[7];
        return (width, height);
    }

    private static (int Width, int Height) ReadBmpDimensions(Stream stream)
    {
        if (stream.Length < 26) return (0, 0);
        stream.Seek(18, SeekOrigin.Begin);
        var buf = new byte[8];
        stream.ReadExactly(buf, 0, 8);
        int width  = BitConverter.ToInt32(buf, 0);
        int height = Math.Abs(BitConverter.ToInt32(buf, 4));
        return (width, height);
    }

    private static (int Width, int Height) ReadGifDimensions(Stream stream)
    {
        if (stream.Length < 10) return (0, 0);
        stream.Seek(6, SeekOrigin.Begin);
        var buf = new byte[4];
        stream.ReadExactly(buf, 0, 4);
        int width  = buf[0] | (buf[1] << 8);
        int height = buf[2] | (buf[3] << 8);
        return (width, height);
    }

    private static (int Width, int Height) ReadJpegDimensions(Stream stream)
    {
        if (stream.Length < 11) return (0, 0);
        stream.Seek(2, SeekOrigin.Begin);
        var buf = new byte[4];

        while (stream.Position < stream.Length - 9)
        {
            if (stream.ReadByte() != 0xFF) break;
            int marker = stream.ReadByte();

            if (marker is 0xD9 or 0xDA) break; // EOI or SOS

            stream.ReadExactly(buf, 0, 2);
            int segLen = (buf[0] << 8) | buf[1];

            // SOF markers that contain image dimensions
            if ((marker >= 0xC0 && marker <= 0xC3) ||
                (marker >= 0xC5 && marker <= 0xC7) ||
                (marker >= 0xC9 && marker <= 0xCB) ||
                (marker >= 0xCD && marker <= 0xCF))
            {
                stream.Seek(1, SeekOrigin.Current); // Skip precision byte
                stream.ReadExactly(buf, 0, 4);
                int height = (buf[0] << 8) | buf[1];
                int width  = (buf[2] << 8) | buf[3];
                return (width, height);
            }

            stream.Seek(segLen - 2, SeekOrigin.Current);
        }

        return (0, 0);
    }
}
