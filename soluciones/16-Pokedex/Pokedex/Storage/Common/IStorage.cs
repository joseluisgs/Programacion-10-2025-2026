// =============================================================================
// INTERFAZ GENÉRICA DE STORAGE
// =============================================================================
// Define un contrato genérico para persistencia de datos.
// Cualquier clase que implemente esta interfaz puede guardar y cargar
// datos de un tipo específico desde/hacia cualquier fuente.
// =============================================================================

using System.Collections.Generic;
using CSharpFunctionalExtensions;
using Pokedex.Errors;

namespace Pokedex.Storage.Common;

/// <summary>
/// Interfaz genérica para operaciones de almacenamiento.
/// </summary>
/// <typeparam name="T">Tipo de entidad a persistir</typeparam>
public interface IStorage<T>
{
    /// <summary>
    /// Guarda una colección de elementos en un archivo.
    /// </summary>
    /// <param name="items">Colección de elementos a guardar</param>
    /// <param name="path">Ruta del archivo</param>
    /// <returns>Result con true si tiene éxito, o DomainError si falla</returns>
    Result<bool, DomainError> Salvar(IEnumerable<T> items, string path);

    /// <summary>
    /// Carga una colección de elementos desde un archivo.
    /// </summary>
    /// <param name="path">Ruta del archivo a leer</param>
    /// <returns>Result con la colección cargada o error si falla</returns>
    Result<IEnumerable<T>, DomainError> Cargar(string path);
}
