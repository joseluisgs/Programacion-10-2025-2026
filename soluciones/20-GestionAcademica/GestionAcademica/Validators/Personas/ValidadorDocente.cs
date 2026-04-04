using CSharpFunctionalExtensions;
using GestionAcademica.Errors.Common;
using GestionAcademica.Errors.Personas;
using GestionAcademica.Models;
using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Personas;
using GestionAcademica.Validators.Common;

namespace GestionAcademica.Validators.Personas;

/// <summary>
/// Validador específico para campos de Docente.
/// Los campos comunes de Persona se validan en ValidadorPersona (DRY).
/// </summary>
public class ValidadorDocente : IValidador<Persona>
{
    public Result<Persona, DomainError> Validar(Persona persona)
    {
        var errores = new List<string>();

        if (persona is not Docente docente)
        {
            errores.Add("La entidad proporcionada no es un Docente.");
            return Result.Failure<Persona, DomainError>(PersonaErrors.Validation(errores));
        }

        // Solo validamos campos específicos de Docente
        // (Nombre, Apellidos, FechaNacimiento están en ValidadorPersona)
        if (docente.Experiencia < 0)
            errores.Add("Los años de experiencia no pueden ser negativos.");

        if (string.IsNullOrWhiteSpace(docente.Especialidad))
            errores.Add("La especialidad o módulo docente debe estar definida.");

        if (!Enum.IsDefined(typeof(Ciclo), docente.Ciclo))
            errores.Add("El ciclo asignado no es un ciclo oficial válido.");

        if (errores.Any())
            return Result.Failure<Persona, DomainError>(PersonaErrors.Validation(errores));

        return Result.Success<Persona, DomainError>(persona);
    }
}
