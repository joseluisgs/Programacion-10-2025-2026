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
        // Configuración de Serilog:
        // - MinimumLevel.Debug(): captura todos los niveles (Debug, Info, Warning, Error)
        // - WriteTo.Console(): muestra los logs en la consola
        // - WriteTo.File(): guarda los logs en archivos, rotando diariamente
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File("logs/starwars-.log", rollingInterval: RollingInterval.Day)
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
    protected override void OnStartup(StartupEventArgs e)
    {
        // Llamar al método base de WPF para初始化 normal
        base.OnStartup(e);
        
        try
        {
            // Paso 1: Crear la colección de servicios de Microsoft.Extensions.DependencyInjection
            var services = new ServiceCollection();
            
            // Paso 2: Registrar todas las dependencias en el proveedor
            // Este método registra:
            // - DroideGenerator como Singleton
            // - MainViewModel como Transient
            DependenciesProvider.ConfigureServices(services);
            
            // Paso 3: Construir el proveedor de servicios
            // Esto crea el contenedor DI que gestionará todas las dependencias
            _serviceProvider = services.BuildServiceProvider();
            
            Log.Information("Dependencias configuradas");
            
            // Paso 4: Obtener la ventana principal del contenedor DI y mostrarla
            // GetRequiredService<MainWindow>() crea (o recupera si ya existe) la instancia
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            
            Log.Information("Ventana principal mostrada");
        }
        catch (Exception ex)
        {
            // Si ocurre cualquier error durante el inicio, lo registramos y终止 la aplicación
            Log.Fatal(ex, "Error al iniciar la aplicación");
            
            // Mostramos un MessageBox con el error para que el usuario sepa qué pasó
            MessageBox.Show(
                $"Error al iniciar la aplicación: {ex.Message}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            
            // Cerramos la aplicación con código de error 1
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
