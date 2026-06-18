// HolaMundoWindow.xaml.cs - Ejercicio 1: Primera ventana WPF
// ==========================================================
// Este ejercicio introduce:
// - Window: ventana de WPF
// - XAML: lenguaje de marcas para diseñar interfaces
// - TextBlock: control para mostrar texto (equivalente a Label)
// - Button: botón con evento Click
// - Close(): cerrar la ventana
//
// Diferencias con WinForms:
// - En WPF se usa XAML para diseñar, no código imperativo
// - Los controles tienen nombres diferentes (TextBlock vs Label)
// - El posicionamiento se hace con Layouts (Grid, StackPanel, etc.)

using System.Windows;

namespace IntroWPF.Views.HolaMundo;

// Window: clase base para ventanas en WPF
public partial class HolaMundoWindow : Window
{
    // Constructor
    public HolaMundoWindow()
    {
        // InitializeComponent(): carga el XAML y crea los controles
        // Debe llamarse siempre en el constructor
        InitializeComponent();
        
        // ---------------------------------------------
        // WindowStartupLocation = CenterOwner
        // ---------------------------------------------
        //Hace que la ventana se abra centrada respecto a su padre (Owner)
        //En este caso, se centrará respecto a MainWindow
    }

    // ============================================================
    // EVENTO Click del botón Cerrar
    // ============================================================
    // private: solo accesible desde esta clase
    // void: no devuelve ningún valor
    // BtnCerrar_Click: nombre del método manejador
    // object sender: referencia al botón que se pulsó
    // RoutedEventArgs e: argumentos del evento (información adicional)
    private void BtnCerrar_Click(object sender, RoutedEventArgs e)
    {
        // Close(): cierra la ventana actual
        Close();
    }
}
