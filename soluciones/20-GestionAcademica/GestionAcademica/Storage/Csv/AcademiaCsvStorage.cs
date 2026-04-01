using System.IO;
using System.Text;
using CSharpFunctionalExtensions;
using GestionAcademica.Config;
using GestionAcademica.Dto.Personas;
using GestionAcademica.Errors.Common;
using GestionAcademica.Errors.Storage;
using GestionAcademica.Mappers.Personas;
using GestionAcademica.Models.Personas;
using Serilog;

namespace GestionAcademica.Storage.Csv;

public class AcademiaCsvStorage : IAcademiaCsvStorage
{
    private readonly ILogger _logger = Log.ForContext<AcademiaCsvStorage>();

    public AcademiaCsvStorage()
    {
        _logger.Debug("Inicializando la clase AcademiaCsvStorage");
        InitStorage();
    }

    public Result<bool, DomainError> Salvar(IEnumerable<Persona> items, string path)
    {
        try
        {
            _logger.Debug("Guardando los items en el archivo '{path}'", path);
            using var writer = new StreamWriter(path, false, new System.Text.UTF8Encoding(false));
            writer.WriteLine("Id;Dni;Nombre;Apellidos;FechaNacimiento;Email;Imagen;Tipo;Experiencia;Especialidad;Ciclo;Curso;Calificacion;CreatedAt;UpdatedAt;IsDeleted;DeletedAt");

            foreach (var p in items)
            {
                var dto = p.ToDto();
                writer.WriteLine($"{dto.Id};{dto.Dni};{dto.Nombre};{dto.Apellidos};{dto.FechaNacimiento};{dto.Email};{dto.Imagen};{dto.Tipo};{dto.Experiencia};{dto.Especialidad};{dto.Ciclo};{dto.Curso};{dto.Calificacion};{dto.CreatedAt};{dto.UpdatedAt};{dto.IsDeleted};{dto.DeletedAt}");
            }
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
            var personas = File.ReadLines(path, Encoding.UTF8)
                .Skip(1)
                .Select(linea => linea.Split(';'))
                .Select(campos => new PersonaDto(
                    int.Parse(campos[0]),
                    campos[1],
                    campos[2],
                    campos[3],
                    campos[4],
                    campos[5],
                    string.IsNullOrEmpty(campos[6]) ? null : campos[6],
                    campos[7],
                    campos[8],
                    campos[9],
                    campos[10],
                    campos[11],
                    campos[12],
                    campos[13],
                    campos[14],
                    bool.TryParse(campos[15], out var isDel) && isDel,
                    string.IsNullOrEmpty(campos[16]) ? null : campos[16]
                ).ToModel());

            return Result.Success<IEnumerable<Persona>, DomainError>(personas);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al cargar los items del archivo '{path}'", path);
            return Result.Failure<IEnumerable<Persona>, DomainError>(StorageErrors.InvalidFormat(ex.Message));
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
