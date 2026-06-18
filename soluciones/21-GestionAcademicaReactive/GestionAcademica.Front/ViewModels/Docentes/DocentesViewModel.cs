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
using GestionAcademica.Views.Docentes;
using Serilog;

namespace GestionAcademica.ViewModels.Docentes;

/// <summary>
///     ViewModel para la vista de gestión de Docentes.
///     Maneja la visualización, filtrado, búsqueda, creación, edición y eliminación de docentes.
/// </summary>
public partial class DocentesViewModel : ObservableObject {
    private readonly IDialogService _dialogService;
    private readonly IImageService _imageService;
    private readonly ILogger _logger = Log.ForContext<DocentesViewModel>();
    private readonly IPersonasService _personasService;

    [ObservableProperty] private string _cicloSeleccionado = "Todos";

    [ObservableProperty] private ObservableCollection<DocenteItemViewModel> _docentes = new();

    [ObservableProperty] private bool _isLoading;

    [ObservableProperty] private bool _mostrarEliminados;

    [ObservableProperty] private TipoOrdenamiento _ordenActual = TipoOrdenamiento.Dni;

    [ObservableProperty] private int _paginaActual = 1;

    [ObservableProperty] private string _searchText = "";

    [ObservableProperty] private DocenteItemViewModel? _selectedDocente;

    [ObservableProperty] private string _statusMessage = "";

    [ObservableProperty] private int _tamanoPagina = 10;

    private List<Docente> _todosLosDocentes = new();

    [ObservableProperty] private int _totalPaginas;

    [ObservableProperty] private int _totalRegistros;

    public DocentesViewModel(
        IPersonasService personasService,
        IImageService imageService,
        IDialogService dialogService) {
        _personasService = personasService;
        _imageService = imageService;
        _dialogService = dialogService;
        LoadDocentes();
    }

    public bool UsaBorradoLogico => AppConfig.UseLogicalDelete;

    public IEnumerable<Ciclo> Ciclos => Enum.GetValues<Ciclo>();

    public List<string> CiclosConTodos =>
        new List<string> { "Todos" }.Concat(Ciclos.Select(c => c.ToString())).ToList();

    public int[] TamanosPagina => [5, 10, 25, 50];

    public bool PuedeIrAPaginaAnterior => PaginaActual > 1;
    public bool PuedeIrAPaginaSiguiente => PaginaActual < TotalPaginas;

    partial void OnSearchTextChanged(string value) {
        FilterDocentes();
    }

    partial void OnCicloSeleccionadoChanged(string value) {
        FilterDocentes();
    }

    partial void OnMostrarEliminadosChanged(bool value) {
        LoadDocentes();
    }

    partial void OnPaginaActualChanged(int value) {
        FilterDocentes();
        PaginaSiguienteCommand.NotifyCanExecuteChanged();
        PaginaAnteriorCommand.NotifyCanExecuteChanged();
    }

    partial void OnTamanoPaginaChanged(int value) {
        PaginaActual = 1;
        FilterDocentes();
        PaginaSiguienteCommand.NotifyCanExecuteChanged();
        PaginaAnteriorCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedDocenteChanged(DocenteItemViewModel? value) {
        EditCommand.NotifyCanExecuteChanged();
        DeleteCommand.NotifyCanExecuteChanged();
        ViewCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanView))]
    private void View() {
        if (SelectedDocente == null) return;

        var detailsWindow = new DocenteDetailsWindow {
            DataContext = new { Docente = SelectedDocente.ToModel() },
            Owner = Application.Current.MainWindow
        };
        detailsWindow.ShowDialog();
    }

    private bool CanView() {
        return SelectedDocente != null;
    }

    /// <summary>
    ///     Filtra los docentes según el texto de búsqueda y el ciclo seleccionado.
    /// </summary>
    private void FilterDocentes() {
        var filtered = _todosLosDocentes.AsEnumerable();

        // Filtro de eliminados (si no se muestran, los quitamos)
        if (!MostrarEliminados)
            filtered = filtered.Where(d => !d.IsDeleted);

        if (CicloSeleccionado != "Todos" && !string.IsNullOrEmpty(CicloSeleccionado))
            if (Enum.TryParse<Ciclo>(CicloSeleccionado, out var cicloEnum))
                filtered = filtered.Where(d => d.Ciclo == cicloEnum);

        if (!string.IsNullOrWhiteSpace(SearchText))
            filtered = filtered.Where(d =>
                d.Nombre.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                d.Apellidos.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                d.Dni.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                d.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

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
            .Select(d => d.ToItemViewModel())
            .ToList();

        Docentes = new ObservableCollection<DocenteItemViewModel>(pagina);

        if (CicloSeleccionado != "Todos" && !string.IsNullOrEmpty(CicloSeleccionado) &&
            !string.IsNullOrWhiteSpace(SearchText))
            StatusMessage =
                $"Página {PaginaActual}/{TotalPaginas} - Mostrando {Docentes.Count} de {TotalRegistros} docentes";
        else if (CicloSeleccionado != "Todos" && !string.IsNullOrEmpty(CicloSeleccionado))
            StatusMessage =
                $"Página {PaginaActual}/{TotalPaginas} - {Docentes.Count} de {TotalRegistros} docentes en {CicloSeleccionado}";
        else if (!string.IsNullOrWhiteSpace(SearchText))
            StatusMessage =
                $"Página {PaginaActual}/{TotalPaginas} - Mostrando {Docentes.Count} de {TotalRegistros} docentes";
        else
            StatusMessage = $"Página {PaginaActual}/{TotalPaginas} - Total: {TotalRegistros} docentes";
    }

    /// <summary>
    ///     Aplica el ordenamiento a la lista de docentes según el criterio especificado.
    /// </summary>
    private IEnumerable<Docente> AplicarOrdenamiento(IEnumerable<Docente> lista, TipoOrdenamiento orden) {
        return orden switch {
            TipoOrdenamiento.Dni => lista.OrderBy(d => d.Dni),
            TipoOrdenamiento.Nombre => lista.OrderBy(d => d.Nombre),
            TipoOrdenamiento.Apellidos => lista.OrderBy(d => d.Apellidos),
            TipoOrdenamiento.Experiencia => lista.OrderByDescending(d => d.Experiencia),
            _ => lista.OrderBy(d => d.Id)
        };
    }

    private void LoadDocentes() {
        IsLoading = true;
        StatusMessage = "Cargando docentes...";

        try {
            var result = _personasService.GetDocentesOrderBy(OrdenActual, 1, int.MaxValue, MostrarEliminados);
            _todosLosDocentes = result.ToList();
            FilterDocentes();
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al cargar docentes");
            StatusMessage = "Error al cargar";
            _dialogService.ShowError("Error al cargar los docentes");
        }
        finally {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void New() {
        var newDocente = new Docente {
            Nombre = "",
            Apellidos = "",
            Dni = "",
            Email = "",
            FechaNacimiento = DateTime.Now.AddYears(-25),
            Ciclo = Ciclo.DAM,
            Especialidad = "",
            Experiencia = 0
        };

        var editViewModel = new DocenteEditViewModel(newDocente, _personasService, _imageService, _dialogService, true);
        var editWindow = new DocenteEditWindow {
            DataContext = editViewModel,
            Owner = Application.Current.MainWindow
        };

        if (editWindow.ShowDialog() == true) {
            // Actualización quirúrgica
            var creado = editViewModel.FormData.ToModel();
            _todosLosDocentes.Add(creado);
            FilterDocentes(); // Refrescamos localmente

            StatusMessage = "Docente creado";
            WeakReferenceMessenger.Default.Send(new PersonaCambiadaMessage());
        }
    }

    [RelayCommand(CanExecute = nameof(CanEdit))]
    private void Edit() {
        if (SelectedDocente == null) return;

        // Mapeamos ItemViewModel a Model para editar
        var editDocente = SelectedDocente.ToModel();

        var editViewModel =
            new DocenteEditViewModel(editDocente, _personasService, _imageService, _dialogService, false);
        var editWindow = new DocenteEditWindow {
            DataContext = editViewModel,
            Owner = Application.Current.MainWindow
        };

        if (editWindow.ShowDialog() == true) {
            // ACTUALIZACIÓN QUIRÚRGICA:
            // 1. Sincronizamos el ItemViewModel reactivo
            SelectedDocente.UpdateFromFormData(editViewModel.FormData);

            // 2. Sincronizamos la lista maestra (cache)
            var index = _todosLosDocentes.FindIndex(d => d.Id == SelectedDocente.Id);
            if (index != -1) _todosLosDocentes[index] = editViewModel.FormData.ToModel();

            StatusMessage = "Docente actualizado";
            WeakReferenceMessenger.Default.Send(new PersonaCambiadaMessage());
        }
    }

    private bool CanEdit() {
        return SelectedDocente != null;
    }

    [RelayCommand(CanExecute = nameof(CanDelete))]
    private void Delete() {
        if (SelectedDocente == null) return;

        if (SelectedDocente.IsDeleted) {
            Restore();
            return;
        }

        var mensaje = AppConfig.UseLogicalDelete
            ? $"¿Eliminar a {SelectedDocente.NombreCompleto}? El borrado es reversible."
            : $"¿Eliminar a {SelectedDocente.NombreCompleto}? Este borrado es IRREVERSIBLE y se eliminarán las imágenes.";

        if (!_dialogService.ShowConfirmation(mensaje))
            return;

        var deleteResult = _personasService.Delete(SelectedDocente.Id, AppConfig.UseLogicalDelete);
        if (deleteResult.IsSuccess) {
            // ACTUALIZACIÓN QUIRÚRGICA
            if (AppConfig.UseLogicalDelete) {
                // Borrado lógico: Actualizamos estado reactivo
                SelectedDocente.IsDeleted = true;
                var index = _todosLosDocentes.FindIndex(d => d.Id == SelectedDocente.Id);
                if (index != -1) _todosLosDocentes[index] = _todosLosDocentes[index] with { IsDeleted = true };

                // Si no estamos mostrando eliminados, lo quitamos de la vista actual
                if (!MostrarEliminados) Docentes.Remove(SelectedDocente);
            }
            else {
                // Borrado físico: Eliminamos de todas las listas
                _todosLosDocentes.RemoveAll(d => d.Id == SelectedDocente.Id);
                Docentes.Remove(SelectedDocente);
            }

            StatusMessage = "Docente eliminado";
            WeakReferenceMessenger.Default.Send(new PersonaCambiadaMessage());
        }
        else {
            _dialogService.ShowError(deleteResult.Error.Message);
        }
    }

    private bool CanDelete() {
        return SelectedDocente != null;
    }

    [RelayCommand(CanExecute = nameof(CanDelete))]
    private void Restore() {
        if (SelectedDocente == null) return;

        if (!_dialogService.ShowConfirmation($"¿Restaurar a {SelectedDocente.NombreCompleto}?"))
            return;

        var result = _personasService.Restore(SelectedDocente.Id);

        if (result.IsSuccess) {
            // ACTUALIZACIÓN QUIRÚRGICA:
            // 1. Sincronizamos el ItemViewModel reactivo
            SelectedDocente.IsDeleted = false;

            // 2. Sincronizamos la lista maestra (cache)
            var index = _todosLosDocentes.FindIndex(d => d.Id == SelectedDocente.Id);
            if (index != -1) _todosLosDocentes[index] = _todosLosDocentes[index] with { IsDeleted = false };

            // Al restaurar, el elemento SIEMPRE debe ser visible (si cumple filtros de búsqueda/ciclo)
            // Por lo que no hacemos Remove. El estilo XAML se encargará de quitar la opacidad/itálica.

            SelectedDocente = null;
            StatusMessage = "Docente restaurado";
            WeakReferenceMessenger.Default.Send(new PersonaCambiadaMessage());
        }
        else {
            _dialogService.ShowError($"Error al restaurar: {result.Error.Message}");
        }
    }

    [RelayCommand]
    private void Load() {
        SearchText = "";
        CicloSeleccionado = "Todos";
        MostrarEliminados = false;
        LoadDocentes();
    }

    [RelayCommand]
    private void OrderBy(TipoOrdenamiento orden) {
        OrdenActual = orden;
        FilterDocentes();
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