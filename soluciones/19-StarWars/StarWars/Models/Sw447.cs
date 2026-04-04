namespace StarWars.Models;

/// <summary>
/// Droide SW447: Variante con escudo protector.
/// Implementa la interfaz IEscudo para absorber parte del daño.
/// </summary>
/// <remarks>
/// Este droide tiene un escudo que absorbe una cantidad fija de daño.
/// Si el daño es menor que el escudo, el droide no recibe ningún daño.
/// Tiene un símbolo visual de 🔵 en el cuadrante.
/// </remarks>
public sealed class Sw447 : Droide, IEscudo
{
    /// <summary>
    /// Constructor del droide SW447.
    /// Inicializa el tipo a SW447 y configura su escudo.
    /// </summary>
    /// <param name="maxEnergy">Energía máxima inicial del droide</param>
    /// <param name="shield">Puntos de escudo que absorben daño</param>
    public Sw447(int maxEnergy, int shield) : base(maxEnergy)
    {
        Model = TipoDroide.SW447;
        Shield = shield;
    }

    /// <summary>
    /// Tipo de droide (implementación de la propiedad abstracta).
    /// </summary>
    public override TipoDroide Model { get; }

    /// <summary>
    /// Puntos de escudo del droide.
    /// Se usa para absorber daño antes de que afecte a la energía.
    /// </summary>
    public int Shield { get; }

    /// <summary>
    /// Símbolo visual del droide en el cuadrante.
    /// </summary>
    public override string Simbolo => "🛡️";

    /// <summary>
    /// Método para usar el escudo: absorbe daño hasta su capacidad.
    /// </summary>
    /// <param name="damage">Daño original del ataque</param>
    /// <returns>El daño efectivo después de absorber con el escudo</returns>
    /// <example>
    /// Si damage=25 y Shield=8, devuelve 17 (25-8=17 de daño efectivo)
    /// Si damage=5 y Shield=8, devuelve 0 (el escudo absorbe todo)
    /// </example>
    public int UsarEscudo(int damage)
    {
        Console.WriteLine($"Droide Sw447-{Id} usa su escudo: {Shield}");
        return Shield > damage ? 0 : damage - Shield;  // Si el escudo es mayor, no hay daño
    }

    /// <summary>
    /// Representación en texto del droide SW447.
    /// </summary>
    /// <returns>String con formato: "Droid-Sw447(🔵 id=X, maxEnergy=Y, shield=Z)"</returns>
    public override string ToString()
    {
        return $"Droid-Sw447({Simbolo} id={Id}, maxEnergy={MaxEnergy}, shield={Shield})";
    }
}
