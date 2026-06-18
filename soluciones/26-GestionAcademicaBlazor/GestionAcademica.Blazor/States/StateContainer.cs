using System;

namespace GestionAcademica.Blazor.Services;

/// <summary>
///     StateContainer (Singleton): servicio de comunicación entre páginas.
///     Sigue el patrón publicador-suscriptor (event bus):
///     - Cualquier página que modifica datos PUBLICA un evento (NotifyChange)
///     - Cualquier página que muestra datos SE SUSCRIBE al evento (OnChange)
///     - Cuando el evento se dispara, las páginas suscritas se refrescan automáticamente
///     - Al destruirse la página (Dispose), se desuscribe para evitar memory leaks
///     - Es Singleton: todos los componentes comparten la misma instancia
/// </summary>
public class StateContainer
{
    private readonly Serilog.ILogger _logger = Serilog.Log.ForContext<StateContainer>();

    // Action es un delegado sin parámetros ni valor de retorno.
    // Las páginas se suscriben con: State.OnChange += MiMetodo
    // donde MiMetodo es un método void sin parámetros.
    // Al usar Action en vez de EventHandler, evitamos tener que
    // declarar parámetros (object? sender, EventArgs e) que no usamos.
    public event Action? OnChange;

    /// <summary>
    ///     Notifica a todas las páginas suscritas que los datos han cambiado.
    ///     Se llama desde:
    ///     - Estudiantes.razor (al crear/editar/eliminar/restaurar un estudiante)
    ///     - Docentes.razor (al crear/editar/eliminar/restaurar un docente)
    ///     - ImportExport.razor (al importar datos)
    ///     - Backup.razor (al restaurar un backup)
    /// </summary>
    public void NotifyChange()
    {
        _logger.Debug("StateContainer: notificando cambio a subscriptores");
        OnChange?.Invoke();
    }
}
