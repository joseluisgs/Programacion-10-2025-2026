using System.IO;
using System.Text.Json;
using CSharpFunctionalExtensions;
using GestionAcademica.Entity;
using GestionAcademica.Errors.Common;
using GestionAcademica.Errors.Personas;
using GestionAcademica.Mappers.Personas;
using GestionAcademica.Models.Personas;
using GestionAcademica.Repositories.Personas.Base;
using Serilog;

namespace GestionAcademica.Repositories.Personas.Json;

public class PersonasJsonRepository : IPersonasRepository
{
    private readonly ILogger _logger = Log.ForContext<PersonasJsonRepository>();
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    
    private readonly string _filePath;
    private int _idCounter = 0;
    private readonly Dictionary<int, PersonaEntity> _porId = new();
    private readonly Dictionary<string, int> _dniIndex = new();
    private readonly Dictionary<string, int> _emailIndex = new(StringComparer.OrdinalIgnoreCase);

    public PersonasJsonRepository(string filePath)
    {
        _filePath = filePath;
        EnsureDirectory();
        
        if (File.Exists(_filePath))
        {
            Load();
        }
    }

    private void EnsureDirectory()
    {
        var dir = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    private void Load()
    {
        try
        {
            if (!File.Exists(_filePath)) return;

            var json = File.ReadAllText(_filePath);
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

    private void Save()
    {
        try
        {
            var json = JsonSerializer.Serialize(_porId.Values.ToList(), _jsonOptions);
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al guardar el archivo JSON.");
        }
    }

    public IEnumerable<Persona> GetAll(int page = 1, int pageSize = 10, bool includeDeleted = true)
    {
        var query = includeDeleted 
            ? _porId.Values.AsEnumerable() 
            : _porId.Values.Where(e => !e.IsDeleted);

        return query
            .OrderBy(e => e.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToModel();
    }

    public IEnumerable<Estudiante> GetEstudiantes(int page = 1, int pageSize = 10, bool includeDeleted = true)
    {
        var query = includeDeleted 
            ? _porId.Values.AsEnumerable() 
            : _porId.Values.Where(e => !e.IsDeleted);

        return query
            .Where(e => e.Tipo == "Estudiante")
            .OrderBy(e => e.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToModel()
            .Cast<Estudiante>();
    }

    public IEnumerable<Docente> GetDocentes(int page = 1, int pageSize = 10, bool includeDeleted = true)
    {
        var query = includeDeleted 
            ? _porId.Values.AsEnumerable() 
            : _porId.Values.Where(e => !e.IsDeleted);

        return query
            .Where(e => e.Tipo == "Docente")
            .OrderBy(e => e.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToModel()
            .Cast<Docente>();
    }

    public Persona? GetById(int id)
    {
        return _porId.GetValueOrDefault(id).ToModel();
    }

    public Persona? GetByDni(string dni)
    {
        return _dniIndex.TryGetValue(dni, out var id) && _porId.TryGetValue(id, out var entity)
            ? entity.ToModel()
            : null;
    }

    public bool ExisteDni(string dni)
    {
        return _dniIndex.ContainsKey(dni);
    }

    public Persona? GetByEmail(string email)
    {
        return _emailIndex.TryGetValue(email, out var id) && _porId.TryGetValue(id, out var entity)
            ? entity.ToModel()
            : null;
    }

    public bool ExisteEmail(string email)
    {
        return _emailIndex.ContainsKey(email);
    }

    public Result<Persona, DomainError> Create(Persona model)
    {
        if (ExisteDni(model.Dni))
            return Result.Failure<Persona, DomainError>(PersonaErrors.DniAlreadyExists(model.Dni));

        if (ExisteEmail(model.Email))
            return Result.Failure<Persona, DomainError>(PersonaErrors.EmailAlreadyExists(model.Email));

        model = model with
        {
            Id = 0,
            FechaNacimiento = model.FechaNacimiento == default ? DateTime.UtcNow.AddYears(-18) : model.FechaNacimiento,
            Email = string.IsNullOrWhiteSpace(model.Email) ? $"{model.Dni.ToLower()}@gestionacademica.local" : model.Email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false,
            DeletedAt = null
        };

        var entity = model.ToEntity();
        entity.Id = ++_idCounter;

        _porId[entity.Id] = entity;
        _dniIndex[entity.Dni] = entity.Id;
        _emailIndex[entity.Email] = entity.Id;

        Save();
        return Result.Success<Persona, DomainError>(entity.ToModel()!);
    }

    public Result<Persona, DomainError> Update(int id, Persona model)
    {
        if (!_porId.TryGetValue(id, out var actual))
            return Result.Failure<Persona, DomainError>(PersonaErrors.NotFound(id.ToString()));

        if (model.Dni != actual.Dni && _dniIndex.TryGetValue(model.Dni, out var otroId) && otroId != id)
            return Result.Failure<Persona, DomainError>(PersonaErrors.DniAlreadyExists(model.Dni));

        var newEmail = string.IsNullOrWhiteSpace(model.Email) ? actual.Email : model.Email;
        if (newEmail != actual.Email && _emailIndex.TryGetValue(newEmail, out var otroEmailId) && otroEmailId != id)
            return Result.Failure<Persona, DomainError>(PersonaErrors.EmailAlreadyExists(newEmail));

        model = model with
        {
            Id = id,
            FechaNacimiento = model.FechaNacimiento == default ? actual.FechaNacimiento : model.FechaNacimiento,
            Email = newEmail,
            CreatedAt = actual.CreatedAt,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false,
            DeletedAt = null
        };

        var entity = model.ToEntity();
        _porId[id] = entity;

        if (actual.Dni != entity.Dni)
        {
            _dniIndex.Remove(actual.Dni);
            _dniIndex[entity.Dni] = id;
        }

        if (actual.Email != entity.Email)
        {
            _emailIndex.Remove(actual.Email);
            _emailIndex[entity.Email] = id;
        }

        Save();
        return Result.Success<Persona, DomainError>(entity.ToModel()!);
    }

    public Persona? Delete(int id, bool isLogical = true)
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
                Save();
                return entity.ToModel();
            }
            else
            {
                _porId.Remove(id);
                _dniIndex.Remove(entity.Dni);
                if (!string.IsNullOrWhiteSpace(entity.Email))
                    _emailIndex.Remove(entity.Email);
                Save();
                return entity.ToModel();
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al eliminar persona");
            return null;
        }
    }

    public bool DeleteAll()
    {
        try
        {
            _porId.Clear();
            _dniIndex.Clear();
            _emailIndex.Clear();
            _idCounter = 0;

            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al eliminar todas las personas");
            return false;
        }
    }

    public int CountEstudiantes(bool includeDeleted = false)
    {
        var query = includeDeleted 
            ? _porId.Values.AsEnumerable() 
            : _porId.Values.Where(e => !e.IsDeleted);
        return query.Count(e => e.Tipo == "Estudiante");
    }

    public int CountDocentes(bool includeDeleted = false)
    {
        var query = includeDeleted 
            ? _porId.Values.AsEnumerable() 
            : _porId.Values.Where(e => !e.IsDeleted);
        return query.Count(e => e.Tipo == "Docente");
    }
}
