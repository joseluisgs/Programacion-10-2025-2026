using System.Globalization;
using System.Windows.Data;

namespace TemasEstilos.Converters;

/// <summary>
/// Convertidor que transforma un valor booleano en texto "Claro" / "Oscuro".
/// </summary>
public class BoolToTextoConverter : IValueConverter
{
    /// <summary>
    /// Convierte un valor booleano a texto.
    /// </summary>
    /// <param name="value">Valor booleano</param>
    /// <param name="targetType">Tipo de destino</param>
    /// <param name="parameter">Parámetro adicional</param>
    /// <param name="culture">Cultura actual</param>
    /// <returns>"Oscuro" si true, "Claro" si false</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue ? "Modo Oscuro" : "Modo Claro";
        }
        
        return value;
    }

    /// <summary>
    /// Convierte texto de vuelta a booleano.
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}