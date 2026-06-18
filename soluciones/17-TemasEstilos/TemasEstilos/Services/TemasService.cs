using TemasEstilos.Models;

namespace TemasEstilos.Services;

/// <summary>
/// Interfaz para el servicio de gestión de temas.
/// </summary>
public interface ITemasService
{
    /// <summary>
    /// Obtiene todos los temas disponibles.
    /// </summary>
    List<ColorPaleta> GetTemas();

    /// <summary>
    /// Obtiene el tema actual.
    /// </summary>
    ColorPaleta GetTemaActual();

    /// <summary>
    /// Cambia el tema actual.
    /// </summary>
    void CambiarTema(string nombreTema);

    /// <summary>
    /// Evento que se dispara cuando cambia el tema.
    /// </summary>
    event Action<ColorPaleta>? TemaCambiado;
}

/// <summary>
/// Servicio que gestiona los temas de la aplicación.
/// </summary>
/// <remarks>
/// Proporciona una lista de temas predefinidos y permite cambiarlos.
/// Al cambiar el tema, se dispara un evento para que la UI se actualice.
/// </remarks>
public class TemasService : ITemasService
{
    /// <summary>
    /// Lista de temas disponibles.
    /// </summary>
    private readonly List<ColorPaleta> _temas;

    /// <summary>
    /// Tema actualmente seleccionado.
    /// </summary>
    private ColorPaleta _temaActual;

    /// <summary>
    /// Evento que notifica el cambio de tema.
    /// </summary>
    public event Action<ColorPaleta>? TemaCambiado;

    /// <summary>
    /// Constructor que inicializa los temas.
    /// </summary>
    public TemasService()
    {
        // Inicializar la lista de temas disponibles
        _temas = new List<ColorPaleta>
        {
            // Tema Claro (por defecto)
            new ColorPaleta
            {
                Nombre = "Claro",
                ColorFondo = "#FFFFFF",
                ColorFondoSecundario = "#F5F5F5",
                ColorTexto = "#212121",
                ColorTextoSecundario = "#757575",
                ColorBoton = "#0078D4",
                ColorBorde = "#E0E0E0",
                ColorAcento = "#0078D4",
                EsOscuro = false
            },

            // Tema Oscuro
            new ColorPaleta
            {
                Nombre = "Oscuro",
                ColorFondo = "#1E1E1E",
                ColorFondoSecundario = "#2D2D2D",
                ColorTexto = "#FFFFFF",
                ColorTextoSecundario = "#B0B0B0",
                ColorBoton = "#3B82F6",
                ColorBorde = "#404040",
                ColorAcento = "#60A5FA",
                EsOscuro = true
            },

            // Tema Azul ( empresarial)
            new ColorPaleta
            {
                Nombre = "Azul",
                ColorFondo = "#F0F7FF",
                ColorFondoSecundario = "#E1EFFE",
                ColorTexto = "#1A365D",
                ColorTextoSecundario = "#4A5568",
                ColorBoton = "#2B6CB0",
                ColorBorde = "#90CDF4",
                ColorAcento = "#3182CE",
                EsOscuro = false
            },

            // Tema Verde ( naturalez)
            new ColorPaleta
            {
                Nombre = "Verde",
                ColorFondo = "#F0FFF4",
                ColorFondoSecundario = "#C6F6D5",
                ColorTexto = "#22543D",
                ColorTextoSecundario = "#48BB78",
                ColorBoton = "#38A169",
                ColorBorde = "#9AE6B4",
                ColorAcento = "#48BB78",
                EsOscuro = false
            },

            // Tema Alto Contraste
            new ColorPaleta
            {
                Nombre = "Alto Contraste",
                ColorFondo = "#000000",
                ColorFondoSecundario = "#1A1A1A",
                ColorTexto = "#FFFFFF",
                ColorTextoSecundario = "#E0E0E0",
                ColorBoton = "#FFFF00",
                ColorBorde = "#FFFFFF",
                ColorAcento = "#00FF00",
                EsOscuro = true
            }
        };

        // Establecer el tema por defecto
        _temaActual = _temas[0];
    }

    /// <summary>
    /// Obtiene todos los temas disponibles.
    /// </summary>
    public List<ColorPaleta> GetTemas() => _temas;

    /// <summary>
    /// Obtiene el tema actual.
    /// </summary>
    public ColorPaleta GetTemaActual() => _temaActual;

    /// <summary>
    /// Cambia el tema actual.
    /// </summary>
    /// <param name="nombreTema">Nombre del tema a aplicar</param>
    public void CambiarTema(string nombreTema)
    {
        var tema = _temas.FirstOrDefault(t => t.Nombre == nombreTema);
        if (tema != null && tema != _temaActual)
        {
            _temaActual = tema;
            // Notificar a los suscriptores que el tema cambió
            TemaCambiado?.Invoke(_temaActual);
        }
    }
}