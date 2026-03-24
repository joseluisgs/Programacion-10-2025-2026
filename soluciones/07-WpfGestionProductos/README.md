# WPF Gestión de Productos — MVVM Completo

## Descripción
Aplicación de gestión de un catálogo de productos que aplica el patrón MVVM de forma exhaustiva con **CommunityToolkit.Mvvm**. Incluye CRUD completo, navegación entre ventanas, diálogos de edición modales, validación de datos y organización del proyecto en capas bien definidas. Es el proyecto de mayor complejidad de la serie y sirve como referencia de buenas prácticas en aplicaciones WPF profesionales.

## Objetivos de Aprendizaje
- Estructurar un proyecto WPF en capas (Models, ViewModels, Views, Services) de forma coherente
- Implementar CRUD completo con `DataGrid` editable y ventana de detalle modal
- Navegar entre ventanas usando un servicio de navegación o diálogo inyectado en el ViewModel
- Aplicar validación de datos con `INotifyDataErrorInfo` o atributos de validación
- Usar `[RelayCommand]` con lógica de `CanExecute` para habilitar/deshabilitar acciones contextuales
- Simular una capa de repositorio/servicio para abstraer el acceso a los datos
- Aplicar estilos y plantillas XAML reutilizables (`Style`, `DataTemplate`, `ControlTemplate`)

## Requisitos Funcionales
- Listado de productos en un `DataGrid` con columnas: nombre, categoría, precio, stock y estado (activo/inactivo)
- Ventana/diálogo modal de "Nuevo producto" y "Editar producto" con formulario completo
- Validación en el formulario: nombre obligatorio, precio mayor que cero, stock no negativo
- Botón "Eliminar" con confirmación mediante `MessageBox` o diálogo personalizado
- Filtrado de productos por nombre y/o categoría desde la ventana principal
- Ordenación de columnas del `DataGrid` haciendo clic en la cabecera
- Panel de resumen: total de productos, productos activos, valor total del inventario
- Exportación básica de la lista a un fichero de texto o CSV (opcional, nivel avanzado)

## Arquitectura
**MVVM completo con CommunityToolkit.Mvvm**

```
┌─────────┐   binding    ┌──────────────────┐   usa    ┌─────────┐
│  Views  │ ──────────► │   ViewModels     │ ────────► │ Models  │
│  (XAML) │             │ (lógica UI)      │           │ (datos) │
└─────────┘             └────────┬─────────┘           └─────────┘
                                  │ usa
                         ┌────────▼─────────┐
                         │    Services      │
                         │ (repositorio,    │
                         │  diálogos, nav.) │
                         └──────────────────┘
```

- **Models**: `Producto`, `Categoria` — clases de datos puras.
- **ViewModels**: `ProductosViewModel`, `ProductoDetalleViewModel` — heredan de `ObservableObject`.
- **Views**: `MainWindow`, `ProductoDetalleWindow` — sin lógica en code-behind.
- **Services**: `IProductoRepository` / `ProductoRepositorio` (en memoria), `IDialogService`.

## Tecnologías
- WPF (.NET 10)
- C# 14
- JetBrains Rider
- CommunityToolkit.Mvvm (NuGet)

## Estructura Sugerida
```
07-WpfGestionProductos/
└── WpfGestionProductos/
    ├── WpfGestionProductos.csproj
    ├── App.xaml
    ├── App.xaml.cs
    ├── Models/
    │   ├── Producto.cs
    │   └── Categoria.cs
    ├── ViewModels/
    │   ├── ProductosViewModel.cs
    │   └── ProductoDetalleViewModel.cs
    ├── Views/
    │   ├── MainWindow.xaml
    │   ├── MainWindow.xaml.cs
    │   ├── ProductoDetalleWindow.xaml
    │   └── ProductoDetalleWindow.xaml.cs
    ├── Services/
    │   ├── IProductoRepository.cs
    │   ├── ProductoRepositorio.cs
    │   └── IDialogService.cs
    └── Resources/
        └── Styles.xaml               ← estilos y plantillas reutilizables
```

## Notas
- Registrar los ViewModels en `App.xaml.cs` o mediante un contenedor de inyección de dependencias simple.
- `IDialogService` permite abrir ventanas modales desde el ViewModel sin acoplar la lógica a la capa de vista.
- Usar `partial class` en los ViewModels para aprovechar la generación de código de CommunityToolkit.Mvvm.
- Los `ResourceDictionary` en `Resources/Styles.xaml` deben fusionarse en `App.xaml` para que los estilos sean globales.
- Este proyecto puede evolucionar sustituyendo el repositorio en memoria por uno que persista datos en JSON o SQLite.
