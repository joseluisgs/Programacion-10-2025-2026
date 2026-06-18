using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionAcademica.Config;
using GestionAcademica.Models.Personas;
using GestionAcademica.Services.Dialogs;
using GestionAcademica.Services.ImportExport;
using GestionAcademica.Services.Personas;
using Microsoft.Win32;
using Serilog;

namespace GestionAcademica.ViewModels.ImportExport;

/// <summary>
///     ViewModel para la importación y exportación de datos.
///     Permite exportar e importar personas en formato CSV y JSON.
/// </summary>
public partial class ImportExportViewModel(
    IPersonasService personasService,
    IImportExportService importExportService,
    IDialogService dialogService
) : ObservableObject {
    private readonly IDialogService _dialogService = dialogService;
    private readonly IImportExportService _importExportService = importExportService;
    private readonly ILogger _logger = Log.ForContext<ImportExportViewModel>();
    private readonly IPersonasService _personasService = personasService;

    [ObservableProperty] private bool _isLoading;

    [ObservableProperty] private string _statusMessage = "";

    [ObservableProperty] private bool _sustituirDatos;

    [RelayCommand]
    private async Task ExportarCsvAsync() {
        try {
            IsLoading = true;
            StatusMessage = "Exportando datos...";

            var dialog = new SaveFileDialog {
                Filter = "CSV|*.csv",
                FileName = $"Exportacion_{DateTime.Now:yyyyMMdd}"
            };

            if (dialog.ShowDialog() == true) {
                var personas = await _personasService.GetAllAsync(1, 1000, false);
                var csvPath = Path.Combine(AppConfig.DataFolder, "personas.csv");
                var result = await _importExportService.ExportarDatosAsync(personas, csvPath);

                if (result.IsSuccess) {
                    File.Copy(Path.Combine(AppConfig.DataFolder, "personas.csv"), dialog.FileName, true);
                    StatusMessage = $"Exportados {result.Value} registros";
                    _dialogService.ShowSuccess($"Exportación completada\n{result.Value} registros");
                }
                else {
                    _dialogService.ShowError(result.Error.Message);
                    StatusMessage = "Error al exportar";
                }
            }
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al exportar");
            StatusMessage = "Error al exportar";
        }
        finally {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ImportarCsvAsync() {
        try {
            var dialog = new OpenFileDialog {
                Filter = "CSV|*.csv",
                Title = "Seleccionar archivo CSV"
            };

            if (dialog.ShowDialog() != true) return;

            IsLoading = true;
            StatusMessage = "Importando datos...";

            if (SustituirDatos) await _personasService.DeleteAllAsync();

            var result = await _importExportService.ImportarDatosAsync(dialog.FileName);

            if (result.IsSuccess) {
                var count = result.Value.Count();
                StatusMessage = $"Importados {count} registros";
                _dialogService.ShowSuccess($"Importación completada\n{count} registros");
            }
            else {
                _dialogService.ShowError(result.Error.Message);
                StatusMessage = "Error al importar";
            }
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al importar");
            _dialogService.ShowError($"Error al importar: {ex.Message}");
            StatusMessage = "Error al importar";
        }
        finally {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ExportarJsonAsync() {
        try {
            IsLoading = true;
            StatusMessage = "Exportando JSON...";

            var dialog = new SaveFileDialog {
                Filter = "JSON|*.json",
                FileName = $"Exportacion_{DateTime.Now:yyyyMMdd}"
            };

            if (dialog.ShowDialog() == true) {
                var personas = await _personasService.GetAllAsync(1, 1000, false);
                var options = new JsonSerializerOptions {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                var json = JsonSerializer.Serialize(personas, options);
                File.WriteAllText(dialog.FileName, json);

                StatusMessage = "Exportación JSON completada";
                _dialogService.ShowSuccess("Exportación JSON completada");
            }
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al exportar JSON");
            StatusMessage = "Error al exportar";
        }
        finally {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ImportarJsonAsync() {
        try {
            var dialog = new OpenFileDialog {
                Filter = "JSON|*.json",
                Title = "Seleccionar archivo JSON"
            };

            if (dialog.ShowDialog() != true) return;

            IsLoading = true;
            StatusMessage = "Importando JSON...";

            if (SustituirDatos) await _personasService.DeleteAllAsync();

            var json = File.ReadAllText(dialog.FileName);
            var options = new JsonSerializerOptions {
                PropertyNameCaseInsensitive = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            var personas = JsonSerializer.Deserialize<IEnumerable<Persona>>(json, options);

            if (personas != null) {
                var count = 0;
                foreach (var persona in personas) {
                    var result = await _personasService.SaveAsync(persona);
                    if (result.IsSuccess) count++;
                }

                StatusMessage = $"Importados {count} registros";
                _dialogService.ShowSuccess($"Importación completada\n{count} registros");
            }
            else {
                _dialogService.ShowError("El archivo JSON no tiene un formato válido");
                StatusMessage = "Error al importar";
            }
        }
        catch (JsonException ex) {
            _logger.Error(ex, "Error al importar JSON - formato inválido");
            _dialogService.ShowError($"Error al importar JSON: El formato del archivo no es válido.\n\nDetalles: {ex.Message}");
            StatusMessage = "Error al importar";
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al importar JSON");
            _dialogService.ShowError($"Error al importar: {ex.Message}");
            StatusMessage = "Error al importar";
        }
        finally {
            IsLoading = false;
        }
    }
}
