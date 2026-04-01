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

namespace GestionAcademica.ViewModels.Docentes;

public partial class DocentesViewModel : ObservableObject
{
    private readonly IPersonasService _personasService;
    private readonly IImageService _imageService;
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

    public DocentesViewModel(IPersonasService personasService, IImageService imageService)
    {
        _personasService = personasService;
        _imageService = imageService;
        LoadDocentes();
    }

    partial void OnSearchTextChanged(string value) => FilterDocentes();

    private void FilterDocentes()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            Docentes = new ObservableCollection<Docente>(_todosLosDocentes);
            StatusMessage = $"Total: {Docentes.Count} docentes";
            return;
        }

        var filtered = _todosLosDocentes.Where(d =>
            d.Nombre.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            d.Apellidos.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            d.Dni.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            d.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        Docentes = new ObservableCollection<Docente>(filtered);
        StatusMessage = $"Encontrados: {Docentes.Count} docentes";
    }

    private void LoadDocentes()
    {
        try
        {
            var result = _personasService.GetDocentesOrderBy(OrdenActual);
            _todosLosDocentes = result.ToList();
            Docentes = new ObservableCollection<Docente>(_todosLosDocentes);
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

        var editViewModel = new DocenteEditViewModel(newDocente, _personasService, _imageService, isNew: true);
        var editWindow = new DocenteEditWindow
        {
            DataContext = editViewModel,
            Owner = Application.Current.MainWindow
        };

        if (editWindow.ShowDialog() == true)
        {
            LoadDocentes();
            StatusMessage = "Docente creado";
        }
    }

    [RelayCommand]
    private void Edit()
    {
        if (SelectedDocente == null) return;

        var editDocente = SelectedDocente.Clone();

        var editViewModel = new DocenteEditViewModel(editDocente, _personasService, _imageService, isNew: false);
        var editWindow = new DocenteEditWindow
        {
            DataContext = editViewModel,
            Owner = Application.Current.MainWindow
        };

        if (editWindow.ShowDialog() == true)
        {
            LoadDocentes();
            StatusMessage = "Docente actualizado";
        }
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
    private void Load()
    {
        LoadDocentes();
    }

}
