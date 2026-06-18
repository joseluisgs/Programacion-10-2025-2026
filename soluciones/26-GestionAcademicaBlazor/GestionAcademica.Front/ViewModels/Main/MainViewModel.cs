// ====================================================================
// MainViewModel.cs - ViewModel principal usando CommunityToolkit.Mvvm
// ====================================================================

using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionAcademica.Services.Backup;
using GestionAcademica.Services.Dialogs;
using GestionAcademica.Services.ImportExport;
using GestionAcademica.Services.Personas;
using GestionAcademica.Services.Report;
using GestionAcademica.Views.AcercaDe;
using GestionAcademica.Views.Backup;
using GestionAcademica.Views.Dashboard;
using GestionAcademica.Views.Docentes;
using GestionAcademica.Views.Estudiantes;
using GestionAcademica.Views.Graficos;
using GestionAcademica.Views.ImportExport;
using GestionAcademica.Views.Informes;
using Serilog;

namespace GestionAcademica.ViewModels;

/// <summary>
///     ViewModel principal de la aplicación.
///     Maneja la navegación entre vistas y acciones del menú.
/// </summary>
public partial class MainViewModel(
    IPersonasService personasService,
    IBackupService backupService,
    IReportService reportService,
    IImportExportService importExportService,
    IDialogService dialogService
) : ObservableObject {
    // ====================================================================
    // EVENTO DE NAVEGACIÓN
    // ====================================================================
    public delegate void NavigateDelegate(Page page);

    private readonly IBackupService _backupService = backupService;
    private readonly IDialogService _dialogService = dialogService;
    private readonly IImportExportService _importExportService = importExportService;

    private readonly ILogger _logger = Log.ForContext<MainViewModel>();

    // ====================================================================
    // DEPENDENCIAS - Servicios inyectados
    // ====================================================================
    private readonly IPersonasService _personasService = personasService;
    private readonly IReportService _reportService = reportService;

    // ====================================================================
    // PROPIEDADES OBSERVABLES
    // ====================================================================

    [ObservableProperty] private bool _isDarkTheme = true;

    [ObservableProperty] private bool _isLoading;

    [ObservableProperty] private string _statusMessage = "Listo";

    // ====================================================================
    // INICIALIZACIÓN
    // ====================================================================

    private void OnInitialized() {
        _logger.Information("✅ MainViewModel inicializado");
    }

    public event NavigateDelegate? OnNavigateRequested;

    // ====================================================================
    // COMANDOS DE NAVEGACIÓN
    // ====================================================================

    [RelayCommand]
    private void NavigateToDashboard() {
        OnNavigateRequested?.Invoke(new DashboardView());
    }

    [RelayCommand]
    private void NavigateToEstudiantes() {
        OnNavigateRequested?.Invoke(new EstudiantesView());
    }

    [RelayCommand]
    private void NavigateToDocentes() {
        OnNavigateRequested?.Invoke(new DocentesView());
    }

    [RelayCommand]
    private void NavigateToInformes() {
        OnNavigateRequested?.Invoke(new InformesView());
    }

    [RelayCommand]
    private void NavigateToGraficos() {
        OnNavigateRequested?.Invoke(new GraficosView());
    }

    [RelayCommand]
    private void NavigateToBackup() {
        OnNavigateRequested?.Invoke(new BackupView());
    }

    [RelayCommand]
    private void NavigateToImportExport() {
        OnNavigateRequested?.Invoke(new ImportExportView());
    }

    // ====================================================================
    // COMANDOS DEL MENÚ
    // ====================================================================

    [RelayCommand]
    private void CambiarTema() {
        IsDarkTheme = !IsDarkTheme;
        ApplyTheme(IsDarkTheme ? "Dark" : "Light");
    }

    [RelayCommand]
    private async Task SalirAsync() {
        if (await _dialogService.ShowConfirmationAsync("¿Estás seguro de que quieres salir?", "Confirmar salida")) {
            _logger.Information("👋 Usuario cerró la aplicación");
            Application.Current.Shutdown();
        }
    }

    [RelayCommand]
    private void MostrarAcercaDe() {
        var aboutWindow = new AcercaDeWindow();
        aboutWindow.ShowDialog();
    }

    // ====================================================================
    // MÉTODOS AUXILIARES
    // ====================================================================

    private void ApplyTheme(string themeName) {
        try {
            var themeUri = new Uri($"../Themes/{themeName}Theme.xaml", UriKind.Relative);
            var themeDictionary = new ResourceDictionary { Source = themeUri };

            var appResources = Application.Current.Resources.MergedDictionaries;

            for (var i = appResources.Count - 1; i >= 0; i--) {
                var dict = appResources[i];
                if (dict.Source != null && dict.Source.OriginalString.Contains("Theme")) appResources.RemoveAt(i);
            }

            appResources.Add(themeDictionary);

            _logger.Information("✅ Tema cambiado a {Theme}", themeName);
        }
        catch (Exception ex) {
            _logger.Error(ex, "❌ Error al aplicar el tema");
        }
    }
}
