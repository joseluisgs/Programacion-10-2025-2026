namespace StarWars.Models;

/// <summary>
/// Interfaz que define el comportamiento de escudo para droides.
/// </summary>
/// <remarks>
/// Implementada por droides que tienen un escudo protector.
/// El escudo absorbe una cantidad fija de daño antes de que afecte la energía.
/// Si el daño es menor que el escudo, el droide no recibe ningún daño.
/// </remarks>
public interface IEscudo
{
    /// <summary>
    /// Puntos de escudo del droide.
    /// </summary>
    int Shield { get; }
    
    /// <summary>
    /// Método que usa el escudo para absorber daño.
    /// </summary>
    /// <param name="damage">Daño original del ataque</param>
    /// <returns>El daño efectivo después de absorber con el escudo</returns>
    int UsarEscudo(int damage);
}
