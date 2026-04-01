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
public partial class DocenteEditViewModel : ObservableObject
{
    private readonly IPersonasService _personasService;
    private readonly IImageService _imageService;
    private readonly IDialogService _dialogService;
    private readonly bool _isNew;
    private readonly ILogger _logger = Log.ForContext<DocenteEditViewModel>();

    /// <summary>FormData con validación IDataErrorInfo para el binding WPF.</summary>
    [ObservableProperty]
    private DocenteFormData _formData;

    [ObservableProperty]
    private string _windowTitle = "";

    public IEnumerable<Ciclo> Ciclos => Enum.GetValues<Ciclo>();

    public Action<bool>? CloseAction { get; set; }

    /// <summary>
    ///     Inicializa el ViewModel convirtiendo el modelo de dominio a FormData.
    /// </summary>
    /// <param name="docente">Modelo de dominio de origen (puede ser vacío para creación).</param>
    /// <param name="personasService">Servicio de persistencia de personas.</param>
    /// <param name="imageService">Servicio de gestión de imágenes.</param>
    /// <param name="dialogService">Servicio de diálogos desacoplado de WPF.</param>
    /// <param name="isNew">True si se está creando un nuevo registro; False si se edita uno existente.</param>
    public DocenteEditViewModel(Docente docente, IPersonasService personasService, IImageService imageService, IDialogService dialogService, bool isNew)
    {
        _personasService = personasService;
        _imageService = imageService;
        _dialogService = dialogService;
        _isNew = isNew;
        _formData = docente.ToFormData();
        WindowTitle = isNew ? "Nuevo Docente" : "Editar Docente";
    }

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

            try
            {
                var fileInfo = new System.IO.FileInfo(dialog.FileName);
                if (fileInfo.Length > 2 * 1024 * 1024)
                {
                    _dialogService.ShowWarning("La imagen no puede superar 2MB");
                    return;
                }

                var decoder = System.Windows.Media.Imaging.BitmapDecoder.Create(
                    new Uri(dialog.FileName, UriKind.Absolute),
                    System.Windows.Media.Imaging.BitmapCreateOptions.DelayCreation,
                    System.Windows.Media.Imaging.BitmapCacheOption.None);
                var frame = decoder.Frames[0];
                if (frame.PixelWidth > 1920 || frame.PixelHeight > 1920)
                {
                    _dialogService.ShowWarning("La imagen no puede superar 1920x1920 píxeles");
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error al validar imagen: {FilePath}", dialog.FileName);
                _dialogService.ShowError("No se pudo leer la imagen seleccionada");
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
