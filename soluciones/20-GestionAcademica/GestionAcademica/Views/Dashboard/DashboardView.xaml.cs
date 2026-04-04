using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using GestionAcademica.ViewModels.Dashboard;
using GestionAcademica.Views.Main;
using GestionAcademica.Services.Personas;
using Serilog;

namespace GestionAcademica.Views.Dashboard;

/// <summary>
/// Página del panel de control (Dashboard).
/// Muestra estadísticas generales del sistema académico.
/// </summary>
public partial class DashboardView : Page
{
    /// <summary>
    /// Inicializa el dashboard y configura el ViewModel correspondiente.
    /// </summary>
    public DashboardView()
    {
        InitializeComponent();
        
        var vm = App.ServiceProvider.GetRequiredService<DashboardViewModel>();
        vm.NavigateAction = OnNavigate;
        DataContext = vm;
        
        Log.Debug("📊 DashboardView cargado");
    }

    /// <summary>
    /// Navega a la vista especificada desde el dashboard.
    /// </summary>
    private void OnNavigate(string view)
    {
        var mainWindow = (MainWindow)System.Windows.Window.GetWindow(this);
        switch (view)
        {
            case "Estudiantes":
                mainWindow.MainFrame.Navigate(new Estudiantes.EstudiantesView());
                break;
            case "Docentes":
                mainWindow.MainFrame.Navigate(new Docentes.DocentesView());
                break;
            case "Graficos":
                mainWindow.MainFrame.Navigate(new Graficos.GraficosView());
                break;
            case "Backup":
                mainWindow.MainFrame.Navigate(new Backup.BackupView());
                break;
        }
    }
}