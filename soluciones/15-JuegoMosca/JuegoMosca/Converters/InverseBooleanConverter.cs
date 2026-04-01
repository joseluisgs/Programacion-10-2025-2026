// =============================================================================
// CONVERTIDOR DE BOOLEANO INVERSO
// =============================================================================
// Los convertidores en WPF permiten transformar datos para mostrarlos en la UI.
// Este convertidor invierte un valor booleano:
// - true -> false
// - false -> true
// =============================================================================

using System;
using System.Globalization;
using System.Windows.Data;

namespace JuegoMosca.Converters;

/// <summary>
/// Convierte un valor booleano a su inverso.
/// Útil para binding de propiedades Enabled/Disabled.
/// </summary>
public class InverseBooleanConverter : IValueConverter
{
    /// <summary>
    /// Convierte true a false y viceversa.
    /// </summary>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
            return !boolValue;
        return false;
    }

    /// <summary>
    /// Convierte de vuelta (invierte otra vez).
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
            return !boolValue;
        return false;
    }
}
