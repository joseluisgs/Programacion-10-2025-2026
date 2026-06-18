using System.Threading.Tasks;
using System.Windows;
using GestionAcademica.Services.Dialogs;

namespace GestionAcademica.Infrastructure;

public class WpfDialogService : IDialogService
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

    public Task<bool> ShowConfirmationAsync(string message, string title = "Confirmar")
    {
        // WPF usa MessageBox (síncrono por naturaleza). Lo ejecutamos en el hilo de UI
        // y envolvemos en Task.FromResult para cumplir con el contrato async de la interfaz.
        var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question)
                     == MessageBoxResult.Yes;
        return Task.FromResult(result);
    }
}
