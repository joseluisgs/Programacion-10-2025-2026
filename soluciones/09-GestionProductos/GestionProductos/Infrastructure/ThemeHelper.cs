// ============================================================
// ThemeHelper.cs - Ayudante para cambiar temas dinámicamente
// ============================================================
// Permite cambiar entre tema claro y oscuro en tiempo de ejecución
//
// USO:
//   ThemeHelper.SetTheme(isDark: true);   // Tema oscuro
//   ThemeHelper.SetTheme(isDark: false);  // Tema claro
//   ThemeHelper.IsDarkTheme;              // Verificar tema actual

using System.Windows;
using MaterialDesignThemes.Wpf;

namespace GestionProductos.Infrastructure;

/// <summary>
/// Clase estática para gestionar el tema de la aplicación.
/// </summary>
public static class ThemeHelper
{
    /// <summary>
    /// Indica si el tema actual es oscuro.
    /// </summary>
    public static bool IsDarkTheme { get; private set; }

    /// <summary>
    /// Cambia el tema de la aplicación entre claro y oscuro.
    /// </summary>
    /// <param name="isDark">True para tema oscuro, false para tema claro</param>
    public static void SetTheme(bool isDark)
    {
        // Obtener el tema actual
        var paletteHelper = new PaletteHelper();
        var theme = paletteHelper.GetTheme();

        // Establecer el tema base
        theme.SetBaseTheme(isDark ? BaseTheme.Dark : BaseTheme.Light);

        // Aplicar los cambios
        paletteHelper.SetTheme(theme);

        // Guardar el estado actual
        IsDarkTheme = isDark;
    }

    /// <summary>
    /// Invierte el tema actual.
    /// </summary>
    public static void ToggleTheme()
    {
        SetTheme(!IsDarkTheme);
    }

    /// <summary>
    /// Inicializa el tema por defecto.
    /// </summary>
    public static void Initialize()
    {
        // Por defecto, tema claro
        SetTheme(false);
    }
}
