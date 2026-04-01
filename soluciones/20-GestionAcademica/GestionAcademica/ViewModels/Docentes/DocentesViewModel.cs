using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpFunctionalExtensions;
using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Personas;
using GestionAcademica.Services.Personas;
using GestionAcademica.Services.Images;
using GestionAcademica.Enums;
using GestionAcademica.Errors.Common;
using Serilog;

namespace GestionAcademica.ViewModels.Docentes;

public partial class DocentesViewModel : ObservableObject
{
    private readonly IPersonasService _personasService;
    private readonly IImageService _imageService;
    private readonly ILogger _logger = Log.ForContext<DocentesViewModel>();

    [ObservableProperty]
    private ObservableCollection<Docente> _docentes = new();

    [ObservableProperty]
    private Docente? _selectedDocente;

    [ObservableProperty]
    private Docente _editingDocente = new();

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private bool _isNewItem;

    [ObservableProperty]
    private string _searchText = "";

    [ObservableProperty]
    private TipoOrdenamiento _ordenActual = TipoOrdenamiento.Dni;

    [ObservableProperty]
    private string _statusMessage = "";

    public IEnumerable<Ciclo> Ciclos => Enum.GetValues<Ciclo>();
    public IEnumerable<string> Especialidades => new List<string> { "Informática", "Sistemas", "Programación", "Base de Datos", "Redes" };

    public DocentesViewModel(IPersonasService personasService, IImageService imageService)
    {
        _personasService = personasService;
        _imageService = imageService;
        LoadDocentes();
    }

    private void LoadDocentes()
    {
        try
        {
            var result = _personasService.GetDocentesOrderBy(OrdenActual);
            Docentes = new ObservableCollection<Docente>(result);
            StatusMessage = $"Total: {Docentes.Count} docentes";
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al cargar docentes");
            StatusMessage = "Error al cargar";
        }
    }

    [RelayCommand]
    private void New()
    {
        EditingDocente = new Docente
        {
            Nombre = "",
            Apellidos = "",
            Dni = "",
            Email = "",
            FechaNacimiento = DateTime.Now.AddYears(-25),
            Ciclo = Ciclo.DAM,
            Especialidad = "",
            Experiencia = 0
        };
        IsNewItem = true;
        IsEditing = true;
    }

    [RelayCommand]
    private void Edit()
    {
        if (SelectedDocente == null) return;

        EditingDocente = new Docente
        {
            Id = SelectedDocente.Id,
            Nombre = SelectedDocente.Nombre,
            Apellidos = SelectedDocente.Apellidos,
            Dni = SelectedDocente.Dni,
            Email = SelectedDocente.Email,
            FechaNacimiento = SelectedDocente.FechaNacimiento,
            Ciclo = SelectedDocente.Ciclo,
            Especialidad = SelectedDocente.Especialidad,
            Experiencia = SelectedDocente.Experiencia,
            Imagen = SelectedDocente.Imagen,
            IsDeleted = SelectedDocente.IsDeleted
        };
        IsNewItem = false;
        IsEditing = true;
    }

    [RelayCommand]
    private void Save()
    {
        try
        {
            Result<Persona, DomainError> result;

            if (IsNewItem)
            {
                result = _personasService.Save(EditingDocente);
            }
            else
            {
                result = _personasService.Update(EditingDocente.Id, EditingDocente);
            }

            if (result.IsSuccess)
            {
                IsEditing = false;
                LoadDocentes();
                StatusMessage = IsNewItem ? "Docente creado" : "Docente actualizado";
            }
            else
            {
                MessageBox.Show(result.Error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al guardar docente");
            StatusMessage = "Error al guardar";
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        IsEditing = false;
    }

    [RelayCommand]
    private void Delete()
    {
        if (SelectedDocente == null) return;

        var result = MessageBox.Show(
            $"¿Eliminar a {SelectedDocente.NombreCompleto}?",
            "Confirmar",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            var deleteResult = _personasService.Delete(SelectedDocente.Id);
            if (deleteResult.IsSuccess)
            {
                LoadDocentes();
                StatusMessage = "Docente eliminado";
            }
            else
            {
                MessageBox.Show(deleteResult.Error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    [RelayCommand]
    private void Search()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            LoadDocentes();
            return;
        }

        var filtered = Docentes.Where(d =>
            d.Nombre.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            d.Apellidos.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            d.Dni.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            d.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        Docentes = new ObservableCollection<Docente>(filtered);
        StatusMessage = $"Encontrados: {Docentes.Count}";
    }

    [RelayCommand]
    private void SelectImage()
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
                EditingDocente = EditingDocente with { Imagen = imageResult.Value };
                OnPropertyChanged(nameof(EditingDocente));
            }
        }
    }
}