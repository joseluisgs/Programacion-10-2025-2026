using StarWars.Config;
using StarWars.Models;

namespace StarWars.Factories;

/// <summary>
/// Generador de droides aleatorios.
/// </summary>
/// <remarks>
/// Esta clase implementa el patrón Factory Method para crear droides.
/// Genera droides de diferentes tipos según las probabilidades configuradas.
/// </remarks>
public class DroideGenerator
{
    /// <summary>
    /// Crea un droide aleatorio basándose en las probabilidades configuradas.
    /// </summary>
    /// <returns>Una nueva instancia de un droide aleatorio</returns>
    /// <remarks>
    /// Las probabilidades de aparición son:
    /// - SW348: 20% (valores 1-20)
    /// - SW447: 40% (valores 21-60)
    /// - SW421: 20% (valores 61-80)
    /// - SW6969: 20% (valores 81-100)
    /// 
    /// Cada tipo de droide se crea con valores aleatorios dentro de los rangos
    /// definidos en Configuration.
    /// </remarks>
    public Droide RandomDroide()
    {
        // Genera un número aleatorio entre 1 y 100 (inclusive)
        var random = new Random().Next(1, Configuration.ProbSw6969 + 1);
        
        // Evalúa el número contra los rangos de probabilidad para decidir el tipo de droide
        return random switch
        {
            // SW348: Probabilidad del 1-20 (20% del total)
            // Este droide tiene energía fija de 50 y defensa aleatoria entre 9-12
            <= Configuration.ProbSw348 => new Sw348(
                Configuration.EnergiaSw348, 
                new Random().Next(Configuration.MinDefensaSw348, Configuration.MaxDefensaSw348 + 1)),
            
            // SW447: Probabilidad del 21-60 (40% del total)
            // Este droide tiene energía fija de 100 y escudo aleatorio entre 5-10
            <= Configuration.ProbSw447 => new Sw447(
                Configuration.EnergiaSw447, 
                new Random().Next(Configuration.MinEscudoSw447, Configuration.MaxEscudoSw447 + 1)),
            
            // SW421: Probabilidad del 61-80 (20% del total)
            // Este droide tiene energía aleatoria entre 100-150 y velocidad aleatoria entre 10-30
            <= Configuration.ProbSw421 => new Sw421(
                new Random().Next(Configuration.MinEnergiaSw421, Configuration.MaxEnergiaSw421 + 1), 
                new Random().Next(Configuration.MinVelocidadSw421, Configuration.MaxVelocidadSw421 + 1)),
            
            // SW6969: Probabilidad del 81-100 (20% del total)
            // Este droide tiene energía de 200, defensa de 7 y velocidad de 20
            // Es el más complejo: tiene defensa, movimiento y puede explotar
            _ => new Sw6969(
                Configuration.EnergiaSw6969, 
                Configuration.DefensaSw6969, 
                Configuration.VelocidadSw6969)
        };
    }
}