using System.Collections.Generic;

namespace Pokedex.Errors;

/// <summary>
/// Errores específicos del dominio de Pokedex
/// </summary>
public abstract record PokedexError(string Message) : DomainError(Message)
{
    /// <summary>Error cuando no se encuentra un pokemon</summary>
    public sealed record NotFound(int Id)
        : PokedexError($"No se ha encontrado ningún pokémon con el identificador: {Id}");

    /// <summary>Error de validación</summary>
    public sealed record Validation(IEnumerable<string> Errors)
        : PokedexError("Se han detectado errores de validación.");

    /// <summary>Error al cargar datos</summary>
    public sealed record LoadError(string Details)
        : PokedexError($"Error al cargar el pokémon: {Details}");

    /// <summary>Error al guardar datos</summary>
    public sealed record SaveError(string Details)
        : PokedexError($"Error al guardar el pokémon: {Details}");
}

/// <summary>
/// Factory para crear errores de Pokedex
/// </summary>
public static class PokedexErrors
{
    public static DomainError NotFound(int id) => new PokedexError.NotFound(id);
    public static DomainError Validation(IEnumerable<string> errors) => new PokedexError.Validation(errors);
    public static DomainError LoadError(string details) => new PokedexError.LoadError(details);
    public static DomainError SaveError(string details) => new PokedexError.SaveError(details);
}
