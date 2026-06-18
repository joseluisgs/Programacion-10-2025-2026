// ============================================================
// ValidadorProducto.cs - Validador para la entidad Producto
// ============================================================
// Validador que implementa las reglas de negocio para productos.
// Devuelve Result<T, DomainError> usando CSharpFunctionalExtensions.
//
// CONCEPTOS IMPORTANTES:
//
// 1. VALIDACIÓN DE ENTIDADES:
//    - Comprobación de que los datos cumplen las reglas de negocio
//    - Se ejecuta antes de guardar en el repositorio
//    - Devuelve Result<T, Error> (éxito o fracaso)
//
// 2. REGLAS DE VALIDACIÓN:
//    - Nombre: obligatorio, entre 2 y 100 caracteres
//    - Cantidad: mayor que 0
//    - Precio: no negativo
//
// 3. INTEGRACIÓN CON ROP:
//    - IValidador<T> devuelve Result<T, DomainError>
//    - El Servicio encadena con Bind()
//    - Si hay errores, devuelve Failure con los mensajes
//
// 4. INYECCIÓN DE DEPENDENCIAS:
//    - Se registra en DependenciesProvider como AddTransient
//    - El servicio lo recibe por constructor

using System.Collections.Generic;
using CSharpFunctionalExtensions;
using ListaCompra.Errors;
using ListaCompra.Models;

namespace ListaCompra.Validators;

public interface IValidador<T>
{
    Result<T, DomainError> Validar(T entity);
}

/// <summary>
/// Validador para la entidad Producto.
/// </summary>
public class ValidadorProducto : IValidador<Producto>
{
    public Result<Producto, DomainError> Validar(Producto producto)
    {
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(producto.Nombre))
            errores.Add("El nombre es obligatorio.");
        else if (producto.Nombre.Length < 2)
            errores.Add("El nombre debe tener al menos 2 caracteres.");
        else if (producto.Nombre.Length > 100)
            errores.Add("El nombre no puede exceder 100 caracteres.");

        if (producto.Cantidad <= 0)
            errores.Add("La cantidad debe ser mayor que 0.");

        if (producto.Precio < 0)
            errores.Add("El precio no puede ser negativo.");

        if (errores.Count > 0)
            return Result.Failure<Producto, DomainError>(ProductoErrors.Validation(errores));

        return Result.Success<Producto, DomainError>(producto);
    }
}
