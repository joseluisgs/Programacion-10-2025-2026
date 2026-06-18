using System;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.DependencyInjection;
using Pokedex.ViewModels;
using Pokedex.Views.Dialog;
using Serilog;

namespace Pokedex.Views.Main;

public partial class MainWindow : Window
{
    private readonly BitmapImage _defaultImage;
    private static readonly HttpClient HttpClient = new();
    
    public MainWindow()
    {
        Log.Information("🔨 Constructor MainWindow iniciado");
        
        InitializeComponent();
        Log.Information("✅ InitializeComponent completado");
        
        _defaultImage = new BitmapImage(new Uri("pack://application:,,,/Resources/images/sin-image.png"));
        Log.Information("✅ Imagen por defecto cargada");
        
        Log.Information("🔧 Obteniendo MainViewModel del ServiceProvider...");
        var viewModel = App.ServiceProvider.GetRequiredService<MainViewModel>();
        Log.Information("✅ MainViewModel obtenido");
        
        DataContext = viewModel;
        Log.Information("✅ DataContext asignado");
        
        // Inicializar el ViewModel y cargar datos
        Log.Information("📊 Inicializando ViewModel...");
        viewModel.Initialize();
        Log.Information("✅ ViewModel inicializado");
        
        ImgPokemon.ImageFailed += ImgPokemon_ImageFailed;
        
        Loaded += (s, e) => CargarImagen(viewModel);
        viewModel.PropertyChanged += (s, e) => 
        {
            if (e.PropertyName == nameof(MainViewModel.ImagenPokemon))
                CargarImagen(viewModel);
        };
        
        Log.Information("✅ Constructor MainWindow completado");
}

    private async void CargarImagen(MainViewModel vm)
    {
        if (string.IsNullOrWhiteSpace(vm.ImagenPokemon))
        {
            ImgPokemon.Source = _defaultImage;
            return;
        }

        try
        {
            var uri = new Uri(vm.ImagenPokemon);
            if (uri.Scheme == "pack")
            {
                var bitmap = new BitmapImage(uri);
                ImgPokemon.Source = bitmap;
                return;
            }

            var bytes = await HttpClient.GetByteArrayAsync(uri);
            using var ms = new MemoryStream(bytes);
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = ms;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();
            ImgPokemon.Source = bitmapImage;
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "⚠️ Error al cargar imagen: {Url}", vm.ImagenPokemon);
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

    private void Estadisticas_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new EstadisticasWindow(App.ServiceProvider.GetRequiredService<MainViewModel>())
        {
            Owner = this
        };
        dialog.ShowDialog();
    }
}
