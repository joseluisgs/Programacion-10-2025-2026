namespace ListaTareasMAUI.Models;

/// <summary>
/// Modelo que representa una tarea en la lista.
/// </summary>
/// <remarks>
/// Este modelo sigue el patrón de diseño "POCO" (Plain Old CLR Object).
/// Es una clase simple con propiedades que no depende de ningún framework.
/// Se usa tanto para almacenar los datos como para la lógica de presentación.
/// </remarks>
public class Tarea
{
    /// <summary>
    /// Identificador único de la tarea.
    /// Se genera automáticamente usando un GUID.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Título o descripción de la tarea.
    /// </summary>
    /// <remarks>
    /// No puede estar vacío. Se valida antes de añadir.
    /// </remarks>
    public string Titulo { get; set; } = string.Empty;

    /// <summary>
    /// Indica si la tarea ha sido completada.
    /// </summary>
    /// <remarks>
    /// Por defecto es false (pendiente). Se togglea con el CheckBox.
    /// </remarks>
    public bool Completada { get; set; } = false;

    /// <summary>
    /// Fecha de creación de la tarea.
    /// </summary>
    /// <remarks>
    /// Se establece automáticamente al crear la tarea.
    /// </remarks>
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
}