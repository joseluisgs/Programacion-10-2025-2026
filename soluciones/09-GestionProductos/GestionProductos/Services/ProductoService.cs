// ============================================================
// ProductoService.cs - Servicio de productos
// ============================================================
// Coordina el repositorio y el validador.
//
// RESPONSABILIDADES:
// - Validar datos antes de operaciones de escritura
// - Coordinar con el repositorio
// - Devolver Result<T, DomainError> (Railway Oriented Programming)

using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using GestionProductos.Errors;
using GestionProductos.Models;
using GestionProductos.Repositories;
using GestionProductos.Validators;

namespace GestionProductos.Services;

/// <summary>
/// Servicio de productos.
/// </summary>
public class ProductoService : IProductoService
{
    private readonly IProductoRepository _repository;
    private readonly IValidador<Producto> _validador;

    public ProductoService(IProductoRepository repository, IValidador<Producto> validador)
    {
        _repository = repository;
        _validador = validador;
    }

    /// <summary>
    /// Obtiene todos los productos.
    /// </summary>
    public Result<IEnumerable<Producto>, DomainError> GetAll()
    {
        return Result.Success<IEnumerable<Producto>, DomainError>(_repository.GetAll());
    }

    /// <summary>
    /// Obtiene un producto por ID.
    /// </summary>
    public Result<Producto, DomainError> GetById(int id)
    {
        var producto = _repository.GetById(id);
        if (producto == null)
            return Result.Failure<Producto, DomainError>(ProductoErrors.ProductoNoEncontrado(id));
        
        return Result.Success<Producto, DomainError>(producto);
    }

    /// <summary>
    /// Crea un nuevo producto.
    /// </summary>
    public Result<Producto, DomainError> Create(string nombre, string descripcion, string categoria, decimal precio, int stock)
    {
        var producto = new Producto(0, nombre.Trim(), descripcion.Trim(), categoria.Trim(), precio, stock);

        // Validar
        var validacion = _validador.Validar(producto);
        if (validacion.IsFailure)
            return Result.Failure<Producto, DomainError>(validacion.Error);

        // Crear en repositorio
        var creado = _repository.Create(producto);
        if (creado == null)
            return Result.Failure<Producto, DomainError>(new DatabaseError("Error al crear el producto"));
        
        return Result.Success<Producto, DomainError>(creado);
    }

    /// <summary>
    /// Actualiza un producto.
    /// </summary>
    public Result<Producto, DomainError> Update(int id, string nombre, string descripcion, string categoria, decimal precio, int stock, bool activo)
    {
        // Verificar que existe
        var existente = _repository.GetById(id);
        if (existente == null)
            return Result.Failure<Producto, DomainError>(ProductoErrors.ProductoNoEncontrado(id));

        // Crear producto actualizado
        var producto = new Producto(id, nombre.Trim(), descripcion.Trim(), categoria.Trim(), precio, stock, activo);

        // Validar
        var validacion = _validador.Validar(producto);
        if (validacion.IsFailure)
            return Result.Failure<Producto, DomainError>(validacion.Error);

        // Actualizar en repositorio
        var actualizado = _repository.Update(id, producto);
        if (actualizado == null)
            return Result.Failure<Producto, DomainError>(new DatabaseError("Error al actualizar el producto"));
        
        return Result.Success<Producto, DomainError>(actualizado);
    }

    /// <summary>
    /// Elimina un producto.
    /// </summary>
    public Result<bool, DomainError> Delete(int id)
    {
        var eliminado = _repository.Delete(id);
        if (eliminado == null)
            return Result.Failure<bool, DomainError>(ProductoErrors.ProductoNoEncontrado(id));
        
        return Result.Success<bool, DomainError>(true);
    }

    /// <summary>
    /// Busca productos por criterio.
    /// </summary>
    public Result<IEnumerable<Producto>, DomainError> Search(string criterio)
    {
        if (string.IsNullOrWhiteSpace(criterio))
            return Result.Success<IEnumerable<Producto>, DomainError>(_repository.GetActivos());

        return Result.Success<IEnumerable<Producto>, DomainError>(_repository.Search(criterio));
    }

    /// <summary>
    /// Obtiene productos por categoría.
    /// </summary>
    public Result<IEnumerable<Producto>, DomainError> GetByCategoria(string categoria)
    {
        if (string.IsNullOrWhiteSpace(categoria))
            return Result.Success<IEnumerable<Producto>, DomainError>(_repository.GetActivos());

        return Result.Success<IEnumerable<Producto>, DomainError>(_repository.GetByCategoria(categoria));
    }

    /// <summary>
    /// Obtiene solo productos activos.
    /// </summary>
    public Result<IEnumerable<Producto>, DomainError> GetActivos()
    {
        return Result.Success<IEnumerable<Producto>, DomainError>(_repository.GetActivos());
    }

    /// <summary>
    /// Obtiene estadísticas del inventario.
    /// </summary>
    public Result<Dictionary<string, int>, DomainError> GetEstadisticas()
    {
        var lista = _repository.GetAll().ToList();

        var stats = new Dictionary<string, int>
        {
            ["TotalProductos"] = lista.Count,
            ["ProductosActivos"] = lista.Count(p => p.Activo),
            ["TotalStock"] = lista.Sum(p => p.Stock),
            ["Categorias"] = lista.Select(p => p.Categoria).Distinct().Count()
        };

        return Result.Success<Dictionary<string, int>, DomainError>(stats);
    }
}