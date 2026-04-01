using CSharpFunctionalExtensions;
using GestionAcademica.Errors.Common;

namespace GestionAcademica.Services.Images;

public interface IImageService
{
    /// <summary>
    /// Guarda una imagen desde una ruta de origen al directorio de imágenes con un nombre único.
    /// Valida extensión, tamaño (máx. 5 MB) y dimensiones (máx. 4096×4096 px).
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
    /// Valida extensión, tamaño (máx. 5 MB) y dimensiones (máx. 4096×4096 px).
    /// </summary>
    /// <param name="sourcePath">Ruta de la nueva imagen.</param>
    /// <param name="existingFileName">Nombre del archivo actual en el directorio de imágenes.</param>
    Result<bool, DomainError> UpdateImage(string sourcePath, string existingFileName);

    /// <summary>
    /// Verifica si un archivo tiene un formato de imagen permitido.
    /// </summary>
    bool IsValidImage(string sourcePath);

    /// <summary>
    /// Valida el tamaño del archivo de imagen.
    /// </summary>
    /// <param name="sourcePath">Ruta del archivo de imagen.</param>
    /// <param name="maxSizeInBytes">Tamaño máximo en bytes (por defecto 5 MB).</param>
    /// <returns>True si el archivo existe y su tamaño es menor o igual al máximo permitido.</returns>
    bool ValidateImageSize(string sourcePath, long maxSizeInBytes = 5_242_880);

    /// <summary>
    /// Valida las dimensiones de la imagen.
    /// </summary>
    /// <remarks>
    /// La validación se implementa leyendo la cabecera del archivo (PNG, BMP, GIF, JPEG).
    /// Si las dimensiones no pueden determinarse (archivo truncado, formato desconocido, etc.),
    /// el método retorna <c>true</c> (comportamiento leniente). En ese caso corresponde al
    /// llamador decidir si procede con la imagen.
    /// </remarks>
    /// <param name="sourcePath">Ruta del archivo de imagen.</param>
    /// <param name="maxWidth">Ancho máximo en píxeles (por defecto 4096).</param>
    /// <param name="maxHeight">Alto máximo en píxeles (por defecto 4096).</param>
    /// <returns>
    /// <c>true</c> si las dimensiones son válidas o no se pueden determinar; 
    /// <c>false</c> si superan los límites indicados.
    /// </returns>
    bool ValidateImageDimensions(string sourcePath, int maxWidth = 4096, int maxHeight = 4096);
}
