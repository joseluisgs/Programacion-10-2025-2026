using System.Windows;
using System.Windows.Controls;
using GestionAcademica.ViewModels.Dashboard;
using GestionAcademica.Views.Backup;
using GestionAcademica.Views.Docentes;
using GestionAcademica.Views.Estudiantes;
using GestionAcademica.Views.Graficos;
using GestionAcademica.Views.Main;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace GestionAcademica.Views.Dashboard;

/// <summary>
///     Página del panel de control (Dashboard).
///     Muestra estadísticas generales del sistema académico.
/// </summary>
public partial class DashboardView : Page {
    /// <summary>
    ///     Inicializa el dashboard y configura el ViewModel correspondiente.
    /// </summary>
    public DashboardView() {
        InitializeComponent();

        var vm = App.ServiceProvider.GetRequiredService<DashboardViewModel>();
        vm.NavigateAction = OnNavigate;
        DataContext = vm;

        Log.Debug("📊 DashboardView cargado");
    }

    /// <summary>
    ///     Navega a la vista especificada desde el dashboard.
    /// </summary>
    private void OnNavigate(string view) {
        var mainWindow = (MainWindow)Window.GetWindow(this);
        switch (view) {
            case "Estudiantes":
                mainWindow.MainFrame.Navigate(new EstudiantesView());
                break;
            case "Docentes":
                mainWindow.MainFrame.Navigate(new DocentesView());
                break;
            case "Graficos":
                mainWindow.MainFrame.Navigate(new GraficosView());
                break;
            case "Backup":
                mainWindow.MainFrame.Navigate(new BackupView());
                break;
        }
    }
}