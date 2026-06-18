// ResourcesWindow.xaml.cs - Ejercicio 8: Recursos Estáticos y Dinámicos
// ======================================================================
// Este ejercicio introduce el sistema de recursos de WPF:
//
// 1. Recursos: valores reutilizables definidos en un lugar
//    - Se definen en Window.Resources o App.Resources
//    - Se referencian con {StaticResource} o {DynamicResource}
//
// 2. StaticResource: referencia estática
//    - Se resuelve en tiempo de carga del XAML
//    - Más rápido en rendimiento
//    - NO detecta cambios en tiempo de ejecución
//    - Úsalo para valores que no cambian (colores, estilos estáticos)
//
// 3. DynamicResource: referencia dinámica
//    - Se resuelve en tiempo de ejecución
//    - Permite cambios dinámicos (temas)
//    - Ligera sobrecarga de rendimiento
//    - Úsalo cuando necesites cambiar valores en runtime
//
// 4. Estilos (Style): conjuntos de propiedades
//    - TargetType: tipo de control al que se aplica
//    - x:Key: nombre para referenciarlo
//    - Setter: propiedad a establecer
//
// 5. Propiedades de recursos comunes:
//    - Color: color ARGB
//    - SolidColorBrush: pincel de color
//    - Thickness: espacio (margen/padding)
//    - FontFamily, FontSize, etc.

using System.Windows;
using System.Windows.Media;

namespace LayoutsComponentes.Views.Ejercicio8_Resources;

public partial class ResourcesWindow : Window
{
    public ResourcesWindow()
    {
        InitializeComponent();
    }

    // ============================================================
    // DynamicResource: Cambiar a tema oscuro
    // ============================================================
    private void BtnTemaOscuro_Click(object sender, RoutedEventArgs e)
    {
        // Al cambiar el recurso, todos los DynamicResource se actualizan automáticamente
        Resources["ColorFondo"] = new SolidColorBrush(Color.FromRgb(45, 45, 48));
        
        // El Border también usa el recurso dinámico, así que se actualiza
        panelDinamico.Background = new SolidColorBrush(Color.FromRgb(30, 30, 30));
    }

    // ============================================================
    // DynamicResource: Cambiar a tema claro
    // ============================================================
    private void BtnTemaClaro_Click(object sender, RoutedEventArgs e)
    {
        // Volver al color original
        Resources["ColorFondo"] = new SolidColorBrush(Colors.White);
        
        panelDinamico.Background = new SolidColorBrush(Colors.White);
    }
}

// ============================================================
// RESUMEN: StaticResource vs DynamicResource
// ============================================================
//
// | Característica       | StaticResource           | DynamicResource              |
// |---------------------|--------------------------|------------------------------|
// | Resolución         | Tiempo de carga         | Tiempo de ejecución         |
// | Rendimiento        | ✅ Más rápido            | ⚠️ Ligeramente más lento    |
// | Detecta cambios    | ❌ No                    | ✅ Sí                        |
// | Si no existe       | 💥 Excepción             | ⚠️ Sin valor visible         |
// | Cuándo usar        | Valores fijos           | Temas dinámicos             |
//
// JERARQUÍA DE BÚSQUEDA DE RECURSOS:
// 1. Recursos del elemento actual
// 2. Recursos del padre (hacia arriba)
// 3. Window.Resources
// 4. App.xaml / Application.Resources
// 5. Recursos del sistema
//
// DEFINICIÓN DE ESTILOS:
// <Style x:Key="Nombre" TargetType="Tipo">
//     <Setter Property="Propiedad" Value="Valor"/>
// </Style>
//
// USO DE ESTILOS:
// <Button Style="{StaticResource Nombre}"/>
