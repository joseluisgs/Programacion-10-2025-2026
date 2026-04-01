# WPF Gestión de Productos — CRUD con Repository Pattern

## Descripción
Aplicación de gestión de un catálogo de productos con **WPF** que implementa:
- CRUD completo (Crear, Leer, Actualizar, Eliminar)
- Repositorio con SQLite
- Validador con Railway Oriented Programming
- Inyección de dependencias
- Patrón Repository
- **Material Design** con tema dinámico claro/oscuro

Este ejercicio aplica los conceptos aprendidos en **UD09** (Services, Repositories, Validators, DI) en una interfaz gráfica WPF con Material Design.

## Objetivos de Aprendizaje
- Aplicar el patrón Repository en una aplicación WPF
- Usar **Railway Oriented Programming** con `Result<T, DomainError>`
- Implementar inyección de dependencias con `Microsoft.Extensions.DependencyInjection`
- Validar entidades con `IValidador<T>`
- Crear diálogos modales para añadir/editar datos
- Usar `DataGrid` para mostrar y gestionar datos
- Aplicar **Material Design** con **tema dinámico** (claro/oscuro)
- Usar controles modernos con hint flotante, iconos y animaciones

## Requisitos Funcionales
- Listado de productos en un `DataGrid` con columnas: ID, nombre, descripción, categoría, precio, stock, total, activo, fecha
- Diálogo modal de "Nuevo producto" y "Editar producto" con formulario completo
- Validación en el formulario: nombre obligatorio, categoría obligatoria, precio no negativo, stock no negativo
- Botones de CRUD con confirmación mediante `MessageBox`
- Filtrado de productos por nombre y/o categoría
- Panel de estadísticas: total de productos, productos activos, stock total, categorías
- Menú con opciones de Archivo, Productos y Ayuda
- **Cambio de tema dinámico** (claro/oscuro) con toggle

## Arquitectura
**MVC con Services, Repositories y Validators**

```
┌─────────┐   usa      ┌──────────────────┐   usa    ┌─────────────┐
│  Views  │ ─────────► │     Services      │ ───────► │ Validators  │
│ (XAML)  │            │ (lógica negocio)  │           │ (validación)│
└─────────┘            └────────┬─────────┘           └─────────────┘
                                   │ usa
                          ┌────────▼─────────┐
                          │   Repositories   │
                          │ (acceso a datos) │
                          └────────┬─────────┘
                                   │ usa
                          ┌────────▼─────────┐
                          │    SQLite DB     │
                          │ (persistencia)   │
                          └──────────────────┘
```

## Tecnologías
- WPF (.NET 10)
- C# 14
- JetBrains Rider
- Microsoft.Extensions.DependencyInjection
- Microsoft.Data.Sqlite
- CSharpFunctionalExtensions (para Result<T, Error>)
- **MaterialDesignThemes** (v5.3.0)

## Estructura del Proyecto
```
GestionProductos/
├── GestionProductos.csproj
├── App.xaml / App.xaml.cs          <- Punto de entrada, DI, Tema Material
├── appsettings.json                 <- Configuración
├── Config/
│   └── AppConfig.cs                 <- Configuración de la app
├── Models/
│   └── Producto.cs                  <- Entidad Producto
├── Repositories/
│   ├── ICrudRepository.cs           <- Interfaz genérica CRUD
│   ├── IProductoRepository.cs       <- Interfaz específica
│   └── ProductoRepository.cs        <- Implementación SQLite
├── Validators/
│   ├── IValidador.cs               <- Interfaz genérica validador
│   └── ValidadorProducto.cs        <- Validador de productos
├── Services/
│   ├── IProductoService.cs          <- Interfaz del servicio
│   └── ProductoService.cs          <- Lógica de negocio
├── Errors/
│   └── DomainError.cs              <- Errores de dominio
├── Infrastructure/
│   ├── DependenciesProvider.cs     <- Registro de dependencias
│   └── ThemeHelper.cs              <- Cambio de tema dinámico
└── Views/
    ├── Main/
    │   ├── MainWindow.xaml         <- Ventana principal
    │   └── MainWindow.xaml.cs      <- Code-behind
    └── Dialog/
        ├── ProductoDialog.xaml      <- Diálogo añadir/editar
        └── ProductoDialog.xaml.cs   <- Code-behind
```

## Conceptos Clave

### 1. Inyección de Dependencias (DI)
```csharp
// En App.xaml.cs
ServiceProvider = DependenciesProvider.BuildServiceProvider();

// En MainWindow.xaml.cs
_servicio = App.ServiceProvider.GetRequiredService<IProductoService>();
```

### 2. Railway Oriented Programming
```csharp
// El servicio devuelve Result<T, DomainError>
var resultado = _servicio.Create(nombre, descripcion, categoria, precio, stock);

if (resultado.IsSuccess)
{
    // Operación exitosa
    CargarProductos();
}
else
{
    // Error
    MostrarError(resultado.Error.Message);
}
```

### 3. Validación con IValidador<T>
```csharp
public Result<Producto, DomainError> Validar(Producto producto)
{
    var errores = new List<string>();
    
    if (string.IsNullOrWhiteSpace(producto.Nombre))
        errores.Add("El nombre es obligatorio");
    
    if (errores.Count > 0)
        return Result.Failure<Producto, DomainError>(
            new ValidationError(string.Join("; ", errores)));
    
    return Result.Success<Producto, DomainError>(producto);
}
```

### 4. Repository Pattern
```csharp
public interface IProductoRepository
{
    IEnumerable<Producto> GetAll();
    Producto? GetById(int id);
    Producto? Create(Producto producto);
    Producto? Update(int id, Producto producto);
    Producto? Delete(int id);
    IEnumerable<Producto> Search(string criterio);
    IEnumerable<Producto> GetByCategoria(string categoria);
    IEnumerable<Producto> GetActivos();
}
```

## Material Design In XAML

### ¿Qué es Material Design?

**Material Design** es un lenguaje de diseño desarrollado por Google que proporciona plantillas y componentes modernos con animaciones y transiciones. **MaterialDesignInXamlToolkit** es la librería más popular para usar Material Design en aplicaciones WPF.

### Paquete NuGet
```xml
<PackageReference Include="MaterialDesignThemes" Version="5.3.0" />
```

### Configuración en App.xaml
```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <!-- BundledTheme: combina tema base + colores -->
            <materialDesign:BundledTheme BaseTheme="Light" 
                                          PrimaryColor="DeepPurple" 
                                          SecondaryColor="Lime" />
            <!-- Recursos por defecto de Material Design 3 -->
            <ResourceDictionary Source="pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesign3.Defaults.xaml" />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

### Controles Material Disponibles

| Control | Descripción | Ejemplo |
|---------|-------------|---------|
| **ColorZone** | Barra de color en header | `Mode="PrimaryMid"` |
| **Card** | Tarjeta con sombra | `UniformCornerRadius="8"` |
| **PackIcon** | Iconos Material | `Kind="Plus"` |
| **RaisedButton** | Botón con relieve | `Style="{StaticResource MaterialDesignRaisedButton}"` |
| **OutlinedTextBox** | TextBox con borde | `Style="{StaticResource MaterialDesignOutlinedTextBox}"` |
| **HintAssist** | Hint flotante | `materialDesign:HintAssist.Hint="Texto..."` |
| **DataGrid** | Tabla con estilo Material | `Style="{StaticResource MaterialDesignDataGrid}"` |

### Hint Flotante (TextBox)
```xml
<TextBox Style="{StaticResource MaterialDesignOutlinedTextBox}"
         materialDesign:HintAssist.Hint="Escribe aquí..."
         materialDesign:HintAssist.IsFloating="True"/>
```

### Iconos Material Design
```xml
<materialDesign:PackIcon Kind="Home" Width="24" Height="24"/>
<materialDesign:PackIcon Kind="Plus"/>
<materialDesign:PackIcon Kind="Delete"/>
<materialDesign:PackIcon Kind="WeatherSunny"/>
```

## Cambio de Tema Dinámico (Light/Dark)

### ThemeHelper.cs
```csharp
using MaterialDesignThemes.Wpf;

public static class ThemeHelper
{
    /// <summary>
    /// Indica si el tema actual es oscuro.
    /// </summary>
    public static bool IsDarkTheme { get; private set; }
    
    /// <summary>
    /// Cambia el tema de la aplicación.
    /// </summary>
    /// <param name="isDark">True para tema oscuro, false para tema claro</param>
    public static void SetTheme(bool isDark)
    {
        // Obtener el tema actual
        var paletteHelper = new PaletteHelper();
        var theme = paletteHelper.GetTheme();
        
        // Establecer el tema base (Dark o Light)
        theme.SetBaseTheme(isDark ? BaseTheme.Dark : BaseTheme.Light);
        
        // Aplicar los cambios
        paletteHelper.SetTheme(theme);
        
        // Guardar el estado actual
        IsDarkTheme = isDark;
    }
    
    /// <summary>
    /// Invierte el tema actual.
    /// </summary>
    public static void ToggleTheme()
    {
        SetTheme(!IsDarkTheme);
    }
}
```

### Uso en el toggle button
```csharp
private void ThemeToggle_Changed(object sender, RoutedEventArgs e)
{
    // Cambiar el tema
    ThemeHelper.ToggleTheme();
    
    // Actualizar el icono según el tema
    if (ThemeHelper.IsDarkTheme)
    {
        ThemeIcon.Kind = PackIconKind.WeatherNight;
    }
    else
    {
        ThemeIcon.Kind = PackIconKind.WeatherSunny;
    }
}
```

### Colores Disponibles

**Primary Colors** (colores principales):
- DeepPurple, Purple, Indigo, Blue, Cyan, Teal, Green, Lime, Yellow, Amber, Orange, Red, Pink

**Secondary Colors** (colores de acento):
- Lime, Green, Teal, Cyan, Blue, Indigo, Purple, Pink, Red, Amber, Orange, DeepOrange

## Flujo de Datos

### Create (Crear)
1. Usuario pulsa "Añadir" → Se abre ProductoDialog
2. Usuario填写 formulario y pulsa "Aceptar"
3. MainWindow llama a `_servicio.Create(...)`
4. ProductoService crea el Producto y valida con ValidadorProducto
5. Si validación falla → devuelve Result.Failure
6. Si validación pasa → llama a ProductoRepository.Create()
7. ProductoRepository inserta en SQLite y devuelve el Producto creado
8. MainWindow recarga el DataGrid

### Update (Actualizar)
1. Usuario selecciona producto → Pulsa "Editar"
2. Se abre ProductoDialog con los datos del producto
3. Usuario modifica y acepta → Confirmación con MessageBox
4. MainWindow llama a `_servicio.Update(...)`
5. ProductoService valida y llama al repositorio
6. SQLite actualiza el registro

### Delete (Eliminar)
1. Usuario selecciona producto → Pulsa "Eliminar"
2. Confirmación con MessageBox
3. MainWindow llama a `_servicio.Delete(...)`
4. ProductoRepository hace **borrado lógico** (Activo = false)

## Configuración (appsettings.json)
```json
{
  "Database": {
    "Type": "Sqlite",
    "ConnectionString": "Data Source=data/productos.db"
  },
  "Repository": {
    "DropData": false,    // Si true, borra datos al iniciar
    "SeedData": true      // Si true, carga datos de ejemplo
  },
  "Logging": {
    "Level": "Information"
  }
}
```

## Ejecutar la Aplicación
```bash
cd GestionProductos
dotnet run
```

## Notas
- El repositorio usa **borrado lógico** (soft delete): `Activo = false`
- El seed data se carga solo si `SeedData = true` Y no hay datos en la BD
- El DropData borra todos los registros antes de cargar el seed (útil para pruebas)
- Los diálogos están en la carpeta `Views/Dialog/` para separarlos de las ventanas principales
- **Material Design** se configura en `App.xaml` con `BundledTheme`
- El **cambio de tema** se realiza con `ThemeHelper.SetTheme()`

---

## Estilos y Temas Personalizados (alternativo)

### ¿Qué son los Estilos?

Los **estilos** en WPF son recursos que permiten definir un conjunto de propiedades visuales para los controles. 

### StaticResource vs DynamicResource

| Recurso | Descripción | Cuándo usar |
|---------|-------------|-------------|
| **StaticResource** | Se resuelve una sola vez al cargar | Valores que no cambian |
| **DynamicResource** | Se resuelve cada vez que cambia | Temas dinámicos |

### Estilos Definidos en App.xaml (versión anterior)

Este proyecto originalmente tenía estilos personalizados en App.xaml:

```xml
<!-- Colores -->
<Color x:Key="ColorPrimario">#1976D2</Color>
<SolidColorBrush x:Key="PincelPrimario" Color="{StaticResource ColorPrimario}"/>

<!-- Botones -->
<Style x:Key="BotonPrimario" TargetType="Button">
    <Setter Property="Background" Value="{StaticResource PincelPrimario}"/>
    <Setter Property="Foreground" Value="White"/>
</Style>
```

### Beneficios de Usar Estilos

1. **Mantenibilidad**: Un cambio afecta a todos los controles
2. **Consistencia**: Toda la app tiene el mismo aspecto
3. **Reutilización**: No repetir propiedades
4. **Separación**: Presentación separada de la lógica

### Buenas Prácticas

1. Definir estilos en `App.xaml` para disponibilidad global
2. Usar `BasedOn` para heredar estilos
3. Nombrar estilos de forma descriptiva
4. Usar colores consistentes
5. Para proyectos profesionales, usar **Material Design**

---

## Recursos

- [Material Design In XAML Toolkit](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit)
- [Documentación de Material Design](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit/wiki)
- [Ejemplos de temas](https://github.com/Keboo/MaterialDesignInXaml.Examples)
