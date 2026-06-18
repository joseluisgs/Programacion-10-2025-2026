// ============================================================
// MainViewModel.cs - ViewModel combinado para la ventana principal
// ============================================================
//
// =================================================================
// GUÍA PARA EL ALUMNO: COMBINAR VIEWMODELS
// =================================================================
//
// PROBLEMA: La ventana tiene demos de BindingDemoViewModel Y
//           demos de FormularioValidacionViewModel.
//
// SOLUCIÓN: Creamos un ViewModel que combine ambos.
//           Esto es un patrón común cuando necesitamos varias funcionalidades.
//
// OTRA OPCIÓN: Podríamos haber creado una nueva ventana separada
//              para el formulario de validación, pero aquí optamos
//              por mantener todo en una sola ventana para comparar.
//
// =================================================================

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfBindingsReactividad.FormData;

namespace WpfBindingsReactividad.ViewModels;

/// <summary>
/// ViewModel principal que combina todas las demos.
/// Expone las propiedades de BindingDemoViewModel Y de FormularioValidacionViewModel.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    // =================================================================
    // PROPIEDADES DEL BINDINGDEMOVIEWMODEL (Demos 1-10)
    // =================================================================
    
    // Demo 1: OneWay
    [ObservableProperty]
    private string _tituloAplicacion = "Demo de Bindings WPF";
    
    // Demo 2: TwoWay
    [ObservableProperty]
    private string _nombreUsuario = "";
    
    [ObservableProperty]
    private string _emailUsuario = "";
    
    // Demo 3: Slider + TextBox
    [ObservableProperty]
    private double _valorSlider = 50;
    
    // Demo 4: UpdateSourceTrigger
    [ObservableProperty]
    private string _textoPropertyChanged = "";
    
    [ObservableProperty]
    private string _textoLostFocus = "";
    
    // Demo 5: StringFormat
    [ObservableProperty]
    private double _precioProducto = 1234.56;
    
    [ObservableProperty]
    private DateTime _fechaActual = DateTime.Now;
    
    // Demo 6: Converters
    [ObservableProperty]
    private int _temperatura = 20;
    
    [ObservableProperty]
    private bool _esMayorDeEdad = false;
    
    // Demo 7: ElementName (no necesita propiedad)
    // Demo 8: Formulario Reactivo
    [ObservableProperty]
    private string _nombre = "";
    
    [ObservableProperty]
    private string _apellidos = "";
    
    [ObservableProperty]
    private int _edad = 0;
    
    [ObservableProperty]
    private string _ciudad = "";
    
    [ObservableProperty]
    private string _resumenPerfil = "Rellena el formulario para ver el resumen";
    
    // Demo 9: OneWay + Eventos
    [ObservableProperty]
    private string _nombreEvento = "";
    
    // =================================================================
    // PROPIEDADES DEL FORMULARIOVALIDACIONVIEWMODEL (Demo 11)
    // =================================================================
    
    // FORMULARIO CON DATOS DE EJEMPLO VÁLIDOS
    // DNI 12345678Z es válido según el algoritmo español
    [ObservableProperty]
    private PersonaFormData _formData = new()
    {
        Dni = "12345678Z",
        Nombre = "Juan",
        Apellidos = "García",
        Email = "juan@ejemplo.com",
        Edad = 25
    };
    
    [ObservableProperty]
    private string _statusMessage = "";

    // =================================================================
    // PATRÓN: REACTIVIDAD CON OBJETOS ANIDADOS
    // =================================================================
    // PROBLEMA: Cuando FormData tiene propiedades inválidas, el botón Guardar
    //          debe desactivarse. pero FormData es un objeto dentro de otro objeto.
    //          
    // SOLUCIÓN: Nos suscribimos a los PropertyChanged del FormData para
    //           notificar al comando cuando cualquier campo cambie.
    //
    // ¿POR QUÉ?: [RelayCommand] con CanExecute no se re-evalúa automáticamente
    //            cuando cambian propiedades de objetos anidados. Hay que notificarlo.
    
    /// <summary>
    /// Se-called cuando FormData cambia (nueva instancia).
    /// Gestionamos el alta/baja de suscripción a los eventos del objeto.
    /// </summary>
    partial void OnFormDataChanged(PersonaFormData? oldValue, PersonaFormData newValue)
    {
        // Damos de baja del objeto anterior (si existía)
        if (oldValue != null)
            oldValue.PropertyChanged -= OnFormDataPropertyChanged;
        
        // Nos inmue al nuevo objeto
        if (newValue != null)
        {
            newValue.PropertyChanged += OnFormDataPropertyChanged;
            // Forzamos re-evaluación del comando
            GuardarCommand.NotifyCanExecuteChanged();
        }
    }

    /// <summary>
    /// Se-called cuando cualquier propiedad del FormData cambia.
    /// Notifica al comando queCanExecute puede haber cambiado.
    /// </summary>
    private void OnFormDataPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        // IMPORTANTE: Sin esto, el botón no se activará/desactivará en tiempo real
        GuardarCommand.NotifyCanExecuteChanged();
    }
    
    // =================================================================
    // PROPIEDADES CALCULADAS (para Demo 8)
    // =================================================================
    
    partial void OnNombreChanged(string value)
    {
        ActualizarResumenPerfil();
    }
    
    partial void OnApellidosChanged(string value)
    {
        ActualizarResumenPerfil();
    }
    
    partial void OnEdadChanged(int value) => ActualizarResumenPerfil();
    partial void OnCiudadChanged(string value) => ActualizarResumenPerfil();
    
    private void ActualizarResumenPerfil()
    {
        var partes = new List<string>();
        
        if (!string.IsNullOrWhiteSpace(Nombre) || !string.IsNullOrWhiteSpace(Apellidos))
            partes.Add($"{Nombre} {Apellidos}".Trim());
        
        if (Edad > 0)
            partes.Add($"{Edad} años");
        
        if (!string.IsNullOrWhiteSpace(Ciudad))
            partes.Add($"de {Ciudad}");
        
        ResumenPerfil = partes.Count > 0 
            ? string.Join(", ", partes) 
            : "Rellena el formulario para ver el resumen";
    }
    
    // =================================================================
    // COMANDOS
    // =================================================================
    
    /// <summary>
    /// Reinicia todos los valores (para el botón de reinicio).
    /// </summary>
    public void Reiniciar()
    {
        NombreUsuario = "";
        EmailUsuario = "";
        ValorSlider = 50;
        TextoPropertyChanged = "";
        TextoLostFocus = "";
        PrecioProducto = 0;
        Temperatura = 20;
        EsMayorDeEdad = false;
        Nombre = "";
        Apellidos = "";
        Edad = 0;
        Ciudad = "";
        NombreEvento = "";
        
        // También reseteamos el formulario de validación
        FormData = new PersonaFormData();
        StatusMessage = "Formulario limpiado.";
    }
    
    // =================================================================
    // GUÍA DEL ALUMNO: CanExecute con objetos anidados
    // =================================================================
    //
    // Para que el botón se active/desactive en tiempo real:
    // 1. CanExecute llama a FormData.IsValid()
    // 2. Cuando FormData cambia (propiedad dentro de propiedad),
    //    debemos notificar al comando con NotifyCanExecuteChanged()
    // 3. Por eso nos inmuemos a FormData.PropertyChanged
    //
    // Sin esta notificación, CanExecute solo se evalúa al inicio
    // y el botón nunca se habilitará aunque los datos sean válidos.

    /// <summary>
    /// Determina si el formulario es válido para guardar.
    /// Se-called por [RelayCommand] para habilitar/deshabilitar el botón.
    /// </summary>
    public bool CanGuardar => FormData.IsValid();
    
    [RelayCommand(CanExecute = nameof(CanGuardar))]
    public void Guardar()
    {
        if (!FormData.IsValid())
        {
            StatusMessage = "Por favor, corrige los errores.";
            return;
        }
        
        var persona = FormData.ToModel(0);
        StatusMessage = $"Persona '{persona.NombreCompleto}' guardada correctamente.";
    }
    
    /// <summary>
    /// Comando para limpiar el formulario (Demo 11).
    /// Al crear nuevo FormData, se disparará OnFormDataChanged
    /// y se reseteará la suscripción de eventos.
    /// </summary>
    [RelayCommand]
    public void LimpiarFormulario()
    {
        FormData = new PersonaFormData();
        StatusMessage = "Formulario limpiado.";
    }
    
    /// <summary>
    /// Comando para cargar datos de ejemplo (Demo 11).
    /// </summary>
    [RelayCommand]
    public void CargarDatosEjemplo()
    {
        FormData = new PersonaFormData
        {
            Dni = "12345678Z",
            Nombre = "Juan",
            Apellidos = "García",
            Email = "juan@ejemplo.com",
            Edad = 25
        };
        StatusMessage = "Datos de ejemplo cargados.";
    }
}

// =================================================================
// NOTA: En un proyecto real, usaríamos herencia o composición.
//       Aquí lo simplificamos con un solo ViewModel para la demo.
// =================================================================