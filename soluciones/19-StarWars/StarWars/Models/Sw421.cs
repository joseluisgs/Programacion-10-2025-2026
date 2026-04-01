namespace StarWars.Models;

/// <summary>
/// Droide SW421: Variante veloz con capacidad de movimiento.
/// Implementa la interfaz IMovimiento para escapar de los ataques.
/// </summary>
/// <remarks>
/// Este droide tiene la habilidad de moverse (escapar) antes de recibir el daño.
/// La probabilidad de escapar depende de su velocidad (Velocity).
/// Tiene un símbolo visual de ⚪ en el cuadrante.
/// </remarks>
public sealed class Sw421 : Droide, IMovimiento
{
    /// <summary>
    /// Constructor del droide SW421.
    /// Inicializa el tipo a SW421 y configura su velocidad.
    /// </summary>
    /// <param name="maxEnergy">Energía máxima inicial del droide</param>
    /// <param name="velocity">Porcentaje de probabilidad de escapar (10-30%)</param>
    public Sw421(int maxEnergy, int velocity) : base(maxEnergy)
    {
        Model = TipoDroide.SW421;
        Velocity = velocity;
    }

    /// <summary>
    /// Tipo de droide (implementación de la propiedad abstracta).
    /// </summary>
    public override TipoDroide Model { get; }

    /// <summary>
    /// Velocidad del droide (porcentaje de probabilidad de escapar).
    /// Un valor más alto significa mayor probabilidad de sobrevivir.
    /// </summary>
    public int Velocity { get; set; }

    /// <summary>
    /// Símbolo visual del droide en el cuadrante.
    /// </summary>
    public override string Simbolo => "⚪";

    /// <summary>
    /// Método de movimiento: intenta escapar del ataque.
    /// </summary>
    /// <returns>True si el droide logra escapar, false si es alcanzado</returns>
    /// <example>
    /// Si Velocity=20, hay un 20% de probabilidad de escapar.
    /// Si escapa, devuelve true y no recibe daño.
    /// Si no escapa, devuelve false y recibe el daño completo.
    /// </example>
    public bool Moverse()
    {
        Console.WriteLine($"Droide Sw421-{Id} se mueve con velocidad {Velocity}");
        // Genera un número aleatorio entre 1 y 100, y lo compara con la velocidad
        return new Random().Next(1, 101) <= Velocity;
    }

    /// <summary>
    /// Representación en texto del droide SW421.
    /// </summary>
    /// <returns>String con formato: "Droid-Sw421(⚪ id=X, maxEnergy=Y, velocity=Z)"</returns>
    public override string ToString()
    {
        return $"Droid-Sw421({Simbolo} id={Id}, maxEnergy={MaxEnergy}, velocity={Velocity})";
    }
}
