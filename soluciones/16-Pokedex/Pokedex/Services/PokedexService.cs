// =============================================================================
// SERVICIO DE POKEDEX - LÓGICA DE NEGOCIO
// =============================================================================
// El patrón Service encapsula la lógica de negocio de la aplicación.
// Actúa como intermediario entre los ViewModels (UI) y los Repositories (datos).
// Aquí se implementa la lógica de caché, validación y coordinación.
// =============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using Pokedex.Cache;
using Pokedex.Config;
using Pokedex.Errors;
using Pokedex.Models;
using Pokedex.Repositories;
using Pokedex.Storage;
using Serilog;

namespace Pokedex.Services;

/// <summary>
/// Implementación del servicio de Pokedex.
/// Coordina el acceso a datos, caché y almacenamiento.
/// </summary>
public class PokedexService : IPokedexService
{
    // Logger para registrar eventos
    private readonly ILogger _logger = Log.ForContext<PokedexService>();
    
    // Dependencias inyectadas a través del constructor
    private readonly IPokedexRepository _repository;    // Acceso a datos
    private readonly IPokedexStorage _storage;         // Persistencia
    private readonly ICache<int, Pokemon> _cache;     // Caché en memoria

    /// <summary>
    /// Constructor con inyección de dependencias.
    /// Todas las dependencias se resuelven en el contenedor DI.
    /// </summary>
    /// <param name="repository">Repositorio de pokemons</param>
    /// <param name="storage">Almacenamiento persistente</param>
    /// <param name="cache">Caché LRU</param>
    public PokedexService(
        IPokedexRepository repository,
        IPokedexStorage storage,
        ICache<int, Pokemon> cache)
    {
        _logger.Debug("PokedexService inicializado");
        _repository = repository;
        _storage = storage;
        _cache = cache;
    }

    /// <inheritdoc/>
    public IEnumerable<Pokemon> GetAll() => _repository.GetAll();

    /// <inheritdoc/>
    public Pokemon? GetById(int id)
    {
        // ---------------------------------------------------------
        // LÓGICA DE CACHÉ:
        // Primero verificamos si el pokemon está en caché.
        // Si está, lo devolvemos sin acceder al repositorio.
        // ---------------------------------------------------------
        if (AppConfig.CacheEnabled)
        {
            var cached = _cache.Get(id);
            if (cached != null)
            {
                _logger.Debug("Pokemon {id} encontrado en caché", id);
                return cached;
            }
        }

        // Si no está en caché, lo buscamos en el repositorio
        var pokemon = _repository.GetById(id);
        
        // Si lo encontramos y el caché está habilitado, lo guardamos
        if (pokemon != null && AppConfig.CacheEnabled)
        {
            _cache.Add(id, pokemon);
        }

        return pokemon;
    }

    /// <inheritdoc/>
    public IEnumerable<Pokemon> GetByName(string name) => _repository.GetByName(name);

    /// <inheritdoc/>
    public IEnumerable<Pokemon> GetByType(string type) => _repository.GetByType(type);

    /// <inheritdoc/>
    public IEnumerable<Pokemon> GetPage(int page, int pageSize) => _repository.GetPage(page, pageSize);

    /// <inheritdoc/>
    public int GetTotalPages(int pageSize) => _repository.GetTotalPages(pageSize);

    /// <inheritdoc/>
    public IEnumerable<string> GetTypes()
    {
        // ---------------------------------------------------------
        // OBTENER TIPOS ÚNICOS:
        // 1. GetAll() - obtiene todos los pokemons
        // 2. SelectMany(p => p.Type) - "aplana" todas las listas de tipos
        //    (cada pokemon tiene 1-2 tipos, SelectMany los combina en una lista)
        // 3. Distinct() - elimina tipos duplicados
        // 4. OrderBy(t => t) - ordena alfabéticamente
        // 5. Concat(["All"]) - añade "All" al principio como opción de filtro
        // ---------------------------------------------------------
        var types = _repository.GetAll()
            .SelectMany(p => p.Type)    // Aplana las listas de tipos
            .Distinct()                // Elimina duplicados
            .OrderBy(t => t)          // Ordena alfabéticamente
            .ToList();
        
        // Añade "All" como primera opción para el filtro
        return new[] { "All" }.Concat(types);
    }

    /// <inheritdoc/>
    public Result<bool, DomainError> LoadFromFile(string path)
    {
        _logger.Information("Cargando pokemons desde archivo: {path}", path);
        
        // ---------------------------------------------------------
        // USO DE ROP (Railway Oriented Programming):
        // El método Cargar del storage retorna Result<T, DomainError>
        // Esto permite encadenar operaciones de forma segura.
        //
        // Si Cargar falla, retornamos el error directamente.
        // Si tiene éxito, ejecutamos el lambda en Map para:
        //   1. Cargar los pokemons en el repositorio
        //   2. Poblar el caché con los nuevos pokemons
        //   3. Retornar true indicando éxito
        // ---------------------------------------------------------
        var resultado = _storage.Cargar(path);
        
        return resultado.Map(pokemons =>
        {
            _repository.Load(pokemons);
            
            // Al cargar nuevos datos, limpiamos y repoblamos el caché
            if (AppConfig.CacheEnabled)
            {
                _cache.Clear();
                foreach (var pokemon in pokemons)
                {
                    _cache.Add(pokemon.Id, pokemon);
                }
            }
            
            return true;
        });
    }

    /// <inheritdoc/>
    public Result<bool, DomainError> SaveToFile(string path)
    {
        _logger.Information("Guardando pokemons en archivo: {path}", path);
        
        // Delega al storage la persistencia de datos
        return _storage.Salvar(_repository.GetAll(), path);
    }

    /// <inheritdoc/>
    public Result<bool, DomainError> LoadDefault()
    {
        _logger.Information("📂 LoadDefault: cargando desde {File}", AppConfig.PokedexFile);
        _logger.Information("📂 Ruta completa: {FullPath}", System.IO.Path.GetFullPath(AppConfig.PokedexFile));
        _logger.Information("📂 Archivo existe: {Exists}", System.IO.File.Exists(AppConfig.PokedexFile));
        
        // Carga desde el archivo configurado en appsettings.json
        return LoadFromFile(AppConfig.PokedexFile);
    }
}
