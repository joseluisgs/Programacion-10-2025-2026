// ============================================================
// MainViewModel.cs - ViewModel con FormData
// ============================================================
// ViewModel usando CommunityToolkit.Mvvm para alumnos de DAW.
// Demuestra reactividad automática entre FormData y Comandos.
// =================================================================

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

public partial class MainViewModel : ObservableObject
{
    private readonly IProductoService _productoService;
    private readonly IBackupService _backupService;
    private readonly ILogger _logger = Log.ForContext<MainViewModel>();

    // --- Propiedades Reactivas (UI) ---
    
    [ObservableProperty] private ObservableCollection<Producto> _productos = [];
    [ObservableProperty] private string _busqueda = "";
    [ObservableProperty] private string _total = "Total: 0.00 €";
    [ObservableProperty] private string _mensaje = "";
    [ObservableProperty] private bool _hayError;
    [ObservableProperty] private Producto? _productoSeleccionado;
    
    [ObservableProperty] private ProductoFormData _formData;
    
    // --- Constructor ---
    
    public MainViewModel(IProductoService productoService, IBackupService backupService)
    {
        _productoService = productoService;
        _backupService = backupService;
        
        // Al asignar a la propiedad, se activa OnFormDataChanged
        FormData = new(); 
    }

    // --- Reactividad Automática (Suscripción a cambios en FormData) ---

    partial void OnFormDataChanged(ProductoFormData? oldValue, ProductoFormData newValue)
    {
        // Limpiamos suscripción anterior si existía
        if (oldValue != null) oldValue.PropertyChanged -= OnFormDataPropertyChanged;
        
        // Nos suscribimos al nuevo para avisar al comando "Añadir"
        if (newValue != null) newValue.PropertyChanged += OnFormDataPropertyChanged;
        
        AñadirCommand?.NotifyCanExecuteChanged();
    }

    private void OnFormDataPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // ¡Magia! Si cambia cualquier propiedad del FormData, avisamos al comando
        AñadirCommand.NotifyCanExecuteChanged();
    }

    // --- Lógica de Búsqueda ---
    
    partial void OnBusquedaChanged(string value) => Buscar();

    private void Buscar()
    {
        var lista = string.IsNullOrWhiteSpace(Busqueda) 
            ? _productoService.GetAll() 
            : _productoService.Buscar(Busqueda);
        
        Productos = new ObservableCollection<Producto>(lista);
        ActualizarTotal();
    }

    // --- Comandos de Acción ---

    [RelayCommand]
    private void LimpiarBusqueda() => Busqueda = "";

    [RelayCommand]
    private void MarcarComprado(Producto? producto)
    {
        if (producto == null) return;

        var resultado = _productoService.MarcarComprado(producto.Id, producto.EstaComprado);
        
        resultado.Match(
            onSuccess: _ => ActualizarTotal(),
            onFailure: error => {
                producto.EstaComprado = !producto.EstaComprado; // Revertir visualmente
                MostrarError(error.Message);
            }
        );
    }

    [RelayCommand]
    private void Eliminar(Producto? producto)
    {
        if (producto == null) return;

        _productoService.Delete(producto.Id).Match(
            onSuccess: _ => {
                Productos.Remove(producto);
                MostrarExito("Producto eliminado");
                ActualizarTotal();
            },
            onFailure: error => MostrarError(error.Message)
        );
    }

    // --- Lógica del Formulario (Añadir) ---

    [RelayCommand(CanExecute = nameof(CanAñadir))]
    private void Añadir()
    {
        _productoService.Add(
            FormData.Nombre.Trim(), 
            FormData.GetCantidad(), 
            FormData.GetPrecio()
        ).Match(
            onSuccess: p => {
                Productos.Add(p);
                FormData = new(); // Resetear formulario
                MostrarExito("Producto añadido");
                ActualizarTotal();
            },
            onFailure: error => MostrarError(error.Message)
        );
    }
    
    private bool CanAñadir() => FormData != null && FormData.IsValid();

    [RelayCommand]
    private void LimpiarFormulario()
    {
        FormData = new();
        Mensaje = "Formulario limpio";
        HayError = false;
    }

    // --- Utilidades ---

    public void CargarProductos()
    {
        Productos = new ObservableCollection<Producto>(_productoService.GetAll());
        ActualizarTotal();
    }

    private void ActualizarTotal()
    {
        Total = $"Total: {Productos.Sum(p => p.Total):N2} €";
    }

    private void MostrarError(string msg)
    {
        _logger.Warning(msg);
        Mensaje = msg;
        HayError = true;
        MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private void MostrarExito(string msg)
    {
        _logger.Information(msg);
        Mensaje = msg;
        HayError = false;
    }

    // --- Importación / Exportación ---

    [RelayCommand]
    private void Exportar()
    {
        var dialog = new SaveFileDialog { Filter = "JSON|*.json|CSV|*.csv" };
        if (dialog.ShowDialog() == true)
        {
            _backupService.Exportar(Productos, dialog.FileName)
                .Match(
                    onSuccess: _ => MostrarExito("Exportado con éxito"),
                    onFailure: error => MostrarError(error.Message)
                );
        }
    }

    [RelayCommand]
    private void Importar()
    {
        var dialog = new OpenFileDialog { Filter = "JSON|*.json|CSV|*.csv" };
        if (dialog.ShowDialog() == true)
        {
            var resultado = _backupService.Importar(dialog.FileName);
            
            resultado.Match(
                onSuccess: productos => {
                    foreach (var p in productos)
                    {
                        _productoService.Add(p.Nombre, p.Cantidad, p.Precio)
                            .Match(
                                onSuccess: nuevo => Productos.Add(nuevo),
                                onFailure: _ => { /* Ignorar errores individuales */ }
                            );
                    }
                    CargarProductos();
                    MostrarExito("Importación completada");
                },
                onFailure: error => MostrarError(error.Message)
            );
        }
    }

    [RelayCommand]
    private void Cerrar() => Application.Current.Shutdown();
}
