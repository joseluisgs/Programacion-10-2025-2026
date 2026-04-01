// ============================================================
// IProductoService.cs - Interfaz del servicio de productos
// ============================================================
// Interfaz para el servicio de productos.
//
// CONCEPTOS IMPORTANTES:
//
// 1. SERVICE (SERVICIO):
//    - Capa intermedia entre la UI (ventana) y los datos (repositorio)
//    - Encapsula la lógica de negocio
//    - Valida los datos antes de guardarlos
//    - Maneja excepciones del dominio
//
// 2. OPERACIONES DEFINIDAS:
//    - GetAll(): Obtener todos los productos
//    - GetById(): Obtener un producto por ID
//    - Buscar(): Buscar productos por nombre (búsqueda parcial)
//    - Add(): Añadir nuevo producto (con validación)
//    - Update(): Actualizar producto existente (con validación)
//    - Delete(): Eliminar producto
//    - MarcarComprado(): Cambiar estado de comprado
//
// 3. USO EN LA APLICACIÓN:
//    - MainWindow usa IProductoService para todas las operaciones
//    - El servicio valida antes de llamar al repositorio
//    - Lanza excepciones específicas si hay errores
//
// 4. IMPLEMENTACIÓN:
//    - Véase ProductoService.cs

using System.Collections.Generic;
using ListaCompra.Models;

namespace ListaCompra.Services;

/// <summary>
/// Interfaz para el servicio de productos.
/// Define todas las operaciones de negocio disponibles.
/// </summary>
public interface IProductoService
{
    /// <summary>
    /// Obtiene todos los productos de la lista.
    /// </summary>
    /// <returns>Enumerable de productos.</returns>
    IEnumerable<Producto> GetAll();

    /// <summary>
    /// Obtiene un producto por su identificador.
    /// </summary>
    /// <param name="id">Identificador del producto.</param>
    /// <returns>Producto encontrado o null si no existe.</returns>
    Producto? GetById(int id);

    /// <summary>
    /// Busca productos cuyo nombre contenga el texto especificado.
    /// Búsqueda parcial e insensitive (no distingue mayúsculas).
    /// </summary>
    /// <param name="nombre">Texto a buscar en el nombre.</param>
    /// <returns>Enumerable de productos que coinciden.</returns>
    IEnumerable<Producto> Buscar(string? nombre);

    /// <summary>
    /// Añade un nuevo producto a la lista.
    /// Valida el producto antes de guardarlo.
    /// </summary>
    /// <param name="nombre">Nombre del producto.</param>
    /// <param name="cantidad">Cantidad a comprar.</param>
    /// <param name="precio">Precio unitario.</param>
    /// <returns>Producto creado o null si hay errores de validación.</returns>
    Producto? Add(string nombre, int cantidad, decimal precio);

    /// <summary>
    /// Actualiza un producto existente.
    /// Valida el producto antes de actualizarlo.
    /// </summary>
    /// <param name="id">ID del producto a actualizar.</param>
    /// <param name="nombre">Nuevo nombre.</param>
    /// <param name="cantidad">Nueva cantidad.</param>
    /// <param name="precio">Nuevo precio.</param>
    /// <param name="comprado">Nuevo estado de comprado.</param>
    /// <returns>Producto actualizado o null si no existe.</returns>
    Producto? Update(int id, string nombre, int cantidad, decimal precio, bool comprado);

    /// <summary>
    /// Elimina un producto de la lista.
    /// </summary>
    /// <param name="id">ID del producto a eliminar.</param>
    /// <returns>True si se eliminó, False si no existía.</returns>
    bool Delete(int id);

    /// <summary>
    /// Marca un producto como comprado o no comprado.
    /// </summary>
    /// <param name="id">ID del producto.</param>
    /// <param name="comprado">True si está comprado, False si no.</param>
    /// <returns>Producto actualizado o null si no existe.</returns>
    Producto? MarcarComprado(int id, bool comprado);
}