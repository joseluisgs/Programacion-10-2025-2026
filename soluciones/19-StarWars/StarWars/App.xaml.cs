using Microsoft.Extensions.DependencyInjection;
using Serilog;
using StarWars.Infrastructure;
using StarWars.ViewModels;
using StarWars.Views.Main;
using System.Windows;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace StarWars;

/// <summary>
/// Clase principal de la aplicación WPF.
/// </summary>
/// <remarks>
/// Esta clase gestiona el ciclo de vida de la aplicación:
/// 1. Configura el logging con Serilog al iniciar
/// 2. Configura la inyección de dependencias
/// 3. Crea y muestra la ventana principal
/// 4. Limpia recursos al cerrar
/// </remarks>
public partial class App : Application
{
    // ============================================
    // ATRIBUTOS
    // ============================================
    
    /// <summary>
    /// Proveedor de servicios para la inyección de dependencias.
    /// Se inicializa en OnStartup y se limpia en OnExit.
    /// </summary>
    private ServiceProvider? _serviceProvider;

    // ============================================
    // CONSTRUCTOR
    // ============================================
    
    /// <summary>
    /// Constructor de la aplicación.
    /// Configura Serilog para el logging desde el momento de creación.
    /// </summary>
    /// <remarks>
    /// Serilog se configura aquí porque necesitamos que el logging esté
    /// disponible lo antes posible para capturar cualquier error durante el inicio.
    /// Se configuran dos destinos:
    /// - Console: para ver logs en la terminal durante desarrollo
    /// - File: para mantener un registro persistente en la carpeta logs/
    /// </remarks>
    public App()
    {
        var logDir = System.IO.Path.Combine(System.Environment.CurrentDirectory, "logs");
        System.IO.Directory.CreateDirectory(logDir);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File(System.IO.Path.Combine(logDir, "starwars-.log"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
            .CreateLogger();
        
        Log.Information("Iniciando aplicación Star Wars");
    }

    // ============================================
    // MÉTODOS DEL CICLO DE VIDA
    // ============================================
    
    /// <summary>
    /// Se ejecuta cuando la aplicación va a iniciar.
    /// Configura la inyección de dependencias y muestra la ventana principal.
    /// </summary>
    /// <param name="e">Argumentos del evento de inicio</param>
    private void App_Startup(object sender, StartupEventArgs e)
    {
        try
        {
            var services = new ServiceCollection();
            DependenciesProvider.ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
            Log.Information("Dependencias configuradas");
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            Log.Information("Ventana principal mostrada");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Error al iniciar la aplicación");
            MessageBox.Show(
                $"Error al iniciar la aplicación: {ex.Message}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown(1);
        }
    }

    /// <summary>
    /// Se ejecuta cuando la aplicación va a cerrar.
    /// Limpia los recursos: cierra los logs y disposa el proveedor de servicios.
    /// </summary>
    /// <param name="e">Argumentos del evento de salida</param>
    protected override void OnExit(ExitEventArgs e)
    {
        Log.Information("Cerrando aplicación Star Wars");
        
        // Cerrar y flush de los logs de Serilog
        // Esto asegura que todos los mensajes pendientes se escriban
        Log.CloseAndFlush();
        
        // Disponer el ServiceProvider para limpiar los recursos
        // Esto es importante para liberar conexiones, etc.
        _serviceProvider?.Dispose();
        
        // Llamar al método base para una limpieza normal
        base.OnExit(e);
    }
}
