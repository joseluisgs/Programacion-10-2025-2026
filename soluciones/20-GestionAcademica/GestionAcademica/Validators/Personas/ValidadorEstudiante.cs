using CSharpFunctionalExtensions;
using GestionAcademica.Errors.Common;
using GestionAcademica.Errors.Personas;
using GestionAcademica.Models;
using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Personas;
using GestionAcademica.Validators.Common;

namespace GestionAcademica.Validators.Personas;

/// <summary>
/// Validador específico para campos de Estudiante.
/// Los campos comunes de Persona se validan en ValidadorPersona (DRY).
/// </summary>
public class ValidadorEstudiante : IValidador<Persona>
{
    public Result<Persona, DomainError> Validar(Persona persona)
    {
        var errores = new List<string>();

        if (persona is not Estudiante estudiante)
        {
            errores.Add("La entidad proporcionada no es un Estudiante.");
            return Result.Failure<Persona, DomainError>(PersonaErrors.Validation(errores));
        }

        // Solo validamos campos específicos de Estudiante
        // (Nombre, Apellidos, FechaNacimiento están en ValidadorPersona)
        if (estudiante.Calificacion is < 0 or > 10)
            errores.Add("La calificación debe estar entre 0.0 y 10.0.");

        if (!Enum.IsDefined(typeof(Ciclo), estudiante.Ciclo))
            errores.Add("El ciclo formativo no es válido.");

        if (!Enum.IsDefined(typeof(Curso), estudiante.Curso))
            errores.Add("El curso académico no es válido (Primero o Segundo).");

        if (errores.Any())
            return Result.Failure<Persona, DomainError>(PersonaErrors.Validation(errores));

        return Result.Success<Persona, DomainError>(persona);
    }
}
