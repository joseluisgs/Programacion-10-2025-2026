using System.Windows;
using Enrutador.Infrastructure;

namespace Enrutador.Views.Main;

public partial class MainView : Window
{
    public MainView()
    {
        InitializeComponent();
    }

    private void BtnModal_Click(object sender, RoutedEventArgs e)
    {
        // Navegación modal mediante enrutador
        RoutesManager.ShowModal(RoutesManager.View.Modal);
    }

    private void BtnNoModal_Click(object sender, RoutedEventArgs e)
    {
        // Abrir nueva ventana mediante enrutador
        RoutesManager.OpenWindow(RoutesManager.View.NoModal);
    }

    private void BtnLogout_Click(object sender, RoutedEventArgs e)
    {
        // Volver al login cerrando la actual
        RoutesManager.NavigateTo(RoutesManager.View.Login);
    }

    private void BtnSalir_Click(object sender, RoutedEventArgs e)
    {
        RoutesManager.Exit();
    }
}
