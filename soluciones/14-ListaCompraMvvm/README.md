# 14-ListaCompraMvvm - Versión con FormData e IDataErrorInfo

## Descripción

Solución con **dos proyectos** que implementan la Lista de la Compra:

| Proyecto | Descripción |
|----------|-------------|
| **ListaCompra** | Proyecto original sin validación IDataErrorInfo |
| **ListaCompra.Forms** | Proyecto con FormData + IDataErrorInfo (validación en tiempo real) |

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