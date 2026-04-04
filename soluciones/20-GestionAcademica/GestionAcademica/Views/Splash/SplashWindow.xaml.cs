using System.Windows;
using Serilog;

namespace GestionAcademica.Views.Splash;

/// <summary>
/// Ventana de splash que se muestra al iniciar la aplicación.
/// </summary>
public partial class SplashWindow : Window
{
    /// <summary>
    /// Inicializa la ventana de splash.
    /// </summary>
    public SplashWindow()
    {
        InitializeComponent();
        Loaded += OnWindowLoaded;
    }

    /// <summary>
    /// Simula la carga de la aplicación y cierra la ventana de splash.
    /// </summary>
    private async void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        Log.Information("📱 SplashWindow cargada");
        
        await Task.Delay(2500);
        
        Log.Information("✅ Carga completada");
        Close();
    }
}