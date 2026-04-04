using CSharpFunctionalExtensions;
using GestionAcademica.Errors.Common;
using GestionAcademica.Models.Personas;

namespace GestionAcademica.Repositories.Personas.Base;

/// <summary>
/// Contrato especializado para la gestión de personas (Estudiantes y Docentes).
/// Define las operaciones de búsqueda, persistencia y validación de identidad.
///
/// KISS Principle: Solo Create y Update usan Result porque son las únicas operaciones
/// que tienen restricciones de dominio (DNI único, Email único). Las demás operaciones
/// (GetById, Delete) usan null/bool que es más simple e idiomático en .NET.
/// </summary>
public interface IPersonasRepository
{
    /// <summary>
    /// Obtiene todas las personas de forma paginada.
    /// </summary>
    IEnumerable<Persona> GetAll(int page = 1, int pageSize = 10, bool includeDeleted = true);

    /// <summary>
    /// Obtiene todos los estudiantes de forma paginada.
    /// </summary>
    IEnumerable<Estudiante> GetEstudiantes(int page = 1, int pageSize = 10, bool includeDeleted = true);

    /// <summary>
    /// Obtiene todos los docentes de forma paginada.
    /// </summary>
    IEnumerable<Docente> GetDocentes(int page = 1, int pageSize = 10, bool includeDeleted = true);

    /// <summary>
    /// Obtiene una persona por su ID.
    /// </summary>
    Persona? GetById(int id);

    /// <summary>
    /// Crea una nueva persona en el sistema.
    /// </summary>
    /// <returns>Result con la persona creada o error de dominio.</returns>
    Result<Persona, DomainError> Create(Persona persona);

    /// <summary>
    /// Actualiza una persona existente.
    /// </summary>
    /// <returns>Result con la persona actualizada o error de dominio.</returns>
    Result<Persona, DomainError> Update(int id, Persona persona);

    /// <summary>
    /// Elimina una persona.
    /// </summary>
    Persona? Delete(int id, bool isLogical = true);

    /// <summary>
    ///     Realiza una búsqueda por el Documento Nacional de Identidad.
    /// </summary>
    Persona? GetByDni(string dni);

    /// <summary>
    ///     Verifica si un DNI ya se encuentra registrado.
    /// </summary>
    bool ExisteDni(string dni);

    /// <summary>
    ///     Realiza una búsqueda por email.
    /// </summary>
    Persona? GetByEmail(string email);

    /// <summary>
    ///     Verifica si un email ya se encuentra registrado.
    /// </summary>
    bool ExisteEmail(string email);

    /// <summary>
    ///     Elimina todas las personas del sistema.
    /// </summary>
    bool DeleteAll();

    /// <summary>
    /// Obtiene el número total de estudiantes registrados.
    /// </summary>
    int CountEstudiantes(bool includeDeleted = false);

    /// <summary>
    /// Obtiene el número total de docentes registrados.
    /// </summary>
    int CountDocentes(bool includeDeleted = false);

    /// <summary>
    /// Restaura una persona eliminada lógicamente (IsDeleted = false, DeletedAt = null).
    /// </summary>
    Result<Persona, DomainError> Restore(int id);
}