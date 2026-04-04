using System.Data;
using Microsoft.Data.Sqlite;
using Dapper;
using CSharpFunctionalExtensions;
using GestionAcademica.Entity;
using GestionAcademica.Errors.Common;
using GestionAcademica.Errors.Personas;
using GestionAcademica.Mappers.Personas;
using GestionAcademica.Models.Personas;
using GestionAcademica.Repositories.Personas.Base;
using Serilog;

namespace GestionAcademica.Repositories.Personas.Dapper;

/// <summary>
/// Repositorio de personas que utiliza Dapper con SQLite.
/// Persiste los datos en una base de datos SQLite.
/// </summary>
public class PersonasDapperRepository : IPersonasRepository
{
    private readonly ILogger _logger = Log.ForContext<PersonasDapperRepository>();
    private readonly IDbConnection _connection;
    private readonly Action? _onDispose;

    public PersonasDapperRepository(IDbConnection connection, Action? onDispose = null, bool dropData = false, bool seedData = false)
    {
        _connection = connection;
        _onDispose = onDispose;
        EnsureTable(dropData);
        
        if (seedData && CountTotal() == 0)
        {
            Seed();
        }
    }

    private void EnsureTable(bool dropData)
    {
        if (_connection.State != ConnectionState.Open)
            _connection.Open();
        
        if (dropData)
        {
            _connection.Execute("DROP TABLE IF EXISTS Personas");
        }
            
        _connection.Execute(@"
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

    private int CountTotal()
    {
        return _connection.ExecuteScalar<int>("SELECT COUNT(1) FROM Personas");
    }

    private void Seed()
    {
        foreach (var p in GestionAcademica.Factories.Personas.PersonasFactory.Seed())
        {
            Create(p);
        }
    }

    public IEnumerable<Persona> GetAll(int page = 1, int pageSize = 10, bool includeDeleted = true)
    {
        try
        {
            var sql = includeDeleted 
                ? "SELECT * FROM Personas ORDER BY Id LIMIT @PageSize OFFSET @Offset"
                : "SELECT * FROM Personas WHERE IsDeleted = 0 ORDER BY Id LIMIT @PageSize OFFSET @Offset";
            
            var entities = _connection.Query<PersonaEntity>(sql, new { PageSize = pageSize, Offset = (page - 1) * pageSize }).ToList();
            return entities.Select(PersonaMapper.ToModel).OfType<Persona>().ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al obtener personas");
            return Enumerable.Empty<Persona>();
        }
    }

    public IEnumerable<Estudiante> GetEstudiantes(int page = 1, int pageSize = 10, bool includeDeleted = true)
    {
        try
        {
            var sql = includeDeleted 
                ? "SELECT * FROM Personas WHERE Tipo = 'Estudiante' ORDER BY Id LIMIT @PageSize OFFSET @Offset"
                : "SELECT * FROM Personas WHERE Tipo = 'Estudiante' AND IsDeleted = 0 ORDER BY Id LIMIT @PageSize OFFSET @Offset";
            
            var entities = _connection.Query<PersonaEntity>(sql, new { PageSize = pageSize, Offset = (page - 1) * pageSize }).ToList();
            return entities.Select(PersonaMapper.ToModel).OfType<Estudiante>().ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al obtener estudiantes");
            return Enumerable.Empty<Estudiante>();
        }
    }

    public IEnumerable<Docente> GetDocentes(int page = 1, int pageSize = 10, bool includeDeleted = true)
    {
        try
        {
            var sql = includeDeleted 
                ? "SELECT * FROM Personas WHERE Tipo = 'Docente' ORDER BY Id LIMIT @PageSize OFFSET @Offset"
                : "SELECT * FROM Personas WHERE Tipo = 'Docente' AND IsDeleted = 0 ORDER BY Id LIMIT @PageSize OFFSET @Offset";
            
            var entities = _connection.Query<PersonaEntity>(sql, new { PageSize = pageSize, Offset = (page - 1) * pageSize }).ToList();
            return entities.Select(PersonaMapper.ToModel).OfType<Docente>().ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al obtener docentes");
            return Enumerable.Empty<Docente>();
        }
    }

    public Persona? GetById(int id)
    {
        try
        {
            var sql = "SELECT * FROM Personas WHERE Id = @Id";
            var entity = _connection.QueryFirstOrDefault<PersonaEntity>(sql, new { Id = id });
            return PersonaMapper.ToModel(entity);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al obtener persona por ID {Id}", id);
            return null;
        }
    }

    public Persona? GetByDni(string dni)
    {
        try
        {
            var sql = "SELECT * FROM Personas WHERE Dni = @Dni";
            var entity = _connection.QueryFirstOrDefault<PersonaEntity>(sql, new { Dni = dni });
            return PersonaMapper.ToModel(entity);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al obtener persona por DNI {Dni}", dni);
            return null;
        }
    }

    public bool ExisteDni(string dni)
    {
        try
        {
            var sql = "SELECT COUNT(1) FROM Personas WHERE Dni = @Dni";
            return _connection.ExecuteScalar<int>(sql, new { Dni = dni }) > 0;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al verificar DNI {Dni}", dni);
            return false;
        }
    }

    public Persona? GetByEmail(string email)
    {
        try
        {
            var sql = "SELECT * FROM Personas WHERE Email = @Email";
            var entity = _connection.QueryFirstOrDefault<PersonaEntity>(sql, new { Email = email });
            return PersonaMapper.ToModel(entity);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al obtener persona por Email {Email}", email);
            return null;
        }
    }

    public bool ExisteEmail(string email)
    {
        try
        {
            var sql = "SELECT COUNT(1) FROM Personas WHERE Email = @Email";
            return _connection.ExecuteScalar<int>(sql, new { Email = email }) > 0;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al verificar Email {Email}", email);
            return false;
        }
    }

    public Result<Persona, DomainError> Create(Persona model)
    {
        if (ExisteDni(model.Dni ?? ""))
            return Result.Failure<Persona, DomainError>(PersonaErrors.DniAlreadyExists(model.Dni ?? ""));

        if (ExisteEmail(model.Email ?? ""))
            return Result.Failure<Persona, DomainError>(PersonaErrors.EmailAlreadyExists(model.Email ?? ""));

        model = model with
        {
            Id = 0,
            FechaNacimiento = model.FechaNacimiento == default ? DateTime.UtcNow.AddYears(-18) : model.FechaNacimiento,
            Email = string.IsNullOrWhiteSpace(model.Email) ? $"{(model.Dni ?? "").ToLower()}@gestionacademica.local" : model.Email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false,
            DeletedAt = null
        };

        var entity = PersonaMapper.ToEntity(model);

        try
        {
            var sql = @"INSERT INTO Personas (Dni, Nombre, Apellidos, FechaNacimiento, Email, Imagen, CreatedAt, UpdatedAt, IsDeleted, DeletedAt, Tipo, Calificacion, Ciclo, Curso, Experiencia, Especialidad)
                        VALUES (@Dni, @Nombre, @Apellidos, @FechaNacimiento, @Email, @Imagen, @CreatedAt, @UpdatedAt, @IsDeleted, @DeletedAt, @Tipo, @Calificacion, @Ciclo, @Curso, @Experiencia, @Especialidad);
                        SELECT last_insert_rowid();";

            entity.Id = _connection.ExecuteScalar<int>(sql, new
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
                DeletedAt = entity.DeletedAt,
                entity.Tipo,
                entity.Calificacion,
                entity.Ciclo,
                entity.Curso,
                entity.Experiencia,
                entity.Especialidad
            });

            return Result.Success<Persona, DomainError>(GetById(entity.Id)!);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al crear persona");
            return Result.Failure<Persona, DomainError>(PersonaErrors.DatabaseError(ex.Message));
        }
    }

    public Result<Persona, DomainError> Update(int id, Persona model)
    {
        var existing = GetById(id);
        if (existing == null)
            return Result.Failure<Persona, DomainError>(PersonaErrors.NotFound(id.ToString()));

        if ((model.Dni ?? "") != (existing.Dni ?? "") && ExisteDni(model.Dni ?? ""))
            return Result.Failure<Persona, DomainError>(PersonaErrors.DniAlreadyExists(model.Dni ?? ""));

        var newEmail = string.IsNullOrWhiteSpace(model.Email) ? (existing.Email ?? "") : model.Email;
        if (newEmail != (existing.Email ?? "") && ExisteEmail(newEmail))
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

        var entity = PersonaMapper.ToEntity(model);

        try
        {
            var sql = @"UPDATE Personas SET 
                        Dni = @Dni, Nombre = @Nombre, Apellidos = @Apellidos, FechaNacimiento = @FechaNacimiento, Email = @Email, Imagen = @Imagen,
                        UpdatedAt = @UpdatedAt, IsDeleted = @IsDeleted, DeletedAt = @DeletedAt,
                        Tipo = @Tipo, Calificacion = @Calificacion, Ciclo = @Ciclo, Curso = @Curso, Experiencia = @Experiencia, Especialidad = @Especialidad
                        WHERE Id = @Id";

            _connection.Execute(sql, new
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
                DeletedAt = entity.DeletedAt,
                entity.Tipo,
                entity.Calificacion,
                entity.Ciclo,
                entity.Curso,
                entity.Experiencia,
                entity.Especialidad
            });

            return Result.Success<Persona, DomainError>(GetById(id)!);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al actualizar persona");
            return Result.Failure<Persona, DomainError>(PersonaErrors.DatabaseError(ex.Message));
        }
    }

    public Persona? Delete(int id, bool isLogical = true)
    {
        try
        {
            var existing = GetById(id);
            if (existing == null)
                return null;

            if (isLogical)
            {
                var sql = "UPDATE Personas SET IsDeleted = 1, DeletedAt = @DeletedAt, UpdatedAt = @UpdatedAt WHERE Id = @Id";
                _connection.Execute(sql, new { Id = id, DeletedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
                return GetById(id);
            }
            else
            {
                var sql = "DELETE FROM Personas WHERE Id = @Id";
                _connection.Execute(sql, new { Id = id });
                return existing;
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
            _connection.Execute("DELETE FROM Personas");
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
        try
        {
            var sql = includeDeleted 
                ? "SELECT COUNT(1) FROM Personas WHERE Tipo = 'Estudiante'"
                : "SELECT COUNT(1) FROM Personas WHERE Tipo = 'Estudiante' AND IsDeleted = 0";
            return _connection.ExecuteScalar<int>(sql);
        }
        catch
        {
            return 0;
        }
    }

    public Result<Persona, DomainError> Restore(int id)
    {
        try
        {
            var existing = GetById(id);
            if (existing == null)
                return Result.Failure<Persona, DomainError>(PersonaErrors.NotFound(id.ToString()));

            var sql = "UPDATE Personas SET IsDeleted = 0, DeletedAt = NULL, UpdatedAt = @UpdatedAt WHERE Id = @Id";
            _connection.Execute(sql, new { Id = id, UpdatedAt = DateTime.UtcNow });
            
            _logger.Information("Persona con ID {Id} restaurada correctamente", id);
            return Result.Success<Persona, DomainError>(GetById(id)!);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al restaurar persona");
            return Result.Failure<Persona, DomainError>(PersonaErrors.DatabaseError(ex.Message));
        }
    }

    public int CountDocentes(bool includeDeleted = false)
    {
        try
        {
            var sql = includeDeleted 
                ? "SELECT COUNT(1) FROM Personas WHERE Tipo = 'Docente'"
                : "SELECT COUNT(1) FROM Personas WHERE Tipo = 'Docente' AND IsDeleted = 0";
            return _connection.ExecuteScalar<int>(sql);
        }
        catch
        {
            return 0;
        }
    }
}
