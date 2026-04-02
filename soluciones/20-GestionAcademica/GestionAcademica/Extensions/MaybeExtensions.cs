using CSharpFunctionalExtensions;

namespace GestionAcademica.Extensions;

/// <summary>
/// Métodos de extensión para convertir Maybe a Result.
/// </summary>
public static class MaybeExtensions
{
    /// <summary>
    /// Convierte un Maybe a un Result, retornando error si no tiene valor.
    /// </summary>
    public static Result<T, TError> ToResult<T, TError>(this Maybe<T> maybe, TError error)
        where T : class
        where TError : Errors.Common.DomainError
    {
        return maybe.HasValue
            ? Result.Success<T, TError>(maybe.Value)
            : Result.Failure<T, TError>(error);
    }
    
    /// <summary>
    /// Convierte un Maybe a un Result usando una fábrica de errores.
    /// </summary>
    public static Result<T, TError> ToResult<T, TError>(this Maybe<T> maybe, Func<TError> errorFactory)
        where T : class
        where TError : Errors.Common.DomainError
    {
        return maybe.HasValue
            ? Result.Success<T, TError>(maybe.Value)
            : Result.Failure<T, TError>(errorFactory());
    }
}
