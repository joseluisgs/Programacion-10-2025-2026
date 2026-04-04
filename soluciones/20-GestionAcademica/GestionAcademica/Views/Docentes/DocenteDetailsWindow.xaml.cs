using System.Windows;

namespace GestionAcademica.Views.Docentes;

/// <summary>
/// Ventana modal para ver los detalles de un Docente.
/// </summary>
public partial class DocenteDetailsWindow : Window
{
    /// <summary>
    /// Inicializa la ventana de detalles de docente.
    /// </summary>
    public DocenteDetailsWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Cierra la ventana de detalles.
    /// </summary>
    private void OnCerrarClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}