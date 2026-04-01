// FormComponentsWindow.xaml.cs - Ejercicio 4: Componentes de Formulario
// ======================================================================
// Este ejercicio introduce componentes de entrada de datos:
//
// 1. TextBox: entrada de texto multilínea
//    - Text: propiedad con el texto
//    - ToolTip: texto de ayuda
//    - MaxLength: límite de caracteres
//    - AcceptsReturn: permite saltos de línea
//    - TextChanged: evento al cambiar el texto
//
// 2. PasswordBox: entrada de contraseña segura
//    - Password: propiedad con la contraseña (NO se puede hacer binding directo)
//    - PasswordChar: carácter de enmascaramiento
//    - MaxLength: límite de caracteres
//    - PasswordChanged: evento al cambiar la contraseña
//
// 3. Slider: control deslizante
//    - Value: valor actual
//    - Minimum/Maximum: rango
//    - IsSnapToTickEnabled: saltos entre marcas
//    - TickFrequency: distancia entre marcas
//    - TickPlacement: posición de las marcas
//    - ValueChanged: evento al cambiar el valor
//
// 4. DatePicker: selector de fecha
//    - SelectedDate: fecha seleccionada (DateTime?)
//    - SelectedDateChanged: evento al cambiar la fecha
//
// 5. RepeatButton: botón de repetición continua
//    - Delay: tiempo antes de iniciar repetición (ms)
//    - Interval: tiempo entre repeticiones (ms)
//    - Click: evento que se repite
//
// 6. ToggleButton: botón de dos estados
//    - IsChecked: estado del botón (bool?)
//    - Checked/Unchecked: eventos de cambio de estado

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LayoutsComponentes.Views.Ejercicio4_FormComponents;

public partial class FormComponentsWindow : Window
{
    // Campo para el contador
    private int _contador = 10;

    public FormComponentsWindow()
    {
        InitializeComponent();
        
        // Inicializar el contador
        lblContador.Text = _contador.ToString();
    }

    // ============================================================
    // PasswordBox: PasswordChanged
    // ============================================================
    private void PwdPassword_PasswordChanged(object sender, RoutedEventArgs e)
    {
        // PasswordBox NO tiene propiedad Text por seguridad
        // Se accede mediante la propiedad Password
        var longitud = pwdPassword.Password.Length;
        lblPasswordInfo.Text = $"Contraseña: {(longitud > 0 ? new string('●', longitud) : "(vacía)")}";
    }

    // ============================================================
    // Slider: ValueChanged
    // ============================================================
    private void SldVolumen_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        // RoutedPropertyChangedEventArgs<double> contiene:
        // - OldValue: valor anterior
        // - NewValue: valor nuevo
        lblVolumen.Text = $"Volumen: {e.NewValue:F0}%";
    }

    // ============================================================
    // DatePicker: SelectedDateChanged
    // ============================================================
    private void DpFecha_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
    {
        if (dpFecha.SelectedDate.HasValue)
        {
            // SelectedDate es DateTime? (puede ser null)
            var fecha = dpFecha.SelectedDate.Value;
            lblFecha.Text = $"Fecha seleccionada: {fecha:dd/MM/yyyy}";
        }
        else
        {
            lblFecha.Text = "Fecha seleccionada: (ninguna)";
        }
    }

    // ============================================================
    // RepeatButton: Click (decrementar)
    // ============================================================
    private void BtnDecrementar_Click(object sender, RoutedEventArgs e)
    {
        _contador--;
        lblContador.Text = _contador.ToString();
    }

    // ============================================================
    // RepeatButton: Click (incrementar)
    // ============================================================
    private void BtnIncrementar_Click(object sender, RoutedEventArgs e)
    {
        _contador++;
        lblContador.Text = _contador.ToString();
    }

    // ============================================================
    // ToggleButton: Checked/Unchecked
    // ============================================================
    private void TglNegrita_Changed(object sender, RoutedEventArgs e)
    {
        // Determinar el estilo de fuente según los toggles
        var fontWeight = tglNegrita.IsChecked == true ? FontWeights.Bold : FontWeights.Normal;
        var fontStyle = tglCursiva.IsChecked == true ? FontStyles.Italic : FontStyles.Normal;
        
        // TextDecorations para el subrayado
        var textDecorations = tglSubrayado.IsChecked == true ? TextDecorations.Underline : null;
        
        // Aplicar al TextBlock de preview
        txtPreview.FontWeight = fontWeight;
        txtPreview.FontStyle = fontStyle;
        txtPreview.TextDecorations = textDecorations;
    }

    // ============================================================
    // Button: Cerrar
    // ============================================================
    private void BtnCerrar_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}

// ============================================================
// RESUMEN: Componentes de Formulario
// ============================================================
//
// | Componente      | Propiedad principal | Uso                              |
// |----------------|-------------------|----------------------------------|
// | TextBox        | Text              | Entrada de texto multilínea      |
// | PasswordBox    | Password          | Contraseña (segura)             |
// | Slider         | Value             | Valor numérico en rango         |
// | DatePicker     | SelectedDate      | Selección de fecha               |
// | RepeatButton   | Delay/Interval    | Botón de repetición continua    |
// | ToggleButton   | IsChecked         | Estado activado/desactivado     |
//
// NOTA DE SEGURIDAD:
// PasswordBox.Password NO es una propiedad de dependencia normal,
// por lo que NO se puede hacer binding directo. Si necesitas
// binding, usa soluciones como PasswordBoxHelper.
