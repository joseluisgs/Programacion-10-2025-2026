// ============================================================
// MainWindow.xaml.cs - Code-behind de la ventana principal
// ============================================================
// Este archivo es el code-behind de MainWindow.xaml.
//
// CONCEPTOS IMPORTANTES:
//
// 1. CODE-BEHIND:
//    - Archivo .xaml.cs que contiene el codigo C# de la vista
//    - En MVVM, el code-behind debe tener muy poca logica
//    - Solo se usa para configurar inicializaciones
//
// 2. DataContext:
//    - Propiedad que establece el origen de datos para el binding
//    - Aqui asignamos el ViewModel como fuente de datos
//    - Todos los bindings de XAML buscaran propiedades en el DataContext
//
// 3. SEPARACION DE RESPONSABILIDADES:
//    - Vista (XAML): Solo define la interfaz grafica
//    - Code-behind: Solo inicializa el DataContext
//    - ViewModel: Contiene toda la logica de presentacion

using System.Windows;
using WpfMVVMBasico.ViewModels;

namespace WpfMVVMBasico.Views.Main;

/// <summary>
/// Ventana principal de la aplicacion.
/// En MVVM, el code-behind debe tener minima logica.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Constructor de la ventana.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        
        // ============================================================
        // DataContext: Establecer el ViewModel como fuente de datos
        // ============================================================
        // Aqui es donde ocurre la "magia" del binding:
        // - DataContext = new ContadorViewModel()
        // - XAML: Text="{Binding Contador}" busca Contador en ContadorViewModel
        // - XAML: Command="{Binding IncrementarCommand}" busca IncrementarCommand
        
        DataContext = new ContadorViewModel();
    }
}

// ============================================================
// RESUMEN: QUE HACE FALTA EN EL CODE-BEHIND
// ============================================================
//
// En este proyecto, el code-behind SOLO hace esto:
//   DataContext = new ContadorViewModel();
//
// No hay ninguna otra logica!
// No hay manejadores de eventos (Click, etc.)
// No hay manipulacion de controles
//
// Todo eso se maneja en el ViewModel mediante propiedades y comandos.
// Esto es lo que queremos en MVVM: maxima separacion de responsabilidades.
//
// ============================================================
// COMPARACION: SIN MVVM vs CON MVVM
// ============================================================
//
// Sin MVVM (code-behind tradicional):
//   private void BtnIncrementar_Click(object sender, RoutedEventArgs e)
//   {
//       Contador++;
//       TxtContador.Text = Contador.ToString();
//       BtnDecrementar.IsEnabled = Contador > 0;
//   }
//
// Con MVVM (este proyecto):
//   - XAML: Command="{Binding IncrementarCommand}"
//   - ViewModel: IncrementarCommand = new RelayCommand(() => Contador++)
//   - No hay codigo en el code-behind!
//
// ============================================================
