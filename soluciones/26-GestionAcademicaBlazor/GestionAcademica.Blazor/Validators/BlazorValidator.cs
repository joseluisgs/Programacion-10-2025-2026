using GestionAcademica.Models.Personas;
using GestionAcademica.Validators.Personas;

namespace GestionAcademica.Blazor.Services;

public static class BlazorValidator
{
    public static string ValidarCampo(string campo, Estudiante form) => campo switch
    {
        "Nombre" => Validar(form.Nombre.IsValidName(), "El nombre es obligatorio (2-30 caracteres)"),
        "Apellidos" => Validar(form.Apellidos.IsValidSurnames(), "Los apellidos son obligatorios (2-50 caracteres)"),
        "Dni" => Validar(form.Dni.IsValidDni(), "El DNI no es válido (8 números y letra correcta)"),
        "Email" => Validar(form.Email.IsValidEmail(), "El formato del email es inválido"),
        "FechaNacimiento" => Validar(form.FechaNacimiento.IsValidBirthDate(), "La fecha debe ser entre 1900 y hoy"),
        "Calificacion" => Validar(form.Calificacion.IsValidCalificacion(), "La nota debe estar entre 0 y 10"),
        _ => ""
    };

    public static string ValidarCampo(string campo, Docente form) => campo switch
    {
        "Nombre" => Validar(form.Nombre.IsValidName(), "El nombre es obligatorio (2-30 caracteres)"),
        "Apellidos" => Validar(form.Apellidos.IsValidSurnames(), "Los apellidos son obligatorios (2-50 caracteres)"),
        "Dni" => Validar(form.Dni.IsValidDni(), "El DNI no es válido (8 números y letra correcta)"),
        "Email" => Validar(form.Email.IsValidEmail(), "El formato del email es inválido"),
        "FechaNacimiento" => Validar(form.FechaNacimiento.IsValidBirthDate(), "La fecha debe ser entre 1900 y hoy"),
        "Experiencia" => Validar(form.Experiencia >= 0 && form.Experiencia <= 50, "La experiencia debe estar entre 0 y 50 años"),
        "Especialidad" => Validar(!string.IsNullOrWhiteSpace(form.Especialidad), "La especialidad es obligatoria"),
        _ => ""
    };

    private static string Validar(bool condicion, string mensaje) =>
        condicion ? "" : mensaje;
}
