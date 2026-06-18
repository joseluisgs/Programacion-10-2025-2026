using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionAcademica.Enums;
using GestionAcademica.Models.Academia;
using GestionAcademica.Services.Personas;
using Serilog;

namespace GestionAcademica.ViewModels.Graficos;

/// <summary>
///     ViewModel para la vista de gráficos estadísticos.
///     Genera datos para visualización de estadísticas de estudiantes y docentes.
/// </summary>
public partial class GraficosViewModel : ObservableObject {
    private readonly ILogger _logger = Log.ForContext<GraficosViewModel>();
    private readonly IPersonasService _personasService;

    [ObservableProperty] private int _estudiantesAprobados;

    [ObservableProperty] private int _estudiantesNotable;

    [ObservableProperty] private int _estudiantesSobresaliente;

    [ObservableProperty] private int _estudiantesSuspensos;

    [ObservableProperty] private double _mediaNotas;

    [ObservableProperty] private string _statusMessage = "";

    [ObservableProperty] private int _totalDocentes;

    [ObservableProperty] private int _totalEstudiantes;

    public GraficosViewModel(IPersonasService personasService) {
        _personasService = personasService;
        LoadStatistics();
    }

    /// <summary>
    ///     Carga las estadísticas desde el servicio de personas.
    ///     Calcula totales, medias y distribución de calificaciones.
    /// </summary>
    private void LoadStatistics() {
        try {
            var estudiantes = _personasService.GetEstudiantesOrderBy(TipoOrdenamiento.Dni, 1, 1000, false).ToList();
            var docentes = _personasService.GetDocentesOrderBy(TipoOrdenamiento.Dni, 1, 1000, false).ToList();

            TotalEstudiantes = estudiantes.Count;
            TotalDocentes = docentes.Count;

            if (estudiantes.Any()) {
                MediaNotas = estudiantes.Average(e => e.Calificacion);
                EstudiantesAprobados = estudiantes.Count(e => e.Calificacion >= 5);
                EstudiantesSuspensos = estudiantes.Count(e => e.Calificacion < 5);
                EstudiantesNotable = estudiantes.Count(e => e.Calificacion >= 7 && e.Calificacion < 9);
                EstudiantesSobresaliente = estudiantes.Count(e => e.Calificacion >= 9);
            }

            StatusMessage = $"Estadísticas cargadas: {TotalEstudiantes} estudiantes, {TotalDocentes} docentes";
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al cargar estadísticas");
            StatusMessage = "Error al cargar";
        }
    }

    [RelayCommand]
    private void Refresh() {
        LoadStatistics();
    }

    public double[] GetCalificacionesData() {
        var estudiantes = _personasService.GetEstudiantesOrderBy(TipoOrdenamiento.Dni, 1, 1000, false);
        var grouped = estudiantes.GroupBy(e => e.Ciclo)
            .Select(g => new { Ciclo = g.Key, Media = g.Average(e => e.Calificacion) })
            .ToList();

        return grouped.Select(g => g.Media).ToArray();
    }

    public string[] GetCicloLabels() {
        var estudiantes = _personasService.GetEstudiantesOrderBy(TipoOrdenamiento.Dni, 1, 1000, false);
        return estudiantes.GroupBy(e => e.Ciclo).Select(g => g.Key.ToString()).ToArray();
    }

    public double[] GetNotasDistribution() {
        var estudiantes = _personasService.GetEstudiantesOrderBy(TipoOrdenamiento.Dni, 1, 1000, false).ToList();
        return new double[] {
            estudiantes.Count(e => e.Calificacion < 5),
            estudiantes.Count(e => e.Calificacion >= 5 && e.Calificacion < 7),
            estudiantes.Count(e => e.Calificacion >= 7 && e.Calificacion < 9),
            estudiantes.Count(e => e.Calificacion >= 9)
        };
    }

    public (double[] values, string[] labels) GetDocentesPorCiclo() {
        var docentes = _personasService.GetDocentesOrderBy(TipoOrdenamiento.Dni, 1, 1000, false).ToList();
        var grouped = docentes.GroupBy(d => d.Ciclo)
            .Select(g => new { Ciclo = g.Key, Count = g.Count() })
            .ToList();

        return (grouped.Select(g => (double)g.Count).ToArray(), grouped.Select(g => g.Ciclo.ToString()).ToArray());
    }

    public (double[] values, string[] labels) GetExperienciaMediaDocentes() {
        var docentes = _personasService.GetDocentesOrderBy(TipoOrdenamiento.Dni, 1, 1000, false).ToList();
        var grouped = docentes.GroupBy(d => d.Ciclo)
            .Select(g => new { Ciclo = g.Key, Media = g.Average(d => d.Experiencia) })
            .ToList();

        return (grouped.Select(g => g.Media).ToArray(), grouped.Select(g => g.Ciclo.ToString()).ToArray());
    }

    public (double[] values, string[] labels) GetEstudiantesCantidadPorCiclo() {
        var estudiantes = _personasService.GetEstudiantesOrderBy(TipoOrdenamiento.Dni, 1, 1000, false).ToList();
        var grouped = estudiantes.GroupBy(e => e.Ciclo)
            .Select(g => new { Ciclo = g.Key, Count = g.Count() })
            .ToList();

        return (grouped.Select(g => (double)g.Count).ToArray(), grouped.Select(g => g.Ciclo.ToString()).ToArray());
    }

    /// <summary>
    ///     Obtiene la distribución de estudiantes por rangos de edad
    /// </summary>
    public Dictionary<string, int> GetEstudiantesPorEdad() {
        _logger.Information("📊 Calculando distribución de estudiantes por edad");

        var estudiantes = _personasService.GetEstudiantesOrderBy(TipoOrdenamiento.Dni, 1, 1000, false).ToList();
        var hoy = DateTime.Now;

        static double CalcularEdad(DateTime fechaNacimiento, DateTime hoy) {
            return (hoy - fechaNacimiento).TotalDays / 365.25;
        }

        var menores18 = estudiantes.Count(e => CalcularEdad(e.FechaNacimiento, hoy) < 18);
        var entre18y25 = estudiantes.Count(e => {
            var edad = CalcularEdad(e.FechaNacimiento, hoy);
            return edad >= 18 && edad < 25;
        });
        var mayores25 = estudiantes.Count(e => CalcularEdad(e.FechaNacimiento, hoy) >= 25);

        _logger.Information($"Menores de 18: {menores18}, Entre 18-25: {entre18y25}, Mayores de 25: {mayores25}");

        return new Dictionary<string, int> {
            ["Menores de 18"] = menores18,
            ["18-25 años"] = entre18y25,
            ["Mayores de 25"] = mayores25
        };
    }

    /// <summary>
    ///     Obtiene la tasa de aprobados por ciclo (aprobados y suspensos)
    /// </summary>
    public (double[] aprobados, double[] suspensos, string[] ciclos) GetTasaAprobadosPorCiclo() {
        _logger.Information("📊 Calculando tasa de aprobados por ciclo");

        var estudiantes = _personasService.GetEstudiantesOrderBy(TipoOrdenamiento.Dni, 1, 1000, false).ToList();
        var grouped = estudiantes.GroupBy(e => e.Ciclo).ToList();

        var ciclos = grouped.Select(g => g.Key.ToString()).ToArray();
        var aprobados = new double[ciclos.Length];
        var suspensos = new double[ciclos.Length];

        for (int i = 0; i < grouped.Count; i++) {
            var total = grouped[i].Count();
            var aprob = grouped[i].Count(e => e.Calificacion >= 5);
            var susp = total - aprob;

            aprobados[i] = (double)aprob / total * 100;
            suspensos[i] = (double)susp / total * 100;
        }

        _logger.Information($"Tasa calculada para {ciclos.Length} ciclos");
        return (aprobados, suspensos, ciclos);
    }

    /// <summary>
    ///     Obtiene la distribución por género de estudiantes y docentes
    /// </summary>
    public (int primero, int segundo, string[] ciclos) GetEstudiantesPorCurso() {
        _logger.Information("📊 Calculando estudiantes por curso");

        var estudiantes = _personasService.GetEstudiantesOrderBy(TipoOrdenamiento.Dni, 1, 1000, false).ToList();
        var ciclos = estudiantes.Select(e => e.Ciclo.ToString()).Distinct().ToArray();

        var primero = new int[ciclos.Length];
        var segundo = new int[ciclos.Length];

        for (int i = 0; i < ciclos.Length; i++) {
            var ciclo = Enum.Parse<Ciclo>(ciclos[i]);
            primero[i] = estudiantes.Count(e => e.Ciclo == ciclo && e.Curso == Curso.Primero);
            segundo[i] = estudiantes.Count(e => e.Ciclo == ciclo && e.Curso == Curso.Segundo);
        }

        _logger.Information($"Cursos calculados para {ciclos.Length} ciclos");
        return (primero.Sum(), segundo.Sum(), new[] { "Primero", "Segundo" });
    }
}