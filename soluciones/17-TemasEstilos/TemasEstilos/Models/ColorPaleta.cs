namespace TemasEstilos.Models;

/// <summary>
/// Modelo que representa una paleta de colores para un tema.
/// </summary>
/// <remarks>
/// Este modelo define los colores que se usarán en la interfaz
/// cuando se aplica un tema específico. Permite cambiar dinámicamente
/// la apariencia de la aplicación.
/// </remarks>
public class ColorPaleta
{
    /// <summary>
    /// Nombre del tema.
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Color de fondo principal.
    /// </summary>
    public string ColorFondo { get; set; } = "#FFFFFF";

    /// <summary>
    /// Color de fondo secundario.
    /// </summary>
    public string ColorFondoSecundario { get; set; } = "#F0F0F0";

    /// <summary>
    /// Color del texto principal.
    /// </summary>
    public string ColorTexto { get; set; } = "#000000";

    /// <summary>
    /// Color del texto secundario.
    /// </summary>
    public string ColorTextoSecundario { get; set; } = "#666666";

    /// <summary>
    /// Color de los botones.
    /// </summary>
    public string ColorBoton { get; set; } = "#0078D4";

    /// <summary>
    /// Color del borde.
    /// </summary>
    public string ColorBorde { get; set; } = "#CCCCCC";

    /// <summary>
    /// Color de acento.
    /// </summary>
    public string ColorAcento { get; set; } = "#FF6600";

    /// <summary>
    /// Indica si el tema es oscuro.
    /// </summary>
    public bool EsOscuro { get; set; } = false;
}