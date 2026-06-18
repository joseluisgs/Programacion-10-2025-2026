using System.Collections.Generic;
using System.Linq;
using Pokedex.Dto;
using Pokedex.Models;

namespace Pokedex.Mappers;

/// <summary>
/// Mapper para convertir entre DTOs y Modelos de dominio
/// </summary>
public static class PokemonMapper 
{
    /// <summary>
    /// Convierte un PokemonDto a Pokemon (Modelo de dominio)
    /// </summary>
    public static Pokemon ToModel(this PokemonDto dto) 
    {
        return new Pokemon(
            dto.Id,
            dto.Name,
            dto.DisplayName,
            dto.Type,
            new BaseStats(
                dto.Base.HP,
                dto.Base.Attack,
                dto.Base.Defense,
                dto.Base.SpAttack,
                dto.Base.SpDefense,
                dto.Base.Speed
            ),
            dto.Species,
            dto.Genus,
            dto.Category,
            dto.Description,
            dto.NextEvolution?.Select(e => new Evolution(e.Id, e.Name, e.Condition)).ToList(),
            dto.PrevEvolution?.Select(e => new Evolution(e.Id, e.Name, e.Condition)).ToList(),
            dto.Height,
            dto.Weight,
            dto.EggGroups,
            dto.Abilities.Select(a => new Ability(a.Name, a.IsHidden, a.Description)).ToList(),
            dto.GenderRatio,
            dto.Sprite,
            dto.Thumbnail,
            dto.Hires,
            dto.Cry,
            dto.Generation,
            dto.Habitat,
            dto.Color,
            dto.Shape,
            dto.CaptureRate,
            dto.BaseHappiness,
            dto.IsDefault,
            dto.BaseExperience,
            dto.Order,
            dto.IsLegendary,
            dto.IsMythical,
            dto.Varieties,
            dto.Moves.Select(m => new MoveInfo(m.Name, m.Description, m.Power, m.Accuracy, m.PP, m.Type, m.DamageClass)).ToList(),
            dto.TotalStats
        );
    }

    /// <summary>
    /// Convierte un Pokemon (Modelo de dominio) a PokemonDto
    /// </summary>
    public static PokemonDto ToDto(this Pokemon model)
    {
        return new PokemonDto(
            model.Id,
            model.Name,
            model.DisplayName,
            model.Type,
            new BaseStatsDto(
                model.Base.HP,
                model.Base.Attack,
                model.Base.Defense,
                model.Base.SpAttack,
                model.Base.SpDefense,
                model.Base.Speed
            ),
            model.Species,
            model.Genus,
            model.Category,
            model.Description,
            model.NextEvolution?.Select(e => new EvolutionDto(e.Id, e.Name, e.Condition)).ToList(),
            model.PrevEvolution?.Select(e => new EvolutionDto(e.Id, e.Name, e.Condition)).ToList(),
            model.Height,
            model.Weight,
            model.EggGroups,
            model.Abilities.Select(a => new AbilityDto(a.Name, a.IsHidden, a.Description)).ToList(),
            model.GenderRatio,
            model.Sprite,
            model.Thumbnail,
            model.Hires,
            model.Cry,
            model.Generation,
            model.Habitat,
            model.Color,
            model.Shape,
            model.CaptureRate,
            model.BaseHappiness,
            model.IsDefault,
            model.BaseExperience,
            model.Order,
            model.IsLegendary,
            model.IsMythical,
            model.Varieties,
            model.Moves.Select(m => new MoveInfoDto(m.Name, m.Description, m.Power, m.Accuracy, m.PP, m.Type, m.DamageClass)).ToList(),
            model.TotalStats
        );
    }
}
