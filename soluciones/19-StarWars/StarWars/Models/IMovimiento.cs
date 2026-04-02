namespace StarWars.Models;

/// <summary>
/// Interfaz que define el comportamiento de movimiento para droides.
/// </summary>
/// <remarks>
/// Implementada por droides que tienen capacidad de movimiento (escapar).
/// El droide puede moverse (escapar) antes de recibir el daño.
/// La probabilidad de escapar depende de la velocidad del droide.
/// </remarks>
public interface IMovimiento
{
    /// <summary>
    /// Velocidad del droide (porcentaje de probabilidad de escapar).
    /// Un valor más alto significa mayor probabilidad de sobrevivir.
    /// </summary>
    int Velocity { get; set; }
    
    /// <summary>
    /// Método que intenta mover el droide para escapar del ataque.
    /// </summary>
    /// <returns>True si el droide logra escapar, false si es alcanzado</returns>
    bool Moverse();
}
