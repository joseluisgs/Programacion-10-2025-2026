using CSharpFunctionalExtensions;
using GestionAcademica.Errors.Common;

namespace GestionAcademica.Validators.Common;

/// <summary>
/// Contrato para validar entidades del dominio.
/// </summary>
/// <typeparam name="T">Tipo de entidad a validar.</typeparam>
public interface IValidador<T>
{
    /// <summary>
    /// Valida una entidad según las reglas de dominio.
    /// </summary>
    /// <param name="entidad">Entidad a validar.</param>
    /// <returns>
    /// Result con la entidad validada o error <see cref="Errors.Personas.PersonaErrors.Validation(string)"/> si no es válida.
    /// </returns>
    Result<T, DomainError> Validar(T entidad);
}
