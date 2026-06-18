using GestionAcademica.Models.Personas;

namespace GestionAcademica.Models.Informes;

public sealed record InformeDocente {
    public IEnumerable<Docente> PorExperiencia { get; init; } = Enumerable.Empty<Docente>();
    public int TotalDocentes { get; init; }
    public double ExperienciaMedia { get; init; }
}