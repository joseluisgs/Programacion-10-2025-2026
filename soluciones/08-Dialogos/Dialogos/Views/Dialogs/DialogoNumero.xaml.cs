// ============================================================
// DialogoNumero.xaml.cs - Diálogo para seleccionar un número
// ============================================================
// Ejemplo de diálogo que:
// - Recibe parámetros en el constructor
// - Usa un Slider para seleccionar valor
// - Devuelve el número seleccionado

using System.Windows;
using System.Windows.Controls;

namespace Dialogos.Views.Dialogs;

public partial class DialogoNumero : Window
{
    // ============================================================
    // PROPIEDADES
    // ============================================================
    
    /// <summary>
    /// Número seleccionado por el usuario.
    /// </summary>
    public int Numero { get; private set; }

    /// <summary>
    /// Valor mínimo permitido.
    /// </summary>
    private readonly int _minimo;
    
    /// <summary>
    /// Valor máximo permitido.
    /// </summary>
    private readonly int _maximo;

    // ============================================================
    // CONSTRUCTOR
    // ============================================================
    
    /// <summary>
    /// Constructor con parámetros.
    /// </summary>
    /// <param name="valorInicial">Valor inicial del slider.</param>
    /// <param name="minimo">Valor mínimo.</param>
    /// <param name="maximo">Valor máximo.</param>
    public DialogoNumero(int valorInicial = 50, int minimo = 0, int maximo = 100)
    {
        InitializeComponent();
        
        // Guardar valores
        _minimo = minimo;
        _maximo = maximo;
        
        // Configurar slider
        SliderNumero.Minimum = minimo;
        SliderNumero.Maximum = maximo;
        SliderNumero.Value = valorInicial;
        
        // Mostrar valor inicial
        Numero = valorInicial;
        TxtValor.Text = $"Valor: {valorInicial}";
    }

    // ============================================================
    // EVENTOS
    // ============================================================
    
    /// <summary>
    /// Se ejecuta cuando cambia el valor del slider.
    /// </summary>
    private void SliderNumero_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (TxtValor != null)
        {
            Numero = (int)SliderNumero.Value;
            TxtValor.Text = $"Valor: {Numero}";
        }
    }

    /// <summary>
    /// Botón Aceptar.
    /// </summary>
    private void BtnAceptar_Click(object sender, RoutedEventArgs e)
    {
        Numero = (int)SliderNumero.Value;
        DialogResult = true;
    }

    /// <summary>
    /// Botón Cancelar.
    /// </summary>
    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
