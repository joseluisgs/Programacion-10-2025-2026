using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace Pokedex.Converters;

public class GenerationToRomanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string generation) return string.Empty;
        
        var match = Regex.Match(generation, @"\d+", RegexOptions.IgnoreCase);
        if (match.Success && int.TryParse(match.Value, out int genNum))
        {
            return genNum switch
            {
                1 => "I", 2 => "II", 3 => "III", 4 => "IV", 5 => "V",
                6 => "VI", 7 => "VII", 8 => "VIII", 9 => "IX", 10 => "X",
                _ => genNum.ToString()
            };
        }
        
        return generation.ToUpper().Replace("GEN ", "");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
