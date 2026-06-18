// MainWindow.xaml.cs - Código subyacente de la ventana principal
// =============================================================
// Esta clase contiene la lógica de la ventana principal.
// En WPF, cada archivo .xaml tiene un archivo .xaml.cs asociado (code-behind)
//
// Este ejemplo introduce:
// - Window: ventana de WPF (equivalente a Form en WinForms)
// - XAML: lenguaje para diseñar interfaces en WPF
// - Code-behind: código C# asociado al XAML
// - Eventos Click de botones
// - Ciclo de vida de una ventana WPF
// - Closing: evento que se dispara al cerrar la ventana
// - MessageBox: diálogos de confirmación

using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace IntroWPF.Views.Views.Main;

// Window: clase base para ventanas en WPF
public partial class MainWindow : Window
{
    // Constructor de la ventana
    public MainWindow()
    {
        // InitializeComponent(): carga el XAML y crea los controles
        // Siempre se llama en el constructor de ventanas WPF
        InitializeComponent();

        // ============================================================
        // EVENTO Loaded: la ventana se ha cargado completamente
        // ============================================================
        // Loaded se dispara después de que el XAML se ha cargado
        // y todos los controles están listos para usarse.
        // Es el lugar ideal para inicializar datos.
        Loaded += (_, _) =>
        {
            Debug.WriteLine("🔵 [MainWindow] Evento Loaded - La ventana se ha cargado");
        };

        // ============================================================
        // EVENTO IsVisibleChanged: la ventana se muestra/oculta
        // ============================================================
        // IsVisibleChanged se dispara cuando cambia la visibilidad de la ventana
        IsVisibleChanged += (_, _) =>
        {
            if (IsVisible)
                Debug.WriteLine("🟢 [MainWindow] La ventana ahora es visible");
        };

        // ============================================================
        // EVENTO Closing: confirmar al cerrar la aplicación
        // ============================================================
        // Closing se dispara justo ANTES de que la ventana se cierre
        // Nos permite cancelar el cierre o ejecutar código antes de cerrar
        Closing += (_, e) =>
        {
            Debug.WriteLine("🟠 [MainWindow] Evento Closing - La ventana está a punto de cerrarse");
            
            // ---------------------------------------------
            // MessageBox: mostrar diálogo de confirmación
            // ---------------------------------------------
            // MessageBox.Show() tiene varios parámetros:
            // - message: texto del mensaje
            // - caption: título de la ventana
            // - button: botones a mostrar (YesNo = Sí/No)
            // - icon: icono a mostrar (Question = signo de interrogación)
            
            var resultado = MessageBox.Show(
                "¿Estás seguro de que quieres salir?",  // Mensaje
                "Confirmar salida",                       // Título
                MessageBoxButton.YesNo,                  // Botones: Sí y No
                MessageBoxImage.Question                  // Icono: pregunta
            );

            // ---------------------------------------------
            // Cancelar si el usuario pulsó "No"
            // ---------------------------------------------
            if (resultado == MessageBoxResult.No)
            {
                // Cancel = true: cancelar el cierre de la ventana
                e.Cancel = true;
                Debug.WriteLine("⚠️  [MainWindow] Cierre cancelado por el usuario");
            }
            // Si pulsó "Sí", la ventana se cerrará normalmente
        };

        // ============================================================
        // EVENTO Closed: la ventana se ha cerrado
        // ============================================================
        // Closed se dispara DESPUÉS de que la ventana se ha cerrado
        // Ya no se puede cancelar. Es útil para limpiar recursos.
        Closed += (_, _) =>
        {
            Debug.WriteLine("🔴 [MainWindow] Evento Closed - La ventana se ha cerrado");
        };
        
        // Mensaje inicial para mostrar el ciclo de vida
        Debug.WriteLine("=" .PadRight(50, '='));
        Debug.WriteLine("🔵 [MainWindow] Constructor - La ventana se está creando");
        Debug.WriteLine("   Próximo evento: Loaded (cuando se cargue el XAML)");
    }

    // ============================================================
    // EVENTO Click del botón Hola Mundo
    // ============================================================
    // Al pulsar el botón, abrir la ventana del ejercicio 1
    private void BtnHolaMundo_Click(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine("\n🖱️  [MainWindow] Botón 'Hola Mundo' pulsado");
        
        // Crear nueva instancia de la ventana y mostrar como diálogo
        var ventana = new HolaMundo.HolaMundoWindow();
        
        // Suscribirnos al ciclo de vida de la ventana hija para ver qué pasa
        ventana.Loaded += (_, _) => Debug.WriteLine("   → [HolaMundoWindow] Loaded");
        ventana.Closed += (_, _) => Debug.WriteLine("   → [HolaMundoWindow] Closed");
        
        Debug.WriteLine("   → Abriendo HolaMundoWindow...");
        ventana.ShowDialog();
        Debug.WriteLine("   → HolaMundoWindow cerrada, volviendo a MainWindow");
    }

    // ============================================================
    // EVENTO Click del botón Calculadora
    // ============================================================
    private void BtnCalculadora_Click(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine("\n🖱️  [MainWindow] Botón 'Calculadora' pulsado");
        
        var ventana = new Calculadora.CalculadoraWindow();
        ventana.Loaded += (_, _) => Debug.WriteLine("   → [CalculadoraWindow] Loaded");
        ventana.Closed += (_, _) => Debug.WriteLine("   → [CalculadoraWindow] Closed");
        
        Debug.WriteLine("   → Abriendo CalculadoraWindow...");
        ventana.ShowDialog();
        Debug.WriteLine("   → CalculadoraWindow cerrada, volviendo a MainWindow");
    }

    // ============================================================
    // EVENTO Click del botón Formulario
    // ============================================================
    private void BtnFormulario_Click(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine("\n🖱️  [MainWindow] Botón 'Formulario' pulsado");
        
        var ventana = new Formulario.FormularioRegistroWindow();
        ventana.Loaded += (_, _) => Debug.WriteLine("   → [FormularioRegistroWindow] Loaded");
        ventana.Closed += (_, _) => Debug.WriteLine("   → [FormularioRegistroWindow] Closed");
        
        Debug.WriteLine("   → Abriendo FormularioRegistroWindow...");
        ventana.ShowDialog();
        Debug.WriteLine("   → FormularioRegistroWindow cerrada, volviendo a MainWindow");
    }

    // ============================================================
    // EVENTO Click del botón Layouts
    // ============================================================
    private void BtnLayouts_Click(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine("\n🖱️  [MainWindow] Botón 'Layouts' pulsado");
        
        var ventana = new Layouts.LayoutsWindow();
        ventana.Loaded += (_, _) => Debug.WriteLine("   → [LayoutsWindow] Loaded");
        ventana.Closed += (_, _) => Debug.WriteLine("   → [LayoutsWindow] Closed");
        
        Debug.WriteLine("   → Abriendo LayoutsWindow...");
        ventana.ShowDialog();
        Debug.WriteLine("   → LayoutsWindow cerrada, volvemos a MainWindow");
    }
}

// ============================================================
// RESUMEN: CICLO DE VIDA DE UNA VENTANA WPF
// ============================================================
//
// 1. Constructor → Se crea el objeto ventana
// 2. InitializeComponent() → Se carga el XAML y se crean los controles
// 3. Loaded → La ventana y sus controles están listos (ideal para inicializar)
// 4. [Usuario interactúa con la ventana]
// 5. Closing → Se va a cerrar (se puede cancelar con e.Cancel = true)
// 6. Closed → Se ha cerrado (no se puede cancelar)
//
// NOTA: Para ver estos mensajes, ejecuta la aplicación y consulta la ventana de salida de depuración.
