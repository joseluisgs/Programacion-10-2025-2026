// ============================================================
// ListaCompraException.cs - Excepciones del dominio Lista de la Compra
// ============================================================
// Excepciones específicas para el dominio de la lista de la compra.
//
// CONCEPTOS IMPORTANTES:
//
// 1. EXCEPCIONES DEL DOMINIO:
//    - Representan errores de negocio específicos de la aplicación
//    - Diferentes de excepciones técnicas (ArgumentNullException, etc.)
//    - Permiten manejar errores de forma más específica
//
// 2. CLASES ANIDADAS:
//    - ListaCompraException.NotFound: Producto no encontrado
//    - ListaCompraException.Validation: Errores de validación
//    - ListaCompraException.AlreadyExists: Producto duplicado
//    - sealed: No se puede heredar de estas clases
//
// 3. USO EN LA APLICACIÓN:
//    - El servicio lanza estas excepciones
//    - La UI las captura y muestra mensajes apropiados
//    - Validation contiene lista de errores (múltiples)
//
// 4. BENEFICIOS:
//    - Código más legible (errores específicos)
//    - La UI puede responder diferente a cada tipo
//    - Separa errores de negocio de errores técnicos

using System;
using System.Collections.Generic;

namespace ListaCompra.Exceptions;

/// <summary>
/// Clase base para excepciones del dominio Lista de la Compra.
/// </summary>
public abstract class ListaCompraException : Exception
{
    /// <summary>
    /// Constructor con mensaje de error.
    /// </summary>
    /// <param name="message">Mensaje descriptivo del error.</param>
    protected ListaCompraException(string message) : base(message) { }

    /// <summary>
    /// Excepción cuando no se encuentra un producto por su ID.
    /// </summary>
    public sealed class NotFound : ListaCompraException
    {
        /// <summary>
        /// Crea excepción de producto no encontrado.
        /// </summary>
        /// <param name="id">ID del producto que no se encontró.</param>
        public NotFound(int id) : base($"No se ha encontrado ningún producto con el identificador: {id}") { }
    }

    /// <summary>
    /// Excepción cuando hay errores de validación en un producto.
    /// </summary>
    public sealed class Validation : ListaCompraException
    {
        /// <summary>
        /// Crea excepción de validación con lista de errores.
        /// </summary>
        /// <param name="errors">Enumerable de mensajes de error.</param>
        public Validation(IEnumerable<string> errors) : base("Se han detectado errores de validación en el producto.")
        {
            Errores = errors;
        }
        
        /// <summary>
        /// Lista de errores de validación.
        /// </summary>
        public IEnumerable<string> Errores { get; init; }
    }

    /// <summary>
    /// Excepción cuando se intenta añadir un producto que ya existe.
    /// </summary>
    public sealed class AlreadyExists : ListaCompraException
    {
        /// <summary>
        /// Crea excepción de producto duplicado.
        /// </summary>
        /// <param name="nombre">Nombre del producto que ya existe.</param>
        public AlreadyExists(string nombre) : base($"Conflicto: El producto '{nombre}' ya existe en la lista.") { }
    }
}