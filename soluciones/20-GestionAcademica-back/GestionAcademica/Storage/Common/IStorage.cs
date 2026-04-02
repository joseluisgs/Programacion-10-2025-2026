using CSharpFunctionalExtensions;
using GestionAcademica.Errors.Common;

namespace GestionAcademica.Storage.Common;

public interface IStorage<T>
{
    Result<bool, DomainError> Salvar(IEnumerable<T> items, string path);
    Result<IEnumerable<T>, DomainError> Cargar(string path);
}
