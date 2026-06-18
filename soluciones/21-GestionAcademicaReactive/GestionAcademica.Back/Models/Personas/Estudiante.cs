using GestionAcademica.Config;
using GestionAcademica.Models.Academia;

namespace GestionAcademica.Models.Personas;

/// <summary>
///     Representa un estudiante en el sistema.
/// </summary>
/// <remarks>
///     Hereda de Persona e implementa IEstudiar. Contains información de ciclo, curso y calificación.
/// </remarks>
public sealed record Estudiante : Persona, IEstudiar {
    /// <summary>Nota del estudiante (rango 0-10).</summary>
    public double Calificacion { get; init; }
    /// <summary>Ciclo formativo al que pertenece el estudiante.</summary>
    public Ciclo Ciclo { get; init; }
    /// <summary>Curso académico (Primero o Segundo).</summary>
    public Curso Curso { get; init; }

    /// <summary>
    ///     Devuelve la calificación cualitativa basada en la nota numérica.
    /// </summary>
    public string CalificacionCualitativa => Calificacion switch {
        < 5 => "Suspenso",
        < 7 => "Aprobado",
        < 9 => "Notable",
        _ => "Sobresaliente"
    };

    /// <summary>
    ///     Simula que el estudiante está estudiando.
    /// </summary>
    public void Estudiar() {
        Console.WriteLine($"Estudiante {NombreCompleto} esta repasando el curso {Curso} de {Ciclo}.");
    }

    /// <summary>
    ///     Representación en string del estudiante para debugging.
    /// </summary>
    public override string ToString() {
        return $"[Estudiante] {NombreCompleto} ({Dni}) - Nota: {Calificacion.ToString("F2", AppConfig.Locale)}";
    }
}