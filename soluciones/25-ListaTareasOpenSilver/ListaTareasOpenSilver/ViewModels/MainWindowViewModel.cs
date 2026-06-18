using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ListaTareasOpenSilver.Models;
using ListaTareasOpenSilver.Services;

namespace ListaTareasOpenSilver.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ITareaService _tareaService;

    public MainWindowViewModel(ITareaService tareaService)
    {
        _tareaService = tareaService;
        CargarTareas();
    }

    [ObservableProperty]
    private ObservableCollection<Tarea> _tareas = new();

    [ObservableProperty]
    private string _nuevaTarea = string.Empty;

    [ObservableProperty]
    private int _pendientes = 0;

    private void CargarTareas()
    {
        var lista = _tareaService.GetAll();
        Tareas = new ObservableCollection<Tarea>(lista);
        ActualizarContador();
    }

    private void ActualizarContador()
    {
        Pendientes = _tareaService.GetPendientes();
    }

    [RelayCommand]
    private void AgregarTarea()
    {
        if (string.IsNullOrWhiteSpace(NuevaTarea)) return;

        _tareaService.Add(NuevaTarea);
        CargarTareas();
        NuevaTarea = string.Empty;
    }

    [RelayCommand]
    private void ToggleTarea(Tarea? tarea)
    {
        if (tarea == null) return;
        _tareaService.Toggle(tarea.Id);
        CargarTareas();
    }

    [RelayCommand]
    private void EliminarTarea(Tarea? tarea)
    {
        if (tarea == null) return;
        _tareaService.Remove(tarea.Id);
        CargarTareas();
    }
}