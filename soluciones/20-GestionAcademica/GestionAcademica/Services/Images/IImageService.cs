using System.Windows.Media.Imaging;
using CSharpFunctionalExtensions;
using GestionAcademica.Errors.Common;

namespace GestionAcademica.Services.Images;

public interface IImageService
{
    /// <summary>
    /// Guarda una imagen desde una ruta de origen al directorio de imágenes con un nombre único.
    /// Valida extensión, tipo MIME, tamaño (máx. 5 MB) y dimensiones (máx. 4096×4096 px).
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
    /// Valida que el tamaño del archivo no exceda el límite (5 MB por defecto).
    /// </summary>
    /// <param name="sourcePath">Ruta del archivo de imagen.</param>
    /// <param name="maxSizeInBytes">Tamaño máximo en bytes (por defecto 5 MB).</param>
    /// <returns>Success si el archivo existe y su tamaño es menor o igual al máximo permitido; Failure con el error correspondiente en caso contrario.</returns>
    Result<bool, DomainError> ValidateImageSize(string sourcePath, long maxSizeInBytes = 5_242_880);

    /// <summary>
    /// Valida que las dimensiones de la imagen no excedan el límite (4096×4096 px por defecto).
    /// </summary>
    /// <remarks>
    /// La validación se implementa leyendo la cabecera del archivo (PNG, BMP, GIF, JPEG).
    /// Si las dimensiones no pueden determinarse (archivo truncado, formato desconocido, etc.),
    /// el método retorna Success (comportamiento leniente).
    /// </remarks>
    /// <param name="sourcePath">Ruta del archivo de imagen.</param>
    /// <param name="maxWidth">Ancho máximo en píxeles (por defecto 4096).</param>
    /// <param name="maxHeight">Alto máximo en píxeles (por defecto 4096).</param>
    /// <returns>Success si las dimensiones son válidas o no se pueden determinar; Failure con el error correspondiente si superan los límites.</returns>
    Result<bool, DomainError> ValidateImageDimensions(string sourcePath, int maxWidth = 4096, int maxHeight = 4096);

    /// <summary>
    /// Crea una previsualización de la imagen antes de guardarla.
    /// Retorna un BitmapImage que puede usarse en WPF.
    /// </summary>
    /// <param name="sourcePath">Ruta completa del archivo de origen.</param>
    /// <param name="maxWidth">Ancho máximo de la previsualización en píxeles (por defecto 300).</param>
    /// <param name="maxHeight">Alto máximo de la previsualización en píxeles (por defecto 300).</param>
    /// <returns>Success con el BitmapImage si la imagen se puede cargar; Failure con el error correspondiente en caso contrario.</returns>
    Result<BitmapImage, DomainError> CreatePreview(string sourcePath, int maxWidth = 300, int maxHeight = 300);
}
