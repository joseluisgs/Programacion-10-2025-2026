// =============================================================================
// PUNTO DE ENTRADA DE LA APLICACIÓN WPF
// =============================================================================

using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TareasBackground.Infrastructure;
using TareasBackground.Views;
using Serilog;

namespace TareasBackground;

/// <summary>
/// Clase principal de la aplicación.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Proveedor de servicios de inyección de dependencias.
    /// </summary>
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    /// <summary>
    /// Se ejecuta cuando la aplicación inicia.
    /// </summary>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        // Configuramos Serilog para logging
        ConfigureSerilog();
        
        Log.Information("🚀 Aplicación iniciada");
        
        // Creamos el contenedor de dependencias
        ServiceProvider = DependenciesProvider.BuildServiceProvider();
        
        Log.Information("✅ ServiceProvider creado");
    }

    /// <summary>
    /// Evento Startup definido en App.xaml.
    /// Muestra la ventana principal.
    /// </summary>
    private void App_Startup(object sender, StartupEventArgs e)
    {
        var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    /// <summary>
    /// Configura Serilog para el logging.
    /// </summary>
    private void ConfigureSerilog()
    {
        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(
                outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: "logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}");

        Log.Logger = loggerConfiguration.CreateLogger();
    }

    /// <summary>
    /// Se ejecuta cuando la aplicación se cierra.
    /// </summary>
    protected override void OnExit(ExitEventArgs e)
    {
        Log.Information("🔴 Aplicación cerrando");
        Log.CloseAndFlush();
        base.OnExit(e);
    }
}
