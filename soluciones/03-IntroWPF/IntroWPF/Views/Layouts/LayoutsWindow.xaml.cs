// LayoutsWindow.xaml.cs - Ejercicio 4: Layouts en WPF
// ===================================================
// Este ejercicio introduce los principales sistemas de diseño en WPF:
//
// 1. DockPanel: anclar controles a los bordes (Top, Bottom, Left, Right)
//    - Equivalente al Dock de WinForms pero más flexible
//    - LastChildFill hace que el último hijo ocupe el espacio restante
//
// 2. Grid: sistema de filas y columnas
//    - Equivalente a una tabla HTML
//    - RowDefinitions y ColumnDefinition para definir estructura
//    - Muy flexible y usado frecuentemente
//
// 3. StackPanel: apila controles en una dirección
//    - Orientation="Horizontal": apila horizontalmente
//    - Orientation="Vertical": apila verticalmente (por defecto)
//
// 4. Border: contenedor con borde, fondo, padding
//    - Útil para agrupar elementos visualmente
//
// Diferencias con WinForms:
// - WPF tiene un sistema de diseño más potente y flexible
// - Los controles se dimensionan automáticamente según el contenido
// - No se usa Location (x,y) sino LayoutPanels

using System.Windows;

namespace IntroWPF.Views.Layouts;

public partial class LayoutsWindow : Window
{
    public LayoutsWindow()
    {
        InitializeComponent();
    }
}
