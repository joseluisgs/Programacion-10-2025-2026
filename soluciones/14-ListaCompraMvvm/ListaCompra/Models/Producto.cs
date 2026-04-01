using System;

namespace ListaCompra.Models;

public record Producto(
    int Id,
    string Nombre,
    int Cantidad,
    decimal Precio,
    bool EstaComprado = false
)
{
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    
    public decimal Total => Cantidad * Precio;
    
    public override string ToString() => 
        $"{Nombre} (x{Cantidad}) - {Precio:C2} = {Total:C2}";
}
