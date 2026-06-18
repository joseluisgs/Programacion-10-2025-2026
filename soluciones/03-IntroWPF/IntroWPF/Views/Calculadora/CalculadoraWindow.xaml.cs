// CalculadoraWindow.xaml.cs - Ejercicio 2: Calculadora básica
// ==========================================================
// Este ejercicio introduce:
// - TextBox: caja de texto para entrada de datos
// - ComboBox: lista desplegable para seleccionar operación
// - TextBlock: etiqueta para mostrar resultado
// - Grid: sistema de diseño en filas y columnas
// - Evento Click con validación de datos
// - TryParse: convertir texto a número de forma segura

using System.Windows;
using System.Windows.Controls;

namespace IntroWPF.Views.Calculadora;

// Window: clase base para ventanas en WPF
public partial class CalculadoraWindow : Window
{
    // Constructor
    public CalculadoraWindow()
    {
        // InitializeComponent(): carga el XAML
        InitializeComponent();
    }

    // ============================================================
    // EVENTO Click del botón Calcular
    // ============================================================
    private void BtnCalcular_Click(object sender, RoutedEventArgs e)
    {
        // ---------------------------------------------
        // OBTENER VALORES DE LOS TextBox
        // ---------------------------------------------
        // TextBox.Text: propiedad que contiene el texto introducido
        // Importante: siempre es un string, hay que convertirlo
        
        // double.TryParse(): intenta convertir string a double
        // Devuelve true si la conversión funciona, false si no
        // out var n1: si funciona, el valor se guarda en n1
        
        if (!double.TryParse(TxtNum1.Text, out var n1) || 
            !double.TryParse(TxtNum2.Text, out var n2))
        {
            // Si la conversión falla, mostrar mensaje de error
            LblRes.Text = "Error: números inválidos";
            return;  // Salir del método
        }

        // ---------------------------------------------
        // OBTENER OPERACIÓN DEL ComboBox
        // ---------------------------------------------
        // SelectedItem: el elemento seleccionado (ComboBoxItem)
        // Content: texto del ComboBoxItem
        // Para obtener el texto: (SelectedItem as ComboBoxItem).Content.ToString()
        
        var operacion = (CmbOp.SelectedItem as ComboBoxItem)?.Content?.ToString();

        // ---------------------------------------------
        // CALCULAR RESULTADO CON switch
        // ---------------------------------------------
        var resultado = operacion switch
        {
            "+" => n1 + n2,
            "-" => n1 - n2,
            "*" => n1 * n2,
            // División: comprobar que no sea división por cero
            "/" => n2 != 0 ? n1 / n2 : double.NaN,
            // Caso por defecto
            _ => 0.0
        };

        // ---------------------------------------------
        // MOSTRAR RESULTADO
        // ---------------------------------------------
        LblRes.Text = $"Resultado: {resultado}";
    }
}
