// ============================================================
// VentanaViewModel.xaml.cs - ViewModel compartido
// ============================================================
// Esta ventana usa un ViewModel compartido con la ventana principal.
//
// CONCEPTO:
// - Ambas ventanas comparten la misma instancia del ViewModel
// - Los cambios en una ventana se reflejan automáticamente en la otra
// - El binding de WPF actualiza la UI cuando cambian las propiedades

using System.Windows;
using NavegacionVentanas.ViewModels;

namespace NavegacionVentanas.Views.VentanaViewModel;

/// <summary>
/// Ventana que usa un ViewModel compartido.
/// </summary>
public partial class VentanaViewModel : Window
{
    /// <summary>
    /// ViewModel compartido con la ventana principal.
    /// </summary>
    private readonly SharedViewModel _viewModel;

    /// <summary>
    /// Constructor que recibe el ViewModel.
    /// </summary>
    /// <param name="viewModel">ViewModel compartido.</param>
    public VentanaViewModel(SharedViewModel viewModel)
    {
        InitializeComponent();
        
        // Guardar referencia y establecer como DataContext
        _viewModel = viewModel;
        DataContext = _viewModel;
    }

    /// <summary>
    /// Incrementa el contador del ViewModel.
    /// </summary>
    private void BtnIncrementar_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.IncrementarContador();
    }

    /// <summary>
    /// Reinicia el contador del ViewModel.
    /// </summary>
    private void BtnReiniciar_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.ReiniciarContador();
    }

    /// <summary>
    /// Guarda el dato editado.
    /// </summary>
    private void BtnGuardar_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.DatoCompartido = TxtEditarDato.Text;
    }

    /// <summary>
    /// Cierra la ventana.
    /// </summary>
    private void BtnCerrar_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
