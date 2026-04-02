// ====================================================================
// App.xaml.cs - Punto de entrada de la aplicación WPF
// ====================================================================
//
// Este código sigue el mismo patrón que el proyecto Pokedex:
//
// 1. Se define una propiedad estática ServiceProvider
// 2. En OnStartup se inicializa con DependenciesProvider.BuildServiceProvider()
// 3. Desde cualquier ViewModel o View se accede via App.ServiceProvider.GetRequiredService<T>()
//
// Así de sencillo y limpio es usar la inyección de dependencias en WPF.
//
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using GestionAcademica.Infrastructure;
using Serilog;
using Serilog.Events;

namespace GestionAcademica;

/// <summary>
/// Clase principal de la aplicación WPF.
/// Controla el ciclo de vida de la aplicación.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Proveedor de servicios para inyección de dependencias.
    /// Acceso global desde cualquier parte de la app:
    ///   var service = App.ServiceProvider.GetRequiredService<IPersonasService>();
    /// </summary>
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    // ====================================================================
    // OnStartup - Se ejecuta al iniciar la aplicación
    // ====================================================================
    protected override void OnStartup(StartupEventArgs e)
    {
        // Forzamos el directorio actual al del ejecutable para que las 
        // rutas relativas de appsettings.json (log/, data/, etc.) funcionen.
        System.IO.Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

        ConfigureSerilog();
        
        Log.Information("🚀 Aplicación WPF iniciada (Modo Senior: Pure Configuration)");
        
        ServiceProvider = DependenciesProvider.BuildServiceProvider();
        
        Log.Information("✅ ServiceProvider creado con todos los servicios");
        
        ConfigureExceptionHandling();
        
        var splash = new Views.Splash.SplashWindow();
        Log.Information("📱 Mostrando SplashWindow");
        splash.ShowDialog();
        Log.Information("📱 SplashWindow cerrado");
        
        var mainWindow = new Views.Main.MainWindow();
        
        MainWindow = mainWindow;
        
        Log.Information("🏠 Llamando a mainWindow.Show()");
        mainWindow.Show();
        
        base.OnStartup(e);
        
        Log.Information("✅ mainWindow.Show() completado");
    }

    // ====================================================================
    // ConfigureSerilog - Configura el logging (PURO JSON + BLINDAJE)
    // ====================================================================
    private void ConfigureSerilog()
    {
        // Activar el SelfLog de Serilog para ver errores internos en la consola de Debug de VS
        // Si el log en fichero falla, aquí nos dirá POR QUÉ (permisos, ruta, etc.)
        Serilog.Debugging.SelfLog.Enable(msg => System.Diagnostics.Debug.WriteLine($"SERILOG DIAG: {msg}"));

        Log.Logger = new LoggerConfiguration()
            // Esto es vital: le decimos que busque los Sinks (File, Console) 
            // en los ensamblados que ya tenemos cargados en el proyecto.
            .ReadFrom.Configuration(GestionAcademica.Config.AppConfig.Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();
            
        Log.Information("🔍 Serilog inicializado desde JSON");
    }

    // ====================================================================
    // ConfigureExceptionHandling - Maneja errores no controlados
    // ====================================================================
    private void ConfigureExceptionHandling()
    {
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        Log.Fatal(e.Exception, "❌ Excepción no manejada");
        MessageBox.Show(
            $"Error: {e.Exception.Message}",
            "Error",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
        e.Handled = true;
    }

    private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Log.Fatal(e.ExceptionObject as Exception, "❌ Exposición no manejada");
    }

    private static void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        Log.Error(e.Exception, "❌ Excepción en tarea");
        e.SetObserved();
    }

    // ====================================================================
    // OnExit - Se ejecuta al cerrar la aplicación
    // ====================================================================
    protected override void OnExit(ExitEventArgs e)
    {
        Log.Information("👋 Aplicación cerrándose");
        Log.CloseAndFlush();
        
        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
        
        base.OnExit(e);
    }
}