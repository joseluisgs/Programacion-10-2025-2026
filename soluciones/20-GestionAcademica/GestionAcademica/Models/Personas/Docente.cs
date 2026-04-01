using GestionAcademica.Models.Academia;

namespace GestionAcademica.Models.Personas;

/// <summary>
///     Representa a un docente dentro del sistema académico.
/// </summary>
public sealed record Docente : Persona, IDocente {
    public int Experiencia { get; init; }
    public string Especialidad { get; init; } = string.Empty;
    public Ciclo Ciclo { get; init; }

    public void ImpartirClase() {
        Console.WriteLine($"👨‍🏫 El docente {NombreCompleto} está impartiendo {Especialidad} en {Ciclo}.");
    }

    public override string ToString() {
        return $"[Docente] {NombreCompleto} ({Dni}) - Exp: {Experiencia} años";
    }
}