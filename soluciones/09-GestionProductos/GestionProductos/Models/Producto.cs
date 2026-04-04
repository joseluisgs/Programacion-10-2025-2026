// ============================================================
// Producto.cs - Modelo de Producto
// ============================================================
// Entidad que representa un producto en el sistema.
//
// PROPIEDADES:
// - Id: Identificador único
// - Nombre: Nombre del producto
// - Descripcion: Descripción del producto
// - Categoria: Categoría del producto
// - Precio: Precio unitario
// - Stock: Cantidad en inventario
// - Activo: Si el producto está activo (para borrado lógico)
// - FechaCreacion: Fecha de creación del registro
// - FechaActualizacion: Fecha de última modificación

namespace GestionProductos.Models;

/// <summary>
/// Entidad que representa un producto.
/// </summary>
public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public string Categoria { get; set; } = "";
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public DateTime FechaActualizacion { get; set; } = DateTime.Now;

    /// <summary>
    /// Valor total del producto en inventario.
    /// </summary>
    public decimal ValorTotal => Precio * Stock;

    /// <summary>
    /// Constructor con parámetros.
    /// </summary>
    public Producto(int id, string nombre, string descripcion, string categoria, 
                   decimal precio, int stock, bool activo = true)
    {
        Id = id;
        Nombre = nombre;
        Descripcion = descripcion;
        Categoria = categoria;
        Precio = precio;
        Stock = stock;
        Activo = activo;
    }

    /// <summary>
    /// Constructor por defecto.
    /// </summary>
    public Producto() { }

    /// <summary>
    /// Representación en string del producto.
    /// </summary>
    public override string ToString() => 
        $"{Nombre} ({Categoria}) - {Precio:C2} (Stock: {Stock})";
}