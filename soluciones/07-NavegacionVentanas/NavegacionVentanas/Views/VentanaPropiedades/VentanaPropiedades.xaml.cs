// ============================================================
// VentanaPropiedades.xaml.cs - Paso de datos por propiedades
// ============================================================
// Esta ventana recibe datos a través de propiedades públicas.
//
// CONCEPTO:
// - Las propiedades se establecen ANTES de Show() o ShowDialog()
// - Más flexible que el constructor para datos opcionales
// - Ideal cuando hay muchos parámetros o algunos son opcionales

using System.Windows;

namespace NavegacionVentanas.Views.VentanaPropiedades;

/// <summary>
/// Ventana que recibe datos por propiedades.
/// </summary>
public partial class VentanaPropiedades : Window
{
    /// <summary>
    /// Título personalizado de la ventana.
    /// </summary>
    public string TituloPersonalizado
    {
        get => Title;
        set => Title = value;
    }

    /// <summary>
    /// Dato principal que se mostrará.
    /// </summary>
    public string DatoPrincipal
    {
        get => TxtDato.Text;
        set => TxtDato.Text = value;
    }

    /// <summary>
    /// Mensaje que el usuario puede editar.
    /// </summary>
    public string Mensaje
    {
        get => TxtMensaje.Text;
        set => TxtMensaje.Text = value;
    }

    /// <summary>
    /// Constructor de la ventana.
    /// </summary>
    public VentanaPropiedades()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Aceptar y cerrar.
    /// </summary>
    private void BtnAceptar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    /// <summary>
    /// Cancelar y cerrar.
    /// </summary>
    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
