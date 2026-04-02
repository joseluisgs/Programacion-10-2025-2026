using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;
using GestionAcademica.Config;
using GestionAcademica.Dto.Personas;
using GestionAcademica.Errors.Common;
using GestionAcademica.Errors.Storage;
using GestionAcademica.Mappers.Personas;
using GestionAcademica.Models.Personas;
using Serilog;

namespace GestionAcademica.Storage.Json;

public class AcademiaJsonStorage : IAcademiaJsonStorage
{
    private readonly ILogger _logger = Log.ForContext<AcademiaJsonStorage>();

    private readonly JsonSerializerOptions _options = new() {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() },
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public AcademiaJsonStorage()
    {
        _logger.Debug("Inicializando la clase AcademiaJsonStorage");
        InitStorage();
    }

    public Result<bool, DomainError> Salvar(IEnumerable<Persona> items, string path)
    {
        try
        {
            _logger.Debug("Guardando los items en el archivo '{path}'", path);
            using var stream = File.Create(path);
            var dtos = items.Select(p => p.ToDto()).ToList();
            JsonSerializer.Serialize(stream, dtos, _options);
            return Result.Success<bool, DomainError>(true);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al guardar los items en el archivo '{path}'", path);
            return Result.Failure<bool, DomainError>(StorageErrors.WriteError(ex.Message));
        }
    }

    public Result<IEnumerable<Persona>, DomainError> Cargar(string path)
    {
        _logger.Debug("Cargando los items del archivo '{path}'", path);
        
        if (!Path.Exists(path))
        {
            _logger.Warning("El archivo '{path}' no existe.", path);
            return Result.Failure<IEnumerable<Persona>, DomainError>(StorageErrors.FileNotFound(path));
        }

        try
        {
            using var stream = File.OpenRead(path);
            var dtos = JsonSerializer.Deserialize<List<PersonaDto>>(stream, _options);

            if (dtos == null)
                return Result.Failure<IEnumerable<Persona>, DomainError>(StorageErrors.InvalidFormat("No se pudieron deserializar los DTOs."));

            return Result.Success<IEnumerable<Persona>, DomainError>(dtos.Select(dto => dto.ToModel()));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al cargar los items del archivo '{path}'", path);
            return Result.Failure<IEnumerable<Persona>, DomainError>(StorageErrors.ReadError(ex.Message));
        }
    }

    private void InitStorage()
    {
        if (Directory.Exists(AppConfig.DataFolder))
            return;
        _logger.Debug("El directorio 'data' no existe. Creándolo...");
        Directory.CreateDirectory("data");
    }
}
