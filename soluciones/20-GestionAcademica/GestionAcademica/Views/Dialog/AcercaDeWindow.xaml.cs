using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace GestionAcademica.Views.Dialog;

public partial class AcercaDeWindow : Window
{
    public AcercaDeWindow()
    {
        InitializeComponent();
    }

    private void OnGitHubClick(object sender, MouseButtonEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/joseluisgs",
                UseShellExecute = true
            });
        }
        catch
        {
            Clipboard.SetText("https://github.com/joseluisgs");
            MessageBox.Show("El enlace se ha copiado al portapapeles.", "GitHub", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void OnCerrarClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}