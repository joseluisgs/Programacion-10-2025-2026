using System.ComponentModel;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using GestionAcademica.Models.Academia;

namespace GestionAcademica.ViewModels.Forms;

/// <summary>
///     FormData para la validación de Estudiante en la capa de presentación.
///     Separa la lógica de validación UI del modelo de dominio puro.
///     IMPORTANTE: Este es el único lugar donde se implementa IDataErrorInfo para Estudiante.
/// </summary>
public partial class EstudianteFormData : ObservableObject, IDataErrorInfo
{
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private string _nombre = string.Empty;

    [ObservableProperty]
    private string _apellidos = string.Empty;

    [ObservableProperty]
    private string _dni = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private DateTime _fechaNacimiento = DateTime.Today.AddYears(-18);

    [ObservableProperty]
    private string? _imagen;

    [ObservableProperty]
    private double _calificacion;

    [ObservableProperty]
    private Ciclo _ciclo;

    [ObservableProperty]
    private Curso _curso;

    /// <summary>Marca de tiempo de creación del registro.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Marca de tiempo de última actualización del registro.</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Indica si el registro está marcado como eliminado (soft delete).</summary>
    public bool IsDeleted { get; set; }

    /// <summary>Marca de tiempo de eliminación lógica, si aplica.</summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>Resumen de errores globales del formulario. Requerido por IDataErrorInfo.</summary>
    public string Error => string.Empty;

    /// <summary>
    ///     Validación campo por campo requerida por IDataErrorInfo para el binding WPF con ValidatesOnDataErrors=True.
    /// </summary>
    /// <param name="columnName">Nombre de la propiedad a validar.</param>
    /// <returns>Mensaje de error en español si el campo es inválido; cadena vacía o null si es válido.</returns>
    public string this[string columnName] => columnName switch
    {
        nameof(Nombre) when string.IsNullOrWhiteSpace(Nombre)
            => "El nombre es obligatorio",
        nameof(Nombre) when Nombre.Length < 2 || Nombre.Length > 30
            => "El nombre debe tener entre 2 y 30 caracteres",

        nameof(Apellidos) when string.IsNullOrWhiteSpace(Apellidos)
            => "Los apellidos son obligatorios",
        nameof(Apellidos) when Apellidos.Length < 2 || Apellidos.Length > 50
            => "Los apellidos deben tener entre 2 y 50 caracteres",

        nameof(Dni) when string.IsNullOrWhiteSpace(Dni)
            => "El DNI es obligatorio",
        nameof(Dni) when !Regex.IsMatch(Dni, @"^\d{8}[A-Z]$")
            => "El DNI debe tener 8 dígitos y una letra mayúscula (ej: 12345678Z)",

        nameof(Email) when string.IsNullOrWhiteSpace(Email)
            => "El email es obligatorio",
        nameof(Email) when !Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")
            => "El formato del email es inválido",

        nameof(FechaNacimiento) when FechaNacimiento.Year < 1900
            => "La fecha debe ser posterior a 1900",
        nameof(FechaNacimiento) when FechaNacimiento > DateTime.Today
            => "La fecha no puede ser futura",

        nameof(Calificacion) when Calificacion < 0 || Calificacion > 10
            => "La nota debe estar entre 0 y 10",

        _ => null!
    };

    /// <summary>
    ///     Verifica que todos los campos del formulario sean válidos antes de persistir.
    /// </summary>
    /// <returns>True si el formulario no tiene errores de validación.</returns>
    public bool IsValid() =>
        string.IsNullOrEmpty(this[nameof(Nombre)]) &&
        string.IsNullOrEmpty(this[nameof(Apellidos)]) &&
        string.IsNullOrEmpty(this[nameof(Dni)]) &&
        string.IsNullOrEmpty(this[nameof(Email)]) &&
        string.IsNullOrEmpty(this[nameof(FechaNacimiento)]) &&
        string.IsNullOrEmpty(this[nameof(Calificacion)]);
}
