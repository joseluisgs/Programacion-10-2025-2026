using System.Windows;
using JuegoMosca.ViewModels;
using JuegoMosca.Views.Dialog;

namespace JuegoMosca.Views.Main;

public partial class MainWindow : Window
{
    private readonly MoscaViewModel _viewModel;
    
    public MainWindow(MoscaViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
        
        _viewModel.MostrarMensaje += OnMostrarMensaje;
    }

    private void OnMostrarMensaje(string titulo, string mensaje)
    {
        MessageBox.Show(mensaje, titulo, MessageBoxButton.OK, 
            titulo.Contains("acertado") ? MessageBoxImage.Information : 
            titulo.Contains("Casi") ? MessageBoxImage.Warning : 
            MessageBoxImage.Error);
    }

    private void MenuSalir_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void MenuAcercaDe_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new AcercaDeWindow
        {
            Owner = this
        };
        dialog.ShowDialog();
    }
}
