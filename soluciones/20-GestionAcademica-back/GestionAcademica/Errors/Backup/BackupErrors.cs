using GestionAcademica.Errors.Common;

namespace GestionAcademica.Errors.Backup;

/// <summary>
/// Contenedor de errores específicos para el dominio de Backup.
/// </summary>
public abstract record BackupError(string Message) : DomainError(Message)
{
    public sealed record FileNotFound(string FilePath)
        : BackupError($"No se ha encontrado el archivo de backup: {FilePath}");

    public sealed record InvalidBackupFile(string Details)
        : BackupError($"El archivo de backup es inválido o está corrupto: {Details}");

    public sealed record CreationError(string Details)
        : BackupError($"Error al crear el backup: {Details}");

    public sealed record RestorationError(string Details)
        : BackupError($"Error al restaurar el backup: {Details}");

    public sealed record DirectoryError(string Details)
        : BackupError($"Error con el directorio de backup: {Details}");
}

/// <summary>
/// Factory para crear errores de dominio de Backup.
/// </summary>
public static class BackupErrors
{
    public static DomainError FileNotFound(string filePath) => new BackupError.FileNotFound(filePath);
    public static DomainError InvalidBackupFile(string details) => new BackupError.InvalidBackupFile(details);
    public static DomainError CreationError(string details) => new BackupError.CreationError(details);
    public static DomainError RestorationError(string details) => new BackupError.RestorationError(details);
    public static DomainError DirectoryError(string details) => new BackupError.DirectoryError(details);
}
