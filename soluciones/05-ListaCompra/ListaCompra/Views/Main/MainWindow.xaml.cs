// ============================================================
// MainWindow.xaml.cs - Ventana principal de la Lista de la Compra
// ============================================================
// Este ejercicio introduce los siguientes conceptos:
//
// 1. PATRÓN REPOSITORY: Separación de la capa de datos
//    - ICrudRepository<TKey, TEntity>: Interfaz genérica para operaciones CRUD
//    - IProductoRepository: Interfaz específica para productos
//    - ProductoRepository: Implementación en memoria (Dictionary)
//
// 2. VALIDADOR: Validación de entidades
//    - IValidador<T>: Interfaz genérica para validadores
//    - ValidadorProducto: Implementación con reglas de validación
//
// 3. SERVICE: Lógica de negocio
//    - IProductoService: Interfaz del servicio
//    - ProductoService: Implementación con validación y excepciones
//
// 4. INYECCIÓN DE DEPENDENCIAS (DI): Como en UD09
//    - DependenciesProvider: Registra todas las dependencias
//    - App.ServiceProvider: Proporciona las dependencias globalmente
//    - GetRequiredService<T>: Obtiene una instancia del servicio
//
// 5. EXCEPCIONES PERSONALIZADAS
//    - ListaCompraException.NotFound: Producto no encontrado
//    - ListaCompraException.Validation: Error de validación
//    - ListaCompraException.AlreadyExists: Producto ya existe
//
// 6. BINDING: Enlazar datos a la UI
//    - ObservableCollection<T>: Notifica cambios a la UI automáticamente
//    - ItemsSource: Fuente de datos del ListBox
//    - DataTemplate: Plantilla para cada item
//    - IValueConverter: Convertidor para el binding
//
// 7. BUSCADOR: Filtrar datos en tiempo real
//    - TextChanged: Evento que se dispara al escribir
//    - Buscar(): Método que filtra por nombre
//
// 8. CRUD: Crear, Leer, Actualizar, Eliminar productos
//    - Add(): Añadir nuevo producto
//    - GetAll(): Obtener todos los productos
//    - MarcarComprado(): Actualizar estado
//    - Delete(): Eliminar producto

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Extensions.DependencyInjection;
using ListaCompra.Exceptions;
using ListaCompra.Models;
using ListaCompra.Services;

namespace ListaCompra.Views.Main;

// ============================================================
// CONVERTIDORES PARA EL BINDING
// ============================================================
// Los convertidores permiten transformar datos durante el binding.
// Se usan cuando el tipo de dato no coincide con lo que espera la UI.

/// <summary>
/// Convertidor que aplica tachado al texto cuando el producto está comprado.
/// Binding: bool EstaComprado -> TextDecoration Strikethrough
/// </summary>
public class BoolToTextDecorationConverter : IValueConverter
{
    /// <summary>
    /// Convierte un valor booleano en una decoración de texto.
    /// </summary>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Si está comprado, devolvemos tachado (Strikethrough)
        if (value is bool estaComprado && estaComprado)
            return TextDecorations.Strikethrough;
        
        // Si no está comprado, devolvemos null (sin decoración)
        return null!;
    }

    /// <summary>
    /// No implementamos la conversión inversa (no se usa en este caso).
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>
/// Convertidor que muestra/oculta un elemento según la longitud del texto.
/// Binding: int longitud -> Visibility
/// </summary>
public class IntToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Convierte la longitud del texto en visibilidad.
    /// Si hay texto (> 0), oculta el elemento. Si no hay texto, lo muestra.
    /// </summary>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int length)
            // Si hay texto, ocultamos (Collapsed). Si no hay texto, mostramos (Visible)
            return length > 0 ? Visibility.Collapsed : Visibility.Visible;
        
        return Visibility.Visible;
    }

    /// <summary>
    /// No implementamos la conversión inversa (no se usa en este caso).
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

// ============================================================
// MAINWINDOW - VENTANA PRINCIPAL
// ============================================================

/// <summary>
/// Ventana principal de la aplicación Lista de la Compra.
/// Gestiona la lista de productos con operaciones CRUD.
/// </summary>
public partial class MainWindow : Window
{
    // ============================================================
    // ATRIBUTOS PRIVADOS
    // ============================================================
    
    /// <summary>
    /// ObservableCollection: Notifica a la UI automáticamente cuando se añaden,
    /// eliminan o modifican elementos. Es ideal para el binding con ListBox.
    /// </summary>
    private readonly ObservableCollection<Producto> _productos = [];
    
    /// <summary>
    /// Servicio de productos que contiene la lógica de negocio.
    /// Se obtiene del ServiceProvider de App mediante inyección de dependencias.
    /// </summary>
    private readonly IProductoService _servicio;

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
        Resources["BoolToTextDecoration"] = new BoolToTextDecorationConverter();
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
        
        // Configurar el ItemsSource para el binding con el ListBox
        ListaProductos.ItemsSource = _productos;
        
        // Cargar los productos iniciales desde el repositorio
        CargarProductos();
    }

    // ============================================================
    // MÉTODO: CARGAR PRODUCTOS DESDE EL REPOSITORIO
    // ============================================================
    
    /// <summary>
    /// Carga todos los productos del repositorio y los muestra en la lista.
    /// </summary>
    private void CargarProductos()
    {
        try
        {
            // Obtener todos los productos del servicio (que usa el repositorio)
            var productos = _servicio.GetAll();
            
            // Limpiar la colección actual
            _productos.Clear();
            
            // Añadir todos los productos a la ObservableCollection
            // Esto notificará automáticamente al ListBox para actualizar la UI
            foreach (var producto in productos)
            {
                _productos.Add(producto);
            }
            
            // Actualizar el total de la compra
            ActualizarTotal();
        }
        catch (Exception ex)
        {
            // Mostrar mensaje de error si falla la carga
            MostrarError($"Error al cargar productos: {ex.Message}");
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
        try
        {
            // Obtener el texto escrito por el usuario
            var texto = TxtBuscar.Text;
            
            // Llamar al método de búsqueda del servicio
            var productos = _servicio.Buscar(texto);
            
            // Actualizar la lista con los resultados
            _productos.Clear();
            foreach (var producto in productos)
            {
                _productos.Add(producto);
            }
            
            // Actualizar el total (solo cuenta los productos visibles)
            ActualizarTotal();
        }
        catch (Exception ex)
        {
            MostrarError($"Error al buscar: {ex.Message}");
        }
    }

    /// <summary>
    /// Limpia el texto de búsqueda y muestra todos los productos.
    /// </summary>
    private void BtnLimpiarBusqueda_Click(object sender, RoutedEventArgs e)
    {
        TxtBuscar.Text = "";
    }

    // ============================================================
    // EVENTO: AÑADIR NUEVO PRODUCTO
    // ============================================================
    
    /// <summary>
    /// Añade un nuevo producto a la lista de la compra.
    /// Valida los datos del formulario antes de añadir.
    /// </summary>
    private void BtnAñadir_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Obtener el nombre del TextBox y eliminar espacios en blanco
            var nombre = TxtNombre.Text.Trim();
            
            // Validar cantidad: debe ser un número mayor que 0
            if (!int.TryParse(TxtCantidad.Text, out var cantidad) || cantidad <= 0)
            {
                MostrarError("La cantidad debe ser un número mayor que 0.");
                return;
            }
            
            // Validar precio: debe ser un número positivo
            if (!decimal.TryParse(TxtPrecio.Text, out var precio) || precio < 0)
            {
                MostrarError("El precio debe ser un número positivo.");
                return;
            }
            
            // Llamar al servicio para añadir el producto
            // El servicio validará el producto y throwará si hay errores
            var producto = _servicio.Add(nombre, cantidad, precio);
            
            if (producto != null)
            {
                // Añadir a la ObservableCollection (notifica a la UI automáticamente)
                _productos.Add(producto);
                
                // Limpiar los campos del formulario
                TxtNombre.Text = "";
                TxtCantidad.Text = "1";
                TxtPrecio.Text = "0";
                
                // Actualizar el total
                ActualizarTotal();
            }
        }
        catch (ListaCompraException.Validation ex)
        {
            // Capturar errores de validación del validador
            // Unimos todos los errores en un solo mensaje
            var mensaje = string.Join("\n", ex.Errores);
            MostrarError(mensaje);
        }
        catch (Exception ex)
        {
            MostrarError($"Error al añadir: {ex.Message}");
        }
    }

    // ============================================================
    // EVENTO: ELIMINAR PRODUCTO
    // ============================================================
    
    /// <summary>
    /// Elimina el producto seleccionado de la lista.
    /// Solicita confirmación antes de eliminar.
    /// </summary>
    private void BtnEliminar_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Obtener el ID del producto desde el Tag del botón
            if (sender is Button btn && btn.Tag is int id)
            {
                // Mostrar diálogo de confirmación
                var resultado = MessageBox.Show(
                    "¿Eliminar este producto de la lista?",
                    "Confirmar",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );
                
                // Si el usuario confirma, eliminar el producto
                if (resultado == MessageBoxResult.Yes)
                {
                    // Llamar al servicio para eliminar
                    if (_servicio.Delete(id))
                    {
                        // Buscar el producto en la colección local
                        var producto = _productos.FirstOrDefault(p => p.Id == id);
                        if (producto != null)
                        {
                            // Eliminar de la ObservableCollection
                            _productos.Remove(producto);
                            ActualizarTotal();
                        }
                    }
                }
            }
        }
        catch (ListaCompraException.NotFound ex)
        {
            // Capturar error si el producto no existe
            MostrarError(ex.Message);
            CargarProductos(); // Recargar para sincronizar
        }
        catch (Exception ex)
        {
            MostrarError($"Error al eliminar: {ex.Message}");
        }
    }

    // ============================================================
    // EVENTO: MARCAR PRODUCTO COMPRADO
    // ============================================================
    
    /// <summary>
    /// Se ejecuta cuando el usuario marca/desmarca el CheckBox.
    /// Actualiza el estado de comprado del producto.
    /// </summary>
    private void CheckBox_Changed(object sender, RoutedEventArgs e)
    {
        try
        {
            // Obtener el CheckBox que desencadenó el evento
            // y el producto asociado (DataContext)
            if (sender is CheckBox chk && chk.DataContext is Producto producto)
            {
                // Llamar al servicio para actualizar el estado
                _servicio.MarcarComprado(producto.Id, chk.IsChecked == true);
                
                // Actualizar el total (los comprados no cuentan)
                ActualizarTotal();
            }
        }
        catch (ListaCompraException.NotFound ex)
        {
            MostrarError(ex.Message);
            CargarProductos();
        }
        catch (Exception ex)
        {
            MostrarError($"Error al actualizar: {ex.Message}");
        }
    }

    /// <summary>
    /// Evento que se dispara al seleccionar un producto en la lista.
    /// Actualmente no se utiliza (reservado para uso futuro).
    /// </summary>
    private void ListaProductos_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Por implementar si necesitamos hacer algo al seleccionar
    }

    // ============================================================
    // MÉTODO: CALCULAR Y MOSTRAR EL TOTAL
    // ============================================================
    
    /// <summary>
    /// Calcula el total de la compra sumando los importes de todos los productos.
    /// Solo cuenta los productos NO comprados.
    /// </summary>
    private void ActualizarTotal()
    {
        // Sumar el total de cada producto (cantidad * precio)
        // Solo productos no comprados
        var total = _productos.Sum(p => p.Total);
        
        // Mostrar en el TextBlock con formato de moneda (C2 = 2 decimales)
        TxtTotal.Text = $"Total: {total:C2}";
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
    // EVENTO: CERRAR VENTANA
    // ============================================================
    
    /// <summary>
    /// Cierra la ventana principal, terminando la aplicación.
    /// </summary>
    private void BtnCerrar_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
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
// | ProductoRepository|  <- Implementación en memoria (Dictionary<int, Producto>)
// +------------------+
//
// TIPOS DE INYECCIÓN DE DEPENDENCIAS:
// - AddSingleton: Una sola instancia para toda la aplicación
// - AddTransient: Nueva instancia cada vez que se solicita
// - AddScoped: Una instancia por ámbito (no usado en WPF)
//
// FLUJO DE DATOS:
// 1. MainWindow llama a _servicio.GetAll()
// 2. ProductoService llama a _repositorio.GetAll()
// 3. ProductoRepository devuelve la lista de productos
// 4. El binding actualiza automáticamente el ListBox
