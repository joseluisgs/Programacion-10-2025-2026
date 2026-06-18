namespace GestionAcademica.Blazor.Models;

public class CicloGrade
{
    public string Ciclo { get; set; } = "";
    public double Media { get; set; }
}

public class DistItem
{
    public string Label { get; set; } = "";
    public int Count { get; set; }
}

public class CicloCount
{
    public string Ciclo { get; set; } = "";
    public int Estudiantes { get; set; }
    public int Docentes { get; set; }
}

public class CicloTasa
{
    public string Ciclo { get; set; } = "";
    public double Porcentaje { get; set; }
}

public class EdadGrupo
{
    public string Grupo { get; set; } = "";
    public int Count { get; set; }
}

public class CicloExp
{
    public string Ciclo { get; set; } = "";
    public double Media { get; set; }
}
