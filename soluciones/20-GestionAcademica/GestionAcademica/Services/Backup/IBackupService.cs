using CSharpFunctionalExtensions;
using GestionAcademica.Errors.Backup;
using GestionAcademica.Errors.Common;
using GestionAcademica.Models.Personas;

namespace GestionAcademica.Services.Backup;

/// <summary>
/// Define el contrato para la gestión de backups del sistema.
/// </summary>
public interface IBackupService
{
    /// <summary>
    /// Realiza un backup de las personas en un archivo ZIP.
    /// </summary>
    /// <param name="personas">Lista de personas a respaldar.</param>
    /// <param name="customBackupDirectory">Directorio personalizado para guardar el backup (opcional).</param>
    /// <returns>
    /// Result con la ruta del archivo ZIP creado o error:
    /// <see cref="BackupErrors.CreationError(string)"/>,
    /// <see cref="BackupErrors.DirectoryError(string)"/> o
    /// <see cref="BackupErrors.InvalidBackupFile(string)"/>.
    /// </returns>
    Result<string, DomainError> RealizarBackup(
        IEnumerable<Persona> personas, 
        string? customBackupDirectory = null);

    /// <summary>
    /// Restaura un backup desde un archivo ZIP.
    /// </summary>
    /// <param name="archivoBackup">Ruta del archivo ZIP.</param>
    /// <param name="customImagesDirectory">Directorio personalizado para restaurar imágenes (opcional).</param>
    /// <returns>
    /// Result con la lista de personas restauradas o error:
    /// <see cref="BackupErrors.FileNotFound(string)"/>,
    /// <see cref="BackupErrors.InvalidBackupFile(string)"/> o
    /// <see cref="BackupErrors.RestorationError(string)"/>.
    /// </returns>
    Result<IEnumerable<Persona>, DomainError> RestaurarBackup(
        string archivoBackup,
        string? customImagesDirectory = null);

    /// <summary>
    /// Lista los archivos de backup disponibles en el directorio.
    /// </summary>
    /// <param name="customBackupDirectory">Directorio personalizado (opcional).</param>
    /// <returns>Enumerable con las rutas de los archivos de backup.</returns>
    IEnumerable<string> ListarBackups(string? customBackupDirectory = null);

    /// <summary>
    /// Realiza un backup completo del sistema incluyendo datos y metadatos.
    /// </summary>
    /// <param name="personas">Lista de personas a respaldar.</param>
    /// <returns>
    /// Result con la ruta del archivo ZIP o error:
    /// <see cref="BackupErrors.CreationError(string)"/>,
    /// <see cref="BackupErrors.DirectoryError(string)"/> o
    /// <see cref="BackupErrors.InvalidBackupFile(string)"/>.
    /// </returns>
    Result<string, DomainError> RealizarBackupSistema(IEnumerable<Persona> personas);

    /// <summary>
    /// Restaura un backup completo del sistema.
    /// </summary>
    /// <param name="archivoBackup">Ruta del archivo ZIP.</param>
    /// <param name="createCallback">Función para crear cada persona en el repositorio.</param>
    /// <param name="customImagesDirectory">Directorio personalizado para imágenes (opcional).</param>
    /// <returns>
    /// Result con el número de personas restauradas o error:
    /// <see cref="BackupErrors.FileNotFound(string)"/>,
    /// <see cref="BackupErrors.InvalidBackupFile(string)"/> o
    /// <see cref="BackupErrors.RestorationError(string)"/>.
    /// </returns>
    Result<int, DomainError> RestaurarBackupSistema(
        string archivoBackup, 
        Func<Persona, Result<Persona, DomainError>> createCallback,
        string? customImagesDirectory = null);
}
