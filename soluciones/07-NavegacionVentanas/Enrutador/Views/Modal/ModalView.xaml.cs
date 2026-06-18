using System.Windows;

namespace Enrutador.Views.Modal;

public partial class ModalView : Window
{
    public ModalView()
    {
        InitializeComponent();
    }

    private void BtnAceptar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }
}
