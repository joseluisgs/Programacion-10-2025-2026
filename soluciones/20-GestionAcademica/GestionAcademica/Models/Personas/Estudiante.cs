using GestionAcademica.Config;
using GestionAcademica.Models.Academia;

namespace GestionAcademica.Models.Personas;

/// <summary>
///     Representa a un estudiante dentro del sistema académico.
///     Implementa la lógica de calificación cualitativa.
/// </summary>
public sealed record Estudiante : Persona, IEstudiar {
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
}