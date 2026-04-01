using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpFunctionalExtensions;
using GestionAcademica.Models.Personas;
using GestionAcademica.Services.Personas;
using GestionAcademica.Services.Backup;
using GestionAcademica.Services.Dialogs;
using Serilog;

namespace GestionAcademica.ViewModels.Backup;

public partial class BackupViewModel : ObservableObject
{
    private readonly IPersonasService _personasService;
    private readonly IBackupService _backupService;
    private readonly IDialogService _dialogService;
    private readonly ILogger _logger = Log.ForContext<BackupViewModel>();

    [ObservableProperty]
    private ObservableCollection<string> _backups = new();

    [ObservableProperty]
    private string? _selectedBackup;

    [ObservableProperty]
    private string _statusMessage = "";

    [ObservableProperty]
    private bool _isLoading;

    public BackupViewModel(IPersonasService personasService, IBackupService backupService, IDialogService dialogService)
    {
        _personasService = personasService;
        _backupService = backupService;
        _dialogService = dialogService;
        LoadBackups();
    }

    private void LoadBackups()
    {
        try
        {
            var backupList = _backupService.ListarBackups();
            Backups = new ObservableCollection<string>(backupList);
            StatusMessage = $"Encontrados {Backups.Count} backups";
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al cargar backups");
            StatusMessage = "Error al cargar backups";
        }
    }

    [RelayCommand]
    private void RealizarBackup()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Realizando backup...";

            var personas = _personasService.GetAll(1, 1000, true);
            var result = _backupService.RealizarBackup(personas);

            if (result.IsSuccess)
            {
                LoadBackups();
                StatusMessage = $"Backup creado: {System.IO.Path.GetFileName(result.Value)}";
                _dialogService.ShowSuccess($"Backup creado correctamente:\n{result.Value}");
            }
            else
            {
                _dialogService.ShowError(result.Error.Message);
                StatusMessage = "Error al crear backup";
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al realizar backup");
            StatusMessage = "Error al crear backup";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void RestaurarBackup()
    {
        if (string.IsNullOrEmpty(SelectedBackup))
        {
            _dialogService.ShowWarning("Selecciona un backup para restaurar");
            return;
        }

        if (!_dialogService.ShowConfirmation(
            $"¿Restaurar el backup {System.IO.Path.GetFileName(SelectedBackup)}?\nEsto reemplazará los datos actuales.",
            "Confirmar restauración"))
            return;

        try
        {
            IsLoading = true;
            StatusMessage = "Restaurando backup...";

            var restoreResult = _backupService.RestaurarBackupSistema(
                SelectedBackup,
                p => _personasService.Save(p));

            if (restoreResult.IsSuccess)
            {
                StatusMessage = $"Restaurados {restoreResult.Value} registros";
                _dialogService.ShowSuccess($"Backup restaurado correctamente\n{restoreResult.Value} registros");
            }
            else
            {
                _dialogService.ShowError(restoreResult.Error.Message);
                StatusMessage = "Error al restaurar";
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al restaurar backup");
            StatusMessage = "Error al restaurar";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void Refresh()
    {
        LoadBackups();
    }

    [RelayCommand]
    private void EliminarBackup()
    {
        if (string.IsNullOrEmpty(SelectedBackup)) return;

        if (!_dialogService.ShowConfirmation($"¿Eliminar el backup {System.IO.Path.GetFileName(SelectedBackup)}?"))
            return;

        try
        {
            System.IO.File.Delete(SelectedBackup);
            LoadBackups();
            StatusMessage = "Backup eliminado";
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al eliminar backup");
            StatusMessage = "Error al eliminar";
        }
    }
}
