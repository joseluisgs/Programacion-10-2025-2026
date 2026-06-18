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

    /// <inheritdoc cref="IPersonasService.TotalPersonas" />
    public int TotalPersonas => repository.GetAll(1, int.MaxValue).Count();

    /// <inheritdoc cref="IPersonasService.GetAll(int, int, bool)" />
    public IEnumerable<Persona> GetAll(int page = 1, int pageSize = 10, bool includeDeleted = true) {
        return repository.GetAll(page, pageSize, includeDeleted);
    }

    /// <inheritdoc cref="IPersonasService.GetEstudiantesOrderBy(TipoOrdenamiento, int, int, bool)" />
    public IEnumerable<Estudiante> GetEstudiantesOrderBy(
        TipoOrdenamiento ordenamiento = TipoOrdenamiento.Dni,
        int page = 1,
        int pageSize = 10,
        bool includeDeleted = true) {
        // Usar paginación real del repositorio
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
        return repository.GetEstudiantesOrderBy(orden, page, pageSize, includeDeleted);
    }

    /// <inheritdoc cref="IPersonasService.GetDocentesOrderBy(TipoOrdenamiento, int, int, bool)" />
    public IEnumerable<Docente> GetDocentesOrderBy(
        TipoOrdenamiento ordenamiento = TipoOrdenamiento.Dni,
        int page = 1,
        int pageSize = 10,
        bool includeDeleted = true) {
        // Usar paginación real del repositorio
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
        return repository.GetDocentesOrderBy(orden, page, pageSize, includeDeleted);
    }

    /// <inheritdoc cref="IPersonasService.GetAllOrderBy(TipoOrdenamiento, Predicate{Persona}?, int, int, bool)" />
    public IEnumerable<Persona> GetAllOrderBy(
        TipoOrdenamiento orden = TipoOrdenamiento.Dni,
        Predicate<Persona>? filtro = null,
        int page = 1,
        int pageSize = 10,
        bool includeDeleted = true) {
        var lista = filtro == null
            ? repository.GetAll(1, int.MaxValue, includeDeleted)
            : repository.GetAll(1, int.MaxValue, includeDeleted).Where(p => filtro(p));

        return AplicarOrdenamientoGeneral(lista, orden)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
    }

    /// <inheritdoc cref="IPersonasService.GetById(int)" />
    public Result<Persona, DomainError> GetById(int id) {
        if (cacheById.Get(id) is { } cached)
            return Result.Success<Persona, DomainError>(cached);

        if (repository.GetById(id) is { } persona) {
            cacheById.Add(id, persona);
            return Result.Success<Persona, DomainError>(persona);
        }

        return Result.Failure<Persona, DomainError>(PersonaErrors.NotFound(id.ToString()));
    }

    /// <inheritdoc cref="IPersonasService.GetByDni(string)" />
    public Result<Persona, DomainError> GetByDni(string dni) {
        if (cacheByDni.Get(dni) is { } cached)
            return Result.Success<Persona, DomainError>(cached);

        if (repository.GetByDni(dni) is { } persona) {
            cacheByDni.Add(dni, persona);
            return Result.Success<Persona, DomainError>(persona);
        }

        return Result.Failure<Persona, DomainError>(PersonaErrors.NotFound(dni));
    }

    /// <inheritdoc cref="IPersonasService.Save(Persona)" />
    public Result<Persona, DomainError> Save(Persona persona) {
        return ValidarPersona(persona)
            .Ensure(p => !repository.ExisteDni(p.Dni ?? ""), p => PersonaErrors.DniAlreadyExists(p.Dni ?? ""))
            .Ensure(p => !repository.ExisteEmail(p.Email ?? ""), p => PersonaErrors.EmailAlreadyExists(p.Email ?? ""))
            .Bind(p => repository.Create(p))
            .Tap(creada => {
                cacheById.Add(creada.Id, creada);
                cacheByDni.Add(creada.Dni, creada);
            });
    }

    /// <inheritdoc cref="IPersonasService.Update(int, Persona)" />
    public Result<Persona, DomainError> Update(int id, Persona persona) {
        return CheckExists(id)
            .Tap(pOriginal => {
                // Si la imagen ha cambiado y la original no era nula, borramos la vieja
                if (!string.IsNullOrEmpty(pOriginal.Imagen) && pOriginal.Imagen != persona.Imagen) {
                    _logger.Debug("Eliminando imagen huérfana {FileName} por actualización", pOriginal.Imagen);
                    imageService.DeleteImage(pOriginal.Imagen);
                }

                // Invalidar ambas caches (por ID y por DNI)
                cacheById.Remove(id);
                cacheByDni.Remove(pOriginal.Dni);
            })
            .Bind(_ => ValidarPersona(persona))
            .Ensure(p => IsDniValidForUpdate(id, p.Dni ?? ""), p => PersonaErrors.DniAlreadyExists(p.Dni ?? ""))
            .Ensure(p => IsEmailValidForUpdate(id, p.Email ?? ""), p => PersonaErrors.EmailAlreadyExists(p.Email ?? ""))
            .Bind(p => repository.Update(id, p));
    }

    /// <inheritdoc cref="IPersonasService.Delete(int, bool)" />
    public Result<Persona, DomainError> Delete(int id, bool isLogical = true) {
        return CheckExists(id)
            .Tap(p => {
                // Solo si el borrado es FÍSICO, borramos la imagen del disco
                // Si es lógico, mantenemos la imagen por si se restaura
                if (!isLogical && !string.IsNullOrEmpty(p.Imagen)) {
                    _logger.Debug("Eliminando imagen {FileName} del disco por borrado físico", p.Imagen);
                    imageService.DeleteImage(p.Imagen);
                }

                // Invalidar ambas caches (por ID y por DNI)
                cacheById.Remove(id);
                cacheByDni.Remove(p.Dni);
            })
            .Map(p => repository.Delete(id, isLogical)!);
    }

    /// <inheritdoc cref="IPersonasService.DeleteAll" />
    public bool DeleteAll() {
        _logger.Warning("Eliminando todas las personas del sistema");
        return repository.DeleteAll();
    }

    /// <inheritdoc cref="IPersonasService.Restore(int)" />
    public Result<Persona, DomainError> Restore(int id) {
        _logger.Information("Restaurando persona con ID {Id}", id);
        return repository.Restore(id);
    }

    /// <inheritdoc cref="IPersonasService.CountEstudiantes(bool)" />
    public int CountEstudiantes(bool includeDeleted = false) {
        return repository.CountEstudiantes(includeDeleted);
    }

    /// <inheritdoc cref="IPersonasService.CountDocentes(bool)" />
    public int CountDocentes(bool includeDeleted = false) {
        return repository.CountDocentes(includeDeleted);
    }

    /// <inheritdoc cref="IPersonasService.CountAprobados(double, bool)" />
    public int CountAprobados(double notaCorte, bool includeDeleted = false) {
        return repository.GetEstudiantes(1, int.MaxValue, includeDeleted)
            .Count(e => e.Calificacion >= notaCorte);
    }

    /// <inheritdoc cref="IPersonasService.CountSuspensos(double, bool)" />
    public int CountSuspensos(double notaCorte, bool includeDeleted = false) {
        return repository.GetEstudiantes(1, int.MaxValue, includeDeleted)
            .Count(e => e.Calificacion < notaCorte);
    }

    /// <inheritdoc cref="IPersonasService.GetEstudiantesPorCiclo(bool)" />
    public Dictionary<Ciclo, int> GetEstudiantesPorCiclo(bool includeDeleted = false) {
        return repository.GetEstudiantes(1, int.MaxValue, includeDeleted)
            .GroupBy(e => e.Ciclo)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    /// <inheritdoc cref="IPersonasService.GetDocentesPorCiclo(bool)" />
    public Dictionary<Ciclo, int> GetDocentesPorCiclo(bool includeDeleted = false) {
        return repository.GetDocentes(1, int.MaxValue, includeDeleted)
            .GroupBy(d => d.Ciclo)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    /// <summary>
    ///     Aplica ordenación a una lista de estudiantes según el criterio especificado.
    /// </summary>
    private IEnumerable<Estudiante> AplicarOrdenamientoEstudiantes(IEnumerable<Estudiante> lista,
        TipoOrdenamiento orden) {
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
    private IEnumerable<Docente> AplicarOrdenamientoDocentes(IEnumerable<Docente> lista, TipoOrdenamiento orden) {
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
    private IEnumerable<Persona> AplicarOrdenamientoGeneral(IEnumerable<Persona> lista, TipoOrdenamiento orden) {
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
    private bool IsDniValidForUpdate(int id, string dni) {
        var p = repository.GetByDni(dni);
        return p == null || p.Id == id;
    }

    /// <summary>
    ///     Valida si el email es válido para actualización (no duplicado en otra persona).
    /// </summary>
    /// <param name="id">ID de la persona que se actualiza.</param>
    /// <param name="email">Email a validar.</param>
    /// <returns>True si es válido.</returns>
    private bool IsEmailValidForUpdate(int id, string email) {
        var p = repository.GetByEmail(email);
        return p == null || p.Id == id;
    }

    /// <summary>
    ///     Valida una persona según su tipo (estudiante o docente).
    /// </summary>
    /// <param name="persona">Persona a validar.</param>
    /// <returns>Result con la persona validada o error de validación.</returns>
    private Result<Persona, DomainError> ValidarPersona(Persona persona) {
        _logger.Debug("Validando persona tipo: {Tipo}", persona.GetType().Name);

        var validationResult = valPersona.Validar(persona);
        if (validationResult.IsFailure) {
            _logger.Warning("Validacion de persona base fallida: {Error}", validationResult.Error.Message);
            return validationResult;
        }

        switch (persona) {
            case Estudiante estudiante:
                _logger.Debug("Ejecutando validador de Estudiante");
                var r1 = valEstudiante.Validar(estudiante);
                // Como estudiante es persona, pero el result es de persona
                // Hacemos el casting de result a result
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

    /// <summary>
    ///     Verifica que una persona existe en el repositorio.
    /// </summary>
    /// <param name="id">ID de la persona.</param>
    /// <returns>Result con la persona si existe.</returns>
    private Result<Persona, DomainError> CheckExists(int id) {
        return repository.GetById(id) is { } persona
            ? Result.Success<Persona, DomainError>(persona)
            : Result.Failure<Persona, DomainError>(PersonaErrors.NotFound(id.ToString()));
    }
}