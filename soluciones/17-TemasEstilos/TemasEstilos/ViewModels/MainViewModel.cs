using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.Diagnostics;
using TemasEstilos.Models;
using TemasEstilos.Services;

namespace TemasEstilos.ViewModels;

/// <summary>
/// ViewModel principal reactivo para la gestión de temas y estilos.
/// </summary>
public partial class MainViewModel : ObservableObject, IDisposable
{
    private readonly ITemasService _temasService;
    private bool _disposed;
    private readonly PaletteHelper _paletteHelper = new();

    public MainViewModel(ITemasService temasService)
    {
        _temasService = temasService;
        _temasService.TemaCambiado += OnTemaCambiado;
        CargarTemas();
        CargarTemasMaterial();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _temasService.TemaCambiado -= OnTemaCambiado;
        _disposed = true;
    }

    [ObservableProperty] private ObservableCollection<ColorPaleta> _temas = new();
    [ObservableProperty] private ObservableCollection<MaterialPaleta> _temasMaterial = new();
    [ObservableProperty] private ColorPaleta? _temaSeleccionado;
    [ObservableProperty] private MaterialPaleta? _temaMaterialSeleccionado;
    [ObservableProperty] private bool _usarMaterialDesign = true;
    [ObservableProperty] private string _nombreTemaActual = "";
    [ObservableProperty] private bool _esTemaOscuro = false;
    [ObservableProperty] private string _textoEjemplo = "Este es un ejemplo de texto que cambia de color según el tema aplicado.";
    [ObservableProperty] private int _numeroEjemplo = 42;

    private void CargarTemas()
    {
        var listaTemas = _temasService.GetTemas();
        Temas = new ObservableCollection<ColorPaleta>(listaTemas);
        var temaActual = _temasService.GetTemaActual();
        TemaSeleccionado = temaActual;
        ActualizarPropiedadesTema(temaActual);
    }

    private void CargarTemasMaterial()
    {
        TemasMaterial = new ObservableCollection<MaterialPaleta>
        {
            new MaterialPaleta { Nombre = "Blue", Primary = "#2196F3", Secondary = "#FF9800" },
            new MaterialPaleta { Nombre = "Indigo", Primary = "#3F51B5", Secondary = "#E91E63" },
            new MaterialPaleta { Nombre = "Teal", Primary = "#009688", Secondary = "#CDDC39" },
            new MaterialPaleta { Nombre = "Green", Primary = "#4CAF50", Secondary = "#009688" },
            new MaterialPaleta { Nombre = "Purple", Primary = "#9C27B0", Secondary = "#FF5722" },
            new MaterialPaleta { Nombre = "Red", Primary = "#F44336", Secondary = "#FFEB3B" },
            new MaterialPaleta { Nombre = "Pink", Primary = "#E91E63", Secondary = "#009688" },
            new MaterialPaleta { Nombre = "Cyan", Primary = "#00BCD4", Secondary = "#E91E63" }
        };
        TemaMaterialSeleccionado = TemasMaterial[0];
    }

    private void ActualizarPropiedadesTema(ColorPaleta tema)
    {
        NombreTemaActual = tema.Nombre;
        // Sincronizar propiedad con el estado del tema del servicio
        _esTemaOscuro = tema.EsOscuro;
        OnPropertyChanged(nameof(EsTemaOscuro));
    }

    private void OnTemaCambiado(ColorPaleta nuevoTema)
    {
        TemaSeleccionado = nuevoTema;
        ActualizarPropiedadesTema(nuevoTema);
    }

    /// <summary>
    /// Reactividad al cambiar el modo Material Design.
    /// </summary>
    partial void OnUsarMaterialDesignChanged(bool value)
    {
        if (value) AplicarTemaMaterialActual();
        else AplicarTemaPersonalizadoActual();
    }

    /// <summary>
    /// Reactividad al cambiar entre Claro/Oscuro.
    /// </summary>
    partial void OnEsTemaOscuroChanged(bool value)
    {
        if (UsarMaterialDesign)
        {
            try {
                var theme = _paletteHelper.GetTheme();
                theme.SetBaseTheme(value ? BaseTheme.Dark : BaseTheme.Light);
                _paletteHelper.SetTheme(theme);
            } catch (Exception ex) { Debug.WriteLine(ex.Message); }
        }
        else
        {
            // Sincronizar con el servicio (Claro o Oscuro)
            _temasService.CambiarTema(value ? "Oscuro" : "Claro");
        }
    }

    private void AplicarTemaMaterialActual()
    {
        if (TemaMaterialSeleccionado != null)
            CambiarTemaMaterial(TemaMaterialSeleccionado);
    }

    private void AplicarTemaPersonalizadoActual()
    {
        if (TemaSeleccionado != null)
            _temasService.CambiarTema(TemaSeleccionado.Nombre);
    }

    [RelayCommand]
    private void CambiarTema(ColorPaleta? tema)
    {
        if (tema == null) return;
        UsarMaterialDesign = false;
        _temasService.CambiarTema(tema.Nombre);
    }

    [RelayCommand]
    private void AplicarTemaSeleccionado()
    {
        if (TemaSeleccionado != null)
        {
            UsarMaterialDesign = false;
            _temasService.CambiarTema(TemaSeleccionado.Nombre);
        }
    }

    [RelayCommand]
    private void CambiarTemaMaterial(MaterialPaleta? paleta)
    {
        if (paleta == null) return;
        try 
        {
            UsarMaterialDesign = true;
            TemaMaterialSeleccionado = paleta;
            NombreTemaActual = $"Material {paleta.Nombre}";
            var theme = _paletteHelper.GetTheme();
            theme.SetPrimaryColor((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(paleta.Primary));
            theme.SetSecondaryColor((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(paleta.Secondary));
            theme.SetBaseTheme(EsTemaOscuro ? BaseTheme.Dark : BaseTheme.Light);
            _paletteHelper.SetTheme(theme);
        }
        catch (Exception ex) { Debug.WriteLine($"Error al cambiar tema Material: {ex.Message}"); }
    }

    [RelayCommand]
    private void ResetearTemas()
    {
        // 1. Resetear propiedades de control
        UsarMaterialDesign = true;
        EsTemaOscuro = false;
        
        // 2. Resetear selecciones de colecciones
        TemaMaterialSeleccionado = TemasMaterial[0];
        TemaSeleccionado = Temas[0];

        // 3. Forzar aplicación física de Material Design (Base Light + Blue/Orange)
        try 
        {
            var theme = _paletteHelper.GetTheme();
            theme.SetBaseTheme(BaseTheme.Light);
            theme.SetPrimaryColor((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(TemasMaterial[0].Primary));
            theme.SetSecondaryColor((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(TemasMaterial[0].Secondary));
            _paletteHelper.SetTheme(theme);
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }

        // 4. Forzar aplicación física de Tema Personalizado (Claro)
        _temasService.CambiarTema("Claro");
        
        // 5. Actualizar información de UI
        NombreTemaActual = "Material Blue";
        
        // Notificar cambios explícitamente para asegurar reactividad total en el reset
        OnPropertyChanged(nameof(UsarMaterialDesign));
        OnPropertyChanged(nameof(EsTemaOscuro));
        OnPropertyChanged(nameof(TemaMaterialSeleccionado));
        OnPropertyChanged(nameof(TemaSeleccionado));
    }
}

public class MaterialPaleta
{
    public string Nombre { get; set; } = "";
    public string Primary { get; set; } = "#2196F3";
    public string Secondary { get; set; } = "#FF9800";
}
