// ====================================================================
// MainViewModel.cs - ViewModel principal usando CommunityToolkit.Mvvm
// ====================================================================

using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionAcademica.Services.Personas;
using GestionAcademica.Services.Backup;
using GestionAcademica.Services.Dialogs;
using GestionAcademica.Services.Report;
using GestionAcademica.Services.ImportExport;
using Serilog;

namespace GestionAcademica.ViewModels;

/// <summary>
/// ViewModel principal de la aplicación.
/// Maneja la navegación entre vistas y acciones del menú.
/// </summary>
public partial class MainViewModel(
    IPersonasService personasService,
    IBackupService backupService,
    IReportService reportService,
    IImportExportService importExportService,
    IDialogService dialogService
) : ObservableObject
{
    // ====================================================================
    // DEPENDENCIAS - Servicios inyectados
    // ====================================================================
    private readonly IPersonasService _personasService = personasService;
    private readonly IBackupService _backupService = backupService;
    private readonly IReportService _reportService = reportService;
    private readonly IImportExportService _importExportService = importExportService;
    private readonly IDialogService _dialogService = dialogService;
    private readonly ILogger _logger = Log.ForContext<MainViewModel>();

    // ====================================================================
    // PROPIEDADES OBSERVABLES
    // ====================================================================
    
    [ObservableProperty]
    private bool _isDarkTheme = true;

    [ObservableProperty]
    private string _statusMessage = "Listo";

    [ObservableProperty]
    private bool _isLoading;

    // ====================================================================
    // INICIALIZACIÓN
    // ====================================================================

    private void OnInitialized()
    {
        _logger.Information("✅ MainViewModel inicializado");
    }

    // ====================================================================
    // EVENTO DE NAVEGACIÓN
    // ====================================================================
    public delegate void NavigateDelegate(Page page);
    public event NavigateDelegate? OnNavigateRequested;

    // ====================================================================
    // COMANDOS DE NAVEGACIÓN
    // ====================================================================
    
    [RelayCommand]
    private void NavigateToDashboard()
    {
        OnNavigateRequested?.Invoke(new Views.Dashboard.DashboardView());
    }

    [RelayCommand]
    private void NavigateToEstudiantes()
    {
        OnNavigateRequested?.Invoke(new Views.Estudiantes.EstudiantesView());
    }

    [RelayCommand]
    private void NavigateToDocentes()
    {
        OnNavigateRequested?.Invoke(new Views.Docentes.DocentesView());
    }

    [RelayCommand]
    private void NavigateToInformes()
    {
        OnNavigateRequested?.Invoke(new Views.Informes.InformesView());
    }

    [RelayCommand]
    private void NavigateToGraficos()
    {
        OnNavigateRequested?.Invoke(new Views.Graficos.GraficosView());
    }

    [RelayCommand]
    private void NavigateToBackup()
    {
        OnNavigateRequested?.Invoke(new Views.Backup.BackupView());
    }

    [RelayCommand]
    private void NavigateToImportExport()
    {
        OnNavigateRequested?.Invoke(new Views.ImportExport.ImportExportView());
    }

    // ====================================================================
    // COMANDOS DEL MENÚ
    // ====================================================================

    [RelayCommand]
    private void CambiarTema()
    {
        IsDarkTheme = !IsDarkTheme;
        ApplyTheme(IsDarkTheme ? "Dark" : "Light");
    }

    [RelayCommand]
    private void Salir()
    {
        if (_dialogService.ShowConfirmation("¿Estás seguro de que quieres salir?", "Confirmar salida"))
        {
            _logger.Information("👋 Usuario cerró la aplicación");
            Application.Current.Shutdown();
        }
    }

    [RelayCommand]
    private void MostrarAcercaDe()
    {
        var aboutWindow = new Views.AcercaDe.AcercaDeWindow();
        aboutWindow.ShowDialog();
    }

    // ====================================================================
    // MÉTODOS AUXILIARES
    // ====================================================================
    
    private void ApplyTheme(string themeName)
    {
        try
        {
            var themeUri = new Uri($"../Themes/{themeName}Theme.xaml", UriKind.Relative);
            var themeDictionary = new ResourceDictionary { Source = themeUri };
            
            var appResources = Application.Current.Resources.MergedDictionaries;
            
            for (int i = appResources.Count - 1; i >= 0; i--)
            {
                var dict = appResources[i];
                if (dict.Source != null && dict.Source.OriginalString.Contains("Theme"))
                {
                    appResources.RemoveAt(i);
                }
            }
            
            appResources.Add(themeDictionary);
            
            _logger.Information("✅ Tema cambiado a {Theme}", themeName);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "❌ Error al aplicar el tema");
        }
    }
}