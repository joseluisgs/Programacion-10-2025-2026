namespace StarWars.Models;

/// <summary>
/// Clase abstracta que representa un droide genérico en el juego.
/// Es la clase base para todos los tipos de droides (SW348, SW447, SW421, SW6969).
/// </summary>
/// <remarks>
/// Esta clase sigue el patrón de diseño "Abstract Factory" implícito, donde cada
/// subclase define un comportamiento específico. También usa encapsulamiento
/// para proteger el estado del droide.
/// </remarks>
public abstract class Droide
{
    // ============================================
    // ATRIBUTOS ESTÁTICOS (ESTADO COMPARTIDO)
    // ============================================
    
    /// <summary>
    /// Contador estático para asignar IDs únicos a cada droide.
    /// Se incrementa cada vez que se crea un nuevo droide.
    /// </summary>
    /// <example>
    /// Primer droide creado tendrá Id=1, el segundo Id=2, etc.
    /// </example>
    private static int _nextId = 1;

    // ============================================
    // ATRIBUTOS DE INSTANCIA (ESTADO PROPIO)
    // ============================================
    
    /// <summary>
    /// Fecha y hora de creación del droide.
    /// Se usa para tracking temporal si fuera necesario.
    /// </summary>
    protected readonly DateTime CreatedAt = DateTime.Now;

    /// <summary>
    /// Constructor protegido (solo accesible desde subclases).
    /// Inicializa la energía máxima y asigna un ID único al droide.
    /// </summary>
    /// <param name="maxEnergy">Energía máxima inicial del droide</param>
    protected Droide(int maxEnergy)
    {
        MaxEnergy = maxEnergy;
        Id = _nextId++;  // Asigna ID único y lo incrementa
    }

    // ============================================
    // PROPIEDADES PÚBLICAS
    // ============================================
    
    /// <summary>
    /// Identificador único del droide.
    /// Se asigna automáticamente al crear el droide.
    /// </summary>
    public int Id { get; }
    
    /// <summary>
    /// Energía actual del droide.
    /// Cuando llega a 0, el droide se considera "muerto".
    /// </summary>
    public int MaxEnergy { get; set; }

    /// <summary>
    /// Indica si el droide está vivo.
    /// Un droide está vivo si su energía es mayor que 0.
    /// </summary>
    /// <value>True si MaxEnergy > 0, false en caso contrario</value>
    public bool IsAlive => MaxEnergy > 0;

    /// <summary>
    /// Símbolo visual que representa al droide en el cuadrante.
    /// Cada subclase puede redefinir este valor.
    /// </summary>
    /// <remarks>
    /// Virtual permite que las subclases lo sobrescriban si necesitan un símbolo diferente.
    /// El valor por defecto es "❓" (droide desconocido).
    /// </remarks>
    public virtual string Simbolo => "❓";

    /// <summary>
    /// Tipo de droide (enum).
    /// Se usa para identificar qué tipo de droide es.
    /// </summary>
    public abstract TipoDroide Model { get; }

    /// <summary>
    /// Representación en texto del droide.
    /// Útil para depuración y para mostrar en los logs.
    /// </summary>
    /// <returns>String con formato: "Droide-TIPO(id=X, maxEnergy=Y)"</returns>
    public override string ToString()
    {
        return $"Droide-{Model}(id={Id}, maxEnergy={MaxEnergy})";
    }

    // ============================================
    // ENUMERACIÓN INTERNA
    // ============================================
    
    /// <summary>
    /// Enum que define los tipos de droides disponibles en el juego.
    /// </summary>
    public enum TipoDroide
    {
        SW348,   // Droide básico con defensa
        SW447,   // Droide con escudo
        SW421,   // Droide veloz (movimiento)
        SW6969   // Droide inestable (explosión)
    }
}
