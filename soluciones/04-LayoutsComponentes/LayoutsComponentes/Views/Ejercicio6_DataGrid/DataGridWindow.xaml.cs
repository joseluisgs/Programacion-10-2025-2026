// DataGridWindow.xaml.cs - Ejercicio 6: DataGrid y ListView
// ==========================================================
// Este ejercicio introduce componentes de datos complejos:
//
// 1. DataGrid: tabla para mostrar y editar datos
//    - AutoGenerateColumns: genera columnas automáticamente
//    - Columnas manuales con DataGridColumn
//    - DataGridTextColumn: columna de texto con formato
//    - DataGridCheckBoxColumn: columna de checkbox
//    - DataGridTemplateColumn: columna con plantilla personalizada
//    - SelectionMode/SelectionUnit: configuración de selección
//    - CanUserSortColumns: permitir ordenar por cabecera
//    - AlternatingRowBackground: color alternativo de filas
//
// 2. ListView: lista flexible con vistas
//    - GridView: vista de tabla
//    - GridViewColumn: columna con DisplayMemberBinding
//
// 3. TabControl: pestañas
//    - TabItem: cada pestaña
//    - Header: título de la pestaña
//
// 4. Binding: enlazar datos a controles
//    - {Binding PropertyName}: sintaxis de binding
//    - StringFormat: formato del valor

using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LayoutsComponentes.Views.Ejercicio6_DataGrid;

// Clase para representar un Alumno
public class Alumno
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public double Nota { get; set; }
    public bool Aprobado { get; set; }
}

// Clase para representar un Producto
public class Producto
{
    public string Codigo { get; set; } = "";
    public string Nombre { get; set; } = "";
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}

public partial class DataGridWindow : Window
{
    // ObservableCollection: notifica cambios a la UI automáticamente
    private ObservableCollection<Alumno> _alumnos = [];
    private ObservableCollection<Producto> _productos = [];

    public DataGridWindow()
    {
        InitializeComponent();
        
        // Cargar datos de ejemplo
        CargarAlumnos();
        CargarProductos();
    }

    // ============================================================
    // Cargar datos en DataGrid
    // ============================================================
    private void CargarAlumnos()
    {
        // Añadir alumnos a la colección
        _alumnos.Add(new Alumno { Id = 1, Nombre = "Ana López", Nota = 7.5, Aprobado = true });
        _alumnos.Add(new Alumno { Id = 2, Nombre = "Carlos Ruiz", Nota = 4.2, Aprobado = false });
        _alumnos.Add(new Alumno { Id = 3, Nombre = "María Torres", Nota = 9.1, Aprobado = true });
        _alumnos.Add(new Alumno { Id = 4, Nombre = "Pedro Sánchez", Nota = 5.0, Aprobado = true });
        _alumnos.Add(new Alumno { Id = 5, Nombre = "Laura García", Nota = 3.8, Aprobado = false });
        
        // Asignar al DataGrid
        dgAlumnos.ItemsSource = _alumnos;
    }

    // ============================================================
    // Cargar datos en ListView
    // ============================================================
    private void CargarProductos()
    {
        _productos.Add(new Producto { Codigo = "P001", Nombre = "Portátil", Precio = 899.99m, Stock = 15 });
        _productos.Add(new Producto { Codigo = "P002", Nombre = "Ratón", Precio = 29.99m, Stock = 50 });
        _productos.Add(new Producto { Codigo = "P003", Nombre = "Teclado", Precio = 49.99m, Stock = 30 });
        _productos.Add(new Producto { Codigo = "P004", Nombre = "Monitor", Precio = 199.99m, Stock = 20 });
        _productos.Add(new Producto { Codigo = "P005", Nombre = "Auriculares", Precio = 79.99m, Stock = 45 });
        
        lvProductos.ItemsSource = _productos;
    }

    // ============================================================
    // DataGrid: Evento de clic en botón
    // ============================================================
    private void BtnVer_Click(object sender, RoutedEventArgs e)
    {
        // Obtener el botón que se pulsó
        if (sender is Button btn && btn.Tag is int id)
        {
            // Buscar el alumno por ID
            var alumno = _alumnos.FirstOrDefault(a => a.Id == id);
            if (alumno != null)
            {
                txtInfo.Text = $"👀 Ver detalles del alumno: {alumno.Nombre} - Nota: {alumno.Nota:F1}";
            }
        }
    }
}

// ============================================================
// RESUMEN: DataGrid y ListView
// ============================================================
//
// DATAGRID:
// | Propiedad              | Descripción                                      |
// |-----------------------|--------------------------------------------------|
// | AutoGenerateColumns   | Genera columnas automáticamente                 |
// | IsReadOnly            | Impide/edita las celdas                          |
// | CanUserSortColumns    | Permite ordenar por clic en cabecera            |
// | AlternatingRowBackground | Color alternativo de filas                |
// | SelectionMode         | Single/Multiple/Extended                        |
// | SelectionUnit         | FullRow/Cell/CellOrRowHeader                    |
//
// COLUMNAS DEL DATAGRID:
// - DataGridTextColumn: texto con formato
// - DataGridCheckBoxColumn: checkbox
// - DataGridTemplateColumn: plantilla personalizada (botones, etc.)
//
// LISTVIEW CON GRIDVIEW:
// - GridView: vista de tabla
// - GridViewColumn: columna con DisplayMemberBinding
// - Ideal para listas con pocas columnas
