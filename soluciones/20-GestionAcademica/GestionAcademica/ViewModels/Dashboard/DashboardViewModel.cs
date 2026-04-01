using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GestionAcademica.Messages;
using GestionAcademica.Models.Academia;
using GestionAcademica.Services.Personas;
using Serilog;

namespace GestionAcademica.ViewModels.Dashboard;

public partial class DashboardViewModel : ObservableObject
{
    private readonly IPersonasService _personasService;
    private readonly ILogger _logger = Log.ForContext<DashboardViewModel>();

    [ObservableProperty]
    private int _totalEstudiantes;

    [ObservableProperty]
    private int _totalDocentes;

    [ObservableProperty]
    private double _porcentajeAprobados;

    [ObservableProperty]
    private double _porcentajeSuspensos;

    [ObservableProperty]
    private int _totalDAM;

    [ObservableProperty]
    private int _totalDAW;

    [ObservableProperty]
    private int _totalASIR;

    [ObservableProperty]
    private string _mensajeEstado = "Cargando...";

    public DashboardViewModel(IPersonasService personasService)
    {
        _personasService = personasService;

        WeakReferenceMessenger.Default.Register<PersonaCambiadaMessage>(this, (r, m) =>
        {
            LoadStatistics();
        });

        LoadStatistics();
    }

    private void LoadStatistics()
    {
        try
        {
            _logger.Information("📊 Cargando estadísticas del dashboard...");

            // Usar métodos de conteo específicos en lugar de GetAll()
            TotalEstudiantes = _personasService.CountEstudiantes(false);
            TotalDocentes = _personasService.CountDocentes(false);

            _logger.Information($"Total estudiantes: {TotalEstudiantes}, Total docentes: {TotalDocentes}");

            var notaCorte = GestionAcademica.Config.AppConfig.NotaAprobado;
            var aprobados = _personasService.CountAprobados(notaCorte, false);
            var suspensos = _personasService.CountSuspensos(notaCorte, false);

            _logger.Information($"Aprobados: {aprobados}, Suspensos: {suspensos}");

            if (TotalEstudiantes > 0)
            {
                PorcentajeAprobados = Math.Round((double)aprobados / TotalEstudiantes * 100, 1);
                PorcentajeSuspensos = Math.Round((double)suspensos / TotalEstudiantes * 100, 1);
            }
            else
            {
                PorcentajeAprobados = 0;
                PorcentajeSuspensos = 0;
            }

            var estudiantesPorCiclo = _personasService.GetEstudiantesPorCiclo(false);
            var docentesPorCiclo = _personasService.GetDocentesPorCiclo(false);

            TotalDAM = estudiantesPorCiclo.GetValueOrDefault(Ciclo.DAM) + docentesPorCiclo.GetValueOrDefault(Ciclo.DAM);
            TotalDAW = estudiantesPorCiclo.GetValueOrDefault(Ciclo.DAW) + docentesPorCiclo.GetValueOrDefault(Ciclo.DAW);
            TotalASIR = estudiantesPorCiclo.GetValueOrDefault(Ciclo.ASIR) + docentesPorCiclo.GetValueOrDefault(Ciclo.ASIR);

            MensajeEstado = $"📊 Datos actualizados - Estudiantes: {TotalEstudiantes}, Docentes: {TotalDocentes}";
            _logger.Information("✅ Dashboard cargado correctamente con conteos precisos");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "❌ Error al cargar estadísticas del dashboard");
            MensajeEstado = "❌ Error al cargar datos";
        }
    }

    public Action<string>? NavigateAction { get; set; }

    [RelayCommand]
    private void AddEstudiante()
    {
        NavigateAction?.Invoke("Estudiantes");
    }

    [RelayCommand]
    private void AddDocente()
    {
        NavigateAction?.Invoke("Docentes");
    }

    [RelayCommand]
    private void CreateBackup()
    {
        NavigateAction?.Invoke("Backup");
    }

    [RelayCommand]
    private void ViewGraficos()
    {
        NavigateAction?.Invoke("Graficos");
    }

    [RelayCommand]
    private void Refrescar()
    {
        LoadStatistics();
    }
}