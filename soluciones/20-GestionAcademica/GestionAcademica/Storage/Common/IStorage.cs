using CSharpFunctionalExtensions;
using GestionAcademica.Errors.Common;

namespace GestionAcademica.Storage.Common;

/// <summary>
/// Define el contrato genérico para persistir y cargar colecciones de objetos en diferentes formatos.
/// </summary>
/// <typeparam name="T">Tipo de objeto a persistir.</typeparam>
public interface IStorage<T>
{
    /// <summary>
    /// Persiste una colección de objetos en un archivo.
    /// </summary>
    /// <param name="items">Enumerable de objetos a guardar.</param>
    /// <param name="path">Ruta del archivo de destino.</param>
    /// <returns>
    /// Result con true si se guardó correctamente o error:
    /// <see cref="Errors.Storage.StorageErrors.FileNotFound(string)"/>,
    /// <see cref="Errors.Storage.StorageErrors.InvalidFormat(string)"/> o
    /// <see cref="Errors.Storage.StorageErrors.WriteError(string)"/>.
    /// </returns>
    Result<bool, DomainError> Salvar(IEnumerable<T> items, string path);

    /// <summary>
    /// Carga una colección de objetos desde un archivo.
    /// </summary>
    /// <param name="path">Ruta del archivo a leer.</param>
    /// <returns>
    /// Result con la colección de objetos o error:
    /// <see cref="Errors.Storage.StorageErrors.FileNotFound(string)"/>,
    /// <see cref="Errors.Storage.StorageErrors.InvalidFormat(string)"/> o
    /// <see cref="Errors.Storage.StorageErrors.ReadError(string)"/>.
    /// </returns>
    Result<IEnumerable<T>, DomainError> Cargar(string path);
}
