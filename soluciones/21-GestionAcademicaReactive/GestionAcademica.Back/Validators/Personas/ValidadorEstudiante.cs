using CSharpFunctionalExtensions;
using GestionAcademica.Errors.Common;
using GestionAcademica.Errors.Personas;
using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Personas;
using GestionAcademica.Validators.Common;

namespace GestionAcademica.Validators.Personas;

public class ValidadorEstudiante : IValidador<Estudiante> {
    public Result<Estudiante, DomainError> Validar(Estudiante estudiante) {
        var errores = new List<string>();

        if (estudiante.Calificacion is < 0 or > 10)
            errores.Add("La calificación debe estar entre 0.0 y 10.0.");

        if (!Enum.IsDefined(typeof(Ciclo), estudiante.Ciclo))
            errores.Add("El ciclo formativo no es válido.");

        if (!Enum.IsDefined(typeof(Curso), estudiante.Curso))
            errores.Add("El curso académico no es válido (Primero o Segundo).");

        if (errores.Any())
            return Result.Failure<Estudiante, DomainError>(PersonaErrors.Validation(errores));

        return Result.Success<Estudiante, DomainError>(estudiante);
    }
}