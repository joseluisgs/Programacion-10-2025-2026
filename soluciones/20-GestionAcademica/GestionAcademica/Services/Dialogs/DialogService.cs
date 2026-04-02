using System.Windows;

namespace GestionAcademica.Services.Dialogs;

/// <summary>
/// Servicio para mostrar diálogos modales al usuario.
/// Proporciona métodos para mostrar mensajes de error, éxito, advertencia y confirmaciones.
/// </summary>
public class DialogService : IDialogService
{
    public void ShowError(string message, string title = "Error")
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public void ShowSuccess(string message, string title = "Éxito")
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public void ShowWarning(string message, string title = "Advertencia")
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    public void ShowInfo(string message, string title = "Información")
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public bool ShowConfirmation(string message, string title = "Confirmar")
    {
        return MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question)
               == MessageBoxResult.Yes;
    }
}
