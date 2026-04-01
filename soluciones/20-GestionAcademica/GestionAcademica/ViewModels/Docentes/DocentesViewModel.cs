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

namespace GestionAcademica.ViewModels.Docentes;

public partial class DocentesViewModel : ObservableObject
{
    private readonly IPersonasService _personasService;
    private readonly IImageService _imageService;
    private readonly IDialogService _dialogService;
    private readonly ILogger _logger = Log.ForContext<DocentesViewModel>();

    private List<Docente> _todosLosDocentes = new();

    [ObservableProperty]
    private ObservableCollection<Docente> _docentes = new();

    [ObservableProperty]
    private Docente? _selectedDocente;

    [ObservableProperty]
    private string _searchText = "";

    [ObservableProperty]
    private TipoOrdenamiento _ordenActual = TipoOrdenamiento.Dni;

    [ObservableProperty]
    private string _statusMessage = "";

    [ObservableProperty]
    private bool _isLoading;

    public DocentesViewModel(
        IPersonasService personasService,
        IImageService imageService,
        IDialogService dialogService)
    {
        _personasService = personasService;
        _imageService = imageService;
        _dialogService = dialogService;
        LoadDocentes();
    }

    partial void OnSearchTextChanged(string value) => FilterDocentes();

    partial void OnSelectedDocenteChanged(Docente? value)
    {
        EditCommand.NotifyCanExecuteChanged();
        DeleteCommand.NotifyCanExecuteChanged();
    }

    private void FilterDocentes()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            var listaOrdenada = AplicarOrdenamiento(_todosLosDocentes, OrdenActual);
            Docentes = new ObservableCollection<Docente>(listaOrdenada);
            StatusMessage = $"Total: {Docentes.Count} docentes";
            return;
        }

        var filtered = _todosLosDocentes.Where(d =>
            d.Nombre.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            d.Apellidos.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            d.Dni.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            d.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        var listaFiltradaOrdenada = AplicarOrdenamiento(filtered, OrdenActual);
        Docentes = new ObservableCollection<Docente>(listaFiltradaOrdenada);
        StatusMessage = $"Encontrados: {Docentes.Count} docentes";
    }

    private IEnumerable<Docente> AplicarOrdenamiento(IEnumerable<Docente> lista, TipoOrdenamiento orden)
    {
        return orden switch
        {
            TipoOrdenamiento.Dni => lista.OrderBy(d => d.Dni),
            TipoOrdenamiento.Nombre => lista.OrderBy(d => d.Nombre),
            TipoOrdenamiento.Apellidos => lista.OrderBy(d => d.Apellidos),
            TipoOrdenamiento.Experiencia => lista.OrderByDescending(d => d.Experiencia),
            _ => lista.OrderBy(d => d.Id)
        };
    }

    private void LoadDocentes()
    {
        IsLoading = true;
        StatusMessage = "Cargando docentes...";

        try
        {
            var result = _personasService.GetDocentesOrderBy(OrdenActual, 1, int.MaxValue, false);
            _todosLosDocentes = result.ToList();
            FilterDocentes();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al cargar docentes");
            StatusMessage = "Error al cargar";
            _dialogService.ShowError("Error al cargar los docentes");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void New()
    {
        var newDocente = new Docente
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

        var editViewModel = new DocenteEditViewModel(newDocente, _personasService, _imageService, _dialogService, isNew: true);
        var editWindow = new DocenteEditWindow(editViewModel)
        {
            Owner = Application.Current.MainWindow
        };

        if (editWindow.ShowDialog() == true)
        {
            LoadDocentes();
            StatusMessage = "Docente creado";
            WeakReferenceMessenger.Default.Send(new PersonaCambiadaMessage());
        }
    }

    [RelayCommand(CanExecute = nameof(CanEdit))]
    private void Edit()
    {
        if (SelectedDocente == null) return;

        var editDocente = SelectedDocente.Clone();

        var editViewModel = new DocenteEditViewModel(editDocente, _personasService, _imageService, _dialogService, isNew: false);
        var editWindow = new DocenteEditWindow(editViewModel)
        {
            Owner = Application.Current.MainWindow
        };

        if (editWindow.ShowDialog() == true)
        {
            LoadDocentes();
            StatusMessage = "Docente actualizado";
            WeakReferenceMessenger.Default.Send(new PersonaCambiadaMessage());
        }
    }

    private bool CanEdit() => SelectedDocente != null;

    [RelayCommand(CanExecute = nameof(CanDelete))]
    private void Delete()
    {
        if (SelectedDocente == null) return;

        if (!_dialogService.ShowConfirmation($"¿Eliminar a {SelectedDocente.NombreCompleto}?"))
            return;

        var deleteResult = _personasService.Delete(SelectedDocente.Id);
        if (deleteResult.IsSuccess)
        {
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                _todosLosDocentes.Remove(SelectedDocente);
                Docentes.Remove(SelectedDocente);
                StatusMessage = $"Docente eliminado - Encontrados: {Docentes.Count}";
            }
            else
            {
                LoadDocentes();
                StatusMessage = "Docente eliminado";
            }
            WeakReferenceMessenger.Default.Send(new PersonaCambiadaMessage());
        }
        else
        {
            _dialogService.ShowError(deleteResult.Error.Message);
        }
    }

    private bool CanDelete() => SelectedDocente != null;

    [RelayCommand]
    private void Load()
    {
        LoadDocentes();
    }

    [RelayCommand]
    private void OrderBy(TipoOrdenamiento orden)
    {
        OrdenActual = orden;
        FilterDocentes();
    }
}
