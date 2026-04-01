using CSharpFunctionalExtensions;
using GestionAcademica.Enums;
using GestionAcademica.Errors.Common;
using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Personas;

namespace GestionAcademica.Services.Personas;

public interface IPersonasService
{
    int TotalPersonas { get; }

    IEnumerable<Persona> GetAll(int page = 1, int pageSize = 10, bool includeDeleted = true);

    IEnumerable<Estudiante> GetEstudiantesOrderBy(
        TipoOrdenamiento ordenamiento = TipoOrdenamiento.Dni,
        int page = 1,
        int pageSize = 10,
        bool includeDeleted = true);

    IEnumerable<Docente> GetDocentesOrderBy(
        TipoOrdenamiento ordenamiento = TipoOrdenamiento.Dni,
        int page = 1,
        int pageSize = 10,
        bool includeDeleted = true);


    IEnumerable<Persona> GetAllOrderBy(
        TipoOrdenamiento orden = TipoOrdenamiento.Dni,
        Predicate<Persona>? filtro = null,
        int page = 1,
        int pageSize = 10,
        bool includeDeleted = true);

    Result<Persona, DomainError> GetById(int id);
    Result<Persona, DomainError> GetByDni(string dni);
    Result<Persona, DomainError> Save(Persona persona);
    Result<Persona, DomainError> Update(int id, Persona persona);
    Result<Persona, DomainError> Delete(int id, bool isLogical = true);
    bool DeleteAll();

    // Métodos de conteo para estadísticas del Dashboard
    int CountEstudiantes(bool includeDeleted = false);
    int CountDocentes(bool includeDeleted = false);
    int CountAprobados(double notaCorte, bool includeDeleted = false);
    int CountSuspensos(double notaCorte, bool includeDeleted = false);
    Dictionary<Ciclo, int> GetEstudiantesPorCiclo(bool includeDeleted = false);
    Dictionary<Ciclo, int> GetDocentesPorCiclo(bool includeDeleted = false);
}
