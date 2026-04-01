using CSharpFunctionalExtensions;
using GestionAcademica.Errors.Backup;
using GestionAcademica.Errors.Common;
using GestionAcademica.Models.Personas;

namespace GestionAcademica.Services.Backup;

public interface IBackupService
{
    /// <summary>
    /// Realiza un backup de las personas. Por defecto usa el directorio configurado.
    /// </summary>
    /// <param name="personas">Lista de personas a respaldar</param>
    /// <param name="customBackupDirectory">Directorio custom para el backup (opcional)</param>
    /// <returns>Ruta del archivo ZIP creado</returns>
    Result<string, DomainError> RealizarBackup(
        IEnumerable<Persona> personas, 
        string? customBackupDirectory = null);

    /// <summary>
    /// Restaura un backup. Por defecto restaura imágenes en el directorio configurado.
    /// </summary>
    /// <param name="archivoBackup">Ruta del archivo ZIP</param>
    /// <param name="customImagesDirectory">Directorio custom para restaurar imágenes (opcional)</param>
    /// <returns>Lista de personas restauradas</returns>
    Result<IEnumerable<Persona>, DomainError> RestaurarBackup(
        string archivoBackup,
        string? customImagesDirectory = null);

    IEnumerable<string> ListarBackups(string? customBackupDirectory = null);

    Result<string, DomainError> RealizarBackupSistema(IEnumerable<Persona> personas);
    Result<int, DomainError> RestaurarBackupSistema(
        string archivoBackup, 
        Func<Persona, Result<Persona, DomainError>> createCallback,
        string? customImagesDirectory = null);
}
