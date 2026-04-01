// =============================================================================
// CONVERTIDOR DE LISTA A STRING
// =============================================================================
// Los convertidores en WPF permiten transformar datos para mostrarlos en la UI.
// Este convertidor toma una lista y la convierte en una cadena separada por comas.
// Ejemplo: ["Fire", "Flying"] -> "Fire, Flying"
// =============================================================================

using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Pokedex.Converters;

/// <summary>
/// Convierte una lista/enumerable en una cadena separada por comas.
/// Útil para mostrar listas de tipos, habilidades, etc. en TextBlocks.
/// </summary>
public class ListToStringConverter : IValueConverter
{
    /// <summary>
    /// Convierte una lista a string separado por comas.
    /// </summary>
    /// <param name="value">Lista a convertir</param>
    /// <param name="targetType">Tipo objetivo (no usado)</param>
    /// <param name="parameter">Parámetro adicional (no usado)</param>
    /// <param name="culture">Cultura (no usada)</param>
    /// <returns>Cadena con los elementos separados por coma</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Verifica que el valor sea enumerable
        if (value is not System.Collections.IEnumerable enumerable)
            return string.Empty;

        // Convierte cada elemento a string y une con ", "
        var items = enumerable.Cast<object>();
        return string.Join(", ", items);
    }

    /// <summary>
    /// No implementado - no permitimos edición de listas desde la UI.
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
