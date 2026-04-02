// SelectionWindow.xaml.cs - Ejercicio 5: Componentes de Selección
// ==============================================================
// Este ejercicio introduce componentes para seleccionar opciones:
//
// 1. CheckBox: casilla de verificación
//    - IsChecked: estado (true/false/null si es ThreeState)
//    - IsThreeState: permite estado indeterminado (null)
//    - Checked/Unchecked/Indeterminate: eventos de cambio
//
// 2. RadioButton: botón de opción (selección exclusiva)
//    - IsChecked: estado seleccionado
//    - GroupName: grupo al que pertenece
//    - RadioButtons del mismo GroupName son mutuamente excluyentes
//
// 3. ComboBox: lista desplegable
//    - SelectedItem: elemento seleccionado
//    - SelectedIndex: índice seleccionado (-1 si ninguno)
//    - ItemsSource: colección de elementos
//    - SelectionChanged: evento al cambiar selección
//
// 4. ListBox: lista con selección
//    - SelectionMode: Single/Multiple/Extended
//    - SelectedItem: elemento seleccionado (Single)
//    - SelectedItems: colección de seleccionados (Multiple/Extended)
//    - SelectionChanged: evento al cambiar selección

using System.Windows;
using System.Windows.Controls;

namespace LayoutsComponentes.Views.Ejercicio5_Selection;

public partial class SelectionWindow : Window
{
    public SelectionWindow()
    {
        InitializeComponent();
        ActualizarResumen();
    }

    // ============================================================
    // CheckBox: Checked/Unchecked
    // ============================================================
    private void ChkAcepto_Changed(object sender, RoutedEventArgs e)
    {
        ActualizarResumen();
    }

    // ============================================================
    // CheckBox "Seleccionar todos": tres estados
    // ============================================================
    private void ChkTodos_Checked(object sender, RoutedEventArgs e)
    {
        // Todas las opciones marcadas
        chkOpcion1.IsChecked = true;
        chkOpcion2.IsChecked = true;
        chkOpcion3.IsChecked = true;
        ActualizarResumen();
    }

    private void ChkTodos_Unchecked(object sender, RoutedEventArgs e)
    {
        // Ninguna opción marcada
        chkOpcion1.IsChecked = false;
        chkOpcion2.IsChecked = false;
        chkOpcion3.IsChecked = false;
        ActualizarResumen();
    }

    private void ChkTodos_Indeterminate(object sender, RoutedEventArgs e)
    {
        // Estado intermedio: algunas marcadas
        chkOpcion1.IsChecked = true;
        chkOpcion2.IsChecked = false;
        chkOpcion3.IsChecked = true;
        ActualizarResumen();
    }

    // ============================================================
    // ComboBox: SelectionChanged
    // ============================================================
    private void CmbPais_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cmbPais.SelectedItem is ComboBoxItem item)
        {
            lblPais.Text = $"Seleccionado: {item.Content}";
            ActualizarResumen();
        }
    }

    // ============================================================
    // ListBox: SelectionChanged
    // ============================================================
    private void LstLenguajes_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (lstLenguajes.SelectedItem is ListBoxItem item)
        {
            lblLenguaje.Text = $"Seleccionado: {item.Content}";
            ActualizarResumen();
        }
    }

    // ============================================================
    // Actualizar el resumen
    // ============================================================
    private void ActualizarResumen()
    {
        var resumen = "📋 RESUMEN:\n";
        
        // CheckBox
        resumen += $"- Acepto términos: {(chkAcepto.IsChecked == true ? "Sí" : "No")}\n";
        
        // CheckBox de tres estados
        var opcionesMarcadas = 0;
        if (chkOpcion1.IsChecked == true) opcionesMarcadas++;
        if (chkOpcion2.IsChecked == true) opcionesMarcadas++;
        if (chkOpcion3.IsChecked == true) opcionesMarcadas++;
        resumen += $"- Opciones ({opcionesMarcadas}/3)\n";
        
        // RadioButton - Talla
        var talla = "M";
        if (rbTallaS.IsChecked == true) talla = "S";
        else if (rbTallaL.IsChecked == true) talla = "L";
        else if (rbTallaXL.IsChecked == true) talla = "XL";
        resumen += $"- Talla: {talla}\n";
        
        // RadioButton - Color
        var color = "Azul";
        if (rbRojo.IsChecked == true) color = "Rojo";
        else if (rbVerde.IsChecked == true) color = "Verde";
        resumen += $"- Color: {color}\n";
        
        // ComboBox
        if (cmbPais.SelectedItem is ComboBoxItem pais)
        {
            resumen += $"- País: {pais.Content}\n";
        }
        
        // ListBox
        if (lstLenguajes.SelectedItem is ListBoxItem lang)
        {
            resumen += $"- Lenguaje: {lang.Content}\n";
        }
        
        txtResumen.Text = resumen;
    }

    // ============================================================
    // Botón cerrar
    // ============================================================
    private void BtnCerrar_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}

// ============================================================
// RESUMEN: Componentes de Selección
// ============================================================
//
// | Componente  | Propiedad clave     | Característica                    |
// |-------------|---------------------|------------------------------------|
// | CheckBox    | IsChecked (bool?)   | Dos o tres estados               |
// | RadioButton | IsChecked           | Selección exclusiva por GroupName |
// | ComboBox    | SelectedItem/Index  | Lista desplegable                |
// | ListBox     | SelectedItem/Items   | Lista con selección              |
//
// RADIOBUTTON:
// - Todos los RadioButton con el mismo GroupName son excluyentes
// - Solo uno puede estar marcado a la vez
// - Si ninguno tiene IsChecked="True", ninguno está seleccionado
//
// CHECKBOX THREE STATE:
// - IsThreeState="True" permite el valor null
// - null = Indeterminate (estado intermedio)
// - Útil para "Seleccionar todos" con estado parcial
//
// LISTBOX SELECTIONMODE:
// - Single: solo un elemento
// - Multiple: varios elementos sin Ctrl
// - Extended: varios elementos con Ctrl/Shift
