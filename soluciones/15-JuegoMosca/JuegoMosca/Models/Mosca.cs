// =============================================================================
// MODELOS DEL JUEGO DE LA MOSCA
// =============================================================================
// En esta sección definimos las estructuras de datos que representan
// la información del juego. Usamos "record" para crear clases inmutables
// que comparan por valor (no por referencia).
// =============================================================================

namespace JuegoMosca.Models;

/// <summary>
/// Representa las coordenadas (fila y columna) de una posición en la matriz.
/// Usamos un "record" que es como una clase inmutable.
/// Ejemplo: new MoscaPosition(2, 3) representa fila 2, columna 3.
/// </summary>
public sealed record MoscaPosition(int Fila, int Columna);

/// <summary>
/// Representa el resultado cuando el jugador acierta la posición de la mosca.
/// Contiene la posición donde estaba la mosca y los intentos usados.
/// </summary>
public sealed record Acertado(MoscaPosition Mosca, int Intentos);

/// <summary>
/// Clase con constantes del juego.
/// Las constantes son valores que no cambian durante la ejecución.
/// </summary>
public static class MoscaConstants
{
    /// <summary>
    /// Constante que representa la posición de la mosca en la matriz.
    /// Usamos -1 porque las celdas normales tienen valor 0.
    /// </summary>
    public const int MOSCA = -1;
}
