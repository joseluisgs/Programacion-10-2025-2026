using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.DependencyInjection;
using Pokedex.ViewModels;
using Pokedex.Views.Dialog;

namespace Pokedex.Views.Main;

public partial class MainWindow : Window
{
    private readonly BitmapImage _defaultImage;
    
    public MainWindow()
    {
        InitializeComponent();
        
        _defaultImage = new BitmapImage(new Uri("pack://application:,,,/Resources/images/sin-image.png"));
        
        var viewModel = App.ServiceProvider.GetRequiredService<MainViewModel>();
        DataContext = viewModel;
        
        ImgPokemon.ImageFailed += ImgPokemon_ImageFailed;
        
        Loaded += (s, e) => CargarImagen(viewModel);
        viewModel.PropertyChanged += (s, e) => 
        {
            if (e.PropertyName == nameof(MainViewModel.ImagenPokemon))
                CargarImagen(viewModel);
        };
    }

    private void CargarImagen(MainViewModel vm)
    {
        if (string.IsNullOrWhiteSpace(vm.ImagenPokemon))
        {
            ImgPokemon.Source = _defaultImage;
            return;
        }

        try
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(vm.ImagenPokemon);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            ImgPokemon.Source = bitmap;
        }
        catch
        {
            ImgPokemon.Source = _defaultImage;
        }
    }

    private void ImgPokemon_ImageFailed(object? sender, ExceptionRoutedEventArgs e)
    {
        ImgPokemon.Source = _defaultImage;
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
