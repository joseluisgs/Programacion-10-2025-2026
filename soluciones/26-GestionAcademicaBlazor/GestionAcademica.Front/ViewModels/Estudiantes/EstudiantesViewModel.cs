using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GestionAcademica.Config;
using GestionAcademica.Enums;
using GestionAcademica.Extensions;
using GestionAcademica.Mappers.Personas;
using GestionAcademica.Messages;
using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Personas;
using GestionAcademica.Services.Dialogs;
using GestionAcademica.Services.Images;
using GestionAcademica.Services.Personas;
using GestionAcademica.Views.Estudiantes;
using Serilog;

namespace GestionAcademica.ViewModels.Estudiantes;

/// <summary>
///     ViewModel para la vista de gestión de Estudiantes.
///     Maneja la visualización, filtrado, búsqueda, creación, edición y eliminación de estudiantes.
/// </summary>
public partial class EstudiantesViewModel : ObservableObject {
    private readonly IDialogService _dialogService;
    private readonly IImageService _imageService;
    private readonly ILogger _logger = Log.ForContext<EstudiantesViewModel>();
    private readonly IPersonasService _personasService;

    [ObservableProperty] private string _cicloSeleccionado = "Todos";

    [ObservableProperty] private ObservableCollection<EstudianteItemViewModel> _estudiantes = [];

    [ObservableProperty] private bool _isLoading;

    [ObservableProperty] private bool _mostrarEliminados;

    [ObservableProperty] private TipoOrdenamiento _ordenActual = TipoOrdenamiento.Dni;

    [ObservableProperty] private int _paginaActual = 1;

    [ObservableProperty] private string _searchText = "";

    [ObservableProperty] private EstudianteItemViewModel? _selectedEstudiante;

    [ObservableProperty] private string _statusMessage = "";

    [ObservableProperty] private int _tamanoPagina = 10;

    private List<Estudiante> _todosLosEstudiantes = new();

    [ObservableProperty] private int _totalPaginas;

    [ObservableProperty] private int _totalRegistros;

    public EstudiantesViewModel(
        IPersonasService personasService,
        IImageService imageService,
        IDialogService dialogService) {
        _personasService = personasService;
        _imageService = imageService;
        _dialogService = dialogService;
        _ = LoadEstudiantesAsync();
    }

    public bool UsaBorradoLogico => AppConfig.UseLogicalDelete;

    public IEnumerable<Ciclo> Ciclos => Enum.GetValues<Ciclo>();

    public List<string> CiclosConTodos =>
        new List<string> { "Todos" }.Concat(Ciclos.Select(c => c.ToString())).ToList();

    public int[] TamanosPagina => [5, 10, 25, 50];

    public bool PuedeIrAPaginaAnterior => PaginaActual > 1;
    public bool PuedeIrAPaginaSiguiente => PaginaActual < TotalPaginas;

    partial void OnSearchTextChanged(string value) {
        FilterEstudiantes();
    }

    partial void OnCicloSeleccionadoChanged(string value) {
        FilterEstudiantes();
    }

    partial void OnMostrarEliminadosChanged(bool value) {
        _ = LoadEstudiantesAsync();
    }

    partial void OnPaginaActualChanged(int value) {
        FilterEstudiantes();
        PaginaSiguienteCommand.NotifyCanExecuteChanged();
        PaginaAnteriorCommand.NotifyCanExecuteChanged();
    }

    partial void OnTamanoPaginaChanged(int value) {
        PaginaActual = 1;
        FilterEstudiantes();
        PaginaSiguienteCommand.NotifyCanExecuteChanged();
        PaginaAnteriorCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedEstudianteChanged(EstudianteItemViewModel? value) {
        EditCommand.NotifyCanExecuteChanged();
        DeleteCommand.NotifyCanExecuteChanged();
        ViewCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanView))]
    private void View() {
        if (SelectedEstudiante == null) return;

        var detailsWindow = new EstudianteDetailsWindow {
            DataContext = new { Estudiante = SelectedEstudiante.ToModel() },
            Owner = Application.Current.MainWindow
        };
        detailsWindow.ShowDialog();
    }

    private bool CanView() {
        return SelectedEstudiante != null;
    }

    /// <summary>
    ///     Filtra los estudiantes según el texto de búsqueda y el ciclo seleccionado.
    /// </summary>
    private void FilterEstudiantes() {
        var filtered = _todosLosEstudiantes.AsEnumerable();

        // Filtro de eliminados (si no se muestran, los quitamos)
        if (!MostrarEliminados)
            filtered = filtered.Where(e => !e.IsDeleted);

        if (CicloSeleccionado != "Todos" && !string.IsNullOrEmpty(CicloSeleccionado))
            if (Enum.TryParse<Ciclo>(CicloSeleccionado, out var cicloEnum))
                filtered = filtered.Where(e => e.Ciclo == cicloEnum);

        if (!string.IsNullOrWhiteSpace(SearchText))
            filtered = filtered.Where(e =>
                e.Nombre.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                e.Apellidos.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                e.Dni.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                e.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        var listaFiltradaOrdenada = AplicarOrdenamiento(filtered, OrdenActual).ToList();

        TotalRegistros = listaFiltradaOrdenada.Count;
        TotalPaginas = TotalRegistros == 0 ? 1 : (int)Math.Ceiling((double)TotalRegistros / TamanoPagina);

        if (PaginaActual > TotalPaginas)
            PaginaActual = TotalPaginas;
        if (PaginaActual < 1)
            PaginaActual = 1;

        var pagina = listaFiltradaOrdenada
            .Skip((PaginaActual - 1) * TamanoPagina)
            .Take(TamanoPagina)
            .Select(e => e.ToItemViewModel())
            .ToList();

        Estudiantes = new ObservableCollection<EstudianteItemViewModel>(pagina);

        if (CicloSeleccionado != "Todos" && !string.IsNullOrEmpty(CicloSeleccionado) &&
            !string.IsNullOrWhiteSpace(SearchText))
            StatusMessage =
                $"Página {PaginaActual}/{TotalPaginas} - Mostrando {Estudiantes.Count} de {TotalRegistros} estudiantes";
        else if (CicloSeleccionado != "Todos" && !string.IsNullOrEmpty(CicloSeleccionado))
            StatusMessage =
                $"Página {PaginaActual}/{TotalPaginas} - {Estudiantes.Count} de {TotalRegistros} estudiantes en {CicloSeleccionado}";
        else if (!string.IsNullOrWhiteSpace(SearchText))
            StatusMessage =
                $"Página {PaginaActual}/{TotalPaginas} - Mostrando {Estudiantes.Count} de {TotalRegistros} estudiantes";
        else
            StatusMessage = $"Página {PaginaActual}/{TotalPaginas} - Total: {TotalRegistros} estudiantes";
    }

    /// <summary>
    ///     Aplica el ordenamiento a la lista de estudiantes según el criterio especificado.
    /// </summary>
    private IEnumerable<Estudiante> AplicarOrdenamiento(IEnumerable<Estudiante> lista, TipoOrdenamiento orden) {
        return orden switch {
            TipoOrdenamiento.Dni => lista.OrderBy(e => e.Dni),
            TipoOrdenamiento.Nombre => lista.OrderBy(e => e.Nombre),
            TipoOrdenamiento.Apellidos => lista.OrderBy(e => e.Apellidos),
            TipoOrdenamiento.Nota => lista.OrderByDescending(e => e.Calificacion),
            _ => lista.OrderBy(e => e.Id)
        };
    }

    private async Task LoadEstudiantesAsync() {
        IsLoading = true;
        StatusMessage = "Cargando estudiantes...";

        try {
            var result = await _personasService.GetEstudiantesOrderByAsync(OrdenActual, 1, int.MaxValue, MostrarEliminados);
            _todosLosEstudiantes = result.ToList();
            FilterEstudiantes();
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al cargar estudiantes");
            StatusMessage = "Error al cargar";
            _dialogService.ShowError("Error al cargar los estudiantes");
        }
        finally {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void New() {
        var newEstudiante = new Estudiante {
            Nombre = "",
            Apellidos = "",
            Dni = "",
            Email = "",
            FechaNacimiento = DateTime.Now.AddYears(-18),
            Ciclo = Ciclo.DAM,
            Curso = Curso.Primero,
            Calificacion = 5.0
        };

        var editViewModel =
            new EstudianteEditViewModel(newEstudiante, _personasService, _imageService, _dialogService, true);
        var editWindow = new EstudianteEditWindow {
            DataContext = editViewModel,
            Owner = Application.Current.MainWindow
        };

        if (editWindow.ShowDialog() == true) {
            // Actualización quirúrgica
            var creado = editViewModel.FormData.ToModel();
            _todosLosEstudiantes.Add(creado);
            FilterEstudiantes(); // Refrescamos para mantener orden y paginación

            StatusMessage = "Estudiante creado";
            WeakReferenceMessenger.Default.Send(new PersonaCambiadaMessage());
        }
    }

    [RelayCommand(CanExecute = nameof(CanEdit))]
    private void Edit() {
        if (SelectedEstudiante == null) return;

        // Mapeamos el ItemViewModel de vuelta a Model para editar
        var editEstudiante = SelectedEstudiante.ToModel();

        var editViewModel =
            new EstudianteEditViewModel(editEstudiante, _personasService, _imageService, _dialogService, false);
        var editWindow = new EstudianteEditWindow {
            DataContext = editViewModel,
            Owner = Application.Current.MainWindow
        };

        if (editWindow.ShowDialog() == true) {
            // ACTUALIZACIÓN QUIRÚRGICA: 
            // 1. Sincronizamos el ItemViewModel que ya está en la ObservableCollection
            SelectedEstudiante.UpdateFromFormData(editViewModel.FormData);

            // 2. Sincronizamos la lista maestra (cache en memoria)
            var index = _todosLosEstudiantes.FindIndex(e => e.Id == SelectedEstudiante.Id);
            if (index != -1) _todosLosEstudiantes[index] = editViewModel.FormData.ToModel();

            StatusMessage = "Estudiante actualizado";
            WeakReferenceMessenger.Default.Send(new PersonaCambiadaMessage());
        }
    }

    private bool CanEdit() {
        return SelectedEstudiante != null;
    }

    [RelayCommand(CanExecute = nameof(CanDelete))]
    private async Task DeleteAsync() {
        if (SelectedEstudiante == null) return;

        if (SelectedEstudiante.IsDeleted) {
            await RestoreAsync();
            return;
        }

        var mensaje = AppConfig.UseLogicalDelete
            ? $"¿Eliminar a {SelectedEstudiante.NombreCompleto}? El borrado es reversible."
            : $"¿Eliminar a {SelectedEstudiante.NombreCompleto}? Este borrado es IRREVERSIBLE y se eliminarán las imágenes.";

        if (!await _dialogService.ShowConfirmationAsync(mensaje))
            return;

        var deleteResult = await _personasService.DeleteAsync(SelectedEstudiante.Id, AppConfig.UseLogicalDelete);
        if (deleteResult.IsSuccess) {
            // ACTUALIZACIÓN QUIRÚRGICA
            if (AppConfig.UseLogicalDelete) {
                // Borrado lógico: Actualizamos estado reactivo
                SelectedEstudiante.IsDeleted = true;
                var index = _todosLosEstudiantes.FindIndex(e => e.Id == SelectedEstudiante.Id);
                if (index != -1) _todosLosEstudiantes[index] = _todosLosEstudiantes[index] with { IsDeleted = true };

                // Si no estamos mostrando eliminados, lo quitamos de la vista actual
                if (!MostrarEliminados) Estudiantes.Remove(SelectedEstudiante);
            }
            else {
                // Borrado físico: Eliminamos de todas las listas
                _todosLosEstudiantes.RemoveAll(e => e.Id == SelectedEstudiante.Id);
                Estudiantes.Remove(SelectedEstudiante);
            }

            StatusMessage = "Estudiante eliminado";
            WeakReferenceMessenger.Default.Send(new PersonaCambiadaMessage());
        }
        else {
            _dialogService.ShowError(deleteResult.Error.Message);
        }
    }

    private bool CanDelete() {
        return SelectedEstudiante != null;
    }

    [RelayCommand]
    private async Task LoadAsync() {
        SearchText = "";
        CicloSeleccionado = "Todos";
        await LoadEstudiantesAsync();
    }

    [RelayCommand]
    private void OrderBy(TipoOrdenamiento orden) {
        OrdenActual = orden;
        FilterEstudiantes();
    }

    [RelayCommand(CanExecute = nameof(CanDelete))]
    private async Task RestoreAsync() {
        if (SelectedEstudiante == null) return;

        if (!await _dialogService.ShowConfirmationAsync($"¿Restaurar a {SelectedEstudiante.NombreCompleto}?"))
            return;

        var result = await _personasService.RestoreAsync(SelectedEstudiante.Id);

        if (result.IsSuccess) {
            // ACTUALIZACIÓN QUIRÚRGICA:
            // 1. Sincronizamos el ItemViewModel reactivo
            SelectedEstudiante.IsDeleted = false;

            // 2. Sincronizamos la lista maestra (cache)
            var index = _todosLosEstudiantes.FindIndex(e => e.Id == SelectedEstudiante.Id);
            if (index != -1) _todosLosEstudiantes[index] = _todosLosEstudiantes[index] with { IsDeleted = false };

            // Al restaurar, el elemento SIEMPRE debe ser visible (si cumple filtros de búsqueda/ciclo)
            // Por lo que no hacemos Remove. El estilo XAML se encargará de quitar la opacidad/itálica.

            SelectedEstudiante = null;
            StatusMessage = "Estudiante restaurado";
            WeakReferenceMessenger.Default.Send(new PersonaCambiadaMessage());
        }
        else {
            _dialogService.ShowError($"Error al restaurar: {result.Error.Message}");
        }
    }

    [RelayCommand(CanExecute = nameof(PuedeIrAPaginaAnterior))]
    private void PaginaAnterior() {
        if (PaginaActual > 1)
            PaginaActual--;
    }

    [RelayCommand(CanExecute = nameof(PuedeIrAPaginaSiguiente))]
    private void PaginaSiguiente() {
        if (PaginaActual < TotalPaginas)
            PaginaActual++;
    }

    [RelayCommand]
    private void PrimeraPagina() {
        PaginaActual = 1;
    }

    [RelayCommand]
    private void UltimaPagina() {
        PaginaActual = TotalPaginas;
    }

    [RelayCommand]
    private void CambiarTamanoPagina(int tamano) {
        TamanoPagina = tamano;
    }
}
