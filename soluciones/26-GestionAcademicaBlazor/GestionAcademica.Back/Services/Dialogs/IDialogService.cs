namespace GestionAcademica.Services.Dialogs;

public interface IDialogService {
    void ShowError(string message, string title = "Error");
    void ShowSuccess(string message, string title = "Éxito");
    void ShowWarning(string message, string title = "Advertencia");
    void ShowInfo(string message, string title = "Información");
    Task<bool> ShowConfirmationAsync(string message, string title = "Confirmar");
}
