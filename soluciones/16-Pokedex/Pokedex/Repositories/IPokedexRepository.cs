// =============================================================================
// INTERFAZ DEL REPOSITORIO DE POKEDEX
// =============================================================================
// El patrón Repository define una abstracción para acceder a los datos,
// separando la lógica de acceso a datos del resto de la aplicación.
// Esto permite cambiar la implementación del repositorio sin afectar al código
// que lo usa (por ejemplo, de memoria a base de datos).
// =============================================================================

using System.Collections.Generic;
using Pokedex.Models;

namespace Pokedex.Repositories;

/// <summary>
/// Interfaz que define las operaciones disponibles en el repositorio de Pokemons.
/// Implementa el patrón Repository para abstraer el acceso a datos.
/// </summary>
public interface IPokedexRepository
{
    /// <summary>
    /// Obtiene todos los pokemons disponibles.
    /// </summary>
    /// <returns>Colección de todos los pokemons</returns>
    IEnumerable<Pokemon> GetAll();

    /// <summary>
    /// Busca un pokemon por su ID único.
    /// </summary>
    /// <param name="id">Identificador numérico del pokemon</param>
    /// <returns>Pokemon encontrado o null si no existe</returns>
    Pokemon? GetById(int id);

    /// <summary>
    /// Busca pokemons cuyo nombre contenga el texto especificado.
    /// Búsqueda case-insensitive.
    /// </summary>
    /// <param name="name">Texto a buscar en el nombre</param>
    /// <returns>Colección de pokemons que coinciden</returns>
    IEnumerable<Pokemon> GetByName(string name);

    /// <summary>
    /// Filtra pokemons por su tipo.
    /// </summary>
    /// <param name="type">Tipo de pokemon a buscar</param>
    /// <returns>Colección de pokemons del tipo especificado</returns>
    IEnumerable<Pokemon> GetByType(string type);

    /// <summary>
    /// Obtiene una página específica de pokemons.
    /// Útil para paginación en interfaces de usuario.
    /// </summary>
    /// <param name="page">Número de página (comienza en 1)</param>
    /// <param name="pageSize">Cantidad de pokemons por página</returns>
    /// <returns>Colección de pokemons de la página solicitada</returns>
    IEnumerable<Pokemon> GetPage(int page, int pageSize);

    /// <summary>
    /// Calcula el número total de páginas disponibles.
    /// </summary>
    /// <param name="pageSize">Cantidad de pokemons por página</param>
    /// <returns>Total de páginas disponibles</returns>
    int GetTotalPages(int pageSize);

    /// <summary>
    /// Carga una colección de pokemons en el repositorio.
    /// reemplaza todos los datos existentes.
    /// </summary>
    /// <param name=" pokemons">Colección de pokemons a cargar</param>
    void Load(IEnumerable<Pokemon> pokemons);

    /// <summary>
    /// Limpia todos los pokemons del repositorio.
    /// </summary>
    void Clear();
}
