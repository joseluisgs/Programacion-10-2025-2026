using CommunityToolkit.Mvvm.ComponentModel;
using GestionAcademica.Models.Academia;
using GestionAcademica.ViewModels.Personas;

namespace GestionAcademica.ViewModels.Estudiantes;

/// <summary>
///     ViewModel reactivo para un estudiante en una lista.
///     Permite actualizaciones granulares sin refrescar la colección completa.
/// </summary>
public partial class EstudianteItemViewModel : PersonaItemViewModel {
    [ObservableProperty] private double _calificacion;

    [ObservableProperty] private Ciclo _ciclo;

    [ObservableProperty] private Curso _curso;
}
