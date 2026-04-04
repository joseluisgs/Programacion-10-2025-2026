// ============================================================
// MainViewModel.cs - ViewModel principal de la aplicación
// ============================================================
// ViewModel usando CommunityToolkit.Mvvm con inyección de dependencias.
// Maneja los comandos de la UI y procesa resultados con ROP.
//
// CONCEPTOS IMPORTANTES:
//
// 1. MVVM (Model-View-ViewModel):
//    - Patrón de arquitectura para WPF
//    - Separa la lógica de la interfaz
//    - ViewModel expone propiedades y comandos
//
// 2. COMMUNITYTOOLKIT.MVVM:
//    - ObservableObject: Notifica cambios a la UI
//    - ObservableProperty: Genera propiedades automáticamente
//    - RelayCommand: Genera comandos automáticamente
//    - Primary Constructor: Simplifica inyección
//
// 3. INYECCIÓN DE DEPENDENCIAS:
//    - ViewModel recibe IProductoService por constructor
//    - ViewModel recibe IBackupService por constructor
//    - Se registra en DependenciesProvider
//    - Se obtiene en MainWindow.xaml.cs
//
// 4. RAILWAY ORIENTED PROGRAMMING:
//    - Los comandos usan result.Match()
//    - onSuccess: Actualiza UI y muestra mensaje de éxito
//    - onFailure: Muestra MessageBox con error
//    - MessageBox.Show() para errores graves
//
// 5. IMPORT/EXPORT:
//    - Usa Microsoft.Win32.FileDialog
//    - Exportar: SaveFileDialog con filtro JSON/CSV
//    - Importar: OpenFileDialog con filtro JSON/CSV
//    - BackupService decide el storage por extensión

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpFunctionalExtensions;
using ListaCompra.Models;
using ListaCompra.Services;
using Microsoft.Win32;
using Serilog;

namespace ListaCompra.ViewModels;

public partial class MainViewModel(IProductoService productoService, IBackupService backupService) : ObservableObject
{
    private readonly ILogger _logger = Log.ForContext<MainViewModel>();
    private readonly IProductoService _productoService = productoService;
    private readonly IBackupService _backupService = backupService;

    [ObservableProperty]
    private ObservableCollection<Producto> _productos = [];

    [ObservableProperty]
    private string _busqueda = "";

    [ObservableProperty]
    private string _nombre = "";

    [ObservableProperty]
    private string _cantidad = "1";

    [ObservableProperty]
    private string _precio = "0";

    [ObservableProperty]
    private string _total = "Total: 0.00 €";

    [ObservableProperty]
    private string _mensaje = "";

    [ObservableProperty]
    private bool _hayError;

    [ObservableProperty]
    private Producto? _productoSeleccionado;

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

    [RelayCommand]
    private void Añadir()
    {
        if (!int.TryParse(Cantidad, out var cantidad) || cantidad <= 0)
        {
            MostrarError("La cantidad debe ser mayor que 0");
            return;
        }
        
        if (!decimal.TryParse(Precio, out var precio) || precio < 0)
        {
            MostrarError("El precio debe ser positivo");
            return;
        }

        var resultado = _productoService.Add(Nombre.Trim(), cantidad, precio);
        
        resultado.Match(
            onSuccess: producto =>
            {
                Productos.Add(producto);
                Nombre = "";
                Cantidad = "1";
                Precio = "0";
                MostrarExito("Producto añadido");
                ActualizarTotal();
            },
            onFailure: error =>
            {
                MostrarError(error.Message);
            }
        );
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
