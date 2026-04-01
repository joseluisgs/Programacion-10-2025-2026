using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using GestionAcademica.ViewModels.Dashboard;
using GestionAcademica.Views.Main;
using GestionAcademica.Services.Personas;
using Serilog;

namespace GestionAcademica.Views.Dashboard;

public partial class DashboardView : Page
{
    public DashboardView()
    {
        InitializeComponent();
        
        var vm = App.ServiceProvider.GetRequiredService<DashboardViewModel>();
        vm.NavigateAction = OnNavigate;
        vm.Initialize();
        DataContext = vm;
        
        Log.Debug("📊 DashboardView cargado");
    }

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