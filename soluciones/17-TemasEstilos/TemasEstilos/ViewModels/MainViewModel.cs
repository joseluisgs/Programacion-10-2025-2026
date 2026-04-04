using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TemasEstilos.Models;
using TemasEstilos.Services;

namespace TemasEstilos.ViewModels;

/// <summary>
/// ViewModel principal para la gestión de temas y estilos.
/// </summary>
/// <remarks>
/// Este ViewModel gestionatoda la lógica relacionada con:
/// - Lista de temas disponibles (personalizados)
/// - Tema actualmente seleccionado
/// - Cambio dinámico de temas
/// - Aplicación de estilos dinámicos
/// - Soporte para Material Design
/// 
/// Usa CommunityToolkit.Mvvm para simplificar el patrón MVVM:
/// - [ObservableProperty] genera automáticamente las propiedades
/// - [RelayCommand] genera automáticamente los comandos
/// </remarks>
public partial class MainViewModel : ObservableObject, IDisposable
{
    /// <summary>
    /// Servicio de temas inyectado mediante DI.
    /// </summary>
    private readonly ITemasService _temasService;
    private bool _disposed;

    /// <summary>
    /// Constructor que recibe el servicio mediante inyección de dependencias.
    /// </summary>
    /// <param name="temasService">Servicio de temas</param>
    public MainViewModel(ITemasService temasService)
    {
        _temasService = temasService;
        
        // Suscribirse al evento de cambio de tema del servicio
        _temasService.TemaCambiado += OnTemaCambiado;
        
        // Cargar los temas iniciales
        CargarTemas();
        
        // Inicializar los temas de Material Design
        CargarTemasMaterial();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _temasService.TemaCambiado -= OnTemaCambiado;
        _disposed = true;
    }

    /// <summary>
    /// Colección observable de temas disponibles (personalizados).
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<ColorPaleta> _temas = new();

    /// <summary>
    /// Colección de paletas de Material Design disponibles.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<MaterialPaleta> _temasMaterial = new();

    /// <summary>
    /// Tema personalizado actualmente seleccionado.
    /// </summary>
    [ObservableProperty]
    private ColorPaleta? _temaSeleccionado;

    /// <summary>
    /// Tema de Material Design actualmente seleccionado.
    /// </summary>
    [ObservableProperty]
    private MaterialPaleta? _temaMaterialSeleccionado;

    /// <summary>
    /// Indica si se está usando modo Material Design.
    /// </summary>
    [ObservableProperty]
    private bool _usarMaterialDesign = true;

    /// <summary>
    /// Nombre del tema actual (para mostrar en la UI).
    /// </summary>
    [ObservableProperty]
    private string _nombreTemaActual = "";

    /// <summary>
    /// Indica si el tema actual es oscuro.
    /// </summary>
    [ObservableProperty]
    private bool _esTemaOscuro = false;

    /// <summary>
    /// Texto de ejemplo que cambia según el tema.
    /// </summary>
    [ObservableProperty]
    private string _textoEjemplo = "Este es un ejemplo de texto que cambia de color según el tema aplicado.";

    /// <summary>
    /// Número de ejemplo para demostrar estilos numéricos.
    /// </summary>
    [ObservableProperty]
    private int _numeroEjemplo = 42;

    /// <summary>
    /// Carga los temas personalizados desde el servicio.
    /// </summary>
    private void CargarTemas()
    {
        // Obtener todos los temas del servicio
        var listaTemas = _temasService.GetTemas();
        
        // Convertir a ObservableCollection para binding
        Temas = new ObservableCollection<ColorPaleta>(listaTemas);
        
        // Establecer el tema actual
        var temaActual = _temasService.GetTemaActual();
        TemaSeleccionado = temaActual;
        ActualizarPropiedadesTema(temaActual);
    }

    /// <summary>
    /// Carga las paletas de Material Design disponibles.
    /// </summary>
    /// <remarks>
    /// Material Design ofrece paletas de colores predefinidas.
    /// Cada paleta tiene un color primario y uno secundario.
    /// </remarks>
    private void CargarTemasMaterial()
    {
        TemasMaterial = new ObservableCollection<MaterialPaleta>
        {
            new MaterialPaleta { Nombre = "Blue", Primary = "Blue", Secondary = "Orange" },
            new MaterialPaleta { Nombre = "Indigo", Primary = "Indigo", Secondary = "Pink" },
            new MaterialPaleta { Nombre = "Teal", Primary = "Teal", Secondary = "Lime" },
            new MaterialPaleta { Nombre = "Green", Primary = "Green", Secondary = "Teal" },
            new MaterialPaleta { Nombre = "Purple", Primary = "Purple", Secondary = "DeepOrange" },
            new MaterialPaleta { Nombre = "Red", Primary = "Red", Secondary = "Yellow" },
            new MaterialPaleta { Nombre = "Pink", Primary = "Pink", Secondary = "Teal" },
            new MaterialPaleta { Nombre = "Cyan", Primary = "Cyan", Secondary = "Pink" }
        };

        // Establecer el primer tema por defecto
        TemaMaterialSeleccionado = TemasMaterial[0];
    }

    /// <summary>
    /// Actualiza las propiedades del tema actual.
    /// </summary>
    /// <param name="tema">Tema a aplicar</param>
    private void ActualizarPropiedadesTema(ColorPaleta tema)
    {
        NombreTemaActual = tema.Nombre;
        EsTemaOscuro = tema.EsOscuro;
    }

    /// <summary>
    /// Se ejecuta cuando el servicio notifica un cambio de tema.
    /// </summary>
    /// <param name="nuevoTema">Nuevo tema aplicado</param>
    private void OnTemaCambiado(ColorPaleta nuevoTema)
    {
        TemaSeleccionado = nuevoTema;
        ActualizarPropiedadesTema(nuevoTema);
    }

    /// <summary>
    /// Comando para cambiar el tema personalizado.
    /// </summary>
    /// <param name="tema">Tema a aplicar</param>
    [RelayCommand]
    private void CambiarTema(ColorPaleta? tema)
    {
        if (tema == null) return;
        
        // Desactivar modo Material Design cuando usamos temas personalizados
        UsarMaterialDesign = false;
        
        // Llamar al servicio para cambiar el tema
        _temasService.CambiarTema(tema.Nombre);
    }

    /// <summary>
    /// Comando para aplicar el tema personalizado seleccionado en el ComboBox.
    /// </summary>
    [RelayCommand]
    private void AplicarTemaSeleccionado()
    {
        if (TemaSeleccionado != null)
        {
            UsarMaterialDesign = false;
            _temasService.CambiarTema(TemaSeleccionado.Nombre);
        }
    }

    /// <summary>
    /// Comando para activar un tema de Material Design.
    /// </summary>
    /// <param name="paleta">Paleta de Material Design a aplicar</param>
    [RelayCommand]
    private void CambiarTemaMaterial(MaterialPaleta? paleta)
    {
        if (paleta == null) return;
        
        // Activar modo Material Design
        UsarMaterialDesign = true;
        TemaMaterialSeleccionado = paleta;
        
        // Actualizar propiedades
        NombreTemaActual = $"Material {paleta.Nombre}";
        EsTemaOscuro = false;
        
        // Notificar cambio
        OnPropertyChanged(nameof(UsarMaterialDesign));
        OnPropertyChanged(nameof(TemaMaterialSeleccionado));
    }

    /// <summary>
    /// Comando para alternar entre modo claro y oscuro de Material Design.
    /// </summary>
    [RelayCommand]
    private void AlternarModoMaterial()
    {
        EsTemaOscuro = !EsTemaOscuro;
        OnPropertyChanged(nameof(EsTemaOscuro));
    }
}

/// <summary>
/// Modelo que representa una paleta de colores de Material Design.
/// </summary>
public class MaterialPaleta
{
    /// <summary>
    /// Nombre de la paleta.
    /// </summary>
    public string Nombre { get; set; } = "";

    /// <summary>
    /// Color primario de la paleta.
    /// </summary>
    public string Primary { get; set; } = "Blue";

    /// <summary>
    /// Color secundario de la paleta.
    /// </summary>
    public string Secondary { get; set; } = "Orange";
}