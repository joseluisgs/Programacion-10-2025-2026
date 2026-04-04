using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using GestionAcademica.ViewModels;
using Serilog;

namespace GestionAcademica.Views.Main;

/// <summary>
/// Ventana principal de la aplicación.
/// Gestiona la navegación entre vistas y las acciones del menú.
/// </summary>
public partial class MainWindow : Window
{
    private bool _exitConfirmedViaMenu = false;

    public MainWindow()
    {
        InitializeComponent();
        
        var viewModel = App.ServiceProvider.GetRequiredService<MainViewModel>();
        DataContext = viewModel;
        
        viewModel.OnNavigateRequested += OnNavigateRequested;
        
        Log.Information("🏠 MainWindow inicializada");
        
        MainFrame.Navigate(new Dashboard.DashboardView());
        
        DeleteTypeText.Text = $"Borrado: {(Config.AppConfig.UseLogicalDelete ? "Lógico" : "Físico")}";
        
        Closing += (s, e) =>
        {
            if (_exitConfirmedViaMenu) return;
            
            var result = MessageBox.Show(
                "¿Está seguro de que desea salir de la aplicación?",
                "Confirmar salida",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                _exitConfirmedViaMenu = true;
            }
        };
    }

    private void OnNavigateRequested(Page page)
    {
        MainFrame.Navigate(page);
    }

    protected override void OnClosed(EventArgs e)
    {
        Log.Information("🏁 MainWindow cerrada");
        base.OnClosed(e);
        Application.Current.Shutdown();
    }

    // ==================== MENU ====================
    
    private void OnSalirClick(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            "¿Está seguro de que desea salir de la aplicación?",
            "Confirmar salida",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            Log.Information("👋 Usuario cerró la aplicación desde el menú");
            _exitConfirmedViaMenu = true;
            Application.Current.Shutdown();
        }
    }

    private void OnExportarClick(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new ImportExport.ImportExportView());
    }

    private void OnImportarClick(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new ImportExport.ImportExportView());
    }

    private void OnCrearBackupClick(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new Backup.BackupView());
    }

    private void OnRestaurarBackupClick(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new Backup.BackupView());
    }

    private void OnEstudiantesClick(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new Estudiantes.EstudiantesView());
    }

    private void OnDocentesClick(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new Docentes.DocentesView());
    }

    private void OnInformesClick(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new Informes.InformesView());
    }

    private void OnGraficosClick(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new Graficos.GraficosView());
    }

    private void OnBackupClick(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new Backup.BackupView());
    }

    private void OnImportExportClick(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new ImportExport.ImportExportView());
    }

    private void OnAcercaDeClick(object sender, RoutedEventArgs e)
    {
        var aboutWindow = new Views.AcercaDe.AcercaDeWindow();
        aboutWindow.Owner = this;
        aboutWindow.ShowDialog();
    }

    private void OnConfiguracionClick(object sender, RoutedEventArgs e)
    {
        var tipoBorrado = GestionAcademica.Config.AppConfig.UseLogicalDelete ? "Lógico" : "Físico";
        MessageBox.Show(
            "Configuración de la aplicación\n\n" +
            $"Repositorio: {GestionAcademica.Config.AppConfig.RepositoryType.ToUpper()}\n" +
            $"Storage: {GestionAcademica.Config.AppConfig.StorageType.ToUpper()}\n" +
            $"Directorio: {GestionAcademica.Config.AppConfig.DataFolder}\n" +
            $"Tipo de borrado: {tipoBorrado}",
            "Configuración",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    // ==================== NAVIGATION ====================
    
    private void OnDashboardClick(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new Dashboard.DashboardView());
    }
}