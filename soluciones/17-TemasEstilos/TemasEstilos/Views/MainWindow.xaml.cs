using System.Globalization;
using System.Windows;
using System.Windows.Data;
using TemasEstilos.ViewModels;

namespace TemasEstilos.Views;

/// <summary>
/// Code-behind de la ventana principal.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Constructor de la ventana.
    /// </summary>
    /// <param name="viewModel">ViewModel inyectado mediante DI</param>
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    /// <summary>
    /// Se ejecuta cuando se selecciona un tema del ComboBox.
    /// </summary>
    /// <param name="sender">ComboBox que generó el evento</param>
    /// <param name="e">Argumentos del evento</param>
    private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        // Cuando el usuario selecciona un tema del ComboBox, aplicar automáticamente
        if (DataContext is MainViewModel viewModel && e.AddedItems.Count > 0)
        {
            viewModel.AplicarTemaSeleccionadoCommand.Execute(null);
        }
    }
}