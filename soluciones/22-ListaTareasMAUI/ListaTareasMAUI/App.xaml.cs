using Microsoft.Extensions.DependencyInjection;
using ListaTareasMAUI.Services;
using ListaTareasMAUI.ViewModels;
using ListaTareasMAUI.Views;
using System.Windows;

namespace ListaTareasMAUI;

/// <summary>
/// Clase principal de la aplicación.
/// </summary>
/// <remarks>
/// Gestiona el ciclo de vida de la aplicación y configura la inyección de dependencias.
/// </remarks>
public partial class App : Application
{
    /// <summary>
    /// Proveedor de servicios para inyección de dependencias.
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Constructor de la aplicación.
    /// </summary>
    /// <remarks>
    /// Configura la inyección de dependencias antes de que la UI se inicialice.
    /// </remarks>
    public App()
    {
        // ============================================================
        // INYECCIÓN DE DEPENDENCIAS EN WPF
        // ============================================================
        
        // Crear el conjunto de servicios
        var services = new ServiceCollection();
        
        // Registrar los servicios
        // AddSingleton: una sola instancia para toda la aplicación
        // AddTransient: nueva instancia cada vez que se solicita
        services.AddSingleton<ITareaService, TareaService>();
        services.AddTransient<MainViewModel>();
        
        // Construir el proveedor de servicios
        _serviceProvider = services.BuildServiceProvider();
    }

    /// <summary>
    /// Se ejecuta cuando la aplicación va a iniciar.
    /// </summary>
    /// <remarks>
    /// Aquí creamos y mostramos la ventana principal,
    /// obteniendo el ViewModel del proveedor de servicios.
    /// </remarks>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        // Obtener el ViewModel del contenedor DI
        var viewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        
        // Crear la ventana principal con el ViewModel
        var mainWindow = new MainWindow(viewModel);
        
        // Mostrar la ventana
        mainWindow.Show();
    }
}