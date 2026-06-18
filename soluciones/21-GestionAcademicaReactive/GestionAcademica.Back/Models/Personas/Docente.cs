using GestionAcademica.Models.Academia;

namespace GestionAcademica.Models.Personas;

/// <summary>
///     Representa un docente en el sistema.
/// </summary>
/// <remarks>
///     Hereda de Persona e implementa IDocente. Contains información de experiencia, especialidad y ciclo.
/// </remarks>
public sealed record Docente : Persona, IDocente {
    /// <summary>Años de experiencia docente.</summary>
    public int Experiencia { get; init; }
    /// <summary>Módulo o especialidad que imparte el docente.</summary>
    public string Especialidad { get; init; } = string.Empty;
    /// <summary>Ciclo formativo en el que imparte clases.</summary>
    public Ciclo Ciclo { get; init; }

    /// <summary>
    ///     Simula que el docente está impartiendo una clase.
    /// </summary>
    public void ImpartirClase() {
        Console.WriteLine($"Docente {NombreCompleto} esta impartiendo {Especialidad} en {Ciclo}.");
    }

    /// <summary>
    ///     Representación en string del docente para debugging.
    /// </summary>
    public override string ToString() {
        return $"[Docente] {NombreCompleto} ({Dni}) - Exp: {Experiencia} anos";
    }
}