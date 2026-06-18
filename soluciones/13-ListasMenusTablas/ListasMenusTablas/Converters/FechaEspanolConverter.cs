using System;
using System.Globalization;
using System.Windows.Data;

namespace ListasMenusTablas.Converters;

/// <summary>
/// Convierte una fecha a formato español.
/// </summary>
public class FechaEspanolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTime fecha)
        {
            var culturaEspanol = new CultureInfo("es-ES");
            return fecha.ToString("dd 'de' MMMM 'de' yyyy", culturaEspanol);
        }
        
        return "No seleccionada";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}