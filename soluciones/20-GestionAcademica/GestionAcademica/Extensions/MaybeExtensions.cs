using CSharpFunctionalExtensions;

namespace GestionAcademica.Extensions;

public static class MaybeExtensions
{
    public static Result<T, TError> ToResult<T, TError>(this Maybe<T> maybe, TError error)
        where T : class
        where TError : Errors.Common.DomainError
    {
        return maybe.HasValue
            ? Result.Success<T, TError>(maybe.Value)
            : Result.Failure<T, TError>(error);
    }
    
    public static Result<T, TError> ToResult<T, TError>(this Maybe<T> maybe, Func<TError> errorFactory)
        where T : class
        where TError : Errors.Common.DomainError
    {
        return maybe.HasValue
            ? Result.Success<T, TError>(maybe.Value)
            : Result.Failure<T, TError>(errorFactory());
    }
}
