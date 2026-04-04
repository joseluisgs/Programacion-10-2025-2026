// ============================================================
// ProductoDto.cs - Data Transfer Object para productos
// ============================================================
// DTO usado para serialización en JSON y CSV.
// Elimina propiedades como CreatedAt/UpdatedAt que no necesitamos en el backup.

namespace ListaCompra.Dto;

public record ProductoDto(
    int Id,
    string Nombre,
    int Cantidad,
    decimal Precio,
    bool EstaComprado
);
