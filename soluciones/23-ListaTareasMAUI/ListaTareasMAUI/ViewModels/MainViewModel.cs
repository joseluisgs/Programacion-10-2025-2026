using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ListaTareasMAUI.Models;
using ListaTareasMAUI.Services;
using System.Collections.ObjectModel;

namespace ListaTareasMAUI.ViewModels;

/// <summary>
/// ViewModel de la página principal de tareas.
/// </summary>
/// <remarks>
/// Utiliza CommunityToolkit.Mvvm para simplificar el patrón MVVM:
/// - [ObservableProperty] genera automáticamente las propiedades
/// - [RelayCommand] genera automáticamente los comandos
/// 
/// Este ViewModel gestionatoda la lógica de la página:
/// - Lista de tareas
/// - Nueva tarea a añadir
/// - Comandos para añadir, togglear y eliminar
/// - Contador de tareas pendientes
/// </remarks>
public partial class MainViewModel : ObservableObject
{
    /// <summary>
    /// Servicio de tareas inyectado mediante DI.
    /// </summary>
    private readonly ITareaService _tareaService;

    /// <summary>
    /// Constructor que recibe el servicio mediante inyección de dependencias.
    /// </summary>
    /// <param name="tareaService">Servicio de tareas</param>
    public MainViewModel(ITareaService tareaService)
    {
        _tareaService = tareaService;
        
        // Cargar las tareas iniciales
        CargarTareas();
    }

    /// <summary>
    /// Colección observable de tareas.
    /// </summary>
    /// <remarks>
    /// ObservableCollection notifica a la UI cuando se añade/elimina elementos.
    /// </remarks>
    [ObservableProperty]
    private ObservableCollection<Tarea> _tareas = new();

    /// <summary>
    /// Texto de la nueva tarea a añadir.
    /// </summary>
    /// <remarks>
    /// Bound con el Entry en la UI mediante @bind.
    /// </remarks>
    [ObservableProperty]
    private string _nuevaTarea = string.Empty;

    /// <summary>
    /// Número de tareas pendientes.
    /// </summary>
    /// <remarks>
    /// Se calcula automáticamente desde el servicio.
    /// </remarks>
    [ObservableProperty]
    private int _pendientes = 0;

    /// <summary>
    /// Carga las tareas del servicio.
    /// </summary>
    private void CargarTareas()
    {
        var lista = _tareaService.GetAll();
        Tareas = new ObservableCollection<Tarea>(lista);
        ActualizarContador();
    }

    /// <summary>
    /// Actualiza el contador de tareas pendientes.
    /// </summary>
    private void ActualizarContador()
    {
        Pendientes = _tareaService.GetPendientes();
    }

    /// <summary>
    /// Comando para añadir una nueva tarea.
    /// </summary>
    /// <remarks>
    /// [RelayCommand] genera automáticamente el comando AgregarTareaCommand
    /// que se puede bindear en la UI.
    /// </remarks>
    [RelayCommand]
    private void AgregarTarea()
    {
        // Validar que no esté vacío
        if (string.IsNullOrWhiteSpace(NuevaTarea))
            return;

        // Añadir la tarea
        _tareaService.Add(NuevaTarea);
        
        // Recargar la lista
        CargarTareas();
        
        // Limpiar el campo
        NuevaTarea = string.Empty;
    }

    /// <summary>
    /// Comando para togglear el estado de una tarea.
    /// </summary>
    /// <param name="tarea">Tarea a togglear</param>
    [RelayCommand]
    private void ToggleTarea(Tarea tarea)
    {
        if (tarea == null) return;
        
        _tareaService.Toggle(tarea.Id);
        CargarTareas();
    }

    /// <summary>
    /// Comando para eliminar una tarea.
    /// </summary>
    /// <param name="tarea">Tarea a eliminar</param>
    [RelayCommand]
    private void EliminarTarea(Tarea tarea)
    {
        if (tarea == null) return;
        
        _tareaService.Remove(tarea.Id);
        CargarTareas();
    }
}