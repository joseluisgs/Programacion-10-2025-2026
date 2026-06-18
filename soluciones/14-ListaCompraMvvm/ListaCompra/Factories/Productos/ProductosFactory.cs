// ============================================================
// ProductosFactory.cs - Factory para datos de semilla
// ============================================================
// Proporciona datos iniciales de ejemplo para la aplicación.
//
// CONCEPTOS IMPORTANTES:
//
// 1. FACTORY:
//    - Patrón de diseño creacional
//    - Método estático Seed() devuelve colección
//    - Datos de ejemplo para probar la app
//
// 2. SEED DATA:
//    - Se carga al iniciar la aplicación
//    - Controlled por appsettings.json
//    - SeedData: true = carga datos
//    - DropData: true = borra antes de cargar

using System.Collections.Generic;
using ListaCompra.Models;

namespace ListaCompra.Factories.Productos;

public static class ProductosFactory
{
    public static IEnumerable<Producto> Seed()
    {
        return new List<Producto>
        {
            new(0, "Leche", 2, 1.50m),
            new(0, "Pan", 1, 0.90m),
            new(0, "Huevos", 12, 2.80m),
            new(0, "Manzanas", 6, 3.50m),
            new(0, "Plátanos", 4, 1.80m),
            new(0, "Carne molida", 1, 5.50m),
            new(0, "Pechuga de pollo", 2, 7.00m),
            new(0, "Pasta", 3, 1.20m),
            new(0, "Arroz", 2, 1.80m),
            new(0, "Tomates", 4, 2.50m),
            new(0, "Cebollas", 3, 1.00m),
            new(0, "Patatas", 5, 2.20m),
            new(0, "Zanahorias", 4, 1.50m),
            new(0, "Aceite de oliva", 1, 5.00m),
            new(0, "Sal", 1, 0.80m),
            new(0, "Café", 2, 4.50m),
            new(0, "Azúcar", 1, 1.50m),
            new(0, "Harina", 1, 1.20m),
            new(0, "Yogures", 6, 2.00m),
            new(0, "Queso", 1, 4.50m, true)
        };
    }
}
