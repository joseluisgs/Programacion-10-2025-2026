// =============================================================================
// ERRORES DEL JUEGO DE LA MOSCA - ROP (Railway Oriented Programming)
// =============================================================================
// En este archivo implementamos el manejo de errores usando ROP.
// ROP es un patrón que trata las operaciones como ferrocarriles:
// - Si todo va bien, el tren sigue por la vía del éxito
// - Si hay error, el tren cambia a la vía de error
//
// Usamos "record" para crear clases inmutables de errores.
// El patrón "sealed" significa que no se puede heredar de esta clase.
// =============================================================================

using JuegoMosca.Models;

namespace JuegoMosca.Errors;

/// <summary>
/// Clase base abstracta para todos los errores del dominio.
/// Todas las clases de error heredarán de esta.
/// El parámetro Message contendrá la descripción del error.
/// </summary>
public abstract record DomainError(string Message);

/// <summary>
/// Errores específicos del juego de la mosca.
/// Usamos "record" con herencia para crear tipos de errores específicos.
/// </summary>
public abstract record MoscaError(string Message) : DomainError(Message)
{
    /// <summary>
    /// Error cuando el jugador no acierta la posición de la mosca.
    /// No contiene datos adicionales, solo el mensaje.
/// </summary>
    public sealed record NoAcertado() : MoscaError("¡Maldita Mosca! No has acertado, sigue intentándolo");

    /// <summary>
    /// Error cuando se agotan los intentos sin encontrar la mosca.
    /// Contiene la posición donde estaba la mosca y los intentos usados.
/// </summary>
    public sealed record FinIntentos(MoscaPosition Mosca, int Intentos) 
        : MoscaError($"Se han acabado los intentos. La mosca estaba en fila {Mosca.Fila + 1}, columna {Mosca.Columna + 1}");

    /// <summary>
    /// Error cuando el jugador se acerca mucho (a distancia 1).
    /// La mosca se mueve a una nueva posición.
/// </summary>
    public sealed record Casi(int Fila, int Columna, int Intentos) 
        : MoscaError($"¡Casi lo has logrado! La mosca estaba cerca de fila {Fila + 1}, columna {Columna + 1}. Llevas {Intentos} intentos");
}

/// <summary>
/// Clase factory (fábrica) para crear errores de forma más sencilla.
/// En lugar de usar "new MoscaError.NoAcertado()", podemos usar "MoscaErrors.NoAcertado()"
/// </summary>
public static class MoscaErrors
{
    /// <summary>Crea un error de tipo NoAcertado</summary>
    public static DomainError NoAcertado() => new MoscaError.NoAcertado();
    
    /// <summary>Crea un error de tipo FinIntentos</summary>
    public static DomainError FinIntentos(MoscaPosition mosca, int intentos) => new MoscaError.FinIntentos(mosca, intentos);
    
    /// <summary>Crea un error de tipo Casi</summary>
    public static DomainError Casi(int fila, int columna, int intentos) => new MoscaError.Casi(fila, columna, intentos);
}
