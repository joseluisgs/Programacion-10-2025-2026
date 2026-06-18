using CSharpFunctionalExtensions;
using GestionAcademica.Enums;
using GestionAcademica.Errors.Common;
using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Personas;

namespace GestionAcademica.Services.Personas;

public interface IPersonasService {
    Task<int> TotalPersonasAsync();

    Task<IEnumerable<Persona>> GetAllAsync(int page = 1, int pageSize = 10, bool includeDeleted = true);

    Task<IEnumerable<Estudiante>> GetEstudiantesOrderByAsync(
        TipoOrdenamiento ordenamiento = TipoOrdenamiento.Dni,
        int page = 1,
        int pageSize = 10,
        bool includeDeleted = true);

    Task<IEnumerable<Docente>> GetDocentesOrderByAsync(
        TipoOrdenamiento ordenamiento = TipoOrdenamiento.Dni,
        int page = 1,
        int pageSize = 10,
        bool includeDeleted = true);

    Task<IEnumerable<Persona>> GetAllOrderByAsync(
        TipoOrdenamiento orden = TipoOrdenamiento.Dni,
        Predicate<Persona>? filtro = null,
        int page = 1,
        int pageSize = 10,
        bool includeDeleted = true);

    Task<Result<Persona, DomainError>> GetByIdAsync(int id);
    Task<Result<Persona, DomainError>> GetByDniAsync(string dni);
    Task<Result<Persona, DomainError>> SaveAsync(Persona persona);
    Task<Result<Persona, DomainError>> UpdateAsync(int id, Persona persona);
    Task<Result<Persona, DomainError>> DeleteAsync(int id, bool isLogical = true);
    Task<bool> DeleteAllAsync();
    Task<Result<Persona, DomainError>> RestoreAsync(int id);

    Task<int> CountEstudiantesAsync(bool includeDeleted = false);
    Task<int> CountDocentesAsync(bool includeDeleted = false);
    Task<int> CountAprobadosAsync(double notaCorte, bool includeDeleted = false);
    Task<int> CountSuspensosAsync(double notaCorte, bool includeDeleted = false);
    Task<Dictionary<Ciclo, int>> GetEstudiantesPorCicloAsync(bool includeDeleted = false);
    Task<Dictionary<Ciclo, int>> GetDocentesPorCicloAsync(bool includeDeleted = false);
}
