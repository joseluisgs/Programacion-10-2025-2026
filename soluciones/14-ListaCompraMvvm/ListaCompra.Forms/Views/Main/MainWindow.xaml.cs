using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ListaCompra.FormData.ViewModels;
using ListaCompra.Forms.Views.Dialog;

namespace ListaCompra.Forms.Views.Main;

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
