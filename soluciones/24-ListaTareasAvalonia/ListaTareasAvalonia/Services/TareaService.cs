using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ListaTareasAvalonia.Models;

namespace ListaTareasAvalonia.Services;

/// <summary>
/// Interfaz para el servicio de gestión de tareas.
/// </summary>
public interface ITareaService
{
    List<Tarea> GetAll();
    void Add(string titulo);
    void Toggle(Guid id);
    void Remove(Guid id);
    int GetPendientes();
}

/// <summary>
/// Implementación del servicio de tareas con persistencia en archivo JSON.
/// </summary>
public class TareaService : ITareaService
{
    private readonly string _filePath;
    private List<Tarea> _tareas = new();

    public TareaService()
    {
        var appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ListaTareasAvalonia");
        
        Directory.CreateDirectory(appDataPath);
        _filePath = Path.Combine(appDataPath, "tareas.json");
        CargarTareas();
    }

    private void CargarTareas()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
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
        File.WriteAllText(_filePath, json);
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