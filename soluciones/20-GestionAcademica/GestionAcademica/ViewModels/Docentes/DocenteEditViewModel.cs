using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpFunctionalExtensions;
using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Personas;
using GestionAcademica.Services.Images;
using GestionAcademica.Services.Personas;
using GestionAcademica.Errors.Common;
using Serilog;

namespace GestionAcademica.ViewModels.Docentes;

public partial class DocenteEditViewModel : ObservableObject
{
    private readonly IPersonasService _personasService;
    private readonly IImageService _imageService;
    private readonly bool _isNew;
    private readonly ILogger _logger = Log.ForContext<DocenteEditViewModel>();

    [ObservableProperty]
    private Docente _docente;

    [ObservableProperty]
    private string _windowTitle = "";

    public IEnumerable<Ciclo> Ciclos => Enum.GetValues<Ciclo>();

    public Action<bool>? CloseAction { get; set; }

    public DocenteEditViewModel(Docente docente, IPersonasService personasService, IImageService imageService, bool isNew)
    {
        _docente = docente;
        _personasService = personasService;
        _imageService = imageService;
        _isNew = isNew;
        WindowTitle = isNew ? "Nuevo Docente" : "Editar Docente";
    }

    [RelayCommand]
    private void Save()
    {
        try
        {
            Result<Persona, DomainError> result;

            if (_isNew)
            {
                result = _personasService.Save(Docente);
            }
            else
            {
                result = _personasService.Update(Docente.Id, Docente);
            }

            if (result.IsSuccess)
            {
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

    [RelayCommand]
    private void Cancel()
    {
        CloseAction?.Invoke(false);
    }

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
                Docente = Docente with { Imagen = imageResult.Value };
            }
        }
    }
}
