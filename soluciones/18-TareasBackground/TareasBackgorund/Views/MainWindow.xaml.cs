using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TareasBackground.ViewModels;

namespace TareasBackground.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
