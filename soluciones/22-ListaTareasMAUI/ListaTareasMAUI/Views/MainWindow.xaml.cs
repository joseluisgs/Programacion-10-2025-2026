using ListaTareasMAUI.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ListaTareasMAUI.Views;

/// <summary>
/// Code-behind de la ventana principal.
/// </summary>
/// <remarks>
/// En WPF, el code-behind contiene código que necesita acceso
/// directo a la UI o eventos que no pertenecen al ViewModel.
///
/// En este caso, usamos los eventos Checked/Unchecked del CheckBox
/// para capturar los cambios y llamar al comando del ViewModel.
/// </remarks>
public partial class MainWindow : Window
{
    /// <summary>
    /// Constructor de la ventana.
    /// </summary>
    /// <param name="viewModel">ViewModel inyectado mediante DI</param>
    /// <remarks>
    /// El ViewModel se inyecta aquí, no se crea manualmente.
    /// Esto permite que el ViewModel tenga sus dependencias (servicios)
    /// inyectados también.
    /// </remarks>
    public MainWindow(MainViewModel viewModel)
    {
        // Inicializar los componentes de la UI (definidos en XAML)
        InitializeComponent();
        
        // Establecer el DataContext para los bindings
        // Esto conecta todas las propiedades y comandos del XAML con el ViewModel
        DataContext = viewModel;
    }

    /// <summary>
    /// Maneja el evento de cambio del CheckBox (Checked y Unchecked).
    /// </summary>
    /// <param name="sender">El CheckBox que generó el evento</param>
    /// <param name="e">Argumentos del evento</param>
    /// <remarks>
    /// Este método se llama cuando el usuario marca/desmarca el CheckBox.
    /// Obtainemos la tarea del DataContext del CheckBox (que es el objeto Tarea)
    /// y llamamos al comando ToggleTarea del ViewModel.
    /// </remarks>
    private void CheckBox_Changed(object sender, RoutedEventArgs e)
    {
        // El sender es el CheckBox, su DataContext es la tarea
        if (sender is CheckBox checkBox && checkBox.DataContext is Models.Tarea tarea)
        {
            // Llamar al comando del ViewModel
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.ToggleTareaCommand.Execute(tarea);
            }
        }
    }
}