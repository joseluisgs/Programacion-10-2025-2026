namespace StarWars.Config;

/// <summary>
/// Configuración centralizada del juego.
/// </summary>
public static class Configuration
{
    // ============================================
    // PARÁMETROS DEL JUEGO
    // ============================================
    
    /// <summary>Dimensión mínima del mapa</summary>
    public const int MinDimension = 5;
    
    /// <summary>Dimensión máxima del mapa</summary>
    public const int MaxDimension = 9;
    
    /// <summary>Dimensión por defecto del mapa</summary>
    public const int DefaultDimension = 5;
    
    /// <summary>Número mínimo de droides</summary>
    public const int MinDroides = 5;
    
    /// <summary>Número máximo de droides</summary>
    public const int MaxDroides = 30;
    
    /// <summary>Número por defecto de droides</summary>
    public const int DefaultDroides = 10;
    
    /// <summary>Tiempo mínimo en segundos</summary>
    public const int MinTiempo = 1;
    
    /// <summary>Tiempo máximo en segundos</summary>
    public const int MaxTiempo = 3;
    
    /// <summary>Tiempo por defecto en segundos</summary>
    public const int DefaultTiempo = 1;

    // ============================================
    // PROBABILIDADES DE APARICIÓN DE DROIDES
    // ============================================
    
    /// <summary>Porcentaje máximo para SW348 (20%)</summary>
    public const int ProbSw348 = 20;
    
    /// <summary>Porcentaje máximo para SW447 (40%)</summary>
    public const int ProbSw447 = 60;
    
    /// <summary>Porcentaje máximo para SW421 (20%)</summary>
    public const int ProbSw421 = 80;
    
    /// <summary>Porcentaje para SW6969 (20%)</summary>
    public const int ProbSw6969 = 100;

    // ============================================
    // ENERGÍA DE LOS DROIDES
    // ============================================
    
    /// <summary>Energía de SW348</summary>
    public const int EnergiaSw348 = 50;
    
    /// <summary>Energía máxima de SW421</summary>
    public const int MaxEnergiaSw421 = 150;
    
    /// <summary>Energía mínima de SW421</summary>
    public const int MinEnergiaSw421 = 100;
    
    /// <summary>Energía de SW6969</summary>
    public const int EnergiaSw6969 = 200;
    
    /// <summary>Energía de SW447</summary>
    public const int EnergiaSw447 = 100;

    // ============================================
    // DEFENSA DE LOS DROIDES
    // ============================================
    
    /// <summary>Defensa mínima de SW348</summary>
    public const int MinDefensaSw348 = 9;
    
    /// <summary>Defensa máxima de SW348</summary>
    public const int MaxDefensaSw348 = 12;
    
    /// <summary>Defensa de SW6969</summary>
    public const int DefensaSw6969 = 7;

    // ============================================
    // ESCUDO DE LOS DROIDES
    // ============================================
    
    /// <summary>Escudo mínimo de SW447</summary>
    public const int MinEscudoSw447 = 5;
    
    /// <summary>Escudo máximo de SW447</summary>
    public const int MaxEscudoSw447 = 10;

    // ============================================
    // VELOCIDAD DE LOS DROIDES
    // ============================================
    
    /// <summary>Velocidad mínima de SW421</summary>
    public const int MinVelocidadSw421 = 10;
    
    /// <summary>Velocidad máxima de SW421</summary>
    public const int MaxVelocidadSw421 = 30;
    
    /// <summary>Velocidad de SW6969</summary>
    public const int VelocidadSw6969 = 20;

    // ============================================
    // DISPAROS
    // ============================================
    
    /// <summary>Daño del disparo normal</summary>
    public const int DañoDisparoNormal = 25;
    
    /// <summary>Daño del disparo crítico</summary>
    public const int DañoDisparoCritico = 50;
    
    /// <summary>Porcentaje de probabilidad de disparo crítico</summary>
    public const int ProbDisparoCritico = 15;

    // ============================================
    // TIEMPOS (en milisegundos)
    // ============================================
    
    /// <summary>Intervalo entre disparos (100ms)</summary>
    public const int IntervaloDisparo = 100;
    
    /// <summary>Intervalo de movimiento de droides (300ms)</summary>
    public const int IntervaloMovimiento = 300;

    // ============================================
    // EXPLOSIÓN DE SW6969
    // ============================================
    
    /// <summary>Porcentaje de probabilidad de explotar</summary>
    public const int ProbExplosion = 30;
}
