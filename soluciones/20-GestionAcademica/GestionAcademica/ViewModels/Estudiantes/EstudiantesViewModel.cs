using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Personas;
using GestionAcademica.Services.Personas;
using GestionAcademica.Services.Images;
using GestionAcademica.Enums;
using GestionAcademica.Views.Dialog;
using GestionAcademica.Extensions;
using Serilog;

namespace GestionAcademica.ViewModels.Estudiantes;

public partial class EstudiantesViewModel : ObservableObject
{
    private readonly IPersonasService _personasService;
    private readonly IImageService _imageService;
    private readonly ILogger _logger = Log.ForContext<EstudiantesViewModel>();

    private List<Estudiante> _todosLosEstudiantes = new();

    [ObservableProperty]
    private ObservableCollection<Estudiante> _estudiantes = new();

    [ObservableProperty]
    private Estudiante? _selectedEstudiante;

    [ObservableProperty]
    private string _searchText = "";

    [ObservableProperty]
    private TipoOrdenamiento _ordenActual = TipoOrdenamiento.Dni;

    [ObservableProperty]
    private string _statusMessage = "";

    public EstudiantesViewModel(IPersonasService personasService, IImageService imageService)
    {
        _personasService = personasService;
        _imageService = imageService;
        LoadEstudiantes();
    }

    partial void OnSearchTextChanged(string value) => FilterEstudiantes();

    private void FilterEstudiantes()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            Estudiantes = new ObservableCollection<Estudiante>(_todosLosEstudiantes);
            StatusMessage = $"Total: {Estudiantes.Count} estudiantes";
            return;
        }

        var filtered = _todosLosEstudiantes.Where(e =>
            e.Nombre.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            e.Apellidos.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            e.Dni.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            e.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        Estudiantes = new ObservableCollection<Estudiante>(filtered);
        StatusMessage = $"Encontrados: {Estudiantes.Count} estudiantes";
    }

    private void LoadEstudiantes()
    {
        try
        {
            var result = _personasService.GetEstudiantesOrderBy(OrdenActual);
            _todosLosEstudiantes = result.ToList();
            Estudiantes = new ObservableCollection<Estudiante>(_todosLosEstudiantes);
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
        var newEstudiante = new Estudiante
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

        var editViewModel = new EstudianteEditViewModel(newEstudiante, _personasService, _imageService, isNew: true);
        var editWindow = new EstudianteEditWindow
        {
            DataContext = editViewModel,
            Owner = Application.Current.MainWindow
        };

        if (editWindow.ShowDialog() == true)
        {
            LoadEstudiantes();
            StatusMessage = "Estudiante creado";
        }
    }

    [RelayCommand]
    private void Edit()
    {
        if (SelectedEstudiante == null) return;

        var editEstudiante = SelectedEstudiante.Clone();

        var editViewModel = new EstudianteEditViewModel(editEstudiante, _personasService, _imageService, isNew: false);
        var editWindow = new EstudianteEditWindow
        {
            DataContext = editViewModel,
            Owner = Application.Current.MainWindow
        };

        if (editWindow.ShowDialog() == true)
        {
            LoadEstudiantes();
            StatusMessage = "Estudiante actualizado";
        }
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
    private void OrderBy(TipoOrdenamiento orden)
    {
        OrdenActual = orden;
        LoadEstudiantes();
    }
}
