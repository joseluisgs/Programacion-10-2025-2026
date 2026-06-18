using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TemasEstilos.Converters;

/// <summary>
/// Convertidor que transforma un valor booleano en visibilidad inversa.
/// </summary>
/// <remarks>
/// True = Collapsed (oculto)
/// False = Visible (visible)
/// Se usa para mostrar/ocultar elementos según el modo de Material Design.
/// </remarks>
public class InverseBoolToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Convierte un valor booleano a visibilidad inversa.
    /// </summary>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            // Invierte la lógica: true -> Collapsed, false -> Visible
            return boolValue ? Visibility.Collapsed : Visibility.Visible;
        }
        return Visibility.Visible;
    }

    /// <summary>
    /// Convierte visibilidad de vuelta a booleano.
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
        {
            return visibility != Visibility.Visible;
        }
        return false;
    }
}