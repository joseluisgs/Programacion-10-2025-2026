using System.ComponentModel;
using System.Text.RegularExpressions;
using GestionAcademica.Models.Academia;

namespace GestionAcademica.Models.Personas;

/// <summary>
///     Representa a un docente dentro del sistema académico.
///     Implementa validación visual a través de IDataErrorInfo.
/// </summary>
public sealed record Docente : Persona, IDocente, IDataErrorInfo {
    public int Experiencia { get; init; }
    public string Especialidad { get; init; } = string.Empty;
    public Ciclo Ciclo { get; init; }

    public void ImpartirClase() {
        Console.WriteLine($"👨‍🏫 El docente {NombreCompleto} está impartiendo {Especialidad} en {Ciclo}.");
    }

    public override string ToString() {
        return $"[Docente] {NombreCompleto} ({Dni}) - Exp: {Experiencia} años";
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
        nameof(Especialidad) when string.IsNullOrWhiteSpace(Especialidad)
            => "La especialidad es obligatoria",
        nameof(Especialidad) when Especialidad.Length < 3
            => "La especialidad debe tener al menos 3 caracteres",
        nameof(Experiencia) when (Experiencia < 0 || Experiencia > 50)
            => "La experiencia debe estar entre 0 y 50 años",
        _ => null!
    };
}