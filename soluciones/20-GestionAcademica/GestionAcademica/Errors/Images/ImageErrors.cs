using GestionAcademica.Errors.Common;

namespace GestionAcademica.Errors.Images;

/// <summary>
/// Contenedor de errores específicos para la gestión de Imágenes.
/// </summary>
public abstract record ImageError(string Message) : DomainError(Message)
{
    public sealed record NotFound(string FileName)
        : ImageError($"No se ha encontrado la imagen: {FileName}");

    public sealed record InvalidFormat(string Extension)
        : ImageError($"El formato de imagen '{Extension}' no está permitido.");

    public sealed record SaveError(string Details)
        : ImageError($"Error al guardar la imagen: {Details}");

    public sealed record DeleteError(string Details)
        : ImageError($"Error al eliminar la imagen: {Details}");

    public sealed record AccessError(string Details)
        : ImageError($"Error de acceso al directorio de imágenes: {Details}");

    public sealed record FileSizeTooLarge(string FileName, long ActualSize, long MaxSize)
        : ImageError($"El archivo {FileName} ({ActualSize / 1024 / 1024}MB) excede el tamaño máximo permitido ({MaxSize / 1024 / 1024}MB)");

    public sealed record DimensionsTooLarge(string FileName, int Width, int Height, int MaxWidth, int MaxHeight)
        : ImageError($"La imagen {FileName} ({Width}x{Height}) excede las dimensiones máximas permitidas ({MaxWidth}x{MaxHeight})");

    public sealed record ValidationError(string Details)
        : ImageError($"Error en la validación de la imagen: {Details}");
}

/// <summary>
/// Factory para crear errores de dominio de Imágenes.
/// </summary>
public static class ImageErrors
{
    public static DomainError NotFound(string fileName) => new ImageError.NotFound(fileName);
    public static DomainError InvalidFormat(string extension) => new ImageError.InvalidFormat(extension);
    public static DomainError SaveError(string details) => new ImageError.SaveError(details);
    public static DomainError DeleteError(string details) => new ImageError.DeleteError(details);
    public static DomainError AccessError(string details) => new ImageError.AccessError(details);
    public static DomainError FileSizeTooLarge(string fileName, long actualSize, long maxSize) => new ImageError.FileSizeTooLarge(fileName, actualSize, maxSize);
    public static DomainError DimensionsTooLarge(string fileName, int width, int height, int maxWidth, int maxHeight) => new ImageError.DimensionsTooLarge(fileName, width, height, maxWidth, maxHeight);
    public static DomainError ValidationError(string details) => new ImageError.ValidationError(details);
}
