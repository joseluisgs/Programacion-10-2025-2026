// ============================================================
// ProductoFormData.cs - FormData con validación IDataErrorInfo
// ============================================================
//
// =================================================================
// GUÍA PARA EL ALUMNO: PATRÓN FORMDATA CON IDATAERRORINFO
// =================================================================
//
// Este proyecto demuestra cómo añadir validación con IDataErrorInfo
// al proyecto existente de Lista de la Compra.
//
// CONCEPTOS:
// 1. FormData: DTO que contiene campos + validación
// 2. IDataErrorInfo: Validación en tiempo real
// 3. ValidatesOnDataErrors en XAML
//
// =================================================================

using CommunityToolkit.Mvvm.ComponentModel;

namespace ListaCompra.FormData;

/// <summary>
/// FormData para validar productos de la lista de la compra.
/// </summary>
public partial class ProductoFormData : ObservableObject, System.ComponentModel.IDataErrorInfo
{
    // =================================================================
    // PROPIEDADES DEL FORMULARIO
    // =================================================================
    
    [ObservableProperty]
    private string _nombre = "";
    
    [ObservableProperty]
    private string _cantidad = "1";
    
    [ObservableProperty]
    private string _precio = "0";
    
    // =================================================================
    // IMPLEMENTACIÓN DE IDATAERRORINFO
    // =================================================================
    
    /// <summary>
    /// Indexador: devuelve el error de una propiedad específica.
    /// </summary>
    public string this[string columnName]
    {
        get
        {
            return columnName switch
            {
                // VALIDACIÓN NOMBRE
                nameof(Nombre) when string.IsNullOrWhiteSpace(Nombre)
                    => "El nombre es obligatorio.",
                nameof(Nombre) when Nombre.Trim().Length < 2
                    => "El nombre debe tener al menos 2 caracteres.",
                nameof(Nombre) when Nombre.Length > 100
                    => "El nombre no puede exceder 100 caracteres.",
                
                // VALIDACIÓN CANTIDAD
                nameof(Cantidad) when string.IsNullOrWhiteSpace(Cantidad)
                    => "La cantidad es obligatoria.",
                nameof(Cantidad) when !int.TryParse(Cantidad, out var cant) || cant <= 0
                    => "La cantidad debe ser un número mayor que 0.",
                nameof(Cantidad) when int.TryParse(Cantidad, out var c) && c > 1000
                    => "La cantidad no puede exceder 1000.",
                
                // VALIDACIÓN PRECIO
                nameof(Precio) when string.IsNullOrWhiteSpace(Precio)
                    => "El precio es obligatorio.",
                nameof(Precio) when !decimal.TryParse(Precio, out var prec) || prec < 0
                    => "El precio debe ser un número positivo.",
                nameof(Precio) when decimal.TryParse(Precio, out var p) && p > 99999.99m
                    => "El precio no puede exceder 99999.99 €.",
                
                _ => string.Empty
            };
        }
    }
    
    /// <summary>
    /// Error general (raramente usado en WPF).
    /// </summary>
    public string Error => string.Empty;
    
    // =================================================================
    // MÉTODO DE VALIDACIÓN GLOBAL
    // =================================================================
    
    /// <summary>
    /// Valida todo el formulario y devuelve true si es válido.
    /// </summary>
    public bool IsValid()
    {
        return string.IsNullOrEmpty(this[nameof(Nombre)]) &&
               string.IsNullOrEmpty(this[nameof(Cantidad)]) &&
               string.IsNullOrEmpty(this[nameof(Precio)]);
    }
    
    // =================================================================
    // PROPIEDADES CONVERTIDAS (para uso en el servicio)
    // =================================================================
    
    /// <summary>
    /// Obtiene la cantidad como entero.
    /// </summary>
    public int GetCantidad()
    {
        return int.TryParse(Cantidad, out var c) ? c : 1;
    }
    
    /// <summary>
    /// Obtiene el precio como decimal.
    /// </summary>
    public decimal GetPrecio()
    {
        return decimal.TryParse(Precio, out var p) ? p : 0;
    }
    
    // =================================================================
    // MÉTODO DE MAPEO
    // =================================================================
    
    /// <summary>
    /// Crea un FormData desde un modelo existente (para editar).
    /// </summary>
    public static ProductoFormData FromModel(ListaCompra.Models.Producto producto)
    {
        return new ProductoFormData
        {
            Nombre = producto.Nombre,
            Cantidad = producto.Cantidad.ToString(),
            Precio = producto.Precio.ToString()
        };
    }
}