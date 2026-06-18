using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using ListaCompra.Models;
using ListaCompra.ViewModels;
using ListaCompra.Views.Dialog;

namespace ListaCompra.Views.Main;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        Resources["BoolToTextDecoration"] = new BoolToTextDecorationConverter();
        InitializeComponent();
        var viewModel = App.ServiceProvider.GetRequiredService<MainViewModel>();
        DataContext = viewModel;
        Loaded += (s, e) => viewModel.CargarProductos();
    }

    private void AcercaDe_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new AcercaDeWindow
        {
            Owner = this
        };
        dialog.ShowDialog();
    }

    private void CheckBox_Changed(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox checkBox && checkBox.DataContext is Producto producto)
        {
            var nuevoEstado = checkBox.IsChecked == true;
            var viewModel = (MainViewModel)DataContext;
            viewModel.CambiarEstado(producto, nuevoEstado);
        }
    }
}

public class BoolToTextDecorationConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool estaComprado && estaComprado)
            return TextDecorations.Strikethrough;
        return null!;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}