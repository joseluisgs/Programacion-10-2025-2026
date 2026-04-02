// App.xaml.cs - Código subyacente de la aplicación
// ===============================================
// Este es el punto de entrada de la aplicación WPF.
//
// En WPF:
// - App.xaml define los recursos y configuración de la aplicación
// - App.xaml.cs contiene el código subyacente (lógica)
// - StartupUri en App.xaml indica qué ventana abrir al inicio
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

namespace IntroWPF;

// Application: clase base para aplicaciones WPF
public partial class App : Application
{
    // ============================================================
    // CONSTRUCTOR DE LA APLICACIÓN
    // ============================================================
    // El constructor se ejecuta cuando se crea el objeto Application
    public App()
    {
        Debug.WriteLine("🔵 [App] Constructor - Objeto Application creado");
    }

    // ============================================================
    // OnStartup: Se dispara cuando la aplicación va a iniciar
    // ============================================================
    // Es virtual (se puede sobrescribir) y se llama antes de Show()
    protected override void OnStartup(StartupEventArgs e)
    {
        // Llamar al método base first
        base.OnStartup(e);
        
        Debug.WriteLine("🔵 [App] OnStartup - La aplicación está iniciando");
        Debug.WriteLine("   Argumentos de línea de comandos: " + 
            (e.Args.Length > 0 ? string.Join(", ", e.Args) : "ninguno"));
    }

    // ============================================================
    // OnExit: Se dispara cuando la aplicación va a terminar
    // ============================================================
    // Es virtual y se llama cuando la aplicación termina
    protected override void OnExit(ExitEventArgs e)
    {
        Debug.WriteLine("🔴 [App] OnExit - La aplicación está terminando");
        Debug.WriteLine("   Código de salida: " + e.ApplicationExitCode);
        
        // Llamar al método base al final
        base.OnExit(e);
    }
}

// ============================================================
// NOTA IMPORTANTE
// ============================================================
// Para ver estos mensajes, ejecuta la aplicación y consulta la ventana de salida de depuración.
