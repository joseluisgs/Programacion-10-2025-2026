using System.Windows;
using Enrutador.Infrastructure;

namespace Enrutador;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        // Iniciamos la aplicación con el Enrutador
        // Por defecto arranca en el Login
        RoutesManager.InitMainStage(RoutesManager.View.Login);
    }
}
