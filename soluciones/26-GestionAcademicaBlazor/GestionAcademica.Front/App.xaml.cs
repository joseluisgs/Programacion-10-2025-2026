using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using GestionAcademica.Config;
using GestionAcademica.Infrastructure.FrontDependenciesProvider;
using GestionAcademica.Views.Main;
using GestionAcademica.Views.Splash;
using Serilog;
using Serilog.Debugging;

namespace GestionAcademica;

/// <summary>
///     Punto de entrada de la aplicación WPF.
///     Configura la inyección de dependencias, Serilog y los manejadores de excepciones.
/// </summary>
public partial class App : Application {
    /// <summary>
    ///     Proveedor de servicios global accesible desde toda la aplicación.
    /// </summary>
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    /// <summary>
    ///     Método ejecutado al iniciar la aplicación.
    ///     Configura el directorio de trabajo, Serilog, DI y muestra las ventanas.
    /// </summary>
    protected override void OnStartup(StartupEventArgs e) {
        // Establecer directorio de trabajo como el directorio base de la aplicación
        Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

        // Configurar Serilog para logging estructurado
        ConfigureSerilog();

        Log.Information("Aplicacion WPF iniciada (Modo Front/Back)");

        // Crear el ServiceProvider con Back + Front
        ServiceProvider = FrontDependenciesProvider.BuildServiceProvider();

        Log.Information("ServiceProvider creado con todos los servicios");

        // Configurar manejadores de excepciones no controladas
        ConfigureExceptionHandling();

        // Mostrar ventana de splash mientras carga datos
        var splash = new SplashWindow();
        Log.Information("Mostrando SplashWindow");
        splash.ShowDialog();
        Log.Information("SplashWindow cerrado");

        // Crear y mostrar ventana principal
        var mainWindow = new MainWindow();
        MainWindow = mainWindow;

        Log.Information("Llamando a mainWindow.Show()");
        mainWindow.Show();

        base.OnStartup(e);

        Log.Information("mainWindow.Show() completado");
    }

    /// <summary>
    ///     Configura Serilog lendo la configuración de appsettings.json.
    /// </summary>
    private void ConfigureSerilog() {
        // Habilitar SelfLog para depuración de Serilog
        SelfLog.Enable(msg => Debug.WriteLine($"SERILOG DIAG: {msg}"));

        // Configurar logger desde JSON
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(AppConfig.Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        Log.Information("Serilog inicializado desde JSON");
    }

    /// <summary>
    ///     Configura los manejadores de excepciones para logging y recuperación.
    /// </summary>
    private void ConfigureExceptionHandling() {
        // Excepciones en el hilo de UI
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        // Excepciones en el hilo principal
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        // Excepciones en tareas asíncronas
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
    }

    /// <summary>
    ///     Maneja excepciones no controladas en el hilo de UI.
    ///     Muestra un MessageBox y marca la excepción como manejada.
    /// </summary>
    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
        Log.Fatal(e.Exception, "Excepcion no manejada");
        MessageBox.Show(
            $"Error: {e.Exception.Message}",
            "Error",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
        e.Handled = true;
    }

    /// <summary>
    ///     Maneja excepciones no controladas en el hilo principal.
    /// </summary>
    private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e) {
        Log.Fatal(e.ExceptionObject as Exception, "Exposicion no manejada");
    }

    /// <summary>
    ///     Maneja excepciones no observadas en tareas asíncronas.
    /// </summary>
    private static void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e) {
        Log.Error(e.Exception, "Excepcion en tarea");
        e.SetObserved();
    }

    /// <summary>
    ///     Método ejecutado al cerrar la aplicación.
    ///     Libera recursos y cierra los logs.
    /// </summary>
    protected override void OnExit(ExitEventArgs e) {
        Log.Information("Aplicacion cerrandose");
        Log.CloseAndFlush();

        // Disponer el ServiceProvider si implementa IDisposable
        if (ServiceProvider is IDisposable disposable) disposable.Dispose();

        base.OnExit(e);
    }
}