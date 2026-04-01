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

namespace GestionAcademica.ViewModels.Estudiantes;

/// <summary>
///     ViewModel especializado para la ventana modal de creación y edición de Estudiante.
///     Trabaja con EstudianteFormData (IDataErrorInfo) y mapea al modelo de dominio para persistir.
/// </summary>
public partial class EstudianteEditViewModel : ObservableObject
{
    private readonly IPersonasService _personasService;
    private readonly IImageService _imageService;
    private readonly bool _isNew;
    private readonly ILogger _logger = Log.ForContext<EstudianteEditViewModel>();

    /// <summary>FormData con validación IDataErrorInfo para el binding WPF.</summary>
    [ObservableProperty]
    private EstudianteFormData _formData;

    [ObservableProperty]
    private string _windowTitle = "";

    public IEnumerable<Ciclo> Ciclos => Enum.GetValues<Ciclo>();
    public IEnumerable<Curso> Cursos => Enum.GetValues<Curso>();

    public Action<bool>? CloseAction { get; set; }

    /// <summary>
    ///     Inicializa el ViewModel convirtiendo el modelo de dominio a FormData.
    /// </summary>
    /// <param name="estudiante">Modelo de dominio de origen (puede ser vacío para creación).</param>
    /// <param name="personasService">Servicio de persistencia de personas.</param>
    /// <param name="imageService">Servicio de gestión de imágenes.</param>
    /// <param name="isNew">True si se está creando un nuevo registro; False si se edita uno existente.</param>
    public EstudianteEditViewModel(Estudiante estudiante, IPersonasService personasService, IImageService imageService, bool isNew)
    {
        _personasService = personasService;
        _imageService = imageService;
        _isNew = isNew;
        _formData = estudiante.ToFormData();
        WindowTitle = isNew ? "Nuevo Estudiante" : "Editar Estudiante";
    }

    /// <summary>
    ///     Guarda el estudiante si el FormData es válido, mapeando de vuelta al modelo de dominio.
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
                _logger.Information("Estudiante {Dni} guardado correctamente", modelo.Dni);
                CloseAction?.Invoke(true);
            }
            else
            {
                MessageBox.Show(result.Error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al guardar estudiante");
            MessageBox.Show("Error al guardar el estudiante.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
        _logger.Debug("Abriendo diálogo para seleccionar imagen");

        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "Imágenes|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
            Title = "Seleccionar imagen"
        };

        if (dialog.ShowDialog() == true)
        {
            _logger.Information("Usuario seleccionó imagen: {FilePath}", dialog.FileName);
            var imageResult = _imageService.SaveImage(dialog.FileName);
            if (imageResult.IsSuccess)
            {
                FormData.Imagen = imageResult.Value;
                _logger.Information("Imagen cambiada exitosamente: {ImagePath}", imageResult.Value);
            }
            else
            {
                _logger.Error("Error al guardar imagen: {Error}", imageResult.Error.Message);
                MessageBox.Show(
                    $"Error al guardar la imagen: {imageResult.Error.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        else
        {
            _logger.Debug("Usuario canceló selección de imagen");
        }
    }
}
