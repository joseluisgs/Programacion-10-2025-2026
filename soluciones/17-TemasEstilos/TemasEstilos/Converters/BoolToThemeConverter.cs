using System.Globalization;
using System.Windows;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;

namespace TemasEstilos.Converters;

/// <summary>
/// Convertidor que transforma un valor booleano a BaseTheme de Material Design.
/// </summary>
/// <remarks>
/// True -> BaseTheme.Dark (oscuro)
/// False -> BaseTheme.Light (claro)
/// Se usa para cambiar el tema de Material Design dinámicamente.
/// </remarks>
public class BoolToThemeConverter : IValueConverter
{
    /// <summary>
    /// Convierte un valor booleano a BaseTheme de Material Design.
    /// </summary>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool esOscuro)
        {
            return esOscuro ? BaseTheme.Dark : BaseTheme.Light;
        }
        return BaseTheme.Light;
    }

    /// <summary>
    /// Convierte BaseTheme de vuelta a booleano.
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is BaseTheme theme)
        {
            return theme == BaseTheme.Dark;
        }
        return false;
    }
}