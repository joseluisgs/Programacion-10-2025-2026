// TabsExpanderWindow.xaml.cs - Ejercicio 7: Tabs, Expander y más
// ==============================================================
// Este ejercicio introduce contenedores especiales:
//
// 1. TabControl: organiza contenido en pestañas
//    - TabItem: cada pestaña
//    - Header: título de la pestaña
//    - IsEnabled: habilitar/deshabilitar pestaña
//    - Content: contenido de la pestaña
//
// 2. Expander: panel expandible/contraíble
//    - Header: título del expander
//    - IsExpanded: estado inicial (expandido/contraído)
//    - Content: contenido que se muestra/oculta
//
// 3. GroupBox: grupo con borde y título
//    - Header: título del grupo
//    - Content: contenido del grupo
//    - Padding: espacio interior
//
// 4. Border: elemento decorativo
//    - Background: color de fondo
//    - BorderBrush/BorderThickness: borde
//    - CornerRadius: esquinas redondeadas
//    - Padding: espacio interior
//    - Effect: efectos (sombra DropShadowEffect)
//
// 5. Separator: línea divisoria horizontal
//    - Margin: espacio alrededor
//    - Useful en menús y listas
//
// 6. ScrollViewer: contenedor con desplazamiento
//    - VerticalScrollBarVisibility: visibilidad del scroll vertical
//    - HorizontalScrollBarVisibility: visibilidad del scroll horizontal

using System.Windows;

namespace LayoutsComponentes.Views.Ejercicio7_TabsExpander;

public partial class TabsExpanderWindow : Window
{
    public TabsExpanderWindow()
    {
        InitializeComponent();
    }
}

// ============================================================
// RESUMEN: Contenedores Especiales
// ============================================================
//
// TABCONTROL:
// - Organiza contenido en pestañas
// - Ideal para interfaces con mucho contenido
// - Mejora la experiencia de usuario
// - IsEnabled=False deshabilita una pestaña
//
// EXPANDER:
// - Panel que se puede expandir/contraer
// - Ideal para opciones avanzadas o configuración
// - IsExpanded=True muestra el contenido inicialmente
//
// GROUPBOX:
// - Agrupa controles relacionados
// - Borde visible con título
// - Mejora la organización visual
//
// BORDER:
// - Contenedor simple con un hijo
// - Útil para decoraciones (bordes, esquinas redondeadas, sombras)
// - CornerRadius para esquinas redondeadas
// - DropShadowEffect para sombras
//
// SEPARATOR:
// - Línea horizontal divisoria
// - Útil en menús y listas de opciones
//
// SCROLLVIEWER:
// - Proporciona desplazamiento cuando el contenido no cabe
// - VerticalScrollBarVisibility="Auto" muestra cuando es necesario
