using System.Globalization;
using System.Windows.Data;

namespace TemasEstilos.Converters;

/// <summary>
/// Convertidor versátil que transforma un valor booleano en texto dinámico.
/// Parámetro: "MODO" para Claro/Oscuro, "SISTEMA" para Personalizado/Material.
/// </summary>
public class BoolToTextoConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            var modo = parameter?.ToString()?.ToUpper() ?? "MODO";
            
            if (modo == "SISTEMA")
                return boolValue ? "✨ Material Design" : "🎨 Personalizado";
                
            return boolValue ? "🌙 Modo Oscuro" : "☀️ Modo Claro";
        }
        
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}