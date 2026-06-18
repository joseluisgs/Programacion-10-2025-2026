using GestionAcademica.Models.Personas;

namespace GestionAcademica.Models.Informes;

public sealed record InformeEstudiante {
    public IEnumerable<Estudiante> PorNota { get; init; } = Enumerable.Empty<Estudiante>();
    public int TotalEstudiantes { get; init; }
    public int Aprobados { get; init; }
    public int Suspensos { get; init; }
    public double NotaMedia { get; init; }
    public double PorcentajeAprobados => TotalEstudiantes > 0 ? (double)Aprobados / TotalEstudiantes * 100 : 0;
}