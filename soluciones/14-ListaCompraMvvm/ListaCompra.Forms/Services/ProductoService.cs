// ============================================================
// ProductoService.cs - Implementación del servicio de productos
// ============================================================
// Servicio con validación de negocio usando ROP (Railway Oriented Programming).
// Coordina repositorio, validador y caché.
//
// CONCEPTOS IMPORTANTES:
//
// 1. SERVICE - LÓGICA DE NEGOCIO:
//    - Coordina el repositorio, el validador y la caché
//    - Valida los datos antes de cualquier operación de escritura
//    - Usa Result<T, Error> para encadenar operaciones
//    - Es el intermediario entre la UI y los datos
//
// 2. RAILWAY ORIENTED PROGRAMMING:
//    - CheckExists(): Verifica que existe antes de operar
//    - Bind(): Encadena validaciones
//    - Map(): Transforma el resultado
//    - Si algo falla, retorna Failure inmediatamente
//
// 3. INYECCIÓN DE DEPENDENCIAS:
//    - Recibe IProductoRepository por constructor
//    - Recibe IValidador<Producto> por constructor
//    - Recibe ICache<int, Producto> por constructor
//    - El ServiceProvider los proporciona automáticamente
//
// 4. CACHÉ LRU:
//    - GetById(): Primero busca en caché, luego en BD
//    - Update(): Elimina de caché al actualizar
//    - Delete(): Elimina de caché al borrar
//    - Configurable en appsettings.json
//
// 5. OPERACIONES:
//    - GetAll/GetById/Buscar: Lectura (no devuelven Result)
//    - Add/Update/Delete/MarcarComprado: Escritura (devuelven Result)

using System.Collections.Generic;
using CSharpFunctionalExtensions;
using ListaCompra.Cache;
using ListaCompra.Config;
using ListaCompra.Errors;
using ListaCompra.Models;
using ListaCompra.Repositories;
using ListaCompra.Validators;
using Serilog;

namespace ListaCompra.Services;

public class ProductoService : IProductoService
{
    private readonly ILogger _logger = Log.ForContext<ProductoService>();
    private readonly IProductoRepository _repository;
    private readonly IValidador<Producto> _validador;
    private readonly ICache<int, Producto> _cache;

    public ProductoService(
        IProductoRepository repository, 
        IValidador<Producto> validador,
        ICache<int, Producto> cache)
    {
        _logger.Debug("ProductoService inicializado");
        _repository = repository;
        _validador = validador;
        _cache = cache;
    }

    public IEnumerable<Producto> GetAll() => _repository.GetAll();

    public Producto? GetById(int id)
    {
        if (AppConfig.CacheEnabled)
        {
            var cached = _cache.Get(id);
            if (cached != null)
            {
                _logger.Debug("Producto {id} encontrado en caché", id);
                return cached;
            }
        }

        var producto = _repository.GetById(id);
        
        if (producto != null && AppConfig.CacheEnabled)
        {
            _cache.Add(id, producto);
        }

        return producto;
    }

    public IEnumerable<Producto> Buscar(string? nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return _repository.GetAll();
        
        return _repository.GetByNombre(nombre);
    }

    public Result<Producto, DomainError> Add(string nombre, int cantidad, decimal precio)
    {
        _logger.Information("Añadiendo producto: {nombre}", nombre);
        
        var producto = new Producto(0, nombre.Trim(), cantidad, precio);
        
        return _validador.Validar(producto)
            .Bind(p => Result.Success<Producto, DomainError>(p))
            .Map(p => _repository.Create(p)!);
    }

    public Result<Producto, DomainError> Update(int id, string nombre, int cantidad, decimal precio, bool comprado)
    {
        _logger.Information("Actualizando producto {id}: {nombre}", id, nombre);
        
        return CheckExists(id)
            .Bind(_ => 
            {
                var producto = new Producto(id, nombre.Trim(), cantidad, precio, comprado);
                return _validador.Validar(producto);
            })
            .Map(p => 
            {
                if (AppConfig.CacheEnabled)
                    _cache.Remove(id);
                return _repository.Update(id, p)!;
            });
    }

    public Result<bool, DomainError> Delete(int id)
    {
        _logger.Information("Eliminando producto {id}", id);
        
        return CheckExists(id)
            .Map(_ => 
            {
                if (AppConfig.CacheEnabled)
                    _cache.Remove(id);
                var resultado = _repository.Delete(id, false);
                return resultado != null;
            });
    }

    public Result<Producto, DomainError> MarcarComprado(int id, bool comprado)
    {
        _logger.Information("Marcando producto {id} como comprado: {comprado}", id, comprado);
        
        return CheckExists(id)
            .Map(existente => 
            {
                var producto = new Producto(
                    existente.Id, 
                    existente.Nombre, 
                    existente.Cantidad, 
                    existente.Precio, 
                    comprado
                );
                return _repository.Update(id, producto)!;
            });
    }

    private Result<Producto, DomainError> CheckExists(int id)
    {
        var producto = GetById(id);
        if (producto == null)
            return Result.Failure<Producto, DomainError>(ProductoErrors.NotFound(id));
        return Result.Success<Producto, DomainError>(producto);
    }
}
