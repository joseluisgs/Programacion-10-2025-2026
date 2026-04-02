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
}
