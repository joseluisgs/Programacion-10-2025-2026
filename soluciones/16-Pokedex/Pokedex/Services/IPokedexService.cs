using System.Collections.Generic;
using CSharpFunctionalExtensions;
using Pokedex.Errors;
using Pokedex.Models;

namespace Pokedex.Services;

/// <summary>
/// Interfaz del servicio de Pokedex
/// </summary>
public interface IPokedexService
{
    /// <summary>Obtiene todos los pokemons</summary>
    IEnumerable<Pokemon> GetAll();
    /// <summary>Obtiene un pokemon por su ID</summary>
    Pokemon? GetById(int id);
    /// <summary>Busca pokemons por nombre</summary>
    IEnumerable<Pokemon> GetByName(string name);
    /// <summary>Filtra pokemons por tipo</summary>
    IEnumerable<Pokemon> GetByType(string type);
    /// <summary>Obtiene una página de pokemons</summary>
    IEnumerable<Pokemon> GetPage(int page, int pageSize);
    /// <summary>Calcula el total de páginas</summary>
    int GetTotalPages(int pageSize);
    /// <summary>Obtiene todos los tipos disponibles</summary>
    IEnumerable<string> GetTypes();
    /// <summary>Carga pokemons desde un archivo</summary>
    Result<bool, DomainError> LoadFromFile(string path);
    /// <summary>Guarda pokemons en un archivo</summary>
    Result<bool, DomainError> SaveToFile(string path);
    /// <summary>Carga pokemons desde el archivo por defecto</summary>
    Result<bool, DomainError> LoadDefault();
}
