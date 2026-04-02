using System.Globalization;
using System.Windows.Data;

namespace StarWars.Converters;

/// <summary>
/// Convertidor que invierte el valor de un booleano.
/// Se utiliza para habilitar/deshabilitar controles cuando la simulación está en ejecución.
/// </summary>
public class InverseBooleanConverter : IValueConverter
{
    /// <summary>
    /// Convierte un valor booleano a su valor inverso
    /// </summary>
    /// <param name="value">El valor booleano a invertir</param>
    /// <param name="targetType">El tipo de destino</param>
    /// <param name="parameter">Parámetro adicional</param>
    /// <param name="culture">Cultura actual</param>
    /// <returns>El valor booleano invertido</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }
        
        return value;
    }

    /// <summary>
    /// Convierte un valor inverso de vuelta al valor original
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }
        
        return value;
    }
}
