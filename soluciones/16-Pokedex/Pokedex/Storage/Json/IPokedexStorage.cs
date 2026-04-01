// =============================================================================
// INTERFAZ DE STORAGE DE POKEDEX
// =============================================================================
// Esta interfaz hereda de IStorage<Pokemon> para especificar el tipo.
// Permite tener múltiples implementaciones de storage (JSON, XML, CSV, etc.)
// =============================================================================

using Pokedex.Models;
using Pokedex.Storage.Common;

namespace Pokedex.Storage;

/// <summary>
/// Interfaz específica para el almacenamiento de pokemons.
/// Hereda de IStorage para mantener una abstracción genérica.
/// </summary>
public interface IPokedexStorage : IStorage<Pokemon> { }
