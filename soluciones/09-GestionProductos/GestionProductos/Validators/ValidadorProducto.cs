// ============================================================
// ValidadorProducto.cs - Validador de productos
// ============================================================
// Valida las reglas de negocio para productos.
//
// REGLAS DE VALIDACIÓN:
// - Nombre: obligatorio, entre 3 y 100 caracteres
// - Categoría: obligatoria
// - Precio: no negativo
// - Stock: no negativo

using System.Collections.Generic;
using CSharpFunctionalExtensions;
using GestionProductos.Errors;
using GestionProductos.Models;

namespace GestionProductos.Validators;

/// <summary>
/// Validador para productos.
/// </summary>
public class ValidadorProducto : IValidador<Producto>
{
    /// <summary>
    /// Valida un producto.
    /// </summary>
    public Result<Producto, DomainError> Validar(Producto producto)
    {
        var errores = new List<string>();

        // Validar nombre
        if (string.IsNullOrWhiteSpace(producto.Nombre))
            errores.Add("El nombre es obligatorio");
        else if (producto.Nombre.Length < 3)
            errores.Add("El nombre debe tener al menos 3 caracteres");
        else if (producto.Nombre.Length > 100)
            errores.Add("El nombre no puede exceder 100 caracteres");

        // Validar categoría
        if (string.IsNullOrWhiteSpace(producto.Categoria))
            errores.Add("La categoría es obligatoria");

        // Validar precio
        if (producto.Precio < 0)
            errores.Add("El precio no puede ser negativo");

        // Validar stock
        if (producto.Stock < 0)
            errores.Add("El stock no puede ser negativo");

        if (errores.Count > 0)
            return Result.Failure<Producto, DomainError>(new ValidationError(string.Join("; ", errores)));

        return Result.Success<Producto, DomainError>(producto);
    }
}