using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using GestionAcademica.ViewModels;
using Serilog;

namespace GestionAcademica.Views.Main;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        var viewModel = App.ServiceProvider.GetRequiredService<MainViewModel>();
        DataContext = viewModel;
        
        viewModel.OnNavigateRequested += OnNavigateRequested;
        
        Log.Information("🏠 MainWindow inicializada");
        
        MainFrame.Navigate(new Dashboard.DashboardView());
    }

    private void OnNavigateRequested(Page page)
    {
        MainFrame.Navigate(page);
    }

    private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
    {
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
            Log.Information("👋 Usuario cerró la aplicación mediante la ventana");
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        Log.Information("🏁 MainWindow cerrada");
        base.OnClosed(e);
    }

    // ==================== MENU ====================
    
    private void OnSalirClick(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            "¿Esta seguro de que desea salir de la aplicacion?",
            "Confirmar salida",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            Log.Information("👋 Usuario cerró la aplicacion");
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
        var aboutWindow = new Dialog.AcercaDeWindow();
        aboutWindow.Owner = this;
        aboutWindow.ShowDialog();
    }

    private void OnConfiguracionClick(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
            "Configuracion de la aplicacion\n\n" +
            "Repositorio: Memory\n" +
            "Storage: JSON\n" +
            "Directorio: data/",
            "Configuracion",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    // ==================== NAVIGATION ====================
    
    private void OnDashboardClick(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new Dashboard.DashboardView());
    }

    // ==================== THEME ====================
    
    private void OnTemaOscuroChecked(object sender, RoutedEventArgs e)
    {
        ApplyTheme("Dark");
    }

    private void OnTemaClaroChecked(object sender, RoutedEventArgs e)
    {
        ApplyTheme("Light");
    }

    private void OnTemaOscuroClick(object sender, RoutedEventArgs e)
    {
        MenuTemaOscuro.IsChecked = true;
        MenuTemaClaro.IsChecked = false;
        TemaOscuroRadio.IsChecked = true;
        ApplyTheme("Dark");
    }

    private void OnTemaClaroClick(object sender, RoutedEventArgs e)
    {
        MenuTemaOscuro.IsChecked = false;
        MenuTemaClaro.IsChecked = true;
        TemaClaroRadio.IsChecked = true;
        ApplyTheme("Light");
    }

    private void ApplyTheme(string themeName)
    {
        try
        {
            var app = Application.Current;
            var resources = app.Resources.MergedDictionaries;
            
            for (int i = resources.Count - 1; i >= 0; i--)
            {
                var dict = resources[i];
                if (dict.Source != null && dict.Source.OriginalString.Contains("Theme"))
                {
                    resources.RemoveAt(i);
                }
            }
            
            var themeUri = new Uri($"pack://application:,,,/Themes/{themeName}Theme.xaml", UriKind.Absolute);
            resources.Add(new ResourceDictionary { Source = themeUri });
            
            Log.Information("✅ Tema cambiado a {Theme}", themeName);
            
            StatusText.Text = $"Tema: {themeName}";
        }
        catch (Exception ex)
        {
            Log.Error(ex, "❌ Error al aplicar el tema");
        }
    }
}