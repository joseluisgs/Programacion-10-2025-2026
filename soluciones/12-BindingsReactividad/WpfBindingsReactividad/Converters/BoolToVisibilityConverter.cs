// ============================================================
// BoolToVisibilityConverter.cs
// ============================================================
//
// =================================================================
// GUIA PARA EL ALUMNO: POR QUE NECESITAMOS CONVERTERS?
// =================================================================
//
// PROBLEMA: La UI y los datos pueden tener tipos diferentes.
//
// EJEMPLO:
//   - ViewModel tiene: bool EsMayorDeEdad (true/false)
//   - UI necesita: Visibility (Visible/Collapsed)
//   
//   Si bindeamos directamente:
//     <TextBlock Visibility="{Binding EsMayorDeEdad}"/>
//   - WPF no sabe convertir bool a Visibility
//   - ERROR en tiempo de ejecucion
//
// SOLUCION: Usar un Converter que transforme un tipo en otro.
//
// QUE HACE UN CONVERTER?
//   - Recibe un valor de un tipo
//   - Devuelve un valor de otro tipo
//   - La UI puede usar cualquier tipo de dato
//
// QUE CONSEGUIMOS?
//   - La UI puede mostrar lo que quieras segun el dato
//   - Separacion: el ViewModel no necesita conocer la UI
//   - Reutilizable: el mismo converter en muchos sitios
//
// EN RESUMEN: Converter es la forma de transformar datos
//             para que la UI pueda mostrarlos de otra forma.
//

using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfBindingsReactividad.Converters;

/// <summary>
/// Convierte bool a Visibility.
/// </summary>
// =================================================================
// PRACTICO: Para que sirve este converter?
// =================================================================
// Si EsMayorDeEdad = true  -> muestra el contenido
// Si EsMayorDeEdad = false -> oculta el contenido
//
// USO EN XAML:
//   <TextBlock Text="Contenido para adultos"
//              Visibility="{Binding EsMayorDeEdad,
//                              Converter={StaticResource BoolToVisibility}}"/>
//
public class BoolToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Convierte bool a Visibility.
    /// Se llama automaticamente cuando cambia el valor del binding.
    /// </summary>
    /// <param name="value">Valor del ViewModel (bool)</param>
    /// <param name="targetType">Tipo que espera la UI (Visibility)</param>
    /// <param name="parameter">Parametro opcional</param>
    /// <param name="culture">Cultura actual</param>
    /// <returns>Visibility.Visible si true, Visibility.Collapsed si false</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // value es el valor que viene del ViewModel (bool)
        // Lo convertimos a Visibility
        
        if (value is bool boolValue)
        {
            // bool true  -> Visibility.Visible (mostrar)
            // bool false -> Visibility.Collapsed (ocultar)
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }
        
        // Si no es bool, por defecto ocultamos
        return Visibility.Collapsed;
    }

    /// <summary>
    /// Convierte Visibility a bool (operacion inversa).
    /// Se usa cuando el binding es TwoWay.
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
        {
            return visibility == Visibility.Visible;
        }
        
        return false;
    }
}
