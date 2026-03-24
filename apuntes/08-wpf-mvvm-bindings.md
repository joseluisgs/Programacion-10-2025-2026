# 08 - Data Binding Avanzado en WPF

## 1. Introducción al Data Binding

El **data binding** (vinculación de datos) es el mecanismo que conecta automáticamente las propiedades de controles UI con propiedades de objetos de datos. Es la característica central que hace a WPF y MVVM tan poderosos.

### 1.1 ¿Por qué Data Binding?

**Sin data binding (imperativo):**

```csharp
// ❌ Tedioso y propenso a errores
textBox.TextChanged += (s, e) =>
{
    persona.Nombre = textBox.Text;
    labelSaludo.Content = $"Hola, {persona.Nombre}";
};
```

**Con data binding (declarativo):**

```xml
<!-- ✅ Declarativo y automático -->
<TextBox Text="{Binding Nombre, UpdateSourceTrigger=PropertyChanged}" />
<Label Content="{Binding SaludoCompleto}" />
```

---

## 2. Sintaxis de Binding en XAML

### 2.1 Sintaxis Básica

```xml
<!-- Forma completa -->
<TextBox Text="{Binding Path=Nombre}" />

<!-- Forma corta (Path es implícito) -->
<TextBox Text="{Binding Nombre}" />

<!-- Binding al objeto completo (DataContext) -->
<TextBox Text="{Binding}" />

<!-- Binding a propiedad anidada -->
<TextBox Text="{Binding Direccion.Calle}" />
```

### 2.2 Componentes de un Binding

```xml
<TextBox Text="{Binding Nombre, 
                        Mode=TwoWay, 
                        UpdateSourceTrigger=PropertyChanged,
                        StringFormat='Hola {0}',
                        FallbackValue='Sin nombre',
                        TargetNullValue='(vacío)'}" />
```

| Propiedad | Descripción | Valores |
|-----------|-------------|---------|
| `Path` | Ruta a la propiedad | Nombre de propiedad |
| `Mode` | Dirección de sincronización | OneWay, TwoWay, OneTime, OneWayToSource |
| `UpdateSourceTrigger` | Cuándo actualizar el origen | PropertyChanged, LostFocus, Explicit |
| `StringFormat` | Formato de string | Formato .NET estándar |
| `FallbackValue` | Valor si el binding falla | Cualquier valor |
| `TargetNullValue` | Valor si la fuente es null | Cualquier valor |
| `Converter` | Conversor de valor | IValueConverter |

---

## 3. Modos de Binding

```mermaid
graph LR
    subgraph "OneWay"
        S1[Source] -->|actualiza| T1[Target]
    end
    
    subgraph "TwoWay"
        S2[Source] <-->|sincronizan| T2[Target]
    end
    
    subgraph "OneTime"
        S3[Source] -.->|solo inicial| T3[Target]
    end
    
    subgraph "OneWayToSource"
        S4[Source] <--|actualiza| T4[Target]
    end
```

### 3.1 OneWay (Origen → Destino)

**Uso:** Mostrar datos que el usuario NO debe modificar.

```xml
<!-- La etiqueta se actualiza cuando cambia la propiedad -->
<TextBlock Text="{Binding FechaCreacion, Mode=OneWay}" />
<TextBlock Text="{Binding TotalCompra}" />
<!-- OneWay es el modo por defecto para TextBlock -->
```

### 3.2 TwoWay (Origen ↔ Destino)

**Uso:** Campos editables que modifican el modelo.

```xml
<!-- Los cambios en el TextBox actualizan la propiedad Y viceversa -->
<TextBox Text="{Binding Nombre, Mode=TwoWay}" />
<CheckBox IsChecked="{Binding Activo, Mode=TwoWay}" />
<!-- TwoWay es el modo por defecto para TextBox, CheckBox, etc. -->
```

### 3.3 OneTime (Solo Lectura Inicial)

**Uso:** Valores que se establecen una vez y nunca cambian.

```xml
<!-- Se lee solo al crear el control -->
<TextBlock Text="{Binding IdUsuario, Mode=OneTime}" />
```

### 3.4 OneWayToSource (Destino → Origen)

**Uso:** Controles que solo envían datos, no los reciben.

```xml
<!-- Raro: solo actualiza la fuente desde el control -->
<TextBox Text="{Binding TerminoBusqueda, Mode=OneWayToSource}" />
```

### 3.5 Tabla Comparativa de Modos

| Modo | Flujo | Cuándo Usar | Performance |
|------|-------|-------------|-------------|
| `OneWay` | Source → Target | Datos de solo lectura | ⚡⚡⚡ Mejor |
| `TwoWay` | Source ↔ Target | Campos editables | ⚡⚡ Media |
| `OneTime` | Source → Target (1 vez) | Datos inmutables | ⚡⚡⚡ Mejor |
| `OneWayToSource` | Target → Source | Controles de entrada pura | ⚡⚡ Media |

---

## 4. UpdateSourceTrigger: Cuándo Actualizar

```xml
<!-- Por defecto: al perder el foco -->
<TextBox Text="{Binding Nombre}" />

<!-- Al cambiar cada carácter (inmediato) -->
<TextBox Text="{Binding Busqueda, UpdateSourceTrigger=PropertyChanged}" />

<!-- Explícitamente (manualmente desde código) -->
<TextBox x:Name="txtNombre" Text="{Binding Nombre, UpdateSourceTrigger=Explicit}" />

<!-- Al perder el foco (explícito) -->
<TextBox Text="{Binding Email, UpdateSourceTrigger=LostFocus}" />
```

**Ejemplo con Explicit:**

```csharp
// Code-behind
private void BtnActualizar_Click(object sender, RoutedEventArgs e)
{
    var binding = txtNombre.GetBindingExpression(TextBox.TextProperty);
    binding?.UpdateSource(); // Actualiza manualmente
}
```

### 4.1 Tabla de UpdateSourceTrigger

| Valor | Cuándo Se Actualiza | Uso Típico |
|-------|---------------------|------------|
| `Default` | Depende del control | Comportamiento estándar |
| `PropertyChanged` | Con cada cambio | Búsqueda en tiempo real, validación inmediata |
| `LostFocus` | Al perder el foco | Formularios normales |
| `Explicit` | Manualmente | Control total sobre la actualización |

---

## 5. DataContext: Fuente de Datos

### 5.1 ¿Qué es el DataContext?

El **DataContext** es la fuente de datos por defecto para todos los bindings de un elemento y sus hijos.

```xml
<Window>
    <!-- DataContext del Window -->
    <Window.DataContext>
        <vm:PersonaViewModel />
    </Window.DataContext>
    
    <!-- Todos los bindings aquí usan PersonaViewModel -->
    <StackPanel>
        <TextBox Text="{Binding Nombre}" />
        <TextBox Text="{Binding Apellido}" />
        <TextBox Text="{Binding Email}" />
    </StackPanel>
</Window>
```

### 5.2 Herencia del DataContext

El DataContext se hereda en el árbol visual:

```xml
<Window DataContext="{Binding PersonaViewModel}">
    <!-- Hereda DataContext del Window -->
    <Grid>
        <!-- Hereda DataContext del Grid -->
        <StackPanel>
            <!-- Hereda DataContext del StackPanel -->
            <TextBox Text="{Binding Nombre}" />
        </StackPanel>
    </Grid>
</Window>
```

### 5.3 Cambiar DataContext en Subárboles

```xml
<Window DataContext="{Binding PersonaViewModel}">
    <!-- Usa PersonaViewModel -->
    <TextBox Text="{Binding Nombre}" />
    
    <!-- Cambia el DataContext para este Grid -->
    <Grid DataContext="{Binding Direccion}">
        <!-- Ahora usa DireccionViewModel -->
        <TextBox Text="{Binding Calle}" />
        <TextBox Text="{Binding Ciudad}" />
    </Grid>
</Window>
```

### 5.4 Establecer DataContext en Code-Behind

```csharp
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        // Opción 1: Asignar directamente
        DataContext = new PersonaViewModel();
        
        // Opción 2: Asignar a un control específico
        miGrid.DataContext = new DireccionViewModel();
    }
}
```

---

## 6. Value Converters: Transformar Datos

### 6.1 La Interfaz IValueConverter

Los **converters** transforman valores entre la fuente y el destino.

```csharp
public interface IValueConverter
{
    // Convierte de Source a Target (lectura)
    object Convert(object value, Type targetType, object parameter, CultureInfo culture);
    
    // Convierte de Target a Source (escritura - para TwoWay)
    object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
}
```

### 6.2 Ejemplo: BoolToVisibilityConverter

```csharp
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Converters;

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }
        
        return Visibility.Collapsed;
    }
    
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
        {
            return visibility == Visibility.Visible;
        }
        
        return false;
    }
}
```

**Uso en XAML:**

```xml
<Window xmlns:conv="clr-namespace:Converters">
    <Window.Resources>
        <!-- Registrar el converter como recurso -->
        <conv:BoolToVisibilityConverter x:Key="BoolToVisibility" />
    </Window.Resources>
    
    <!-- Usar el converter -->
    <TextBlock Text="¡Visible!" 
               Visibility="{Binding MostrarMensaje, Converter={StaticResource BoolToVisibility}}" />
</Window>
```

### 6.3 Converters Comunes

#### NullToVisibilityConverter

```csharp
public class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value == null ? Visibility.Collapsed : Visibility.Visible;
    }
    
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
```

#### NumberToColorConverter

```csharp
public class NumberToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int number)
        {
            return number switch
            {
                < 0 => Brushes.Red,
                0 => Brushes.Gray,
                _ => Brushes.Green
            };
        }
        
        return Brushes.Black;
    }
    
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
```

#### InverseBoolConverter

```csharp
public class InverseBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool b ? !b : false;
    }
    
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool b ? !b : false;
    }
}
```

### 6.4 Converter con Parámetros

```csharp
public class ComparisonConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int number && parameter is string paramStr && int.TryParse(paramStr, out int threshold))
        {
            return number > threshold;
        }
        
        return false;
    }
    
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
```

**Uso con parámetro:**

```xml
<TextBlock Text="Alto" 
           Visibility="{Binding Edad, 
                        Converter={StaticResource ComparisonConverter}, 
                        ConverterParameter=18}" />
```

---

## 7. StringFormat: Formateo de Datos

### 7.1 Formatos Numéricos

```xml
<!-- Formato de moneda -->
<TextBlock Text="{Binding Precio, StringFormat='{}{0:C}'}" />
<!-- Salida: $1,234.56 -->

<!-- Formato de número con decimales -->
<TextBlock Text="{Binding Temperatura, StringFormat='{}{0:F2}°C'}" />
<!-- Salida: 23.45°C -->

<!-- Formato de porcentaje -->
<TextBlock Text="{Binding Progreso, StringFormat='{}{0:P0}'}" />
<!-- Salida: 75% -->

<!-- Separador de miles -->
<TextBlock Text="{Binding Poblacion, StringFormat='{}{0:N0}'}" />
<!-- Salida: 1,234,567 -->
```

### 7.2 Formatos de Fecha y Hora

```xml
<!-- Fecha corta -->
<TextBlock Text="{Binding FechaNacimiento, StringFormat='{}{0:d}'}" />
<!-- Salida: 15/03/1990 -->

<!-- Fecha larga -->
<TextBlock Text="{Binding Fecha, StringFormat='{}{0:D}'}" />
<!-- Salida: viernes, 15 de marzo de 1990 -->

<!-- Hora -->
<TextBlock Text="{Binding Hora, StringFormat='{}{0:HH:mm:ss}'}" />
<!-- Salida: 14:30:25 -->

<!-- Fecha y hora personalizada -->
<TextBlock Text="{Binding FechaHora, StringFormat='{}{0:dd/MM/yyyy HH:mm}'}" />
<!-- Salida: 15/03/2025 14:30 -->
```

### 7.3 Formatos con Texto

```xml
<!-- Con texto prefijo -->
<TextBlock Text="{Binding Edad, StringFormat='Edad: {0} años'}" />
<!-- Salida: Edad: 25 años -->

<!-- Multiple valores (requiere MultiBinding) -->
<TextBlock>
    <TextBlock.Text>
        <MultiBinding StringFormat="{}{0} {1}">
            <Binding Path="Nombre" />
            <Binding Path="Apellido" />
        </MultiBinding>
    </TextBlock.Text>
</TextBlock>
<!-- Salida: Juan Pérez -->
```

**Nota:** Los `{}` iniciales en `StringFormat='{}{0}'` son necesarios para escapar las llaves en XAML.

---

## 8. ElementName y RelativeSource

### 8.1 ElementName: Binding a Otro Control

```xml
<StackPanel>
    <!-- Control fuente -->
    <Slider x:Name="sliderVolumen" Minimum="0" Maximum="100" />
    
    <!-- Binding al Slider por nombre -->
    <TextBlock Text="{Binding ElementName=sliderVolumen, Path=Value}" />
    
    <!-- Binding con formato -->
    <TextBlock Text="{Binding ElementName=sliderVolumen, 
                              Path=Value, 
                              StringFormat='Volumen: {0:F0}%'}" />
</StackPanel>
```

**Ejemplo interactivo:**

```xml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="*" />
    </Grid.RowDefinitions>
    
    <Slider x:Name="sliderTamaño" Minimum="10" Maximum="72" Value="16" />
    
    <TextBox Grid.Row="1" 
             Text="Texto con tamaño variable" 
             FontSize="{Binding ElementName=sliderTamaño, Path=Value}" />
</Grid>
```

### 8.2 RelativeSource: Binding Relativo

#### Self: Binding a sí mismo

```xml
<!-- Ancho igual a la altura -->
<Button Width="{Binding RelativeSource={RelativeSource Self}, Path=ActualHeight}" 
        Height="50" 
        Content="Cuadrado" />
```

#### FindAncestor: Binding a un ancestro

```xml
<Window x:Name="ventanaPrincipal" Title="Mi Ventana">
    <StackPanel>
        <Button Content="Cerrar">
            <!-- Binding al comando del Window padre -->
            <Button.Command>
                <Binding RelativeSource="{RelativeSource AncestorType=Window}" 
                         Path="DataContext.CerrarCommand" />
            </Button.Command>
        </Button>
        
        <!-- Binding al título del Window -->
        <TextBlock Text="{Binding RelativeSource={RelativeSource AncestorType=Window}, 
                                  Path=Title}" />
    </StackPanel>
</Window>
```

#### TemplatedParent: En templates

```xml
<ControlTemplate TargetType="Button">
    <Border Background="{TemplateBinding Background}">
        <!-- Equivalente a RelativeSource TemplatedParent -->
        <ContentPresenter Content="{Binding RelativeSource={RelativeSource TemplatedParent}, 
                                           Path=Content}" />
    </Border>
</ControlTemplate>
```

### 8.3 Tabla RelativeSource Modes

| Mode | Descripción | Uso |
|------|-------------|-----|
| `Self` | El propio control | Binding circular dentro del control |
| `FindAncestor` | Busca un ancestro por tipo | Acceder a propiedades de contenedores |
| `PreviousData` | Elemento anterior en lista | Comparar con item anterior |
| `TemplatedParent` | Control que usa el template | Dentro de ControlTemplates |

---

## 9. MultiBinding: Binding Múltiple

### 9.1 IMultiValueConverter

```csharp
public interface IMultiValueConverter
{
    object Convert(object[] values, Type targetType, object parameter, CultureInfo culture);
    object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture);
}
```

### 9.2 Ejemplo: Nombre Completo

```csharp
public class NombreCompletoConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length >= 2 && values[0] is string nombre && values[1] is string apellido)
        {
            return $"{nombre} {apellido}";
        }
        
        return string.Empty;
    }
    
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
```

**Uso:**

```xml
<Window.Resources>
    <local:NombreCompletoConverter x:Key="NombreCompletoConverter" />
</Window.Resources>

<TextBlock>
    <TextBlock.Text>
        <MultiBinding Converter="{StaticResource NombreCompletoConverter}">
            <Binding Path="Nombre" />
            <Binding Path="Apellido" />
        </MultiBinding>
    </TextBlock.Text>
</TextBlock>
```

### 9.3 Ejemplo: Validación Múltiple

```csharp
public class TodosVerdaderosConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return values.All(v => v is bool b && b);
    }
    
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
```

**Uso para habilitar botón:**

```xml
<Button Content="Registrar">
    <Button.IsEnabled>
        <MultiBinding Converter="{StaticResource TodosVerdaderosConverter}">
            <Binding Path="NombreValido" />
            <Binding Path="EmailValido" />
            <Binding Path="AceptaTerminos" />
        </MultiBinding>
    </Button.IsEnabled>
</Button>
```

---

## 10. Ejemplo Completo: Formulario con Bindings Avanzados

### 10.1 ViewModel

```csharp
using CommunityToolkit.Mvvm.ComponentModel;

namespace BindingAvanzado;

public partial class PersonaViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NombreCompleto))]
    [NotifyPropertyChangedFor(nameof(EsValido))]
    private string _nombre = "";
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NombreCompleto))]
    private string _apellido = "";
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EsValido))]
    private string _email = "";
    
    [ObservableProperty]
    private int _edad = 18;
    
    [ObservableProperty]
    private bool _activo = true;
    
    [ObservableProperty]
    private DateTime _fechaNacimiento = DateTime.Now.AddYears(-18);
    
    [ObservableProperty]
    private decimal _sueldo = 1000;
    
    public string NombreCompleto => $"{Nombre} {Apellido}";
    
    public bool EsValido => 
        !string.IsNullOrWhiteSpace(Nombre) && 
        !string.IsNullOrWhiteSpace(Email) && 
        Email.Contains('@');
}
```

### 10.2 Converters

```csharp
// EdadToColorConverter.cs
public class EdadToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int edad)
        {
            return edad switch
            {
                < 18 => Brushes.Red,
                < 65 => Brushes.Green,
                _ => Brushes.Orange
            };
        }
        
        return Brushes.Black;
    }
    
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
```

### 10.3 Vista

```xml
<Window x:Class="BindingAvanzado.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:local="clr-namespace:BindingAvanzado"
        Title="Binding Avanzado" Height="500" Width="500">
    
    <Window.DataContext>
        <local:PersonaViewModel />
    </Window.DataContext>
    
    <Window.Resources>
        <local:BoolToVisibilityConverter x:Key="BoolToVisibility" />
        <local:EdadToColorConverter x:Key="EdadToColor" />
        <local:InverseBoolConverter x:Key="InverseBool" />
    </Window.Resources>
    
    <StackPanel Margin="20">
        <!-- Nombre con UpdateSourceTrigger inmediato -->
        <TextBlock Text="Nombre:" />
        <TextBox Text="{Binding Nombre, UpdateSourceTrigger=PropertyChanged}" 
                 Margin="0,5,0,15" />
        
        <!-- Apellido -->
        <TextBlock Text="Apellido:" />
        <TextBox Text="{Binding Apellido, UpdateSourceTrigger=PropertyChanged}" 
                 Margin="0,5,0,15" />
        
        <!-- Nombre completo (calculado) -->
        <TextBlock Text="Nombre Completo:" FontWeight="Bold" />
        <TextBlock Text="{Binding NombreCompleto}" 
                   FontSize="16" Foreground="DarkBlue" 
                   Margin="0,5,0,15" />
        
        <!-- Email -->
        <TextBlock Text="Email:" />
        <TextBox Text="{Binding Email, UpdateSourceTrigger=PropertyChanged}" 
                 Margin="0,5,0,15" />
        
        <!-- Edad con slider y color dinámico -->
        <TextBlock Text="Edad:" />
        <Slider x:Name="sliderEdad" 
                Value="{Binding Edad}" 
                Minimum="0" Maximum="120" 
                TickFrequency="1" IsSnapToTickEnabled="True" />
        <TextBlock Text="{Binding ElementName=sliderEdad, Path=Value, StringFormat='Edad: {0:F0} años'}" 
                   Foreground="{Binding Edad, Converter={StaticResource EdadToColor}}" 
                   FontWeight="Bold" 
                   Margin="0,5,0,15" />
        
        <!-- Fecha de nacimiento -->
        <TextBlock Text="Fecha de Nacimiento:" />
        <DatePicker SelectedDate="{Binding FechaNacimiento}" 
                    Margin="0,5,0,15" />
        <TextBlock Text="{Binding FechaNacimiento, StringFormat='Nacido el {0:dd/MM/yyyy}'}" 
                   FontSize="12" Foreground="Gray" 
                   Margin="0,0,0,15" />
        
        <!-- Sueldo -->
        <TextBlock Text="Sueldo:" />
        <TextBox Text="{Binding Sueldo, StringFormat=C}" 
                 Margin="0,5,0,15" />
        
        <!-- CheckBox activo -->
        <CheckBox Content="Activo" IsChecked="{Binding Activo}" 
                  Margin="0,10,0,10" />
        
        <!-- Mensaje visible solo si activo -->
        <TextBlock Text="✅ Usuario activo" 
                   Foreground="Green" 
                   Visibility="{Binding Activo, Converter={StaticResource BoolToVisibility}}" 
                   Margin="0,0,0,15" />
        
        <!-- Mensaje visible solo si inactivo -->
        <TextBlock Text="❌ Usuario inactivo" 
                   Foreground="Red" 
                   Visibility="{Binding Activo, Converter={StaticResource BoolToVisibility}, 
                                        ConverterParameter=Inverse}" 
                   Margin="0,0,0,15" />
        
        <!-- Botón habilitado solo si es válido -->
        <Button Content="Guardar" 
                Height="40" 
                IsEnabled="{Binding EsValido}" />
    </StackPanel>
</Window>
```

---

## 11. Debugging de Bindings

### 11.1 Output Window

WPF escribe errores de binding en la ventana de Output:

```
System.Windows.Data Error: 40 : BindingExpression path error: 'Nombre' property not found...
```

### 11.2 PresentationTraceSources

```xml
<TextBox Text="{Binding Nombre, 
                        diag:PresentationTraceSources.TraceLevel=High}" 
         xmlns:diag="clr-namespace:System.Diagnostics;assembly=WindowsBase" />
```

### 11.3 FallbackValue para Debugging

```xml
<!-- Muestra "ERROR" si el binding falla -->
<TextBox Text="{Binding PropiedadInexistente, FallbackValue='ERROR'}" />
```

---

## 12. Resumen

| Concepto | Descripción |
|----------|-------------|
| Data Binding | Sincronización automática entre UI y datos |
| Mode | Dirección de sincronización (OneWay, TwoWay, etc.) |
| UpdateSourceTrigger | Cuándo actualizar la fuente |
| DataContext | Fuente de datos por defecto |
| IValueConverter | Transforma valores entre fuente y destino |
| StringFormat | Formatea strings en bindings |
| ElementName | Binding a otro control por nombre |
| RelativeSource | Binding relativo (Self, FindAncestor) |
| MultiBinding | Binding de múltiples valores |

---

## 13. Ejercicios Propuestos

1. **Conversor de Temperatura**: Crea bindings bidireccionales entre Celsius, Fahrenheit y Kelvin usando converters.

2. **Validación Visual**: Implementa un formulario donde los campos cambien de color según validación usando converters.

3. **Calculadora de IMC**: Usa MultiBinding para calcular IMC desde peso y altura en tiempo real.

4. **Tabla Dinámica**: Crea una tabla donde el tamaño de fuente se controle con un slider usando ElementName.

---

## 14. Referencias

- [Data Binding Overview](https://learn.microsoft.com/dotnet/desktop/wpf/data/)
- [Binding Modes](https://learn.microsoft.com/dotnet/desktop/wpf/data/binding-declarations-overview)
- [Value Converters](https://learn.microsoft.com/dotnet/desktop/wpf/data/how-to-convert-bound-data)

Ver ejemplos completos en `/soluciones/08-wpf-bindings/`

---

*Documento elaborado para el módulo de Programación del ciclo formativo 1º DAW · Curso 2025-2026*
