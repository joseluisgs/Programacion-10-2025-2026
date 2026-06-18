using CSharpFunctionalExtensions;
using GestionAcademica.Cache;
using GestionAcademica.Enums;
using GestionAcademica.Errors.Common;
using GestionAcademica.Errors.Personas;
using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Personas;
using GestionAcademica.Repositories.Personas.Base;
using GestionAcademica.Services.Images;
using GestionAcademica.Validators.Common;
using Serilog;

namespace GestionAcademica.Services.Personas;

/// <summary>
///     Implementación del servicio de gestión de personas.
/// </summary>
/// <remarks>
///     Utiliza dos cachés LRU: una por ID (int) y otra por DNI (string) para optimizar lecturas.
/// </remarks>
public class PersonasService(
    IPersonasRepository repository,
    IValidador<Persona> valPersona,
    IValidador<Estudiante> valEstudiante,
    IValidador<Docente> valDocente,
    ICache<int, Persona> cacheById,
    ICache<string, Persona> cacheByDni,
    IImageService imageService
) : IPersonasService {
    private readonly ILogger _logger = Log.ForContext<PersonasService>();

    public async Task<int> TotalPersonasAsync()
    {
        var personas = await repository.GetAllAsync(1, int.MaxValue);
        return personas.Count();
    }

    public async Task<IEnumerable<Persona>> GetAllAsync(int page = 1, int pageSize = 10, bool includeDeleted = true)
    {
        return await repository.GetAllAsync(page, pageSize, includeDeleted);
    }

    public async Task<IEnumerable<Estudiante>> GetEstudiantesOrderByAsync(
        TipoOrdenamiento ordenamiento = TipoOrdenamiento.Dni,
        int page = 1,
        int pageSize = 10,
        bool includeDeleted = true)
    {
        var orden = ordenamiento switch {
            TipoOrdenamiento.Id => "id",
            TipoOrdenamiento.Dni => "dni",
            TipoOrdenamiento.Nombre => "nombre",
            TipoOrdenamiento.Apellidos => "apellidos",
            TipoOrdenamiento.Nota => "nota",
            TipoOrdenamiento.Ciclo => "ciclo",
            TipoOrdenamiento.Curso => "curso",
            _ => "dni"
        };
        return await repository.GetEstudiantesOrderByAsync(orden, page, pageSize, includeDeleted);
    }

    public async Task<IEnumerable<Docente>> GetDocentesOrderByAsync(
        TipoOrdenamiento ordenamiento = TipoOrdenamiento.Dni,
        int page = 1,
        int pageSize = 10,
        bool includeDeleted = true)
    {
        var orden = ordenamiento switch {
            TipoOrdenamiento.Id => "id",
            TipoOrdenamiento.Dni => "dni",
            TipoOrdenamiento.Nombre => "nombre",
            TipoOrdenamiento.Apellidos => "apellidos",
            TipoOrdenamiento.Experiencia => "experiencia",
            TipoOrdenamiento.Ciclo => "ciclo",
            TipoOrdenamiento.Modulo => "modulo",
            _ => "dni"
        };
        return await repository.GetDocentesOrderByAsync(orden, page, pageSize, includeDeleted);
    }

    public async Task<IEnumerable<Persona>> GetAllOrderByAsync(
        TipoOrdenamiento orden = TipoOrdenamiento.Dni,
        Predicate<Persona>? filtro = null,
        int page = 1,
        int pageSize = 10,
        bool includeDeleted = true)
    {
        var todas = await repository.GetAllAsync(1, int.MaxValue, includeDeleted);
        var lista = filtro == null
            ? todas
            : todas.Where(p => filtro(p));

        return AplicarOrdenamientoGeneral(lista, orden)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
    }

    public async Task<Result<Persona, DomainError>> GetByIdAsync(int id)
    {
        if (cacheById.Get(id) is { } cached)
            return Result.Success<Persona, DomainError>(cached);

        var persona = await repository.GetByIdAsync(id);
        if (persona is not null)
        {
            cacheById.Add(id, persona);
            return Result.Success<Persona, DomainError>(persona);
        }

        return Result.Failure<Persona, DomainError>(PersonaErrors.NotFound(id.ToString()));
    }

    public async Task<Result<Persona, DomainError>> GetByDniAsync(string dni)
    {
        if (cacheByDni.Get(dni) is { } cached)
            return Result.Success<Persona, DomainError>(cached);

        var persona = await repository.GetByDniAsync(dni);
        if (persona is not null)
        {
            cacheByDni.Add(dni, persona);
            return Result.Success<Persona, DomainError>(persona);
        }

        return Result.Failure<Persona, DomainError>(PersonaErrors.NotFound(dni));
    }

    public async Task<Result<Persona, DomainError>> SaveAsync(Persona persona)
    {
        var validationResult = await ValidarPersonaAsync(persona);
        if (validationResult.IsFailure)
            return validationResult;

        var existsDni = await repository.ExisteDniAsync(persona.Dni ?? "");
        if (existsDni)
            return Result.Failure<Persona, DomainError>(PersonaErrors.DniAlreadyExists(persona.Dni ?? ""));

        var existsEmail = await repository.ExisteEmailAsync(persona.Email ?? "");
        if (existsEmail)
            return Result.Failure<Persona, DomainError>(PersonaErrors.EmailAlreadyExists(persona.Email ?? ""));

        var created = await repository.CreateAsync(persona);
        if (created.IsSuccess)
        {
            cacheById.Add(created.Value.Id, created.Value);
            cacheByDni.Add(created.Value.Dni, created.Value);
        }
        return created;
    }

    public async Task<Result<Persona, DomainError>> UpdateAsync(int id, Persona persona)
    {
        var checkResult = await CheckExistsAsync(id);
        if (checkResult.IsFailure)
            return checkResult;

        var pOriginal = checkResult.Value;

        if (!string.IsNullOrEmpty(pOriginal.Imagen) && pOriginal.Imagen != persona.Imagen)
        {
            _logger.Debug("Eliminando imagen huérfana {FileName} por actualización", pOriginal.Imagen);
            _ = await imageService.DeleteImageAsync(pOriginal.Imagen);
        }

        cacheById.Remove(id);
        cacheByDni.Remove(pOriginal.Dni);

        var validationResult = await ValidarPersonaAsync(persona);
        if (validationResult.IsFailure)
            return validationResult;

        var dniValid = await IsDniValidForUpdateAsync(id, persona.Dni ?? "");
        if (!dniValid)
            return Result.Failure<Persona, DomainError>(PersonaErrors.DniAlreadyExists(persona.Dni ?? ""));

        var emailValid = await IsEmailValidForUpdateAsync(id, persona.Email ?? "");
        if (!emailValid)
            return Result.Failure<Persona, DomainError>(PersonaErrors.EmailAlreadyExists(persona.Email ?? ""));

        return await repository.UpdateAsync(id, persona);
    }

    public async Task<Result<Persona, DomainError>> DeleteAsync(int id, bool isLogical = true)
    {
        var checkResult = await CheckExistsAsync(id);
        if (checkResult.IsFailure)
            return checkResult;

        var p = checkResult.Value;

        if (!isLogical && !string.IsNullOrEmpty(p.Imagen))
        {
            _logger.Debug("Eliminando imagen {FileName} del disco por borrado físico", p.Imagen);
            _ = await imageService.DeleteImageAsync(p.Imagen);
        }

        cacheById.Remove(id);
        cacheByDni.Remove(p.Dni);

        var deleted = await repository.DeleteAsync(id, isLogical);
        return deleted is not null
            ? Result.Success<Persona, DomainError>(deleted)
            : Result.Failure<Persona, DomainError>(PersonaErrors.NotFound(id.ToString()));
    }

    public async Task<bool> DeleteAllAsync()
    {
        _logger.Warning("Eliminando todas las personas del sistema");
        return await repository.DeleteAllAsync();
    }

    public async Task<Result<Persona, DomainError>> RestoreAsync(int id)
    {
        _logger.Information("Restaurando persona con ID {Id}", id);
        return await repository.RestoreAsync(id);
    }

    public async Task<int> CountEstudiantesAsync(bool includeDeleted = false)
    {
        return await repository.CountEstudiantesAsync(includeDeleted);
    }

    public async Task<int> CountDocentesAsync(bool includeDeleted = false)
    {
        return await repository.CountDocentesAsync(includeDeleted);
    }

    public async Task<int> CountAprobadosAsync(double notaCorte, bool includeDeleted = false)
    {
        var estudiantes = await repository.GetEstudiantesAsync(1, int.MaxValue, includeDeleted);
        return estudiantes.Count(e => e.Calificacion >= notaCorte);
    }

    public async Task<int> CountSuspensosAsync(double notaCorte, bool includeDeleted = false)
    {
        var estudiantes = await repository.GetEstudiantesAsync(1, int.MaxValue, includeDeleted);
        return estudiantes.Count(e => e.Calificacion < notaCorte);
    }

    public async Task<Dictionary<Ciclo, int>> GetEstudiantesPorCicloAsync(bool includeDeleted = false)
    {
        var estudiantes = await repository.GetEstudiantesAsync(1, int.MaxValue, includeDeleted);
        return estudiantes
            .GroupBy(e => e.Ciclo)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public async Task<Dictionary<Ciclo, int>> GetDocentesPorCicloAsync(bool includeDeleted = false)
    {
        var docentes = await repository.GetDocentesAsync(1, int.MaxValue, includeDeleted);
        return docentes
            .GroupBy(d => d.Ciclo)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    /// <summary>
    ///     Aplica ordenación a una lista de estudiantes según el criterio especificado.
    /// </summary>
    private IEnumerable<Estudiante> AplicarOrdenamientoEstudiantes(IEnumerable<Estudiante> lista,
        TipoOrdenamiento orden)
    {
        return orden switch {
            TipoOrdenamiento.Dni => lista.OrderBy(p => p.Dni),
            TipoOrdenamiento.Nombre => lista.OrderBy(p => p.Nombre),
            TipoOrdenamiento.Apellidos => lista.OrderBy(p => p.Apellidos),
            TipoOrdenamiento.Nota => lista.OrderByDescending(p => p.Calificacion),
            _ => lista.OrderBy(p => p.Id)
        };
    }

    /// <summary>
    ///     Aplica ordenación a una lista de estudiantes según el criterio especificado.
    /// </summary>
    private IEnumerable<Docente> AplicarOrdenamientoDocentes(IEnumerable<Docente> lista, TipoOrdenamiento orden)
    {
        return orden switch {
            TipoOrdenamiento.Dni => lista.OrderBy(p => p.Dni),
            TipoOrdenamiento.Nombre => lista.OrderBy(p => p.Nombre),
            TipoOrdenamiento.Apellidos => lista.OrderBy(p => p.Apellidos),
            TipoOrdenamiento.Experiencia => lista.OrderByDescending(p => p.Experiencia),
            _ => lista.OrderBy(p => p.Id)
        };
    }

    /// <summary>
    ///     Aplica ordenación a una lista de personas según el criterio especificado.
    /// </summary>
    private IEnumerable<Persona> AplicarOrdenamientoGeneral(IEnumerable<Persona> lista, TipoOrdenamiento orden)
    {
        return orden switch {
            TipoOrdenamiento.Dni => lista.OrderBy(p => p.Dni),
            TipoOrdenamiento.Nombre => lista.OrderBy(p => p.Nombre),
            TipoOrdenamiento.Apellidos => lista.OrderBy(p => p.Apellidos),
            _ => lista.OrderBy(p => p.Id)
        };
    }

    /// <summary>
    ///     Valida si el DNI es válido para actualización (no duplicado en otra persona).
    /// </summary>
    /// <param name="id">ID de la persona que se actualiza.</param>
    /// <param name="dni">DNI a validar.</param>
    /// <returns>True si es válido.</returns>
    private async Task<bool> IsDniValidForUpdateAsync(int id, string dni)
    {
        var p = await repository.GetByDniAsync(dni);
        return p == null || p.Id == id;
    }

    /// <summary>
    ///     Valida si el email es válido para actualización (no duplicado en otra persona).
    /// </summary>
    /// <param name="id">ID de la persona que se actualiza.</param>
    /// <param name="email">Email a validar.</param>
    /// <returns>True si es válido.</returns>
    private async Task<bool> IsEmailValidForUpdateAsync(int id, string email)
    {
        var p = await repository.GetByEmailAsync(email);
        return p == null || p.Id == id;
    }

    /// <summary>
    ///     Verifica que una persona existe en el repositorio.
    /// </summary>
    /// <param name="id">ID de la persona.</param>
    /// <returns>Result con la persona si existe.</returns>
    private async Task<Result<Persona, DomainError>> CheckExistsAsync(int id)
    {
        var persona = await repository.GetByIdAsync(id);
        return persona is not null
            ? Result.Success<Persona, DomainError>(persona)
            : Result.Failure<Persona, DomainError>(PersonaErrors.NotFound(id.ToString()));
    }

    /// <summary>
    ///     Valida una persona según su tipo (estudiante o docente).
    /// </summary>
    /// <param name="persona">Persona a validar.</param>
    /// <returns>Result con la persona validada o error de validación.</returns>
    private async Task<Result<Persona, DomainError>> ValidarPersonaAsync(Persona persona)
    {
        await Task.CompletedTask;

        _logger.Debug("Validando persona tipo: {Tipo}", persona.GetType().Name);

        var validationResult = valPersona.Validar(persona);
        if (validationResult.IsFailure)
        {
            _logger.Warning("Validacion de persona base fallida: {Error}", validationResult.Error.Message);
            return validationResult;
        }

        switch (persona)
        {
            case Estudiante estudiante:
                _logger.Debug("Ejecutando validador de Estudiante");
                var r1 = valEstudiante.Validar(estudiante);
                return r1.IsSuccess
                    ? Result.Success<Persona, DomainError>(r1.Value)
                    : Result.Failure<Persona, DomainError>(r1.Error);
            case Docente docente:
                _logger.Debug("Ejecutando validador de Docente");
                var r2 = valDocente.Validar(docente);
                return r2.IsSuccess
                    ? Result.Success<Persona, DomainError>(r2.Value)
                    : Result.Failure<Persona, DomainError>(r2.Error);
            default:
                return Result.Failure<Persona, DomainError>(PersonaErrors.Validation(new[]
                    { "Tipo de entidad no soportada." }));
        }
    }
}
