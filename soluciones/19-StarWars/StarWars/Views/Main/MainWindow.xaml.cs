using System.Windows;
using System.Windows.Controls;
using StarWars.ViewModels;
using StarWars.Views.Dialog;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace StarWars.Views.Main;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;

        _viewModel.MostrarAlerta += OnMostrarAlerta;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnMostrarAlerta(string titulo, string mensaje)
    {
        Dispatcher.Invoke(() => MessageBox.Show(mensaje, titulo, MessageBoxButton.OK, MessageBoxImage.Information));
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.Operacion))
        {
            Dispatcher.Invoke(() => ScrollToEnd(TextOperacion));
        }
        if (e.PropertyName == nameof(MainViewModel.Cuadrante))
        {
            Dispatcher.Invoke(() => ScrollToEnd(TextCuadrante));
        }
    }

    private void ScrollToEnd(TextBox tb)
    {
        tb.ScrollToEnd();
        tb.CaretIndex = tb.Text.Length;
    }

    private void MenuSalir_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("¿Seguro que quieres salir?", "Salir", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
            Application.Current.Shutdown();
    }

    private void MenuAcercaDe_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new AcercaDeWindow { Owner = this };
        dialog.ShowDialog();
    }
}
