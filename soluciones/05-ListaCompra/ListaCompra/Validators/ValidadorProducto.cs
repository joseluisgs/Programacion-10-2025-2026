// ============================================================
// ValidadorProducto.cs - Validador para la entidad Producto
// ============================================================
// Implementación del validador para productos de la lista de la compra.
//
// CONCEPTOS IMPORTANTES:
//
// 1. VALIDACIÓN DE ENTIDADES:
//    - Comprobación de que los datos cumplen las reglas de negocio
//    - Se ejecuta antes de guardar en el repositorio
//    - Devuelve lista de errores (vacía = válido)
//
// 2. REGLAS DE VALIDACIÓN DEFINIDAS:
//    - Nombre: obligatorio, entre 2 y 100 caracteres
//    - Cantidad: mayor que 0
//    - Precio: no negativo (mayor o igual que 0)
//
// 3. INTEGRACIÓN CON EL SERVICIO:
//    - ProductoService llama a ValidadorProducto.Validar()
//    - Si hay errores, lanza ListaCompraException.Validation
//    - Los errores se muestran al usuario
//
// 4. INYECCIÓN DE DEPENDENCIAS:
//    - Se registra en DependenciesProvider como AddTransient
//    - El servicio lo recibe por constructor

using System.Collections.Generic;
using System.Linq;
using ListaCompra.Models;

namespace ListaCompra.Validators;

/// <summary>
/// Validador para la entidad Producto.
/// Implementa las reglas de negocio para validar productos.
/// </summary>
public class ValidadorProducto : IValidador<Producto>
{
    /// <summary>
    /// Valida un producto y devuelve los errores encontrados.
    /// </summary>
    /// <param name="producto">Producto a validar.</param>
    /// <returns>Lista de mensajes de error. Vacía si es válido.</returns>
    public IEnumerable<string> Validar(Producto producto)
    {
        // Lista para acumular los errores encontrados
        var errores = new List<string>();

        // ============================================================
        // VALIDAR NOMBRE
        // ============================================================
        // Reglas: obligatorio, mínimo 2 caracteres, máximo 100
        if (string.IsNullOrWhiteSpace(producto.Nombre))
            errores.Add("El nombre es obligatorio.");
        else if (producto.Nombre.Length < 2)
            errores.Add("El nombre debe tener al menos 2 caracteres.");
        else if (producto.Nombre.Length > 100)
            errores.Add("El nombre no puede exceder 100 caracteres.");

        // ============================================================
        // VALIDAR CANTIDAD
        // ============================================================
        // Regla: debe ser mayor que 0
        if (producto.Cantidad <= 0)
            errores.Add("La cantidad debe ser mayor que 0.");

        // ============================================================
        // VALIDAR PRECIO
        // ============================================================
        // Regla: no puede ser negativo
        if (producto.Precio < 0)
            errores.Add("El precio no puede ser negativo.");

        // Devolver la lista de errores (vacía si todo correcto)
        return errores;
    }
}