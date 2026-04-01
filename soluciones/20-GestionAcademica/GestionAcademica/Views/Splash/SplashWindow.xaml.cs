using System.Windows;
using Serilog;

namespace GestionAcademica.Views.Splash;

public partial class SplashWindow : Window
{
    public SplashWindow()
    {
        InitializeComponent();
        Loaded += OnWindowLoaded;
    }

    private async void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        Log.Information("📱 SplashWindow cargada");
        
        // Simulación de carga
        await Task.Delay(2500);
        
        Log.Information("✅ Carga completada");
        Close();
    }
}