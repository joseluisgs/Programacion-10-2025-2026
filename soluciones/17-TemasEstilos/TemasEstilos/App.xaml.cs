using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TemasEstilos.Infrastructure;
using TemasEstilos.ViewModels;
using TemasEstilos.Views;

namespace TemasEstilos;

/// <summary>
/// Clase principal de la aplicación.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Proveedor de servicios para inyección de dependencias.
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Constructor de la aplicación.
    /// </summary>
    public App()
    {
        // Configurar la inyección de dependencias
        var services = new ServiceCollection();
        DependenciesProvider.ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    /// <summary>
    /// Se ejecuta cuando la aplicación va a iniciar.
    /// </summary>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}