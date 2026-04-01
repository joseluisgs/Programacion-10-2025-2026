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
}
