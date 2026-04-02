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
        ConfigureSerilog();
        
        Log.Information("🚀 Aplicación WPF iniciada");
        
        ServiceProvider = DependenciesProvider.BuildServiceProvider();
        
        Log.Information("✅ ServiceProvider creado con todos los servicios");
        
        ConfigureExceptionHandling();
        
        var splash = new Views.Splash.SplashWindow();
        Log.Information("📱 Mostrando SplashWindow");
        splash.ShowDialog();
        Log.Information("📱 SplashWindow cerrado");
        
        var mainWindow = new Views.Main.MainWindow();
        MainWindow = mainWindow;
        
        base.OnStartup(e);
        
        Log.Information("🏠 Llamando a mainWindow.Show()");
        mainWindow.Show();
        Log.Information("✅ mainWindow.Show() completado");
    }

    // ====================================================================
    // ConfigureSerilog - Configura el logging
    // ====================================================================
    private void ConfigureSerilog()
    {
        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(
                outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: System.IO.Path.Combine(
                    GestionAcademica.Config.AppConfig.LogDirectory, 
                    "log-.txt"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: GestionAcademica.Config.AppConfig.LogRetainDays,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
        
        Log.Logger = loggerConfiguration.CreateLogger();
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