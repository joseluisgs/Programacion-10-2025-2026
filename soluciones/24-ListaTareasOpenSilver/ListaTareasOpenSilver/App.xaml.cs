using System.Windows;
using ListaTareasOpenSilver.Services;
using ListaTareasOpenSilver.ViewModels;
using ListaTareasOpenSilver.Views;

namespace ListaTareasOpenSilver;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var tareaService = new TareaService();
        var viewModel = new MainWindowViewModel(tareaService);
        
        var mainWindow = new MainWindow
        {
            DataContext = viewModel
        };
        
        mainWindow.Show();
    }
}