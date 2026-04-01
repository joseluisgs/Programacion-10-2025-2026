using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
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

namespace GestionAcademica.ViewModels.Estudiantes;

public partial class EstudiantesViewModel : ObservableObject
{
    private readonly IPersonasService _personasService;
    private readonly IImageService _imageService;
    private readonly ILogger _logger = Log.ForContext<EstudiantesViewModel>();

    [ObservableProperty]
    private ObservableCollection<Estudiante> _estudiantes = new();

    [ObservableProperty]
    private Estudiante? _selectedEstudiante;

    [ObservableProperty]
    private Estudiante _editingEstudiante = new();

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
    public IEnumerable<Curso> Cursos => Enum.GetValues<Curso>();

    public EstudiantesViewModel(IPersonasService personasService, IImageService imageService)
    {
        _personasService = personasService;
        _imageService = imageService;
        LoadEstudiantes();
    }

    private void LoadEstudiantes()
    {
        try
        {
            var result = _personasService.GetEstudiantesOrderBy(OrdenActual);
            Estudiantes = new ObservableCollection<Estudiante>(result);
            StatusMessage = $"Total: {Estudiantes.Count} estudiantes";
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al cargar estudiantes");
            StatusMessage = "Error al cargar";
        }
    }

    [RelayCommand]
    private void New()
    {
        EditingEstudiante = new Estudiante
        {
            Nombre = "",
            Apellidos = "",
            Dni = "",
            Email = "",
            FechaNacimiento = DateTime.Now.AddYears(-18),
            Ciclo = Ciclo.DAM,
            Curso = Curso.Primero,
            Calificacion = 5.0
        };
        IsNewItem = true;
        IsEditing = true;
    }

    [RelayCommand]
    private void Edit()
    {
        if (SelectedEstudiante == null) return;
        
        EditingEstudiante = new Estudiante
        {
            Id = SelectedEstudiante.Id,
            Nombre = SelectedEstudiante.Nombre,
            Apellidos = SelectedEstudiante.Apellidos,
            Dni = SelectedEstudiante.Dni,
            Email = SelectedEstudiante.Email,
            FechaNacimiento = SelectedEstudiante.FechaNacimiento,
            Ciclo = SelectedEstudiante.Ciclo,
            Curso = SelectedEstudiante.Curso,
            Calificacion = SelectedEstudiante.Calificacion,
            Imagen = SelectedEstudiante.Imagen,
            IsDeleted = SelectedEstudiante.IsDeleted
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
                result = _personasService.Save(EditingEstudiante);
            }
            else
            {
                result = _personasService.Update(EditingEstudiante.Id, EditingEstudiante);
            }

            if (result.IsSuccess)
            {
                IsEditing = false;
                LoadEstudiantes();
                StatusMessage = IsNewItem ? "Estudiante creado" : "Estudiante actualizado";
            }
            else
            {
                MessageBox.Show(result.Error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al guardar estudiante");
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
        if (SelectedEstudiante == null) return;

        var result = MessageBox.Show(
            $"¿Eliminar a {SelectedEstudiante.NombreCompleto}?",
            "Confirmar",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            var deleteResult = _personasService.Delete(SelectedEstudiante.Id);
            if (deleteResult.IsSuccess)
            {
                LoadEstudiantes();
                StatusMessage = "Estudiante eliminado";
            }
            else
            {
                MessageBox.Show(deleteResult.Error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    [RelayCommand]
    private void Load()
    {
        LoadEstudiantes();
    }

    [RelayCommand]
    private void Search()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            LoadEstudiantes();
            return;
        }

        var filtered = Estudiantes.Where(e =>
            e.Nombre.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            e.Apellidos.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            e.Dni.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            e.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        Estudiantes = new ObservableCollection<Estudiante>(filtered);
        StatusMessage = $"Encontrados: {Estudiantes.Count}";
    }

    [RelayCommand]
    private void OrderBy(TipoOrdenamiento orden)
    {
        OrdenActual = orden;
        LoadEstudiantes();
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
                EditingEstudiante = EditingEstudiante with { Imagen = imageResult.Value };
                OnPropertyChanged(nameof(EditingEstudiante));
            }
        }
    }
}