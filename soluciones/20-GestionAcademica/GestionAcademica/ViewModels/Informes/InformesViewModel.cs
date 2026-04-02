using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpFunctionalExtensions;
using GestionAcademica.Enums;
using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Personas;
using GestionAcademica.Services.Personas;
using GestionAcademica.Services.Dialogs;
using GestionAcademica.Services.Report;
using Serilog;

namespace GestionAcademica.ViewModels.Informes;

/// <summary>
/// ViewModel para la generación de informes.
/// Permite generar informes en formato PDF y HTML de estudiantes y docentes.
/// </summary>
public partial class InformesViewModel(
    IPersonasService personasService,
    IReportService reportService,
    IDialogService dialogService
) : ObservableObject
{
    private readonly IPersonasService _personasService = personasService;
    private readonly IReportService _reportService = reportService;
    private readonly IDialogService _dialogService = dialogService;
    private readonly ILogger _logger = Log.ForContext<InformesViewModel>();

    [ObservableProperty]
    private string _statusMessage = "";

    [ObservableProperty]
    private bool _isGenerating;

    [ObservableProperty]
    private Ciclo? _selectedCiclo;

    [ObservableProperty]
    private Curso? _selectedCurso;

    [ObservableProperty]
    private double _notaAprobado = 5.0;

    [ObservableProperty]
    private bool _mostrarEliminados = false;

    [ObservableProperty]
    private bool _mostrarMenoresEdad = false;

    public IEnumerable<Ciclo> Ciclos => Enum.GetValues<Ciclo>();
    public IEnumerable<Curso> Cursos => Enum.GetValues<Curso>();

    [RelayCommand]
    private void GenerarInformeEstudiantesPdf()
    {
        try
        {
            IsGenerating = true;
            StatusMessage = "Generando informe de estudiantes...";

            var estudiantes = _personasService.GetEstudiantesOrderBy(
                TipoOrdenamiento.Nombre,
                1,
                1000,
                MostrarEliminados);

            var informeHtml = _reportService.GenerarInformeEstudiantesHtml(estudiantes, MostrarEliminados, MostrarMenoresEdad);
            if (informeHtml.IsFailure)
            {
                _dialogService.ShowError(informeHtml.Error.Message);
                return;
            }

            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF|*.pdf",
                FileName = $"Informe_Estudiantes_{DateTime.Now:yyyyMMdd}"
            };

            if (saveDialog.ShowDialog() == true)
            {
                var result = _reportService.GuardarInformePdf(informeHtml.Value, saveDialog.FileName);
                if (result.IsSuccess)
                {
                    StatusMessage = "Informe PDF generado";
                    _dialogService.ShowSuccess("Informe PDF generado correctamente");
                }
                else
                {
                    _dialogService.ShowError(result.Error.Message);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al generar informe estudiantes PDF");
            StatusMessage = "Error al generar";
        }
        finally
        {
            IsGenerating = false;
        }
    }

    [RelayCommand]
    private void GenerarListadoCompletoPdf()
    {
        try
        {
            IsGenerating = true;
            StatusMessage = "Generando listado completo PDF...";

            var personas = _personasService.GetAllOrderBy(
                TipoOrdenamiento.Nombre,
                null,
                1,
                1000,
                MostrarEliminados);

            var result = _reportService.GenerarListadoPersonasHtml(personas, MostrarEliminados, MostrarMenoresEdad);
            if (result.IsFailure)
            {
                _dialogService.ShowError(result.Error.Message);
                return;
            }

            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF|*.pdf",
                FileName = $"Listado_Personas_{DateTime.Now:yyyyMMdd}"
            };

            if (saveDialog.ShowDialog() == true)
            {
                var pdfResult = _reportService.GuardarInformePdf(result.Value, saveDialog.FileName);
                if (pdfResult.IsSuccess)
                {
                    StatusMessage = "Listado PDF generado";
                    _dialogService.ShowSuccess("Listado PDF generado correctamente");
                }
                else
                {
                    _dialogService.ShowError(pdfResult.Error.Message);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al generar listado PDF");
            StatusMessage = "Error al generar";
        }
        finally
        {
            IsGenerating = false;
        }
    }

    [RelayCommand]
    private void GenerarInformeEstudiantesHtml()
    {
        try
        {
            IsGenerating = true;
            StatusMessage = "Generando informe HTML...";

            var estudiantes = _personasService.GetEstudiantesOrderBy(
                TipoOrdenamiento.Nombre,
                1,
                1000,
                MostrarEliminados);

            var result = _reportService.GenerarInformeEstudiantesHtml(estudiantes, MostrarEliminados, MostrarMenoresEdad);
            if (result.IsFailure)
            {
                _dialogService.ShowError(result.Error.Message);
                return;
            }

            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "HTML|*.html",
                FileName = $"Informe_Estudiantes_{DateTime.Now:yyyyMMdd}"
            };

            if (saveDialog.ShowDialog() == true)
            {
                var saveResult = _reportService.GuardarInforme(result.Value, saveDialog.FileName);
                if (saveResult.IsSuccess)
                {
                    StatusMessage = "Informe HTML guardado";
                    _dialogService.ShowSuccess("Informe HTML generado correctamente");
                }
                else
                {
                    _dialogService.ShowError(saveResult.Error.Message);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al generar informe estudiantes HTML");
            StatusMessage = "Error al generar";
        }
        finally
        {
            IsGenerating = false;
        }
    }

    [RelayCommand]
    private void GenerarInformeDocentesPdf()
    {
        try
        {
            IsGenerating = true;
            StatusMessage = "Generando informe de docentes...";

            var docentes = _personasService.GetDocentesOrderBy(
                TipoOrdenamiento.Nombre,
                1,
                1000,
                MostrarEliminados);

            var informeHtml = _reportService.GenerarInformeDocentesHtml(docentes, MostrarEliminados);
            if (informeHtml.IsFailure)
            {
                _dialogService.ShowError(informeHtml.Error.Message);
                return;
            }

            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF|*.pdf",
                FileName = $"Informe_Docentes_{DateTime.Now:yyyyMMdd}"
            };

            if (saveDialog.ShowDialog() == true)
            {
                var result = _reportService.GuardarInformePdf(informeHtml.Value, saveDialog.FileName);
                if (result.IsSuccess)
                {
                    StatusMessage = "Informe PDF generado";
                    _dialogService.ShowSuccess("Informe PDF generado correctamente");
                }
                else
                {
                    _dialogService.ShowError(result.Error.Message);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al generar informe docentes PDF");
            StatusMessage = "Error al generar";
        }
        finally
        {
            IsGenerating = false;
        }
    }

    [RelayCommand]
    private void GenerarInformeDocentesHtml()
    {
        try
        {
            IsGenerating = true;
            StatusMessage = "Generando informe HTML...";

            var docentes = _personasService.GetDocentesOrderBy(
                TipoOrdenamiento.Nombre,
                1,
                1000,
                MostrarEliminados);

            var result = _reportService.GenerarInformeDocentesHtml(docentes, MostrarEliminados);
            if (result.IsFailure)
            {
                _dialogService.ShowError(result.Error.Message);
                return;
            }

            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "HTML|*.html",
                FileName = $"Informe_Docentes_{DateTime.Now:yyyyMMdd}"
            };

            if (saveDialog.ShowDialog() == true)
            {
                var saveResult = _reportService.GuardarInforme(result.Value, saveDialog.FileName);
                if (saveResult.IsSuccess)
                {
                    StatusMessage = "Informe HTML guardado";
                    _dialogService.ShowSuccess("Informe HTML generado correctamente");
                }
                else
                {
                    _dialogService.ShowError(saveResult.Error.Message);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al generar informe docentes HTML");
            StatusMessage = "Error al generar";
        }
        finally
        {
            IsGenerating = false;
        }
    }

    [RelayCommand]
    private void GenerarListadoCompletoHtml()
    {
        try
        {
            IsGenerating = true;
            StatusMessage = "Generando listado completo...";

            var personas = _personasService.GetAllOrderBy(
                TipoOrdenamiento.Nombre,
                null,
                1,
                1000,
                MostrarEliminados);

            var result = _reportService.GenerarListadoPersonasHtml(personas, MostrarEliminados, MostrarMenoresEdad);
            if (result.IsFailure)
            {
                _dialogService.ShowError(result.Error.Message);
                return;
            }

            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "HTML|*.html",
                FileName = $"Listado_Personas_{DateTime.Now:yyyyMMdd}"
            };

            if (saveDialog.ShowDialog() == true)
            {
                var saveResult = _reportService.GuardarInforme(result.Value, saveDialog.FileName);
                if (saveResult.IsSuccess)
                {
                    StatusMessage = "Listado HTML guardado";
                    _dialogService.ShowSuccess("Listado generado correctamente");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al generar listado");
            StatusMessage = "Error al generar";
        }
        finally
        {
            IsGenerating = false;
        }
    }
}
