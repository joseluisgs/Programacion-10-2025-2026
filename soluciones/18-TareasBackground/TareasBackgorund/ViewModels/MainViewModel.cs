// =============================================================================
// VIEWMODEL DEL PROBLEMA DE BACKGROUND
// =============================================================================
// Este ViewModel demuestra cómo manejar tareas en segundo plano en WPF.
// El objetivo es mostrar la diferencia entre:
// 1. Hacer trabajo pesado en el hilo principal (bloquea la UI)
// 2. Hacer trabajo pesado en un hilo secundario (no bloquea la UI)
// 3. Uso de async/await (forma moderna y limpia)
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
    
    [ObservableProperty] private double _progreso = 0.0;
    [ObservableProperty] private bool _botonHabilitado = true;
    [ObservableProperty] private string _mensaje = "Pulsa el botón para comenzar";
    [ObservableProperty] private bool _estaEjecutando = false;

    public MainViewModel()
    {
        _logger.Debug("MainViewModel inicializado");
    }

    /// <summary>
    /// MÉTODO INCORRECTO - BLOQUEA LA UI
    /// </summary>
    [RelayCommand]
    private void EjecutarTareaBloqueante()
    {
        _logger.Information("Iniciando tarea BLOQUEANTE");
        PrepararTarea("Ejecutando tarea bloqueante...");
        
        for (var i = 0; i <= 100; i++)
        {
            Progreso = i;
            Mensaje = $"Progreso: {i}%";
            Log.Information($"Progreso: {i}%");
            Thread.Sleep(50); // Bloquea el hilo de la UI
        }
        
        FinalizarTarea("¡Tarea completada! (pero bloquéaste la UI)");
    }

    /// <summary>
    /// MÉTODO CORRECTO - HILO SECUNDARIO
    /// </summary>
    [RelayCommand]
    private async Task EjecutarTareaNoBloqueante()
    {
        _logger.Information("Iniciando tarea NO BLOQUEANTE");
        PrepararTarea("Ejecutando tarea no bloqueante...");
        
        await Task.Run(() =>
        {
            for (int i = 0; i <= 100; i++)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Progreso = i;
                    Mensaje = $"Progreso: {i}%";
                    Log.Information($"Progreso: {i}%");
                });
                Thread.Sleep(50);
            }
        });
        
        FinalizarTarea("¡Tarea completada! (sin bloquear la UI)");
    }

    /// <summary>
    /// MÉTODO ÓPTIMO - ASYNC/AWAIT PURO
    /// </summary>
    [RelayCommand]
    public async Task EjecutarTareaOptima()
    {
        _logger.Information("Iniciando tarea ÓPTIMA");
        PrepararTarea("Ejecutando tarea óptima...");
        
        try
        {
            for (int i = 0; i <= 100; i++)
            {
                Progreso = i;
                Mensaje = $"Progreso: {i}%";
                Log.Information($"Progreso: {i}%");
                
                // await Task.Delay es la clave: 
                // Pausa el método y devuelve el control a la UI
                // permitiendo que se refresque sin bloquearla.
                await Task.Delay(50); 
            }
            FinalizarTarea("¡Tarea óptima completada!");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error en la tarea");
            FinalizarTarea($"Error: {ex.Message}");
        }
    }

    private void PrepararTarea(string mensaje)
    {
        BotonHabilitado = false;
        Progreso = 0.0;
        EstaEjecutando = true;
        Mensaje = mensaje;
    }

    private void FinalizarTarea(string mensaje)
    {
        Mensaje = mensaje;
        BotonHabilitado = true;
        EstaEjecutando = false;
        _logger.Information("Tarea finalizada: {Mensaje}", mensaje);
    }

    [RelayCommand]
    public void Reiniciar()
    {
        Progreso = 0.0;
        Mensaje = "Pulsa el botón para comenzar";
        EstaEjecutando = false;
        BotonHabilitado = true;
    }
}
