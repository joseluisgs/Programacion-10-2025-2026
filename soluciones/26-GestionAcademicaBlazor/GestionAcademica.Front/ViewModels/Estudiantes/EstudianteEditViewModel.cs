using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionAcademica.Mappers.Personas;
using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Personas;
using GestionAcademica.Services.Dialogs;
using GestionAcademica.Services.Images;
using GestionAcademica.Services.Personas;
using GestionAcademica.Validators.Personas;
using GestionAcademica.ViewModels.Forms;
using Microsoft.Win32;
using Serilog;

namespace GestionAcademica.ViewModels.Estudiantes;

/// <summary>
///     ViewModel especializado para la ventana modal de creación y edición de Estudiante.
///     Trabaja con EstudianteFormData (IDataErrorInfo) y mapea al modelo de dominio para persistir.
/// </summary>
public partial class EstudianteEditViewModel(
    Estudiante estudiante,
    IPersonasService personasService,
    IImageService imageService,
    IDialogService dialogService,
    bool isNew
) : ObservableObject {
    private readonly IDialogService _dialogService = dialogService;
    private readonly IImageService _imageService = imageService;
    private readonly bool _isNew = isNew;
    private readonly ILogger _logger = Log.ForContext<EstudianteEditViewModel>();
    private readonly IPersonasService _personasService = personasService;

    /// <summary>FormData con validación IDataErrorInfo para el binding WPF.</summary>
    [ObservableProperty] private EstudianteFormData _formData = estudiante.ToFormData();

    /// <summary>
    ///     Título de la ventana según el modo de operación (nuevo o edición).
    /// </summary>
    [ObservableProperty] private string _windowTitle = isNew ? "Nuevo Estudiante" : "Editar Estudiante";

    public IEnumerable<Ciclo> Ciclos => Enum.GetValues<Ciclo>();
    public IEnumerable<Curso> Cursos => Enum.GetValues<Curso>();

    public Action<bool>? CloseAction { get; set; }

    /// <summary>
    ///     Guarda el estudiante si el FormData es válido, mapeando de vuelta al modelo de dominio.
    /// </summary>
    [RelayCommand]
    private async Task SaveAsync() {
        if (!FormData.IsValid()) {
            _dialogService.ShowWarning(
                $"Se han detectado los siguientes errores de validación:\n\n{FormData.GetValidationErrors()}",
                "Errores de validación");
            return;
        }

        if (!FormData.FechaNacimiento.IsValidBirthDate()) {
            _dialogService.ShowWarning(
                "La fecha de nacimiento debe ser entre 1900 y hoy",
                "Errores de validación");
            return;
        }

        try {
            var modelo = FormData.ToModel();

            if (!_isNew)
                modelo = modelo with {
                    CreatedAt = estudiante.CreatedAt,
                    IsDeleted = estudiante.IsDeleted,
                    DeletedAt = estudiante.DeletedAt
                };

            var result = _isNew
                ? await _personasService.SaveAsync(modelo)
                : await _personasService.UpdateAsync(modelo.Id, modelo);


            if (result.IsSuccess) {
                _logger.Information("Estudiante {Dni} guardado correctamente", modelo.Dni);
                CloseAction?.Invoke(true);
            }
            else {
                _dialogService.ShowError(result.Error.Message);
            }
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al guardar estudiante");
            _dialogService.ShowError("Error al guardar el estudiante.");
        }
    }

    /// <summary>
    ///     Cancela la edición y cierra la ventana modal.
    /// </summary>
    [RelayCommand]
    private void Cancel() {
        CloseAction?.Invoke(false);
    }

    /// <summary>
    ///     Limpia la imagen del formulario.
    /// </summary>
    [RelayCommand]
    private void LimpiarImagen() {
        FormData.Imagen = null;
        _logger.Debug("Imagen limpiada");
    }

    /// <summary>
    ///     Abre el selector de archivo, valida la imagen y actualiza el FormData.
    /// </summary>
    [RelayCommand]
    private async Task ChangeImageAsync() {
        _logger.Debug("Abriendo diálogo para seleccionar imagen");

        var dialog = new OpenFileDialog {
            Filter = "Imágenes|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
            Title = "Seleccionar imagen"
        };

        if (dialog.ShowDialog() == true) {
            _logger.Information("Usuario seleccionó imagen: {FilePath}", dialog.FileName);

            var sizeCheck = await _imageService.ValidateImageSizeAsync(dialog.FileName, 2 * 1024 * 1024);
            if (sizeCheck.IsFailure) {
                _dialogService.ShowWarning("La imagen no puede superar 2MB");
                return;
            }

            var dimensionsCheck = await _imageService.ValidateImageDimensionsAsync(dialog.FileName, 1920, 1920);
            if (dimensionsCheck.IsFailure) {
                _dialogService.ShowWarning("La imagen no puede superar 1920x1920 píxeles");
                return;
            }

            var imageResult = await _imageService.SaveImageAsync(dialog.FileName);
            if (imageResult.IsSuccess) {
                FormData.Imagen = imageResult.Value;
                _logger.Information("Imagen cambiada exitosamente: {ImagePath}", imageResult.Value);
            }
            else {
                _logger.Error("Error al guardar imagen: {Error}", imageResult.Error.Message);
                _dialogService.ShowError($"Error al guardar la imagen: {imageResult.Error.Message}");
            }
        }
        else {
            _logger.Debug("Usuario canceló selección de imagen");
        }
    }
}
