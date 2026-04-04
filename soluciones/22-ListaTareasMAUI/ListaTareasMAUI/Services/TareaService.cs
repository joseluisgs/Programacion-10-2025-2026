using System.IO;
using System.Text.Json;
using ListaTareasMAUI.Models;

namespace ListaTareasMAUI.Services;

/// <summary>
/// Interfaz para el servicio de gestión de tareas.
/// </summary>
/// <remarks>
/// Define el contrato que debe implementar cualquier servicio de tareas.
/// Esto permite cambiar la implementación (en memoria, archivo, base de datos)
/// sin modificar el código que usa el servicio (principio de inversión de dependencias).
/// </remarks>
public interface ITareaService
{
    /// <summary>
    /// Obtiene todas las tareas.
    /// </summary>
    /// <returns>Lista de todas las tareas</returns>
    List<Tarea> GetAll();

    /// <summary>
    /// Añade una nueva tarea.
    /// </summary>
    /// <param name="titulo">Título de la tarea</param>
    void Add(string titulo);

    /// <summary>
    /// Alterna el estado de completada de una tarea.
    /// </summary>
    /// <param name="id">ID de la tarea</param>
    void Toggle(Guid id);

    /// <summary>
    /// Elimina una tarea por su ID.
    /// </summary>
    /// <param name="id">ID de la tarea a eliminar</param>
    void Remove(Guid id);

    /// <summary>
    /// Obtiene el número de tareas pendientes (no completadas).
    /// </summary>
    /// <returns>Count de tareas sin completar</returns>
    int GetPendientes();
}

/// <summary>
/// Implementación del servicio de tareas con persistencia en archivo JSON.
/// </summary>
/// <remarks>
/// Esta implementación guarda las tareas en un archivo JSON en el directorio
/// de datos de la aplicación. Los datos persisten entre ejecuciones.
/// </remarks>
public class TareaService : ITareaService
{
    /// <summary>
    /// Ruta del archivo donde se guardan las tareas.
    /// </summary>
    private readonly string _filePath;

    /// <summary>
    /// Lista en memoria de tareas (cache).
    /// </summary>
    private List<Tarea> _tareas = new();

    /// <summary>
    /// Constructor que inicializa el servicio y carga los datos.
    /// </summary>
    /// <remarks>
    /// Obtiene la ruta del archivo de datos y carga las tareas existentes.
    /// </remarks>
    public TareaService()
    {
        // Obtener la ruta del archivo en el directorio de datos de la aplicación
        var appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ListaTareasMAUI");
        
        // Crear el directorio si no existe
        Directory.CreateDirectory(appDataPath);
        
        // Ruta completa del archivo JSON
        _filePath = Path.Combine(appDataPath, "tareas.json");
        
        // Cargar las tareas del archivo
        CargarTareas();
    }

    /// <summary>
    /// Carga las tareas desde el archivo JSON.
    /// </summary>
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
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar tareas: {ex.Message}");
            _tareas = new List<Tarea>();
        }
    }

    /// <summary>
    /// Guarda las tareas en el archivo JSON.
    /// </summary>
    private void GuardarTareas()
    {
        try
        {
            var json = JsonSerializer.Serialize(_tareas, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al guardar tareas: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtiene todas las tareas ordenadas por fecha de creación.
    /// </summary>
    public List<Tarea> GetAll() => _tareas.OrderBy(t => t.FechaCreacion).ToList();

    /// <summary>
    /// Añade una nueva tarea a la lista.
    /// </summary>
    /// <param name="titulo">Título de la tarea</param>
    public void Add(string titulo)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            return;
            
        _tareas.Add(new Tarea { Titulo = titulo });
        GuardarTareas();
    }

    /// <summary>
    /// Alterna el estado de completada de una tarea.
    /// </summary>
    /// <param name="id">ID de la tarea</param>
    public void Toggle(Guid id)
    {
        var tarea = _tareas.FirstOrDefault(t => t.Id == id);
        if (tarea != null)
        {
            tarea.Completada = !tarea.Completada;
            GuardarTareas();
        }
    }

    /// <summary>
    /// Elimina una tarea de la lista.
    /// </summary>
    /// <param name="id">ID de la tarea</param>
    public void Remove(Guid id)
    {
        var tarea = _tareas.FirstOrDefault(t => t.Id == id);
        if (tarea != null)
        {
            _tareas.Remove(tarea);
            GuardarTareas();
        }
    }

    /// <summary>
    /// Obtiene el count de tareas pendientes.
    /// </summary>
    public int GetPendientes() => _tareas.Count(t => !t.Completada);
}