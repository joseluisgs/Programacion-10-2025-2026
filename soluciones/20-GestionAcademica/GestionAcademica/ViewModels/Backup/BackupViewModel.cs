using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpFunctionalExtensions;
using GestionAcademica.Models.Personas;
using GestionAcademica.Services.Personas;
using GestionAcademica.Services.Backup;
using Serilog;

namespace GestionAcademica.ViewModels.Backup;

public partial class BackupViewModel : ObservableObject
{
    private readonly IPersonasService _personasService;
    private readonly IBackupService _backupService;
    private readonly ILogger _logger = Log.ForContext<BackupViewModel>();

    [ObservableProperty]
    private ObservableCollection<string> _backups = new();

    [ObservableProperty]
    private string? _selectedBackup;

    [ObservableProperty]
    private string _statusMessage = "";

    [ObservableProperty]
    private bool _isLoading;

    public BackupViewModel(IPersonasService personasService, IBackupService backupService)
    {
        _personasService = personasService;
        _backupService = backupService;
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
                MessageBox.Show($"Backup creado correctamente:\n{result.Value}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(result.Error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            MessageBox.Show("Selecciona un backup para restaurar", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = MessageBox.Show(
            $"¿Restaurar el backup {System.IO.Path.GetFileName(SelectedBackup)}?\nEsto reemplazará los datos actuales.",
            "Confirmar restauración",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes) return;

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
                MessageBox.Show($"Backup restaurado correctamente\n{restoreResult.Value} registros", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(restoreResult.Error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        var result = MessageBox.Show(
            $"¿Eliminar el backup {System.IO.Path.GetFileName(SelectedBackup)}?",
            "Confirmar",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
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
}