using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using GestionProductos.Infrastructure;
using GestionProductos.Views.Main;

namespace GestionProductos;

public partial class App : System.Windows.Application
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        // Inicializar tema por defecto (claro)
        ThemeHelper.Initialize();
        
        // Configurar inyección de dependencias
        ServiceProvider = DependenciesProvider.BuildServiceProvider();
        
        // Mostrar ventana principal
        var mainWindow = new MainWindow();
        mainWindow.Show();
    }
}
