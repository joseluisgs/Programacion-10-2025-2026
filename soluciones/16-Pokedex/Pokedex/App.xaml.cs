// =============================================================================
// PUNTO DE ENTRADA DE LA APLICACIÓN
// =============================================================================
// Configura el logging, inicializa la inyección de dependencias
// y muestra las ventanas (Splash y Main).
// =============================================================================

using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Pokedex.Config;
using Pokedex.Infrastructure;
using Pokedex.Views.Main;
using Pokedex.Views.Splash;
using Serilog;

namespace Pokedex;

/// <summary>
/// Clase principal de la aplicación WPF.
/// Controla el ciclo de vida de la aplicación.
/// </summary>
public partial class App : Application
{
    /// <summary>Proveedor de servicios de inyección de dependencias</summary>
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    /// <summary>
    /// Se ejecuta al iniciar la aplicación.
    /// Configura logging, muestra splash y ventana principal.
    /// </summary>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        // Forzamos el directorio actual al del ejecutable para que las 
        // rutas relativas de appsettings.json (log/, data/, etc.) funcionen.
        System.IO.Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        
        // 1. Configura Serilog para logging
        ConfigureSerilog();
        
        Log.Information("🚀 Aplicación iniciada");
        Log.Information("📂 Directorio base: {BaseDirectory}", AppDomain.CurrentDomain.BaseDirectory);
        Log.Information("📂 Directorio actual: {CurrentDirectory}", Environment.CurrentDirectory);
        
        // Configurar manejo de excepciones
        DispatcherUnhandledException += (s, args) =>
        {
            Log.Fatal(args.Exception, "💥 Excepción no capturada en Dispatcher");
            args.Handled = true;
        };
        
        AppDomain.CurrentDomain.UnhandledException += (s, args) =>
        {
            var ex = args.ExceptionObject as Exception;
            Log.Fatal(ex, "💥 Excepción no capturada en AppDomain");
        };
        
        try
        {
            // 2. Muestra ventana de carga (splash)
            Log.Information("🎬 Creando SplashWindow...");
            var splash = new SplashWindow();
            Log.Information("📺 Mostrando SplashWindow...");
            splash.Show();
            
            // 3. Inicializa el contenedor de inyección de dependencias
            Log.Information("⚙️ Inicializando ServiceProvider...");
            ServiceProvider = DependenciesProvider.BuildServiceProvider();
            
            Log.Information("✅ ServiceProvider creado");
            
            // 4. Crea y establece la ventana principal
            Log.Information("🎬 Creando MainWindow...");
            var mainWindow = new MainWindow();
            MainWindow = mainWindow; // Establecer como ventana principal PRIMERO
            
            Log.Information("📺 Mostrando MainWindow...");
            mainWindow.Show();
            
            // 5. Cierra splash después de mostrar la ventana principal
            Log.Information("🚪 Cerrando SplashWindow...");
            splash.Close();
            
            Log.Information("✅ Aplicación iniciada completamente");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "💥 Error fatal durante el inicio");
            MessageBox.Show($"Error: {ex.Message}\n\n{ex.StackTrace}", "Error Fatal", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(1);
        }
    }

    /// <summary>
    /// Configura Serilog según la configuración de appsettings.json.
    /// </summary>
    private void ConfigureSerilog()
    {
        // Configuración base con nivel debug
        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Debug();

        // Configura logging a consola si está habilitado
        if (AppConfig.LogToConsole)
        {
            loggerConfiguration.WriteTo.Console(
                outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
        }

        // Configura logging a archivo si está habilitado
        if (AppConfig.LogToFile)
        {
            loggerConfiguration.WriteTo.File(
                path: System.IO.Path.Combine(AppConfig.LogDirectory, "log-.txt"),
                rollingInterval: RollingInterval.Day,                      // Un archivo por día
                retainedFileCountLimit: AppConfig.LogRetainDays,          // Días a conservar
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                restrictedToMinimumLevel: AppConfig.LogLevel);
        }

        // Inicializa el logger global
        Log.Logger = loggerConfiguration.CreateLogger();
    }

    /// <summary>
    /// Se ejecuta al cerrar la aplicación.
    /// Limpia recursos y cierra el logger.
    /// </summary>
    protected override void OnExit(ExitEventArgs e)
    {
        Log.Information("🔴 Aplicación cerrando");
        
        // Cierra y flush del logger
        Log.CloseAndFlush();
        
        // Disposing del ServiceProvider si implementa IDisposable
        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
        
        base.OnExit(e);
    }
}
