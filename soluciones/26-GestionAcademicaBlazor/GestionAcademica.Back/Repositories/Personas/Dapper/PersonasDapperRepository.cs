using System.Data;
using CSharpFunctionalExtensions;
using Dapper;
using GestionAcademica.Entity;
using GestionAcademica.Errors.Common;
using GestionAcademica.Errors.Personas;
using GestionAcademica.Factories.Personas;
using GestionAcademica.Mappers.Personas;
using GestionAcademica.Models.Personas;
using GestionAcademica.Repositories.Personas.Base;
using Serilog;

namespace GestionAcademica.Repositories.Personas.Dapper;

public class PersonasDapperRepository : IPersonasRepository
{
    private readonly IDbConnection _connection;
    private readonly ILogger _logger = Log.ForContext<PersonasDapperRepository>();
    private readonly Action? _onDispose;

    public PersonasDapperRepository(IDbConnection connection, Action? onDispose = null, bool dropData = false,
        bool seedData = false)
    {
        _connection = connection;
        _onDispose = onDispose;
        _ = InitializeAsync(dropData, seedData);
    }

    private async Task InitializeAsync(bool dropData, bool seedData)
    {
        await EnsureTableAsync(dropData);
        if (seedData && await CountTotalAsync() == 0)
            await SeedAsync();
    }

    public async Task<IEnumerable<Persona>> GetAllAsync(int page = 1, int pageSize = 10, bool includeDeleted = true)
    {
        try
        {
            var sql = includeDeleted
                ? "SELECT * FROM Personas ORDER BY Id LIMIT @PageSize OFFSET @Offset"
                : "SELECT * FROM Personas WHERE IsDeleted = 0 ORDER BY Id LIMIT @PageSize OFFSET @Offset";

            var entities = (await _connection
                .QueryAsync<PersonaEntity>(sql, new { PageSize = pageSize, Offset = (page - 1) * pageSize })).ToList();
            return entities.Select(PersonaMapper.ToModel).OfType<Persona>().ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al obtener personas");
            return [];
        }
    }

    public async Task<IEnumerable<Estudiante>> GetEstudiantesAsync(int page = 1, int pageSize = 10, bool includeDeleted = true)
    {
        try
        {
            var sql = includeDeleted
                ? "SELECT * FROM Personas WHERE Tipo = 'Estudiante' ORDER BY Id LIMIT @PageSize OFFSET @Offset"
                : "SELECT * FROM Personas WHERE Tipo = 'Estudiante' AND IsDeleted = 0 ORDER BY Id LIMIT @PageSize OFFSET @Offset";

            var entities = (await _connection
                .QueryAsync<PersonaEntity>(sql, new { PageSize = pageSize, Offset = (page - 1) * pageSize })).ToList();
            return entities.Select(PersonaMapper.ToModel).OfType<Estudiante>().ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al obtener estudiantes");
            return [];
        }
    }

    public async Task<IEnumerable<Docente>> GetDocentesAsync(int page = 1, int pageSize = 10, bool includeDeleted = true)
    {
        try
        {
            var sql = includeDeleted
                ? "SELECT * FROM Personas WHERE Tipo = 'Docente' ORDER BY Id LIMIT @PageSize OFFSET @Offset"
                : "SELECT * FROM Personas WHERE Tipo = 'Docente' AND IsDeleted = 0 ORDER BY Id LIMIT @PageSize OFFSET @Offset";

            var entities = (await _connection
                .QueryAsync<PersonaEntity>(sql, new { PageSize = pageSize, Offset = (page - 1) * pageSize })).ToList();
            return entities.Select(PersonaMapper.ToModel).OfType<Docente>().ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al obtener docentes");
            return [];
        }
    }

    public async Task<Persona?> GetByIdAsync(int id)
    {
        try
        {
            var sql = "SELECT * FROM Personas WHERE Id = @Id";
            var entity = await _connection.QueryFirstOrDefaultAsync<PersonaEntity>(sql, new { Id = id });
            return entity.ToModel();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al obtener persona por ID {Id}", id);
            return null;
        }
    }

    public async Task<Persona?> GetByDniAsync(string dni)
    {
        try
        {
            var sql = "SELECT * FROM Personas WHERE Dni = @Dni";
            var entity = await _connection.QueryFirstOrDefaultAsync<PersonaEntity>(sql, new { Dni = dni });
            return entity.ToModel();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al obtener persona por DNI {Dni}", dni);
            return null;
        }
    }

    public async Task<bool> ExisteDniAsync(string dni)
    {
        try
        {
            var sql = "SELECT COUNT(1) FROM Personas WHERE Dni = @Dni";
            return await _connection.ExecuteScalarAsync<int>(sql, new { Dni = dni }) > 0;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al verificar DNI {Dni}", dni);
            return false;
        }
    }

    public async Task<Persona?> GetByEmailAsync(string email)
    {
        try
        {
            var sql = "SELECT * FROM Personas WHERE Email = @Email";
            var entity = await _connection.QueryFirstOrDefaultAsync<PersonaEntity>(sql, new { Email = email });
            return entity.ToModel();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al obtener persona por Email {Email}", email);
            return null;
        }
    }

    public async Task<bool> ExisteEmailAsync(string email)
    {
        try
        {
            var sql = "SELECT COUNT(1) FROM Personas WHERE Email = @Email";
            return await _connection.ExecuteScalarAsync<int>(sql, new { Email = email }) > 0;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al verificar Email {Email}", email);
            return false;
        }
    }

    public async Task<Result<Persona, DomainError>> CreateAsync(Persona model)
    {
        if (await ExisteDniAsync(model.Dni ?? ""))
            return Result.Failure<Persona, DomainError>(PersonaErrors.DniAlreadyExists(model.Dni ?? ""));

        if (await ExisteEmailAsync(model.Email ?? ""))
            return Result.Failure<Persona, DomainError>(PersonaErrors.EmailAlreadyExists(model.Email ?? ""));

        model = model with
        {
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

        var entity = model.ToEntity();

        try
        {
            var sql =
                @"INSERT INTO Personas (Dni, Nombre, Apellidos, FechaNacimiento, Email, Imagen, CreatedAt, UpdatedAt, IsDeleted, DeletedAt, Tipo, Calificacion, Ciclo, Curso, Experiencia, Especialidad)
                        VALUES (@Dni, @Nombre, @Apellidos, @FechaNacimiento, @Email, @Imagen, @CreatedAt, @UpdatedAt, @IsDeleted, @DeletedAt, @Tipo, @Calificacion, @Ciclo, @Curso, @Experiencia, @Especialidad);
                        SELECT last_insert_rowid();";

            entity.Id = await _connection.ExecuteScalarAsync<int>(sql, new
            {
                Dni = entity.Dni ?? "",
                entity.Nombre,
                entity.Apellidos,
                FechaNacimiento = entity.FechaNacimiento.ToString("o"),
                Email = entity.Email ?? "",
                entity.Imagen,
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.IsDeleted,
                entity.DeletedAt,
                entity.Tipo,
                entity.Calificacion,
                entity.Ciclo,
                entity.Curso,
                entity.Experiencia,
                entity.Especialidad
            });

            return Result.Success<Persona, DomainError>((await GetByIdAsync(entity.Id))!);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al crear persona");
            return Result.Failure<Persona, DomainError>(PersonaErrors.DatabaseError(ex.Message));
        }
    }

    public async Task<Result<Persona, DomainError>> UpdateAsync(int id, Persona model)
    {
        var existing = await GetByIdAsync(id);
        if (existing == null)
            return Result.Failure<Persona, DomainError>(PersonaErrors.NotFound(id.ToString()));

        if ((model.Dni ?? "") != (existing.Dni ?? "") && await ExisteDniAsync(model.Dni ?? ""))
            return Result.Failure<Persona, DomainError>(PersonaErrors.DniAlreadyExists(model.Dni ?? ""));

        var newEmail = string.IsNullOrWhiteSpace(model.Email) ? existing.Email ?? "" : model.Email;
        if (newEmail != (existing.Email ?? "") && await ExisteEmailAsync(newEmail))
            return Result.Failure<Persona, DomainError>(PersonaErrors.EmailAlreadyExists(newEmail));

        model = model with
        {
            Id = id,
            FechaNacimiento = model.FechaNacimiento == default ? existing.FechaNacimiento : model.FechaNacimiento,
            Email = newEmail,
            CreatedAt = existing.CreatedAt,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = existing.IsDeleted,
            DeletedAt = existing.DeletedAt
        };

        var entity = model.ToEntity();

        try
        {
            var sql = @"UPDATE Personas SET 
                        Dni = @Dni, Nombre = @Nombre, Apellidos = @Apellidos, FechaNacimiento = @FechaNacimiento, Email = @Email, Imagen = @Imagen,
                        UpdatedAt = @UpdatedAt, IsDeleted = @IsDeleted, DeletedAt = @DeletedAt,
                        Tipo = @Tipo, Calificacion = @Calificacion, Ciclo = @Ciclo, Curso = @Curso, Experiencia = @Experiencia, Especialidad = @Especialidad
                        WHERE Id = @Id";

            await _connection.ExecuteAsync(sql, new
            {
                Id = id,
                Dni = entity.Dni ?? "",
                entity.Nombre,
                entity.Apellidos,
                FechaNacimiento = entity.FechaNacimiento.ToString("o"),
                Email = entity.Email ?? "",
                entity.Imagen,
                entity.UpdatedAt,
                entity.IsDeleted,
                entity.DeletedAt,
                entity.Tipo,
                entity.Calificacion,
                entity.Ciclo,
                entity.Curso,
                entity.Experiencia,
                entity.Especialidad
            });

            return Result.Success<Persona, DomainError>((await GetByIdAsync(id))!);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al actualizar persona");
            return Result.Failure<Persona, DomainError>(PersonaErrors.DatabaseError(ex.Message));
        }
    }

    public async Task<Persona?> DeleteAsync(int id, bool isLogical = true)
    {
        try
        {
            var existing = await GetByIdAsync(id);
            if (existing == null)
                return null;

            if (isLogical)
            {
                var sql =
                    "UPDATE Personas SET IsDeleted = 1, DeletedAt = @DeletedAt, UpdatedAt = @UpdatedAt WHERE Id = @Id";
                await _connection.ExecuteAsync(sql, new { Id = id, DeletedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
                return await GetByIdAsync(id);
            }
            else
            {
                var sql = "DELETE FROM Personas WHERE Id = @Id";
                await _connection.ExecuteAsync(sql, new { Id = id });
                return existing;
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al eliminar persona");
            return null;
        }
    }

    public async Task<bool> DeleteAllAsync()
    {
        try
        {
            await _connection.ExecuteAsync("DELETE FROM Personas");
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al eliminar todas las personas");
            return false;
        }
    }

    public async Task<int> CountEstudiantesAsync(bool includeDeleted = false)
    {
        try
        {
            var sql = includeDeleted
                ? "SELECT COUNT(1) FROM Personas WHERE Tipo = 'Estudiante'"
                : "SELECT COUNT(1) FROM Personas WHERE Tipo = 'Estudiante' AND IsDeleted = 0";
            return await _connection.ExecuteScalarAsync<int>(sql);
        }
        catch
        {
            return 0;
        }
    }

    public async Task<int> CountDocentesAsync(bool includeDeleted = false)
    {
        try
        {
            var sql = includeDeleted
                ? "SELECT COUNT(1) FROM Personas WHERE Tipo = 'Docente'"
                : "SELECT COUNT(1) FROM Personas WHERE Tipo = 'Docente' AND IsDeleted = 0";
            return await _connection.ExecuteScalarAsync<int>(sql);
        }
        catch
        {
            return 0;
        }
    }

    public async Task<IEnumerable<Estudiante>> GetEstudiantesOrderByAsync(string orden, int page = 1, int pageSize = 10, bool includeDeleted = true)
    {
        var where = includeDeleted ? "WHERE Tipo = 'Estudiante'" : "WHERE Tipo = 'Estudiante' AND IsDeleted = 0";
        var orderBy = orden.ToLower() switch
        {
            "id" => "Id",
            "dni" => "Dni",
            "nombre" => "Nombre",
            "apellidos" => "Apellidos",
            "nota" => "Calificacion DESC",
            "ciclo" => "Ciclo",
            "curso" => "Curso",
            _ => "Dni"
        };
        var offset = (page - 1) * pageSize;
        var sql = $"SELECT * FROM Personas {where} ORDER BY {orderBy} LIMIT {pageSize} OFFSET {offset}";
        var entities = await _connection.QueryAsync<PersonaEntity>(sql);
        return entities.ToModel().Cast<Estudiante>();
    }

    public async Task<IEnumerable<Docente>> GetDocentesOrderByAsync(string orden, int page = 1, int pageSize = 10, bool includeDeleted = true)
    {
        var where = includeDeleted ? "WHERE Tipo = 'Docente'" : "WHERE Tipo = 'Docente' AND IsDeleted = 0";
        var orderBy = orden.ToLower() switch
        {
            "id" => "Id",
            "dni" => "Dni",
            "nombre" => "Nombre",
            "apellidos" => "Apellidos",
            "experiencia" => "Experiencia DESC",
            "ciclo" => "Ciclo",
            "modulo" => "Especialidad",
            _ => "Dni"
        };
        var offset = (page - 1) * pageSize;
        var sql = $"SELECT * FROM Personas {where} ORDER BY {orderBy} LIMIT {pageSize} OFFSET {offset}";
        var entities = await _connection.QueryAsync<PersonaEntity>(sql);
        return entities.ToModel().Cast<Docente>();
    }

    public async Task<Result<Persona, DomainError>> RestoreAsync(int id)
    {
        try
        {
            var existing = await GetByIdAsync(id);
            if (existing == null)
                return Result.Failure<Persona, DomainError>(PersonaErrors.NotFound(id.ToString()));

            var sql = "UPDATE Personas SET IsDeleted = 0, DeletedAt = NULL, UpdatedAt = @UpdatedAt WHERE Id = @Id";
            await _connection.ExecuteAsync(sql, new { Id = id, UpdatedAt = DateTime.UtcNow });

            _logger.Information("Persona con ID {Id} restaurada correctamente", id);
            return Result.Success<Persona, DomainError>((await GetByIdAsync(id))!);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al restaurar persona");
            return Result.Failure<Persona, DomainError>(PersonaErrors.DatabaseError(ex.Message));
        }
    }

    private async Task EnsureTableAsync(bool dropData)
    {
        if (_connection.State != ConnectionState.Open)
            _connection.Open();

        if (dropData) await _connection.ExecuteAsync("DROP TABLE IF EXISTS Personas");

        await _connection.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS Personas (
                Id INTEGER PRIMARY KEY,
                Dni TEXT NOT NULL UNIQUE,
                Nombre TEXT NOT NULL,
                Apellidos TEXT NOT NULL,
                FechaNacimiento TEXT NOT NULL,
                Email TEXT NOT NULL UNIQUE,
                Imagen TEXT,
                CreatedAt TEXT NOT NULL,
                UpdatedAt TEXT NOT NULL,
                IsDeleted INTEGER NOT NULL DEFAULT 0,
                DeletedAt TEXT,
                Tipo TEXT NOT NULL DEFAULT 'Persona',
                Calificacion REAL,
                Ciclo INTEGER,
                Curso INTEGER,
                Experiencia INTEGER,
                Especialidad TEXT
            )");
    }

    private async Task<int> CountTotalAsync()
    {
        return await _connection.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM Personas");
    }

    private async Task SeedAsync()
    {
        foreach (var p in PersonasFactory.Seed()) await CreateAsync(p);
    }
}
