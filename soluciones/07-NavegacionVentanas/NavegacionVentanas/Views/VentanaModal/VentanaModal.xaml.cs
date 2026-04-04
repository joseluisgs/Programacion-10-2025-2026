// ============================================================
// VentanaModal.xaml.cs - Ventana modal con DialogResult
// ============================================================
// Esta ventana usa DialogResult para comunicar el resultado a la ventana padre.
//
// CONCEPTO:
// - DialogResult es una propiedad que se puede establecer en una ventana
// - Solo funciona cuando la ventana se abrió con ShowDialog()
// - La ventana padre puede leer el resultado después de ShowDialog()

using System.Windows;

namespace NavegacionVentanas.Views.VentanaModal;

/// <summary>
/// Ventana modal que devuelve DialogResult.
/// </summary>
public partial class VentanaModal : Window
{
    /// <summary>
    /// Constructor de la ventana modal.
    /// </summary>
    public VentanaModal()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Establece DialogResult = true y cierra la ventana.
    /// </summary>
    private void BtnAceptar_Click(object sender, RoutedEventArgs e)
    {
        // Establecer DialogResult a true indica "Aceptar"
        // Esto cierra automáticamente la ventana
        DialogResult = true;
    }

    /// <summary>
    /// Establece DialogResult = false y cierra la ventana.
    /// </summary>
    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        // Establecer DialogResult a false indica "Cancelar"
        // Esto cierra automáticamente la ventana
        DialogResult = false;
    }
}
