// =============================================================================
// VIEWMODEL DEL JUEGO DE LA MOSCA
// =============================================================================
// El ViewModel es el corazón de la arquitectura MVVM.
// Maneja toda la lógica del juego y se comunica con la Vista (XAML).
//
// PATRONES USADOS:
// - MVVM: Model-View-ViewModel
// - CommunityToolkit.Mvvm: Librería que facilita MVVM
// - ROP: Railway Oriented Programming para manejo de errores
// - Observables: Propuestas que notifican a la UI cuando cambian
// =============================================================================

using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpFunctionalExtensions;
using JuegoMosca.Errors;
using JuegoMosca.Models;
using Serilog;
using static JuegoMosca.Models.MoscaConstants;

namespace JuegoMosca.ViewModels;

/// <summary>
/// ViewModel principal del juego de la mosca.
/// Gestiona el estado del juego y la lógica de negocio.
/// Hereda de ObservableObject para que las propiedades sean reactivas.
/// </summary>
public partial class MoscaViewModel : ObservableObject
{
    // Logger para registrar eventos y depuración
    private readonly ILogger _logger = Log.ForContext<MoscaViewModel>();
    
    // Matriz del juego: representa el tablero donde está escondida la mosca
    // Es un array bidimensional [fila, columna]
    private int[,] _matrix = new int[3, 3];
    
    // Posición actual de la mosca (coordenadas)
    private MoscaPosition _moscaPosition = new(0, 0);
    
    // Evento para mostrar mensajes en la UI
    public event Action<string, string>? MostrarMensaje;

    // =============================================================================
    // PROPIEDADES OBSERVABLES (Bindings con la Vista)
    // =============================================================================
    // Las propiedades con [ObservableProperty] se automáticamente:
    // - Notifican a la UI cuando cambian
    // - Generan un campo privado con el mismo nombre precedido de _
    // =============================================================================
    
    /// <summary>
    /// Dimensión de la matriz (por ejemplo, 3 = tablero 3x3)
    /// </summary>
    [ObservableProperty]
    private int _dimension = 3;

    /// <summary>
    /// Número máximo de intentos permitidos
    /// </summary>
    [ObservableProperty]
    private int _intentos = 5;

    /// <summary>
    /// Contador de golpes/intentos usados
    /// </summary>
    [ObservableProperty]
    private int _golpes = 0;

    /// <summary>
    /// Fila seleccionada por el usuario (1-indexed para el usuario)
    /// </summary>
    [ObservableProperty]
    private int _filaSeleccionada = 1;

    /// <summary>
    /// Columna seleccionada por el usuario (1-indexed para el usuario)
    /// </summary>
    [ObservableProperty]
    private int _columnaSeleccionada = 1;

    /// <summary>
    /// Texto que representa la matriz del juego para mostrar en la UI.
    /// Se actualiza automáticamente cuando cambia.
    /// </summary>
    [ObservableProperty]
    private string _cuadrante = "";

    /// <summary>
    /// Indica si el juego ha terminado (acertar o agotar intentos)
    /// </summary>
    [ObservableProperty]
    private bool _isTerminado = false;

    /// <summary>
    /// Indica si debe mostrarse el botón de "Otra partida"
    /// </summary>
    [ObservableProperty]
    private bool _mostrarBotonComenzar = false;

    /// <summary>
    /// Mensaje de información o error para el usuario
    /// </summary>
    [ObservableProperty]
    private string _mensaje = "";

    // =============================================================================
    // CONSTRUCTOR
    // =============================================================================
    
    /// <summary>
    /// Constructor del ViewModel.
    /// Se ejecuta cuando se crea una instancia.
    /// </summary>
    public MoscaViewModel()
    {
        _logger.Debug("MoscaViewModel inicializado");
    }

    // =============================================================================
    // MÉTODOS PÚBLICOS (Comandos)
    // =============================================================================
    // Los comandos se usan para conectar botones de la UI con métodos del ViewModel.
    // [RelayCommand] genera automáticamente el comando.
    // =============================================================================

    /// <summary>
    /// Inicia un nuevo juego con la dimensión e intentos configurados.
    /// Este método se conecta al botón "Comenzar" o "Otra partida".
    /// </summary>
    [RelayCommand]
    public void IniciarJuego()
    {
        _logger.Debug("Iniciando juego con dimensión {Dimension} e intentos {Intentos}", Dimension, Intentos);
        
        // Validamos que la dimensión esté entre 3 y 10
        Dimension = Math.Max(3, Math.Min(Dimension, 10));
        // Validamos que los intentos estén entre 1 y 20
        Intentos = Math.Max(1, Math.Min(Intentos, 20));
        
        // Reiniciamos el estado del juego
        Golpes = 0;
        FilaSeleccionada = 1;
        ColumnaSeleccionada = 1;
        IsTerminado = false;
        MostrarBotonComenzar = false;
        
        // Creamos una nueva matriz según la dimensión
        _matrix = new int[Dimension, Dimension];
        InitMatrix();
        SituarMosca();
        
        // Actualizamos la visualización del tablero
        Cuadrante = MatrixToAreaJuego();
        Mensaje = "";
        
        _logger.Information("Juego iniciado con éxito");
    }

    /// <summary>
    /// Golpea en la posición seleccionada por el usuario.
    /// Este método implementa la lógica principal del juego.
    /// 
    /// RETORNO: Result<Acertado, DomainError>
    /// - Si tiene éxito (acierta): Result.Success con los datos del acierto
    /// - Si falla: Result.Failure con el tipo de error
    /// 
    /// Esto es ROP: Railway Oriented Programming
    /// </summary>
    public Result<Acertado, DomainError> Golpear()
    {
        // Convertimos de 1-indexed (usuario) a 0-indexed (matriz)
        var fila = FilaSeleccionada - 1;
        var columna = ColumnaSeleccionada - 1;
        
        _logger.Debug("Golpeando fila {Fila}, columna {Columna}", fila + 1, columna + 1);
        
        // Incrementamos el contador de golpes
        Golpes++;
        
        // ---------------------------------------------------------------------
        // CASO 1: Se acabaron los intentos
        // ---------------------------------------------------------------------
        if (Golpes >= Intentos)
        {
            IsTerminado = true;
            MostrarBotonComenzar = true;
            Cuadrante = MatrixToAreaFin();
            
            var error = MoscaErrors.FinIntentos(_moscaPosition, Golpes);
            MostrarMensaje?.Invoke("Fin de intentos", error.Message);
            
            return Result.Failure<Acertado, DomainError>(error);
        }
        
        // ---------------------------------------------------------------------
        // CASO 2: El jugador acierta la posición de la mosca
        // ---------------------------------------------------------------------
        if (_matrix[fila, columna] == MOSCA)
        {
            IsTerminado = true;
            MostrarBotonComenzar = true;
            Cuadrante = MatrixToAreaFin();
            
            var acierto = new Acertado(new MoscaPosition(fila, columna), Golpes);
            MostrarMensaje?.Invoke("¡Has acertado!", $"La mosca estaba en fila {fila + 1}, columna {columna + 1}.\nHas necesitado {acierto.Intentos} intentos.");
            
            return Result.Success<Acertado, DomainError>(acierto);
        }
        
        // ---------------------------------------------------------------------
        // CASO 3: El jugador se acerca mucho (a distancia 1 en cualquier dirección)
        // ---------------------------------------------------------------------
        var diffFila = Math.Abs(_moscaPosition.Fila - fila);
        var diffColumna = Math.Abs(_moscaPosition.Columna - columna);
        
        if (diffFila <= 1 && diffColumna <= 1 && !(diffFila == 0 && diffColumna == 0))
        {
            _logger.Debug("¡Casi! La mosca estaba cerca");
            
            // La mosca se mueve a una nueva posición
            InitMatrix();
            SituarMosca();
            Cuadrante = MatrixToAreaJuego();
            
            var error = MoscaErrors.Casi(fila, columna, Golpes);
            MostrarMensaje?.Invoke("¡Casi lo has logrado!", error.Message);
            
            return Result.Failure<Acertado, DomainError>(error);
        }
        
        // ---------------------------------------------------------------------
        // CASO 4: No acertó ni se acercó
        // ---------------------------------------------------------------------
        Cuadrante = MatrixToAreaJuego();
        var noAcertado = MoscaErrors.NoAcertado();
        MostrarMensaje?.Invoke("No acertado", noAcertado.Message);
        
        return Result.Failure<Acertado, DomainError>(noAcertado);
    }

    // =============================================================================
    // MÉTODOS PRIVADOS (Lógica interna)
    // =============================================================================

    /// <summary>
    /// Inicializa la matriz con ceros (celdas vacías).
    /// Se llama al inicio de cada juego.
    /// </summary>
    private void InitMatrix()
    {
        for (int i = 0; i < Dimension; i++)
        {
            for (int j = 0; j < Dimension; j++)
            {
                _matrix[i, j] = 0;
            }
        }
    }

    /// <summary>
    /// Sitúa la mosca en una posición aleatoria del tablero.
    /// La posición se guarda en _moscaPosition.
    /// </summary>
    private void SituarMosca()
    {
        int fila, columna;
        
        // Repetimos hasta encontrar una posición distinta a la anterior
        // (así la mosca se mueve cuando el jugador está "casi")
        do
        {
            fila = Random.Shared.Next(Dimension);
            columna = Random.Shared.Next(Dimension);
        } while (fila == _moscaPosition.Fila && columna == _moscaPosition.Columna);
        
        // Marcamos la posición de la mosca con la constante MOSCA (-1)
        _matrix[fila, columna] = MOSCA;
        _moscaPosition = new MoscaPosition(fila, columna);
        
        _logger.Debug("La mosca está en fila {Fila}, columna {Columna}", fila + 1, columna + 1);
    }

    /// <summary>
    /// Convierte la matriz a texto para mostrar durante el juego.
    /// Muestra [   ] para cada celda vacía.
    /// </summary>
    private string MatrixToAreaJuego()
    {
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < Dimension; i++)
        {
            for (int j = 0; j < Dimension; j++)
            {
                sb.Append("[   ]");
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    /// <summary>
    /// Convierte la matriz a texto al final del juego.
    /// Muestra 🪰 (emoji de mosca) donde estaba la mosca.
    /// </summary>
    private string MatrixToAreaFin()
    {
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < Dimension; i++)
        {
            for (int j = 0; j < Dimension; j++)
            {
                if (_matrix[i, j] == MOSCA)
                {
                    sb.Append("[ 🪰 ]"); // Emoji de mosca
                }
                else
                {
                    sb.Append("[    ]");
                }
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }
}
