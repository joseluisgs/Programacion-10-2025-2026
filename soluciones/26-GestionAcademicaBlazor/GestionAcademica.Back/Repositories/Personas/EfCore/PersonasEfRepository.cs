using CSharpFunctionalExtensions;
using GestionAcademica.Config;
using GestionAcademica.Entity;
using GestionAcademica.Errors.Common;
using GestionAcademica.Errors.Personas;
using GestionAcademica.Factories.Personas;
using GestionAcademica.Mappers.Personas;
using GestionAcademica.Models.Personas;
using GestionAcademica.Repositories.Personas.Base;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace GestionAcademica.Repositories.Personas.EfCore;

/// <summary>
///     Repositorio de personas que utiliza Entity Framework Core con SQLite.
///     Cada método crea su propio AppDbContext con using (patrón DbContextFactory).
///
///     NOTA EDUCATIVA SOBRE DbContext EN EF CORE:
///     DbContext está diseñado para ser de vida CORTA (crear, usar, destruir).
///     NO debe ser Singleton ni almacenarse como campo de clase porque:
///     1. El ChangeTracker acumula entidades en memoria → memory leak
///     2. Dos usuarios comparten el mismo contexto → datos entremezclados
///     3. Las consultas en cache pueden devolver datos obsoletos
///
///     Por eso cada método crea su propio contexto con using y lo descarta
///     al terminar. En una aplicación real usaríamos IDbContextFactory&lt;T&gt;
///     (inyectado por DI) en lugar de new AppDbContext(...). Pero para este
///     proyecto educativo, crear el contexto manualmente es más explícito
///     y muestra el concepto con claridad.
/// </summary>
public class PersonasEfRepository : IPersonasRepository {
    private readonly string _connectionString;
    private readonly ILogger _logger = Log.ForContext<PersonasEfRepository>();
    private bool _initialized;
    private readonly SemaphoreSlim _initSemaphore = new(1, 1);
    private readonly bool _dropData;
    private readonly bool _seedData;

    public PersonasEfRepository(string connectionString, bool dropData = false, bool seedData = false) {
        _connectionString = connectionString;
        _dropData = dropData;
        _seedData = seedData;
    }

    /// <summary>
    ///     Inicializa la base de datos (crear tablas, sembrar datos) UNA SOLA VEZ.
    ///     Se usa un SemaphoreSlim estático para evitar condiciones de carrera en el primer arranque.
    /// </summary>
    private async Task InitializeAsync() {
        if (_initialized) return;
        await _initSemaphore.WaitAsync();
        try {
            if (_initialized) return;

            using var context = CreateContext();
            if (_dropData) await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            if (_seedData && !await context.Personas.AnyAsync()) {
                _logger.Information("Sembrando datos de personas...");
                foreach (var p in PersonasFactory.Seed()) {
                    using var seedCtx = CreateContext();
                    var entity = p.ToEntity();
                    seedCtx.Personas.Add(entity);
                    await seedCtx.SaveChangesAsync();
                }
            }

            _initialized = true;
        }
        finally {
            _initSemaphore.Release();
        }
    }

    /// <summary>
    ///     Crea un nuevo AppDbContext con la connection string configurada.
    ///     Cada llamada devuelve un contexto FRESCO, sin entidades en el ChangeTracker.
    /// </summary>
    private AppDbContext CreateContext() {
        return new AppDbContext(_connectionString);
    }

    public async Task<IEnumerable<Persona>> GetAllAsync(int page = 1, int pageSize = 10, bool includeDeleted = true) {
        try {
            await InitializeAsync();
            using var context = CreateContext();
            var query = includeDeleted
                ? context.Personas.AsNoTracking()
                : context.Personas.Where(p => !p.IsDeleted).AsNoTracking();

            var entities = await query
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return entities.ToModel();
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al obtener personas");
            return Enumerable.Empty<Persona>();
        }
    }

    public async Task<IEnumerable<Estudiante>> GetEstudiantesAsync(int page = 1, int pageSize = 10, bool includeDeleted = true) {
        try {
            await InitializeAsync();
            using var context = CreateContext();
            var query = includeDeleted
                ? context.Personas.Where(p => p.Tipo == "Estudiante").AsNoTracking()
                : context.Personas.Where(p => p.Tipo == "Estudiante" && !p.IsDeleted).AsNoTracking();

            var entities = await query
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return entities.ToModel().Cast<Estudiante>();
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al obtener estudiantes");
            return Enumerable.Empty<Estudiante>();
        }
    }

    public async Task<IEnumerable<Docente>> GetDocentesAsync(int page = 1, int pageSize = 10, bool includeDeleted = true) {
        try {
            await InitializeAsync();
            using var context = CreateContext();
            var query = includeDeleted
                ? context.Personas.Where(p => p.Tipo == "Docente").AsNoTracking()
                : context.Personas.Where(p => p.Tipo == "Docente" && !p.IsDeleted).AsNoTracking();

            var entities = await query
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return entities.ToModel().Cast<Docente>();
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al obtener docentes");
            return Enumerable.Empty<Docente>();
        }
    }

    public async Task<Persona?> GetByIdAsync(int id) {
        try {
            await InitializeAsync();
            using var context = CreateContext();
            var entity = await context.Personas.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            return entity.ToModel();
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al obtener persona por ID {Id}", id);
            return null;
        }
    }

    public async Task<Persona?> GetByDniAsync(string dni) {
        try {
            await InitializeAsync();
            using var context = CreateContext();
            var entity = await context.Personas.AsNoTracking().FirstOrDefaultAsync(p => p.Dni == dni);
            return entity.ToModel();
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al obtener persona por DNI {Dni}", dni);
            return null;
        }
    }

    public async Task<bool> ExisteDniAsync(string dni) {
        try {
            await InitializeAsync();
            using var context = CreateContext();
            return await context.Personas.AnyAsync(p => p.Dni == dni);
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al verificar DNI {Dni}", dni);
            return false;
        }
    }

    public async Task<Persona?> GetByEmailAsync(string email) {
        try {
            await InitializeAsync();
            using var context = CreateContext();
            var entity = await context.Personas.FirstOrDefaultAsync(p => p.Email == email);
            return entity.ToModel();
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al obtener persona por Email {Email}", email);
            return null;
        }
    }

    public async Task<bool> ExisteEmailAsync(string email) {
        try {
            await InitializeAsync();
            using var context = CreateContext();
            return await context.Personas.AnyAsync(p => p.Email == email);
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al verificar Email {Email}", email);
            return false;
        }
    }

    public async Task<Result<Persona, DomainError>> CreateAsync(Persona model) {
        await InitializeAsync();
        using var context = CreateContext();
        if (await context.Personas.AnyAsync(p => p.Dni == (model.Dni ?? "")))
            return Result.Failure<Persona, DomainError>(PersonaErrors.DniAlreadyExists(model.Dni ?? ""));

        if (await context.Personas.AnyAsync(p => p.Email == (model.Email ?? "")))
            return Result.Failure<Persona, DomainError>(PersonaErrors.EmailAlreadyExists(model.Email ?? ""));

        model = model with {
            Id = 0,
            FechaNacimiento = model.FechaNacimiento == default ? DateTime.UtcNow.AddYears(-18) : model.FechaNacimiento,
            Email = string.IsNullOrWhiteSpace(model.Email)
                ? $"{(model.Dni ?? "").ToLower()}@gestionacademica.local"
                : model.Email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false,
            DeletedAt = null
        };

        try {
            var entity = model.ToEntity();
            context.Personas.Add(entity);
            await context.SaveChangesAsync();
            using var readCtx = CreateContext();
            var created = await readCtx.Personas.AsNoTracking().FirstOrDefaultAsync(p => p.Dni == entity.Dni);
            return Result.Success<Persona, DomainError>(created.ToModel()!);
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al crear persona");
            return Result.Failure<Persona, DomainError>(PersonaErrors.DatabaseError(ex.Message));
        }
    }

    public async Task<Result<Persona, DomainError>> UpdateAsync(int id, Persona model) {
        try {
            await InitializeAsync();
            using var context = CreateContext();
            var entity = await context.Personas.FirstOrDefaultAsync(p => p.Id == id);
            if (entity == null)
                return Result.Failure<Persona, DomainError>(PersonaErrors.NotFound(id.ToString()));

            var existingModel = entity.ToModel();
            if (existingModel == null)
                return Result.Failure<Persona, DomainError>(PersonaErrors.NotFound(id.ToString()));

            if ((model.Dni ?? "") != (existingModel.Dni ?? "") &&
                await context.Personas.AnyAsync(p => p.Dni == (model.Dni ?? "") && p.Id != id))
                return Result.Failure<Persona, DomainError>(PersonaErrors.DniAlreadyExists(model.Dni ?? ""));

            var newEmail = string.IsNullOrWhiteSpace(model.Email) ? existingModel.Email ?? "" : model.Email;
            if (newEmail != (existingModel.Email ?? "") &&
                await context.Personas.AnyAsync(p => p.Email == newEmail && p.Id != id))
                return Result.Failure<Persona, DomainError>(PersonaErrors.EmailAlreadyExists(newEmail));

            entity.Dni = model.Dni ?? "";
            entity.Nombre = model.Nombre ?? "";
            entity.Apellidos = model.Apellidos ?? "";
            entity.FechaNacimiento = model.FechaNacimiento == default ? entity.FechaNacimiento : model.FechaNacimiento;
            entity.Email = newEmail;
            entity.Imagen = model.Imagen;
            entity.UpdatedAt = DateTime.UtcNow;

            if (model is Estudiante e) {
                entity.Tipo = "Estudiante";
                entity.Calificacion = e.Calificacion;
                entity.Ciclo = (int)e.Ciclo;
                entity.Curso = (int)e.Curso;
            }
            else if (model is Docente d) {
                entity.Tipo = "Docente";
                entity.Experiencia = d.Experiencia;
                entity.Especialidad = d.Especialidad;
                entity.Ciclo = (int)d.Ciclo;
            }

            await context.SaveChangesAsync();

            using var readCtx = CreateContext();
            var updated = await readCtx.Personas.AsNoTracking().FirstAsync(p => p.Id == id);
            return Result.Success<Persona, DomainError>(updated.ToModel()!);
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al actualizar persona");
            return Result.Failure<Persona, DomainError>(PersonaErrors.DatabaseError(ex.Message));
        }
    }

    public async Task<Persona?> DeleteAsync(int id, bool isLogical = true) {
        try {
            await InitializeAsync();
            using var context = CreateContext();
            var entity = await context.Personas.FirstOrDefaultAsync(p => p.Id == id);
            if (entity == null) return null;

            if (isLogical) {
                entity.IsDeleted = true;
                entity.DeletedAt = DateTime.UtcNow;
                entity.UpdatedAt = DateTime.UtcNow;
                await context.SaveChangesAsync();

                using var readCtx = CreateContext();
                return (await readCtx.Personas.AsNoTracking().FirstAsync(p => p.Id == id)).ToModel();
            }

            context.Personas.Remove(entity);
            await context.SaveChangesAsync();
            return entity.ToModel();
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al eliminar persona");
            return null;
        }
    }

    public async Task<bool> DeleteAllAsync() {
        try {
            await InitializeAsync();
            using var context = CreateContext();
            context.Personas.RemoveRange(context.Personas);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al eliminar todas las personas");
            return false;
        }
    }

    public async Task<int> CountEstudiantesAsync(bool includeDeleted = false) {
        try {
            await InitializeAsync();
            using var context = CreateContext();
            var query = includeDeleted
                ? context.Personas.Where(p => p.Tipo == "Estudiante")
                : context.Personas.Where(p => p.Tipo == "Estudiante" && !p.IsDeleted);
            return await query.CountAsync();
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al contar estudiantes");
            return 0;
        }
    }

    public async Task<int> CountDocentesAsync(bool includeDeleted = false) {
        try {
            await InitializeAsync();
            using var context = CreateContext();
            var query = includeDeleted
                ? context.Personas.Where(p => p.Tipo == "Docente")
                : context.Personas.Where(p => p.Tipo == "Docente" && !p.IsDeleted);
            return await query.CountAsync();
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al contar docentes");
            return 0;
        }
    }

    public async Task<IEnumerable<Estudiante>> GetEstudiantesOrderByAsync(string orden, int page = 1, int pageSize = 10, bool includeDeleted = true) {
        try {
            await InitializeAsync();
            using var context = CreateContext();
            var query = includeDeleted
                ? context.Personas.Where(p => p.Tipo == "Estudiante").AsNoTracking()
                : context.Personas.Where(p => p.Tipo == "Estudiante" && !p.IsDeleted).AsNoTracking();

            query = orden.ToLower() switch {
                "id" => query.OrderBy(p => p.Id),
                "dni" => query.OrderBy(p => p.Dni),
                "nombre" => query.OrderBy(p => p.Nombre),
                "apellidos" => query.OrderBy(p => p.Apellidos),
                "nota" => query.OrderByDescending(p => p.Calificacion),
                "ciclo" => query.OrderBy(p => p.Ciclo),
                "curso" => query.OrderBy(p => p.Curso),
                _ => query.OrderBy(p => p.Dni)
            };

            var entities = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return entities.ToModel().Cast<Estudiante>();
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al obtener estudiantes ordenados");
            return [];
        }
    }

    public async Task<IEnumerable<Docente>> GetDocentesOrderByAsync(string orden, int page = 1, int pageSize = 10, bool includeDeleted = true) {
        try {
            await InitializeAsync();
            using var context = CreateContext();
            var query = includeDeleted
                ? context.Personas.Where(p => p.Tipo == "Docente")
                : context.Personas.Where(p => p.Tipo == "Docente" && !p.IsDeleted);

            query = orden.ToLower() switch {
                "id" => query.OrderBy(p => p.Id),
                "dni" => query.OrderBy(p => p.Dni),
                "nombre" => query.OrderBy(p => p.Nombre),
                "apellidos" => query.OrderBy(p => p.Apellidos),
                "experiencia" => query.OrderByDescending(p => p.Experiencia),
                "ciclo" => query.OrderBy(p => p.Ciclo),
                "modulo" or "especialidad" => query.OrderBy(p => p.Especialidad),
                _ => query.OrderBy(p => p.Dni)
            };

            var entities = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return entities.ToModel().Cast<Docente>();
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al obtener docentes ordenados");
            return [];
        }
    }

    public async Task<Result<Persona, DomainError>> RestoreAsync(int id) {
        try {
            await InitializeAsync();
            using var context = CreateContext();
            var entity = await context.Personas.FindAsync(id);
            if (entity == null)
                return Result.Failure<Persona, DomainError>(PersonaErrors.NotFound(id.ToString()));

            entity.IsDeleted = false;
            entity.DeletedAt = null;
            entity.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();

            _logger.Information("Persona con ID {Id} restaurada correctamente", id);
            return Result.Success<Persona, DomainError>(entity.ToModel()!);
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al restaurar persona");
            return Result.Failure<Persona, DomainError>(PersonaErrors.DatabaseError(ex.Message));
        }
    }
}
