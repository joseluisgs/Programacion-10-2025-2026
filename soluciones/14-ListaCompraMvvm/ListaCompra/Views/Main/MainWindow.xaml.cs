using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ListaCompra.ViewModels;
using ListaCompra.Views.Dialog;

namespace ListaCompra.Views.Main;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = App.ServiceProvider.GetRequiredService<MainViewModel>();
    }

    private void AcercaDe_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new AcercaDeWindow
        {
            Owner = this
        };
        dialog.ShowDialog();
    }
}
