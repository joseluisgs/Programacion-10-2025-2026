using CSharpFunctionalExtensions;
using GestionAcademica.Entity;
using GestionAcademica.Errors.Common;
using GestionAcademica.Errors.Personas;
using GestionAcademica.Factories.Personas;
using GestionAcademica.Mappers.Personas;
using GestionAcademica.Models.Personas;
using GestionAcademica.Repositories.Personas.Base;
using Serilog;

namespace GestionAcademica.Repositories.Personas.Memory;

/// <summary>
/// Repositorio en memoria para la gestión de Personas.
/// Utiliza diccionarios para almacenamiento rápido.
/// </summary>
public class PersonasMemoryRepository : IPersonasRepository
{
    private readonly ILogger _logger = Log.ForContext<PersonasMemoryRepository>();
    private int _idCounter = 0;
    private readonly Dictionary<int, PersonaEntity> _porId = [];
    private readonly Dictionary<string, int> _dniIndex = [];
    private readonly Dictionary<string, int> _emailIndex = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Constructor principal. Carga datos de semilla si está configurado.
    /// </summary>
    public PersonasMemoryRepository(bool dropData, bool seedData)
    {
        if (dropData)
        {
            _logger.Warning("Borrando datos en memoria...");
            DeleteAll();
        }

        if (seedData)
        {
            _logger.Information("Cargando datos de semilla...");
            foreach (var persona in PersonasFactory.Seed())
            {
                Create(persona);
            }
            _logger.Information("SeedData completado.");
        }
    }

    /// <inheritdoc />
    public IEnumerable<Persona> GetAll(int page = 1, int pageSize = 10, bool includeDeleted = true)
    {
        _logger.Debug("Obteniendo personas con paginación: página {Page}, tamaño {PageSize}, incluir borrados: {IncludeDeleted}", 
            page, pageSize, includeDeleted);

        var query = includeDeleted 
            ? _porId.Values.AsEnumerable() 
            : _porId.Values.Where(e => !e.IsDeleted);

        return query
            .OrderBy(e => e.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToModel();
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
    public Persona? GetById(int id)
    {
        _logger.Debug("Obteniendo persona con id {Id}", id);
        return _porId.GetValueOrDefault(id).ToModel();
    }

    /// <inheritdoc />
    public Persona? GetByDni(string dni)
    {
        _logger.Debug("Obteniendo persona con DNI {Dni}", dni);
        return _dniIndex.TryGetValue(dni, out var id) && _porId.TryGetValue(id, out var entity)
            ? entity.ToModel()
            : null;
    }

    /// <inheritdoc />
    public bool ExisteDni(string dni)
    {
        return _dniIndex.ContainsKey(dni);
    }

    /// <inheritdoc />
    public Persona? GetByEmail(string email)
    {
        _logger.Debug("Obteniendo persona con Email {Email}", email);
        return _emailIndex.TryGetValue(email, out var id) && _porId.TryGetValue(id, out var entity)
            ? entity.ToModel()
            : null;
    }

    /// <inheritdoc />
    public bool ExisteEmail(string email)
    {
        return _emailIndex.ContainsKey(email);
    }

    /// <inheritdoc />
    public Result<Persona, DomainError> Create(Persona model)
    {
        _logger.Debug("Creando nueva persona {Dni}", model.Dni);

        if (ExisteDni(model.Dni))
        {
            _logger.Warning("No se puede crear: DNI {Dni} ya existe", model.Dni);
            return Result.Failure<Persona, DomainError>(PersonaErrors.DniAlreadyExists(model.Dni));
        }

        if (ExisteEmail(model.Email))
        {
            _logger.Warning("No se puede crear: Email {Email} ya existe", model.Email);
            return Result.Failure<Persona, DomainError>(PersonaErrors.EmailAlreadyExists(model.Email));
        }

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

        _logger.Information("Persona creada con ID {Id}", entity.Id);
        return Result.Success<Persona, DomainError>(entity.ToModel()!);
    }

    /// <inheritdoc />
    public Result<Persona, DomainError> Update(int id, Persona model)
    {
        _logger.Debug("Actualizando persona con Id {Id}", id);

        if (!_porId.TryGetValue(id, out var actual))
        {
            _logger.Warning("No se puede actualizar: persona con id {Id} no encontrada", id);
            return Result.Failure<Persona, DomainError>(PersonaErrors.NotFound(id.ToString()));
        }

        if (model.Dni != actual.Dni && _dniIndex.TryGetValue(model.Dni, out var otroId) && otroId != id)
        {
            _logger.Warning("No se puede actualizar: DNI {Dni} ya está en uso", model.Dni);
            return Result.Failure<Persona, DomainError>(PersonaErrors.DniAlreadyExists(model.Dni));
        }

        var newEmail = string.IsNullOrWhiteSpace(model.Email) ? actual.Email : model.Email;
        if (newEmail != actual.Email && _emailIndex.TryGetValue(newEmail, out var otroEmailId) && otroEmailId != id)
        {
            _logger.Warning("No se puede actualizar: Email {Email} ya está en uso", newEmail);
            return Result.Failure<Persona, DomainError>(PersonaErrors.EmailAlreadyExists(newEmail));
        }

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

        _logger.Information("Persona actualizada con ID {Id}", id);
        return Result.Success<Persona, DomainError>(entity.ToModel()!);
    }

    /// <inheritdoc />
    public Persona? Delete(int id, bool isLogical = true)
    {
        _logger.Debug("Eliminando persona con id {Id} (borrado lógico: {IsLogical})", id, isLogical);

        if (!_porId.TryGetValue(id, out var entity))
        {
            _logger.Warning("No se puede eliminar: persona con id {Id} no encontrada", id);
            return null;
        }

        if (isLogical)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            _logger.Information("Borrado lógico de persona con ID {Id}", id);
            return entity.ToModel();
        }
        else
        {
            _porId.Remove(id);
            _dniIndex.Remove(entity.Dni);
            if (!string.IsNullOrWhiteSpace(entity.Email))
                _emailIndex.Remove(entity.Email);
            _logger.Information("Borrado físico de persona con ID {Id}", id);
            return entity.ToModel();
        }
    }

    /// <inheritdoc />
    public bool DeleteAll()
    {
        _logger.Warning("Eliminando permanentemente todas las personas");
        _porId.Clear();
        _dniIndex.Clear();
        _emailIndex.Clear();
        _idCounter = 0;
        return true;
    }

    /// <inheritdoc />
    public int CountEstudiantes(bool includeDeleted = false)
    {
        var query = includeDeleted 
            ? _porId.Values.AsEnumerable() 
            : _porId.Values.Where(e => !e.IsDeleted);
        return query.Count(e => e.Tipo == "Estudiante");
    }

    /// <inheritdoc />
    public int CountDocentes(bool includeDeleted = false)
    {
        var query = includeDeleted 
            ? _porId.Values.AsEnumerable() 
            : _porId.Values.Where(e => !e.IsDeleted);
        return query.Count(e => e.Tipo == "Docente");
    }
}
