// ============================================================
// ProductoRepository.cs - Implementación del repositorio en memoria
// ============================================================
// Implementación del repositorio usando un diccionario en memoria.
//
// CONCEPTOS IMPORTANTES:
//
// 1. REPOSITORIO EN MEMORIA:
//    - Usa un Dictionary<int, Producto> para almacenar los datos
//    - No persiste datos (se pierde al cerrar la aplicación)
//    - Ideal para ejemplos y prototipos
//    - En producción se usaría una base de datos
//
// 2. ESTRUCTURA DE DATOS:
//    - Dictionary<TKey, TValue>: Colección clave-valor optimizada
//    - Clave: ID del producto (int)
//    - Valor: Objeto Producto
//    - Búsqueda O(1) por ID
//
// 3. OPERACIONES:
//    - GetAll: Devuelve todos los valores del diccionario
//    - GetById: TryGetValue para evitar excepciones
//    - GetByNombre: Filtrado con LINQ (búsqueda parcial)
//    - Create: Genera ID automático, añade al diccionario
//    - Update: Modifica el valor asociado a la clave
//    - Delete: Remove del diccionario
//
// 4. DATOS DE EJEMPLO:
//    - El constructor inicializa con algunos productos de ejemplo
//    - Para probar la aplicación nada más ejecutarla

using System.Collections.Generic;
using System.Linq;
using ListaCompra.Models;

namespace ListaCompra.Repositories;

/// <summary>
/// Implementación del repositorio de productos en memoria.
/// Usa un Dictionary para almacenar los productos.
/// </summary>
public class ProductoRepository : IProductoRepository
{
    // ============================================================
    // ATRIBUTOS PRIVADOS
    // ============================================================
    
    // Diccionario para almacenar los productos (ID -> Producto)
    // Dictionary es más eficiente que List para búsquedas por ID
    private readonly Dictionary<int, Producto> _productos = [];
    
    // Contador para generar IDs automáticos secuenciales
    private int _siguienteId = 1;

    // Constructor: inicializa con algunos datos de ejemplo
    public ProductoRepository()
    {
        // Añadir productos de ejemplo al iniciar (para probar la UI)
        // El ID 0 se reemplaza con ID automático en Create()
        Create(new Producto(0, "Leche", 2, 1.50m));
        Create(new Producto(0, "Pan", 1, 0.80m));
        Create(new Producto(0, "Huevos", 12, 2.20m));
        Create(new Producto(0, "Manzanas", 5, 1.00m));
    }

    /// <summary>
    /// Obtiene todos los productos.
    /// </summary>
    public IEnumerable<Producto> GetAll()
    {
        // .Values devuelve todos los valores del diccionario
        // .ToList() crea una lista para evitar enumeración múltiple
        return _productos.Values.ToList();
    }

    /// <summary>
    /// Obtiene un producto por su ID.
    /// </summary>
    public Producto? GetById(int id)
    {
        // TryGetValue: devuelve true si existe y el valor en 'producto'
        // Evita excepción si no existe (más eficiente que _productos[id])
        return _productos.TryGetValue(id, out var producto) ? producto : null;
    }

    /// <summary>
    /// Busca productos por nombre (búsqueda parcial, case-insensitive).
    /// </summary>
    public IEnumerable<Producto> GetByNombre(string nombre)
    {
        // Convertir a minúsculas para comparación case-insensitive
        var nombreLower = nombre.ToLower();
        
        // Filtrar productos cuyo nombre contenga el texto (Contains)
        // .ToList() para materialize el resultado
        return _productos.Values
            .Where(p => p.Nombre.ToLower().Contains(nombreLower))
            .ToList();
    }

    /// <summary>
    /// Añade un nuevo producto.
    /// </summary>
    public Producto? Create(Producto producto)
    {
        // Generar ID automático secuencial
        producto.Id = _siguienteId++;
        
        // Añadir al diccionario con el ID como clave
        _productos[producto.Id] = producto;
        
        // Devolver el producto (ahora tiene ID asignado)
        return producto;
    }

    /// <summary>
    /// Actualiza un producto existente.
    /// </summary>
    public Producto? Update(int id, Producto producto)
    {
        // Verificar que existe antes de actualizar
        if (!_productos.ContainsKey(id))
            return null;
        
        // Actualizar el ID del producto y sustituir en el diccionario
        producto.Id = id;
        _productos[id] = producto;
        
        return producto;
    }

    /// <summary>
    /// Elimina un producto.
    /// </summary>
    public Producto? Delete(int id, bool logical = true)
    {
        // TryGetValue para obtener y luego Remove
        if (_productos.TryGetValue(id, out var producto))
        {
            _productos.Remove(id);
            return producto;
        }
        return null;
    }
}