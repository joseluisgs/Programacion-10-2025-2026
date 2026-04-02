using System.IO;
using System.Text;
using CSharpFunctionalExtensions;
using GestionAcademica.Config;
using GestionAcademica.Dto.Personas;
using GestionAcademica.Errors.Common;
using GestionAcademica.Errors.Storage;
using GestionAcademica.Mappers.Personas;
using GestionAcademica.Models.Personas;
using GestionAcademica.Storage.Common;
using Serilog;

namespace GestionAcademica.Storage.Binary;

public class AcademiaBinStorage : IAcademiaBinStorage
{
    private readonly ILogger _logger = Log.ForContext<AcademiaBinStorage>();

    public AcademiaBinStorage()
    {
        _logger.Debug("Inicializando la clase AcademiaBinStorage");
        InitStorage();
    }

    public Result<bool, DomainError> Salvar(IEnumerable<Persona> items, string path)
    {
        try
        {
            _logger.Debug("Guardando los items en el archivo binario '{path}'", path);
            using var stream = File.Create(path);
            using var writer = new BinaryWriter(stream, Encoding.UTF8);

            var dtos = items.Select(p => p.ToDto()).ToList();
            writer.Write(dtos.Count);

            foreach (var dto in dtos)
            {
                writer.Write(dto.Id);
                writer.Write(dto.Dni);
                writer.Write(dto.Nombre);
                writer.Write(dto.Apellidos);
                writer.Write(dto.FechaNacimiento);
                writer.Write(dto.Email);
                writer.Write(dto.Imagen ?? "");
                writer.Write(dto.Tipo);
                writer.Write(dto.Experiencia ?? "");
                writer.Write(dto.Especialidad ?? "");
                writer.Write(dto.Ciclo);
                writer.Write(dto.Curso ?? "");
                writer.Write(dto.Calificacion ?? "");
                writer.Write(dto.CreatedAt);
                writer.Write(dto.UpdatedAt);
                writer.Write(dto.IsDeleted);
                writer.Write(dto.DeletedAt ?? "");
            }
            return Result.Success<bool, DomainError>(true);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al guardar los items en el archivo binario '{path}'", path);
            return Result.Failure<bool, DomainError>(StorageErrors.WriteError(ex.Message));
        }
    }

    public Result<IEnumerable<Persona>, DomainError> Cargar(string path)
    {
        _logger.Debug("Cargando los items del archivo binario '{path}'", path);

        if (!File.Exists(path))
        {
            _logger.Warning("El archivo '{path}' no existe.", path);
            return Result.Failure<IEnumerable<Persona>, DomainError>(StorageErrors.FileNotFound(path));
        }

        try
        {
            using var stream = File.OpenRead(path);
            using var reader = new BinaryReader(stream, Encoding.UTF8);

            var count = reader.ReadInt32();
            var personas = new List<Persona>();

            for (var i = 0; i < count; i++)
            {
                var dto = new PersonaDto(
                    reader.ReadInt32(),
                    reader.ReadString(),
                    reader.ReadString(),
                    reader.ReadString(),
                    reader.ReadString(),
                    reader.ReadString(),
                    string.IsNullOrEmpty(reader.ReadString()) ? null : reader.ReadString(),
                    reader.ReadString(),
                    reader.ReadString(),
                    reader.ReadString(),
                    reader.ReadString(),
                    reader.ReadString(),
                    reader.ReadString(),
                    reader.ReadString(),
                    reader.ReadString(),
                    reader.ReadBoolean(),
                    string.IsNullOrEmpty(reader.ReadString()) ? null : reader.ReadString()
                );
                personas.Add(dto.ToModel());
            }

            return Result.Success<IEnumerable<Persona>, DomainError>(personas);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al cargar los items del archivo binario '{path}'", path);
            return Result.Failure<IEnumerable<Persona>, DomainError>(StorageErrors.InvalidFormat(ex.Message));
        }
    }

    private void InitStorage()
    {
        if (Directory.Exists(AppConfig.DataFolder))
            return;
        _logger.Debug("El directorio 'data' no existe. Creándolo...");
        Directory.CreateDirectory(AppConfig.DataFolder);
    }
}
