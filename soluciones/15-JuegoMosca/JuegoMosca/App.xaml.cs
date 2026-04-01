// =============================================================================
// PUNTO DE ENTRADA DE LA APLICACIÓN WPF
// =============================================================================
// Este archivo contiene el código que se ejecuta al iniciar la aplicación.
// Configura el logging, crea el contenedor de dependencias y muestra la primera ventana.
// =============================================================================

using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using JuegoMosca.Infrastructure;
using JuegoMosca.Views.Config;
using Serilog;

namespace JuegoMosca;

/// <summary>
/// Clase principal de la aplicación WPF.
/// Hereda de Application que es la clase base de toda aplicación WPF.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Proveedor de servicios de inyección de dependencias.
    /// Permite obtener instancias de cualquier clase registrada.
    /// </summary>
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    /// <summary>
    /// Se ejecuta cuando la aplicación va a iniciar.
    /// Configura el logging y crea el ServiceProvider.
    /// </summary>
    protected override void OnStartup(StartupEventArgs e)
    {
        // Configuramos Serilog para el logging
        ConfigureSerilog();
        
        Log.Information("🚀 Aplicación iniciada");
        
        // Creamos el contenedor de inyección de dependencias
        ServiceProvider = DependenciesProvider.BuildServiceProvider();
        
        Log.Information("✅ ServiceProvider creado");
        
        // Llamamos al método base para completar el inicio
        base.OnStartup(e);
    }

    /// <summary>
    /// Configura Serilog para el logging de la aplicación.
    /// Escribe logs tanto en consola como en archivo.
    /// </summary>
    private void ConfigureSerilog()
    {
        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Debug()  // Nivel mínimo de logs
            // Escribir en consola
            .WriteTo.Console(
                outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            // Escribir en archivo (un archivo por día)
            .WriteTo.File(
                path: "logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}");

        // Inicializamos el logger global
        Log.Logger = loggerConfiguration.CreateLogger();
    }

    /// <summary>
    /// Se ejecuta cuando la aplicación se cierra.
    /// Cierra el logger correctamente.
    /// </summary>
    protected override void OnExit(ExitEventArgs e)
    {
        Log.Information("🔴 Aplicación cerrando");
        
        // Cerramos y flush del logger para asegurar que se escriban todos los logs
        Log.CloseAndFlush();
        
        base.OnExit(e);
    }

    /// <summary>
    /// Manejador del evento Startup definido en App.xaml.
    /// Muestra la ventana de configuración al iniciar.
    /// </summary>
    private void App_Startup(object sender, StartupEventArgs e)
    {
        // Obtenemos la ventana de configuración del ServiceProvider
        var configWindow = ServiceProvider.GetRequiredService<ConfigWindow>();
        
        // Mostramos la ventana
        configWindow.Show();
    }
}
