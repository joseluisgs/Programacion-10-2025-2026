using CommunityToolkit.Mvvm.ComponentModel;
using GestionAcademica.Models.Academia;
using GestionAcademica.ViewModels.Personas;

namespace GestionAcademica.ViewModels.Docentes;

/// <summary>
///     ViewModel reactivo para un docente en una lista.
///     Permite actualizaciones granulares sin refrescar la colección completa.
/// </summary>
public partial class DocenteItemViewModel : PersonaItemViewModel {
    [ObservableProperty] private Ciclo _ciclo;

    [ObservableProperty] private string _especialidad = string.Empty;

    [ObservableProperty] private int _experiencia;
}
