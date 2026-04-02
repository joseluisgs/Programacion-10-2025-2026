using GestionAcademica.Errors.Common;

namespace GestionAcademica.Errors.Storage;

/// <summary>
/// Contenedor de errores específicos para el subdominio de Almacenamiento (Storage).
/// </summary>
public abstract record StorageError(string Message) : DomainError(Message)
{
    public sealed record FileNotFound(string FilePath)
        : StorageError($"No se ha encontrado el archivo en la ruta: {FilePath}");

    public sealed record InvalidFormat(string Details)
        : StorageError($"El formato del archivo es inválido o incompatible: {Details}");

    public sealed record WriteError(string Details)
        : StorageError($"Error al escribir en el almacenamiento: {Details}");

    public sealed record ReadError(string Details)
        : StorageError($"Error al leer del almacenamiento: {Details}");

    public sealed record AccessError(string Details)
        : StorageError($"Error de acceso al almacenamiento: {Details}");
}

/// <summary>
/// Factory para crear errores de dominio de Storage.
/// </summary>
public static class StorageErrors
{
    public static DomainError FileNotFound(string filePath) => new StorageError.FileNotFound(filePath);
    public static DomainError InvalidFormat(string details) => new StorageError.InvalidFormat(details);
    public static DomainError WriteError(string details) => new StorageError.WriteError(details);
    public static DomainError ReadError(string details) => new StorageError.ReadError(details);
    public static DomainError AccessError(string details) => new StorageError.AccessError(details);
}
