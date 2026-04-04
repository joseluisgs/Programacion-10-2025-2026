namespace StarWars.Models;

/// <summary>
/// Droide SW348: Variante básica con capacidad de defensa.
/// Implementa la interfaz IDefensa para reducir el daño recibido.
/// </summary>
/// <remarks>
/// Este droide es el más simple del juego. Su única habilidad especial es
/// aplicar una defensa que reduce el daño de los ataques. Tiene un símbolo
/// visual de 🔴 en el cuadrante.
/// </remarks>
public sealed class Sw348 : Droide, IDefensa
{
    /// <summary>
    /// Constructor del droide SW348.
    /// Inicializa el tipo a SW348 y configura su defensa.
    /// </summary>
    /// <param name="maxEnergy">Energía máxima inicial del droide</param>
    /// <param name="defense">Puntos de defensa que reducen el daño</param>
    public Sw348(int maxEnergy, int defense) : base(maxEnergy)
    {
        Model = TipoDroide.SW348;
        Defense = defense;
    }

    /// <summary>
    /// Tipo de droide (implementación de la propiedad abstracta).
    /// </summary>
    public override TipoDroide Model { get; }

    /// <summary>
    /// Puntos de defensa del droide.
    /// Se usa para reducir el daño recibido en cada ataque.
    /// </summary>
    public int Defense { get; set; }

    /// <summary>
    /// Símbolo visual del droide en el cuadrante.
    /// </summary>
    public override string Simbolo => "🤖";

    /// <summary>
    /// Método de defensa: reduce el daño basado en la defensa del droide.
    /// </summary>
    /// <param name="damage">Daño original del ataque</param>
    /// <returns>El daño efectivo después de aplicar la defensa</returns>
    /// <example>
    /// Si damage=25 y Defense=10, devuelve 10 (el daño no puede ser mayor que la defensa)
    /// </example>
    public int Defender(int damage)
    {
        Console.WriteLine($"Droide Sw348-{Id} trata de defenderse con defensa: {Defense}");
        return Math.Min(damage, Defense);
    }

    /// <summary>
    /// Representación en texto del droide SW348.
    /// </summary>
    /// <returns>String con formato: "Droid-Sw348(🔴 id=X, maxEnergy=Y, defense=Z)"</returns>
    public override string ToString()
    {
        return $"Droid-Sw348({Simbolo} id={Id}, maxEnergy={MaxEnergy}, defense={Defense})";
    }
}
