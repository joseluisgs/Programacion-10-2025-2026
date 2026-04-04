using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace GestionAcademica.Views.AcercaDe;

/// <summary>
/// Ventana de información sobre la aplicación.
/// </summary>
public partial class AcercaDeWindow : Window
{
    /// <summary>
    /// Inicializa la ventana Acerca de.
    /// </summary>
    public AcercaDeWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Abre el enlace a GitHub en el navegador o copia al portapapeles.
    /// </summary>
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

    /// <summary>
    /// Cierra la ventana.
    /// </summary>
    private void OnCerrarClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}