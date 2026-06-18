// WrapCanvasWindow.xaml.cs - Ejercicio 3: WrapPanel y Canvas
// ===========================================================
// Este ejercicio introduce:
//
// 1. WrapPanel: Contenedor de flujo
//    - Distribuye hijos en línea
//    - Salta a siguiente fila/columna cuando no cabe
//    - Orientation="Horizontal" o "Vertical"
//    - ItemWidth/ItemHeight: tamaño de cada elemento
//
// 2. Canvas: Posicionamiento absoluto
//    - Coordenadas Left y Top absolutas
//    - No gestiona tamaños automáticamente
//    - Ideal para dibujo, juegos, diagramas
//
// 3. ScrollViewer: permite desplazamiento
//    - VerticalScrollBarVisibility="Auto"
//    - HorizontalScrollBarVisibility="Auto"
//
// 4. Border con CornerRadius: bordes redondeados
//
// 5. Ellipse y Rectangle: formas básicas
//
// EJEMPLO VISUAL DEL WRAPPANEL:
//
// Si ItemWidth="100" y el contenedor tiene 350px de ancho:
// +----------+----------+----------+
// | Elemento | Elemento | Elemento |
// |   1     |    2     |    3     |
// +----------+----------+----------+
// | Elemento | Elemento |          |
// |   4     |    5     |          |
// +----------+----------+----------+
//
// EJEMPLO VISUAL DEL CANVAS:
//
// Canvas con 300x200:
// +------------------------------|
// |    ○                         |
// |       ┌──────────┐           |
// |       │ rectángulo│          |
// | ¡Hola!│          │ ┌────┐   |
// |       └──────────┘ │btn │   |
// |                └────┘            |
// +------------------------------|

using System.Windows;

namespace LayoutsComponentes.Views.Ejercicio3_WrapCanvas;

public partial class WrapCanvasWindow : Window
{
    public WrapCanvasWindow()
    {
        InitializeComponent();
    }
}

// ============================================================
// RESUMEN: WrapPanel vs Canvas
// ============================================================
//
// WRAPPANEL:
// - Flujo automático: salta de línea cuando no cabe
// - Ideal para: galerías, iconos, chips, etiquetas
// - No necesitas especificar posiciones
// - Se adapta al tamaño del contenedor
//
// CANVAS:
// - Posicionamiento absoluto con coordenadas
// - Canvas.Left, Canvas.Top, Canvas.Right, Canvas.Bottom
// - Ideal para: dibujo técnico, juegos, diagramas
// - Tienes control total de cada posición
// - No redimensiona hijos automáticamente
//
// PROPIEDADES DEL CANVAS:
// - Canvas.Left: distancia desde el borde izquierdo
// - Canvas.Top: distancia desde el borde superior
// - Canvas.Right: distancia desde el borde derecho
// - Canvas.Bottom: distancia desde el borde inferior
//
// NOTA: Canvas es el único panel que NO redistribuye hijos
// cuando el tamaño cambia. Los hijos mantienen su posición.
