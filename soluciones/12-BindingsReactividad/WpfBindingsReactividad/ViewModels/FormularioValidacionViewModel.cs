// ============================================================
// FormularioValidacionViewModel.cs - ViewModel con FormData
// ============================================================
//
// =================================================================
// GUÍA PARA EL ALUMNO: CÓMO USAR FORMDATA EN EL VIEWMODEL
// =================================================================
//
// CONCEPTO: El ViewModel expone el FormData como una propiedad.
// Los comandos usan FormData.IsValid() para habilitar/deshabilitar.
// El XAML bindea directamente a FormData.Property.
//
// FLUJO:
// 1. Usuario escribe en el TextBox
// 2. Binding TwoWay actualiza FormData.Dni
// 3. IDataErrorInfo valida y devuelve error si hay
// 4. ValidatesOnDataErrors muestra el error en la UI
// 5. El botón Guardar se habilita/desabilita según IsValid()
//
// VENTAJAS:
// + ViewModel limpio (no tiene lógica de validación)
// + FormData reutilizable
// + Validación en tiempo real
// + Fácil de testear
//
// =================================================================

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfBindingsReactividad.FormData;
using WpfBindingsReactividad.Models;

namespace WpfBindingsReactividad.ViewModels;

/// <summary>
/// ViewModel que usa FormData para validación de formulario.
/// </summary>
public partial class FormularioValidacionViewModel : ObservableObject
{
    // =================================================================
    // PROPIEDAD FORMDATA (expuesta a la UI)
    // =================================================================
    // El FormData contiene:
    // - Los campos del formulario (Dni, Nombre, etc.)
    // - La lógica de validación (IDataErrorInfo)
    // - Los métodos de mapeo (ToModel, FromModel)
    
    [ObservableProperty]
    private PersonaFormData _formData = new();
    
    // =================================================================
    // OTRAS PROPIEDADES DEL VIEWMODEL
    // =================================================================
    
    /// <summary>
    /// Indica si estamos en modo edición (para mostrar título diferente).
    /// </summary>
    [ObservableProperty]
    private bool _esEdicion;
    
    /// <summary>
    /// Modelo original (para saber si editamos uno existente).
    /// </summary>
    private Persona? _personaOriginal;
    
    /// <summary>
    /// Mensaje de estado (para feedback al usuario).
    /// </summary>
    [ObservableProperty]
    private string _statusMessage = "";
    
    // =================================================================
    // CONSTRUCTOR
    // =================================================================
    
    /// <summary>
    /// Constructor por defecto: modo creación.
    /// </summary>
    public FormularioValidacionViewModel()
    {
        EsEdicion = false;
        _personaOriginal = null;
    }
    
    /// <summary>
    /// Constructor con persona: modo edición.
    /// </summary>
    public FormularioValidacionViewModel(Persona persona)
    {
        EsEdicion = true;
        _personaOriginal = persona;
        
        // Cargar los datos de la persona en el FormData
        FormData = PersonaFormData.FromModel(persona);
    }
    
    // =================================================================
    // COMANDOS
    // =================================================================
    
    /// <summary>
    /// Comando para guardar (solo se habilita si el formulario es válido).
    /// CanExecute usa FormData.IsValid() para habilitar/deshabilitar el botón.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanGuardar))]
    private void Guardar()
    {
        // doble verificación (por seguridad)
        if (!FormData.IsValid())
        {
            StatusMessage = "Por favor, corrige los errores.";
            return;
        }
        
        // Convertir FormData a Modelo
        var persona = FormData.ToModel(_personaOriginal?.Id ?? 0);
        
        if (EsEdicion)
        {
            StatusMessage = $"Persona '{persona.NombreCompleto}' actualizada correctamente.";
        }
        else
        {
            StatusMessage = $"Persona '{persona.NombreCompleto}' creada correctamente.";
        }
        
        // Aquí normalmente llamaríamos al servicio/repositorio para guardar
        // _personaService.Save(persona);
    }
    
    /// <summary>
    /// Determina si el comando Guardar puede ejecutarse.
    /// </summary>
    private bool CanGuardar()
    {
        return FormData.IsValid();
    }
    
    /// <summary>
    /// Comando para limpiar el formulario.
    /// </summary>
    [RelayCommand]
    private void Limpiar()
    {
        FormData = new PersonaFormData();
        StatusMessage = "Formulario limpiado.";
    }
    
    /// <summary>
    /// Comando para cargar datos de ejemplo (para demostrar).
    /// </summary>
    [RelayCommand]
    private void CargarDatosEjemplo()
    {
        FormData = new PersonaFormData
        {
            Dni = "12345678A",
            Nombre = "Juan",
            Apellidos = "García",
            Email = "juan@ejemplo.com",
            Edad = 25
        };
        StatusMessage = "Datos de ejemplo cargados.";
    }
}

// =================================================================
// RESUMEN: PATRÓN FORMDATA
// =================================================================
//
// 1. ViewModel tiene una propiedad FormData
// 2. XAML bindea a FormData.Campo
// 3. FormData implementa IDataErrorInfo para validación
// 4. XAML usa ValidatesOnDataErrors=True en los bindings
// 5. Botón usa CanExecute = FormData.IsValid() para habilitarse
//
// VENTAJAS VS VALIDACIÓN EN VIEWMODEL:
// - Separación de responsabilidades
// - FormData reutilizable
// - ViewModel más limpio
// - Validación testeable
//