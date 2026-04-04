using System.Windows;

namespace GestionAcademica.Views.Estudiantes;

/// <summary>
/// Ventana modal para ver los detalles de un Estudiante.
/// </summary>
public partial class EstudianteDetailsWindow : Window
{
    /// <summary>
    /// Inicializa la ventana de detalles de estudiante.
    /// </summary>
    public EstudianteDetailsWindow()
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