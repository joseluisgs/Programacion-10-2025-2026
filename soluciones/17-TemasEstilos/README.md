# 17-TemasEstilos

## Descripción

Aplicación de demostración de **temas, estilos dinámicos y Material Design en WPF**. Permite cambiar la apariencia de la aplicación en tiempo real entre temas personalizados y el sistema de diseño Material Design.

## Objetivos de Aprendizaje

- Entender cómo funcionan los recursos y estilos en WPF
- Implementar cambio de temas dinámicamente en tiempo de ejecución
- Usar data binding para conectar el ViewModel con los estilos
- Integrar **Material Design Themes** para obtener una apariencia profesional
- Aprender a usar triggers, converters y estilos heredados
- Inyección de dependencias para el servicio de temas

## Requisitos Funcionales

- RF-01: Mostrar lista de temas personalizados (Claro, Oscuro, Azul, Verde, Alto Contraste)
- RF-02: Cambiar tema personalizado mediante ComboBox con selección
- RF-03: Cambiar tema personalizado mediante botones de cambio rápido
- RF-04: Integrar biblioteca **Material Design Themes** (MaterialDesignThemes)
- RF-05: Mostrar paletas de colores de Material Design (8 opciones)
- RF-06: Alternar entre modo claro y oscuro de Material Design
- RF-07: Toggle para cambiar entre temas personalizados y Material Design
- RF-08: Los estilos se aplican dinámicamente a todos los componentes

## Tecnologías

- WPF (.NET 10)
- C# 14
- CommunityToolkit.Mvvm
- Microsoft.Extensions.DependencyInjection
- **MaterialDesignThemes** (v5.1.0)

## Estructura

```
17-TemasEstilos/
├── TemasEstilos/
│   ├── Models/
│   │   └── ColorPaleta.cs           # Modelo de paleta de colores personalizada
│   ├── Services/
│   │   └── TemasService.cs          # Servicio de gestión de temas
│   ├── ViewModels/
│   │   └── MainViewModel.cs         # ViewModel principal con temas personalizados y Material
│   ├── Views/
│   │   ├── MainWindow.xaml          # Ventana principal con UI completa
│   │   └── MainWindow.xaml.cs       # Code-behind
│   ├── Converters/
│   │   ├── BoolToTextoConverter.cs      # Bool a texto "Modo Claro/Oscuro"
│   │   ├── InverseBoolToVisibilityConverter.cs  # Visibilidad inversa
│   │   └── BoolToThemeConverter.cs      # Bool a BaseTheme de Material
│   ├── Infrastructure/
│   │   └── DependenciesProvider.cs  # Proveedor DI
│   ├── App.xaml                     # Aplicación con recursos de Material
│   └── App.xaml.cs                  # Punto de entrada
└── README.md
```

## Conceptos Clave Explicados

### 1. Estilos en WPF

Los estilos en WPF permiten definir un conjunto de propiedades que se aplicarán a múltiples controles:

```xml
<!-- Definir un estilo en recursos -->
<Window.Resources>
    <Style x:Key="MiBoton" TargetType="Button">
        <Setter Property="Background" Value="Blue"/>
        <Setter Property="Foreground" Value="White"/>
    </Style>
</Window.Resources>

<!-- Usar el estilo -->
<Button Style="{StaticResource MiBoton}">Texto</Button>
```

### 2. Binding de Estilos Dinámicos

Para que los estilos cambien dinámicamente, usamos binding directamente en las propiedades del estilo:

```xml
<!-- El color del botón se bindea al tema actual -->
<Setter Property="Background" Value="{Binding TemaSeleccionado.ColorBoton}"/>
```

### 3. Triggers

Los triggers permiten cambiar propiedades según condiciones:

```xml
<Style TargetType="Button">
    <Style.Triggers>
        <Trigger Property="IsMouseOver" Value="True">
            <Setter Property="Opacity" Value="0.9"/>
        </Trigger>
    </Style.Triggers>
</Style>
```

### 4. Material Design Integration

**Material Design Themes** es una biblioteca popular que proporciona estilos predefinidos:

```xml
<!-- En App.xaml -->
<materialDesign:BundledTheme BaseTheme="Light" PrimaryColor="Blue" SecondaryColor="Orange" />
<ResourceDictionary Source="pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesign2.Defaults.xaml" />
```

Uso en controles:
```xml
<Button Style="{StaticResource MaterialDesignRaisedButton}" Content="Primary"/>
<Button Style="{StaticResource MaterialDesignOutlinedButton}" Content="Outlined"/>
<TextBox Style="{StaticResource MaterialDesignOutlinedTextBox}"/>
```

### 5. Converters

Los converters transforman datos para la presentación:

```csharp
// Bool a texto "Modo Claro" / "Modo Oscuro"
public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
{
    if (value is bool boolValue)
        return boolValue ? "Modo Oscuro" : "Modo Claro";
    return value;
}
```

### 6. Cambio Dinámico entre Sistemas de Estilos

El proyecto demuestra cómo alternar entre:
- **Temas personalizados** (definidos manualmente con ColorPaleta)
- **Material Design** (biblioteca externa)

Se usa un toggle para activar/desactivar cada modo.

## Temas Personalizados Disponibles

| Tema | Fondo | Texto | Botón | Uso |
|------|-------|-------|-------|-----|
| Claro | White | #212121 | #0078D4 | Por defecto |
| Oscuro | #1E1E1E | #FFFFFF | #3B82F6 | Modo oscuro |
| Azul | #F0F7FF | #1A365D | #2B6CB0 | Empresarial |
| Verde | #F0FFF4 | #22543D | #38A169 | Natural |
| Alto Contraste | #000000 | #FFFFFF | #FFFF00 | Accesibilidad |

## Paletas de Material Design

| Nombre | Primary | Secondary |
|--------|---------|------------|
| Blue | Blue | Orange |
| Indigo | Indigo | Pink |
| Teal | Teal | Lime |
| Green | Green | Teal |
| Purple | Purple | DeepOrange |
| Red | Red | Yellow |
| Pink | Pink | Teal |
| Cyan | Cyan | Pink |

## Estilos Demonstrados

- **StaticResource**: Referencia un estilo definido en recursos
- **DynamicResource**: Referencia un estilo que puede cambiar en tiempo real
- **Style TargetType**: Define el tipo de control al que aplica el estilo
- **BasedOn**: Hereda propiedades de otro estilo
- **Triggers**: Cambia propiedades según condiciones (hover, etc.)
- **Binding en Setters**: Los valores de los estilos vienen del ViewModel
- **Material Design**: Estilos predefinidos de la biblioteca MaterialDesignThemes

## Componentes Material Design Usados

- `materialDesign:DialogHost`
- `materialDesign:Card`
- `materialDesign:RaisedButton`
- `materialDesign:OutlinedButton`
- `materialDesign:FlatButton`
- `materialDesign:IconButton`
- `materialDesign:OutlinedTextBox`
- `materialDesign:CheckBox`
- `materialDesign:SwitchToggleButton`
- `materialDesign:Slider`
- `materialDesign:LinearProgressBar`
- `materialDesign:PackIcon`
- `materialDesign:FloatingActionMiniButton`
- `materialDesign:BundledTheme`

## Cómo Ejecutar

```bash
cd 17-TemasEstilos/TemasEstilos
dotnet run
```

## Notas para el Desarrollador

- Los estilos personalizados se definen en `Window.Resources`
- Los converters se registran en los recursos para usarlos en bindings
- El evento `SelectionChanged` del ComboBox permite cambio automático
- Los botones usan `CommandParameter` para indicar qué tema aplicar
- FallbackValue proporciona valores por defecto si el binding falla
- MaterialDesignThemes se integra en App.xaml y se cambia dinámicamente
- El ToggleButton permite cambiar entre modos personalizado y Material