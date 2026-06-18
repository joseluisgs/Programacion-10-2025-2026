using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using GestionAcademica.Dto.Personas;
using GestionAcademica.Errors.Common;
using GestionAcademica.Errors.Storage;
using GestionAcademica.Mappers.Personas;
using GestionAcademica.Models.Personas;
using Serilog;

namespace GestionAcademica.Services.ImportExport;

public class ImportExportService : IImportExportService
{
    private readonly ILogger _logger = Log.ForContext<ImportExportService>();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() },
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public async Task<Result<int, DomainError>> ExportarDatosAsync(IEnumerable<Persona> personas, string path)
    {
        _logger.Information("Exportando datos a {Path}", path);
        var lista = personas.ToList();
        var ext = Path.GetExtension(path)?.ToLowerInvariant();

        return ext switch
        {
            ".csv" => await ExportarCsvAsync(lista, path),
            _ => await ExportarJsonAsync(lista, path)
        };
    }

    public async Task<Result<IEnumerable<Persona>, DomainError>> ImportarDatosAsync(string path)
    {
        _logger.Information("Importando datos desde {Path}", path);
        var ext = Path.GetExtension(path)?.ToLowerInvariant();

        return ext switch
        {
            ".csv" => await ImportarCsvAsync(path),
            _ => await ImportarJsonAsync(path)
        };
    }

    public async Task<Result<int, DomainError>> ExportarDatosSistemaAsync(IEnumerable<Persona> personas) =>
        await ExportarDatosAsync(personas, string.Empty);

    public async Task<Result<IEnumerable<Persona>, DomainError>> ImportarDatosSistemaAsync(string path) =>
        await ImportarDatosAsync(path);

    private async Task<Result<int, DomainError>> ExportarJsonAsync(List<Persona> lista, string path)
    {
        try
        {
            var dtos = lista.Select(p => p.ToDto()).ToList();
            var json = JsonSerializer.Serialize(dtos, JsonOptions);
            await File.WriteAllTextAsync(path, json, new UTF8Encoding(false));
            return Result.Success<int, DomainError>(lista.Count);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al exportar JSON a '{Path}'", path);
            return Result.Failure<int, DomainError>(StorageErrors.WriteError(ex.Message));
        }
    }

    private async Task<Result<int, DomainError>> ExportarCsvAsync(List<Persona> lista, string path)
    {
        try
        {
            await using var writer = new StreamWriter(path, false, new UTF8Encoding(false));
            await writer.WriteLineAsync("Id;Dni;Nombre;Apellidos;FechaNacimiento;Email;Imagen;Tipo;Experiencia;Especialidad;Ciclo;Curso;Calificacion;CreatedAt;UpdatedAt;IsDeleted;DeletedAt");

            foreach (var p in lista)
            {
                var dto = p.ToDto();
                await writer.WriteLineAsync(
                    $"{dto.Id};{EscapeCsvField(dto.Dni)};{EscapeCsvField(dto.Nombre)};{EscapeCsvField(dto.Apellidos)};{dto.FechaNacimiento};{EscapeCsvField(dto.Email)};{EscapeCsvField(dto.Imagen ?? "")};{dto.Tipo};{EscapeCsvField(dto.Experiencia ?? "")};{EscapeCsvField(dto.Especialidad ?? "")};{dto.Ciclo};{EscapeCsvField(dto.Curso ?? "")};{EscapeCsvField(dto.Calificacion ?? "")};{dto.CreatedAt};{dto.UpdatedAt};{dto.IsDeleted};{EscapeCsvField(dto.DeletedAt ?? "")}");
            }

            return Result.Success<int, DomainError>(lista.Count);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al exportar CSV a '{Path}'", path);
            return Result.Failure<int, DomainError>(StorageErrors.WriteError(ex.Message));
        }
    }

    private async Task<Result<IEnumerable<Persona>, DomainError>> ImportarJsonAsync(string path)
    {
        if (!File.Exists(path))
            return Result.Failure<IEnumerable<Persona>, DomainError>(StorageErrors.FileNotFound(path));

        try
        {
            var json = await File.ReadAllTextAsync(path, Encoding.UTF8);
            var dtos = JsonSerializer.Deserialize<List<PersonaDto>>(json, JsonOptions);

            if (dtos is null)
                return Result.Failure<IEnumerable<Persona>, DomainError>(
                    StorageErrors.InvalidFormat("No se pudieron deserializar los DTOs."));

            return Result.Success<IEnumerable<Persona>, DomainError>(dtos.Select(dto => dto.ToModel()));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al importar JSON desde '{Path}'", path);
            return Result.Failure<IEnumerable<Persona>, DomainError>(StorageErrors.ReadError(ex.Message));
        }
    }

    private async Task<Result<IEnumerable<Persona>, DomainError>> ImportarCsvAsync(string path)
    {
        if (!File.Exists(path))
            return Result.Failure<IEnumerable<Persona>, DomainError>(StorageErrors.FileNotFound(path));

        try
        {
            var lines = File.ReadLines(path, Encoding.UTF8).ToList();
            var personas = lines
                .Skip(1)
                .Select(linea => linea.Split(';'))
                .Select(campos => new PersonaDto(
                    int.Parse(campos[0]), campos[1], campos[2], campos[3], campos[4], campos[5],
                    string.IsNullOrEmpty(campos[6]) ? null : campos[6],
                    campos[7], campos[8], campos[9], campos[10], campos[11], campos[12],
                    campos[13], campos[14],
                    bool.TryParse(campos[15], out var isDel) && isDel,
                    string.IsNullOrEmpty(campos[16]) ? null : campos[16]
                ).ToModel());

            return Result.Success<IEnumerable<Persona>, DomainError>(personas);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al importar CSV desde '{Path}'", path);
            return Result.Failure<IEnumerable<Persona>, DomainError>(StorageErrors.InvalidFormat(ex.Message));
        }
    }

    private static string EscapeCsvField(string field)
    {
        if (string.IsNullOrEmpty(field)) return "";
        if (field.Contains(';') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
            return $"\"{field.Replace("\"", "\"\"")}\"";
        return field;
    }
}
