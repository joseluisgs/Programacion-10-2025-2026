using Microsoft.EntityFrameworkCore;
using CSharpFunctionalExtensions;
using GestionAcademica.Entity;
using GestionAcademica.Errors.Common;
using GestionAcademica.Errors.Personas;
using GestionAcademica.Mappers.Personas;
using GestionAcademica.Models.Personas;
using GestionAcademica.Repositories.Personas.Base;
using Serilog;

namespace GestionAcademica.Repositories.Personas.EfCore;

public class PersonasEfRepository : IPersonasRepository
{
    private readonly ILogger _logger = Log.ForContext<PersonasEfRepository>();
    private readonly AppDbContext _context;

    public PersonasEfRepository(AppDbContext context)
    {
        _context = context;
        if (_context.Database.CanConnect())
            _context.Database.EnsureCreated();
    }

    public IEnumerable<Persona> GetAll(int page = 1, int pageSize = 10, bool includeDeleted = true)
    {
        try
        {
            var query = includeDeleted 
                ? _context.Personas.AsQueryable()
                : _context.Personas.Where(p => !p.IsDeleted);

            var entities = query
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return PersonaMapper.ToModel(entities);
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
            var query = includeDeleted 
                ? _context.Personas.Where(p => p.Tipo == "Estudiante")
                : _context.Personas.Where(p => p.Tipo == "Estudiante" && !p.IsDeleted);

            var entities = query
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return PersonaMapper.ToModel(entities).Cast<Estudiante>();
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
            var query = includeDeleted 
                ? _context.Personas.Where(p => p.Tipo == "Docente")
                : _context.Personas.Where(p => p.Tipo == "Docente" && !p.IsDeleted);

            var entities = query
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return PersonaMapper.ToModel(entities).Cast<Docente>();
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
            var entity = _context.Personas.FirstOrDefault(p => p.Id == id);
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
            var entity = _context.Personas.FirstOrDefault(p => p.Dni == dni);
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
            return _context.Personas.Any(p => p.Dni == dni);
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
            var entity = _context.Personas.FirstOrDefault(p => p.Email == email);
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
            return _context.Personas.Any(p => p.Email == email);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al verificar Email {Email}", email);
            return false;
        }
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

        try
        {
            var entity = PersonaMapper.ToEntity(model);
            _context.Personas.Add(entity);
            _context.SaveChanges();

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
        var entity = _context.Personas.FirstOrDefault(p => p.Id == id);
        if (entity == null)
            return Result.Failure<Persona, DomainError>(PersonaErrors.NotFound(id.ToString()));

        var existingModel = PersonaMapper.ToModel(entity);
        if (existingModel == null)
            return Result.Failure<Persona, DomainError>(PersonaErrors.NotFound(id.ToString()));

        if (model.Dni != existingModel.Dni && _context.Personas.Any(p => p.Dni == model.Dni && p.Id != id))
            return Result.Failure<Persona, DomainError>(PersonaErrors.DniAlreadyExists(model.Dni));

        var newEmail = string.IsNullOrWhiteSpace(model.Email) ? existingModel.Email : model.Email;
        if (newEmail != existingModel.Email && _context.Personas.Any(p => p.Email == newEmail && p.Id != id))
            return Result.Failure<Persona, DomainError>(PersonaErrors.EmailAlreadyExists(newEmail));

        entity.Dni = model.Dni;
        entity.Nombre = model.Nombre;
        entity.Apellidos = model.Apellidos;
        entity.FechaNacimiento = model.FechaNacimiento == default ? entity.FechaNacimiento : model.FechaNacimiento;
        entity.Email = newEmail;
        entity.Imagen = model.Imagen;
        entity.UpdatedAt = DateTime.UtcNow;

        if (model is Estudiante e)
        {
            entity.Tipo = "Estudiante";
            entity.Calificacion = e.Calificacion;
            entity.Ciclo = (int)e.Ciclo;
            entity.Curso = (int)e.Curso;
        }
        else if (model is Docente d)
        {
            entity.Tipo = "Docente";
            entity.Experiencia = d.Experiencia;
            entity.Especialidad = d.Especialidad;
            entity.Ciclo = (int)d.Ciclo;
        }

        try
        {
            _context.SaveChanges();
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
            var entity = _context.Personas.FirstOrDefault(p => p.Id == id);
            if (entity == null)
                return null;

            if (isLogical)
            {
                entity.IsDeleted = true;
                entity.DeletedAt = DateTime.UtcNow;
                entity.UpdatedAt = DateTime.UtcNow;
                _context.SaveChanges();
                return GetById(id);
            }
            else
            {
                _context.Personas.Remove(entity);
                _context.SaveChanges();
                return PersonaMapper.ToModel(entity);
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
            _context.Personas.RemoveRange(_context.Personas);
            _context.SaveChanges();
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
            var query = includeDeleted
                ? _context.Personas.Where(p => p.Tipo == "Estudiante")
                : _context.Personas.Where(p => p.Tipo == "Estudiante" && !p.IsDeleted);
            return query.Count();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al contar estudiantes");
            return 0;
        }
    }

    public int CountDocentes(bool includeDeleted = false)
    {
        try
        {
            var query = includeDeleted
                ? _context.Personas.Where(p => p.Tipo == "Docente")
                : _context.Personas.Where(p => p.Tipo == "Docente" && !p.IsDeleted);
            return query.Count();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al contar docentes");
            return 0;
        }
    }
}
