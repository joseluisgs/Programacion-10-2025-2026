// ============================================================
// IProductoService.cs - Interfaz del servicio de productos
// ============================================================

using System.Collections.Generic;
using CSharpFunctionalExtensions;
using GestionProductos.Errors;
using GestionProductos.Models;

namespace GestionProductos.Services;

/// <summary>
/// Interfaz del servicio de productos.
/// </summary>
public interface IProductoService
{
    Result<IEnumerable<Producto>, DomainError> GetAll();
    Result<Producto, DomainError> GetById(int id);
    Result<Producto, DomainError> Create(string nombre, string descripcion, string categoria, decimal precio, int stock);
    Result<Producto, DomainError> Update(int id, string nombre, string descripcion, string categoria, decimal precio, int stock, bool activo);
    Result<bool, DomainError> Delete(int id);
    Result<IEnumerable<Producto>, DomainError> Search(string criterio);
    Result<IEnumerable<Producto>, DomainError> GetByCategoria(string categoria);
    Result<IEnumerable<Producto>, DomainError> GetActivos();
    Result<Dictionary<string, int>, DomainError> GetEstadisticas();
}