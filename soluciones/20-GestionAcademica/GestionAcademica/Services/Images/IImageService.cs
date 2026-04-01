using CSharpFunctionalExtensions;
using GestionAcademica.Errors.Common;

namespace GestionAcademica.Services.Images;

public interface IImageService
{
    /// <summary>
    /// Guarda una imagen desde una ruta de origen al directorio de imágenes con un nombre único.
    /// </summary>
    /// <param name="sourcePath">Ruta completa del archivo de origen.</param>
    /// <returns>Nombre del archivo guardado (UUID + extensión).</returns>
    Result<string, DomainError> SaveImage(string sourcePath);

    /// <summary>
    /// Elimina una imagen del directorio de imágenes.
    /// </summary>
    /// <param name="fileName">Nombre del archivo a eliminar.</param>
    Result<bool, DomainError> DeleteImage(string fileName);

    /// <summary>
    /// Actualiza el contenido de una imagen existente manteniendo su nombre original en disco.
    /// </summary>
    /// <param name="sourcePath">Ruta de la nueva imagen.</param>
    /// <param name="existingFileName">Nombre del archivo actual en el directorio de imágenes.</param>
    Result<bool, DomainError> UpdateImage(string sourcePath, string existingFileName);

    /// <summary>
    /// Verifica si un archivo tiene un formato de imagen permitido.
    /// </summary>
    bool IsValidImage(string sourcePath);
}
