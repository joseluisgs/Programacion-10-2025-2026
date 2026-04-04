// ============================================================
// App.xaml.cs - Punto de entrada de la aplicación WPF
// ============================================================
// Configura la inyección de dependencias y Serilog al iniciar.
//
// CONCEPTOS IMPORTANTES:
//
// 1. WPF (Windows Presentation Foundation):
//    - Framework de Microsoft para crear aplicaciones de escritorio
//    - Usa XAML para definir la interfaz gráfica
//    - Permite separar la UI del código (code-behind)
//
// 2. Application:
//    - Clase base para toda aplicación WPF
//    - Controla el ciclo de vida de la aplicación
//    - Tiene eventos como OnStartup, OnExit
//
// 3. INYECCIÓN DE DEPENDENCIAS (DI):
//    - Patrón de diseño que permite gestionar dependencias
//    - DependenciesProvider registra los servicios
//    - ServiceProvider proporciona las instancias
//    - ViewModels reciben servicios por constructor
//
// 4. SERILOG (LOGGING):
//    - Configuración centralizada de logs
//    - Console y File sinks
//    - Se configura en OnStartup
//    - Se cierra en OnExit
//
// PUNTO DE ENTRADA:
// 1. Se crea el objeto Application (Main() generado automáticamente)
// 2. Se ejecuta OnStartup() <- aquí configuramos Serilog y DI
// 3. Se crea el ServiceProvider con las dependencias
// 4. Se abre la ventana principal (MainWindow)
// 5. Cuando se cierra la última ventana, se ejecuta OnExit()

using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ListaCompra.Config;
using ListaCompra.Infrastructure;
using ListaCompra.Views.Main;
using Serilog;

namespace ListaCompra;

public partial class App : Application
{
    // ============================================================
    // SERVICE PROVIDER ESTÁTICO
    // ============================================================
    // Se hace público y estático para que todas las ventanas puedan
    // acceder a los servicios registrados sin necesidad de recibirlos por constructor.
    //
    // Ejemplo de uso en una ventana:
    //   var servicio = App.ServiceProvider.GetRequiredService<IProductoService>();
    //
    /// <summary>
    /// Proveedor de servicios con todas las dependencias registradas.
    /// </summary>
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        ConfigureSerilog();
        
        Log.Information("🚀 Aplicación iniciada");
        
        ServiceProvider = DependenciesProvider.BuildServiceProvider();
        
        Log.Information("✅ ServiceProvider creado");
        
        var mainWindow = new MainWindow();
        mainWindow.Show();
    }

    private void ConfigureSerilog()
    {
        var logDir = System.IO.Path.Combine(AppConfig.LogDirectory, "");
        System.IO.Directory.CreateDirectory(logDir);
        
        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Debug();

        if (AppConfig.LogToConsole)
        {
            loggerConfiguration.WriteTo.Console(
                outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
        }

        if (AppConfig.LogToFile)
        {
            loggerConfiguration.WriteTo.File(
                path: System.IO.Path.Combine(AppConfig.LogDirectory, "log-.txt"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: AppConfig.LogRetainDays,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                restrictedToMinimumLevel: AppConfig.LogLevel);
        }

        Log.Logger = loggerConfiguration.CreateLogger();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.Information("🔴 Aplicación cerrando");
        Log.CloseAndFlush();
        
        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
        
        base.OnExit(e);
    }
}
