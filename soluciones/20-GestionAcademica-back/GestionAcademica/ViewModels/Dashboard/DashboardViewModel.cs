using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionAcademica.Models.Personas;
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
        LoadStatistics();
    }

    private void LoadStatistics()
    {
        try
        {
            var todasPersonas = _personasService.GetAll().ToList();
            var estudiantes = todasPersonas.OfType<Estudiante>().ToList();
            var docentes = todasPersonas.OfType<Docente>().ToList();

            TotalEstudiantes = estudiantes.Count;
            TotalDocentes = docentes.Count;

            var notaCorte = GestionAcademica.Config.AppConfig.NotaAprobado;
            var aprobados = estudiantes.Count(e => e.Calificacion >= notaCorte);
            var suspensos = estudiantes.Count(e => e.Calificacion < notaCorte);

            if (estudiantes.Count > 0)
            {
                PorcentajeAprobados = Math.Round((double)aprobados / estudiantes.Count * 100, 1);
                PorcentajeSuspensos = Math.Round((double)suspensos / estudiantes.Count * 100, 1);
            }

            TotalDAM = estudiantes.Count(e => e.Ciclo == Ciclo.DAM) + docentes.Count(d => d.Ciclo == Ciclo.DAM);
            TotalDAW = estudiantes.Count(e => e.Ciclo == Ciclo.DAW) + docentes.Count(d => d.Ciclo == Ciclo.DAW);
            TotalASIR = estudiantes.Count(e => e.Ciclo == Ciclo.ASIR) + docentes.Count(d => d.Ciclo == Ciclo.ASIR);

            MensajeEstado = "Datos cargados correctamente";
            
            _logger.Information("📊 Dashboard cargado");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "❌ Error al cargar estadísticas");
            MensajeEstado = "Error al cargar datos";
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