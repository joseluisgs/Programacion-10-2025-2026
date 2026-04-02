using System.Collections.Generic;

namespace Pokedex.Models;

/// <summary>
/// Representa la información de evolución de un Pokemon
/// </summary>
public sealed record Evolution(
    int? Id,
    string? Name,
    string? Condition
);

/// <summary>
/// Representa las estadísticas base de un Pokemon
/// </summary>
public sealed record BaseStats(
    int HP,
    int Attack,
    int Defense,
    int SpAttack,
    int SpDefense,
    int Speed
);

/// <summary>
/// Representa una habilidad de un Pokemon
/// </summary>
public sealed record Ability(
    string Name,
    bool IsHidden,
    string? Description
);

/// <summary>
/// Representa información de tipo (para movimientos)
/// </summary>
public sealed record TypeInfo(
    string Name,
    int Slot
);

/// <summary>
/// Representa información de un movimiento
/// </summary>
public sealed record MoveInfo(
    string Name,
    string? Description,
    int? Power,
    int? Accuracy,
    int? PP,
    string? Type,
    string? DamageClass
);

/// <summary>
/// Modelo principal de Pokemon con todos sus datos
/// </summary>
public sealed record Pokemon(
    int Id,
    string Name,
    string DisplayName,
    List<string> Type,
    BaseStats Base,
    string Species,
    string Genus,
    string Category,
    string Description,
    List<Evolution>? NextEvolution,
    List<Evolution>? PrevEvolution,
    double Height,
    double Weight,
    List<string> EggGroups,
    List<Ability> Abilities,
    string GenderRatio,
    string Sprite,
    string Thumbnail,
    string Hires,
    string? Cry,
    string Generation,
    string? Habitat,
    string? Color,
    string? Shape,
    int? CaptureRate,
    int? BaseHappiness,
    bool IsDefault,
    int BaseExperience,
    int Order,
    bool IsLegendary,
    bool IsMythical,
    List<string> Varieties,
    List<MoveInfo> Moves,
    double? TotalStats
);
