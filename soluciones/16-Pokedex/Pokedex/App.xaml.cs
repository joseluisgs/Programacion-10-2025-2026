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
        
        // 1. Configura Serilog para logging
        ConfigureSerilog();
        
        Log.Information("🚀 Aplicación iniciada");
        
        // 2. Muestra ventana de carga (splash)
        var splash = new SplashWindow();
        splash.Show();
        
        // 3. Inicializa el contenedor de inyección de dependencias
        ServiceProvider = DependenciesProvider.BuildServiceProvider();
        
        Log.Information("✅ ServiceProvider creado");
        
        // 4. Cierra splash y muestra ventana principal
        splash.Close();
        
        var mainWindow = new MainWindow();
        mainWindow.Show();
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
