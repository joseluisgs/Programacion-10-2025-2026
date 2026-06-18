// ============================================================
// App.xaml.cs - Punto de entrada de la aplicación
// ============================================================
// Este proyecto muestra diferentes técnicas de navegación entre ventanas.
//
// CONCEPTOS:
// - Show(): Abre ventana no modal (puede interactuar con otras)
// - ShowDialog(): Abre ventana modal (bloquea la padre)
// - DialogResult: Devuelve resultado de ventana modal
// - ViewModel compartido: Comunicación entre ventanas

using System.Windows;
using NavegacionVentanas.ViewModels;

namespace NavegacionVentanas;

/// <summary>
/// Clase principal de la aplicación.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// ViewModel compartido entre ventanas (demo avanzada).
    /// Se crea aquí para que ambas ventanas tengan la misma instancia.
    /// </summary>
    public static SharedViewModel? SharedViewModel { get; private set; }

    /// <summary>
    /// Se ejecuta al iniciar la aplicación.
    /// </summary>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        // Inicializar el ViewModel compartido
        // Este ViewModel se pasará a las ventanas que lo necesiten
        SharedViewModel = new SharedViewModel();
    }
}