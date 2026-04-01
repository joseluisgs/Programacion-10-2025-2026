using System.IO;
using CSharpFunctionalExtensions;
using GestionAcademica.Errors.Common;
using GestionAcademica.Errors.Images;
using Serilog;

namespace GestionAcademica.Services.Images;

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

        if (!ValidateImageSize(sourcePath))
        {
            var fileInfo = new FileInfo(sourcePath);
            return Result.Failure<string, DomainError>(
                ImageErrors.FileSizeTooLarge(Path.GetFileName(sourcePath), fileInfo.Length, DefaultMaxSizeInBytes));
        }

        var dims = GetImageDimensions(sourcePath);
        if (dims.Width > 0 && dims.Height > 0 && (dims.Width > DefaultMaxWidth || dims.Height > DefaultMaxHeight))
        {
            return Result.Failure<string, DomainError>(
                ImageErrors.DimensionsTooLarge(Path.GetFileName(sourcePath), dims.Width, dims.Height, DefaultMaxWidth, DefaultMaxHeight));
        }

        try
        {
            var extension = Path.GetExtension(sourcePath).ToLower();
            var newFileName = $"{Guid.NewGuid()}{extension}";
            var destinationPath = Path.Combine(_imagesDirectory, newFileName);

            File.Copy(sourcePath, destinationPath, true);
            _logger.Information("Imagen guardada exitosamente como {NewFileName}", newFileName);
            
            return Result.Success<string, DomainError>(newFileName);
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

        if (!ValidateImageSize(sourcePath))
        {
            var fileInfo = new FileInfo(sourcePath);
            return Result.Failure<bool, DomainError>(
                ImageErrors.FileSizeTooLarge(Path.GetFileName(sourcePath), fileInfo.Length, DefaultMaxSizeInBytes));
        }

        var dims = GetImageDimensions(sourcePath);
        if (dims.Width > 0 && dims.Height > 0 && (dims.Width > DefaultMaxWidth || dims.Height > DefaultMaxHeight))
        {
            return Result.Failure<bool, DomainError>(
                ImageErrors.DimensionsTooLarge(Path.GetFileName(sourcePath), dims.Width, dims.Height, DefaultMaxWidth, DefaultMaxHeight));
        }

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

    public bool ValidateImageSize(string sourcePath, long maxSizeInBytes = DefaultMaxSizeInBytes)
    {
        if (!File.Exists(sourcePath))
            return false;

        return new FileInfo(sourcePath).Length <= maxSizeInBytes;
    }

    public bool ValidateImageDimensions(string sourcePath, int maxWidth = DefaultMaxWidth, int maxHeight = DefaultMaxHeight)
    {
        if (!File.Exists(sourcePath))
            return false;

        try
        {
            var dims = GetImageDimensions(sourcePath);
            // If dimensions cannot be determined (file too small, unknown format, etc.), 
            // allow the image (lenient validation). The caller must decide whether to 
            // proceed with an image whose dimensions could not be verified.
            if (dims.Width <= 0 || dims.Height <= 0)
                return true;

            return dims.Width <= maxWidth && dims.Height <= maxHeight;
        }
        catch (Exception ex)
        {
            // If the header cannot be read, log the issue and allow the image (lenient)
            _logger.Warning(ex, "No se pudieron leer las dimensiones de {SourcePath}; se asume que son válidas", sourcePath);
            return true;
        }
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
