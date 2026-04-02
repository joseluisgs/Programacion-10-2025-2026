// =============================================================================
// VIEWMODEL DE CONFIGURACIÓN DEL JUEGO
// =============================================================================
// Este ViewModel gestiona la pantalla inicial donde el usuario
// configura la dimensión del tablero y el número de intentos.
// =============================================================================

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JuegoMosca.ViewModels;
using Serilog;

namespace JuegoMosca.ViewModels;

/// <summary>
/// ViewModel para la pantalla de configuración del juego.
/// Permite al usuario elegir la dificultad antes de empezar.
/// </summary>
public partial class ConfigViewModel : ObservableObject
{
    private readonly ILogger _logger = Log.ForContext<ConfigViewModel>();
    private readonly MoscaViewModel _moscaViewModel;
    
    /// <summary>Dimensión del tablero (entre 3 y 10)</summary>
    [ObservableProperty]
    private int _dimension = 3;
    
    /// <summary>Número de intentos (entre 1 y 20)</summary>
    [ObservableProperty]
    private int _intentos = 5;
    
    /// <summary>
    /// Evento que se dispara cuando el usuario hace clic en "Comenzar".
    /// La ventana escuchará este evento para abrir la siguiente ventana.
    /// </summary>
    public event System.Action? JuegoIniciado;

    /// <summary>
    /// Constructor que recibe el MoscaViewModel.
    /// Usamos inyección de dependencias para compartir el estado del juego.
    /// </summary>
    public ConfigViewModel(MoscaViewModel moscaViewModel)
    {
        _moscaViewModel = moscaViewModel;
        _logger.Debug("ConfigViewModel inicializado");
    }

    /// <summary>
    /// Método que se ejecuta cuando el usuario hace clic en "Comenzar".
    /// Configura el juego y notifica que debe mostrarse la siguiente ventana.
    /// </summary>
    [RelayCommand]
    public void Comenzar()
    {
        _logger.Information("Iniciando juego con dimensión {Dimension} e intentos {Intentos}", Dimension, Intentos);
        
        // Pasamos la configuración al ViewModel del juego
        _moscaViewModel.Dimension = Dimension;
        _moscaViewModel.Intentos = Intentos;
        
        // Iniciamos el juego
        _moscaViewModel.IniciarJuegoCommand.Execute(null);
        
        // Notificamos a la ventana que debe cambiar a la siguiente
        JuegoIniciado?.Invoke();
    }
}
