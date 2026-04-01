// ============================================================
// DomainError.cs - Clase base para errores del dominio
// ============================================================
// Clase base abstracta para todos los errores del dominio.
// Usada con CSharpFunctionalExtensions para Railway Oriented Programming.
//
// CONCEPTOS IMPORTANTES:
//
// 1. RAILWAY ORIENTED PROGRAMMING (ROP):
//    - Patrón de manejo de errores funcionales
//    - Result<T, Error>: Puede ser Success o Failure
//    - Encadenamiento con Bind() y Map()
//    - Sin excepciones lanzadas
//
// 2. DomainError (record):
//    - Clase base para todos los errores
//    - Inmutable (record)
//    - Contiene mensaje descriptivo
//
// 3. USO EN LA APLICACIÓN:
//    - Validator devuelve Result<T, DomainError>
//    - Service encadena validaciones con Bind
//    - ViewModel maneja resultado con Match

namespace ListaCompra.Errors;

/// <summary>
/// Clase base abstracta para todos los errores del dominio.
/// </summary>
public abstract record DomainError(string Message);
