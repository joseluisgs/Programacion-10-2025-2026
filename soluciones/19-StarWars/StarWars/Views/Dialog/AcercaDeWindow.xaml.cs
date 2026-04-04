using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace StarWars.Views.Dialog;

public partial class AcercaDeWindow : Window
{
    public AcercaDeWindow()
    {
        InitializeComponent();
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = e.Uri.AbsoluteUri,
            UseShellExecute = true
        });
        e.Handled = true;
    }
}
