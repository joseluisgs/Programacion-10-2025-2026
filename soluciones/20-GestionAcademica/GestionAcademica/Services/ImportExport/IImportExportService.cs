using CSharpFunctionalExtensions;
using GestionAcademica.Errors.Common;
using GestionAcademica.Models.Personas;

namespace GestionAcademica.Services.ImportExport;

/// <summary>
/// Define el contrato para importar y exportar datos del sistema.
/// </summary>
public interface IImportExportService
{
    /// <summary>
    /// Exporta personas a un archivo en el formato configurado.
    /// </summary>
    /// <param name="personas">Enumerable de personas a exportar.</param>
    /// <param name="path">Ruta del archivo de destino.</param>
    /// <returns>
    /// Result con el número de personas exportadas o error:
    /// <see cref="Errors.Storage.StorageErrors.InvalidFormat(string)"/> o
    /// <see cref="Errors.Storage.StorageErrors.WriteError(string)"/>.
    /// </returns>
    Result<int, DomainError> ExportarDatos(IEnumerable<Persona> personas, string path);

    /// <summary>
    /// Importa personas desde un archivo.
    /// </summary>
    /// <param name="path">Ruta del archivo a importar.</param>
    /// <returns>
    /// Result con la lista de personas importadas o error:
    /// <see cref="Errors.Storage.StorageErrors.FileNotFound(string)"/>,
    /// <see cref="Errors.Storage.StorageErrors.InvalidFormat(string)"/> o
    /// <see cref="Errors.Storage.StorageErrors.ReadError(string)"/>.
    /// </returns>
    Result<IEnumerable<Persona>, DomainError> ImportarDatos(string path);

    /// <summary>
    /// Exporta personas usando el directorio configurado en el sistema.
    /// </summary>
    /// <param name="personas">Enumerable de personas a exportar.</param>
    /// <returns>
    /// Result con el número de personas exportadas o error:
    /// <see cref="Errors.Storage.StorageErrors.InvalidFormat(string)"/> o
    /// <see cref="Errors.Storage.StorageErrors.WriteError(string)"/>.
    /// </returns>
    Result<int, DomainError> ExportarDatosSistema(IEnumerable<Persona> personas);

    /// <summary>
    /// Importa personas desde el directorio configurado en el sistema.
    /// </summary>
    /// <param name="path">Ruta del archivo a importar.</param>
    /// <returns>
    /// Result con la lista de personas importadas o error:
    /// <see cref="Errors.Storage.StorageErrors.FileNotFound(string)"/>,
    /// <see cref="Errors.Storage.StorageErrors.InvalidFormat(string)"/> o
    /// <see cref="Errors.Storage.StorageErrors.ReadError(string)"/>.
    /// </returns>
    Result<IEnumerable<Persona>, DomainError> ImportarDatosSistema(string path);
}
