// ============================================================
// VentanaNoModal.xaml.cs - Ventana no modal con Show()
// ============================================================
// Esta ventana se abre con Show() y no bloquea la ventana padre.
//
// CONCEPTO:
// - Show() abre una ventana no modal
// - El código siguiente se ejecuta inmediatamente (no espera)
// - Se puede interactuar con todas las ventanas a la vez

using System.ComponentModel;
using System.Windows;

namespace NavegacionVentanas.Views.VentanaNoModal;

/// <summary>
/// Ventana no modal que puede coexistir con otras.
/// </summary>
public partial class VentanaNoModal : Window
{
    /// <summary>
    /// Constructor de la ventana no modal.
    /// </summary>
    public VentanaNoModal()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Cierra la ventana manualmente.
    /// </summary>
    private void BtnCerrar_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    /// <summary>
    /// Se ejecuta cuando la ventana se está cerrando.
    /// </summary>
    private void Window_Closing(object sender, CancelEventArgs e)
    {
        // Aquí se puede cancelar el cierre si es necesario
        // e.Cancel = true; // Cancelaría el cierre
    }
}
