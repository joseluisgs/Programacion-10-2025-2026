using CSharpFunctionalExtensions;
using GestionAcademica.Errors.Common;
using GestionAcademica.Models.Personas;

namespace GestionAcademica.Services.ImportExport;

public interface IImportExportService
{
    Result<int, DomainError> ExportarDatos(IEnumerable<Persona> personas, string path);
    Result<IEnumerable<Persona>, DomainError> ImportarDatos(string path);

    // Métodos de operación del sistema
    Result<int, DomainError> ExportarDatosSistema(IEnumerable<Persona> personas);
    Result<IEnumerable<Persona>, DomainError> ImportarDatosSistema(string path);
}
