using Serilog;

namespace GestionAcademica.Services.Dialogs;

public class DialogService : IDialogService {
    private readonly ILogger _logger = Log.ForContext<DialogService>();

    public void ShowError(string message, string title = "Error") {
        _logger.Error("{Title}: {Message}", title, message);
    }

    public void ShowSuccess(string message, string title = "Éxito") {
        _logger.Information("{Title}: {Message}", title, message);
    }

    public void ShowWarning(string message, string title = "Advertencia") {
        _logger.Warning("{Title}: {Message}", title, message);
    }

    public void ShowInfo(string message, string title = "Información") {
        _logger.Information("{Title}: {Message}", title, message);
    }

    public bool ShowConfirmation(string message, string title = "Confirmar") {
        _logger.Information("{Title}: {Message}", title, message);
        return true;
    }

    public Task<bool> ShowConfirmationAsync(string message, string title = "Confirmar") {
        _logger.Information("{Title}: {Message}", title, message);
        return Task.FromResult(true);
    }
}