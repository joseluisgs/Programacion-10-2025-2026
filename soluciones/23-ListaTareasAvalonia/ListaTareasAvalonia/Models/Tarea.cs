using System;

namespace ListaTareasAvalonia.Models;

/// <summary>
/// Modelo que representa una tarea en la lista.
/// </summary>
public class Tarea
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Titulo { get; set; } = string.Empty;
    public bool Completada { get; set; } = false;
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
}