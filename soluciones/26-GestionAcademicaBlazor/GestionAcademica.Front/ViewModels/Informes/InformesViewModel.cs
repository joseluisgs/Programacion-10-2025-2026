using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionAcademica.Enums;
using GestionAcademica.Models.Academia;
using GestionAcademica.Services.Dialogs;
using GestionAcademica.Services.Personas;
using GestionAcademica.Services.Report;
using Microsoft.Win32;
using Serilog;

namespace GestionAcademica.ViewModels.Informes;

/// <summary>
///     ViewModel para la generación de informes.
///     Permite generar informes en formato PDF y HTML de estudiantes y docentes.
/// </summary>
public partial class InformesViewModel(
    IPersonasService personasService,
    IReportService reportService,
    IDialogService dialogService
) : ObservableObject {
    private readonly IDialogService _dialogService = dialogService;
    private readonly ILogger _logger = Log.ForContext<InformesViewModel>();
    private readonly IPersonasService _personasService = personasService;
    private readonly IReportService _reportService = reportService;

    [ObservableProperty] private bool _isGenerating;

    [ObservableProperty] private bool _mostrarEliminados;

    [ObservableProperty] private bool _mostrarMenoresEdad;

    [ObservableProperty] private double _notaAprobado = 5.0;

    [ObservableProperty] private Ciclo? _selectedCiclo;

    [ObservableProperty] private Curso? _selectedCurso;

    [ObservableProperty] private string _statusMessage = "";

    public IEnumerable<Ciclo> Ciclos => Enum.GetValues<Ciclo>();
    public IEnumerable<Curso> Cursos => Enum.GetValues<Curso>();

    [RelayCommand]
    private async Task GenerarInformeEstudiantesPdfAsync() {
        try {
            IsGenerating = true;
            StatusMessage = "Generando informe de estudiantes...";

            var estudiantes = await _personasService.GetEstudiantesOrderByAsync(
                TipoOrdenamiento.Nombre,
                1,
                1000,
                MostrarEliminados);

            var informeHtml =
                await _reportService.GenerarInformeEstudiantesHtmlAsync(estudiantes, MostrarEliminados, MostrarMenoresEdad);
            if (informeHtml.IsFailure) {
                _dialogService.ShowError(informeHtml.Error.Message);
                return;
            }

            var saveDialog = new SaveFileDialog {
                Filter = "PDF|*.pdf",
                FileName = $"Informe_Estudiantes_{DateTime.Now:yyyyMMdd}"
            };

            if (saveDialog.ShowDialog() == true) {
                var result = await _reportService.GuardarInformePdfAsync(informeHtml.Value, saveDialog.FileName);
                if (result.IsSuccess) {
                    StatusMessage = "Informe PDF generado";
                    _dialogService.ShowSuccess("Informe PDF generado correctamente");
                }
                else {
                    _dialogService.ShowError(result.Error.Message);
                }
            }
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al generar informe estudiantes PDF");
            StatusMessage = "Error al generar";
        }
        finally {
            IsGenerating = false;
        }
    }

    [RelayCommand]
    private async Task GenerarListadoCompletoPdfAsync() {
        try {
            IsGenerating = true;
            StatusMessage = "Generando listado completo PDF...";

            var personas = await _personasService.GetAllOrderByAsync(
                TipoOrdenamiento.Nombre,
                null,
                1,
                1000,
                MostrarEliminados);

            var result = await _reportService.GenerarListadoPersonasHtmlAsync(personas, MostrarEliminados, MostrarMenoresEdad);
            if (result.IsFailure) {
                _dialogService.ShowError(result.Error.Message);
                return;
            }

            var saveDialog = new SaveFileDialog {
                Filter = "PDF|*.pdf",
                FileName = $"Listado_Personas_{DateTime.Now:yyyyMMdd}"
            };

            if (saveDialog.ShowDialog() == true) {
                var pdfResult = await _reportService.GuardarInformePdfAsync(result.Value, saveDialog.FileName);
                if (pdfResult.IsSuccess) {
                    StatusMessage = "Listado PDF generado";
                    _dialogService.ShowSuccess("Listado PDF generado correctamente");
                }
                else {
                    _dialogService.ShowError(pdfResult.Error.Message);
                }
            }
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al generar listado PDF");
            StatusMessage = "Error al generar";
        }
        finally {
            IsGenerating = false;
        }
    }

    [RelayCommand]
    private async Task GenerarInformeEstudiantesHtmlAsync() {
        try {
            IsGenerating = true;
            StatusMessage = "Generando informe HTML...";

            var estudiantes = await _personasService.GetEstudiantesOrderByAsync(
                TipoOrdenamiento.Nombre,
                1,
                1000,
                MostrarEliminados);

            var result =
                await _reportService.GenerarInformeEstudiantesHtmlAsync(estudiantes, MostrarEliminados, MostrarMenoresEdad);
            if (result.IsFailure) {
                _dialogService.ShowError(result.Error.Message);
                return;
            }

            var saveDialog = new SaveFileDialog {
                Filter = "HTML|*.html",
                FileName = $"Informe_Estudiantes_{DateTime.Now:yyyyMMdd}"
            };

            if (saveDialog.ShowDialog() == true) {
                var saveResult = await _reportService.GuardarInformeAsync(result.Value, saveDialog.FileName);
                if (saveResult.IsSuccess) {
                    StatusMessage = "Informe HTML guardado";
                    _dialogService.ShowSuccess("Informe HTML generado correctamente");
                }
                else {
                    _dialogService.ShowError(saveResult.Error.Message);
                }
            }
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al generar informe estudiantes HTML");
            StatusMessage = "Error al generar";
        }
        finally {
            IsGenerating = false;
        }
    }

    [RelayCommand]
    private async Task GenerarInformeDocentesPdfAsync() {
        try {
            IsGenerating = true;
            StatusMessage = "Generando informe de docentes...";

            var docentes = await _personasService.GetDocentesOrderByAsync(
                TipoOrdenamiento.Nombre,
                1,
                1000,
                MostrarEliminados);

            var informeHtml = await _reportService.GenerarInformeDocentesHtmlAsync(docentes, MostrarEliminados);
            if (informeHtml.IsFailure) {
                _dialogService.ShowError(informeHtml.Error.Message);
                return;
            }

            var saveDialog = new SaveFileDialog {
                Filter = "PDF|*.pdf",
                FileName = $"Informe_Docentes_{DateTime.Now:yyyyMMdd}"
            };

            if (saveDialog.ShowDialog() == true) {
                var result = await _reportService.GuardarInformePdfAsync(informeHtml.Value, saveDialog.FileName);
                if (result.IsSuccess) {
                    StatusMessage = "Informe PDF generado";
                    _dialogService.ShowSuccess("Informe PDF generado correctamente");
                }
                else {
                    _dialogService.ShowError(result.Error.Message);
                }
            }
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al generar informe docentes PDF");
            StatusMessage = "Error al generar";
        }
        finally {
            IsGenerating = false;
        }
    }

    [RelayCommand]
    private async Task GenerarInformeDocentesHtmlAsync() {
        try {
            IsGenerating = true;
            StatusMessage = "Generando informe HTML...";

            var docentes = await _personasService.GetDocentesOrderByAsync(
                TipoOrdenamiento.Nombre,
                1,
                1000,
                MostrarEliminados);

            var result = await _reportService.GenerarInformeDocentesHtmlAsync(docentes, MostrarEliminados);
            if (result.IsFailure) {
                _dialogService.ShowError(result.Error.Message);
                return;
            }

            var saveDialog = new SaveFileDialog {
                Filter = "HTML|*.html",
                FileName = $"Informe_Docentes_{DateTime.Now:yyyyMMdd}"
            };

            if (saveDialog.ShowDialog() == true) {
                var saveResult = await _reportService.GuardarInformeAsync(result.Value, saveDialog.FileName);
                if (saveResult.IsSuccess) {
                    StatusMessage = "Informe HTML guardado";
                    _dialogService.ShowSuccess("Informe HTML generado correctamente");
                }
                else {
                    _dialogService.ShowError(saveResult.Error.Message);
                }
            }
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al generar informe docentes HTML");
            StatusMessage = "Error al generar";
        }
        finally {
            IsGenerating = false;
        }
    }

    [RelayCommand]
    private async Task GenerarListadoCompletoHtmlAsync() {
        try {
            IsGenerating = true;
            StatusMessage = "Generando listado completo...";

            var personas = await _personasService.GetAllOrderByAsync(
                TipoOrdenamiento.Nombre,
                null,
                1,
                1000,
                MostrarEliminados);

            var result = await _reportService.GenerarListadoPersonasHtmlAsync(personas, MostrarEliminados, MostrarMenoresEdad);
            if (result.IsFailure) {
                _dialogService.ShowError(result.Error.Message);
                return;
            }

            var saveDialog = new SaveFileDialog {
                Filter = "HTML|*.html",
                FileName = $"Listado_Personas_{DateTime.Now:yyyyMMdd}"
            };

            if (saveDialog.ShowDialog() == true) {
                var saveResult = await _reportService.GuardarInformeAsync(result.Value, saveDialog.FileName);
                if (saveResult.IsSuccess) {
                    StatusMessage = "Listado HTML guardado";
                    _dialogService.ShowSuccess("Listado generado correctamente");
                }
            }
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al generar listado");
            StatusMessage = "Error al generar";
        }
        finally {
            IsGenerating = false;
        }
    }
}
