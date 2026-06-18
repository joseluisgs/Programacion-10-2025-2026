// ============================================================
// MainWindow.xaml.cs - Ventana principal con demos de navegación
// ============================================================
// Esta ventana contiene los botones para abrir cada tipo de ventana secundaria.
//
// CONCEPTOS DEMOSTRADOS:
// 1. ShowDialog(): Abre ventana modal, bloquea la padre, devuelve DialogResult
// 2. Show(): Abre ventana no modal, permite interactuar con ambas
// 3. Paso por constructor: Pasar datos al crear la ventana
// 4. Paso por propiedades: Establecer propiedades antes de Show()
// 5. ViewModel compartido: Instancia única que comunican ventanas
// 6. Flujo Login -> Main: Cambio de ventana inicial

using System.Windows;
using NavegacionVentanas.Views.VentanaModal;
using NavegacionVentanas.Views.VentanaNoModal;
using NavegacionVentanas.Views.VentanaDatos;
using NavegacionVentanas.Views.VentanaPropiedades;
using NavegacionVentanas.Views.VentanaViewModel;
using NavegacionVentanas.Views.Login;
using NavegacionVentanas.ViewModels;

namespace NavegacionVentanas.Views.Main;

/// <summary>
/// Ventana principal con botones para demos de navegación.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Referencia a la ventana no modal (para actualizarla).
    /// </summary>
    private VentanaNoModal.VentanaNoModal? _ventanaNoModal;

    /// <summary>
    /// Constructor de la ventana principal.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        ActualizarInfoViewModel();
    }

    /// <summary>
    /// Actualiza el texto que muestra el estado del ViewModel.
    /// </summary>
    private void ActualizarInfoViewModel()
    {
        if (App.SharedViewModel != null)
        {
            TxtViewModelInfo.Text = $"Contador: {App.SharedViewModel.Contador} | Mensaje: {App.SharedViewModel.Mensaje}";
        }
    }

    // ============================================================
    // Demo 1: Ventana Modal con ShowDialog
    // ============================================================
    
    /// <summary>
    /// Abre una ventana modal con ShowDialog().
    /// ShowDialog() bloquea la ventana padre hasta que se cierre.
    /// </summary>
    private void BtnVentanaModal_Click(object sender, RoutedEventArgs e)
    {
        // Crear la ventana modal
        var ventana = new VentanaModal.VentanaModal();
        
        // Abrir con ShowDialog() - devuelve DialogResult (true/false/null)
        // El código siguiente no se ejecuta hasta que se cierre la ventana
        var resultado = ventana.ShowDialog();
        
        // Mostrar el resultado
        if (resultado == true)
        {
            MessageBox.Show("El usuario hizo clic en Aceptar", "Resultado", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else if (resultado == false)
        {
            MessageBox.Show("El usuario canceló", "Resultado", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    // ============================================================
    // Demo 2: Ventana No Modal con Show
    // ============================================================
    
    /// <summary>
    /// Abre una ventana no modal con Show().
    /// Show() permite tener ambas ventanas abiertas y activas.
    /// </summary>
    private void BtnVentanaNoModal_Click(object sender, RoutedEventArgs e)
    {
        // Si ya existe, solo enfocarla
        if (_ventanaNoModal != null && _ventanaNoModal.IsVisible)
        {
            _ventanaNoModal.Activate();
            return;
        }
        
        // Crear nueva ventana no modal
        _ventanaNoModal = new VentanaNoModal.VentanaNoModal();
        
        // Evento para actualizar cuando se cierre
        _ventanaNoModal.Closed += (s, args) =>
        {
            _ventanaNoModal = null;
            ActualizarInfoViewModel();
        };
        
        // Abrir con Show() - no bloquea
        _ventanaNoModal.Show();
    }

    // ============================================================
    // Demo 3: Paso de datos por constructor
    // ============================================================
    
    /// <summary>
    /// Abre una ventana pasando datos por el constructor.
    /// El constructor recibe los parámetros al crear la instancia.
    /// </summary>
    private void BtnDatosConstructor_Click(object sender, RoutedEventArgs e)
    {
        // Crear la ventana pasando datos en el constructor
        var nombre = "Juan";
        var edad = 25;
        
        var ventana = new VentanaDatos.VentanaDatos(nombre, edad);
        
        // Abrir como modal
        var resultado = ventana.ShowDialog();
        
        if (resultado == true)
        {
            MessageBox.Show($"Datos recibidos: {ventana.Resultado}", "Datos", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    // ============================================================
    // Demo 4: Paso de datos por propiedades
    // ============================================================
    
    /// <summary>
    /// Abre una ventana estableciendo propiedades antes de Show().
    /// Más flexible que el constructor para casos complejos.
    /// </summary>
    private void BtnDatosPropiedades_Click(object sender, RoutedEventArgs e)
    {
        // Crear la ventana
        var ventana = new VentanaPropiedades.VentanaPropiedades();
        
        // Establecer propiedades ANTES de abrir
        ventana.TituloPersonalizado = "Datos del Cliente";
        ventana.DatoPrincipal = "Información importante";
        
        // Abrir
        ventana.ShowDialog();
    }

    // ============================================================
    // Demo 5: ViewModel compartido
    // ============================================================
    
    /// <summary>
    /// Abre una ventana que usa el ViewModel compartido.
    /// Ambas ventanas ven los mismos datos en tiempo real.
    /// </summary>
    private void BtnViewModelCompartido_Click(object sender, RoutedEventArgs e)
    {
        // Obtener el ViewModel compartido de App
        var viewModel = App.SharedViewModel!;
        
        // Crear ventana pasándole el ViewModel
        var ventana = new VentanaViewModel.VentanaViewModel(viewModel);
        
        // Actualizar info al cerrar
        ventana.Closed += (s, args) => ActualizarInfoViewModel();
        
        // Abrir
        ventana.Show();
    }

    // ============================================================
    // Demo 6: Flujo Login -> Main
    // ============================================================
    
    /// <summary>
    /// Simula el flujo Login -> Main.
    /// Cierra esta ventana y abre la de login.
    /// </summary>
    private void BtnLogin_Click(object sender, RoutedEventArgs e)
    {
        // Crear ventana de login
        var loginWindow = new LoginWindow();
        
        // Mostrar login
        var resultado = loginWindow.ShowDialog();
        
        // Si el login fue exitoso, cerrar esta ventana
        // (En este demo, siempre volveremos a esta ventana)
        if (resultado == true)
        {
            MessageBox.Show("¡Login exitoso! Bienvenido.", "Login", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    // ============================================================
    // Botón Salir
    // ============================================================
    
    /// <summary>
    /// Cierra la aplicación.
    /// </summary>
    private void BtnSalir_Click(object sender, RoutedEventArgs e)
    {
        // Cerrar cualquier ventana no modal abierta
        _ventanaNoModal?.Close();
        
        // Cerrar la aplicación
        Application.Current.Shutdown();
    }
}

// ============================================================
// RESUMEN: NAVEGACIÓN EN WPF
// ============================================================
//
// SHOW() vs SHOWDIALOG():
// - Show(): Ventana no modal, puedes interactuar con otras ventanas
// - ShowDialog(): Ventana modal, bloquea la ventana padre hasta cerrar
//
// DIALOGRESULT:
// - Solo funciona con ShowDialog()
// - DialogResult = true: El usuario aceptó
// - DialogResult = false: El usuario canceló
// - DialogResult = null: Se cerró sin botones (X)
//
// PASAR DATOS:
// - Por constructor: Simple, datos obligatorios
// - Por propiedades: Flexible, datos opcionales
// - Por ViewModel: Para comunicación bidireccional en tiempo real
//
// VIEWMODEL COMPARTIDO:
// - Crear en App.xaml.cs
// - Pasar a ambas ventanas
// - Cambios se reflejan en todas las ventanas bindeadas
//
// FLUJO LOGIN -> MAIN:
// - Cambiar StartupUri en App.xaml
// - O cerrar MainWindow y abrir Login al inicio
// - Luego de login exitoso, abrir Main y cerrar Login
