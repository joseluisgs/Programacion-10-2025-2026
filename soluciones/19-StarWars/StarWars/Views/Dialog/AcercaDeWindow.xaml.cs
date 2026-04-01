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

    /// <summary>
    /// Maneja la navegación al enlace de GitHub
    /// </summary>
    private void LinkGitHub_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        // Abrir el navegador predeterminado con el enlace
        Process.Start(new ProcessStartInfo
        {
            FileName = e.Uri.AbsoluteUri,
            UseShellExecute = true
        });
        
        // Indicar que hemos manejado el evento
        e.Handled = true;
    }
}
