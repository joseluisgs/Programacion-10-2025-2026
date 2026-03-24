# 11-WpfListasMenusTablas

## Descripción
Proyecto de referencia para controles de listas, tablas y menús en WPF. Cubre `ListView` con `ItemTemplate` personalizado, `DataGrid` con columnas configurables, `ComboBox` enlazado a datos, `Menu` y `ContextMenu`. Todo bajo el patrón MVVM con binding a colecciones observables.

## Objetivos de Aprendizaje
- Mostrar colecciones de objetos en `ListView` con `DataTemplate` personalizado
- Configurar columnas de `DataGrid` con tipos distintos (`DataGridTextColumn`, `DataGridCheckBoxColumn`, `DataGridTemplateColumn`)
- Enlazar un `ComboBox` a una lista de opciones con `SelectedItem` en el ViewModel
- Construir menús de aplicación con `Menu` y `MenuItem` enlazados a comandos
- Añadir `ContextMenu` a elementos de lista para acciones contextuales
- Filtrar y ordenar colecciones usando `CollectionViewSource`

## Requisitos Funcionales
- RF-01: `ListView` de productos con imagen, nombre, precio y categoría usando `DataTemplate`
- RF-02: `DataGrid` de estudiantes con columnas de texto, número y checkbox (activo/inactivo)
- RF-03: `ComboBox` de categorías enlazado a una lista; filtra los productos del `ListView`
- RF-04: `Menu` principal con opciones: Archivo (Nuevo, Abrir, Guardar, Salir), Ver, Ayuda
- RF-05: `ContextMenu` en la lista de productos con opciones Editar y Eliminar
- RF-06: Barra de estado con el número de elementos mostrados

## Requisitos No Funcionales

| Código | Requisito | Descripción |
|--------|-----------|-------------|
| RNF-01 | MVVM | Comandos en el ViewModel para todas las acciones de menú |
| RNF-02 | Reutilización | Los `DataTemplate` se definen en `ResourceDictionary` |

## Arquitectura
**MVVM con CommunityToolkit.Mvvm**

```
MainWindow  ──binding──►  ListasViewModel
                              ├── Productos : ObservableCollection<Producto>
                              ├── Estudiantes : ObservableCollection<Estudiante>
                              ├── Categorias : ObservableCollection<string>
                              └── CategoriaSeleccionada
```

## Tecnologías
- WPF (.NET 10)
- C# 14
- JetBrains Rider
- CommunityToolkit.Mvvm (NuGet)

## Estructura Sugerida
```
11-WpfListasMenusTablas/
└── WpfListasMenusTablas/
    ├── WpfListasMenusTablas.csproj
    ├── App.xaml
    ├── App.xaml.cs
    ├── Models/
    │   ├── Producto.cs
    │   └── Estudiante.cs
    ├── ViewModels/
    │   └── ListasViewModel.cs
    ├── Views/
    │   ├── MainWindow.xaml
    │   └── MainWindow.xaml.cs
    └── Resources/
        └── DataTemplates.xaml       ← templates reutilizables
```

## Funcionalidades Clave
- `ListView` con `GridView` para vista de tabla y con `ItemTemplate` para vista de tarjetas
- Ordenación de `DataGrid` por columna con clic en la cabecera
- `CollectionViewSource` para filtrar y ordenar sin modificar la colección original
- Menú con separadores, atajos de teclado (`InputGestureText`) y estado habilitado/deshabilitado

## Notas
- Alineado con la teoría de `09-wpf-mvvm-bindings-reactividad.md`
- `DataGrid.CanUserAddRows = False` evita que el usuario inserte filas directamente en la tabla
- Para el `ContextMenu` en items de lista, usar `DataContext` del item contenedor, no de la ventana
- `CollectionViewSource.GetDefaultView(coleccion)` devuelve la vista de la colección para aplicar filtros
