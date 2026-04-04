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
        
        // Regex profesional para emails según estándares modernos (RFC 5322)
        var emailRegex = new Regex(
            @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
            
        return emailRegex.IsMatch(email);
    }

    public static bool IsValidName(this string name) => 
        !string.IsNullOrWhiteSpace(name) && name.Trim().Length >= 2 && name.Length <= 30;

    public static bool IsValidSurnames(this string surnames) => 
        !string.IsNullOrWhiteSpace(surnames) && surnames.Trim().Length >= 2 && surnames.Length <= 50;

    public static bool IsValidBirthDate(this DateTime date) => 
        date.Year >= 1900 && date <= DateTime.Today;

    public static bool IsValidCalificacion(this double calif) => 
        calif >= 0 && calif <= 10;

    public static bool IsValidImage(this string? imagen)
    {
        if (string.IsNullOrWhiteSpace(imagen))
            return true;
        
        var extension = Path.GetExtension(imagen).ToLowerInvariant();
        return AppConfig.AllowedImageExtensions.Contains(extension);
    }

    public static bool IsValidDni(this string dni)
    {
        if (string.IsNullOrWhiteSpace(dni)) return false;

        // Limpiar el formato: quitar espacios y guiones
        dni = dni.Trim().ToUpper().Replace(" ", "").Replace("-", "");

        // Expresión regular: 8 números y una letra
        if (!Regex.IsMatch(dni, @"^[0-9]{8}[A-Z]$")) return false;

        string numero = dni.Substring(0, 8);
        char letra = dni[8];

        // Algoritmo módulo 23
        const string letrasValidas = "TRWAGMYFPDXBNJZSQVHLCKE";
        if (!int.TryParse(numero, out int dniNumerico)) return false;
        
        int resto = dniNumerico % 23;
        char letraCalculada = letrasValidas[resto];

        return letra == letraCalculada;
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

        if (!persona.Nombre.IsValidName())
            errores.Add("El nombre es obligatorio (2-30 car.).");

        if (!persona.Apellidos.IsValidSurnames())
            errores.Add("Los apellidos son obligatorios (2-50 car.).");

        if (!persona.FechaNacimiento.IsValidBirthDate())
            errores.Add("La fecha de nacimiento debe ser entre 1900 y hoy.");

        if (!persona.Email.IsValidEmail())
            errores.Add("El email no tiene un formato válido.");

        if (!persona.Dni.IsValidDni())
            errores.Add("El DNI no es válido (8 números y letra correcta).");

        if (!persona.Imagen.IsValidImage())
            errores.Add("La imagen debe ser png, jpg, jpeg o bmp.");

        if (errores.Any())
            return Result.Failure<Persona, DomainError>(PersonaErrors.Validation(errores));

        return Result.Success<Persona, DomainError>(persona);
    }
}
