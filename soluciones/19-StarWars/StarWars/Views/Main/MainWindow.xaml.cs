using System.Windows;
using StarWars.ViewModels;
using StarWars.Views.Dialog;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace StarWars.Views.Main;

/// <summary>
/// Ventana principal del juego Star Wars.
/// </summary>
/// <remarks>
/// Esta clase es el code-behind de MainWindow.xaml. Gestiona:
/// - La interacción del usuario con los menús
/// - La inyección del ViewModel en el DataContext
/// 
/// El patrón MVVM en WPF recomienda mantener el code-behind mínimo,
/// delegando la lógica al ViewModel. Aquí solo hay eventos de menú
/// que no belongecen a la lógica de negocio.
/// </remarks>
public partial class MainWindow : Window
{
    /// <summary>
    /// ViewModel de la ventana principal.
    /// Se inyecta mediante el constructor (Dependency Injection).
    /// </summary>
    private readonly MainViewModel _viewModel;

    /// <summary>
    /// Constructor de la ventana principal.
    /// </summary>
    /// <param name="viewModel">ViewModel inyectado por el contenedor DI</param>
    /// <remarks>
    /// El ViewModel se inyecta aquí en lugar de crearlo manualmente.
    /// Esto permite:
    /// - Control de dependencias
    /// - Facilidades para testing (podemos inyectar mocks)
    /// -follow el patrón de DI establecido en la aplicación
    /// </remarks>
    public MainWindow(MainViewModel viewModel)
    {
        // Inicializar los componentes XAML (botones, textos, etc.)
        InitializeComponent();
        
        // Guardar referencia al ViewModel
        _viewModel = viewModel;
        
        // Establecer el DataContext para que los bindings funcionen
        // Ahora todos los controles de la UI pueden bindearse a las propiedades del ViewModel
        DataContext = _viewModel;
    }

    /// <summary>
    /// Evento del menú "Salir".
    /// </summary>
    /// <param name="sender">Elemento que generó el evento</param>
    /// <param name="e">Argumentos del evento</param>
    /// <remarks>
    /// Muestra un MessageBox de confirmación antes de cerrar la aplicación.
    /// </remarks>
    private void MenuSalir_Click(object sender, RoutedEventArgs e)
    {
        // Mostrar diálogo de confirmación
        var result = MessageBox.Show(
            "¿Seguro que quieres salir?",  // Mensaje
            "Salir",                        // Título
            MessageBoxButton.YesNo,        // Botones (Sí/No)
            MessageBoxImage.Question);     // Icono (pregunta)
        
        // Si el usuario eligió "Sí", cerrar la aplicación
        if (result == MessageBoxResult.Yes)
        {
            // Shutdown() cierra todas las ventanas y termina la aplicación
            Application.Current.Shutdown();
        }
    }

    /// <summary>
    /// Evento del menú "Acerca de".
    /// </summary>
    /// <param name="sender">Elemento que generó el evento</param>
    /// <param name="e">Argumentos del evento</param>
    /// <remarks>
    /// Abre la ventana de diálogo "Acerca de" como ventana modal.
    /// </remarks>
    private void MenuAcercaDe_Click(object sender, RoutedEventArgs e)
    {
        // Crear la ventana de diálogo
        var dialog = new AcercaDeWindow { Owner = this };
        
        // ShowDialog() abre la ventana como modal (bloquea la principal)
        // ShowDialog() devuelve el resultado cuando se cierra
        dialog.ShowDialog();
    }
}
