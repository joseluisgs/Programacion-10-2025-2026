// ============================================================
// IValidador.cs - Interfaz genérica para validadores
// ============================================================
// Interfaz genérica para validar entidades.
//
// CONCEPTOS IMPORTANTES:
//
// 1. INTERFAZ GENÉRICA:
//    - IValidador<T> donde T es la entidad a validar
//    - Permite crear validadores reutilizables para diferentes tipos
//    - Ejemplo: IValidador<Producto>, IValidador<Cliente>, etc.
//
// 2. Validar(T entity):
//    - Recibe una entidad y devuelve una colección de errores
//    - Si la colección está vacía, la entidad es válida
//    - Cada string es un mensaje de error específico
//
// 3. USO EN LA APLICACIÓN:
//    - El servicio (ProductoService) usa el validador antes de guardar
//    - Si hay errores, lanza ListaCompraException.Validation
//    - Los errores se muestran al usuario en un MessageBox
//
// EJEMPLO DE USO:
//
//   var validador = new ValidadorProducto();
//   var errores = validador.Validar(producto);
//   
//   if (errores.Any())
//       throw new ListaCompraException.Validation(errores);

using System.Collections.Generic;

namespace ListaCompra.Validators;

/// <summary>
/// Interfaz para el validador de entidades.
/// </summary>
public interface IValidador<T> where T : class
{
    /// <summary>
    /// Valida la entidad y devuelve una lista de errores.
    /// </summary>
    /// <param name="entity">Entidad a validar.</param>
    /// <returns>Enumerable de mensajes de error. Si está vacío, la entidad es válida.</returns>
    IEnumerable<string> Validar(T entity);
}