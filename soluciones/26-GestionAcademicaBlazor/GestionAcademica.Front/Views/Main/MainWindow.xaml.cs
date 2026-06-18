using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GestionAcademica.Config;
using GestionAcademica.ViewModels;
using GestionAcademica.Views.AcercaDe;
using GestionAcademica.Views.Backup;
using GestionAcademica.Views.Dashboard;
using GestionAcademica.Views.Docentes;
using GestionAcademica.Views.Estudiantes;
using GestionAcademica.Views.Graficos;
using GestionAcademica.Views.ImportExport;
using GestionAcademica.Views.Informes;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace GestionAcademica.Views.Main;

/// <summary>
///     Ventana principal de la aplicación.
///     Gestiona la navegación entre vistas y las acciones del menú.
/// </summary>
public partial class MainWindow : Window {
    private bool _exitConfirmedViaMenu;

    public MainWindow() {
        InitializeComponent();

        var viewModel = App.ServiceProvider.GetRequiredService<MainViewModel>();
        DataContext = viewModel;

        viewModel.OnNavigateRequested += OnNavigateRequested;

        Log.Information("🏠 MainWindow inicializada");

        MainFrame.Navigate(new DashboardView());

        DeleteTypeText.Text = $"Borrado: {(AppConfig.UseLogicalDelete ? "Lógico" : "Físico")}";

        Closing += (s, e) => {
            if (_exitConfirmedViaMenu) return;

            var result = MessageBox.Show(
                "¿Está seguro de que desea salir de la aplicación?",
                "Confirmar salida",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
                e.Cancel = true;
            else
                _exitConfirmedViaMenu = true;
        };
    }

    private void OnNavigateRequested(Page page) {
        MainFrame.Navigate(page);
    }

    protected override void OnClosed(EventArgs e) {
        Log.Information("🏁 MainWindow cerrada");
        base.OnClosed(e);
        Application.Current.Shutdown();
    }

    // ==================== MENU ====================

    private void OnSalirClick(object sender, RoutedEventArgs e) {
        var result = MessageBox.Show(
            "¿Está seguro de que desea salir de la aplicación?",
            "Confirmar salida",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes) {
            Log.Information("👋 Usuario cerró la aplicación desde el menú");
            _exitConfirmedViaMenu = true;
            Application.Current.Shutdown();
        }
    }

    private void OnExportarClick(object sender, RoutedEventArgs e) {
        MainFrame.Navigate(new ImportExportView());
    }

    private void OnImportarClick(object sender, RoutedEventArgs e) {
        MainFrame.Navigate(new ImportExportView());
    }

    private void OnCrearBackupClick(object sender, RoutedEventArgs e) {
        MainFrame.Navigate(new BackupView());
    }

    private void OnRestaurarBackupClick(object sender, RoutedEventArgs e) {
        MainFrame.Navigate(new BackupView());
    }

    private void OnEstudiantesClick(object sender, RoutedEventArgs e) {
        MainFrame.Navigate(new EstudiantesView());
    }

    private void OnDocentesClick(object sender, RoutedEventArgs e) {
        MainFrame.Navigate(new DocentesView());
    }

    private void OnInformesClick(object sender, RoutedEventArgs e) {
        MainFrame.Navigate(new InformesView());
    }

    private void OnGraficosClick(object sender, RoutedEventArgs e) {
        MainFrame.Navigate(new GraficosView());
    }

    private void OnBackupClick(object sender, RoutedEventArgs e) {
        MainFrame.Navigate(new BackupView());
    }

    private void OnImportExportClick(object sender, RoutedEventArgs e) {
        MainFrame.Navigate(new ImportExportView());
    }

    private void OnAcercaDeClick(object sender, RoutedEventArgs e) {
        var aboutWindow = new AcercaDeWindow();
        aboutWindow.Owner = this;
        aboutWindow.ShowDialog();
    }

    private void OnConfiguracionClick(object sender, RoutedEventArgs e) {
        var tipoBorrado = AppConfig.UseLogicalDelete ? "Lógico" : "Físico";
        MessageBox.Show(
            "Configuración de la aplicación\n\n" +
            $"Repositorio: {AppConfig.RepositoryType.ToUpper()}\n" +
            $"Storage: {AppConfig.StorageType.ToUpper()}\n" +
            $"Directorio: {AppConfig.DataFolder}\n" +
            $"Tipo de borrado: {tipoBorrado}",
            "Configuración",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    // ==================== NAVIGATION ====================

    private void OnDashboardClick(object sender, RoutedEventArgs e) {
        MainFrame.Navigate(new DashboardView());
    }

    private void OnPreviewKeyDown(object sender, KeyEventArgs e) {
        if (e.Key == Key.F4 && (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt) {
            OnSalirClick(sender, e);
            e.Handled = true;
            return;
        }

        if (Keyboard.Modifiers == ModifierKeys.Control)
            switch (e.Key) {
                case Key.E:
                    OnExportarClick(sender, e);
                    e.Handled = true;
                    break;
                case Key.I:
                    OnImportarClick(sender, e);
                    e.Handled = true;
                    break;
                case Key.B:
                    OnCrearBackupClick(sender, e);
                    e.Handled = true;
                    break;
                case Key.R:
                    OnRestaurarBackupClick(sender, e);
                    e.Handled = true;
                    break;
                case Key.G:
                    OnInformesClick(sender, e);
                    e.Handled = true;
                    break;
                case Key.OemComma:
                    OnConfiguracionClick(sender, e);
                    e.Handled = true;
                    break;
            }
        else if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
            switch (e.Key) {
                case Key.E:
                    OnEstudiantesClick(sender, e);
                    e.Handled = true;
                    break;
                case Key.D:
                    OnDocentesClick(sender, e);
                    e.Handled = true;
                    break;
                case Key.G:
                    OnGraficosClick(sender, e);
                    e.Handled = true;
                    break;
                case Key.A:
                    OnAcercaDeClick(sender, e);
                    e.Handled = true;
                    break;
            }
    }
}