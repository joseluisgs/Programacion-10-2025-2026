using CSharpFunctionalExtensions;
using GestionAcademica.Errors.Common;
using GestionAcademica.Models.Personas;

namespace GestionAcademica.Repositories.Personas.Base;

public interface IPersonasRepository {
    Task<IEnumerable<Persona>> GetAllAsync(int page = 1, int pageSize = 10, bool includeDeleted = true);
    Task<IEnumerable<Estudiante>> GetEstudiantesAsync(int page = 1, int pageSize = 10, bool includeDeleted = true);
    Task<IEnumerable<Docente>> GetDocentesAsync(int page = 1, int pageSize = 10, bool includeDeleted = true);
    Task<Persona?> GetByIdAsync(int id);
    Task<Result<Persona, DomainError>> CreateAsync(Persona persona);
    Task<Result<Persona, DomainError>> UpdateAsync(int id, Persona persona);
    Task<Persona?> DeleteAsync(int id, bool isLogical = true);
    Task<Persona?> GetByDniAsync(string dni);
    Task<bool> ExisteDniAsync(string dni);
    Task<Persona?> GetByEmailAsync(string email);
    Task<bool> ExisteEmailAsync(string email);
    Task<bool> DeleteAllAsync();
    Task<int> CountEstudiantesAsync(bool includeDeleted = false);
    Task<int> CountDocentesAsync(bool includeDeleted = false);
    Task<IEnumerable<Estudiante>> GetEstudiantesOrderByAsync(string orden, int page = 1, int pageSize = 10, bool includeDeleted = true);
    Task<IEnumerable<Docente>> GetDocentesOrderByAsync(string orden, int page = 1, int pageSize = 10, bool includeDeleted = true);
    Task<Result<Persona, DomainError>> RestoreAsync(int id);
}
