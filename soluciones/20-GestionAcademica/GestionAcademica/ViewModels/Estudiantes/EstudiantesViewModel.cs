using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GestionAcademica.Messages;
using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Personas;
using GestionAcademica.Services.Personas;
using GestionAcademica.Services.Images;
using GestionAcademica.Services.Dialogs;
using GestionAcademica.Enums;
using GestionAcademica.Views.Dialog;
using GestionAcademica.Extensions;
using Serilog;
using System.Windows;

namespace GestionAcademica.ViewModels.Estudiantes;

public partial class EstudiantesViewModel(
    IPersonasService personasService,
    IImageService imageService,
    IDialogService dialogService
) : ObservableObject
{
    private readonly IPersonasService _personasService = personasService;
    private readonly IImageService _imageService = imageService;
    private readonly IDialogService _dialogService = dialogService;
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

    [ObservableProperty]
    private bool _isLoading;

    /// <summary>
    /// Inicializa el ViewModel cargando los datos iniciales.
    /// Debe llamarse explícitamente tras la construcción del objeto.
    /// </summary>
    public void Initialize()
    {
        LoadEstudiantes();
    }

    partial void OnSearchTextChanged(string value) => FilterEstudiantes();

    partial void OnSelectedEstudianteChanged(Estudiante? value)
    {
        EditCommand.NotifyCanExecuteChanged();
        DeleteCommand.NotifyCanExecuteChanged();
    }

    private void FilterEstudiantes()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            var listaOrdenada = AplicarOrdenamiento(_todosLosEstudiantes, OrdenActual);
            Estudiantes = new ObservableCollection<Estudiante>(listaOrdenada);
            StatusMessage = $"Total: {Estudiantes.Count} estudiantes";
            return;
        }

        var filtered = _todosLosEstudiantes.Where(e =>
            e.Nombre.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            e.Apellidos.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            e.Dni.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            e.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        var listaFiltradaOrdenada = AplicarOrdenamiento(filtered, OrdenActual);
        Estudiantes = new ObservableCollection<Estudiante>(listaFiltradaOrdenada);
        StatusMessage = $"Encontrados: {Estudiantes.Count} estudiantes";
    }

    private IEnumerable<Estudiante> AplicarOrdenamiento(IEnumerable<Estudiante> lista, TipoOrdenamiento orden)
    {
        return orden switch
        {
            TipoOrdenamiento.Dni => lista.OrderBy(e => e.Dni),
            TipoOrdenamiento.Nombre => lista.OrderBy(e => e.Nombre),
            TipoOrdenamiento.Apellidos => lista.OrderBy(e => e.Apellidos),
            TipoOrdenamiento.Nota => lista.OrderByDescending(e => e.Calificacion),
            _ => lista.OrderBy(e => e.Id)
        };
    }

    private void LoadEstudiantes()
    {
        IsLoading = true;
        StatusMessage = "Cargando estudiantes...";

        try
        {
            var result = _personasService.GetEstudiantesOrderBy(OrdenActual, 1, int.MaxValue, false);
            _todosLosEstudiantes = result.ToList();
            FilterEstudiantes();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al cargar estudiantes");
            StatusMessage = "Error al cargar";
            _dialogService.ShowError("Error al cargar los estudiantes");
        }
        finally
        {
            IsLoading = false;
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

        var editViewModel = new EstudianteEditViewModel(newEstudiante, _personasService, _imageService, _dialogService, isNew: true);
        var editWindow = new EstudianteEditWindow
        {
            DataContext = editViewModel,
            Owner = Application.Current.MainWindow
        };

        if (editWindow.ShowDialog() == true)
        {
            LoadEstudiantes();
            StatusMessage = "Estudiante creado";
            WeakReferenceMessenger.Default.Send(new PersonaCambiadaMessage());
        }
    }

    [RelayCommand(CanExecute = nameof(CanEdit))]
    private void Edit()
    {
        if (SelectedEstudiante == null) return;

        var editEstudiante = SelectedEstudiante.Clone();

        var editViewModel = new EstudianteEditViewModel(editEstudiante, _personasService, _imageService, _dialogService, isNew: false);
        var editWindow = new EstudianteEditWindow
        {
            DataContext = editViewModel,
            Owner = Application.Current.MainWindow
        };

        if (editWindow.ShowDialog() == true)
        {
            LoadEstudiantes();
            StatusMessage = "Estudiante actualizado";
            WeakReferenceMessenger.Default.Send(new PersonaCambiadaMessage());
        }
    }

    private bool CanEdit() => SelectedEstudiante != null;

    [RelayCommand(CanExecute = nameof(CanDelete))]
    private void Delete()
    {
        if (SelectedEstudiante == null) return;

        if (!_dialogService.ShowConfirmation($"¿Eliminar a {SelectedEstudiante.NombreCompleto}?"))
            return;

        var deleteResult = _personasService.Delete(SelectedEstudiante.Id);
        if (deleteResult.IsSuccess)
        {
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                _todosLosEstudiantes.Remove(SelectedEstudiante);
                Estudiantes.Remove(SelectedEstudiante);
                StatusMessage = $"Estudiante eliminado - Encontrados: {Estudiantes.Count}";
            }
            else
            {
                LoadEstudiantes();
                StatusMessage = "Estudiante eliminado";
            }
            WeakReferenceMessenger.Default.Send(new PersonaCambiadaMessage());
        }
        else
        {
            _dialogService.ShowError(deleteResult.Error.Message);
        }
    }

    private bool CanDelete() => SelectedEstudiante != null;

    [RelayCommand]
    private void Load()
    {
        LoadEstudiantes();
    }

    [RelayCommand]
    private void OrderBy(TipoOrdenamiento orden)
    {
        OrdenActual = orden;
        FilterEstudiantes();
    }
}
