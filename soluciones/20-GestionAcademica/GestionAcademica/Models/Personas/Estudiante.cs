using System.ComponentModel;
using System.Text.RegularExpressions;
using GestionAcademica.Config;
using GestionAcademica.Models.Academia;

namespace GestionAcademica.Models.Personas;

/// <summary>
///     Representa a un estudiante dentro del sistema académico.
///     Implementa la lógica de calificación cualitativa y validación visual.
/// </summary>
public sealed record Estudiante : Persona, IEstudiar, IDataErrorInfo {
    public double Calificacion { get; init; }
    public Ciclo Ciclo { get; init; }
    public Curso Curso { get; init; }

    /// <summary>
    ///     Calcula la representación textual de la nota según el estándar educativo.
    /// </summary>
    public string CalificacionCualitativa => Calificacion switch {
        < 5 => "Suspenso",
        < 7 => "Aprobado",
        < 9 => "Notable",
        _ => "Sobresaliente"
    };

    public void Estudiar() {
        Console.WriteLine($"📚 El estudiante {NombreCompleto} está repasando el curso {Curso} de {Ciclo}.");
    }

    public override string ToString() {
        return $"[Estudiante] {NombreCompleto} ({Dni}) - Nota: {Calificacion.ToString("F2", AppConfig.Locale)}";
    }

    // IDataErrorInfo
    public string Error => string.Empty;

    public string this[string columnName] => columnName switch {
        nameof(Nombre) when string.IsNullOrWhiteSpace(Nombre)
            => "El nombre es obligatorio",
        nameof(Nombre) when (Nombre.Length < 2 || Nombre.Length > 30)
            => "El nombre debe tener entre 2 y 30 caracteres",
        nameof(Apellidos) when string.IsNullOrWhiteSpace(Apellidos)
            => "Los apellidos son obligatorios",
        nameof(Apellidos) when (Apellidos.Length < 2 || Apellidos.Length > 50)
            => "Los apellidos deben tener entre 2 y 50 caracteres",
        nameof(Dni) when string.IsNullOrWhiteSpace(Dni)
            => "El DNI es obligatorio",
        nameof(Dni) when !Regex.IsMatch(Dni, @"^\d{8}[A-Z]$")
            => "El DNI debe tener 8 dígitos y una letra mayúscula (ej: 12345678Z)",
        nameof(Email) when string.IsNullOrWhiteSpace(Email)
            => "El email es obligatorio",
        nameof(Email) when !Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")
            => "El email debe tener un formato válido",
        nameof(FechaNacimiento) when FechaNacimiento.Year <= 1900
            => "La fecha de nacimiento debe ser posterior a 1900",
        nameof(FechaNacimiento) when FechaNacimiento > DateTime.Today
            => "La fecha de nacimiento no puede ser futura",
        nameof(Calificacion) when (Calificacion < 0 || Calificacion > 10)
            => "La calificación debe estar entre 0 y 10",
        _ => null!
    };
}