using CSharpFunctionalExtensions;
using GestionAcademica.Errors.Common;
using GestionAcademica.Models.Personas;
using GestionAcademica.Storage.Common;
using Serilog;

namespace GestionAcademica.Services.ImportExport;

public class ImportExportService(
    IStorage<Persona> storage
) : IImportExportService
{
    private readonly ILogger _logger = Log.ForContext<ImportExportService>();

    public Result<int, DomainError> ExportarDatos(IEnumerable<Persona> personas, string path)
    {
        _logger.Information("Exportando datos a {Path}", path);
        var lista = personas.ToList();
        return storage.Salvar(lista, path)
            .Map(_ => lista.Count);
    }

    public Result<IEnumerable<Persona>, DomainError> ImportarDatos(string path)
    {
        _logger.Information("Importando datos desde {Path}", path);
        return storage.Cargar(path);
    }

    public Result<int, DomainError> ExportarDatosSistema(IEnumerable<Persona> personas)
    {
        return ExportarDatos(personas, string.Empty);
    }

    public Result<IEnumerable<Persona>, DomainError> ImportarDatosSistema(string path)
    {
        return ImportarDatos(path);
    }
}
