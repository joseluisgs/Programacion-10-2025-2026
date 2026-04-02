// ============================================================
// App.xaml.cs - Punto de entrada de la aplicacion
// ============================================================
// Este archivo es el punto de entrada de la aplicacion WPF.
//
// En WPF, el metodo Main() se genera automaticamente.
// Este archivo contiene la clase App que se instancia al iniciar.

using System.Windows;

namespace WpfMVVMBasico;

/// <summary>
/// Clase principal de la aplicacion WPF.
/// Gestiona el ciclo de vida de la aplicacion.
/// </summary>
public partial class App : Application
{
    // No hay codigo especial aqui porque:
    // - StartupUri en App.xaml indica que ventana abrir
    // - MainWindow.xaml.cs establece el DataContext
    // - No necesitamos sobreescribir OnStartup porque no hay configuracion extra
}
