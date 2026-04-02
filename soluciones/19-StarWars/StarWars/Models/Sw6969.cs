using StarWars.Config;

namespace StarWars.Models;

/// <summary>
/// Droide SW6969: Variante especial inestable.
/// Implementa múltiples interfaces: IDefensa, IMovimiento, y tiene capacidad de explotar.
/// </summary>
/// <remarks>
/// Este es el droide más complejo del juego:
/// - Tiene defensa (reduce el daño)
/// - Tiene velocidad (puede escapar)
/// - Tiene probabilidad de explotar al ser alcanzado (30%)
/// Si explota, su energía se reduce a 0 instantáneamente.
/// Tiene un símbolo visual de 🛸 en el cuadrante.
/// </remarks>
public sealed class Sw6969 : Droide, IDefensa, IMovimiento
{
    /// <summary>
    /// Constructor del droide SW6969.
    /// Inicializa el tipo a SW6969, su defensa y velocidad.
    /// </summary>
    /// <param name="maxEnergy">Energía máxima inicial del droide</param>
    /// <param name="defense">Puntos de defensa que reducen el daño</param>
    /// <param name="velocity">Porcentaje de probabilidad de escapar</param>
    public Sw6969(int maxEnergy, int defense, int velocity) : base(maxEnergy)
    {
        Model = TipoDroide.SW6969;
        Defense = defense;
        Velocity = velocity;
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
    /// Velocidad del droide (porcentaje de probabilidad de escapar).
    /// </summary>
    public int Velocity { get; set; }

    /// <summary>
    /// Símbolo visual del droide en el cuadrante.
    /// </summary>
    public override string Simbolo => "🛸";

    /// <summary>
    /// Método de defensa: reduce el daño basado en la defensa del droide.
    /// </summary>
    /// <param name="damage">Daño original del ataque</param>
    /// <returns>El daño efectivo después de aplicar la defensa</returns>
    /// <example>
    /// Si damage=25 y Defense=7, devuelve 7 (el daño no puede ser mayor que la defensa)
    /// </example>
    public int Defender(int damage)
    {
        Console.WriteLine($"Droide Sw6969-{Id} trata de defenderse con defensa: {Defense}");
        return Math.Min(damage, Defense);
    }

    /// <summary>
    /// Método de movimiento: intenta escapar del ataque.
    /// </summary>
    /// <returns>True si el droide logra escapar, false si es alcanzado</returns>
    /// <example>
    /// Si Velocity=20, hay un 20% de probabilidad de escapar.
    /// </example>
    public bool Moverse()
    {
        Console.WriteLine($"Droide Sw6969-{Id} se mueve con velocidad {Velocity}");
        return new Random().Next(1, 101) <= Velocity;
    }

    /// <summary>
    /// Método especial: hace que el droide explote con cierta probabilidad.
    /// Si explota, el droide pierde toda su energía instantáneamente.
    /// </summary>
    /// <remarks>
    /// Este método se llama después de que el droide recibe un impacto.
    /// La probabilidad de explosión está definida en Configuration.ProbExplosion (30%).
    /// Si explota, el droide se considera "muerto" inmediatamente.
    /// </remarks>
    public void Explotar()
    {
        // Genera un número aleatorio entre 1 y 100
        var random = new Random().Next(1, 101);
        
        // Si el número es menor o igual a la probabilidad de explosión (30%),
        // el droide explota y pierde toda su energía
        if (random <= Configuration.ProbExplosion)
        {
            Console.WriteLine($"Droide 6969-{Id} explota");
            MaxEnergy = 0;  // Energía a 0 = droide muerto
        }
        else
        {
            Console.WriteLine($"Droide 6969-{Id} no explota");
        }
    }

    /// <summary>
    /// Representación en texto del droide SW6969.
    /// </summary>
    /// <returns>String con formato: "Droid-SW6969(🛸 id=X, maxEnergy=Y, defense=Z, velocity=W)"</returns>
    public override string ToString()
    {
        return $"Droid-SW6969({Simbolo} id={Id}, maxEnergy={MaxEnergy}, defense={Defense}, velocity={Velocity})";
    }
}