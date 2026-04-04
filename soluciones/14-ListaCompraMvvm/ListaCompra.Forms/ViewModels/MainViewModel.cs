// ============================================================
// MainViewModel.cs - ViewModel con FormData
// ============================================================
// ViewModel usando CommunityToolkit.Mvvm con inyección de dependencias.
// Ahora usa FormData para validación con IDataErrorInfo.
//
// CONCEPTOS NUEVOS EN ESTE PROYECTO:
//
// 1. FORMDATA + IDATAERRORINFO:
//    - ProductoFormData contiene los campos del formulario
//    - Implementa IDataErrorInfo para validación en tiempo real
//    - El binding usa ValidatesOnDataErrors=True en XAML
//
// 2. VALIDACIÓN EN TIEMPO REAL:
//    - UpdateSourceTrigger=PropertyChanged para validar mientras escribe
//    - El botón Añadir se habilita solo si FormData.IsValid() = true
//    - Mensajes de error específicos por campo
//
// 3. SEPARACIÓN DE RESPONSABILIDADES:
//    - FormData: datos del formulario + validación
//    - ViewModel: lógica de presentación + comandos
//    - Service: lógica de negocio
//
// =================================================================

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpFunctionalExtensions;
using ListaCompra.FormData;
using ListaCompra.Models;
using ListaCompra.Services;
using Microsoft.Win32;
using Serilog;

namespace ListaCompra.FormData.ViewModels;

public partial class MainViewModel(IProductoService productoService, IBackupService backupService) : ObservableObject
{
    private readonly ILogger _logger = Log.ForContext<MainViewModel>();
    private readonly IProductoService _productoService = productoService;
    private readonly IBackupService _backupService = backupService;

    // =================================================================
    // PROPIEDADES ORIGINALES (para la lista y búsqueda)
    // =================================================================
    
    [ObservableProperty]
    private ObservableCollection<Producto> _productos = [];

    [ObservableProperty]
    private string _busqueda = "";

    [ObservableProperty]
    private string _total = "Total: 0.00 €";

    [ObservableProperty]
    private string _mensaje = "";

    [ObservableProperty]
    private bool _hayError;

    [ObservableProperty]
    private Producto? _productoSeleccionado;
    
    // =================================================================
    // NUEVO: PROPIEDAD FORMDATA (para el formulario de añadir)
    // =================================================================
    
    [ObservableProperty]
    private ProductoFormData _formData = new();
    
    // =================================================================
    // CONSTRUCTOR
    // =================================================================
    
    public MainViewModel() : this(null!, null!)
    {
    }

    partial void OnBusquedaChanged(string value)
    {
        Buscar();
    }

    private void CargarProductos()
    {
        var lista = _productoService.GetAll();
        Productos = new ObservableCollection<Producto>(lista);
        ActualizarTotal();
    }

    private void Buscar()
    {
        var lista = string.IsNullOrWhiteSpace(Busqueda) 
            ? _productoService.GetAll() 
            : _productoService.Buscar(Busqueda);
        Productos = new ObservableCollection<Producto>(lista);
        ActualizarTotal();
    }

    private void ActualizarTotal()
    {
        var total = Productos.Sum(p => p.Total);
        Total = $"Total: {total:C2}";
    }

    private void MostrarError(string mensaje)
    {
        _logger.Warning("Error: {mensaje}", mensaje);
        Mensaje = mensaje;
        HayError = true;
        MessageBox.Show(mensaje, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private void MostrarExito(string mensaje)
    {
        _logger.Information("Éxito: {mensaje}", mensaje);
        Mensaje = mensaje;
        HayError = false;
    }

    // =================================================================
    // COMANDOS
    // =================================================================

    [RelayCommand]
    private void BuscarCommand()
    {
        Buscar();
    }

    [RelayCommand]
    private void LimpiarBusqueda()
    {
        Busqueda = "";
    }

    /// <summary>
    /// Comando para añadir producto.
    /// Usa FormData para validación.
    /// CanExecute usa FormData.IsValid() para habilitar/deshabilitar.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanAñadir))]
    private void Añadir()
    {
        // Doble verificación por seguridad
        if (!FormData.IsValid())
        {
            MostrarError("Por favor, corrige los errores del formulario.");
            return;
        }

        var nombre = FormData.Nombre.Trim();
        var cantidad = FormData.GetCantidad();
        var precio = FormData.GetPrecio();

        var resultado = _productoService.Add(nombre, cantidad, precio);
        
        resultado.Match(
            onSuccess: producto =>
            {
                Productos.Add(producto);
                FormData = new ProductoFormData(); // Limpiar formulario
                MostrarExito("Producto añadido");
                ActualizarTotal();
            },
            onFailure: error =>
            {
                MostrarError(error.Message);
            }
        );
    }
    
    /// <summary>
    /// Determina si el comando Añadir puede ejecutarse.
    /// Se ejecuta cada vez que cambia alguna propiedad del FormData.
    /// </summary>
    private bool CanAñadir()
    {
        return FormData.IsValid();
    }

    /// <summary>
    /// Limpia el formulario de añadir.
    /// </summary>
    [RelayCommand]
    private void LimpiarFormulario()
    {
        FormData = new ProductoFormData();
        Mensaje = "Formulario limpiado.";
        HayError = false;
    }

    [RelayCommand]
    private void Eliminar()
    {
        if (ProductoSeleccionado == null) return;

        var id = ProductoSeleccionado.Id;
        var resultado = _productoService.Delete(id);
        
        resultado.Match(
            onSuccess: _ =>
            {
                var producto = Productos.FirstOrDefault(p => p.Id == id);
                if (producto != null)
                {
                    Productos.Remove(producto);
                    MostrarExito("Producto eliminado");
                    ActualizarTotal();
                }
            },
            onFailure: error =>
            {
                MostrarError(error.Message);
            }
        );
    }

    [RelayCommand]
    private void MarcarComprado(Producto producto)
    {
        if (producto == null) return;

        var nuevoEstado = !producto.EstaComprado;
        var resultado = _productoService.MarcarComprado(producto.Id, nuevoEstado);
        
        resultado.Match(
            onSuccess: _ =>
            {
                var idx = Productos.IndexOf(producto);
                if (idx >= 0)
                    Productos[idx] = producto with { EstaComprado = nuevoEstado };
                ActualizarTotal();
            },
            onFailure: error =>
            {
                MostrarError(error.Message);
            }
        );
    }

    [RelayCommand]
    private void Exportar()
    {
        var dialog = new SaveFileDialog
        {
            Filter = "JSON Files (*.json)|*.json|CSV Files (*.csv)|*.csv",
            DefaultExt = ".json",
            Title = "Exportar productos"
        };

        if (dialog.ShowDialog() == true)
        {
            var resultado = _backupService.Exportar(Productos, dialog.FileName);
            
            resultado.Match(
                onSuccess: _ =>
                {
                    MostrarExito($"Productos exportados a {dialog.FileName}");
                },
                onFailure: error =>
                {
                    MostrarError(error.Message);
                }
            );
        }
    }

    [RelayCommand]
    private void Importar()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "JSON Files (*.json)|*.json|CSV Files (*.csv)|*.csv",
            DefaultExt = ".json",
            Title = "Importar productos"
        };

        if (dialog.ShowDialog() == true)
        {
            var resultado = _backupService.Importar(dialog.FileName);
            
            resultado.Match(
                onSuccess: productos =>
                {
                    var importados = 0;
                    var errores = 0;
                    foreach (var producto in productos)
                    {
                        var addResult = _productoService.Add(
                            producto.Nombre, 
                            producto.Cantidad, 
                            producto.Precio
                        );
                        
                        addResult.Match(
                            onSuccess: p => { Productos.Add(p); importados++; },
                            onFailure: _ => { errores++; }
                        );
                    }
                    var msg = $"Productos importados desde {dialog.FileName}";
                    if (errores > 0)
                        msg += $"\nImportados: {importados}, Errores: {errores}";
                    MostrarExito(msg);
                    ActualizarTotal();
                },
                onFailure: error =>
                {
                    MostrarError(error.Message);
                }
            );
        }
    }

    [RelayCommand]
    private void Cerrar()
    {
        System.Windows.Application.Current.Shutdown();
    }
}