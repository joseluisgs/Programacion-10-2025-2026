using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using ListaCompra.FormData.ViewModels;
using ListaCompra.Forms.Views.Dialog;

namespace ListaCompra.Forms.Views.Main;

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