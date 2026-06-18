using System.IO;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using GestionAcademica.Config;
using GestionAcademica.Errors.Common;
using GestionAcademica.Errors.Images;
using GestionAcademica.Services.Images;
using Serilog;

namespace GestionAcademica.Blazor.Services;

public class BlazorImageService : IImageService
{
    private readonly Serilog.ILogger _logger = Log.ForContext<BlazorImageService>();
    private readonly string _imagesDirectory;
    private readonly string[] _allowedExtensions;

    public BlazorImageService()
    {
        _imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
        _allowedExtensions = AppConfig.AllowedImageExtensions;
        if (!Directory.Exists(_imagesDirectory))
            Directory.CreateDirectory(_imagesDirectory);
    }

    public async Task<Result<string, DomainError>> SaveImageAsync(string sourcePath)
    {
        if (!File.Exists(sourcePath))
            return Result.Failure<string, DomainError>(ImageErrors.NotFound(sourcePath));

        if (!await IsValidImageAsync(sourcePath))
            return Result.Failure<string, DomainError>(ImageErrors.InvalidFormat(Path.GetExtension(sourcePath)));

        var sizeResult = await ValidateImageSizeAsync(sourcePath);
        if (sizeResult.IsFailure)
            return Result.Failure<string, DomainError>(sizeResult.Error);

        var mimeResult = ValidateMimeType(sourcePath);
        if (mimeResult.IsFailure)
            return Result.Failure<string, DomainError>(mimeResult.Error);

        var dimResult = await ValidateImageDimensionsAsync(sourcePath);
        if (dimResult.IsFailure)
            return Result.Failure<string, DomainError>(dimResult.Error);

        try
        {
            var extension = Path.GetExtension(sourcePath).ToLower();
            var fileName = SanitizeFileName($"{Guid.NewGuid()}{extension}");
            var dest = Path.Combine(_imagesDirectory, fileName);
            File.Copy(sourcePath, dest, true);
            _logger.Information("Imagen guardada: {FileName}", fileName);
            return Result.Success<string, DomainError>($"images/{fileName}");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error guardando imagen");
            return Result.Failure<string, DomainError>(ImageErrors.SaveError(ex.Message));
        }
    }

    public async Task<Result<bool, DomainError>> DeleteImageAsync(string fileName)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileName);
        if (!File.Exists(path))
            return Result.Success<bool, DomainError>(true);

        try
        {
            File.Delete(path);
            return Result.Success<bool, DomainError>(true);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool, DomainError>(ImageErrors.DeleteError(ex.Message));
        }
    }

    public async Task<Result<bool, DomainError>> UpdateImageAsync(string sourcePath, string existingFileName)
    {
        if (!File.Exists(sourcePath))
            return Result.Failure<bool, DomainError>(ImageErrors.NotFound(sourcePath));

        if (!await IsValidImageAsync(sourcePath))
            return Result.Failure<bool, DomainError>(ImageErrors.InvalidFormat(Path.GetExtension(sourcePath)));

        var sizeResult = await ValidateImageSizeAsync(sourcePath);
        if (sizeResult.IsFailure)
            return Result.Failure<bool, DomainError>(sizeResult.Error);

        var mimeResult = ValidateMimeType(sourcePath);
        if (mimeResult.IsFailure)
            return Result.Failure<bool, DomainError>(mimeResult.Error);

        var dimResult = await ValidateImageDimensionsAsync(sourcePath);
        if (dimResult.IsFailure)
            return Result.Failure<bool, DomainError>(dimResult.Error);

        var dest = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingFileName);
        try
        {
            File.Copy(sourcePath, dest, true);
            return Result.Success<bool, DomainError>(true);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool, DomainError>(ImageErrors.SaveError(ex.Message));
        }
    }

    public Task<bool> IsValidImageAsync(string sourcePath)
    {
        var ext = Path.GetExtension(sourcePath).ToLower();
        return Task.FromResult(_allowedExtensions.Contains(ext));
    }

    public Task<Result<bool, DomainError>> ValidateImageSizeAsync(string sourcePath, long maxSizeInBytes = 5_242_880)
    {
        if (!File.Exists(sourcePath))
            return Task.FromResult(Result.Failure<bool, DomainError>(ImageErrors.NotFound(sourcePath)));

        var info = new FileInfo(sourcePath);
        if (info.Length > maxSizeInBytes)
            return Task.FromResult(Result.Failure<bool, DomainError>(
                ImageErrors.FileSizeTooLarge(Path.GetFileName(sourcePath), info.Length, maxSizeInBytes)));

        return Task.FromResult(Result.Success<bool, DomainError>(true));
    }

    public Task<Result<bool, DomainError>> ValidateImageDimensionsAsync(string sourcePath, int maxWidth = 4096, int maxHeight = 4096)
    {
        if (!File.Exists(sourcePath))
            return Task.FromResult(Result.Failure<bool, DomainError>(ImageErrors.NotFound(sourcePath)));

        try
        {
            var dims = GetImageDimensions(sourcePath);
            if (dims.Width <= 0 || dims.Height <= 0)
                return Task.FromResult(Result.Success<bool, DomainError>(true));

            if (dims.Width > maxWidth || dims.Height > maxHeight)
                return Task.FromResult(Result.Failure<bool, DomainError>(
                    ImageErrors.DimensionsTooLarge(
                        Path.GetFileName(sourcePath),
                        dims.Width, dims.Height,
                        maxWidth, maxHeight)));

            return Task.FromResult(Result.Success<bool, DomainError>(true));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error validando dimensiones");
            return Task.FromResult(Result.Failure<bool, DomainError>(ImageErrors.ValidationError(ex.Message)));
        }
    }

    public Task<Result<string, DomainError>> CreatePreviewAsync(string sourcePath, int maxWidth = 300, int maxHeight = 300)
    {
        if (!File.Exists(sourcePath))
            return Task.FromResult(Result.Failure<string, DomainError>(ImageErrors.NotFound(sourcePath)));
        return Task.FromResult(Result.Success<string, DomainError>(sourcePath));
    }

    private static string SanitizeFileName(string fileName)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var sanitized = string.Join("_", fileName.Split(invalid));
        if (sanitized.Length > 200) sanitized = sanitized[..200];
        return sanitized.Replace(" ", "_").Replace("..", ".").Replace("--", "-");
    }

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
            var allowedMimeTypes = new[] { "image/png", "image/jpeg", "image/bmp", "image/gif" };

            if (!allowedMimeTypes.Contains(mimeType))
                return Result.Failure<string, DomainError>(
                    ImageErrors.InvalidFormat($"Tipo MIME no permitido: {mimeType}. Solo se permiten PNG, JPEG, BMP y GIF."));

            return Result.Success<string, DomainError>(mimeType);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error validando MIME");
            return Result.Failure<string, DomainError>(ImageErrors.SaveError($"Error al validar el tipo de archivo: {ex.Message}"));
        }
    }

    private static string GetMimeTypeFromBytes(byte[] buffer)
    {
        if (buffer.Length >= 4 && buffer[0] == 0x89 && buffer[1] == 0x50 &&
            buffer[2] == 0x4E && buffer[3] == 0x47)
            return "image/png";

        if (buffer.Length >= 3 && buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF)
            return "image/jpeg";

        if (buffer.Length >= 2 && buffer[0] == 0x42 && buffer[1] == 0x4D)
            return "image/bmp";

        if (buffer.Length >= 4 && buffer[0] == 0x47 && buffer[1] == 0x49 &&
            buffer[2] == 0x46 && buffer[3] == 0x38)
            return "image/gif";

        return "unknown";
    }

    private static (int Width, int Height) GetImageDimensions(string path)
    {
        var ext = Path.GetExtension(path).ToLower();
        using var stream = File.OpenRead(path);

        return ext switch
        {
            ".png" => ReadPngDimensions(stream),
            ".bmp" => ReadBmpDimensions(stream),
            ".gif" => ReadGifDimensions(stream),
            ".jpg" or ".jpeg" => ReadJpegDimensions(stream),
            _ => (0, 0)
        };
    }

    private static (int Width, int Height) ReadPngDimensions(Stream stream)
    {
        if (stream.Length < 24) return (0, 0);
        stream.Seek(16, SeekOrigin.Begin);
        var buf = new byte[8];
        stream.ReadExactly(buf, 0, 8);
        var width = (buf[0] << 24) | (buf[1] << 16) | (buf[2] << 8) | buf[3];
        var height = (buf[4] << 24) | (buf[5] << 16) | (buf[6] << 8) | buf[7];
        return (width, height);
    }

    private static (int Width, int Height) ReadBmpDimensions(Stream stream)
    {
        if (stream.Length < 26) return (0, 0);
        stream.Seek(18, SeekOrigin.Begin);
        var buf = new byte[8];
        stream.ReadExactly(buf, 0, 8);
        var width = BitConverter.ToInt32(buf, 0);
        var height = Math.Abs(BitConverter.ToInt32(buf, 4));
        return (width, height);
    }

    private static (int Width, int Height) ReadGifDimensions(Stream stream)
    {
        if (stream.Length < 10) return (0, 0);
        stream.Seek(6, SeekOrigin.Begin);
        var buf = new byte[4];
        stream.ReadExactly(buf, 0, 4);
        var width = buf[0] | (buf[1] << 8);
        var height = buf[2] | (buf[3] << 8);
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
            var marker = stream.ReadByte();

            if (marker is 0xD9 or 0xDA) break;

            stream.ReadExactly(buf, 0, 2);
            var segLen = (buf[0] << 8) | buf[1];

            if ((marker >= 0xC0 && marker <= 0xC3) ||
                (marker >= 0xC5 && marker <= 0xC7) ||
                (marker >= 0xC9 && marker <= 0xCB) ||
                (marker >= 0xCD && marker <= 0xCF))
            {
                stream.Seek(1, SeekOrigin.Current);
                stream.ReadExactly(buf, 0, 4);
                var height = (buf[0] << 8) | buf[1];
                var width = (buf[2] << 8) | buf[3];
                return (width, height);
            }

            stream.Seek(segLen - 2, SeekOrigin.Current);
        }

        return (0, 0);
    }
}
