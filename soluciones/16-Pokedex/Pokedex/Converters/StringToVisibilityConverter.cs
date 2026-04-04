// =============================================================================
// CONVERTIDOR DE STRING A VISIBILITY
// =============================================================================
// Convierte una cadena a Visibility.
// Si la cadena tiene contenido, retorna Visible.
// Si está vacía o es null, retorna Collapsed.
// Útil para mostrar/ocultar mensajes de error o información.
// =============================================================================

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Pokedex.Converters;

/// <summary>
/// Convierte una cadena a Visibility.
/// Retorna Visible si la cadena tiene contenido, Collapsed si está vacía.
/// </summary>
public class StringToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Convierte string a Visibility.
    /// </summary>
    /// <param name="value">Valor a evaluar</param>
    /// <param name="targetType">Tipo objetivo</param>
    /// <param name="parameter">Parámetro adicional</param>
    /// <param name="culture">Cultura</param>
    /// <returns>Visible si tiene contenido, Collapsed si está vacío</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Verifica que sea string y que no esté vacío
        if (value is string str && !string.IsNullOrWhiteSpace(str))
        {
            return Visibility.Visible;
        }
        return Visibility.Collapsed;
    }

    /// <summary>No implementado</summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
