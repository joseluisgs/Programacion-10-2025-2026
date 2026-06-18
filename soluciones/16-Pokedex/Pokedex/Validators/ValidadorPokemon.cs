using System.Collections.Generic;
using CSharpFunctionalExtensions;
using Pokedex.Errors;
using Pokedex.Models;

namespace Pokedex.Validators;

public interface IValidador<T>
{
    Result<T, DomainError> Validar(T entity);
}

public class ValidadorPokemon : IValidador<Pokemon>
{
    public Result<Pokemon, DomainError> Validar(Pokemon pokemon)
    {
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(pokemon.Name))
            errores.Add("El nombre es obligatorio.");

        if (pokemon.Id <= 0)
            errores.Add("El ID debe ser mayor que 0.");

        if (errores.Count > 0)
            return Result.Failure<Pokemon, DomainError>(PokedexErrors.Validation(errores));

        return Result.Success<Pokemon, DomainError>(pokemon);
    }
}
