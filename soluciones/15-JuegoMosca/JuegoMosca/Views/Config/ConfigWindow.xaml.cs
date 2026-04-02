using System.Windows;
using JuegoMosca.ViewModels;
using JuegoMosca.Views.Dialog;

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
        var moscaViewModel = App.ServiceProvider.GetService(typeof(MoscaViewModel)) as MoscaViewModel;
        var mainWindow = new Main.MainWindow(moscaViewModel!);
        mainWindow.Show();
        Close();
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
