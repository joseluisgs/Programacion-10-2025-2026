// ============================================================
// ProductoErrors.cs - Errores específicos del dominio de Productos
// ============================================================
// Errores específicos para el dominio de Productos.
// Sigue el patrón de GestionAcademicaPro.
//
// CONCEPTOS IMPORTANTES:
//
// 1. ERRORES ESPECÍFICOS:
//    - Cada tipo de error tiene su propia clase
//    - Hereda de DomainError
//    - Mensajes descriptivos para el usuario
//
// 2. TIPOS DE ERRORES:
//    - NotFound: Producto no existe en la base de datos
//    - Validation: Errores de validación del validador
//    - AlreadyExists: Conflicto de integridad (duplicado)
//
// 3. FACTORY STATIC:
//    - Clase estática para crear errores fácilmente
//    - Devuelve DomainError (interfaz pública)
//    - Oculta la implementación interna

using System.Collections.Generic;

namespace ListaCompra.Errors;

/// <summary>
/// Errores específicos del dominio de Productos.
/// </summary>
public abstract record ProductoError(string Message) : DomainError(Message)
{
    public sealed record NotFound(int Id)
        : ProductoError($"No se ha encontrado ningún producto con el identificador: {Id}");

    public sealed record Validation(IEnumerable<string> Errors)
        : ProductoError("Se han detectado errores de validación en el producto.");

    public sealed record AlreadyExists(string Nombre)
        : ProductoError($"Conflicto de integridad: El producto '{Nombre}' ya existe en el sistema.");
}

/// <summary>
/// Factory para crear errores de dominio de Productos.
/// </summary>
public static class ProductoErrors
{
    public static DomainError NotFound(int id) => new ProductoError.NotFound(id);
    public static DomainError Validation(IEnumerable<string> errors) => new ProductoError.Validation(errors);
    public static DomainError AlreadyExists(string nombre) => new ProductoError.AlreadyExists(nombre);
}
