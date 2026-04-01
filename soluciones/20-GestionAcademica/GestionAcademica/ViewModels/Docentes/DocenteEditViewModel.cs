using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpFunctionalExtensions;
using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Personas;
using GestionAcademica.Services.Images;
using GestionAcademica.Services.Personas;
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
    /// <param name="isNew">True si se está creando un nuevo registro; False si se edita uno existente.</param>
    public DocenteEditViewModel(Docente docente, IPersonasService personasService, IImageService imageService, bool isNew)
    {
        _personasService = personasService;
        _imageService = imageService;
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
            MessageBox.Show("Por favor, corrija los errores del formulario antes de guardar.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show(result.Error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al guardar docente");
            MessageBox.Show("Error al guardar el docente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>Cancela la edición y cierra la ventana modal.</summary>
    [RelayCommand]
    private void Cancel()
    {
        CloseAction?.Invoke(false);
    }

    /// <summary>Abre el selector de archivo y actualiza la imagen del FormData.</summary>
    [RelayCommand]
    private void ChangeImage()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "Imágenes|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
            Title = "Seleccionar imagen"
        };

        if (dialog.ShowDialog() == true)
        {
            var imageResult = _imageService.SaveImage(dialog.FileName);
            if (imageResult.IsSuccess)
            {
                FormData.Imagen = imageResult.Value;
            }
        }
    }
}
