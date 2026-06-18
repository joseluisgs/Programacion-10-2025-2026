using CommunityToolkit.Mvvm.ComponentModel;

namespace GestionAcademica.ViewModels.Personas;

/// <summary>
///     Base reactiva para elementos de personas en listas.
///     Proporciona notificación de cambios para las propiedades comunes.
/// </summary>
public abstract partial class PersonaItemViewModel : ObservableObject {
    [ObservableProperty] private string _apellidos = string.Empty;

    [ObservableProperty] private string _dni = string.Empty;

    [ObservableProperty] private string _email = string.Empty;

    [ObservableProperty] private DateTime _fechaNacimiento;

    [ObservableProperty] private int _id;

    [ObservableProperty] private string? _imagen;

    [ObservableProperty] private bool _isDeleted;

    [ObservableProperty] private string _nombre = string.Empty;

    /// <summary>
    ///     Retorna el nombre completo formateado para visualización reactiva.
    /// </summary>
    public string NombreCompleto => $"{Nombre} {Apellidos}";
}
