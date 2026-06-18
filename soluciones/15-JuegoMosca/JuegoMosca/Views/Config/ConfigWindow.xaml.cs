using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using JuegoMosca.ViewModels;
using JuegoMosca.Views.Dialog;
using JuegoMosca.Views.Main;

namespace JuegoMosca.Views.Config;

public partial class ConfigWindow : Window
{
    private readonly ConfigViewModel _viewModel;
    
    public ConfigWindow(ConfigViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
        _viewModel.JuegoIniciado += OnJuegoIniciado;
    }

    private void OnJuegoIniciado()
    {
        var mainWindow = App.ServiceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
        Close();
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
