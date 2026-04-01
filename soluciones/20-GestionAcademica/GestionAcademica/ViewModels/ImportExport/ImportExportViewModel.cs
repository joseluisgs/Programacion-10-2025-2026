using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpFunctionalExtensions;
using GestionAcademica.Config;
using GestionAcademica.Models.Personas;
using GestionAcademica.Services.Personas;
using GestionAcademica.Services.Dialogs;
using GestionAcademica.Services.ImportExport;
using Serilog;

namespace GestionAcademica.ViewModels.ImportExport;

public partial class ImportExportViewModel : ObservableObject
{
    private readonly IPersonasService _personasService;
    private readonly IImportExportService _importExportService;
    private readonly IDialogService _dialogService;
    private readonly ILogger _logger = Log.ForContext<ImportExportViewModel>();

    [ObservableProperty]
    private string _statusMessage = "";

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _sustituirDatos = false;

    public ImportExportViewModel(IPersonasService personasService, IImportExportService importExportService, IDialogService dialogService)
    {
        _personasService = personasService;
        _importExportService = importExportService;
        _dialogService = dialogService;
    }

    [RelayCommand]
    private void ExportarCsv()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Exportando datos...";

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "CSV|*.csv",
                FileName = $"Exportacion_{DateTime.Now:yyyyMMdd}"
            };

            if (dialog.ShowDialog() == true)
            {
                var personas = _personasService.GetAll(1, 1000, false);
                var result = _importExportService.ExportarDatosSistema(personas);

                if (result.IsSuccess)
                {
                    System.IO.File.Copy(System.IO.Path.Combine(AppConfig.DataFolder, "personas.csv"), dialog.FileName, true);
                    StatusMessage = $"Exportados {result.Value} registros";
                    _dialogService.ShowSuccess($"Exportación completada\n{result.Value} registros");
                }
                else
                {
                    _dialogService.ShowError(result.Error.Message);
                    StatusMessage = "Error al exportar";
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al exportar");
            StatusMessage = "Error al exportar";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void ImportarCsv()
    {
        try
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "CSV|*.csv",
                Title = "Seleccionar archivo CSV"
            };

            if (dialog.ShowDialog() != true) return;

            IsLoading = true;
            StatusMessage = "Importando datos...";

            if (SustituirDatos)
            {
                _personasService.DeleteAll();
            }

            var result = _importExportService.ImportarDatosSistema(dialog.FileName);

            if (result.IsSuccess)
            {
                var count = result.Value.Count();
                StatusMessage = $"Importados {count} registros";
                _dialogService.ShowSuccess($"Importación completada\n{count} registros");
            }
            else
            {
                _dialogService.ShowError(result.Error.Message);
                StatusMessage = "Error al importar";
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al importar");
            StatusMessage = "Error al importar";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void ExportarJson()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Exportando JSON...";

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "JSON|*.json",
                FileName = $"Exportacion_{DateTime.Now:yyyyMMdd}"
            };

            if (dialog.ShowDialog() == true)
            {
                var personas = _personasService.GetAll(1, 1000, false);
                var json = System.Text.Json.JsonSerializer.Serialize(personas, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                System.IO.File.WriteAllText(dialog.FileName, json);

                StatusMessage = "Exportación JSON completada";
                _dialogService.ShowSuccess("Exportación JSON completada");
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al exportar JSON");
            StatusMessage = "Error al exportar";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void ImportarJson()
    {
        try
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JSON|*.json",
                Title = "Seleccionar archivo JSON"
            };

            if (dialog.ShowDialog() != true) return;

            IsLoading = true;
            StatusMessage = "Importando JSON...";

            if (SustituirDatos)
            {
                _personasService.DeleteAll();
            }

            var json = System.IO.File.ReadAllText(dialog.FileName);
            var personas = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<Persona>>(json);

            if (personas != null)
            {
                int count = 0;
                foreach (var persona in personas)
                {
                    var result = _personasService.Save(persona);
                    if (result.IsSuccess) count++;
                }

                StatusMessage = $"Importados {count} registros";
                _dialogService.ShowSuccess($"Importación completada\n{count} registros");
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al importar JSON");
            StatusMessage = "Error al importar";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
