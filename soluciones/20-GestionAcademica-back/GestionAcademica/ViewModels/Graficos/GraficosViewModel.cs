using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionAcademica.Models.Personas;
using GestionAcademica.Services.Personas;
using GestionAcademica.Enums;
using Serilog;

namespace GestionAcademica.ViewModels.Graficos;

public partial class GraficosViewModel : ObservableObject
{
    private readonly IPersonasService _personasService;
    private readonly ILogger _logger = Log.ForContext<GraficosViewModel>();

    [ObservableProperty]
    private string _statusMessage = "";

    [ObservableProperty]
    private int _totalEstudiantes;

    [ObservableProperty]
    private int _totalDocentes;

    [ObservableProperty]
    private double _mediaNotas;

    [ObservableProperty]
    private int _estudiantesAprobados;

    [ObservableProperty]
    private int _estudiantesSuspensos;

    [ObservableProperty]
    private int _estudiantesNotable;

    [ObservableProperty]
    private int _estudiantesSobresaliente;

    public GraficosViewModel(IPersonasService personasService)
    {
        _personasService = personasService;
        LoadStatistics();
    }

    private void LoadStatistics()
    {
        try
        {
            var estudiantes = _personasService.GetEstudiantesOrderBy(TipoOrdenamiento.Dni, 1, 1000, false).ToList();
            var docentes = _personasService.GetDocentesOrderBy(TipoOrdenamiento.Dni, 1, 1000, false).ToList();

            TotalEstudiantes = estudiantes.Count;
            TotalDocentes = docentes.Count;

            if (estudiantes.Any())
            {
                MediaNotas = estudiantes.Average(e => e.Calificacion);
                EstudiantesAprobados = estudiantes.Count(e => e.Calificacion >= 5);
                EstudiantesSuspensos = estudiantes.Count(e => e.Calificacion < 5);
                EstudiantesNotable = estudiantes.Count(e => e.Calificacion >= 7 && e.Calificacion < 9);
                EstudiantesSobresaliente = estudiantes.Count(e => e.Calificacion >= 9);
            }

            StatusMessage = $"Estadísticas cargadas: {TotalEstudiantes} estudiantes, {TotalDocentes} docentes";
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al cargar estadísticas");
            StatusMessage = "Error al cargar";
        }
    }

    [RelayCommand]
    private void Refresh()
    {
        LoadStatistics();
    }

    public double[] GetCalificacionesData()
    {
        var estudiantes = _personasService.GetEstudiantesOrderBy(TipoOrdenamiento.Dni, 1, 1000, false);
        var grouped = estudiantes.GroupBy(e => e.Ciclo)
            .Select(g => new { Ciclo = g.Key, Media = g.Average(e => e.Calificacion) })
            .ToList();

        return grouped.Select(g => g.Media).ToArray();
    }

    public string[] GetCicloLabels()
    {
        var estudiantes = _personasService.GetEstudiantesOrderBy(TipoOrdenamiento.Dni, 1, 1000, false);
        return estudiantes.GroupBy(e => e.Ciclo).Select(g => g.Key.ToString()).ToArray();
    }

    public double[] GetNotasDistribution()
    {
        var estudiantes = _personasService.GetEstudiantesOrderBy(TipoOrdenamiento.Dni, 1, 1000, false).ToList();
        return new double[]
        {
            estudiantes.Count(e => e.Calificacion < 5),
            estudiantes.Count(e => e.Calificacion >= 5 && e.Calificacion < 7),
            estudiantes.Count(e => e.Calificacion >= 7 && e.Calificacion < 9),
            estudiantes.Count(e => e.Calificacion >= 9)
        };
    }

    public (double[] values, string[] labels) GetDocentesPorCiclo()
    {
        var docentes = _personasService.GetDocentesOrderBy(TipoOrdenamiento.Dni, 1, 1000, false).ToList();
        var grouped = docentes.GroupBy(d => d.Ciclo)
            .Select(g => new { Ciclo = g.Key, Count = g.Count() })
            .ToList();

        return (grouped.Select(g => (double)g.Count).ToArray(), grouped.Select(g => g.Ciclo.ToString()).ToArray());
    }
}