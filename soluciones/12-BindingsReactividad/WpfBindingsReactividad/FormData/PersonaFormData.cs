// ============================================================
// PersonaFormData.cs - FormData con validación IDataErrorInfo
// ============================================================
//
// =================================================================
// GUÍA PARA EL ALUMNO: PATRÓN FORMDATA CON IDATAERRORINFO
// =================================================================
//
// PROBLEMA: En el ejemplo anterior del formulario reactivo,
//           la validación se hacía en los comandos o manualmente.
//
// Pero hay una forma MEJOR: usar IDataErrorInfo para validación
// en tiempo real directamente en los campos.
//
// CONCEPTOS CLAVE:
// 1. FormData: Un DTO (Data Transfer Object) que contiene los datos
//    del formulario Y la lógica de validación.
// 2. IDataErrorInfo: Interfaz de WPF que permite mostrar errores
//    directamente en los controles.
// 3. ValidatesOnDataErrors: En el binding XAML, activa la validación.
//
// VENTAJAS DEL PATRÓN FORMDATA:
// + Separa la lógica de validación del ViewModel
// + Validación en tiempo real (mientras escribe)
// + Mensajes de error específicos por campo
// + El botón Guardar se puede deshabilitar automáticamente
// + Código testeable (FormData se puede testear unitariamente)
//
// =================================================================

using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfBindingsReactividad.FormData;

/// <summary>
/// FormData para validar datos de una persona.
/// Implementa IDataErrorInfo para validación en tiempo real.
/// </summary>
public partial class PersonaFormData : ObservableObject, System.ComponentModel.IDataErrorInfo
{
    // =================================================================
    // PROPIEDADES DEL FORMULARIO
    // =================================================================
    
    [ObservableProperty]
    private string _dni = "";
    
    [ObservableProperty]
    private string _nombre = "";
    
    [ObservableProperty]
    private string _apellidos = "";
    
    [ObservableProperty]
    private string _email = "";
    
    [ObservableProperty]
    private int _edad;
    
    // =================================================================
    // IMPLEMENTACIÓN DE IDATAERRORINFO
    // =================================================================
    // El indexador this[string columnName] se llama automáticamente
    // cuando el binding valida la propiedad.
    //
    // WPF llama a este método cuando:
    // - La propiedad cambia (gracias a UpdateSourceTrigger=PropertyChanged)
    // - El binding tiene ValidatesOnDataErrors=True
    //
    // IMPORTANTE: No necesitamos NotificationPropertyChanged aquí
    // porque el indexador se consulta por separado.
    //
    
    /// <summary>
    /// Indexador: devuelve el error de una propiedad específica.
    /// WPF lo llama automáticamente para cada propiedad bindeada.
    /// </summary>
    public string this[string columnName]
    {
        get
        {
            return columnName switch
            {
                // VALIDACIÓN DNI
                nameof(Dni) when string.IsNullOrWhiteSpace(Dni)
                    => "El DNI es obligatorio.",
                nameof(Dni) when !EsDniValido(Dni)
                    => "El DNI debe tener 8 dígitos y una letra válida.",
                
                // VALIDACIÓN NOMBRE
                nameof(Nombre) when string.IsNullOrWhiteSpace(Nombre)
                    => "El nombre es obligatorio.",
                nameof(Nombre) when Nombre.Length < 2
                    => "El nombre debe tener al menos 2 caracteres.",
                
                // VALIDACIÓN APELLIDOS
                nameof(Apellidos) when string.IsNullOrWhiteSpace(Apellidos)
                    => "Los apellidos son obligatorios.",
                nameof(Apellidos) when Apellidos.Length < 2
                    => "Los apellidos deben tener al menos 2 caracteres.",
                
                // VALIDACIÓN EMAIL
                nameof(Email) when string.IsNullOrWhiteSpace(Email)
                    => "El email es obligatorio.",
                nameof(Email) when !Email.Contains('@')
                    => "El email debe contener '@'.",
                nameof(Email) when !Email.Contains('.')
                    => "El email debe tener un dominio válido.",
                
                // VALIDACIÓN EDAD
                nameof(Edad) when Edad < 0
                    => "La edad no puede ser negativa.",
                nameof(Edad) when Edad > 120
                    => "La edad no puede superar 120 años.",
                
                // Sin error para otras propiedades
                _ => string.Empty
            };
        }
    }
    
    /// <summary>
    /// Error general (raramente usado en WPF).
    /// </summary>
    public string Error => string.Empty;
    
    // =================================================================
    // MÉTODO DE VALIDACIÓN GLOBAL
    // =================================================================
    
    /// <summary>
    /// Valida todo el formulario y devuelve true si es válido.
    /// </summary>
    public bool IsValid()
    {
        return string.IsNullOrEmpty(this[nameof(Dni)]) &&
               string.IsNullOrEmpty(this[nameof(Nombre)]) &&
               string.IsNullOrEmpty(this[nameof(Apellidos)]) &&
               string.IsNullOrEmpty(this[nameof(Email)]) &&
               string.IsNullOrEmpty(this[nameof(Edad)]);
    }
    
    // =================================================================
    // MÉTODOS DE MAPEO (DEL FORMULARIO AL MODELO Y VICEVERSA)
    // =================================================================
    
    /// <summary>
    /// Convierte el FormData a un modelo de dominio.
    /// </summary>
    public Models.Persona ToModel(int id = 0)
    {
        return new Models.Persona
        {
            Id = id,
            Dni = Dni.Trim(),
            Nombre = Nombre.Trim(),
            Apellidos = Apellidos.Trim(),
            Email = Email.Trim().ToLower(),
            Edad = Edad
        };
    }
    
    /// <summary>
    /// Crea un FormData desde un modelo existente (para editar).
    /// </summary>
    public static PersonaFormData FromModel(Models.Persona persona)
    {
        return new PersonaFormData
        {
            Dni = persona.Dni,
            Nombre = persona.Nombre,
            Apellidos = persona.Apellidos,
            Email = persona.Email,
            Edad = persona.Edad
        };
    }
    
    // =================================================================
    // HELPER: VALIDAR FORMATO DNI ESPAÑOL
    // =================================================================
    
    /// <summary>
    /// Valida el formato del DNI español (8 dígitos + letra).
    /// </summary>
    private static bool EsDniValido(string dni)
    {
        if (string.IsNullOrWhiteSpace(dni) || dni.Length != 9)
            return false;
        
        var numeros = dni[..8];
        var letra = dni[8];
        
        if (!int.TryParse(numeros, out _))
            return false;
        
        // Letras válidas para el DNI español
        var letrasValidas = "TRWAGMYFPDXBNJZSVQHLCKE";
        var letraCorrecta = letrasValidas[int.Parse(numeros) % 23];
        
        return char.ToUpper(letra) == letraCorrecta;
    }
}