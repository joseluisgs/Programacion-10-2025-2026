using CSharpFunctionalExtensions;
using GestionAcademica.Errors.Common;

namespace GestionAcademica.Validators.Common;

public interface IValidador<T>
{
    Result<T, DomainError> Validar(T entidad);
}
