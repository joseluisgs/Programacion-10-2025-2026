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
//    - Usa Result<T, Error> para manejo de errores
//
// 2. OPERACIONES DEFINIDAS:
//    - GetAll(): Obtener todos los productos
//    - GetById(): Obtener un producto por ID (usa caché)
//    - Buscar(): Buscar productos por nombre
//    - Add(): Añadir nuevo producto (con validación ROP)
//    - Update(): Actualizar producto (con validación ROP)
//    - Delete(): Eliminar producto (con validación ROP)
//    - MarcarComprado(): Cambiar estado
//
// 3. USO DE ROP:
//    - Operaciones de escritura devuelven Result<T, DomainError>
//    - UI usa Match() para manejar éxito/error
//    - Errores se muestran en MessageBox
//
// 4. IMPLEMENTACIÓN:
//    - Véase ProductoService.cs

using System.Collections.Generic;
using CSharpFunctionalExtensions;
using ListaCompra.Errors;
using ListaCompra.Models;

namespace ListaCompra.Services;

public interface IProductoService
{
    IEnumerable<Producto> GetAll();
    Producto? GetById(int id);
    IEnumerable<Producto> Buscar(string? nombre);
    Result<Producto, DomainError> Add(string nombre, int cantidad, decimal precio);
    Result<Producto, DomainError> Update(int id, string nombre, int cantidad, decimal precio, bool comprado);
    Result<bool, DomainError> Delete(int id);
    Result<Producto, DomainError> MarcarComprado(int id, bool comprado);
}
