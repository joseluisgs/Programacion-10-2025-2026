using CSharpFunctionalExtensions;
using GestionAcademica.Cache;
using GestionAcademica.Enums;
using GestionAcademica.Errors.Common;
using GestionAcademica.Errors.Personas;
using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Informes;
using GestionAcademica.Models.Personas;
using GestionAcademica.Repositories.Personas.Base;
using GestionAcademica.Validators.Common;
using Serilog;

namespace GestionAcademica.Services.Personas;

public class PersonasService(
    IPersonasRepository repository,
    IValidador<Persona> valPersona,
    IValidador<Persona> valEstudiante,
    IValidador<Persona> valDocente,
    ICache<int, Persona> cache
) : IPersonasService
{
    private readonly ILogger _logger = Log.ForContext<PersonasService>();

    public int TotalPersonas => repository.GetAll(1, int.MaxValue).Count();

    public IEnumerable<Persona> GetAll(int page = 1, int pageSize = 10, bool includeDeleted = true)
    {
        return repository.GetAll(page, pageSize, includeDeleted);
    }

    public IEnumerable<Estudiante> GetEstudiantesOrderBy(
        TipoOrdenamiento ordenamiento = TipoOrdenamiento.Dni,
        int page = 1, 
        int pageSize = 10, 
        bool includeDeleted = true)
    {
        // Obtenemos todos los estudiantes para poder ordenar correctamente antes de paginar
        var lista = repository.GetEstudiantes(1, int.MaxValue, includeDeleted);
            
        return AplicarOrdenamientoEstudiantes(lista, ordenamiento)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
    }

    public IEnumerable<Docente> GetDocentesOrderBy(
        TipoOrdenamiento ordenamiento = TipoOrdenamiento.Dni,
        int page = 1, 
        int pageSize = 10, 
        bool includeDeleted = true)
    {
        // Obtenemos todos los docentes para poder ordenar correctamente antes de paginar
        var lista = repository.GetDocentes(1, int.MaxValue, includeDeleted);

        return AplicarOrdenamientoDocentes(lista, ordenamiento)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
    }

    public IEnumerable<Persona> GetAllOrderBy(
        TipoOrdenamiento orden = TipoOrdenamiento.Dni,
        Predicate<Persona>? filtro = null,
        int page = 1,
        int pageSize = 10,
        bool includeDeleted = true)
    {
        var lista = filtro == null
            ? repository.GetAll(1, int.MaxValue, includeDeleted)
            : repository.GetAll(1, int.MaxValue, includeDeleted).Where(p => filtro(p));

        return AplicarOrdenamientoGeneral(lista, orden)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
    }

    private IEnumerable<Estudiante> AplicarOrdenamientoEstudiantes(IEnumerable<Estudiante> lista, TipoOrdenamiento orden)
    {
        return orden switch
        {
            TipoOrdenamiento.Dni => lista.OrderBy(p => p.Dni),
            TipoOrdenamiento.Nombre => lista.OrderBy(p => p.Nombre),
            TipoOrdenamiento.Apellidos => lista.OrderBy(p => p.Apellidos),
            TipoOrdenamiento.Nota => lista.OrderByDescending(p => p.Calificacion),
            _ => lista.OrderBy(p => p.Id)
        };
    }

    private IEnumerable<Docente> AplicarOrdenamientoDocentes(IEnumerable<Docente> lista, TipoOrdenamiento orden)
    {
        return orden switch
        {
            TipoOrdenamiento.Dni => lista.OrderBy(p => p.Dni),
            TipoOrdenamiento.Nombre => lista.OrderBy(p => p.Nombre),
            TipoOrdenamiento.Apellidos => lista.OrderBy(p => p.Apellidos),
            TipoOrdenamiento.Experiencia => lista.OrderByDescending(p => p.Experiencia),
            _ => lista.OrderBy(p => p.Id)
        };
    }

    private IEnumerable<Persona> AplicarOrdenamientoGeneral(IEnumerable<Persona> lista, TipoOrdenamiento orden)
    {
        return orden switch
        {
            TipoOrdenamiento.Dni => lista.OrderBy(p => p.Dni),
            TipoOrdenamiento.Nombre => lista.OrderBy(p => p.Nombre),
            TipoOrdenamiento.Apellidos => lista.OrderBy(p => p.Apellidos),
            _ => lista.OrderBy(p => p.Id)
        };
    }

    public Result<Persona, DomainError> GetById(int id)
    {
        if (cache.Get(id) is {} cached)
            return Result.Success<Persona, DomainError>(cached);
        
        if (repository.GetById(id) is {} persona)
        {
            cache.Add(id, persona);
            return Result.Success<Persona, DomainError>(persona);
        }
        
        return Result.Failure<Persona, DomainError>(PersonaErrors.NotFound(id.ToString()));
    }

    public Result<Persona, DomainError> GetByDni(string dni)
    {
        if (repository.GetByDni(dni) is {} persona)
            return Result.Success<Persona, DomainError>(persona);
        
        return Result.Failure<Persona, DomainError>(PersonaErrors.NotFound(dni));
    }

    public Result<Persona, DomainError> Save(Persona persona)
    {
        return ValidarPersona(persona)
            .Bind(_ => CheckDniNotExists(persona.Dni))
            .Bind(_ => CheckEmailNotExists(persona.Email))
            .Bind(p => repository.Create(p));
    }

    public Result<Persona, DomainError> Update(int id, Persona persona)
    {
        return CheckExists(id)
            .Bind(_ => ValidarPersona(persona))
            .Bind(_ => CheckDniNotExistsForUpdate(id, persona.Dni))
            .Bind(_ => CheckEmailNotExistsForUpdate(id, persona.Email))
            .Tap(_ => cache.Remove(id))
            .Bind(p => repository.Update(id, p));
    }

    public Result<Persona, DomainError> Delete(int id, bool isLogical = true)
    {
        return CheckExists(id)
            .Map(p =>
            {
                cache.Remove(id);
                return repository.Delete(id, isLogical)!;
            });
    }

    public bool DeleteAll()
    {
        _logger.Warning("Eliminando todas las personas del sistema");
        return repository.DeleteAll();
    }

    private Result<Persona, DomainError> ValidarPersona(Persona persona)
    {
        return persona switch
        {
            Estudiante => valPersona.Validar(persona)
                .Bind(_ => valEstudiante.Validar(persona)),
            Docente => valPersona.Validar(persona)
                .Bind(_ => valDocente.Validar(persona)),
            _ => Result.Failure<Persona, DomainError>(PersonaErrors.Validation(new[] { "Tipo de entidad no soportada." }))
        };
    }

    private Result<Persona, DomainError> CheckExists(int id)
    {
        if (repository.GetById(id) is {} exists)
            return Result.Success<Persona, DomainError>(exists);
        return Result.Failure<Persona, DomainError>(PersonaErrors.NotFound(id.ToString()));
    }

    private Result<Persona, DomainError> CheckDniNotExists(string dni)
    {
        if (repository.GetByDni(dni) is {} exists)
            return Result.Failure<Persona, DomainError>(PersonaErrors.DniAlreadyExists(dni));
        return Result.Success<Persona, DomainError>(null!);
    }

    private Result<Persona, DomainError> CheckDniNotExistsForUpdate(int id, string dni)
    {
        if (repository.GetByDni(dni) is {} exists && exists.Id != id)
            return Result.Failure<Persona, DomainError>(PersonaErrors.DniAlreadyExists(dni));
        return Result.Success<Persona, DomainError>(null!);
    }

    private Result<Persona, DomainError> CheckEmailNotExists(string email)
    {
        if (repository.GetByEmail(email) is {} exists)
            return Result.Failure<Persona, DomainError>(PersonaErrors.EmailAlreadyExists(email));
        return Result.Success<Persona, DomainError>(null!);
    }

    private Result<Persona, DomainError> CheckEmailNotExistsForUpdate(int id, string email)
    {
        if (repository.GetByEmail(email) is {} exists && exists.Id != id)
            return Result.Failure<Persona, DomainError>(PersonaErrors.EmailAlreadyExists(email));
        return Result.Success<Persona, DomainError>(null!);
    }
}