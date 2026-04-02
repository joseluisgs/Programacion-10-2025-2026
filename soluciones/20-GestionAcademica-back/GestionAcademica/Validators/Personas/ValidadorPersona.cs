using System.IO;
using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using GestionAcademica.Config;
using GestionAcademica.Errors.Common;
using GestionAcademica.Errors.Personas;
using GestionAcademica.Models.Personas;
using GestionAcademica.Validators.Common;

namespace GestionAcademica.Validators.Personas;

public static class ValidadorPersonaExtensions
{
    public static bool IsValidEmail(this string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;
        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
        return emailRegex.IsMatch(email);
    }

    public static bool IsValidImage(this string? imagen)
    {
        if (string.IsNullOrWhiteSpace(imagen))
            return true;
        
        var extension = Path.GetExtension(imagen).ToLowerInvariant();
        return AppConfig.AllowedImageExtensions.Contains(extension);
    }
}

/// <summary>
/// Validador base para campos comunes de Persona.
/// Aplica el principio DRY: los campos de Persona se validan aquí una sola vez.
/// </summary>
public class ValidadorPersona : IValidador<Persona>
{
    public Result<Persona, DomainError> Validar(Persona persona)
    {
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(persona.Nombre) || persona.Nombre.Length < 2)
            errores.Add("El nombre es obligatorio (mín. 2 car.).");

        if (string.IsNullOrWhiteSpace(persona.Apellidos) || persona.Apellidos.Length < 2)
            errores.Add("Los apellidos son obligatorios (mín. 2 car.).");

        if (persona.FechaNacimiento > DateTime.UtcNow)
            errores.Add("La fecha de nacimiento no puede ser futura.");

        if (!persona.Email.IsValidEmail())
            errores.Add("El email no es válido.");

        if (!persona.Imagen.IsValidImage())
            errores.Add("La imagen debe ser png, jpg, jpeg o bmp.");

        if (errores.Any())
            return Result.Failure<Persona, DomainError>(PersonaErrors.Validation(errores));

        return Result.Success<Persona, DomainError>(persona);
    }
}
