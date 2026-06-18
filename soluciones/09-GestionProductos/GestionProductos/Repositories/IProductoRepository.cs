// ============================================================
// IProductoRepository.cs - Interfaz del repositorio de productos
// ============================================================
// Hereda de ICrudRepository y añade métodos específicos.
//
// MÉTODOS CRUD (heredados):
// - GetAll(): Obtener todos
// - GetById(int id): Obtener por ID
// - Create(Producto): Crear
// - Update(int id, Producto): Actualizar
// - Delete(int id): Eliminar
//
// MÉTODOS ESPECÍFICOS:
// - GetByNombre(string): Buscar por nombre
// - GetByCategoria(string): Buscar por categoría
// - GetActivos(): Solo activos
// - Search(string): Búsqueda general

using System.Collections.Generic;
using GestionProductos.Models;

namespace GestionProductos.Repositories;

/// <summary>
/// Interfaz del repositorio de productos.
/// </summary>
public interface IProductoRepository : ICrudRepository<int, Producto>
{
    IEnumerable<Producto> GetByNombre(string nombre);
    IEnumerable<Producto> GetByCategoria(string categoria);
    IEnumerable<Producto> GetActivos();
    IEnumerable<Producto> Search(string criterio);
}