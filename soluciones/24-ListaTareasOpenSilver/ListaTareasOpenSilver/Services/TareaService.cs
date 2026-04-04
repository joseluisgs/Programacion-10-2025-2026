using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using ListaTareasOpenSilver.Models;

namespace ListaTareasOpenSilver.Services;

public interface ITareaService
{
    List<Tarea> GetAll();
    void Add(string titulo);
    void Toggle(Guid id);
    void Remove(Guid id);
    int GetPendientes();
}

public class TareaService : ITareaService
{
    private List<Tarea> _tareas = new();

    public TareaService()
    {
        CargarTareas();
    }

    private void CargarTareas()
    {
        try
        {
            var json = OpenSilver.Interop.ExecuteJavaScript<string>("localStorage.getItem('tareas');");
            if (!string.IsNullOrEmpty(json))
            {
                _tareas = JsonSerializer.Deserialize<List<Tarea>>(json) ?? new List<Tarea>();
            }
        }
        catch
        {
            _tareas = new List<Tarea>();
        }
    }

    private void GuardarTareas()
    {
        var json = JsonSerializer.Serialize(_tareas, new JsonSerializerOptions { WriteIndented = true });
        OpenSilver.Interop.ExecuteJavaScriptVoid($"localStorage.setItem('tareas', '{json.Replace("'", "\\'")}');");
    }

    public List<Tarea> GetAll() => _tareas.OrderBy(t => t.FechaCreacion).ToList();

    public void Add(string titulo)
    {
        if (string.IsNullOrWhiteSpace(titulo)) return;
        _tareas.Add(new Tarea { Titulo = titulo });
        GuardarTareas();
    }

    public void Toggle(Guid id)
    {
        var tarea = _tareas.FirstOrDefault(t => t.Id == id);
        if (tarea != null)
        {
            tarea.Completada = !tarea.Completada;
            GuardarTareas();
        }
    }

    public void Remove(Guid id)
    {
        var tarea = _tareas.FirstOrDefault(t => t.Id == id);
        if (tarea != null)
        {
            _tareas.Remove(tarea);
            GuardarTareas();
        }
    }

    public int GetPendientes() => _tareas.Count(t => !t.Completada);
}