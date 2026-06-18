using System.IO;
using System.Text.Json;
using CSharpFunctionalExtensions;
using GestionAcademica.Entity;
using GestionAcademica.Errors.Common;
using GestionAcademica.Errors.Personas;
using GestionAcademica.Factories.Personas;
using GestionAcademica.Mappers.Personas;
using GestionAcademica.Models.Personas;
using GestionAcademica.Repositories.Personas.Base;
using Serilog;

namespace GestionAcademica.Repositories.Personas.Json;

/// <summary>
///     Repositorio de personas que utiliza almacenamiento en archivo JSON.
///     Persiste los datos en un archivo JSON con soporte para paginación.
/// </summary>
public class PersonasJsonRepository : IPersonasRepository
{
    private readonly Dictionary<string, int> _dniIndex = new();
    private readonly Dictionary<string, int> _emailIndex = new(StringComparer.OrdinalIgnoreCase);
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly ILogger _logger = Log.ForContext<PersonasJsonRepository>();
    private readonly Dictionary<int, PersonaEntity> _porId = new();
    private int _idCounter;

    private PersonasJsonRepository(string filePath)
    {
        _filePath = filePath;
        EnsureDirectory();
    }

    public static async Task<PersonasJsonRepository> CreateAsync(string filePath, bool dropData = false, bool seedData = false)
    {
        var repo = new PersonasJsonRepository(filePath);
        await repo.InitializeAsync(dropData, seedData);
        return repo;
    }

    private async Task InitializeAsync(bool dropData, bool seedData)
    {
        if (dropData && File.Exists(_filePath))
            File.Delete(_filePath);

        if (File.Exists(_filePath))
            await LoadAsync();

        if (seedData && _porId.Count == 0)
            foreach (var p in PersonasFactory.Seed())
                await CreateAsync(p);
    }

    public Task<IEnumerable<Persona>> GetAllAsync(int page = 1, int pageSize = 10, bool includeDeleted = true)
    {
        var query = includeDeleted
            ? _porId.Values.AsEnumerable()
            : _porId.Values.Where(e => !e.IsDeleted);

        return Task.FromResult(query
            .OrderBy(e => e.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToModel());
    }

    public Task<IEnumerable<Estudiante>> GetEstudiantesAsync(int page = 1, int pageSize = 10, bool includeDeleted = true)
    {
        var query = includeDeleted
            ? _porId.Values.AsEnumerable()
            : _porId.Values.Where(e => !e.IsDeleted);

        return Task.FromResult(query
            .Where(e => e.Tipo == "Estudiante")
            .OrderBy(e => e.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToModel()
            .Cast<Estudiante>());
    }

    public Task<IEnumerable<Docente>> GetDocentesAsync(int page = 1, int pageSize = 10, bool includeDeleted = true)
    {
        var query = includeDeleted
            ? _porId.Values.AsEnumerable()
            : _porId.Values.Where(e => !e.IsDeleted);

        return Task.FromResult(query
            .Where(e => e.Tipo == "Docente")
            .OrderBy(e => e.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToModel()
            .Cast<Docente>());
    }

    public Task<Persona?> GetByIdAsync(int id)
    {
        return Task.FromResult(_porId.GetValueOrDefault(id).ToModel());
    }

    public Task<Persona?> GetByDniAsync(string dni)
    {
        return Task.FromResult(
            _dniIndex.TryGetValue(dni, out var id) && _porId.TryGetValue(id, out var entity)
                ? entity.ToModel()
                : null);
    }

    public Task<bool> ExisteDniAsync(string dni)
    {
        return Task.FromResult(_dniIndex.ContainsKey(dni));
    }

    public Task<Persona?> GetByEmailAsync(string email)
    {
        return Task.FromResult(
            _emailIndex.TryGetValue(email, out var id) && _porId.TryGetValue(id, out var entity)
                ? entity.ToModel()
                : null);
    }

    public Task<bool> ExisteEmailAsync(string email)
    {
        return Task.FromResult(_emailIndex.ContainsKey(email));
    }

    public async Task<Result<Persona, DomainError>> CreateAsync(Persona model)
    {
        if (_dniIndex.ContainsKey(model.Dni ?? ""))
            return Result.Failure<Persona, DomainError>(PersonaErrors.DniAlreadyExists(model.Dni ?? ""));

        if (_emailIndex.ContainsKey(model.Email ?? ""))
            return Result.Failure<Persona, DomainError>(PersonaErrors.EmailAlreadyExists(model.Email ?? ""));

        model = model with
        {
            Id = ++_idCounter,
            FechaNacimiento = model.FechaNacimiento == default ? DateTime.UtcNow.AddYears(-18) : model.FechaNacimiento,
            Email = string.IsNullOrWhiteSpace(model.Email)
                ? $"{(model.Dni ?? "").ToLower()}@gestionacademica.local"
                : model.Email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false,
            DeletedAt = null
        };

        var entity = model.ToEntity();
        _porId[entity.Id] = entity;
        _dniIndex[entity.Dni ?? ""] = entity.Id;
        _emailIndex[entity.Email ?? ""] = entity.Id;

        await SaveAsync();
        return Result.Success<Persona, DomainError>(entity.ToModel()!);
    }

    public async Task<Result<Persona, DomainError>> UpdateAsync(int id, Persona model)
    {
        if (!_porId.TryGetValue(id, out var actual))
            return Result.Failure<Persona, DomainError>(PersonaErrors.NotFound(id.ToString()));

        if ((model.Dni ?? "") != (actual.Dni ?? "") && _dniIndex.TryGetValue(model.Dni ?? "", out var otroId) &&
            otroId != id)
            return Result.Failure<Persona, DomainError>(PersonaErrors.DniAlreadyExists(model.Dni ?? ""));

        var newEmail = string.IsNullOrWhiteSpace(model.Email) ? actual.Email ?? "" : model.Email;
        if (newEmail != (actual.Email ?? "") && _emailIndex.TryGetValue(newEmail, out var otroEmailId) &&
            otroEmailId != id)
            return Result.Failure<Persona, DomainError>(PersonaErrors.EmailAlreadyExists(newEmail));

        model = model with
        {
            Id = id,
            FechaNacimiento = model.FechaNacimiento == default ? actual.FechaNacimiento : model.FechaNacimiento,
            Email = newEmail,
            CreatedAt = actual.CreatedAt,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = actual.IsDeleted,
            DeletedAt = actual.DeletedAt
        };

        var entity = model.ToEntity();
        _porId[id] = entity;

        if (actual.Dni != entity.Dni)
        {
            _dniIndex.Remove(actual.Dni ?? "");
            _dniIndex[entity.Dni ?? ""] = id;
        }

        if (actual.Email != entity.Email)
        {
            _emailIndex.Remove(actual.Email ?? "");
            _emailIndex[entity.Email ?? ""] = id;
        }

        await SaveAsync();
        return Result.Success<Persona, DomainError>(entity.ToModel()!);
    }

    public async Task<Persona?> DeleteAsync(int id, bool isLogical = true)
    {
        try
        {
            if (!_porId.TryGetValue(id, out var entity))
                return null;

            if (isLogical)
            {
                entity.IsDeleted = true;
                entity.DeletedAt = DateTime.UtcNow;
                entity.UpdatedAt = DateTime.UtcNow;
                await SaveAsync();
                return entity.ToModel();
            }

            _porId.Remove(id);
            _dniIndex.Remove(entity.Dni);
            if (!string.IsNullOrWhiteSpace(entity.Email))
                _emailIndex.Remove(entity.Email);
            await SaveAsync();
            return entity.ToModel();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al eliminar persona");
            return null;
        }
    }

    public Task<bool> DeleteAllAsync()
    {
        try
        {
            _porId.Clear();
            _dniIndex.Clear();
            _emailIndex.Clear();
            _idCounter = 0;

            if (File.Exists(_filePath)) File.Delete(_filePath);

            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al eliminar todas las personas");
            return Task.FromResult(false);
        }
    }

    public Task<int> CountEstudiantesAsync(bool includeDeleted = false)
    {
        var query = includeDeleted
            ? _porId.Values.AsEnumerable()
            : _porId.Values.Where(e => !e.IsDeleted);
        return Task.FromResult(query.Count(e => e.Tipo == "Estudiante"));
    }

    public Task<int> CountDocentesAsync(bool includeDeleted = false)
    {
        var query = includeDeleted
            ? _porId.Values.AsEnumerable()
            : _porId.Values.Where(e => !e.IsDeleted);
        return Task.FromResult(query.Count(e => e.Tipo == "Docente"));
    }

    public Task<IEnumerable<Estudiante>> GetEstudiantesOrderByAsync(string orden, int page = 1, int pageSize = 10, bool includeDeleted = true)
    {
        var query = includeDeleted
            ? _porId.Values.AsEnumerable()
            : _porId.Values.Where(e => !e.IsDeleted);

        query = query.Where(e => e.Tipo == "Estudiante");

        query = orden.ToLower() switch
        {
            "id" => query.OrderBy(e => e.Id),
            "dni" => query.OrderBy(e => e.Dni),
            "nombre" => query.OrderBy(e => e.Nombre),
            "apellidos" => query.OrderBy(e => e.Apellidos),
            "nota" => query.OrderByDescending(e => e.Calificacion),
            "ciclo" => query.OrderBy(e => e.Ciclo),
            "curso" => query.OrderBy(e => e.Curso),
            _ => query.OrderBy(e => e.Dni)
        };

        return Task.FromResult(query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToModel()
            .Cast<Estudiante>());
    }

    public Task<IEnumerable<Docente>> GetDocentesOrderByAsync(string orden, int page = 1, int pageSize = 10, bool includeDeleted = true)
    {
        var query = includeDeleted
            ? _porId.Values.AsEnumerable()
            : _porId.Values.Where(e => !e.IsDeleted);

        query = query.Where(e => e.Tipo == "Docente");

        query = orden.ToLower() switch
        {
            "id" => query.OrderBy(e => e.Id),
            "dni" => query.OrderBy(e => e.Dni),
            "nombre" => query.OrderBy(e => e.Nombre),
            "apellidos" => query.OrderBy(e => e.Apellidos),
            "experiencia" => query.OrderByDescending(e => e.Experiencia),
            "ciclo" => query.OrderBy(e => e.Ciclo),
            "modulo" or "especialidad" => query.OrderBy(e => e.Especialidad),
            _ => query.OrderBy(e => e.Dni)
        };

        return Task.FromResult(query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToModel()
            .Cast<Docente>());
    }

    public async Task<Result<Persona, DomainError>> RestoreAsync(int id)
    {
        if (!_porId.TryGetValue(id, out var entity))
        {
            _logger.Warning("No se puede restaurar: persona con id {Id} no encontrada", id);
            return Result.Failure<Persona, DomainError>(PersonaErrors.NotFound(id.ToString()));
        }

        entity.IsDeleted = false;
        entity.DeletedAt = null;
        entity.UpdatedAt = DateTime.UtcNow;

        await SaveAsync();

        _logger.Information("Persona con ID {Id} restaurada correctamente", id);
        return Result.Success<Persona, DomainError>(entity.ToModel()!);
    }

    private void EnsureDirectory()
    {
        var dir = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
    }

    private async Task LoadAsync()
    {
        try
        {
            if (!File.Exists(_filePath)) return;

            var json = await File.ReadAllTextAsync(_filePath);
            var entities = JsonSerializer.Deserialize<List<PersonaEntity>>(json, _jsonOptions);

            if (entities == null) return;

            foreach (var e in entities)
            {
                _porId[e.Id] = e;
                _dniIndex[e.Dni] = e.Id;
                if (!string.IsNullOrWhiteSpace(e.Email))
                    _emailIndex[e.Email] = e.Id;
                if (e.Id > _idCounter) _idCounter = e.Id;
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al cargar el archivo JSON.");
        }
    }

    private async Task SaveAsync()
    {
        try
        {
            var json = JsonSerializer.Serialize(_porId.Values.ToList(), _jsonOptions);
            await File.WriteAllTextAsync(_filePath, json);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al guardar el archivo JSON.");
        }
    }
}
