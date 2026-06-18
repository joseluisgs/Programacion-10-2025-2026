// ============================================================
// BindingDemoViewModel.cs - ViewModel para demostrar bindings
// ============================================================
//
// =================================================================
// GUIA PARA EL ALUMNO: ENTENDIENDO LOS MODOS DE BINDING
// =================================================================
//
// PROBLEMA: Como sabemos, en MVVM separamos la UI (Vista) de los datos (ViewModel).
//           El binding es el "pegamento" que conecta ambos mundos.
//
// Pero no todos los bindings son iguales!
// Algunos fluyen en una dirección, otros en ambas.
// Algunos actualizan en tiempo real, otros solo una vez.
//
// Por eso existen los MODOS DE BINDING.
//
// =================================================================
// 1. QUE ES UN MODO DE BINDING?
// =================================================================
//
// Es la "dirección" del flujo de datos entre UI y ViewModel.
//
// Imaginemos una calle:
// - Unidireccional: solo puede pasar gente en un sentido
// - Bidireccional: puede pasar en ambos sentidos
//
// Con los binding pasa igual:
// - OneWay: UI <- ViewModel (solo lee)
// - TwoWay: UI <-> ViewModel (lee y escribe)
// - OneTime: UI <- ViewModel (solo una vez al inicio)
// - OneWayToSource: UI -> ViewModel (solo escribe)
//
// CADA MODO SIRVE PARA DIFERENTES SITUACIONES.
//
// =================================================================
// 2. CUALES SON LOS MODOS Y CUANDO USARLOS?
// =================================================================
//
// ONEWAY (Predeterminado para la mayoria):
//   - Flujo: ViewModel -> UI
//   - Uso: Mostrar datos que nunca cambian desde la UI
//   - Ejemplo: Titulo de ventana, nombre de usuario logueado
//
// TWOWAY:
//   - Flujo: UI <-> ViewModel
//   - Uso: Formularios donde el usuario introduce datos
//   - Ejemplo: TextBox de nombre, TextBox de email
//
// ONETIME:
//   - Flujo: ViewModel -> UI (solo una vez)
//   - Uso: Datos estaticos que no cambian nunca
//   - Ejemplo: Version de la app, fecha de compilacion
//
// ONEWAYTOSOURCE:
//   - Flujo: UI -> ViewModel
//   - Uso: Casos raros donde la UI escribe y el ViewModel solo lee
//   - Ejemplo: Controles de solo escritura
//
// =================================================================
// 3. ACTUALIZANDO LA FUENTE: UPDATESOURCETRIGGER
// =================================================================
//
// PROBLEMA: En un binding TwoWay, cuando se actualiza la UI,
//           cuando se envia el cambio al ViewModel?
//
// Por defecto (PropertyChanged): Inmediatamente al escribir cada caracter.
//
// PERO a veces queremos esperar:
// - Hasta que el usuario sale del campo (LostFocus)
// - Hasta que nosotros lo decidamos (Explicit)
//
// =================================================================
// TEORIA TERMINA - EMPEZAMOS PRACTICA
// =================================================================
//

using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfBindingsReactividad.ViewModels;

/// <summary>
/// ViewModel de demostración para todos los modos de binding.
/// </summary>
public partial class BindingDemoViewModel : ObservableObject
{
    // =================================================================
    // DEMO 1: ONEWAY - Solo lectura desde el ViewModel
    // =================================================================
    // USO: Mostrar un valor que solo cambia desde el codigo
    //
    // Ejemplo: Un titulo, un mensaje de bienvenida, un contador interno
    // Cuando cambia en el ViewModel, la UI se actualiza.
    // Pero si el usuario intenta cambiarlo en la UI, no funciona.
    //
    // EN XAML: {Binding Nombre} (es lo mismo que {Binding Nombre, Mode=OneWay})
    //
    [ObservableProperty]
    private string _tituloAplicacion = "Demo de Bindings WPF";

    // =================================================================
    // DEMO 2: TWOWAY - Edicion bidireccional
    // =================================================================
    // USO: Cuando el usuario debe poder editar un valor
    //
    // Ejemplo: Campos de formulario (nombre, email, telefono)
    // Cuando el usuario escribe, el ViewModel se actualiza.
    // Cuando el codigo cambia el valor, la UI se actualiza.
    //
    // EN XAML: {Binding Nombre, Mode=TwoWay}
    //
    [ObservableProperty]
    private string _nombreUsuario = "";

    [ObservableProperty]
    private string _emailUsuario = "";

    // =================================================================
    // DEMO 3: SLIDER BIDIRECCIONAL - Mismo valor, dos controles
    // =================================================================
    // USO: Demonstrar que TwoWay permite sincronizar controles
    //
    // Un Slider y un TextBox comparten el mismo valor.
    // Mover el Slider cambia el TextBox.
    // Escribir en el TextBox cambia el Slider.
    //
    [ObservableProperty]
    private double _valorSlider = 50;

    // =================================================================
    // DEMO 4: UPDATESOURCETRIGGER - Controlar cuando se actualiza
    // =================================================================
    //
    // PropertyChanged: Se actualiza con cada caracter (escribe "h", ya se envia)
    // LostFocus: Se actualiza cuando el usuario sale del campo
    // Explicit: Solo se actualiza cuando nosotros lo decis desde codigo
    //
    [ObservableProperty]
    private string _textoPropertyChanged = "";

    [ObservableProperty]
    private string _textoLostFocus = "";

    // =================================================================
    // DEMO 5: STRINGFORMAT - Formatear numeros y fechas
    // =================================================================
    //
    // Problema: A veces queremos mostrar el valor de forma diferente
    //           a como se almacena.
    //
    // Ejemplo:
    //   - ViewModel: double Precio = 19.99
    //   - UI queremos: "19,99 €"
    //
    // Solucion: StringFormat en el binding
    //
    [ObservableProperty]
    private double _precioProducto = 1234.56;

    [ObservableProperty]
    private DateTime _fechaActual = DateTime.Now;

    // =================================================================
    // DEMO 6: CONVERTER - Transformar tipos de datos
    // =================================================================
    //
    // Problema: La UI espera un tipo diferente al del ViewModel
    //
    // Ejemplo:
    //   - ViewModel: bool EsActivo = true
    //   - UI necesita: Visibility (Visible/Collapsed)
    //
    // Solucion: Usar un Converter que transforme bool a Visibility
    //           (Ya vimos BoolToVisibilityConverter)
    //
    // OTRO Ejemplo:
    //   - ViewModel: int Temperatura = 15
    //   - UI necesita: Un color (azul/verde/rojo)
    //
    // Solucion: NumberToColorConverter transforma 15 en un Brush
    //
    [ObservableProperty]
    private int _temperatura = 20;

    [ObservableProperty]
    private bool _esMayorDeEdad = false;

    // =================================================================
    // DEMO 7: ELEMENTNAME - Enlazar propiedades de controles entre si
    // =================================================================
    //
    // Problema: Quiero que un control dependa de otro control
    //           (no del ViewModel)
    //
    // Ejemplo:
    //   - Un Slider que cambia el ancho de un rectangulo
    //   - No necesito pasar por el ViewModel!
    //
    // Solucion: ElementName binding
    //           {Binding Value, ElementName=miSlider}
    //
    // =================================================================
    // DEMO 8: FORMULARIO REACTIVO - Resumen en tiempo real
    // =================================================================
    //
    // Problema: Tengo un formulario con muchos campos
    //           y quiero mostrar un resumen que se actualice solo
    //
    // Solucion: Propiedades calculadas en el ViewModel
    //           Que se actualizan cuando cambian los campos
    //
    [ObservableProperty]
    private string _nombre = "";

    [ObservableProperty]
    private string _apellidos = "";

    [ObservableProperty]
    private int _edad = 0;

    [ObservableProperty]
    private string _ciudad = "";

    // =================================================================
    // PROPIEDADES CALCULADAS - Se actualizan automaticamente
    // =================================================================
    //
    // Estas propiedades no se setean directamente.
    // Se calculan a partir de otras propiedades.
    // Como estan marcadas con [ObservableProperty],
    // la UI se entera cuando cambian.
    //
    // IMPORTANTE: Usamos 'partial void On[Propiedad]Changed'
    //             para recalcular cuando cambian las propiedades base.
    //

    /// <summary>
    /// Nombre completo concatenado.
    /// </summary>
    [ObservableProperty]
    private string _nombreCompleto = "";

    partial void OnNombreChanged(string value)
    {
        ActualizarNombreCompleto();
        ActualizarResumenPerfil();
    }
    
    partial void OnApellidosChanged(string value)
    {
        ActualizarNombreCompleto();
        ActualizarResumenPerfil();
    }
    
    partial void OnEdadChanged(int value) => ActualizarResumenPerfil();
    partial void OnCiudadChanged(string value) => ActualizarResumenPerfil();

    private void ActualizarNombreCompleto()
    {
        NombreCompleto = string.IsNullOrWhiteSpace(Nombre) && string.IsNullOrWhiteSpace(Apellidos)
            ? "Sin nombre"
            : $"{Nombre} {Apellidos}".Trim();
    }

    /// <summary>
    /// Resumen del perfil del usuario.
    /// </summary>
    [ObservableProperty]
    private string _resumenPerfil = "Rellena el formulario para ver el resumen";

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
    // DEMO: ONEWAYTOSOURCE - Para casos especiales
    // =================================================================
    //
    // Este modo es utiles para controles que no tienen propiedad "lectura".
    // Por ejemplo, un ScrollBar tiene Value que es de solo lectura desde fuera.
    // Con OneWayToSource, podemos obtener el valor sin afectar la UI.
    //
    // En este ejemplo simulamos un control de solo escritura.
    //
    [ObservableProperty]
    private string _valorSoloEscritura = "";

    // =================================================================
    // COMANDO: Actualizar fuente explicitamente
    // =================================================================
    //
    // Para UpdateSourceTrigger=Explicit, necesitamos un comando
    // que llame a UpdateSource() desde codigo.
    //
    // En WPF, esto se hace desde el code-behind, no desde el ViewModel.
    // El ViewModel solo necesita tener la propiedad.
    //
    [ObservableProperty]
    private string _textoExplicit = "";

    // =================================================================
    // DEMO 9: ONEWAY + EVENTOS - Enfoque manual
    // =================================================================
    //
    // Este enfoque NO usa TwoWay binding.
    // En su lugar, usamos OneWay y actualizamos desde el code-behind.
    //
    // DIFERENCIA CON TWOWAY:
    // - TwoWay: El binding envia automaticamente el valor al ViewModel
    // - OneWay + Eventos: Nosotros controllamos cuando actualizar
    //
    // PARA QUE SIRVE?
    // - Para casos donde necesitamos LOGICA EXTRA antes de actualizar
    // - Validaciones asincronas
    // - Optimizacion extrema
    //
    // VENTAJAS:
    // + Control total sobre cuando se actualiza
    // + Podemos añadir logica antes/despues de actualizar
    //
    // DESVENTAJAS:
    // - Mas codigo
    // - Rompe un poco MVVM (code-behind conoce el ViewModel)
    // - Mas dificil de mantener
    //
    [ObservableProperty]
    private string _nombreEvento = "";

    // =================================================================
    // COMANDOS
    // =================================================================

    /// <summary>
    /// Reinicia todos los valores a su estado inicial.
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
        ValorSoloEscritura = "";
        TextoExplicit = "";
        NombreEvento = "";
    }
}

// =================================================================
// RESUMEN: QUE HEMOS APRENDIDO?
// =================================================================
//
// 1. ONEWAY: UI <- ViewModel (solo lectura)
//    Ejemplo: Titulos, labels, datos que no se.editan
//
// 2. TWOWAY: UI <-> ViewModel (edicion)
//    Ejemplo: TextBox, ComboBox, CheckBox
//
// 3. ONETIME: UI <- ViewModel (una sola vez)
//    Ejemplo: Version, fecha estatica
//
// 4. ONEWAYTOSOURCE: UI -> ViewModel (solo escritura)
//    Ejemplo: Controles de solo escritura
//
// 5. UPDATESOURCETRIGGER:
//    - PropertyChanged: inmediat (escribe una letra, ya se actualiza)
//    - LostFocus: al salir del campo (escribe todo, luego se actualiza)
//    - Explicit: cuando nosotros queramos (manualmente)
//
// 6. STRINGFORMAT: Formatear valores en la UI
//    Ejemplo: "{Binding Precio, StringFormat='{}{0:C2}'}"
//
// 7. CONVERTER: Transformar tipos
//    Ejemplo: bool -> Visibility, int -> Color
//
// 8. ELEMENTNAME: Enlazar controles entre si
//    Ejemplo: Slider -> Rectangulo (sin pasar por ViewModel)
//
// =================================================================
