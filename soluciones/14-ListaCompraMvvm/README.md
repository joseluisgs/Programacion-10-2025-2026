# 14-ListaCompraMvvm - Versión con FormData e IDataErrorInfo

## Descripción

Solución con **dos proyectos** que implementan la Lista de la Compra:

| Proyecto | Descripción |
|----------|-------------|
| **ListaCompra** | Proyecto original con menú y aceleradores de teclado |
| **ListaCompra.Forms** | Proyecto con FormData + IDataErrorInfo (validación en tiempo real) |

## Proyecto 14-ListaCompra (Original)

### Menú con Aceleradores de Teclado

El proyecto incluye un menú con **aceleradores de teclado funcionales**:

```xml
<!-- XAML: InputGestureText muestra el atajo -->
<MenuItem Header="_Importar" Command="{Binding ImportarCommand}" InputGestureText="Ctrl+I"/>
<MenuItem Header="_Exportar" Command="{Binding ExportarCommand}" InputGestureText="Ctrl+E"/>

<!-- KeyBinding hace que funcione -->
<Window.InputBindings>
    <KeyBinding Key="I" Modifiers="Control" Command="{Binding ImportarCommand}"/>
    <KeyBinding Key="E" Modifiers="Control" Command="{Binding ExportarCommand}"/>
</Window.InputBindings>
```

| Atajo | Acción |
|-------|--------|
| **Ctrl+I** | Importar productos (JSON/CSV) |
| **Ctrl+E** | Exportar productos (JSON/CSV) |
| **Alt+F4** | Salir de la aplicación |

> 💡 Ver teoría en [04-introduccion.md: 4.4.6. Menu y MenuItem](../../04-introduccion.md#446-menu-y-menuitem)

## Estructura de la Solución

```
14-ListaCompraMvvm/
├── ListaCompra.slnx              # Solución original (1 proyecto)
├── ListaCompra.Forms.slnx         # Nueva solución con 2 proyectos
├── ListaCompra/                  # Proyecto original
│   ├── ViewModels/MainViewModel.cs
│   └── Views/Main/MainWindow.xaml
└── ListaCompra.Forms/            # NUEVO proyecto con FormData
    ├── FormData/
    │   └── ProductoFormData.cs   # FormData con IDataErrorInfo
    ├── ViewModels/
    │   └── MainViewModel.cs      # ViewModel con FormData
    └── Views/Main/
        └── MainWindow.xaml       # XAML con ValidatesOnDataErrors
```

## Nuevo Proyecto: ListaCompra.Forms

Este proyecto demuestra el **patrón FormData con IDataErrorInfo**:

### Conceptos Aprendidos

| Concepto | Descripción |
|----------|-------------|
| **FormData** | DTO que encapsula datos del formulario + validación |
| **IDataErrorInfo** | Interfaz para validación en tiempo real en WPF |
| **ValidatesOnDataErrors** | Binding que activa la validación en la UI |
| **UpdateSourceTrigger=PropertyChanged** | Validación mientras el usuario escribe |
| **CanExecute = FormData.IsValid()** | Botón habilitado solo si el formulario es válido |

### Archivos Clave

1. **`FormData/ProductoFormData.cs`**
   - Implementa `IDataErrorInfo`
   - Valida: Nombre (obligatorio, 2-100 caracteres), Cantidad (1-1000), Precio (0-99999.99)
   - Métodos `GetCantidad()` y `GetPrecio()` para conversión

2. **`ViewModels/MainViewModel.cs`**
   - Nueva propiedad `FormData` de tipo `ProductoFormData`
   - Comando `Añadir` usa `CanExecute = FormData.IsValid()`
   - Validación en tiempo real

3. **`MainWindow.xaml`**
   - Campos con `ValidatesOnDataErrors=True`
   - `UpdateSourceTrigger=PropertyChanged` para validación continua
   - Botón "Añadir" se habilita/deshabilita automáticamente

### Diferencia entre Proyectos

| Aspecto | ListaCompra (Original) | ListaCompra.Forms (Nuevo) |
|---------|----------------------|--------------------------|
| Validación | En el comando Añadir | En tiempo real (IDataErrorInfo) |
| Feedback | Solo al pulsar botón | Mientras escribe (borde rojo) |
| Botón | Siempre habilitado | Se habilita si `IsValid()` |
| Mensajes | Genéricos | Específicos por campo |

### Reactividad en Modelos (ObservableProperty)

En esta solución, el modelo **`Producto`** ha sido transformado de un `record` inmutable a una clase que hereda de **`ObservableObject`** de Community Toolkit.

#### ¿Por qué usar [ObservableProperty] en el Modelo?

En una `ObservableCollection<T>`, la colección notifica a la UI cuando se añaden o eliminan elementos, pero **no cuando cambian las propiedades internas** de los objetos que ya están en la lista (como `EstaComprado`).

Si el modelo es un `record` (inmutable):
- No se puede cambiar `EstaComprado` directamente.
- Para actualizar la UI (tachado), habría que reemplazar el objeto entero en la colección.
- Esto es ineficiente y puede causar parpadeos o pérdida de foco en la UI.

#### Ventajas de usar ObservableObject + [ObservableProperty]:

1.  **Actualización Automática e Instantánea**: Cuando el usuario marca el `CheckBox`, la propiedad `EstaComprado` cambia y notifica inmediatamente al `TextBlock` del nombre para aplicar el tachado (vía Converter).
2.  **Rendimiento Óptimo**: Solo se redibuja el elemento específico que ha cambiado, no toda la lista.
3.  **Código Limpio (Source Generators)**: Al usar `[ObservableProperty]`, el Community Toolkit genera automáticamente el código de `INotifyPropertyChanged` por nosotros.
4.  **Propiedades Calculadas**: Con **`[NotifyPropertyChangedFor(nameof(Total))]`**, si cambia la cantidad o el precio, el `Total` de ese producto se actualiza automáticamente en la UI sin código extra.

```csharp
// Ejemplo en Producto.cs
public partial class Producto : ObservableObject
{
    [ObservableProperty]
    private bool _estaComprado; // Genera la propiedad EstaComprado con notificación
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Total))]
    private int _cantidad; // Al cambiar, también notifica que 'Total' ha cambiado
    
    public decimal Total => Cantidad * Precio;
}
```

## Tecnologías

- .NET 10 (WPF)
- C# 14
- CommunityToolkit.Mvvm
- Material Design
- IDataErrorInfo

## Cómo Ejecutar

```bash
# Proyecto original
cd 14-ListaCompraMvvm/ListaCompra
dotnet run

# Proyecto con FormData
cd 14-ListaCompraMvvm/ListaCompra.Forms
dotnet run
```

## Comparación de Validación

### Original (sin IDataErrorInfo):
```csharp
[RelayCommand]
private void Añadir()
{
    // Validación solo al pulsar el botón
    if (!int.TryParse(Cantidad, out var cantidad) || cantidad <= 0)
    {
        MostrarError("La cantidad debe ser mayor que 0");
        return;
    }
    // ...
}
```

### Nuevo (con IDataErrorInfo):
```csharp
// Validación en tiempo real mientras escribe
TextBox Text="{Binding FormData.Cantidad, UpdateSourceTrigger=PropertyChanged, ValidatesOnDataErrors=True}"

// Botón habilitado solo si el formulario es válido
[RelayCommand(CanExecute = nameof(CanAñadir))]
private bool CanAñadir() => FormData.IsValid();
```