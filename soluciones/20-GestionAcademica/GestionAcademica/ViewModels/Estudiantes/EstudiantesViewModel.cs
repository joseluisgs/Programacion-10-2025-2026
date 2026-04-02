using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GestionAcademica.Config;
using GestionAcademica.Messages;
using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Personas;
using GestionAcademica.Services.Personas;
using GestionAcademica.Services.Images;
using GestionAcademica.Services.Dialogs;
using GestionAcademica.Enums;
using GestionAcademica.Views.Estudiantes;
using GestionAcademica.Extensions;
using Serilog;
using System.Windows;

namespace GestionAcademica.ViewModels.Estudiantes;

/// <summary>
/// ViewModel para la vista de gestión de Estudiantes.
/// Maneja la visualización, filtrado, búsqueda, creación, edición y eliminación de estudiantes.
/// </summary>
public partial class EstudiantesViewModel : ObservableObject
{
    private readonly IPersonasService _personasService;
    private readonly IImageService _imageService;
    private readonly IDialogService _dialogService;
    private readonly ILogger _logger = Log.ForContext<EstudiantesViewModel>();

    private List<Estudiante> _todosLosEstudiantes = new();

    [ObservableProperty]
    private ObservableCollection<Estudiante> _estudiantes = new();

    [ObservableProperty]
    private Estudiante? _selectedEstudiante;

    [ObservableProperty]
    private string _searchText = "";

    [ObservableProperty]
    private string _cicloSeleccionado = "Todos";

    [ObservableProperty]
    private bool _mostrarEliminados;

    public bool UsaBorradoLogico => Config.AppConfig.UseLogicalDelete;

    public IEnumerable<Ciclo> Ciclos => Enum.GetValues<Ciclo>();

    public List<string> CiclosConTodos => new List<string> { "Todos" }.Concat(Ciclos.Select(c => c.ToString())).ToList();

    [ObservableProperty]
    private TipoOrdenamiento _ordenActual = TipoOrdenamiento.Dni;

    [ObservableProperty]
    private string _statusMessage = "";

    [ObservableProperty]
    private bool _isLoading;

    public EstudiantesViewModel(
        IPersonasService personasService,
        IImageService imageService,
        IDialogService dialogService)
    {
        _personasService = personasService;
        _imageService = imageService;
        _dialogService = dialogService;
        LoadEstudiantes();
    }

    partial void OnSearchTextChanged(string value) => FilterEstudiantes();

    partial void OnCicloSeleccionadoChanged(string value) => FilterEstudiantes();

    partial void OnMostrarEliminadosChanged(bool value) => LoadEstudiantes();

    partial void OnSelectedEstudianteChanged(Estudiante? value)
    {
        EditCommand.NotifyCanExecuteChanged();
        DeleteCommand.NotifyCanExecuteChanged();
        ViewCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanView))]
    private void View()
    {
        if (SelectedEstudiante == null) return;

        var detailsWindow = new EstudianteDetailsWindow
        {
            DataContext = new { Estudiante = SelectedEstudiante },
            Owner = Application.Current.MainWindow
        };
        detailsWindow.ShowDialog();
    }

    private bool CanView() => SelectedEstudiante != null;

    /// <summary>
    /// Filtra los estudiantes según el texto de búsqueda y el ciclo seleccionado.
    /// </summary>
    private void FilterEstudiantes()
    {
        var filtered = _todosLosEstudiantes.AsEnumerable();

        if (CicloSeleccionado != "Todos" && !string.IsNullOrEmpty(CicloSeleccionado))
        {
            if (Enum.TryParse<Ciclo>(CicloSeleccionado, out var cicloEnum))
            {
                filtered = filtered.Where(e => e.Ciclo == cicloEnum);
            }
        }

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            filtered = filtered.Where(e =>
                e.Nombre.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                e.Apellidos.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                e.Dni.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                e.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }

        var listaFiltradaOrdenada = AplicarOrdenamiento(filtered, OrdenActual);
        Estudiantes = new ObservableCollection<Estudiante>(listaFiltradaOrdenada);
        
        if (CicloSeleccionado != "Todos" && !string.IsNullOrEmpty(CicloSeleccionado) && !string.IsNullOrWhiteSpace(SearchText))
            StatusMessage = $"Encontrados: {Estudiantes.Count} estudiantes";
        else if (CicloSeleccionado != "Todos" && !string.IsNullOrEmpty(CicloSeleccionado))
            StatusMessage = $"Total: {Estudiantes.Count} estudiantes en {CicloSeleccionado}";
        else if (!string.IsNullOrWhiteSpace(SearchText))
            StatusMessage = $"Encontrados: {Estudiantes.Count} estudiantes";
        else
            StatusMessage = $"Total: {Estudiantes.Count} estudiantes";
    }

    /// <summary>
    /// Aplica el ordenamiento a la lista de estudiantes según el criterio especificado.
    /// </summary>
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
            var result = _personasService.GetEstudiantesOrderBy(OrdenActual, 1, int.MaxValue, MostrarEliminados);
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

        if (SelectedEstudiante.IsDeleted)
        {
            Restore();
            return;
        }

        var mensaje = AppConfig.UseLogicalDelete
            ? $"¿Eliminar a {SelectedEstudiante.NombreCompleto}? El borrado es reversible."
            : $"¿Eliminar a {SelectedEstudiante.NombreCompleto}? Este borrado es IRREVERSIBLE y se eliminarán las imágenes.";

        if (!_dialogService.ShowConfirmation(mensaje))
            return;

        var deleteResult = _personasService.Delete(SelectedEstudiante.Id, AppConfig.UseLogicalDelete);
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
        SearchText = "";
        CicloSeleccionado = "Todos";
        LoadEstudiantes();
    }

    [RelayCommand]
    private void OrderBy(TipoOrdenamiento orden)
    {
        OrdenActual = orden;
        FilterEstudiantes();
    }

    [RelayCommand(CanExecute = nameof(CanDelete))]
    private void Restore()
    {
        if (SelectedEstudiante == null) return;

        if (!_dialogService.ShowConfirmation($"¿Restaurar a {SelectedEstudiante.NombreCompleto}?"))
            return;

        var result = _personasService.Restore(SelectedEstudiante.Id);
        
        if (result.IsSuccess)
        {
            LoadEstudiantes();
            StatusMessage = "Estudiante restaurado";
            WeakReferenceMessenger.Default.Send(new PersonaCambiadaMessage());
        }
        else
        {
            _dialogService.ShowError($"Error al restaurar: {result.Error.Message}");
        }
    }
}
