using ListaTareasBlazor.Models;

namespace ListaTareasBlazor.Services;

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
/// Implementación en memoria del servicio de tareas.
/// </summary>
/// <remarks>
/// Esta implementación guarda las tareas en una lista en memoria.
/// Es apropiada para pruebas y ejemplos, pero no persiste datos entre sesiones.
/// Para persistencia real, implementaríamos una versión que use archivo o base de datos.
/// </remarks>
public class TareaService : ITareaService
{
    /// <summary>
    /// Lista interna de tareas.
    /// </summary>
    private readonly List<Tarea> _tareas = new();

    /// <summary>
    /// Obtiene todas las tareas.
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
        }
    }

    /// <summary>
    /// Obtiene el count de tareas pendientes.
    /// </summary>
    public int GetPendientes() => _tareas.Count(t => !t.Completada);
}