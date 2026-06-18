// StackDockWindow.xaml.cs - Ejercicio 2: StackPanel y DockPanel
// ==============================================================
// Este ejercicio introduce:
//
// 1. DockPanel: Contenedor que acopla hijos a los bordes
//    - DockPanel.Dock="Top/Bottom/Left/Right"
//    - LastChildFill: el último hijo ocupa el espacio restante
//
// 2. StackPanel: Contenedor que apila elementos
//    - Orientation="Vertical": apila de arriba a abajo (por defecto)
//    - Orientation="Horizontal": apila de izquierda a derecha
//
// 3. Menu y MenuItem: Barra de menú tradicional
//    - Header: Texto que se muestra
//    - _X: Tecla de acceso (Alt+X)
//    - Click: Evento al hacer clic
//    - InputGestureText: Texto del atajo de teclado
//    - IsCheckable: Marca/desmarca el item
//    - Separator: Línea separadora
//
// 4. StatusBar: Barra de estado en la parte inferior
//
// 5. Label con tecla mnemónica:
//    - "_N" crea una tecla de acceso (Alt+N)
//    - Target vincula el Label al control correspondiente
//
// Ejemplo visual del DockPanel:
// +----------------------------------+
// | Menu (DockPanel.Dock="Top")     |
// +----+-----------------------------+
// |    |                             |
// | L  |     Área central            |
// | E  |   (LastChildFill=True)      |
// | F  |                             |
// | T  |                             |
// +----+-----------------------------+
// | StatusBar (Dock="Bottom")        |
// +----------------------------------+

using System;
using System.Windows;
using System.Windows.Controls;

namespace LayoutsComponentes.Views.Ejercicio2_StackDock;

public partial class StackDockWindow : Window
{
    // ============================================================
    // ATRIBUTOS
    // ============================================================
    
    /// <summary>
    /// Estado de la barra de herramientas.
    /// </summary>
    private bool _barraHerramientasVisible = true;
    
    /// <summary>
    /// Estado de la barra de estado.
    /// </summary>
    private bool _barraEstadoVisible = true;
    
    /// <summary>
    /// Zoom actual.
    /// </summary>
    private double _zoom = 1.0;

    // ============================================================
    // CONSTRUCTOR
    // ============================================================
    
    /// <summary>
    /// Constructor de la ventana.
    /// </summary>
    public StackDockWindow()
    {
        InitializeComponent();
        ActualizarEstado();
    }

    // ============================================================
    // MENÚ ARCHIVO
    // ============================================================
    
    /// <summary>
    /// Archivo -> Nuevo
    /// </summary>
    private void MenuNuevo_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("📄 Nuevo documento creado", "Archivo", MessageBoxButton.OK, MessageBoxImage.Information);
        ActualizarEstado("Nuevo documento creado");
    }

    /// <summary>
    /// Archivo -> Abrir
    /// </summary>
    private void MenuAbrir_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("📂 Abrir archivo (simulación)", "Archivo", MessageBoxButton.OK, MessageBoxImage.Information);
        ActualizarEstado("Archivo abierto");
    }

    /// <summary>
    /// Archivo -> Guardar
    /// </summary>
    private void MenuGuardar_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("💾 Archivo guardado", "Archivo", MessageBoxButton.OK, MessageBoxImage.Information);
        ActualizarEstado("Archivo guardado");
    }

    /// <summary>
    /// Archivo -> Guardar como
    /// </summary>
    private void MenuGuardarComo_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("📋 Guardar como... (simulación)", "Archivo", MessageBoxButton.OK, MessageBoxImage.Information);
        ActualizarEstado("Archivo guardado como");
    }

    /// <summary>
    /// Archivo -> Acerca de
    /// </summary>
    private void MenuAcercaDe_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
            "📐 Ejercicio 2: StackPanel y DockPanel\n\n" +
            "Este ejercicio muestra:\n" +
            "• Menu y MenuItem\n" +
            "• DockPanel\n" +
            "• StackPanel\n" +
            "• StatusBar\n" +
            "• Label con mnemónicas",
            "Acerca de",
            MessageBoxButton.OK,
            MessageBoxImage.Information
        );
    }

    /// <summary>
    /// Archivo -> Salir
    /// </summary>
    private void MenuSalir_Click(object sender, RoutedEventArgs e)
    {
        var resultado = MessageBox.Show(
            "¿Seguro que quieres salir?",
            "Confirmar",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question
        );
        
        if (resultado == MessageBoxResult.Yes)
        {
            Close();
        }
    }

    // ============================================================
    // MENÚ EDITAR
    // ============================================================
    
    /// <summary>
    /// Editar -> Cortar
    /// </summary>
    private void MenuCortar_Click(object sender, RoutedEventArgs e)
    {
        ActualizarEstado("Acción: Cortar (Ctrl+X)");
    }

    /// <summary>
    /// Editar -> Copiar
    /// </summary>
    private void MenuCopiar_Click(object sender, RoutedEventArgs e)
    {
        ActualizarEstado("Acción: Copiar (Ctrl+C)");
    }

    /// <summary>
    /// Editar -> Pegar
    /// </summary>
    private void MenuPegar_Click(object sender, RoutedEventArgs e)
    {
        ActualizarEstado("Acción: Pegar (Ctrl+V)");
    }

    /// <summary>
    /// Editar -> Deshacer
    /// </summary>
    private void MenuDeshacer_Click(object sender, RoutedEventArgs e)
    {
        ActualizarEstado("Acción: Deshacer (Ctrl+Z)");
    }

    /// <summary>
    /// Editar -> Rehacer
    /// </summary>
    private void MenuRehacer_Click(object sender, RoutedEventArgs e)
    {
        ActualizarEstado("Acción: Rehacer (Ctrl+Y)");
    }

    // ============================================================
    // MENÚ VER
    // ============================================================
    
    /// <summary>
    /// Ver -> Aumentar zoom
    /// </summary>
    private void MenuAumentar_Click(object sender, RoutedEventArgs e)
    {
        _zoom = Math.Min(_zoom + 0.1, 2.0);
        ActualizarEstado($"Zoom: {_zoom * 100:F0}%");
    }

    /// <summary>
    /// Ver -> Disminuir zoom
    /// </summary>
    private void MenuDisminuir_Click(object sender, RoutedEventArgs e)
    {
        _zoom = Math.Max(_zoom - 0.1, 0.5);
        ActualizarEstado($"Zoom: {_zoom * 100:F0}%");
    }

    /// <summary>
    /// Ver -> Zoom predeterminado
    /// </summary>
    private void MenuZoomPredeterminado_Click(object sender, RoutedEventArgs e)
    {
        _zoom = 1.0;
        ActualizarEstado($"Zoom: {_zoom * 100:F0}%");
    }

    /// <summary>
    /// Ver -> Barra de herramientas (marcable)
    /// </summary>
    private void MenuBarraHerramientas_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem item)
        {
            _barraHerramientasVisible = item.IsChecked;
            ActualizarEstado($"Barra de herramientas: {(_barraHerramientasVisible ? "visible" : "oculta")}");
        }
    }

    /// <summary>
    /// Ver -> Barra de estado (marcable)
    /// </summary>
    private void MenuBarraEstado_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem item)
        {
            _barraEstadoVisible = item.IsChecked;
            ActualizarEstado($"Barra de estado: {(_barraEstadoVisible ? "visible" : "oculta")}");
        }
    }

    // ============================================================
    // MENÚ AYUDA
    // ============================================================
    
    /// <summary>
    /// Ayuda -> Manual
    /// </summary>
    private void MenuManual_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("📖 Manual de ayuda (simulación)", "Ayuda", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    /// <summary>
    /// Ayuda -> Reportar error
    /// </summary>
    private void MenuReportarError_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("🐛 Formulario de reporte de error (simulación)", "Reportar", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    // ============================================================
    // MÉTODOS AUXILIARES
    // ============================================================
    
    /// <summary>
    /// Actualiza el texto de la barra de estado.
    /// </summary>
    private void ActualizarEstado(string mensaje = "Listo")
    {
        // Buscar el StatusBarItem en el DockPanel
        if (FindName("txtEstado") is TextBlock txt)
        {
            txt.Text = mensaje;
        }
    }
}

// ============================================================
// RESUMEN: StackPanel vs DockPanel
// ============================================================
//
// STACKPANEL:
// - Apila elementos en UNA dirección
// - Orientation="Vertical" (por defecto): arriba→abajo
// - Orientation="Horizontal": izquierda→derecha
// - No redimensiona hijos automáticamente
// - Ideal para formularios, listas simples
//
// DOCKPANEL:
// - Acopla elementos a los CUATRO bordes
// - DockPanel.Dock="Top/Bottom/Left/Right"
// - LastChildFill=True: el último hijo ocupa el centro
// - Ideal para estructura de ventana clásica (menú, barra lateral, área central)
//
// MENU:
// - Menu: Contenedor principal de la barra de menú
// - MenuItem: Elemento del menú
// - _X: Tecla de acceso (Alt+X)
// - InputGestureText: Muestra atajo de teclado
// - IsCheckable: Permite marcar/desmarcar
// - Separator: Línea separadora entre items
//
// PROPIEDADES COMUNES:
// - Margin: espacio exterior entre controles
// - Padding: espacio interior dentro del contenedor
// - HorizontalAlignment: alineación horizontal
// - VerticalAlignment: alineación vertical
