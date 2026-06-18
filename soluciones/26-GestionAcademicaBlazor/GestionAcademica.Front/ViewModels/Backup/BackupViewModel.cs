using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionAcademica.Services.Backup;
using GestionAcademica.Services.Dialogs;
using GestionAcademica.Services.Personas;
using Serilog;

namespace GestionAcademica.ViewModels.Backup;

/// <summary>
///     ViewModel para la gestión de copias de seguridad.
///     Permite crear, restaurar y eliminar backups del sistema.
/// </summary>
public partial class BackupViewModel : ObservableObject {
    private readonly IBackupService _backupService;
    private readonly IDialogService _dialogService;
    private readonly ILogger _logger = Log.ForContext<BackupViewModel>();
    private readonly IPersonasService _personasService;

    [ObservableProperty] private ObservableCollection<string> _backups = new();

    [ObservableProperty] private bool _isLoading;

    [ObservableProperty] private string? _selectedBackup;

    [ObservableProperty] private string _statusMessage = "";

    public BackupViewModel(
        IPersonasService personasService,
        IBackupService backupService,
        IDialogService dialogService) {
        _personasService = personasService;
        _backupService = backupService;
        _dialogService = dialogService;
        _ = LoadBackupsAsync();
    }

    /// <summary>
    ///     Carga la lista de archivos de backup disponibles en el directorio de backups.
    /// </summary>
    private async Task LoadBackupsAsync() {
        try {
            var backupList = await _backupService.ListarBackupsAsync();
            Backups = new ObservableCollection<string>(backupList);
            StatusMessage = $"Encontrados {Backups.Count} backups";
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al cargar backups");
            StatusMessage = "Error al cargar backups";
        }
    }

    [RelayCommand]
    private async Task RealizarBackupAsync() {
        try {
            IsLoading = true;
            StatusMessage = "Realizando backup...";

            var personas = await _personasService.GetAllAsync(1, 1000);
            var result = await _backupService.RealizarBackupAsync(personas);

            if (result.IsSuccess) {
                await LoadBackupsAsync();
                StatusMessage = $"Backup creado: {Path.GetFileName(result.Value)}";
                _dialogService.ShowSuccess($"Backup creado correctamente:\n{result.Value}");
            }
            else {
                _dialogService.ShowError(result.Error.Message);
                StatusMessage = "Error al crear backup";
            }
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al realizar backup");
            StatusMessage = "Error al crear backup";
        }
        finally {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task RestaurarBackupAsync() {
        if (string.IsNullOrEmpty(SelectedBackup)) {
            _dialogService.ShowWarning("Selecciona un backup para restaurar");
            return;
        }

        if (!await _dialogService.ShowConfirmationAsync(
                $"¿Restaurar el backup {Path.GetFileName(SelectedBackup)}?\n\n" +
                $"⚠️ ADVERTENCIA: Se borrarán TODOS los datos actuales (personas e imágenes)\n" +
                $"y se reemplazarán por el contenido de la copia de seguridad.\n\n" +
                $"Esta acción no se puede deshacer.",
                "Confirmar restauración"))
            return;

        try {
            IsLoading = true;
            StatusMessage = "Restaurando backup...";

            var restoreResult = await _backupService.RestaurarBackupSistemaAsync(
                SelectedBackup,
                () => _personasService.DeleteAllAsync().GetAwaiter().GetResult(),
                p => _personasService.SaveAsync(p).GetAwaiter().GetResult());

            if (restoreResult.IsSuccess) {
                StatusMessage = $"Restaurados {restoreResult.Value} registros";
                _dialogService.ShowSuccess($"Backup restaurado correctamente\n{restoreResult.Value} registros");
            }
            else {
                _dialogService.ShowError(restoreResult.Error.Message);
                StatusMessage = "Error al restaurar";
            }
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al restaurar backup");
            StatusMessage = "Error al restaurar";
        }
        finally {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync() {
        await LoadBackupsAsync();
    }

    [RelayCommand]
    private async Task EliminarBackupAsync() {
        if (string.IsNullOrEmpty(SelectedBackup)) return;

        if (!await _dialogService.ShowConfirmationAsync($"¿Eliminar el backup {Path.GetFileName(SelectedBackup)}?"))
            return;

        try {
            File.Delete(SelectedBackup);
            await LoadBackupsAsync();
            StatusMessage = "Backup eliminado";
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al eliminar backup");
            StatusMessage = "Error al eliminar";
        }
    }
}
