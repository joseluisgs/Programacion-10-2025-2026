using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ListasMenusTablas.Models;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxImage = System.Windows.MessageBoxImage;
using MessageBoxResult = System.Windows.MessageBoxResult;

namespace ListasMenusTablas.ViewModels;

/// <summary>
/// ViewModel principal de la aplicación de Listas, Menús y Tablas.
/// Gestiona los datos de productos, estudiantes y controles de interfaz.
/// </summary>
public partial class ListasViewModel : ObservableObject
{
    /// <summary>
    /// Constructor que inicializa los datos y las propiedades por defecto.
    /// </summary>
    public ListasViewModel()
    {
        CargarDatos();
        FechaActualDelSistema = DateTime.Today;
        FechaSeleccionada = DateTime.Today;
    }

    #region Colecciones de datos

    /// <summary>
    /// Colección completa de productos disponibles.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<Producto> _productos = new();

    /// <summary>
    /// Colección de productos filtrada según la categoría seleccionada.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<Producto> _productosFiltrados = new();

    /// <summary>
    /// Colección de estudiantes para mostrar en el DataGrid.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<Estudiante> _estudiantes = new();

    /// <summary>
    /// Lista de categorías disponibles para filtrar productos.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<string> _categorias = new();

    #endregion

    #region Propiedades de selección

    /// <summary>
    /// Categoría actualmente seleccionada en el ComboBox.
    /// </summary>
    [ObservableProperty]
    private string? _categoriaSeleccionada;

    /// <summary>
    /// Producto seleccionado en el ListView.
    /// </summary>
    [ObservableProperty]
    private Producto? _productoSeleccionado;

    /// <summary>
    /// Estudiante seleccionado en el DataGrid.
    /// </summary>
    [ObservableProperty]
    private Estudiante? _estudianteSeleccionado;

    /// <summary>
    /// Total de elementos mostrados en la lista filtrada.
    /// </summary>
    [ObservableProperty]
    private int _totalElementos;

    #endregion

    #region Propiedades de CheckBox (selección múltiple)

    /// <summary>
    /// Indica si la Opción A está marcada.
    /// </summary>
    [ObservableProperty]
    private bool _opcionA;

    /// <summary>
    /// Indica si la Opción B está marcada.
    /// </summary>
    [ObservableProperty]
    private bool _opcionB;

    /// <summary>
    /// Indica si la Opción C está marcada.
    /// </summary>
    [ObservableProperty]
    private bool _opcionC;

    /// <summary>
    /// Texto que muestra las opciones seleccionadas.
    /// Se actualiza automáticamente cuando cambia cualquier CheckBox.
    /// </summary>
    [ObservableProperty]
    private string _opcionesSeleccionadas = "Ninguna";

    #endregion

    #region Propiedades de RadioButton (selección única)

    /// <summary>
    /// Prioridad actualmente seleccionada.
    /// Solo puede haber una opción seleccionada a la vez.
    /// </summary>
    [ObservableProperty]
    private string _prioridadSeleccionada = "Media";

    #endregion

    #region Propiedades de Calendar

    /// <summary>
    /// Fecha actual del sistema (para mostrar en el calendario).
    /// </summary>
    [ObservableProperty]
    private DateTime _fechaActualDelSistema;

    /// <summary>
    /// Fecha seleccionada por el usuario en el calendario.
    /// </summary>
    [ObservableProperty]
    private DateTime? _fechaSeleccionada;

    #endregion

    #region Métodos de actualización

    /// <summary>
    /// Actualiza el texto de opciones seleccionadas cuando cambia algún CheckBox.
    /// Se invoca automáticamente mediante los métodos partial generados por CommunityToolkit.
    /// </summary>
    partial void OnOpcionAChanged(bool value) => ActualizarOpcionesSeleccionadas();
    partial void OnOpcionBChanged(bool value) => ActualizarOpcionesSeleccionadas();
    partial void OnOpcionCChanged(bool value) => ActualizarOpcionesSeleccionadas();

    /// <summary>
    /// Construye una cadena con las opciones marcadas.
    /// </summary>
    private void ActualizarOpcionesSeleccionadas()
    {
        var opciones = new List<string>();
        if (OpcionA) opciones.Add("A");
        if (OpcionB) opciones.Add("B");
        if (OpcionC) opciones.Add("C");
        
        OpcionesSeleccionadas = opciones.Count > 0 
            ? string.Join(", ", opciones) 
            : "Ninguna";
    }

    /// <summary>
    /// Se ejecuta cuando cambia la categoría seleccionada.
    /// Filtra los productos según la categoría.
    /// </summary>
    partial void OnCategoriaSeleccionadaChanged(string? value)
    {
        FiltrarProductos();
    }

    /// <summary>
    /// Se ejecuta cuando cambia la prioridad seleccionada.
    /// Muestra un mensaje informativo al usuario.
    /// </summary>
    partial void OnPrioridadSeleccionadaChanged(string value)
    {
        MessageBox.Show(
            $"Prioridad cambiada a: {value}", 
            "Cambio de Prioridad", 
            MessageBoxButton.OK, 
            MessageBoxImage.Information);
    }

    #endregion

    #region Comandos

    /// <summary>
    /// Establece la fecha seleccionada a la fecha actual del sistema.
    /// </summary>
    [RelayCommand]
    private void EstablecerFechaActual()
    {
        FechaSeleccionada = DateTime.Today;
        MessageBox.Show(
            $"Fecha establecida a: {FechaSeleccionada:dd/MM/yyyy}", 
            "Fecha Actual", 
            MessageBoxButton.OK, 
            MessageBoxImage.Information);
    }

    /// <summary>
    /// Procesa la fecha seleccionada mostrando información detallada.
    /// </summary>
    [RelayCommand]
    private void ProcesarFecha()
    {
        if (FechaSeleccionada == null)
        {
            MessageBox.Show(
                "Por favor, selecciona una fecha.", 
                "Procesar Fecha", 
                MessageBoxButton.OK, 
                MessageBoxImage.Warning);
            return;
        }

        var fecha = FechaSeleccionada.Value;
        var diaSemana = CultureInfo.CurrentUICulture.DateTimeFormat.GetDayName(fecha.DayOfWeek);
        
        var mensaje = $"Fecha seleccionada: {fecha:dd/MM/yyyy}\n" +
                      $"Día de la semana: {diaSemana}\n" +
                      $"Año: {fecha.Year}\n" +
                      $"Mes: {CultureInfo.CurrentUICulture.DateTimeFormat.GetMonthName(fecha.Month)}";

        MessageBox.Show(
            mensaje, 
            "Información de Fecha", 
            MessageBoxButton.OK, 
            MessageBoxImage.Information);
    }

    /// <summary>
    /// Comando para crear un nuevo elemento.
    /// </summary>
    [RelayCommand]
    private void Nuevo()
    {
        MessageBox.Show("Nuevo elemento", "Acción", 
            MessageBoxButton.OK, 
            MessageBoxImage.Information);
    }

    /// <summary>
    /// Comando para abrir un archivo.
    /// </summary>
    [RelayCommand]
    private void Abrir()
    {
        MessageBox.Show("Abrir archivo", "Acción", 
            MessageBoxButton.OK, 
            MessageBoxImage.Information);
    }

    /// <summary>
    /// Comando para guardar los cambios.
    /// </summary>
    [RelayCommand]
    private void Guardar()
    {
        MessageBox.Show("Guardar cambios", "Acción", 
            MessageBoxButton.OK, 
            MessageBoxImage.Information);
    }

    /// <summary>
    /// Comando para salir de la aplicación.
    /// </summary>
    [RelayCommand]
    private void Salir()
    {
        System.Windows.Application.Current.Shutdown();
    }

    /// <summary>
    /// Comando para mostrar la ayuda.
    /// </summary>
    [RelayCommand]
    private void VerAyuda()
    {
        MessageBox.Show("Ayuda de Listas, Menús y Tablas", "Ayuda", 
            MessageBoxButton.OK, 
            MessageBoxImage.Question);
    }

    /// <summary>
    /// Comando para editar un producto.
    /// </summary>
    [RelayCommand]
    private void EditarProducto(Producto? producto)
    {
        if (producto == null) return;
        MessageBox.Show($"Editar: {producto.Nombre}", "Editar Producto", 
            MessageBoxButton.OK, 
            MessageBoxImage.Information);
    }

    /// <summary>
    /// Comando para eliminar un producto con confirmación.
    /// </summary>
    [RelayCommand]
    private void EliminarProducto(Producto? producto)
    {
        if (producto == null) return;
        var resultado = MessageBox.Show(
            $"¿Eliminar {producto.Nombre}?", "Confirmar", 
            MessageBoxButton.YesNo, 
            MessageBoxImage.Question);
        
        if (resultado == MessageBoxResult.Yes)
        {
            Productos.Remove(producto);
            FiltrarProductos();
        }
    }

    /// <summary>
    /// Comando para ver los detalles de un estudiante.
    /// </summary>
    [RelayCommand]
    private void VerEstudiante(Estudiante? estudiante)
    {
        if (estudiante == null) return;
        MessageBox.Show(
            $"Ver detalles de {estudiante.Nombre} {estudiante.Apellidos}", 
            "Ver Estudiante", 
            MessageBoxButton.OK, 
            MessageBoxImage.Information);
    }

    /// <summary>
    /// Comando para editar un estudiante.
    /// </summary>
    [RelayCommand]
    private void EditarEstudiante(Estudiante? estudiante)
    {
        if (estudiante == null) return;
        MessageBox.Show(
            $"Editar: {estudiante.Nombre} {estudiante.Apellidos}", 
            "Editar Estudiante", 
            MessageBoxButton.OK, 
            MessageBoxImage.Information);
    }

    #endregion

    #region Métodos privados

    /// <summary>
    /// Carga los datos iniciales: productos, estudiantes y categorías.
    /// </summary>
    private void CargarDatos()
    {
        Categorias = new ObservableCollection<string>
        {
            "Todas",
            "Electrónica",
            "Alimentación",
            "Ropa",
            "Hogar"
        };
        CategoriaSeleccionada = "Todas";

        Productos = new ObservableCollection<Producto>
        {
            new() { Id = 1, Nombre = "Portátil", Precio = 899.99m, Categoria = "Electrónica" },
            new() { Id = 2, Nombre = "Móvil", Precio = 499.99m, Categoria = "Electrónica" },
            new() { Id = 3, Nombre = "Tablet", Precio = 349.99m, Categoria = "Electrónica" },
            new() { Id = 4, Nombre = "Pan", Precio = 1.50m, Categoria = "Alimentación" },
            new() { Id = 5, Nombre = "Leche", Precio = 1.20m, Categoria = "Alimentación" },
            new() { Id = 6, Nombre = "Camisa", Precio = 29.99m, Categoria = "Ropa" },
            new() { Id = 7, Nombre = "Pantalón", Precio = 49.99m, Categoria = "Ropa" },
            new() { Id = 8, Nombre = "Lámpara", Precio = 25.00m, Categoria = "Hogar" },
            new() { Id = 9, Nombre = "Silla", Precio = 79.99m, Categoria = "Hogar" }
        };

        Estudiantes = new ObservableCollection<Estudiante>
        {
            new() { Id = 1, Nombre = "Ana", Apellidos = "García López", Edad = 20, Activo = true, NotaMedia = 8.5 },
            new() { Id = 2, Nombre = "Carlos", Apellidos = "Martínez Sánchez", Edad = 22, Activo = true, NotaMedia = 7.2 },
            new() { Id = 3, Nombre = "María", Apellidos = "Rodríguez Pérez", Edad = 19, Activo = false, NotaMedia = 6.8 },
            new() { Id = 4, Nombre = "Juan", Apellidos = "Fernández Ruiz", Edad = 21, Activo = true, NotaMedia = 9.1 },
            new() { Id = 5, Nombre = "Laura", Apellidos = "López González", Edad = 23, Activo = true, NotaMedia = 7.9 }
        };

        FiltrarProductos();
        ActualizarContador();
    }

    /// <summary>
    /// Filtra los productos según la categoría seleccionada.
    /// </summary>
    private void FiltrarProductos()
    {
        if (string.IsNullOrEmpty(CategoriaSeleccionada) || CategoriaSeleccionada == "Todas")
        {
            ProductosFiltrados = new ObservableCollection<Producto>(Productos);
        }
        else
        {
            ProductosFiltrados = new ObservableCollection<Producto>(
                Productos.Where(p => p.Categoria == CategoriaSeleccionada));
        }
        ActualizarContador();
    }

    /// <summary>
    /// Actualiza el contador de elementos mostrados.
    /// </summary>
    private void ActualizarContador()
    {
        TotalElementos = ProductosFiltrados.Count;
    }

    #endregion
}
