using GestionAcademica.Services.Dialogs;
using Microsoft.JSInterop;
using Serilog;

namespace GestionAcademica.Blazor.Services;

public class BlazorDialogService : IDialogService
{
    private readonly Serilog.ILogger _logger = Log.ForContext<BlazorDialogService>();
    private readonly IJSRuntime _js;

    public BlazorDialogService(IJSRuntime js)
    {
        _js = js;
    }

    public event Action<string, string>? OnShowError;
    public event Action<string, string>? OnShowSuccess;
    public event Action<string, string>? OnShowWarning;
    public event Action<string, string>? OnShowInfo;

    public void ShowError(string message, string title = "Error")
    {
        _logger.Error("{Title}: {Message}", title, message);
        OnShowError?.Invoke(title, message);
    }

    public void ShowSuccess(string message, string title = "Exito")
    {
        _logger.Information("{Title}: {Message}", title, message);
        OnShowSuccess?.Invoke(title, message);
    }

    public void ShowWarning(string message, string title = "Advertencia")
    {
        _logger.Warning("{Title}: {Message}", title, message);
        OnShowWarning?.Invoke(title, message);
    }

    public void ShowInfo(string message, string title = "Informacion")
    {
        _logger.Information("{Title}: {Message}", title, message);
        OnShowInfo?.Invoke(title, message);
    }

    public async Task<bool> ShowConfirmationAsync(string message, string title = "Confirmar")
    {
        _logger.Information("{Title}: {Message}", title, message);
        try
        {
            // InvokeAsync<bool> es el método RECOMENDADO para Blazor Server:
            // encaja con el modelo async, no necesita IJSInProcessRuntime,
            // y evita confusiones entre paradigmas síncrono/asíncrono.
            return await _js.InvokeAsync<bool>("confirm", message);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al mostrar confirmacion");
            return false;
        }
    }
}
