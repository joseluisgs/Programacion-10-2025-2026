using System.Windows;

namespace Enrutador.Views.NoModal;

public partial class NoModalView : Window
{
    public NoModalView()
    {
        InitializeComponent();
    }

    private void BtnCerrar_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
