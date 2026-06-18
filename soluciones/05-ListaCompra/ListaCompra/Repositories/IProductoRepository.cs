// ============================================================
// IProductoRepository.cs - Interfaz del repositorio de productos
// ============================================================
// Interfaz específica para el repositorio de productos.
//
// CONCEPTOS IMPORTANTES:
//
// 1. HERENCIA DE ICrudRepository:
//    - IProductoRepository hereda de ICrudRepository<int, Producto>
//    - Hereda todos los métodos CRUD básicos
//    - Añade métodos específicos de productos
//
// 2. MÉTODOS ESPECÍFICOS:
//    - GetByNombre(string): Búsqueda por nombre (parcial, insensitive)
//    - Permite buscar productos sin tener que traer todos y filtrar
//
// 3. IMPLEMENTACIÓN:
//    - Véase ProductoRepository.cs

using System.Collections.Generic;
using ListaCompra.Models;

namespace ListaCompra.Repositories;

/// <summary>
/// Interfaz para el repositorio de productos.
/// Hereda de ICrudRepository y añade métodos específicos.
/// </summary>
public interface IProductoRepository : ICrudRepository<int, Producto>
{
    /// <summary>
    /// Busca productos cuyo nombre contenga el texto especificado.
    /// Búsqueda parcial e insensitive (no distingue mayúsculas).
    /// </summary>
    /// <param name="nombre">Texto a buscar.</param>
    /// <returns>Enumerable de productos que coinciden.</returns>
    IEnumerable<Producto> GetByNombre(string nombre);
}