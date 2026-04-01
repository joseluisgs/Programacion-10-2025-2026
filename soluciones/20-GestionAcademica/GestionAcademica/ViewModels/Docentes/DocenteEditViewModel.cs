using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpFunctionalExtensions;
using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Personas;
using GestionAcademica.Services.Images;
using GestionAcademica.Services.Personas;
using GestionAcademica.Services.Dialogs;
using GestionAcademica.Errors.Common;
using GestionAcademica.ViewModels.Forms;
using GestionAcademica.Mappers.Personas;
using Serilog;

namespace GestionAcademica.ViewModels.Docentes;

/// <summary>
///     ViewModel especializado para la ventana modal de creación y edición de Docente.
///     Trabaja con DocenteFormData (IDataErrorInfo) y mapea al modelo de dominio para persistir.
/// </summary>
public partial class DocenteEditViewModel(
    Docente docente,
    IPersonasService personasService,
    IImageService imageService,
    IDialogService dialogService,
    bool isNew
) : ObservableObject
{
    private readonly IPersonasService _personasService = personasService;
    private readonly IImageService _imageService = imageService;
    private readonly IDialogService _dialogService = dialogService;
    private readonly bool _isNew = isNew;
    private readonly ILogger _logger = Log.ForContext<DocenteEditViewModel>();

    /// <summary>FormData con validación IDataErrorInfo para el binding WPF.</summary>
    [ObservableProperty]
    private DocenteFormData _formData = docente.ToFormData();

    [ObservableProperty]
    private string _windowTitle = isNew ? "Nuevo Docente" : "Editar Docente";

    public IEnumerable<Ciclo> Ciclos => Enum.GetValues<Ciclo>();

    public Action<bool>? CloseAction { get; set; }

    /// <summary>
    ///     Guarda el docente si el FormData es válido, mapeando de vuelta al modelo de dominio.
    /// </summary>
    [RelayCommand]
    private void Save()
    {
        if (!FormData.IsValid())
        {
            _dialogService.ShowWarning("Por favor, corrija los errores del formulario antes de guardar.");
            return;
        }

        try
        {
            var modelo = FormData.ToModel();
            Result<Persona, DomainError> result = _isNew
                ? _personasService.Save(modelo)
                : _personasService.Update(modelo.Id, modelo);

            if (result.IsSuccess)
            {
                _logger.Information("Docente {Dni} guardado correctamente", modelo.Dni);
                CloseAction?.Invoke(true);
            }
            else
            {
                _dialogService.ShowError(result.Error.Message);
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al guardar docente");
            _dialogService.ShowError("Error al guardar el docente.");
        }
    }

    /// <summary>Cancela la edición y cierra la ventana modal.</summary>
    [RelayCommand]
    private void Cancel()
    {
        CloseAction?.Invoke(false);
    }

    /// <summary>Abre el selector de archivo, valida la imagen y actualiza el FormData.</summary>
    [RelayCommand]
    private void ChangeImage()
    {
        _logger.Debug("Abriendo diálogo para seleccionar imagen");

        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "Imágenes|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
            Title = "Seleccionar imagen"
        };

        if (dialog.ShowDialog() == true)
        {
            _logger.Information("Usuario seleccionó imagen: {FilePath}", dialog.FileName);

            if (!_imageService.ValidateImageSize(dialog.FileName, 2 * 1024 * 1024))
            {
                _dialogService.ShowWarning("La imagen no puede superar 2MB");
                return;
            }

            if (!_imageService.ValidateImageDimensions(dialog.FileName, 1920, 1920))
            {
                _dialogService.ShowWarning("La imagen no puede superar 1920x1920 píxeles");
                return;
            }

            var imageResult = _imageService.SaveImage(dialog.FileName);
            if (imageResult.IsSuccess)
            {
                FormData.Imagen = imageResult.Value;
                _logger.Information("Imagen cambiada exitosamente: {ImagePath}", imageResult.Value);
            }
            else
            {
                _logger.Error("Error al guardar imagen: {Error}", imageResult.Error.Message);
                _dialogService.ShowError($"Error al guardar la imagen: {imageResult.Error.Message}");
            }
        }
        else
        {
            _logger.Debug("Usuario canceló selección de imagen");
        }
    }
}
