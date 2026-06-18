using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Pokedex.Dto;

/// <summary>
/// DTO para deserializar habilidades desde JSON
/// </summary>
public sealed record AbilityDto(
    [property: JsonPropertyName("Name")] string Name,
    [property: JsonPropertyName("IsHidden")] bool IsHidden,
    [property: JsonPropertyName("Description")] string? Description
);

/// <summary>
/// DTO para deserializar estadísticas base desde JSON
/// </summary>
public sealed record BaseStatsDto(
    [property: JsonPropertyName("HP")] int HP,
    [property: JsonPropertyName("Attack")] int Attack,
    [property: JsonPropertyName("Defense")] int Defense,
    [property: JsonPropertyName("SpAttack")] int SpAttack,
    [property: JsonPropertyName("SpDefense")] int SpDefense,
    [property: JsonPropertyName("Speed")] int Speed
);

/// <summary>
/// DTO para deserializar evoluciones desde JSON
/// </summary>
public sealed record EvolutionDto(
    [property: JsonPropertyName("Id")] int? Id,
    [property: JsonPropertyName("Name")] string? Name,
    [property: JsonPropertyName("Condition")] string? Condition
);

/// <summary>
/// DTO para deserializar movimientos desde JSON
/// </summary>
public sealed record MoveInfoDto(
    [property: JsonPropertyName("Name")] string Name,
    [property: JsonPropertyName("Description")] string? Description,
    [property: JsonPropertyName("Power")] int? Power,
    [property: JsonPropertyName("Accuracy")] int? Accuracy,
    [property: JsonPropertyName("PP")] int? PP,
    [property: JsonPropertyName("Type")] string? Type,
    [property: JsonPropertyName("DamageClass")] string? DamageClass
);

/// <summary>
/// DTO principal para deserializar Pokemon desde JSON
/// </summary>
public sealed record PokemonDto(
    [property: JsonPropertyName("Id")] int Id,
    [property: JsonPropertyName("Name")] string Name,
    [property: JsonPropertyName("DisplayName")] string DisplayName,
    [property: JsonPropertyName("Type")] List<string> Type,
    [property: JsonPropertyName("Base")] BaseStatsDto Base,
    [property: JsonPropertyName("Species")] string Species,
    [property: JsonPropertyName("Genus")] string Genus,
    [property: JsonPropertyName("Category")] string Category,
    [property: JsonPropertyName("Description")] string Description,
    [property: JsonPropertyName("NextEvolution")] List<EvolutionDto>? NextEvolution,
    [property: JsonPropertyName("PrevEvolution")] List<EvolutionDto>? PrevEvolution,
    [property: JsonPropertyName("Height")] double Height,
    [property: JsonPropertyName("Weight")] double Weight,
    [property: JsonPropertyName("EggGroups")] List<string> EggGroups,
    [property: JsonPropertyName("Abilities")] List<AbilityDto> Abilities,
    [property: JsonPropertyName("GenderRatio")] string GenderRatio,
    [property: JsonPropertyName("Sprite")] string Sprite,
    [property: JsonPropertyName("Thumbnail")] string Thumbnail,
    [property: JsonPropertyName("Hires")] string Hires,
    [property: JsonPropertyName("Cry")] string? Cry,
    [property: JsonPropertyName("Generation")] string Generation,
    [property: JsonPropertyName("Habitat")] string? Habitat,
    [property: JsonPropertyName("Color")] string? Color,
    [property: JsonPropertyName("Shape")] string? Shape,
    [property: JsonPropertyName("CaptureRate")] int? CaptureRate,
    [property: JsonPropertyName("BaseHappiness")] int? BaseHappiness,
    [property: JsonPropertyName("IsDefault")] bool IsDefault,
    [property: JsonPropertyName("BaseExperience")] int BaseExperience,
    [property: JsonPropertyName("Order")] int Order,
    [property: JsonPropertyName("IsLegendary")] bool IsLegendary,
    [property: JsonPropertyName("IsMythical")] bool IsMythical,
    [property: JsonPropertyName("Varieties")] List<string> Varieties,
    [property: JsonPropertyName("Moves")] List<MoveInfoDto> Moves,
    [property: JsonPropertyName("TotalStats")] double? TotalStats
);
