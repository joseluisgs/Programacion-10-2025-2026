using System.ComponentModel;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using GestionAcademica.Models.Academia;
using GestionAcademica.Validators.Personas;

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
        nameof(Nombre) when !Nombre.IsValidName()
            => "El nombre es obligatorio (2-30 caracteres)",

        nameof(Apellidos) when !Apellidos.IsValidSurnames()
            => "Los apellidos son obligatorios (2-50 caracteres)",

        nameof(Dni) when !Dni.IsValidDni()
            => "El DNI no es válido (8 números y letra correcta)",

        nameof(Email) when !Email.IsValidEmail()
            => "El formato del email es inválido",

        nameof(FechaNacimiento) when !FechaNacimiento.IsValidBirthDate()
            => "La fecha debe ser entre 1900 y hoy",

        nameof(Calificacion) when !Calificacion.IsValidCalificacion()
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

    /// <summary>
    ///     Devuelve una cadena con todos los errores de validación actuales, uno por línea.
    /// </summary>
    /// <returns>Texto con los errores de validación, o cadena vacía si el formulario es válido.</returns>
    public string GetValidationErrors()
    {
        var campos = new[]
        {
            (nameof(Nombre),            "Nombre"),
            (nameof(Apellidos),         "Apellidos"),
            (nameof(Dni),               "DNI"),
            (nameof(Email),             "Email"),
            (nameof(FechaNacimiento),   "Fecha de Nacimiento"),
            (nameof(Calificacion),      "Calificación"),
        };

        var errores = campos
            .Select(c => (Campo: c.Item2, Error: this[c.Item1]))
            .Where(c => !string.IsNullOrEmpty(c.Error))
            .Select(c => $"• {c.Campo}: {c.Error}");

        return string.Join("\n", errores);
    }
}
