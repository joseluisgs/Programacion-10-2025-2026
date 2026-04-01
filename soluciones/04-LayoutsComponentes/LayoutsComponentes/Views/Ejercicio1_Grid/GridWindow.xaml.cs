// GridWindow.xaml.cs - Ejercicio 1: Grid (Filas y Columnas)
// ==========================================================
// Este ejercicio introduce:
//
// 1. Grid: el contenedor más importante de WPF
//    - Divide el espacio en filas y columnas
//    - Permite posicionar controles en celdas específicas
//
// 2. RowDefinitions y ColumnDefinitions:
//    - Auto: se ajusta al contenido
//    - * (Star): ocupa espacio proporcional
//    - Número: tamaño fijo en píxeles
//
// 3. Propiedades adjuntas:
//    - Grid.Row: fila donde se coloca el control
//    - Grid.Column: columna donde se coloca el control
//    - Grid.RowSpan: número de filas que ocupa
//    - Grid.ColumnSpan: número de columnas que ocupa
//
// 4. Border: contenedor con borde, fondo y padding
//
// Ejemplo visual del Grid:
// +------------------------------------------+
// | Encabezado (Grid.ColumnSpan=3)          |
// +----------+---------------+--------------+
// | Izquierdo | Central       |              |
// | (1/3)    | (2/3)         |              |
// +----------+---------------+--------------+
// | Pie de página (Grid.ColumnSpan=3)       |
// +------------------------------------------+

using System.Windows;

namespace LayoutsComponentes.Views.Ejercicio1_Grid;

// Window: clase base para ventanas en WPF
public partial class GridWindow : Window
{
    // Constructor
    public GridWindow()
    {
        // InitializeComponent(): carga el XAML y crea los controles
        InitializeComponent();
    }
}

// ============================================================
// RESUMEN: Grid
// ============================================================
//
// | Unidad   | Ejemplo      | Descripción                              |
// |----------|--------------|------------------------------------------|
// | Auto     | Height="Auto"| Se ajusta al contenido del hijo          |
// | *        | Height="*"   | Ocupa el espacio restante (proporcional) |
// | 2*       | Width="2*"   | Ocupa el doble de espacio que "*"       |
// | Número   | Width="100"  | Tamaño fijo en píxeles                   |
//
// Propiedades adjuntas:
// - Grid.Row: fila (0, 1, 2, ...)
// - Grid.Column: columna (0, 1, 2, ...)
// - Grid.RowSpan: ocupa N filas
// - Grid.ColumnSpan: ocupa N columnas
