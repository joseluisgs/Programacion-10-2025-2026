using System.Windows;
using Microsoft.Extensions.DependencyInjection;
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
        
        _viewModel.MostrarInfo += OnMostrarInfo;
        _viewModel.MostrarWarning += OnMostrarWarning;
        _viewModel.MostrarError += OnMostrarError;
        _viewModel.VolverConfig += OnVolverConfig;
    }

    private void OnVolverConfig()
    {
        Dispatcher.Invoke(() =>
        {
            var configWindow = App.ServiceProvider.GetRequiredService<Views.Config.ConfigWindow>();
            configWindow.Show();
            Close();
        });
    }

    private void OtraPartida_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.VolverConfiguracion();
    }

    private void OnMostrarInfo(string titulo, string mensaje)
    {
        Dispatcher.Invoke(() => MessageBox.Show(mensaje, titulo, MessageBoxButton.OK, MessageBoxImage.Information));
    }

    private void OnMostrarWarning(string titulo, string mensaje)
    {
        Dispatcher.Invoke(() => MessageBox.Show(mensaje, titulo, MessageBoxButton.OK, MessageBoxImage.Warning));
    }

    private void OnMostrarError(string titulo, string mensaje)
    {
        Dispatcher.Invoke(() => MessageBox.Show(mensaje, titulo, MessageBoxButton.OK, MessageBoxImage.Error));
    }

    private void MenuSalir_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("¿Estás seguro de que quieres salir?", "Salir", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
            Application.Current.Shutdown();
    }

    private void MenuAcercaDe_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new AcercaDeWindow { Owner = this };
        dialog.ShowDialog();
    }
}
