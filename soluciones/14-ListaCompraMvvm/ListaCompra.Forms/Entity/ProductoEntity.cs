// ============================================================
// ProductoEntity.cs - Entidad para la base de datos
// ============================================================
// Representa la tabla Productos en SQLite.
//
// CONCEPTOS IMPORTANTES:
//
// 1. ENTITY vs MODEL:
//    - Entity: Representa la tabla en BD
//    - Model: Representa el objeto del dominio
//    - Mapper convierte entre ambos
//
// 2. PROPIEDADES:
//    - Coinciden con columnas de la tabla
//    - Tipos compatibles con SQLite
//    - CreatedAt/UpdatedAt para auditoría

using System;

namespace ListaCompra.Entity;

public class ProductoEntity
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public int Cantidad { get; set; }
    public decimal Precio { get; set; }
    public bool EstaComprado { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
