using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionAcademica.Models.Personas;
using GestionAcademica.Services.Personas;
using GestionAcademica.Enums;
using Serilog;

namespace GestionAcademica.ViewModels.Graficos;

public partial class GraficosViewModel(
    IPersonasService personasService
) : ObservableObject
{
    private readonly IPersonasService _personasService = personasService;
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

    /// <summary>
    /// Inicializa el ViewModel cargando las estadísticas iniciales.
    /// Debe llamarse explícitamente tras la construcción del objeto.
    /// </summary>
    public void Initialize()
    {
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

    /// <summary>
    /// Obtiene la distribución de estudiantes por rangos de edad
    /// </summary>
    public Dictionary<string, int> GetEstudiantesPorEdad()
    {
        _logger.Information("📊 Calculando distribución de estudiantes por edad");

        var estudiantes = _personasService.GetEstudiantesOrderBy(TipoOrdenamiento.Dni, 1, 1000, false).ToList();
        var hoy = DateTime.Now;

        static double CalcularEdad(DateTime fechaNacimiento, DateTime hoy) =>
            (hoy - fechaNacimiento).TotalDays / 365.25;

        var menores18 = estudiantes.Count(e => CalcularEdad(e.FechaNacimiento, hoy) < 18);
        var entre18y25 = estudiantes.Count(e =>
        {
            var edad = CalcularEdad(e.FechaNacimiento, hoy);
            return edad >= 18 && edad < 25;
        });
        var mayores25 = estudiantes.Count(e => CalcularEdad(e.FechaNacimiento, hoy) >= 25);

        _logger.Information($"Menores de 18: {menores18}, Entre 18-25: {entre18y25}, Mayores de 25: {mayores25}");

        return new Dictionary<string, int>
        {
            ["Menores de 18"] = menores18,
            ["18-25 años"] = entre18y25,
            ["Mayores de 25"] = mayores25
        };
    }
}