using CSharpFunctionalExtensions;
using GestionAcademica.Enums;
using GestionAcademica.Errors.Common;
using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Personas;

namespace GestionAcademica.Services.Personas;

/// <summary>
/// Define el contrato para la gestión de personas (estudiantes y docentes) en el sistema.
/// </summary>
public interface IPersonasService
{
    /// <summary>
    /// Obtiene el número total de personas registradas (estudiantes y docentes).
    /// </summary>
    /// <returns>Total de personas.</returns>
    int TotalPersonas { get; }

    /// <summary>
    /// Obtiene todas las personas de forma paginada.
    /// </summary>
    /// <param name="page">Número de página (por defecto 1).</param>
    /// <param name="pageSize">Tamaño de página (por defecto 10).</param>
    /// <param name="includeDeleted">Indica si incluye personas eliminadas lógicamente.</param>
    /// <returns>Enumerable de personas.</returns>
    IEnumerable<Persona> GetAll(int page = 1, int pageSize = 10, bool includeDeleted = true);

    /// <summary>
    /// Obtiene todos los estudiantes ordenados según el criterio especificado.
    /// </summary>
    /// <param name="ordenamiento">Criterio de ordenación (por defecto DNI).</param>
    /// <param name="page">Número de página.</param>
    /// <param name="pageSize">Tamaño de página.</param>
    /// <param name="includeDeleted">Indica si incluye eliminados.</param>
    /// <returns>Enumerable de estudiantes.</returns>
    IEnumerable<Estudiante> GetEstudiantesOrderBy(
        TipoOrdenamiento ordenamiento = TipoOrdenamiento.Dni,
        int page = 1,
        int pageSize = 10,
        bool includeDeleted = true);

    /// <summary>
    /// Obtiene todos los docentes ordenados según el criterio especificado.
    /// </summary>
    /// <param name="ordenamiento">Criterio de ordenación (por defecto DNI).</param>
    /// <param name="page">Número de página.</param>
    /// <param name="pageSize">Tamaño de página.</param>
    /// <param name="includeDeleted">Indica si incluye eliminados.</param>
    /// <returns>Enumerable de docentes.</returns>
    IEnumerable<Docente> GetDocentesOrderBy(
        TipoOrdenamiento ordenamiento = TipoOrdenamiento.Dni,
        int page = 1,
        int pageSize = 10,
        bool includeDeleted = true);

    /// <summary>
    /// Obtiene todas las personas ordenadas con opcional filtrado.
    /// </summary>
    /// <param name="orden">Criterio de ordenación.</param>
    /// <param name="filtro">Predicado opcional para filtrar resultados.</param>
    /// <param name="page">Número de página.</param>
    /// <param name="pageSize">Tamaño de página.</param>
    /// <param name="includeDeleted">Indica si incluye eliminados.</param>
    /// <returns>Enumerable de personas filtradas y ordenadas.</returns>
    IEnumerable<Persona> GetAllOrderBy(
        TipoOrdenamiento orden = TipoOrdenamiento.Dni,
        Predicate<Persona>? filtro = null,
        int page = 1,
        int pageSize = 10,
        bool includeDeleted = true);

    /// <summary>
    /// Obtiene una persona por su identificador único.
    /// </summary>
    /// <param name="id">Identificador de la persona.</param>
    /// <returns>
    /// Result con la persona encontrada o error <see cref="Errors.Personas.PersonaErrors.NotFound(string)"/> si no existe.
    /// </returns>
    Result<Persona, DomainError> GetById(int id);

    /// <summary>
    /// Obtiene una persona por su DNI.
    /// </summary>
    /// <param name="dni">Número de DNI.</param>
    /// <returns>
    /// Result con la persona encontrada o error <see cref="Errors.Personas.PersonaErrors.NotFound(string)"/> si no existe.
    /// </returns>
    Result<Persona, DomainError> GetByDni(string dni);

    /// <summary>
    /// Persiste una nueva persona en el sistema.
    /// </summary>
    /// <param name="persona">Datos de la persona a crear.</param>
    /// <returns>
    /// Result con la persona creada o error de validación:
    /// <see cref="Errors.Personas.PersonaErrors.DniAlreadyExists(string)"/>,
    /// <see cref="Errors.Personas.PersonaErrors.EmailAlreadyExists(string)"/> o
    /// <see cref="Errors.Personas.PersonaErrors.Validation(string)"/>.
    /// </returns>
    Result<Persona, DomainError> Save(Persona persona);

    /// <summary>
    /// Actualiza una persona existente.
    /// </summary>
    /// <param name="id">Identificador de la persona a actualizar.</param>
    /// <param name="persona">Nuevos datos de la persona.</param>
    /// <returns>
    /// Result con la persona actualizada o error:
    /// <see cref="Errors.Personas.PersonaErrors.NotFound(string)"/>,
    /// <see cref="Errors.Personas.PersonaErrors.DniAlreadyExists(string)"/>,
    /// <see cref="Errors.Personas.PersonaErrors.EmailAlreadyExists(string)"/> o
    /// <see cref="Errors.Personas.PersonaErrors.Validation(string)"/>.
    /// </returns>
    Result<Persona, DomainError> Update(int id, Persona persona);

    /// <summary>
    /// Elimina una persona del sistema.
    /// </summary>
    /// <param name="id">Identificador de la persona a eliminar.</param>
    /// <param name="isLogical">Indica si el borrado es lógico (true) o físico (false). Por defecto true.</param>
    /// <returns>
    /// Result con la persona eliminada o error <see cref="Errors.Personas.PersonaErrors.NotFound(string)"/> si no existe.
    /// </returns>
    Result<Persona, DomainError> Delete(int id, bool isLogical = true);

    /// <summary>
    /// Elimina todas las personas del sistema (usado en tests o reset).
    /// </summary>
    /// <returns>True si la operación fue exitosa.</returns>
    bool DeleteAll();

    /// <summary>
    /// Restaura una persona eliminada lógicamente.
    /// </summary>
    /// <param name="id">Identificador de la persona a restaurar.</param>
    /// <returns>
    /// Result con la persona restaurada o error <see cref="Errors.Personas.PersonaErrors.NotFound(string)"/> si no existe o no está eliminada.
    /// </returns>
    Result<Persona, DomainError> Restore(int id);

    // Métodos de conteo para estadísticas del Dashboard

    /// <summary>
    /// Cuenta el número de estudiantes registrados.
    /// </summary>
    /// <param name="includeDeleted">Indica si incluye eliminados.</param>
    /// <returns>Total de estudiantes.</returns>
    int CountEstudiantes(bool includeDeleted = false);

    /// <summary>
    /// Cuenta el número de docentes registrados.
    /// </summary>
    /// <param name="includeDeleted">Indica si incluye eliminados.</param>
    /// <returns>Total de docentes.</returns>
    int CountDocentes(bool includeDeleted = false);

    /// <summary>
    /// Cuenta el número de estudiantes aprobados según nota de corte.
    /// </summary>
    /// <param name="notaCorte">Nota mínima para considerar aprobado.</param>
    /// <param name="includeDeleted">Indica si incluye eliminados.</param>
    /// <returns>Total de aprobados.</returns>
    int CountAprobados(double notaCorte, bool includeDeleted = false);

    /// <summary>
    /// Cuenta el número de estudiantes suspensos según nota de corte.
    /// </summary>
    /// <param name="notaCorte">Nota máxima para considerar suspenso.</param>
    /// <param name="includeDeleted">Indica si incluye eliminados.</param>
    /// <returns>Total de suspensos.</returns>
    int CountSuspensos(double notaCorte, bool includeDeleted = false);

    /// <summary>
    /// Obtiene el número de estudiantes por cada ciclo formativo.
    /// </summary>
    /// <param name="includeDeleted">Indica si incluye eliminados.</param>
    /// <returns>Diccionario con el ciclo como clave y el número de estudiantes como valor.</returns>
    Dictionary<Ciclo, int> GetEstudiantesPorCiclo(bool includeDeleted = false);

    /// <summary>
    /// Obtiene el número de docentes por cada ciclo formativo.
    /// </summary>
    /// <param name="includeDeleted">Indica si incluye eliminados.</param>
    /// <returns>Diccionario con el ciclo como clave y el número de docentes como valor.</returns>
    Dictionary<Ciclo, int> GetDocentesPorCiclo(bool includeDeleted = false);
}
