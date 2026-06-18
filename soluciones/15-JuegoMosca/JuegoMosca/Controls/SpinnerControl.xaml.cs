// =============================================================================
// SPINNER CONTROL - COMPONENTE PERSONALIZADO
// =============================================================================
// Este control permite al usuario seleccionar un valor numérico entre un mínimo
// y un máximo. Es como un "InputSpinner" o "NumberPicker".
//
// Características:
// - TextBox para escribir el valor directamente
// - Botones ▲/▼ para incrementar/decrementar
// - Validación de entrada (solo números)
// - Binding bidireccional con el ViewModel
// =============================================================================

using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace JuegoMosca.Controls;

/// <summary>
/// Control personalizado que permite seleccionar valores numéricos.
/// </summary>
public partial class SpinnerControl : UserControl
{
    // =============================================================================
    // ATRIBUTOS PRIVADOS
    // =============================================================================
    
    /// <summary>
    /// Flag para evitar bucles infinitos de actualización.
    /// Cuando estamos actualizando el TextBox desde código, no queremos
    /// que el cambio dispare nuevamente la actualización.
    /// </summary>
    private bool _isUpdating;

    // =============================================================================
    // PROPIEDADES DE DEPENDENCY (PROPIEDADES ENLAZABLES)
    // =============================================================================
    // Las DependencyProperty permiten que el control sea usado en bindings XAML.
    // Son el equivalente a las propiedades @ObservableProperty pero a nivel de control.
    
    /// <summary>
    /// Propiedad que almacena el valor actual del spinner.
    /// BindsTwoWayByDefault significa que se sincroniza en ambos sentidos:
    /// - ViewModel → Spinner (cuando el ViewModel cambia)
    /// - Spinner → ViewModel (cuando el usuario cambia el valor)
    /// </summary>
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(int), typeof(SpinnerControl),
            new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

    /// <summary>
    /// Valor mínimo permitidO.
    /// </summary>
    public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register(nameof(Minimum), typeof(int), typeof(SpinnerControl),
            new FrameworkPropertyMetadata(1, OnMinChanged));

    /// <summary>
    /// Valor máximo permitido.
    /// </summary>
    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register(nameof(Maximum), typeof(int), typeof(SpinnerControl),
            new FrameworkPropertyMetadata(10, OnMaxChanged));

    // =============================================================================
    // PROPIEDADES PÚBLICAS ( wrappers de las DependencyProperty)
    // =============================================================================
    // Son las propiedades que usamos en XAML: Value="{Binding ...}"
    
    /// <summary>
    /// Valor actual del spinner. Se enlaza con el ViewModel.
    /// </summary>
    public int Value
    {
        get => (int)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Límite inferior del valor.
    /// </summary>
    public int Minimum
    {
        get => (int)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    /// <summary>
    /// Límite superior del valor.
    /// </summary>
    public int Maximum
    {
        get => (int)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    // =============================================================================
    // CONSTRUCTOR
    // =============================================================================
    
    /// <summary>
    /// Constructor del control. Inicializa el componente y configura el binding.
    /// </summary>
    public SpinnerControl()
    {
        InitializeComponent();
        
        // El evento Loaded se dispara cuando el control ya está renderizado.
        // Aquí podemos acceder al TextBox y configurar el binding.
        Loaded += (s, e) =>
        {
            // Creamos un binding dinámico desde código (en lugar de en XAML)
            // Esto nos da más control sobre el comportamiento.
            ValueTextBox.SetBinding(TextBox.TextProperty, new Binding(nameof(Value))
            {
                Source = this,                          // El origen es este control
                Mode = BindingMode.TwoWay,              // Sincronización bidireccional
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged  // Actualiza en cada carácter
            });
            SyncTextBox();  // Sincroniza el texto inicial
        };
    }

    // =============================================================================
    // CALLBACKS DE CAMBIO DE PROPIEDADES
    // =============================================================================
    // Se llaman automáticamente cuando cambia el valor de las propiedades.
    
    /// <summary>
    /// Se ejecuta cuando cambia el valor del spinner.
    /// </summary>
    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var s = (SpinnerControl)d;
        
        // Si ya estamos actualizando, salimos para evitar bucle infinito
        if (s._isUpdating) return;
        
        s._isUpdating = true;
        
        // Clamp: asegurar que el valor está dentro del rango [Minimum, Maximum]
        var v = Math.Clamp((int)e.NewValue, s.Minimum, s.Maximum);
        
        // Si el valor estaba fuera del rango, corregirlo
        if (v != (int)e.NewValue)
            s.SetValue(ValueProperty, v);
        // Si el valor es válido, actualizar el TextBox para mostrarlo
        else if (s.ValueTextBox != null && s.ValueTextBox.Text != v.ToString())
            s.ValueTextBox.Text = v.ToString();
            
        s._isUpdating = false;
    }

    /// <summary>
    /// Se ejecuta cuando cambia el valor mínimo.
    /// Si el valor actual es menor que el nuevo mínimo, lo ajustamos.
    /// </summary>
    private static void OnMinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var s = (SpinnerControl)d;
        if (s.Value < s.Minimum) s.Value = s.Minimum;
    }

    /// <summary>
    /// Se ejecuta cuando cambia el valor máximo.
    /// Si el valor actual es mayor que el nuevo máximo, lo ajustamos.
    /// </summary>
    private static void OnMaxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var s = (SpinnerControl)d;
        if (s.Value > s.Maximum) s.Value = s.Maximum;
    }

    // =============================================================================
    // MÉTODOS AUXILIARES
    // =============================================================================

    /// <summary>
    /// Sincroniza el texto del TextBox con el valor actual.
    /// Usa _isUpdating para evitar bucles infinitos.
    /// </summary>
    private void SyncTextBox()
    {
        if (_isUpdating || ValueTextBox == null) return;
        _isUpdating = true;
        ValueTextBox.Text = Value.ToString();
        _isUpdating = false;
    }

    // =============================================================================
    // EVENTOS DE LOS BOTONES
    // =============================================================================

    /// <summary>
    /// Botón ▲ para incrementar el valor.
    /// </summary>
    private void Up_Click(object sender, RoutedEventArgs e)
    {
        if (Value < Maximum) 
        {
            Value++;
            // Mover el foco para que se disparte LostFocus y actualice el binding
            ValueTextBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }
    }

    /// <summary>
    /// Botón ▼ para decrementar el valor.
    /// </summary>
    private void Down_Click(object sender, RoutedEventArgs e)
    {
        if (Value > Minimum) 
        {
            Value--;
            ValueTextBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }
    }

    // =============================================================================
    // VALIDACIÓN DEL TEXTBOX
    // =============================================================================
    // Estos eventos controlan que solo se puedan escribir números.

    /// <summary>
    /// Se ejecuta antes de que se inserten caracteres.
    /// Solo permite dígitos (0-9).
    /// </summary>
    private void ValueTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        // Regex: solo acepta números
        e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
    }

    /// <summary>
    /// Se ejecuta cuando se pega texto desde el portapapeles.
    /// Cancela si el texto pegado no es numérico.
    /// </summary>
    private void ValueTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
    {
        if (e.DataObject.GetDataPresent(typeof(string)))
        {
            var text = (string)e.DataObject.GetData(typeof(string));
            if (!Regex.IsMatch(text, "^[0-9]+$"))
                e.CancelCommand();
        }
        else
        {
            e.CancelCommand();
        }
    }

    /// <summary>
    /// Se ejecuta cuando el TextBox pierde el foco.
    /// Valida y corrige el valor.
    /// </summary>
    private void ValueTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(ValueTextBox.Text, out var val))
        {
            // Clamp: asegurar que está en el rango
            Value = Math.Clamp(val, Minimum, Maximum);
        }
        SyncTextBox();
    }
}