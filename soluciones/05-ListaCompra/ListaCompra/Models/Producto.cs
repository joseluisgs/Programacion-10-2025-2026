// ============================================================
// Producto.cs - Modelo de la entidad Producto
// ============================================================
// Representa un producto en la lista de la compra.
//
// CONCEPTOS IMPORTANTES:
//
// 1. MODELO (ENTITY):
//    - Representa un objeto del dominio
//    - Contiene solo datos (sin lógica de negocio)
//    - También llamada "Entidad" o "Domain Model"
//
// 2. PROPIEDADES:
//    - Id: Identificador único (clave primaria)
//    - Nombre: Nombre del producto
//    - Cantidad: Cantidad a comprar
//    - Precio: Precio unitario
//    - EstaComprado: Estado (comprado o no)
//    - Total: Propiedad calculada (solo lectura)
//
// 3. CONSTRUCTOR PRIMARIO (C# 14):
//    - Sintaxis simplificada para inicializar propiedades
//    - public Producto(int id, string nombre, int cantidad, decimal precio)
//    - Las propiedades se inicializan automáticamente
//
// 4. PROPIEDADES CALCULADAS:
//    - Total: No es un campo, se calcula (Cantidad * Precio)
//    - Se recalcula automáticamente cada vez que se accede
//    - No tiene setter, solo getter

namespace ListaCompra.Models;

/// <summary>
/// Clase que representa un producto en la lista de la compra.
/// </summary>
public class Producto
{
    /// <summary>
    /// Constructor primario de C# 14.
    /// Inicializa las propiedades con los valores proporcionados.
    /// </summary>
    /// <param name="id">Identificador único (0 para nuevos productos).</param>
    /// <param name="nombre">Nombre del producto.</param>
    /// <param name="cantidad">Cantidad a comprar.</param>
    /// <param name="precio">Precio unitario.</param>
    /// <param name="estaComprado">Indica si está comprado (default: false).</param>
    public Producto(int id, string nombre, int cantidad, decimal precio, bool estaComprado = false)
    {
        Id = id;
        Nombre = nombre;
        Cantidad = cantidad;
        Precio = precio;
        EstaComprado = estaComprado;
    }

    /// <summary>
    /// Identificador único del producto.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Nombre del producto (obligatorio).
    /// </summary>
    public string Nombre { get; set; }
    
    /// <summary>
    /// Cantidad a comprar.
    /// </summary>
    public int Cantidad { get; set; }
    
    /// <summary>
    /// Precio unitario.
    /// </summary>
    public decimal Precio { get; set; }
    
    /// <summary>
    /// Indica si el producto ya ha sido comprado.
    /// </summary>
    public bool EstaComprado { get; set; }
    
    /// <summary>
    /// Propiedad calculada: precio total (cantidad * precio).
    /// No es un campo, se calcula automáticamente.
    /// </summary>
    public decimal Total => Cantidad * Precio;
    
    /// <summary>
    /// Representación en texto del producto.
    /// </summary>
    public override string ToString() => 
        $"{Nombre} (x{Cantidad}) - {Precio:C2} = {Total:C2}";
}