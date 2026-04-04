// ============================================================
// MainWindow.xaml.cs - Ventana principal de Gestión de Productos
// ============================================================
// Este ejercicio introduce los siguientes conceptos:
//
// 1. WPF CON DATAGRID:
//    - DataGrid: Componente para mostrar datos en formato tabla
//    - AutoGenerateColumns: Generar columnas automáticamente
//    - SelectionMode: Selección única o múltiple
//
// 2. OBSERVABLECOLLECTION:
//    - Notifica a la UI automáticamente cuando se modifican elementos
//    - Ideal para binding con DataGrid
//
// 3. RAILWAY ORIENTED PROGRAMMING:
//    - Result<T, DomainError>: Tipo que representa éxito o fallo
//    - IsSuccess/IsFailure: Verificar el estado
//    - Value/Error: Obtener el valor o el error
//
// 4. INYECCIÓN DE DEPENDENCIAS:
//    - Obtener servicios del ServiceProvider
//    - Dependency Injection en el constructor
//
// 5. PATRÓN REPOSITORY:
//    - ICrudRepository<TKey, TEntity>: Interfaz genérica para operaciones CRUD
//    - IProductoRepository: Interfaz específica para productos
//    - ProductoRepository: Implementación con SQLite
//
// 6. VALIDADOR:
//    - IValidador<T>: Interfaz genérica para validadores
//    - ValidadorProducto: Implementación con reglas de validación
//
// 7. SERVICE (LÓGICA DE NEGOCIO):
//    - IProductoService: Interfaz del servicio
//    - ProductoService: Implementación con validación y Result<T, DomainError>

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using GestionProductos.Models;
using GestionProductos.Services;
using GestionProductos.Errors;
using GestionProductos.Infrastructure;
using GestionProductos.Views.Dialog;

namespace GestionProductos.Views.Main;

// ============================================================
// CONVERTIDOR PARA EL BINDING
// ============================================================
// Los convertidores permiten transformar datos durante el binding.
// Se usan cuando el tipo de dato no coincide con lo que espera la UI.

/// <summary>
/// Convertidor que muestra/oculta un elemento según la longitud del texto.
/// Binding: int longitud -> Visibility
/// </summary>
public class IntToVisibilityConverter : System.Windows.Data.IValueConverter
{
    /// <summary>
    /// Convierte la longitud del texto en visibilidad.
    /// Si hay texto (> 0), oculta el elemento. Si no hay texto, lo muestra.
    /// </summary>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        // Si hay texto, ocultamos el placeholder (Collapsed). Si no hay texto, lo mostramos (Visible)
        if (value is int length)
            return length > 0 ? Visibility.Collapsed : Visibility.Visible;
        return Visibility.Visible;
    }

    /// <summary>
    /// No implementamos la conversión inversa (no se usa en este caso).
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        => throw new NotImplementedException();
}

// ============================================================
// MAINWINDOW - VENTANA PRINCIPAL
// ============================================================

/// <summary>
/// Ventana principal para gestionar productos.
/// Gestiona el CRUD completo de productos con validaciones y confirmaciones.
/// </summary>
public partial class MainWindow : Window
{
    // ============================================================
    // ATRIBUTOS PRIVADOS
    // ============================================================
    
    /// <summary>
    /// ObservableCollection: Notifica a la UI automáticamente cuando se añaden,
    /// eliminan o modifican elementos. Es ideal para el binding con DataGrid.
    /// </summary>
    private readonly ObservableCollection<Producto> _productos = [];
    
    /// <summary>
    /// Servicio de productos que contiene la lógica de negocio.
    /// Se obtiene del ServiceProvider de App mediante inyección de dependencias.
    /// </summary>
    private readonly IProductoService _servicio;

    /// <summary>
    /// Producto seleccionado en el DataGrid.
    /// </summary>
    private Producto? _productoSeleccionado;

    // ============================================================
    // CONSTRUCTOR
    // ============================================================
    
    /// <summary>
    /// Constructor de la ventana principal.
    /// Inicializa los componentes, configura el binding y carga los datos.
    /// </summary>
    public MainWindow()
    {
        // ============================================================
        // IMPORTANTE: Añadir convertidores ANTES de InitializeComponent()
        // porque el XAML los necesita al cargar los recursos.
        // ============================================================
        Resources["IntToVisibilityConverter"] = new IntToVisibilityConverter();
        
        // Inicializar componentes XAML (carga el archivo .xaml)
        InitializeComponent();
        
        // ============================================================
        // INYECCIÓN DE DEPENDENCIAS
        // ============================================================
        // Obtener el servicio del ServiceProvider estático en App.
        // GetRequiredService<T> crea automáticamente la instancia con sus dependencias.
        // En este caso: ProductoService depende de IProductoRepository y IValidador<Producto>.
        _servicio = App.ServiceProvider.GetRequiredService<IProductoService>();
        
        // Configurar el ItemsSource para el binding con el DataGrid
        GridProductos.ItemsSource = _productos;
        
        // Cargar los productos iniciales desde el repositorio
        CargarProductos();
        CargarCategorias();
    }

    // ============================================================
    // MÉTODO: CARGAR PRODUCTOS DESDE EL REPOSITORIO
    // ============================================================
    
    /// <summary>
    /// Carga todos los productos del repositorio y los muestra en el DataGrid.
    /// </summary>
    private void CargarProductos()
    {
        // Obtener todos los productos del servicio (que usa el repositorio)
        // El servicio devuelve Result<Producto, DomainError> para manejar errores
        var resultado = _servicio.GetAll();
        
        if (resultado.IsSuccess)
        {
            // Limpiar la colección actual
            _productos.Clear();
            
            // Añadir todos los productos a la ObservableCollection
            // Esto notificará automáticamente al DataGrid para actualizar la UI
            foreach (var producto in resultado.Value)
            {
                _productos.Add(producto);
            }
            
            // Actualizar las estadísticas
            ActualizarEstadisticas();
        }
        else
        {
            // Mostrar mensaje de error si falla la carga
            MostrarError(resultado.Error.Message);
        }
    }

    /// <summary>
    /// Carga las categorías únicas en el ComboBox para filtrar.
    /// </summary>
    private void CargarCategorias()
    {
        var resultado = _servicio.GetAll();
        
        if (resultado.IsSuccess)
        {
            // Obtener categorías únicas y ordenarlas
            var categorias = resultado.Value
                .Select(p => p.Categoria)           // Seleccionar solo la categoría
                .Distinct()                          // Eliminar duplicados
                .OrderBy(c => c)                    // Ordenar alfabéticamente
                .ToList();

            // Limpiar y añadir las categorías al ComboBox
            CmbCategoria.Items.Clear();
            CmbCategoria.Items.Add(new ComboBoxItem { Content = "Todas", IsSelected = true });
            
            foreach (var categoria in categorias)
            {
                CmbCategoria.Items.Add(new ComboBoxItem { Content = categoria });
            }
        }
    }

    /// <summary>
    /// Calcula y muestra las estadísticas del inventario.
    /// </summary>
    private void ActualizarEstadisticas()
    {
        var resultado = _servicio.GetEstadisticas();
        
        if (resultado.IsSuccess)
        {
            // Obtener estadísticas del diccionario
            var stats = resultado.Value;
            
            // Mostrar en los TextBlock correspondientes
            TxtTotalProductos.Text = $"Total: {stats["TotalProductos"]}";
            TxtProductosActivos.Text = $"Activos: {stats["ProductosActivos"]}";
            TxtTotalStock.Text = $"Stock: {stats["TotalStock"]}";
            TxtCategorias.Text = $"Categorías: {stats["Categorias"]}";
        }
    }

    // ============================================================
    // EVENTO: BUSCADOR (filtrar productos al escribir)
    // ============================================================
    
    /// <summary>
    /// Se ejecuta cuando el usuario escribe en el cuadro de búsqueda.
    /// Filtra los productos que coinciden con el texto escrito.
    /// </summary>
    private void TxtBuscar_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Obtener el texto escrito por el usuario
        var texto = TxtBuscar.Text;
        
        // Si está vacío, cargar todos los productos
        if (string.IsNullOrWhiteSpace(texto))
        {
            CargarProductos();
            return;
        }

        // Llamar al método de búsqueda del servicio
        var resultado = _servicio.Search(texto);
        
        if (resultado.IsSuccess)
        {
            // Actualizar el DataGrid con los resultados
            _productos.Clear();
            foreach (var producto in resultado.Value)
            {
                _productos.Add(producto);
            }
        }
    }

    /// <summary>
    /// Se ejecuta al seleccionar una categoría del ComboBox.
    /// Filtra los productos por la categoría seleccionada.
    /// </summary>
    private void CmbCategoria_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Verificar que hay un elemento seleccionado
        if (CmbCategoria.SelectedItem is ComboBoxItem item)
        {
            // Obtener el texto de la categoría
            var categoria = item.Content?.ToString();
            
            // Si es "Todas" o está vacío, cargar todos los productos
            if (string.IsNullOrWhiteSpace(categoria) || categoria == "Todas")
            {
                CargarProductos();
                return;
            }

            // Filtrar por categoría
            var resultado = _servicio.GetByCategoria(categoria);
            
            if (resultado.IsSuccess)
            {
                _productos.Clear();
                foreach (var producto in resultado.Value)
                {
                    _productos.Add(producto);
                }
            }
        }
    }

    // ============================================================
    // EVENTO: ACTUALIZAR DATOS
    // ============================================================
    
    /// <summary>
    /// Limpia el buscador y recarga todos los productos y categorías.
    /// </summary>
    private void BtnActualizar_Click(object sender, RoutedEventArgs e)
    {
        TxtBuscar.Text = "";
        CargarProductos();
        CargarCategorias();
    }

    // ============================================================
    // EVENTO: SELECCIÓN EN EL DATAGRID
    // ============================================================
    
    /// <summary>
    /// Se ejecuta cuando el usuario selecciona un producto en el DataGrid.
    /// Habilita/deshabilita los botones según si hay selección.
    /// </summary>
    private void GridProductos_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Obtener el producto seleccionado (puede ser null)
        _productoSeleccionado = GridProductos.SelectedItem as Producto;
        
        // Verificar si hay selección
        var haySeleccion = _productoSeleccionado != null;
        
        // Habilitar/deshabilitar botones según la selección
        BtnEditar.IsEnabled = haySeleccion;
        BtnEliminar.IsEnabled = haySeleccion;
        
        // Los botones de activar/desactivar dependen del estado actual del producto
        BtnActivar.IsEnabled = haySeleccion && _productoSeleccionado!.Activo == false;
        BtnDesactivar.IsEnabled = haySeleccion && _productoSeleccionado!.Activo;
    }

    // ============================================================
    // EVENTO: AÑADIR NUEVO PRODUCTO
    // ============================================================
    
    /// <summary>
    /// Abre el diálogo para añadir un nuevo producto.
    /// </summary>
    private void BtnAñadir_Click(object sender, RoutedEventArgs e)
    {
        // Crear el diálogo para añadir producto
        var dialog = new ProductoDialog();
        dialog.Owner = this;
        
        // Mostrar el diálogo y verificar si el usuario pulsó "Aceptar"
        if (dialog.ShowDialog() == true)
        {
            // Llamar al servicio para crear el producto
            // El servicio validará el producto y devolverá Result<T, DomainError>
            var resultado = _servicio.Create(
                dialog.Nombre,
                dialog.Descripcion,
                dialog.Categoria,
                dialog.Precio,
                dialog.Stock
            );

            if (resultado.IsSuccess)
            {
                // Recargar productos y categorías
                CargarProductos();
                CargarCategorias();
                MessageBox.Show("Producto creado correctamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Mostrar el error de validación
                MostrarError(resultado.Error.Message);
            }
        }
    }

    // ============================================================
    // EVENTO: EDITAR PRODUCTO
    // ============================================================
    
    /// <summary>
    /// Abre el diálogo para editar el producto seleccionado.
    /// Solicita confirmación antes de guardar los cambios.
    /// </summary>
    private void BtnEditar_Click(object sender, RoutedEventArgs e)
    {
        // Verificar que hay un producto seleccionado
        if (_productoSeleccionado == null) return;

        // Crear el diálogo pasando el producto a editar
        var dialog = new ProductoDialog(_productoSeleccionado);
        dialog.Owner = this;
        
        // Mostrar el diálogo
        if (dialog.ShowDialog() == true)
        {
            // Solicitar confirmación antes de guardar
            var confirmacion = MessageBox.Show(
                $"¿Guardar los cambios del producto '{_productoSeleccionado.Nombre}'?",
                "Confirmar",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );
            
            // Si el usuario no confirma, salir
            if (confirmacion != MessageBoxResult.Yes) return;
            
            // Llamar al servicio para actualizar el producto
            var resultado = _servicio.Update(
                _productoSeleccionado.Id,
                dialog.Nombre,
                dialog.Descripcion,
                dialog.Categoria,
                dialog.Precio,
                dialog.Stock,
                _productoSeleccionado.Activo  // Mantener el estado activo actual
            );

            if (resultado.IsSuccess)
            {
                CargarProductos();
                MessageBox.Show("Producto actualizado correctamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MostrarError(resultado.Error.Message);
            }
        }
    }

    // ============================================================
    // EVENTO: ELIMINAR PRODUCTO
    // ============================================================
    
    /// <summary>
    /// Elimina el producto seleccionado (borrado lógico).
    /// Solicita confirmación antes de eliminar.
    /// </summary>
    private void BtnEliminar_Click(object sender, RoutedEventArgs e)
    {
        // Verificar que hay un producto seleccionado
        if (_productoSeleccionado == null) return;

        // Mostrar diálogo de confirmación
        var resultado = MessageBox.Show(
            $"¿Eliminar el producto '{_productoSeleccionado.Nombre}'?",
            "Confirmar",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question
        );

        // Si el usuario confirma, eliminar el producto
        if (resultado == MessageBoxResult.Yes)
        {
            // Llamar al servicio para eliminar (borrado lógico)
            var result = _servicio.Delete(_productoSeleccionado.Id);
            
            if (result.IsSuccess)
            {
                CargarProductos();
                MessageBox.Show("Producto eliminado correctamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MostrarError(result.Error.Message);
            }
        }
    }

    // ============================================================
    // EVENTO: ACTIVAR PRODUCTO
    // ============================================================
    
    /// <summary>
    /// Activa un producto previamente desactivado.
    /// Solicita confirmación antes de activar.
    /// </summary>
    private void BtnActivar_Click(object sender, RoutedEventArgs e)
    {
        // Verificar que hay un producto seleccionado
        if (_productoSeleccionado == null) return;

        // Solicitar confirmación
        var confirmacion = MessageBox.Show(
            $"¿Activar el producto '{_productoSeleccionado.Nombre}'?",
            "Confirmar",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question
        );
        
        // Si no confirma, salir
        if (confirmacion != MessageBoxResult.Yes) return;

        // Llamar al servicio para activar (Update con Activo = true)
        var resultado = _servicio.Update(
            _productoSeleccionado.Id,
            _productoSeleccionado.Nombre,
            _productoSeleccionado.Descripcion,
            _productoSeleccionado.Categoria,
            _productoSeleccionado.Precio,
            _productoSeleccionado.Stock,
            true  // Activar
        );

        if (resultado.IsSuccess)
        {
            CargarProductos();
            MessageBox.Show("Producto activado", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            MostrarError(resultado.Error.Message);
        }
    }

    // ============================================================
    // EVENTO: DESACTIVAR PRODUCTO
    // ============================================================
    
    /// <summary>
    /// Desactiva un producto (borrado lógico).
    /// Solicita confirmación antes de desactivar.
    /// </summary>
    private void BtnDesactivar_Click(object sender, RoutedEventArgs e)
    {
        // Verificar que hay un producto seleccionado
        if (_productoSeleccionado == null) return;

        // Solicitar confirmación
        var confirmacion = MessageBox.Show(
            $"¿Desactivar el producto '{_productoSeleccionado.Nombre}'?",
            "Confirmar",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question
        );
        
        // Si no confirma, salir
        if (confirmacion != MessageBoxResult.Yes) return;

        // Llamar al servicio para desactivar (Update con Activo = false)
        var resultado = _servicio.Update(
            _productoSeleccionado.Id,
            _productoSeleccionado.Nombre,
            _productoSeleccionado.Descripcion,
            _productoSeleccionado.Categoria,
            _productoSeleccionado.Precio,
            _productoSeleccionado.Stock,
            false  // Desactivar
        );

        if (resultado.IsSuccess)
        {
            CargarProductos();
            MessageBox.Show("Producto desactivado", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            MostrarError(resultado.Error.Message);
        }
    }

    // ============================================================
    // MÉTODO: MOSTRAR MENSAJE DE ERROR
    // ============================================================
    
    /// <summary>
    /// Muestra un MessageBox con el mensaje de error especificado.
    /// </summary>
    private void MostrarError(string mensaje)
    {
        MessageBox.Show(mensaje, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    // ============================================================
    // MENÚ: ACTUALIZAR
    // ============================================================
    
    /// <summary>
    /// Actualiza los datos desde el menú.
    /// </summary>
    private void MenuActualizar_Click(object sender, RoutedEventArgs e)
    {
        TxtBuscar.Text = "";
        CargarProductos();
        CargarCategorias();
    }

    // ============================================================
    // MENÚ: SALIR
    // ============================================================
    
    /// <summary>
    /// Cierra la aplicación.
    /// </summary>
    private void MenuSalir_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    // ============================================================
    // MENÚ: ACERCA DE
    // ============================================================
    
    /// <summary>
    /// Muestra información sobre la aplicación.
    /// </summary>
    private void MenuAcercaDe_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
            "Gestión de Productos v1.0\n\n" +
            "Aplicación de ejemplo para UD10\n" +
            "Interfaces Gráficas con .NET\n\n" +
            "Desarrollado por: DAW",
            "Acerca de",
            MessageBoxButton.OK,
            MessageBoxImage.Information
        );
    }

    // ============================================================
    // RESUMEN: ARQUITECTURA DE LA APLICACIÓN
    // ============================================================
//
// +------------------+
// |    App.xaml.cs  |  <- Punto de entrada de la aplicación
// +--------+---------+
//          |
// +--------v---------+
// |DependenciesProvider| <- Registra las dependencias (servicios, repositorios)
// +--------+---------+
//          |
// +--------v---------+
// | App.ServiceProvider| <- Proporciona las dependencias registradas
// +--------+---------+
//          |
// +--------v---------+
// |  MainWindow     |  <- Obtiene servicios de App.ServiceProvider
// +--------+---------+
//          |
// +--------v---------+
// |  ProductoService |  <- Lógica de negocio, usa IValidador<Producto>
// +--------+---------+
//          |
// +--------v---------+
// | IProductoRepository| <- Interfaz del repositorio
// +--------+---------+
//          |
// +--------v---------+
// | ProductoRepository|  <- Implementación con SQLite
// +--------+---------+
//          |
// +--------v---------+
// |   SQLite DB     |  <- Base de datos SQLite en archivo
// +------------------+
//
// FLUJO DE DATOS (Create/Update/Delete):
// 1. MainWindow llama a _servicio.Create/Update/Delete()
// 2. ProductoService valida el producto con IValidador<Producto>
// 3. Si la validación falla, devuelve Result.Failure<DomainError>
// 4. Si la validación pasa, llama al repositorio
// 5. El repositorio ejecuta la operación en SQLite
// 6. El servicio devuelve Result.Success o Result.Failure
// 7. MainWindow muestra el resultado al usuario
//
// TIPOS DE INYECCIÓN DE DEPENDENCIAS:
// - AddSingleton: Una sola instancia para toda la aplicación
// - AddTransient: Nueva instancia cada vez que se solicita
// - AddScoped: Una instancia por ámbito (no usado en WPF)

    // ============================================================
    // EVENTO: CAMBIAR TEMA (CLARO/OSCURO)
    // ============================================================
    
    /// <summary>
    /// Se ejecuta cuando el usuario togglea el botón de tema.
    /// Cambia dinámicamente entre tema claro y oscuro.
    /// </summary>
    private void ThemeToggle_Changed(object sender, RoutedEventArgs e)
    {
        // Cambiar el tema
        ThemeHelper.ToggleTheme();
        
        // Actualizar el icono según el tema
        if (ThemeHelper.IsDarkTheme)
        {
            ThemeIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.WeatherNight;
        }
        else
        {
            ThemeIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.WeatherSunny;
        }
    }
}
