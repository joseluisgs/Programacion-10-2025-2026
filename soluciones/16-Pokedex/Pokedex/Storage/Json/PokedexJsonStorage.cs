// =============================================================================
// STORAGE JSON PARA POKEDEX
// =============================================================================
// Implementa IPokedexStorage usando archivos JSON.
// Serializa y deserializa pokemons usando System.Text.Json.
// =============================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;
using Pokedex.Dto;
using Pokedex.Errors;
using Pokedex.Mappers;
using Pokedex.Models;
using Pokedex.Storage;
using Serilog;

namespace Pokedex.Storage.Json;

/// <summary>
/// Implementación de almacenamiento que usa archivos JSON.
/// Serializa/deserializa pokemons usando System.Text.Json.
/// </summary>
public class PokedexJsonStorage : IPokedexStorage
{
    private readonly ILogger _logger = Log.ForContext<PokedexJsonStorage>();

    // ---------------------------------------------------------
    // CONFIGURACIÓN DE SERIALIZACIÓN JSON:
    // - WriteIndented: formato legible (true) o minificado (false)
    // - PropertyNamingPolicy: usa camelCase en JSON
    // - DefaultIgnoreCondition: ignora propiedades null al serializar
    // - JsonStringEnumConverter: serializa enums como strings
    // - Encoder: permite caracteres especiales (acentos, emoji, etc.)
    // ---------------------------------------------------------
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() },
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    /// <inheritdoc/>
    public Result<bool, DomainError> Salvar(IEnumerable<Pokemon> items, string path)
    {
        try
        {
            _logger.Information("Guardando pokemons en JSON: {path}", path);
            
            // 1. Convierte modelos a DTOs (necesario para serialización)
            var pokemons = items.Select(PokemonMapper.ToDto).ToList();
            
            // 2. Serializa a JSON con opciones configuradas
            var json = JsonSerializer.Serialize(pokemons, _options);
            
            // 3. Escribe al archivo
            File.WriteAllText(path, json);
            
            // 4. Retorna éxito usando ROP
            return Result.Success<bool, DomainError>(true);
        }
        catch (Exception ex)
        {
            // Captura cualquier excepción y la convierte en error ROP
            _logger.Error(ex, "Error al guardar en JSON: {path}", path);
            return Result.Failure<bool, DomainError>(PokedexErrors.SaveError(ex.Message));
        }
    }

    /// <inheritdoc/>
    public Result<IEnumerable<Pokemon>, DomainError> Cargar(string path)
    {
        _logger.Information("Cargando pokemons desde JSON: {path}", path);
        
        // Verifica que el archivo existe antes de intentar leer
        if (!File.Exists(path))
        {
            _logger.Warning("Archivo no encontrado: {path}", path);
            return Result.Failure<IEnumerable<Pokemon>, DomainError>(PokedexErrors.LoadError($"Archivo no encontrado: {path}"));
        }

        try
        {
            // 1. Abre el archivo como stream para mejor rendimiento
            using var stream = File.OpenRead(path);
            
            // 2. Deserializa directamente a lista de DTOs
            var pokemonsDto = JsonSerializer.Deserialize<List<PokemonDto>>(stream, _options);

            // 3. Verifica que la deserialización fue exitosa
            if (pokemonsDto == null)
                return Result.Failure<IEnumerable<Pokemon>, DomainError>(PokedexErrors.LoadError("No se pudieron deserializar los datos."));

            // 4. Convierte DTOs a modelos y retorna éxito
            return Result.Success<IEnumerable<Pokemon>, DomainError>(pokemonsDto.Select(p => p.ToModel()));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al cargar desde JSON: {path}", path);
            return Result.Failure<IEnumerable<Pokemon>, DomainError>(PokedexErrors.LoadError(ex.Message));
        }
    }
}
