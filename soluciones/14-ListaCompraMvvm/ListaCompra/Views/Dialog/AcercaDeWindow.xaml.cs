using System.Windows;

namespace ListaCompra.Views.Dialog;

public partial class AcercaDeWindow : Window
{
    public AcercaDeWindow()
    {
        InitializeComponent();
    }

    private void Cerrar_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
