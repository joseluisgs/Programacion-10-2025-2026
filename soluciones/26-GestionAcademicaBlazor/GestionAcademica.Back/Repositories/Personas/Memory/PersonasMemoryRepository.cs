using CSharpFunctionalExtensions;
using GestionAcademica.Config;
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
///     Repositorio en memoria para la gestión de Personas.
///     Utiliza diccionarios para almacenamiento rápido.
/// </summary>
public class PersonasMemoryRepository : IPersonasRepository {
    private readonly Dictionary<string, int> _dniIndex = [];
    private readonly Dictionary<string, int> _emailIndex = new(StringComparer.OrdinalIgnoreCase);
    private readonly ILogger _logger = Log.ForContext<PersonasMemoryRepository>();
    private readonly Dictionary<int, PersonaEntity> _porId = [];
    private int _idCounter;

    /// <summary>
    ///     Constructor delegado que usa la configuración de la aplicación.
    /// </summary>
    public PersonasMemoryRepository() : this(AppConfig.DropData, AppConfig.SeedData) { }

    /// <summary>
    ///     Constructor principal que contiene la lógica de inicialización necesaria.
    /// </summary>
    public PersonasMemoryRepository(bool dropData, bool seedData) {
        if (dropData) {
            _logger.Warning("Borrando datos en memoria...");
            DeleteAll();
        }

        if (seedData) {
            _logger.Information("Cargando datos de semilla...");
            foreach (var persona in PersonasFactory.Seed()) CreateCore(persona);
            _logger.Information("SeedData completado.");
        }
    }

    /// <inheritdoc />
    public Task<IEnumerable<Persona>> GetAllAsync(int page = 1, int pageSize = 10, bool includeDeleted = true) {
        _logger.Debug(
            "Obteniendo personas con paginación: página {Page}, tamaño {PageSize}, incluir borrados: {IncludeDeleted}",
            page, pageSize, includeDeleted);

        var query = includeDeleted
            ? _porId.Values.AsEnumerable()
            : _porId.Values.Where(e => !e.IsDeleted);

        var result = query
            .OrderBy(e => e.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToModel();

        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public Task<IEnumerable<Estudiante>> GetEstudiantesAsync(int page = 1, int pageSize = 10, bool includeDeleted = true) {
        var query = includeDeleted
            ? _porId.Values.AsEnumerable()
            : _porId.Values.Where(e => !e.IsDeleted);

        var result = query
            .Where(e => e.Tipo == "Estudiante")
            .OrderBy(e => e.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToModel()
            .Cast<Estudiante>();

        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public Task<IEnumerable<Docente>> GetDocentesAsync(int page = 1, int pageSize = 10, bool includeDeleted = true) {
        var query = includeDeleted
            ? _porId.Values.AsEnumerable()
            : _porId.Values.Where(e => !e.IsDeleted);

        var result = query
            .Where(e => e.Tipo == "Docente")
            .OrderBy(e => e.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToModel()
            .Cast<Docente>();

        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public Task<Persona?> GetByIdAsync(int id) {
        _logger.Debug("Obteniendo persona con id {Id}", id);
        return Task.FromResult(_porId.GetValueOrDefault(id).ToModel());
    }

    /// <inheritdoc />
    public Task<Persona?> GetByDniAsync(string dni) {
        _logger.Debug("Obteniendo persona con DNI {Dni}", dni);
        var result = _dniIndex.TryGetValue(dni, out var id) && _porId.TryGetValue(id, out var entity)
            ? entity.ToModel()
            : null;
        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public Task<bool> ExisteDniAsync(string dni) {
        return Task.FromResult(_dniIndex.ContainsKey(dni));
    }

    /// <inheritdoc />
    public Task<Persona?> GetByEmailAsync(string email) {
        _logger.Debug("Obteniendo persona con Email {Email}", email);
        var result = _emailIndex.TryGetValue(email, out var id) && _porId.TryGetValue(id, out var entity)
            ? entity.ToModel()
            : null;
        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public Task<bool> ExisteEmailAsync(string email) {
        return Task.FromResult(_emailIndex.ContainsKey(email));
    }

    /// <inheritdoc />
    public Task<Result<Persona, DomainError>> CreateAsync(Persona model) {
        return Task.FromResult(CreateCore(model));
    }

    /// <inheritdoc />
    public Task<Result<Persona, DomainError>> UpdateAsync(int id, Persona model) {
        _logger.Debug("Actualizando persona con Id {Id}", id);

        if (!_porId.TryGetValue(id, out var actual)) {
            _logger.Warning("No se puede actualizar: persona con id {Id} no encontrada", id);
            return Task.FromResult(Result.Failure<Persona, DomainError>(PersonaErrors.NotFound(id.ToString())));
        }

        if ((model.Dni ?? "") != (actual.Dni ?? "") && _dniIndex.TryGetValue(model.Dni ?? "", out var otroId) &&
            otroId != id) {
            _logger.Warning("No se puede actualizar: DNI {Dni} ya está en uso", model.Dni);
            return Task.FromResult(Result.Failure<Persona, DomainError>(PersonaErrors.DniAlreadyExists(model.Dni ?? "")));
        }

        var newEmail = string.IsNullOrWhiteSpace(model.Email) ? actual.Email ?? "" : model.Email;
        if (newEmail != (actual.Email ?? "") && _emailIndex.TryGetValue(newEmail, out var otroEmailId) &&
            otroEmailId != id) {
            _logger.Warning("No se puede actualizar: Email {Email} ya está en uso", newEmail);
            return Task.FromResult(Result.Failure<Persona, DomainError>(PersonaErrors.EmailAlreadyExists(newEmail)));
        }

        // El repositorio preserva los campos de auditoría originales (CreatedAt, IsDeleted)
        // y actualiza la fecha de modificación (UpdatedAt)
        model = model with {
            Id = id,
            FechaNacimiento = model.FechaNacimiento == default ? actual.FechaNacimiento : model.FechaNacimiento,
            Email = newEmail,
            CreatedAt = actual.CreatedAt, // Preservamos fecha creación original
            UpdatedAt = DateTime.UtcNow, // Nueva fecha de actualización
            IsDeleted = actual.IsDeleted, // Preservamos estado de borrado
            DeletedAt = actual.DeletedAt // Preservamos fecha de borrado
        };

        var entity = model.ToEntity();
        _porId[id] = entity;

        if (actual.Dni != entity.Dni) {
            _dniIndex.Remove(actual.Dni ?? "");
            _dniIndex[entity.Dni ?? ""] = id;
        }

        if (actual.Email != entity.Email) {
            _emailIndex.Remove(actual.Email ?? "");
            _emailIndex[entity.Email ?? ""] = id;
        }

        _logger.Information("Persona con ID {Id} actualizada correctamente", id);
        return Task.FromResult(Result.Success<Persona, DomainError>(entity.ToModel()!));
    }

    /// <inheritdoc />
    public Task<Persona?> DeleteAsync(int id, bool isLogical = true) {
        _logger.Debug("Eliminando persona con id {Id} (borrado lógico: {IsLogical})", id, isLogical);

        if (!_porId.TryGetValue(id, out var entity)) {
            _logger.Warning("No se puede eliminar: persona con id {Id} no encontrada", id);
            return Task.FromResult<Persona?>(null);
        }

        if (isLogical) {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            _logger.Information("Borrado lógico de persona con ID {Id}", id);
            return Task.FromResult(entity.ToModel());
        }

        _porId.Remove(id);
        _dniIndex.Remove(entity.Dni);
        if (!string.IsNullOrWhiteSpace(entity.Email))
            _emailIndex.Remove(entity.Email);
        _logger.Information("Borrado físico de persona con ID {Id}", id);
        return Task.FromResult(entity.ToModel());
    }

    /// <inheritdoc />
    public Task<bool> DeleteAllAsync() {
        _logger.Warning("Eliminando permanentemente todas las personas");
        _porId.Clear();
        _dniIndex.Clear();
        _emailIndex.Clear();
        _idCounter = 0;
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public Task<int> CountEstudiantesAsync(bool includeDeleted = false) {
        var query = includeDeleted
            ? _porId.Values.AsEnumerable()
            : _porId.Values.Where(e => !e.IsDeleted);
        return Task.FromResult(query.Count(e => e.Tipo == "Estudiante"));
    }

    /// <inheritdoc />
    public Task<int> CountDocentesAsync(bool includeDeleted = false) {
        var query = includeDeleted
            ? _porId.Values.AsEnumerable()
            : _porId.Values.Where(e => !e.IsDeleted);
        return Task.FromResult(query.Count(e => e.Tipo == "Docente"));
    }

    /// <inheritdoc />
    public Task<IEnumerable<Estudiante>> GetEstudiantesOrderByAsync(string orden, int page = 1, int pageSize = 10, bool includeDeleted = true) {
        var query = includeDeleted
            ? _porId.Values.AsEnumerable()
            : _porId.Values.Where(e => !e.IsDeleted);

        query = query.Where(e => e.Tipo == "Estudiante");

        query = orden.ToLower() switch {
            "id" => query.OrderBy(e => e.Id),
            "dni" => query.OrderBy(e => e.Dni),
            "nombre" => query.OrderBy(e => e.Nombre),
            "apellidos" => query.OrderBy(e => e.Apellidos),
            "nota" => query.OrderByDescending(e => e.Calificacion),
            "ciclo" => query.OrderBy(e => e.Ciclo),
            "curso" => query.OrderBy(e => e.Curso),
            _ => query.OrderBy(e => e.Dni)
        };

        var result = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToModel()
            .Cast<Estudiante>();

        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public Task<IEnumerable<Docente>> GetDocentesOrderByAsync(string orden, int page = 1, int pageSize = 10, bool includeDeleted = true) {
        var query = includeDeleted
            ? _porId.Values.AsEnumerable()
            : _porId.Values.Where(e => !e.IsDeleted);

        query = query.Where(e => e.Tipo == "Docente");

        query = orden.ToLower() switch {
            "id" => query.OrderBy(e => e.Id),
            "dni" => query.OrderBy(e => e.Dni),
            "nombre" => query.OrderBy(e => e.Nombre),
            "apellidos" => query.OrderBy(e => e.Apellidos),
            "experiencia" => query.OrderByDescending(e => e.Experiencia),
            "ciclo" => query.OrderBy(e => e.Ciclo),
            "modulo" or "especialidad" => query.OrderBy(e => e.Especialidad),
            _ => query.OrderBy(e => e.Dni)
        };

        var result = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToModel()
            .Cast<Docente>();

        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public Task<Result<Persona, DomainError>> RestoreAsync(int id) {
        if (!_porId.TryGetValue(id, out var entity)) {
            _logger.Warning("No se puede restaurar: persona con id {Id} no encontrada", id);
            return Task.FromResult(Result.Failure<Persona, DomainError>(PersonaErrors.NotFound(id.ToString())));
        }

        var restored = new PersonaEntity {
            Id = entity.Id,
            Dni = entity.Dni,
            Nombre = entity.Nombre,
            Apellidos = entity.Apellidos,
            FechaNacimiento = entity.FechaNacimiento,
            Email = entity.Email,
            Imagen = entity.Imagen,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false,
            DeletedAt = null,
            Tipo = entity.Tipo,
            Calificacion = entity.Calificacion,
            Ciclo = entity.Ciclo,
            Curso = entity.Curso,
            Experiencia = entity.Experiencia,
            Especialidad = entity.Especialidad
        };
        _porId[id] = restored;

        _dniIndex[restored.Dni ?? ""] = id;
        if (!string.IsNullOrWhiteSpace(restored.Email))
            _emailIndex[restored.Email] = id;

        _logger.Information("Persona con ID {Id} restaurada correctamente", id);
        return Task.FromResult(Result.Success<Persona, DomainError>(restored.ToModel()!));
    }

    private Result<Persona, DomainError> CreateCore(Persona model) {
        _logger.Debug("Creando nueva persona {Dni}", model.Dni);

        if (_dniIndex.ContainsKey(model.Dni ?? "")) {
            _logger.Warning("No se puede crear: DNI {Dni} ya existe", model.Dni);
            return Result.Failure<Persona, DomainError>(PersonaErrors.DniAlreadyExists(model.Dni ?? ""));
        }

        if (_emailIndex.ContainsKey(model.Email ?? "")) {
            _logger.Warning("No se puede crear: Email {Email} ya existe", model.Email);
            return Result.Failure<Persona, DomainError>(PersonaErrors.EmailAlreadyExists(model.Email ?? ""));
        }

        // El repositorio es el dueño de la creación de metadatos
        model = model with {
            Id = ++_idCounter,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false,
            DeletedAt = null
        };

        var entity = model.ToEntity();
        _porId[entity.Id] = entity;
        _dniIndex[entity.Dni] = entity.Id;
        _emailIndex[entity.Email] = entity.Id;

        _logger.Information("Persona creada con ID {Id}", entity.Id);
        return Result.Success<Persona, DomainError>(entity.ToModel()!);
    }

    private bool DeleteAll() {
        _logger.Warning("Eliminando permanentemente todas las personas");
        _porId.Clear();
        _dniIndex.Clear();
        _emailIndex.Clear();
        _idCounter = 0;
        return true;
    }
}
