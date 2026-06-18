# 11. Estilos y Temas en WPF

- [11.1. Introducción a Estilos en WPF](#111-introducción-a-estilos-en-wpf)
  - [11.1.1. Estilos Inline](#1111-estilos-inline)
  - [11.1.2. Estilos con nombre (TargetType)](#1112-estilos-con-nombre-targettype)
  - [11.1.3. Estilos con nombre específico](#1113-estilos-con-nombre-específico)
- [11.2. Herencia de Estilos](#112-herencia-de-estilos)
- [11.3. Triggers: Estilos Dinámicos](#113-triggers-estilos-dinámicos)
  - [11.3.1. Property Triggers](#1131-property-triggers)
  - [11.3.2. Data Triggers](#1132-data-triggers)
  - [11.3.3. MultiTriggers](#1133-multitriggers)
- [11.4. Recursos a Nivel de Aplicación](#114-recursos-a-nivel-de-aplicación)
- [11.5. Temas en WPF](#115-temas-en-wpf)
  - [11.5.1. Temas del Sistema](#1151-temas-del-sistema)
  - [11.5.2. Cambiar Tema Dinámicamente](#1152-cambiar-tema-dinámicamente)
- [11.6. Plantillas (Templates)](#116-plantillas-templates)
  - [11.6.1. ControlTemplate](#1161-controltemplate)
  - [11.6.2. DataTemplate](#1162-datatemplate)
  - [11.6.3. HierarchicalDataTemplate](#1163-hierarchicaldatatemplate)
- [11.7. Bibliotecas de Estilos Populares](#117-bibliotecas-de-estilos-populares)
  - [11.7.1. MaterialDesignInXaml](#1171-materialdesigninxaml)
  - [11.7.2. MahApps.Metro](#1172-mahappsmetro)
- [11.8. Animaciones Simples](#118-animaciones-simples)

## 11.1. Introducción a Estilos en WPF

En WPF, los **estilos** permiten definir un conjunto de propiedades que pueden aplicarse a múltiples elementos visuales. Son similares a las hojas de estilo CSS en web, pero con toda la potencia de XAML y el sistema de propiedades de dependencia.

> 📝 **Nota del Profesor**: Los estilos en WPF son fundamentales para mantener la consistencia visual y reducir código repetitivo. Un estilo bien definido puede aplicarse a docenas de botones con una sola línea.

### 11.1.1. Estilos Inline

El forma más básica de aplicar estilos es inline:

```xml
<Button Content="Aceptar" 
        Background="Blue" 
        Foreground="White" 
        FontSize="14" 
        Padding="10,5"/>
```

### 11.1.2. Estilos con nombre (TargetType)

Definimos estilos en la sección Resources de un elemento:

```xml
<Window.Resources>
    <!-- Estilo para todos los botones de la ventana -->
    <Style TargetType="Button">
        <Setter Property="Background" Value="#2196F3"/>
        <Setter Property="Foreground" Value="White"/>
        <Setter Property="FontSize" Value="14"/>
        <Setter Property="Padding" Value="10,5"/>
        <Setter Property="Margin" Value="5"/>
    </Style>
</Window.Resources>
```

Ahora todos los botones de la ventana usarán ese estilo automáticamente.

### 11.1.3. Estilos con nombre específico

Para aplicar un estilo solo a elementos específicos:

```xml
<Window.Resources>
    <!-- Estilo base para todos los botones -->
    <Style TargetType="Button">
        <Setter Property="Padding" Value="10,5"/>
    </Style>
    
    <!-- Estilo con nombre para botones primarios -->
    <Style x:Key="BotonPrimario" TargetType="Button">
        <Setter Property="Background" Value="#4CAF50"/>
        <Setter Property="Foreground" Value="White"/>
    </Style>
    
    <!-- Estilo para botones de peligro -->
    <Style x:Key="BotonPeligro" TargetType="Button">
        <Setter Property="Background" Value="#F44336"/>
        <Setter Property="Foreground" Value="White"/>
    </Style>
</Window.Resources>

<!-- Uso -->
<Button Content="Aceptar" Style="{StaticResource BotonPrimario}"/>
<Button Content="Cancelar" Style="{StaticResource BotonPeligro}"/>
<Button Content="Normal"/> <!-- Usa el estilo base -->
```

---

## 11.2. Herencia de Estilos

WPF permite que los estilos hereden de otros mediante la propiedad `BasedOn`:

```xml
<Window.Resources>
    <!-- Estilo base -->
    <Style x:Key="EstiloBase" TargetType="Button">
        <Setter Property="Padding" Value="10,5"/>
        <Setter Property="FontSize" Value="12"/>
    </Style>
    
    <!-- Hereda de EstiloBase -->
    <Style x:Key="BotonGrande" TargetType="Button" BasedOn="{StaticResource EstiloBase}">
        <Setter Property="FontSize" Value="18"/>
        <Setter Property="Padding" Value="20,10"/>
    </Style>
    
    <!-- Hereda de BotonGrande -->
    <Style x:Key="BotonPrincipal" TargetType="Button" BasedOn="{StaticResource BotonGrande}">
        <Setter Property="Background" Value="#2196F3"/>
        <Setter Property="Foreground" Value="White"/>
    </Style>
</Window.Resources>
```

> 💡 **Tip del Examinador**: Pregunta frecuente: "¿Cómo crear un estilo que herede de otro?" La respuesta: usa `BasedOn="{StaticResource NombreEstilo}"`. Esto permite crear jerarquías de estilos.

---

## 11.3. Triggers: Estilos Dinámicos

Los **triggers** permiten cambiar el estilo automáticamente según condiciones:

### 11.3.1. Property Triggers

```xml
<Style TargetType="Button">
    <Setter Property="Background" Value="#2196F3"/>
    <Setter Property="Foreground" Value="White"/>
    
    <!-- Cuando el mouse está sobre el botón -->
    <Style.Triggers>
        <Trigger Property="IsMouseOver" Value="True">
            <Setter Property="Background" Value="#1976D2"/>
            <Setter Property="Cursor" Value="Hand"/>
        </Trigger>
        
        <!-- Cuando el botón está presionado -->
        <Trigger Property="IsPressed" Value="True">
            <Setter Property="Background" Value="#0D47A1"/>
        </Trigger>
        
        <!-- Cuando el botón está deshabilitado -->
        <Trigger Property="IsEnabled" Value="False">
            <Setter Property="Background" Value="#BDBDBD"/>
            <Setter Property="Foreground" Value="#757575"/>
        </Trigger>
    </Style.Triggers>
</Style>
```

### 11.3.2. Data Triggers

Para condiciones basadas en datos:

```xml
<Style TargetType="TextBox">
    <Setter Property="BorderBrush" Value="Gray"/>
    <Setter Property="BorderThickness" Value="1"/>
    
    <Style.Triggers>
        <DataTrigger Binding="{Binding TextoValidado}" Value="True">
            <Setter Property="BorderBrush" Value="Green"/>
        </DataTrigger>
        <DataTrigger Binding="{Binding TextoValidado}" Value="False">
            <Setter Property="BorderBrush" Value="Red"/>
        </DataTrigger>
    </Style.Triggers>
</Style>
```

### 11.3.3. MultiTriggers

Para múltiples condiciones:

```xml
<Style TargetType="Button">
    <Style.Triggers>
        <MultiTrigger>
            <MultiTrigger.Conditions>
                <Condition Property="IsMouseOver" Value="True"/>
                <Condition Property="IsPressed" Value="False"/>
            </MultiTrigger.Conditions>
            <Setter Property="Background" Value="#FF5722"/>
        </MultiTrigger>
    </Style.Triggers>
</Style>
```

---

## 11.4. Recursos a Nivel de Aplicación

Para estilos que se usen en toda la aplicación, definirlos en `App.xaml`:

```xml
<!-- App.xaml -->
<Application.Resources>
    <!-- Estilos globales -->
    <Style TargetType="Button">
        <Setter Property="FontFamily" Value="Segoe UI"/>
    </Style>
    
    <Style TargetType="TextBlock">
        <Setter Property="FontFamily" Value="Segoe UI"/>
    </Style>
    
    <!-- Colores globales -->
    <SolidColorBrush x:Key="ColorPrimario" Color="#2196F3"/>
    <SolidColorBrush x:Key="ColorSecundario" Color="#FFC107"/>
    <SolidColorBrush x:Key="ColorPeligro" Color="#F44336"/>
    
    <!-- Estilos con nombre -->
    <Style x:Key="BotonGlobal" TargetType="Button">
        <Setter Property="Background" Value="{StaticResource ColorPrimario}"/>
        <Setter Property="Foreground" Value="White"/>
        <Setter Property="Padding" Value="15,8"/>
    </Style>
</Application.Resources>
```

> 📝 **Nota del Profesor**: Define los estilos globales en App.xaml para mantener consistencia en toda la aplicación. Usa StaticResource para recursos que no cambian y DynamicResource para temas que pueden modificarse en tiempo de ejecución.

---

## 11.5. Temas en WPF

Los **temas** permiten cambiar la apariencia visual completa de la aplicación.

### 11.5.1. Temas del Sistema

WPF puede detectar y usar los temas del sistema Windows:

```xml
<Window.Resources>
    <!-- Usar colores del tema del sistema -->
    <SolidColorBrush x:Key="FondoVentana" 
                     Color="{DynamicResource {x:Static SystemColors.WindowColorKey}}"/>
    <SolidColorBrush x:Key="ColorTexto" 
                     Color="{DynamicResource {x:Static SystemColors.WindowTextColorKey}}"/>
</Window.Resources>
```

### 11.5.2. Cambiar Tema Dinámicamente

```xml
<Application.Resources>
    <!-- Tema Claro (por defecto) -->
    <ResourceDictionary x:Key="TemaClaro">
        <SolidColorBrush x:Key="Fondo" Color="White"/>
        <SolidColorBrush x:Key="Texto" Color="Black"/>
        <SolidColorBrush x:Key="BotonFondo" Color="#2196F3"/>
    </ResourceDictionary>
    
    <!-- Tema Oscuro -->
    <ResourceDictionary x:Key="TemaOscuro">
        <SolidColorBrush x:Key="Fondo" Color="#1E1E1E"/>
        <SolidColorBrush x:Key="Texto" Color="White"/>
        <SolidColorBrush x:Key="BotonFondo" Color="#BB86FC"/>
    </ResourceDictionary>
</Application.Resources>
```

**Cambiar tema desde código:**

```csharp
public void CambiarTema(string tema)
{
    var recursos = Application.Current.Resources;
    
    // Eliminar diccionario actual si existe
    ResourceDictionary? diccionarioActual = null;
    foreach (var dict in recursos.MergedDictionaries)
    {
        if (dict.Source?.ToString().Contains("Tema") == true)
        {
            diccionarioActual = dict;
            break;
        }
    }
    
    if (diccionarioActual != null)
        recursos.MergedDictionaries.Remove(diccionarioActual);
    
    // Añadir nuevo tema
    var nuevoTema = tema == "Oscuro" 
        ? new ResourceDictionary { Source = new Uri("pack://application:,,,/Temas/TemaOscuro.xaml") }
        : new ResourceDictionary { Source = new Uri("pack://application:,,,/Temas/TemaClaro.xaml") };
    
    recursos.MergedDictionaries.Add(nuevoTema);
}
```

---

## 11.6. Plantillas (Templates)

Las **plantillas** definen la estructura visual completa de un control.

### 11.6.1. ControlTemplate

```xml
<Style TargetType="Button">
    <Setter Property="Template">
        <Setter.Value>
            <ControlTemplate TargetType="Button">
                <Border x:Name="border"
                        Background="{TemplateBinding Background}"
                        BorderBrush="{TemplateBinding BorderBrush}"
                        BorderThickness="{TemplateBinding BorderThickness}"
                        CornerRadius="5"
                        Padding="{TemplateBinding Padding}">
                    <ContentPresenter HorizontalAlignment="Center" 
                                      VerticalAlignment="Center"/>
                </Border>
                <ControlTemplate.Triggers>
                    <Trigger Property="IsMouseOver" Value="True">
                        <Setter TargetName="border" Property="Background" Value="#1976D2"/>
                    </Trigger>
                    <Trigger Property="IsPressed" Value="True">
                        <Setter TargetName="border" Property="Background" Value="#0D47A1"/>
                    </Trigger>
                </ControlTemplate.Triggers>
            </ControlTemplate>
        </Setter.Value>
    </Setter>
</Style>
```

### 11.6.2. DataTemplate

Para mostrar datos en controles:

```xml
<!-- DataTemplate para mostrar productos en un ListBox -->
<ListBox ItemsSource="{Binding Productos}">
    <ListBox.ItemTemplate>
        <DataTemplate>
            <StackPanel Orientation="Horizontal" Margin="5">
                <TextBlock Text="{Binding Nombre}" FontWeight="Bold" Width="150"/>
                <TextBlock Text="{Binding Precio, StringFormat={}{0:C}}" Width="80"/>
                <TextBlock Text="{Binding Stock}" Foreground="Gray"/>
            </StackPanel>
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>
```

### 11.6.3. HierarchicalDataTemplate

Para árboles y menús:

```xml
<HierarchicalDataTemplate DataType="{x:Type local:Categoria}" 
                          ItemsSource="{Binding Subcategorias}">
    <StackPanel Orientation="Horizontal">
        <TextBlock Text="{Binding Nombre}" FontWeight="Bold"/>
    </StackPanel>
</HierarchicalDataTemplate>
```

---

## 11.7. Bibliotecas de Estilos Populares

### 11.7.1. MaterialDesignInXaml

**Instalación:**
```bash
dotnet add package MaterialDesignThemes
```

**Configuración en App.xaml:**
```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Light.xaml"/>
            <ResourceDictionary Source="pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Defaults.xaml"/>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

**Uso de componentes:**
```xml
<Button Content="Aceptar" Style="{StaticResource MaterialDesignRaisedButton}"/>
<TextBox Style="{StaticResource MaterialDesignTextBox}"/>
<CheckBox Style="{StaticResource MaterialDesignCheckBox}"/>
```

### 11.7.2. MahApps.Metro

**Instalación:**
```bash
dotnet add package MahApps.Metro
```

**Uso (MetroWindow):**
```xml
<mah:MetroWindow xmlns:mah="http://metro.mahapps.com/winfx/xaml/controls"
                 Title="Mi App" Height="450" Width="600">
    <Button Content="Hola" Style="{StaticResource MahApps.Styles.Button}"/>
</mah:MetroWindow>
```

---

### Ejemplo práctico: Cambio de Temas en Tiempo de Ejecución

**ViewModel con cambio de temas:**
```csharp
public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string _temaSeleccionado = "Claro";
    
    private readonly Dictionary<string, ResourceDictionary> _temas = new();
    
    public void CambiarTema(string nombreTema)
    {
        var app = Application.Current;
        // Quitar tema actual
        var temaActual = app.Resources.MergedDictionaries
            .FirstOrDefault(d => d.Source?.ToString().Contains("Temas") == true);
        if (temaActual != null)
            app.Resources.MergedDictionaries.Remove(temaActual);
        
        // Aplicar nuevo tema
        app.Resources.MergedDictionaries.Add(_temas[nombreTema]);
        TemaSeleccionado = nombreTema;
    }
}
```

**XAML con ComboBox para cambiar tema:**
```xml
<ComboBox SelectedItem="{Binding TemaSeleccionado}" 
         Style="{StaticResource MaterialDesignComboBox}">
    <ComboBoxItem Content="Claro"/>
    <ComboBoxItem Content="Oscuro"/>
    <ComboBoxItem Content="Azul"/>
</ComboBox>
```

**Recursos de tema (Claro.xaml):**
```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <SolidColorBrush x:Key="Fondo" Color="White"/>
    <SolidColorBrush x:Key="Texto" Color="Black"/>
    <SolidColorBrush x:Key="Boton" Color="#2196F3"/>
</ResourceDictionary>
```

**Recursos de tema (Oscuro.xaml):**
```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <SolidColorBrush x:Key="Fondo" Color="#1E1E1E"/>
    <SolidColorBrush x:Key="Texto" Color="White"/>
    <SolidColorBrush x:Key="Boton" Color="#BB86FC"/>
</ResourceDictionary>
```

**Enlazar estilos a recursos dinámicos:**
```xml
<Window Background="{DynamicResource Fondo}">
    <Button Background="{DynamicResource Boton}" 
            Foreground="{DynamicResourceTexto}"/>
</Window>
```

---

### Comparativa: ¿Cuál biblioteca usar?

| Biblioteca | Estilo | Complejidad | Uso típico |
|------------|--------|-------------|-------------|
| **MaterialDesign** | Material Design (Google) | Media | Apps modernas, empresarial |
| **MahApps** | Windows 10/11 | Baja | Apps estilo Windows |
| **WPF UI** | Windows 11 Fluent | Media | Apps estilo Windows 11 |
| **Sin biblioteca** | WPF nativo | Muy baja | Apps simples, legacy |

### 11.7.3. WPF UI (Fluent Windows 11)

**WPF UI** es una biblioteca moderna que trae el estilo **Fluent Design de Windows 11** a WPF.

**Características:**
- Estilo Windows 11 Fluent
- Controles modernos: NavigationView, NumberBox, Dialog, Snackbar
- Mica y Acrylic efectos (fondo translúcido)
- Iconos Fluent de Microsoft

**Instalación:**
```bash
dotnet add package WPF-UI
```

**Configuración en App.xaml:**
```xml
<Application xmlns:ui="http://schemas.lepo.co/wpfui/2022/xaml">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ui:ThemesDictionary Theme="Dark"/>
                <ui:ControlsDictionary/>
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

** Uso con FluentWindow:**
```xml
<ui:FluentWindow xmlns:ui="http://schemas.lepo.co/wpfui/2022/xaml"
                 Title="Mi App Windows 11"
                 Width="800" Height="600">
    
    <ui:FluentWindow.Header>
        <ui:TitleBar Title="Mi App"/>
    </ui:FluentWindow.Header>
    
    <Grid>
        <ui:NavigationView>
            <ui:NavigationView.MenuItems>
                <ui:NavigationViewItem Content="Inicio" Icon="{ui:SymbolIcon Home24}"/>
                <ui:NavigationViewItem Content="Ajustes" Icon="{ui:SymbolIcon Settings24}"/>
            </ui:NavigationView.MenuItems>
        </ui:NavigationView>
    </Grid>
</ui:FluentWindow>
```

**Cambiar tema oscuro/claro:**
```csharp
// En el ViewModel o code-behind
Wpf.Ui.Appearance.ApplicationThemeManager.Apply(
    Wpf.Ui.Appearance.ApplicationTheme.Dark);
```

> 💡 **Tip del Examinador**: WPF UI es la más moderna y recomendada para apps que quieren aspecto Windows 11. Se lleva muy bien con CommunityToolkit.Mvvm.

> 📝 **Nota del Profesor**: Para el examen, basta con saber instalar y usar una biblioteca. Para proyectos nuevos con WPF 10, **WPF UI** o **MaterialDesign** son las más recomendadas.

---

## 11.8. Animaciones Simples

WPF soporta animaciones declarativas:

```xml
<Button Content="Hover Me" Width="100" Height="40">
    <Button.Triggers>
        <EventTrigger RoutedEvent="MouseEnter">
            <BeginStoryboard>
                <Storyboard>
                    <DoubleAnimation Storyboard.TargetProperty="Width"
                                     To="150" Duration="0:0:0.3"/>
                </Storyboard>
            </BeginStoryboard>
        </EventTrigger>
        <EventTrigger RoutedEvent="MouseLeave">
            <BeginStoryboard>
                <Storyboard>
                    <DoubleAnimation Storyboard.TargetProperty="Width"
                                     To="100" Duration="0:0:0.3"/>
                </Storyboard>
            </BeginStoryboard>
        </EventTrigger>
    </Button.Triggers>
</Button>
```

---

## Resumen

| Concepto | Descripción |
|----------|-------------|
| **Style** | Conjunto de propiedades aplicables a múltiples elementos |
| **TargetType** | Tipo de control al que se aplica el estilo |
| **x:Key** | Nombre para referenciar el estilo |
| **BasedOn** | Herencia entre estilos |
| **Triggers** | Cambios dinámicos según condiciones |
| **Resources** | Diccionario de recursos a nivel de ventana/aplicación |
| **ControlTemplate** | Plantilla que define la estructura visual |
| **DataTemplate** | Plantilla para mostrar datos |
| **Temas** | Colección de estilos para cambio de apariencia global |

### Puntos clave

1. **Estilos inline** vs **estilos con nombre**: usa nombre para reutilización
2. **Herencia con BasedOn**: crea jerarquías de estilos
3. **Triggers**: IsMouseOver, IsPressed, IsEnabled, DataTrigger
4. **Recursos globales**: definir en App.xaml
5. **Temas**: cambiar apariencia completa dinámicamente
6. **Plantillas**: ControlTemplate y DataTemplate
7. **Bibliotecas**: MaterialDesign, MahApps.Metro

> 📝 **Nota del Profesor**: Los estilos y temas son lo que hacen que una aplicación WPF parezca profesional. Invierte tiempo en crear un sistema de estilos coherente desde el inicio. MahApps.Metro y MaterialDesign te ahorran mucho trabajo.

> 💡 **Tip del Examinador**: Pregunta frecuente: "¿Cuál es la diferencia entre StaticResource y DynamicResource?" - Respuesta: StaticResource se resuelve en tiempo de carga (más rápido). DynamicResource se resuelve en tiempo de ejecución (necesario para temas que cambian). Otra pregunta común: "¿Cómo hacer que un botón cambie al pasar el mouse?" → Usar Trigger con IsMouseOver.

---

## Referencias

- [Estilos en WPF](https://learn.microsoft.com/dotnet/desktop/wpf/controls/styling-and-templating)
- [Triggers](https://learn.microsoft.com/dotnet/desktop/wpf/styles/triggers)
- [Plantillas](https://learn.microsoft.com/dotnet/desktop/wpf/data/data-templating-overview)
- [MaterialDesignInXaml](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit)
- [MahApps.Metro](https://mahapps.com/)
