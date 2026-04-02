namespace GestionAcademica.Services.Dialogs;

/// <summary>
/// Define el contrato para mostrar diálogos modales al usuario.
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Muestra un diálogo de error.
    /// </summary>
    /// <param name="message">Mensaje de error.</param>
    /// <param name="title">Título de la ventana (por defecto "Error").</param>
    void ShowError(string message, string title = "Error");

    /// <summary>
    /// Muestra un diálogo de éxito.
    /// </summary>
    /// <param name="message">Mensaje de éxito.</param>
    /// <param name="title">Título de la ventana (por defecto "Éxito").</param>
    void ShowSuccess(string message, string title = "Éxito");

    /// <summary>
    /// Muestra un diálogo de advertencia.
    /// </summary>
    /// <param name="message">Mensaje de advertencia.</param>
    /// <param name="title">Título de la ventana (por defecto "Advertencia").</param>
    void ShowWarning(string message, string title = "Advertencia");

    /// <summary>
    /// Muestra un diálogo de información.
    /// </summary>
    /// <param name="message">Mensaje informativo.</param>
    /// <param name="title">Título de la ventana (por defecto "Información").</param>
    void ShowInfo(string message, string title = "Información");

    /// <summary>
    /// Muestra un diálogo de confirmación (Sí/No).
    /// </summary>
    /// <param name="message">Mensaje de confirmación.</param>
    /// <param name="title">Título de la ventana (por defecto "Confirmar").</param>
    /// <returns>True si el usuario pulsó Sí, False si pulsó No.</returns>
    bool ShowConfirmation(string message, string title = "Confirmar");
}
