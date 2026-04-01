// =============================================================================
// REPOSITORIO DE POKEDEX - IMPLEMENTACIÓN EN MEMORIA
// =============================================================================
// Esta implementación del repositorio almacena los datos en memoria.
// En una aplicación real, esto podría ser una base de datos, API, archivo, etc.
// El repositorio actúa como una colección en memoria de pokemons.
// =============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using Pokedex.Models;
using Serilog;

namespace Pokedex.Repositories;

/// <summary>
/// Implementación del repositorio de pokemons en memoria.
/// Almacena los datos en una lista para acceso rápido.
/// </summary>
public class PokedexRepository : IPokedexRepository
{
    // Logger para registrar eventos y depuración
    private readonly ILogger _logger = Log.ForContext<PokedexRepository>();
    
    // Almacén interno de pokemons en memoria
    private List<Pokemon> _pokemons = [];

    /// <summary>
    /// Constructor del repositorio.
    /// Inicializa el logger para depuración.
    /// </summary>
    public PokedexRepository()
    {
        _logger.Debug("PokedexRepository inicializado");
    }

    /// <inheritdoc/>
    public IEnumerable<Pokemon> GetAll() => _pokemons;

    /// <inheritdoc/>
    public Pokemon? GetById(int id) => _pokemons.FirstOrDefault(p => p.Id == id);

    /// <inheritdoc/>
    public IEnumerable<Pokemon> GetByName(string name)
    {
        // Si no hay texto de búsqueda, devuelve todos
        if (string.IsNullOrWhiteSpace(name))
            return _pokemons;
        
        // Búsqueda case-insensitive usando LINQ
        // Contains busca cualquier pokemon cuyo nombre contenga el texto
        return _pokemons.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
    }

    /// <inheritdoc/>
    public IEnumerable<Pokemon> GetByType(string type)
    {
        // Si el tipo es vacío o "All", devuelve todos
        if (string.IsNullOrWhiteSpace(type) || type == "All")
            return _pokemons;
        
        // Filtra pokemons que tengan el tipo especificado en su lista de tipos
        // Un pokemon puede tener 1 o 2 tipos
        return _pokemons.Where(p => p.Type.Contains(type, StringComparer.OrdinalIgnoreCase));
    }

    /// <inheritdoc/>
    public IEnumerable<Pokemon> GetPage(int page, int pageSize)
    {
        // Skip: salta los elementos de páginas anteriores
        // Take: toma solo los elementos de la página actual
        // Ejemplo: página 2 con 20 elementos por página:
        // Skip( (2-1) * 20 ) = Skip(20) - salta los primeros 20
        // Take(20) - toma los siguientes 20
        return _pokemons
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
    }

    /// <inheritdoc/>
    public int GetTotalPages(int pageSize)
    {
        // Validación para evitar división por cero
        if (pageSize <= 0) return 0;
        
        // Math.Ceiling redondea hacia arriba para manejar el resto
        // Ejemplo: 25 pokemons / 20 por página = 1.25 -> 2 páginas
        return (int)Math.Ceiling((double)_pokemons.Count / pageSize);
    }

    /// <inheritdoc/>
    public void Load(IEnumerable<Pokemon> pokemons)
    {
        // Convierte IEnumerable a List y reemplaza la lista existente
        _pokemons = pokemons.ToList();
        _logger.Information("Cargados {count} pokemons", _pokemons.Count);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        // Limpia todos los elementos de la lista
        _pokemons.Clear();
        _logger.Information("Repositorio limpiado");
    }
}
