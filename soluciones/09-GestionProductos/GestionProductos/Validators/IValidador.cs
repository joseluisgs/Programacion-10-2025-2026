// ============================================================
// IValidador.cs - Interfaz genérica para validadores
// ============================================================
// Interfaz para validar entidades usando Railway Oriented Programming.
//
// USO:
// - Validar devuelve Result<T, DomainError>
// - Result.Success(entidad) si es válido
// - Result.Failure(error) si hay errores

using CSharpFunctionalExtensions;

namespace GestionProductos.Validators;

/// <summary>
/// Interfaz para validadores de entidades.
/// </summary>
public interface IValidador<T>
{
    /// <summary>
    /// Valida una entidad.
    /// </summary>
    Result<T, Errors.DomainError> Validar(T entidad);
}