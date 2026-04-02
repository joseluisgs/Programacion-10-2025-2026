// =============================================================================
// VIEWMODEL DEL PROBLEMA DE BACKGROUND
// =============================================================================
// Este ViewModel demuestra cómo manejar tareas en segundo plano en WPF.
// El objetivo es mostrar la diferencia entre:
// 1. Hacer trabajo pesado en el hilo principal (bloquea la UI)
// 2. Hacer trabajo pesado en un hilo secundario (no bloquea la UI)
// =============================================================================

using System;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;

namespace TareasBackground.ViewModels;

/// <summary>
/// ViewModel que demuestra el manejo de tareas en segundo plano.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly ILogger _logger = Log.ForContext<MainViewModel>();
    
    // =============================================================================
    // PROPIEDADES OBSERVABLES
    // =============================================================================
    // Estas propiedades se usan para el binding con la UI.
    // Cuando cambian, la UI se actualiza automáticamente.
    // =============================================================================

    /// <summary>
    /// Progreso de la barra (0 a 100).
    /// Binding con: ProgressBar Value
    /// </summary>
    [ObservableProperty]
    private double _progreso = 0.0;

    /// <summary>
    /// Indica si el botón está habilitado.
    /// Binding con: Button IsEnabled
    /// </summary>
    [ObservableProperty]
    private bool _botonHabilitado = true;

    /// <summary>
    /// Mensaje de estado actual.
    /// Binding con: TextBlock Text
    /// </summary>
    [ObservableProperty]
    private string _mensaje = "Pulsa el botón para comenzar";

    /// <summary>
    /// Indica si la tarea está ejecutándose.
    /// </summary>
    [ObservableProperty]
    private bool _estaEjecutando = false;

    // =============================================================================
    // CONSTRUCTOR
    // =============================================================================

    public MainViewModel()
    {
        _logger.Debug("MainViewModel inicializado");
    }

    // =============================================================================
    // MÉTODOS PÚBLICOS - MANEJO DE TAREAS EN SEGUNDO PLANO
    // =============================================================================

    /// <summary>
    /// ============================================================
    /// MÉTODO INCORRECTO - NO USAR EN PRODUCCIÓN
    /// ============================================================
    /// 
    /// Este método ejecuta la tarea en el HILO PRINCIPAL (UI).
    /// Esto BLOQUEA la interfaz de usuario completamente.
    /// 
    /// El usuario NO PUEDE:
    /// - Hacer clic en botones
    /// - Mover la ventana
    /// - Escribir en campos de texto
    /// - Cerrar la aplicación (parece que no responde)
    /// 
    /// Esto es lo que sucede cuando haces operaciones pesadas en el hilo de la UI:
    /// - Leer archivos grandes
    /// - Procesar muchos datos
    /// - Descargar archivos grandes
    /// - Consultas a base de datos lentas
    /// </summary>
    [RelayCommand]
    public void EjecutarTareaBloqueante()
    {
        _logger.Information("Iniciando tarea BLOQUEANTE en el hilo principal");
        
        BotonHabilitado = false;
        Progreso = 0.0;
        EstaEjecutando = true;
        Mensaje = "Ejecutando tarea bloqueante...";
        
        // ==========================================================================
        // ESTO BLOQUEA LA UI - NO HACER ESTO EN PRODUCCIÓN
        // ==========================================================================
        for (int i = 0; i <= 100; i++)
        {
            Progreso = i;
            Mensaje = $"Progreso: {i}%";
            
            // Thread.Sleep bloquea el HILO ACTUAL
            // Como estamos en el hilo de la UI, se bloquea toda la aplicación
            System.Threading.Thread.Sleep(50);
        }
        
        Mensaje = "¡Tarea completada! (pero bloquéaste la UI)";
        BotonHabilitado = true;
        EstaEjecutando = false;
        
        _logger.Information("Tarea BLOQUEANTE finalizada");
    }

    /// <summary>
    /// ============================================================
    /// MÉTODO CORRECTO - USAR EN PRODUCCIÓN
    /// ============================================================
    /// 
    /// Este método ejecuta la tarea en un HILO SECUNDARIO.
    /// NO bloquea la interfaz de usuario.
    /// 
    /// El usuario PUEDE:
    /// - Interactuar con la UI normalmente
    /// - Mover la ventana
    /// - Usar otros botones
    /// - Cerrar la aplicación
    /// 
    /// CÓMO FUNCIONA:
    /// 1. Task.Run() - Ejecuta el código en un hilo diferente
    /// 2. Dispatcher.Invoke() - Actualiza la UI desde el hilo secundario
    ///    (Debes usar Dispatcher porque solo el hilo de la UI puede modificar la UI)
    /// </summary>
    [RelayCommand]
    public async Task EjecutarTareaNoBloqueanteAsync()
    {
        _logger.Information("Iniciando tarea NO BLOQUEANTE en hilo secundario");
        
        BotonHabilitado = false;
        Progreso = 0.0;
        EstaEjecutando = true;
        Mensaje = "Ejecutando tarea no bloqueante...";
        
        // ==========================================================================
        // Task.Run() crea un nuevo hilo para no bloquear la UI
        // ==========================================================================
        await Task.Run(() =>
        {
            // Este código se ejecuta en un HILO SECUNDARIO
            // Por lo tanto, NO bloquea la UI
            for (int i = 0; i <= 100; i++)
            {
                // ==========================================================================
                // IMPORTANTE: Solo el hilo de la UI puede modificar controles de la UI
                // Por eso usamos Dispatcher.Invoke() para actualizar la ProgressBar
                // ==========================================================================
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Progreso = i;
                    Mensaje = $"Progreso: {i}%";
                });
                
                // Thread.Sleep funciona en hilos secundarios
                // No bloquea la UI, solo ese hilo
                System.Threading.Thread.Sleep(50);
            }
        });
        
        // Este código se ejecuta cuando termina el Task
        Mensaje = "¡Tarea completada! (sin bloquear la UI)";
        BotonHabilitado = true;
        EstaEjecutando = false;
        
        _logger.Information("Tarea NO BLOQUEANTE finalizada");
    }

    /// <summary>
    /// ============================================================
    /// MÉTODO RECOMENDADO - MEJOR PRÁCTICA
    /// ============================================================
    /// 
    /// Versión moderna usando async/await de forma limpia.
    /// Esta es la forma más recomendada porque:
    /// - Código más legible
    /// - Manejo automático de excepciones con try/catch
    /// - No necesita Dispatcher explícito (a veces)
    /// - Mejor integración con async/await de C#
    /// 
    /// Diferencias con el método anterior:
    /// - Usa await Task.Delay() en lugar de Thread.Sleep()
    /// - Task.Delay es NO-BLOQUEANTE
    /// </summary>
    [RelayCommand]
    public async Task EjecutarTareaAsync()
    {
        _logger.Information("Iniciando tarea ASYNC/AWAIT");
        
        BotonHabilitado = false;
        Progreso = 0.0;
        EstaEjecutando = true;
        Mensaje = "Ejecutando tarea async...";
        
        try
        {
            // ==========================================================================
            // await Task.Run() - Ejecuta en hilo secundario
            // await Task.Delay() - Espera sin bloquear (mejor que Thread.Sleep)
            // ==========================================================================
            await Task.Run(async () =>
            {
                for (int i = 0; i <= 100; i++)
                {
                    // Dispatcher.Invoke para actualizar la UI
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Progreso = i;
                        Mensaje = $"Progreso: {i}%";
                    });
                    
                    // ==========================================================================
                    // Task.Delay vs Thread.Sleep:
                    /// - Thread.Sleep: BLOQUEA el hilo actual
                    /// - Task.Delay: NO bloquea, solo pausa
                    // ==========================================================================
                    await Task.Delay(50);
                }
            });
            
            Mensaje = "¡Tarea async completada!";
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error en la tarea");
            Mensaje = $"Error: {ex.Message}";
        }
        finally
        {
            // ==========================================================================
            // El bloque finally siempre se ejecuta, con o sin error
            // Es ideal para limpiar recursos
            // ==========================================================================
            BotonHabilitado = true;
            EstaEjecutando = false;
        }
        
        _logger.Information("Tarea ASYNC/AWAIT finalizada");
    }

    /// <summary>
    /// Reinicia el estado inicial
    /// </summary>
    [RelayCommand]
    public void Reiniciar()
    {
        Progreso = 0.0;
        Mensaje = "Pulsa el botón para comenzar";
        EstaEjecutando = false;
        BotonHabilitado = true;
    }
}
