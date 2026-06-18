// ============================================================
// VentanaDatos.xaml.cs - Paso de datos por constructor
// ============================================================
// Esta ventana recibe datos a través de su constructor.
//
// CONCEPTO:
// - El constructor puede recibir parámetros
// - Los datos se pasan al crear la instancia: new VentanaDatos(nombre, edad)
// - Ideal cuando los datos son obligatorios

using System.Windows;

namespace NavegacionVentanas.Views.VentanaDatos;

/// <summary>
/// Ventana que recibe datos por constructor.
/// </summary>
public partial class VentanaDatos : Window
{
    /// <summary>
    /// Resultado que se devolverá a la ventana padre.
    /// </summary>
    public string Resultado { get; private set; } = "";

    /// <summary>
    /// Constructor que recibe datos.
    /// </summary>
    /// <param name="nombre">Nombre del usuario.</param>
    /// <param name="edad">Edad del usuario.</param>
    public VentanaDatos(string nombre, int edad)
    {
        InitializeComponent();
        
        // Mostrar los datos recibidos
        TxtNombre.Text = nombre;
        TxtEdad.Text = $"{edad} años";
        
        // Inicializar el resultado
        Resultado = $"Nombre: {nombre}, Edad: {edad}";
    }

    /// <summary>
    /// Aceptar y cerrar.
    /// </summary>
    private void BtnAceptar_Click(object sender, RoutedEventArgs e)
    {
        // Actualizar el resultado con lo editable
        Resultado = TxtResultado.Text;
        DialogResult = true;
    }

    /// <summary>
    /// Cancelar y cerrar.
    /// </summary>
    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
