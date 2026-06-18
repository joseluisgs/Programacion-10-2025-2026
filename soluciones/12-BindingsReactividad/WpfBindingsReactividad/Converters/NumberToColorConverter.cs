// ============================================================
// NumberToColorConverter.cs
// ============================================================
//
// =================================================================
// GUIA PARA EL ALUMNO: TRANSFORMANDO NUMEROS EN COLORES
// =================================================================
//
// PROBLEMA: A veces queremos que la UI reaccione visualmente
//           segun el valor de un numero.
//
// EJEMPLO:
//   - Un slider de temperatura: 
//     * Frio (< 10) -> Azul
//     * Normal (10-25) -> Verde
//     * Calor (> 25) -> Rojo
//
//   - Un indicador de nivel:
//     * Bajo (< 30%) -> Rojo
//     * Medio (30-70%) -> Amarillo
//     * Alto (> 70%) -> Verde
//
// QUE HACEMOS?
//   - Usamos un Converter que analiza el numero
//   - Devuelve un color (Brush) segun el rango
//
// POR QUE USAR COLORES?
//   - Feedback visual inmediato al usuario
//   - Entendemos mejor los datos sin leer numeros
//   - Interfaz mas intuitiva y profesional
//
// EN RESUMEN: Converter transforma numeros en algo visual
//             que el usuario puede interpretar rapidamente.
//

using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfBindingsReactividad.Converters;

/// <summary>
/// Convierte un número en un color según rangos predefinidos.
/// </summary>
// =================================================================
// PRACTICO: Para que sirve este converter?
// =================================================================
// Convierte temperatura en color:
//   - Temperatura < 10  -> Azul (frio)
//   - Temperatura 10-25  -> Verde (normal)
//   - Temperatura > 25   -> Rojo (calor)
//
// USO EN XAML:
//   <Rectangle Fill="{Binding Temperatura,
//                     Converter={StaticResource NumberToColor}}"/>
//
public class NumberToColorConverter : IValueConverter
{
    /// <summary>
    /// Convierte un número en un Brush de color según rangos.
    /// </summary>
    /// <param name="value">Valor numérico (típicamente de tipo int o double)</param>
    /// <param name="targetType">Tipo destino (Brush para Fill)</param>
    /// <param name="parameter">Parámetro opcional para personalizar rangos</param>
    /// <param name="culture">Cultura actual</param>
    /// <returns>Brush de color según el rango del valor</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // value es el numero que viene del ViewModel
        // Lo analizamos y devolvemos un color
        
        if (value is int intValue)
        {
            return GetColorForValue(intValue);
        }
        
        if (value is double doubleValue)
        {
            return GetColorForValue((int)doubleValue);
        }
        
        // Por defecto, gris si no es un numero valido
        return new SolidColorBrush(Colors.Gray);
    }

    private static SolidColorBrush GetColorForValue(int value)
    {
        // Definimos los rangos de temperatura
        //   < 10  -> Frio -> Azul
        //   10-25 -> Normal -> Verde
        //   > 25  -> Calor -> Rojo
        
        return value switch
        {
            < 10 => new SolidColorBrush(Color.FromRgb(33, 150, 243)),   // Azul
            <= 25 => new SolidColorBrush(Color.FromRgb(76, 175, 80)),  // Verde
            _ => new SolidColorBrush(Color.FromRgb(244, 67, 54))       // Rojo
        };
    }

    /// <summary>
    /// Convierte un color de vuelta a número (no implementado).
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException("No soportamos convertir color a numero");
    }
}
