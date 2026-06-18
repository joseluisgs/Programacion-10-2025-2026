using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GestionAcademica.Config;
using GestionAcademica.Messages;
using GestionAcademica.Models.Academia;
using GestionAcademica.Services.Personas;
using Serilog;

namespace GestionAcademica.ViewModels.Dashboard;

/// <summary>
///     ViewModel para el panel de control (Dashboard).
///     Muestra estadísticas generales del sistema académico.
/// </summary>
public partial class DashboardViewModel : ObservableObject {
    private readonly ILogger _logger = Log.ForContext<DashboardViewModel>();
    private readonly IPersonasService _personasService;

    [ObservableProperty] private string _mensajeEstado = "Cargando...";

    [ObservableProperty] private double _porcentajeAprobados;

    [ObservableProperty] private double _porcentajeSuspensos;

    [ObservableProperty] private int _totalASIR;

    [ObservableProperty] private int _totalDAM;

    [ObservableProperty] private int _totalDAW;

    [ObservableProperty] private int _totalDocentes;

    [ObservableProperty] private int _totalEstudiantes;

    public DashboardViewModel(IPersonasService personasService) {
        _personasService = personasService;
        WeakReferenceMessenger.Default.Register<PersonaCambiadaMessage>(this, (r, m) => { _ = LoadStatisticsAsync(); });
        _ = LoadStatisticsAsync();
    }

    public Action<string>? NavigateAction { get; set; }

    /// <summary>
    ///     Carga las estadísticas del dashboard desde el servicio de personas.
    ///     Calcula totales, porcentajes y distribución por ciclos.
    /// </summary>
    private async Task LoadStatisticsAsync() {
        try {
            _logger.Information("📊 Cargando estadísticas del dashboard...");

            // Usar métodos de conteo específicos en lugar de GetAll()
            TotalEstudiantes = await _personasService.CountEstudiantesAsync();
            TotalDocentes = await _personasService.CountDocentesAsync();

            _logger.Information($"Total estudiantes: {TotalEstudiantes}, Total docentes: {TotalDocentes}");

            var notaCorte = AppConfig.NotaAprobado;
            var aprobados = await _personasService.CountAprobadosAsync(notaCorte);
            var suspensos = await _personasService.CountSuspensosAsync(notaCorte);

            _logger.Information($"Aprobados: {aprobados}, Suspensos: {suspensos}");

            if (TotalEstudiantes > 0) {
                PorcentajeAprobados = Math.Round((double)aprobados / TotalEstudiantes * 100, 1);
                PorcentajeSuspensos = Math.Round((double)suspensos / TotalEstudiantes * 100, 1);
            }
            else {
                PorcentajeAprobados = 0;
                PorcentajeSuspensos = 0;
            }

            var estudiantesPorCiclo = await _personasService.GetEstudiantesPorCicloAsync();
            var docentesPorCiclo = await _personasService.GetDocentesPorCicloAsync();

            TotalDAM = estudiantesPorCiclo.GetValueOrDefault(Ciclo.DAM) + docentesPorCiclo.GetValueOrDefault(Ciclo.DAM);
            TotalDAW = estudiantesPorCiclo.GetValueOrDefault(Ciclo.DAW) + docentesPorCiclo.GetValueOrDefault(Ciclo.DAW);
            TotalASIR = estudiantesPorCiclo.GetValueOrDefault(Ciclo.ASIR) +
                        docentesPorCiclo.GetValueOrDefault(Ciclo.ASIR);

            MensajeEstado = $"📊 Datos actualizados - Estudiantes: {TotalEstudiantes}, Docentes: {TotalDocentes}";
            _logger.Information("✅ Dashboard cargado correctamente con conteos precisos");
        }
        catch (Exception ex) {
            _logger.Error(ex, "❌ Error al cargar estadísticas del dashboard");
            MensajeEstado = "❌ Error al cargar datos";
        }
    }

    [RelayCommand]
    private void AddEstudiante() {
        NavigateAction?.Invoke("Estudiantes");
    }

    [RelayCommand]
    private void AddDocente() {
        NavigateAction?.Invoke("Docentes");
    }

    [RelayCommand]
    private void CreateBackup() {
        NavigateAction?.Invoke("Backup");
    }

    [RelayCommand]
    private void ViewGraficos() {
        NavigateAction?.Invoke("Graficos");
    }

    [RelayCommand]
    private async Task RefrescarAsync() {
        await LoadStatisticsAsync();
    }
}
