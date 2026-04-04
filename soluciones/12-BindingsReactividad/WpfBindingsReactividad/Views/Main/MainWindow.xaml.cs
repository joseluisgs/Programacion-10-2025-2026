// ============================================================
// MainWindow.xaml.cs - Code-behind de la ventana principal
// ============================================================
//
// =================================================================
// GUIA PARA EL ALUMNO: POR QUE EXISTE EL CODE-BEHIND?
// =================================================================
//
// En MVVM, el objetivo es que la vista (XAML) no tenga logica de negocio.
// Pero a veces necesitamos ejecutar codigo que solo tiene sentido en la UI.
//
// El CODE-BEHIND existe para:
//
// 1. EVENTOS DE LA UI
//    - Click de botones (aunque usemos Commands)
//    - Eventos de teclado
//    - Animaciones
//
// 2. LOGICA DE PRESENTACION
//    - Cosas que solo afectan a la vista
//    - No tienen sentido en el ViewModel
//
// 3. LLAMADAS EXPLICITAS AL BINDING
//    - UpdateSourceTrigger=Explicit
//    - Necesitamos llamar a UpdateSource() manualmente
//
// EN ESTE CASO:
// Usamos el code-behind para el boton de reinicio porque es un caso
// simple donde no necesitamos un comando completo.
//
// EN RESUMEN: Code-behind es para LOGICA DE VISTA, NO DE NEGOCIO.
//             La logica de negocio siempre va en el ViewModel.
//

using System.Windows;
using System.Windows.Controls;
using WpfBindingsReactividad.ViewModels;

namespace WpfBindingsReactividad.Views.Main;

/// <summary>
/// Ventana principal que muestra todas las demos de binding.
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        // Establecemos el DataContext con un ViewModel combinado
        // que tiene todas las demos anteriores + la nueva de FormData
        DataContext = new MainViewModel();
    }

    /// <summary>
    /// Manejador del boton Reiniciar.
    /// </summary>
    /// <remarks>
    /// NOTA: En MVVM puro, usariamos un ICommand en el ViewModel.
    /// Pero para algo tan simple como reiniciar, un manejador esta bien.
    /// 
    /// Ademas, esto nos permite demostrar que el code-behind PUEDE
    /// acceder al ViewModel a traves de DataContext.
    /// </remarks>
    private void BtnReiniciar_Click(object sender, RoutedEventArgs e)
    {
        // Obtenemos el ViewModel desde el DataContext
        if (DataContext is BindingDemoViewModel viewModel)
        {
            // Llamamos al metodo de reinicio del ViewModel
            viewModel.Reiniciar();
        }
    }

    /// <summary>
    /// Manejador del evento TextChanged para el enfoque OneWay + Eventos.
    /// </summary>
    /// <remarks>
    /// NOTA: Este es el enfoque "OneWay + Eventos" que NO usa TwoWay.
    /// 
    /// - El binding en XAML es Mode=OneWay (solo lee del ViewModel)
    /// - Aqui actualizamos el ViewModel manualmente
    /// - Podemos añadir LOGICA EXTRA antes de actualizar
    /// 
    /// En este ejemplo:
    /// 1. Contamos caracteres
    /// 2. Solo actualizamos si tiene al menos 3 caracteres
    /// 3. Esto seria dificil de hacer con TwoWay
    /// </remarks>
    private void TxtOneWay_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Obtenemos el TextBox
        if (sender is TextBox textBox)
        {
            // Actualizamos el contador de caracteres
            txtContador.Text = textBox.Text.Length.ToString();

            //Aqui tenemos CONTROL TOTAL sobre cuando actualizar el ViewModel
            //Por ejemplo, podemos validar antes de actualizar
            if (textBox.Text.Length >= 3)
            {
                // Solo actualizamos si tiene al menos 3 caracteres
                // Esto seria dificil de hacer con TwoWay
                if (DataContext is BindingDemoViewModel viewModel)
                {
                    viewModel.NombreEvento = textBox.Text;
                }
            }
            else
            {
                // No actualizamos si tiene menos de 3 caracteres
                // Pero el TextBox sigue mostrando lo que escribe el usuario
                // porque el binding es OneWay (solo lee del ViewModel)
            }
        }
    }
}
