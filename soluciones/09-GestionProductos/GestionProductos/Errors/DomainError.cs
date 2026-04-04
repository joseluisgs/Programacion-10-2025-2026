// ============================================================
// DomainError.cs - Errores de dominio
// ============================================================
// Jerarquía de errores de dominio para el módulo de productos.
//
// TIPOS DE ERRORES:
// - ValidationError: Errores de validación de datos
// - NotFoundError: Recurso no encontrado
// - ConflictError: Conflicto de datos (ya existe)
// - DatabaseError: Error de base de datos

namespace GestionProductos.Errors;

/// <summary>
/// Clase base para errores de dominio.
/// </summary>
public abstract class DomainError
{
    /// <summary>
    /// Código de error único.
    /// </summary>
    public string Code { get; }
    
    /// <summary>
    /// Mensaje descriptivo del error.
    /// </summary>
    public string Message { get; }
    
    /// <summary>
    /// Diccionario con información adicional.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; init; }

    protected DomainError(string code, string message)
    {
        Code = code;
        Message = message;
    }

    /// <summary>
    /// Representación en string del error.
    /// </summary>
    public override string ToString() => $"[{Code}] {Message}";
}

/// <summary>
/// Error de validación de datos.
/// </summary>
public class ValidationError : DomainError
{
    public ValidationError(string message) : base("VALIDATION_ERROR", message) { }
    
    public ValidationError(string message, Dictionary<string, object> metadata) 
        : base("VALIDATION_ERROR", message) 
    {
        Metadata = metadata;
    }
}

/// <summary>
/// Error cuando un recurso no se encuentra.
/// </summary>
public class NotFoundError : DomainError
{
    public NotFoundError(string message) : base("NOT_FOUND", message) { }
    
    public static NotFoundError FromId<T>(int id) => 
        new($"{typeof(T).Name} con ID {id} no encontrado");
    
    public static NotFoundError FromName<T>(string name) => 
        new($"{typeof(T).Name} con nombre '{name}' no encontrado");
}

/// <summary>
/// Error de conflicto de datos (ya existe).
/// </summary>
public class ConflictError : DomainError
{
    public ConflictError(string message) : base("CONFLICT", message) { }
    
    public static ConflictError AlreadyExists<T>(string identifier) => 
        new($"{typeof(T).Name} '{identifier}' ya existe");
}

/// <summary>
/// Error de base de datos.
/// </summary>
public class DatabaseError : DomainError
{
    public DatabaseError(string message) : base("DATABASE_ERROR", message) { }
    
    public DatabaseError(string message, Exception innerException) 
        : base("DATABASE_ERROR", message) 
    {
        Metadata = new Dictionary<string, object>
        {
            ["InnerException"] = innerException
        };
    }
}

/// <summary>
/// Errores específicos del módulo de productos.
/// </summary>
public static class ProductoErrors
{
    public static DomainError NombreRequerido => 
        new ValidationError("El nombre del producto es obligatorio");
    
    public static DomainError NombreMuyCorto => 
        new ValidationError("El nombre debe tener al menos 3 caracteres");
    
    public static DomainError NombreMuyLargo => 
        new ValidationError("El nombre no puede exceder 100 caracteres");
    
    public static DomainError PrecioNegativo => 
        new ValidationError("El precio no puede ser negativo");
    
    public static DomainError StockNegativo => 
        new ValidationError("El stock no puede ser negativo");
    
    public static DomainError CategoriaRequerida => 
        new ValidationError("La categoría es obligatoria");
    
    public static DomainError ProductoNoEncontrado(int id) => 
        new NotFoundError($"Producto con ID {id} no encontrado");
    
    public static DomainError ProductoYaExiste(string nombre) => 
        new ConflictError($"El producto '{nombre}' ya existe");
}