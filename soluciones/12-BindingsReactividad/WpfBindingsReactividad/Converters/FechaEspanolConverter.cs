using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfBindingsReactividad.Converters;

/// <summary>
/// Convierte una fecha a formato español (día de la semana y mes en español).
/// </summary>
public class FechaEspanolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTime fecha)
        {
            // Crear cultura española
            var culturaEspanol = new CultureInfo("es-ES");
            
            // Formatear la fecha en español
            return fecha.ToString("dddd, dd 'de' MMMM 'de' yyyy", culturaEspanol);
        }
        
        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}