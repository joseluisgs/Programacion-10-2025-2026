using CSharpFunctionalExtensions;
using GestionAcademica.Errors.Common;
using GestionAcademica.Errors.Personas;
using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Personas;
using GestionAcademica.Validators.Common;

namespace GestionAcademica.Validators.Personas;

public class ValidadorDocente : IValidador<Docente> {
    public Result<Docente, DomainError> Validar(Docente docente) {
        var errores = new List<string>();

        if (docente.Experiencia < 0)
            errores.Add("Los años de experiencia no pueden ser negativos.");

        if (string.IsNullOrWhiteSpace(docente.Especialidad))
            errores.Add("La especialidad o módulo docente debe estar definida.");

        if (!Enum.IsDefined(typeof(Ciclo), docente.Ciclo))
            errores.Add("El ciclo asignado no es un ciclo oficial válido.");

        if (errores.Any())
            return Result.Failure<Docente, DomainError>(PersonaErrors.Validation(errores));

        return Result.Success<Docente, DomainError>(docente);
    }
}