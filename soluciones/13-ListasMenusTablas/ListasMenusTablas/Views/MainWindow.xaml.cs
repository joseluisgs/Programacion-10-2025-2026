using System.Windows;

namespace ListasMenusTablas.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Actualizar_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Lista actualizada", "Actualizar", 
            MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void AcercaDe_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Listas, Menús y Tablas\nVersión 1.0\nEjemplo educativo de WPF", 
            "Acerca de", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
