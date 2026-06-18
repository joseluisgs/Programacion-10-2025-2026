namespace StarWars.Models;

/// <summary>
/// Interfaz que define el comportamiento de defensa para droides.
/// </summary>
/// <remarks>
/// Implementada por droides que tienen capacidad de defenderse de los ataques.
/// La defensa reduce el daño recibido antes de afectar la energía del droide.
/// </remarks>
public interface IDefensa
{
    /// <summary>
    /// Puntos de defensa del droide.
    /// </summary>
    int Defense { get; }
    
    /// <summary>
    /// Método que aplica la defensa del droide a un daño recibido.
    /// </summary>
    /// <param name="damage">Daño original del ataque</param>
    /// <returns>El daño efectivo después de aplicar la defensa</returns>
    int Defender(int damage);
}
