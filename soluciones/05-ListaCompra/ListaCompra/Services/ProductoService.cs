// ============================================================
// ProductoService.cs - Implementación del servicio de productos
// ============================================================
// Implementación del servicio con validación de negocio.
//
// CONCEPTOS IMPORTANTES:
//
// 1. SERVICE - LÓGICA DE NEGOCIO:
//    - Coordina el repositorio y el validador
//    - Valida los datos antes de cualquier operación de escritura
//    - Lanza excepciones específicas del dominio
//    - Es el intermediario entre la UI y los datos
//
// 2. INYECCIÓN DE DEPENDENCIAS:
//    - Recibe el repositorio y el validador por constructor
//    - El ServiceProvider los proporciona automáticamente
//    - Esto permite cambiar implementaciones sin cambiar el código
//
// 3. EXCEPCIONES DEL DOMINIO:
//    - ListaCompraException.NotFound: Producto no encontrado
//    - ListaCompraException.Validation: Errores de validación
//    - ListaCompraException.AlreadyExists: Producto duplicado
//
// 4. OPERACIONES:
//    - GetAll/GetById: Delegan al repositorio directamente
//    - Buscar: Lógica específica (si texto vacío, devuelve todos)
//    - Add/Update: Validan primero, luego delegan al repositorio
//    - Delete: Verifica que existe, luego elimina
//    - MarcarComprado: Crea nuevo producto con estado actualizado

using System.Collections.Generic;
using System.Linq;
using ListaCompra.Exceptions;
using ListaCompra.Models;
using ListaCompra.Repositories;
using ListaCompra.Validators;

namespace ListaCompra.Services;

/// <summary>
/// Implementación del servicio de productos.
/// Gestiona la lógica de negocio y la validación de productos.
/// </summary>
public class ProductoService : IProductoService
{
    // ============================================================
    // ATRIBUTOS (inyectados por constructor)
    // ============================================================
    
    /// <summary>
    /// Repositorio para el acceso a datos.
    /// </summary>
    private readonly IProductoRepository _repository;
    
    /// <summary>
    /// Validador para validar productos.
    /// </summary>
    private readonly IValidador<Producto> _validador;

    /// <summary>
    /// Constructor que recibe el repositorio y el validador por inyección de dependencias.
    /// El ServiceProvider crea automáticamente estas instancias.
    /// </summary>
    /// <param name="repository">Repositorio de productos.</param>
    /// <param name="validador">Validador de productos.</param>
    public ProductoService(IProductoRepository repository, IValidador<Producto> validador)
    {
        _repository = repository;
        _validador = validador;
    }

    /// <summary>
    /// Obtiene todos los productos.
    /// </summary>
    public IEnumerable<Producto> GetAll() => _repository.GetAll();

    /// <summary>
    /// Obtiene un producto por ID.
    /// </summary>
    public Producto? GetById(int id) => _repository.GetById(id);

    /// <summary>
    /// Busca productos por nombre (búsqueda parcial, insensitive).
    /// </summary>
    public IEnumerable<Producto> Buscar(string? nombre)
    {
        // Si no hay texto de búsqueda, devolver todos los productos
        if (string.IsNullOrWhiteSpace(nombre))
            return _repository.GetAll();
        
        // Llamar al método de búsqueda del repositorio
        return _repository.GetByNombre(nombre);
    }

    /// <summary>
    /// Añade un nuevo producto con validación.
    /// </summary>
    public Producto? Add(string nombre, int cantidad, decimal precio)
    {
        // Crear el producto con los datos proporcionados
        var producto = new Producto(0, nombre.Trim(), cantidad, precio);
        
        // ============================================================
        // VALIDACIÓN USANDO EL VALIDADOR
        // ============================================================
        // Obtener la lista de errores de validación
        var errores = _validador.Validar(producto).ToList();
        
        // Si hay errores, lanzar excepción de validación
        // La UI capturará esta excepción y mostrará los errores
        if (errores.Count > 0)
            throw new ListaCompraException.Validation(errores);
        
        // Si la validación pasa, guardar en el repositorio
        return _repository.Create(producto);
    }

    /// <summary>
    /// Actualiza un producto existente con validación.
    /// </summary>
    public Producto? Update(int id, string nombre, int cantidad, decimal precio, bool comprado)
    {
        // Verificar que el producto existe
        var existente = _repository.GetById(id);
        if (existente == null)
            throw new ListaCompraException.NotFound(id);
        
        // Crear producto actualizado con los nuevos datos
        var producto = new Producto(id, nombre.Trim(), cantidad, precio, comprado);
        
        // ============================================================
        // VALIDACIÓN USANDO EL VALIDADOR
        // ============================================================
        var errores = _validador.Validar(producto).ToList();
        
        // Si hay errores, lanzar excepción
        if (errores.Count > 0)
            throw new ListaCompraException.Validation(errores);
        
        // Actualizar en el repositorio
        return _repository.Update(id, producto);
    }

    /// <summary>
    /// Elimina un producto.
    /// </summary>
    public bool Delete(int id)
    {
        // Verificar que el producto existe antes de eliminar
        var existente = _repository.GetById(id);
        if (existente == null)
            throw new ListaCompraException.NotFound(id);
        
        // Eliminar del repositorio (logical = false -> borrado físico)
        var resultado = _repository.Delete(id, false);
        
        // Devolver true si se eliminó correctamente
        return resultado != null;
    }

    /// <summary>
    /// Marca un producto como comprado o no comprado.
    /// </summary>
    public Producto? MarcarComprado(int id, bool comprado)
    {
        // Verificar que existe
        var existente = _repository.GetById(id);
        if (existente == null)
            throw new ListaCompraException.NotFound(id);
        
        // Crear nuevo producto con el mismo contenido pero diferente estado
        // Devolvemos un nuevo objeto en lugar de modificar el existente
        // (principio de inmutabilidad)
        return _repository.Update(id, new Producto(
            existente.Id, 
            existente.Nombre, 
            existente.Cantidad, 
            existente.Precio, 
            comprado
        ));
    }
}