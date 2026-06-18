// App.xaml.cs - Código subyacente de la aplicación
// ===============================================
// Este es el punto de entrada de la aplicación WPF.
//
// CICLO DE VIDA DE LA APLICACIÓN:
// ================================
// 1. App() → Se construye el objeto Application
// 2. OnStartup() → Se está iniciando la aplicación
// 3. MainWindow se crea → Constructor
// 4. MainWindow carga → InitializeComponent()
// 5. MainWindow.Loaded → La ventana es visible
// 6. [Usuario interactúa]
// 7. MainWindow.Closing → Se va a cerrar
// 8. MainWindow.Closed → Se ha cerrado
// 9. OnExit() → La aplicación termina

using System.Diagnostics;
using System.Windows;

namespace LayoutsComponentes;

// Application: clase base para aplicaciones WPF
public partial class App : Application
{
    // Constructor de la aplicación
    public App()
    {
        Debug.WriteLine("🔵 [App] Constructor - Objeto Application creado");
    }

    // OnStartup: Se dispara cuando la aplicación va a iniciar
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        Debug.WriteLine("🔵 [App] OnStartup - La aplicación está iniciando");
    }

    // OnExit: Se dispara cuando la aplicación va a terminar
    protected override void OnExit(ExitEventArgs e)
    {
        Debug.WriteLine("🔴 [App] OnExit - La aplicación está terminando");
        base.OnExit(e);
    }
}

// NOTA: Para ver estos mensajes, consulta la ventana de salida de depuración.
