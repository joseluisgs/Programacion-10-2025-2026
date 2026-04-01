# 13-ListasMenusTablas

## Descripción
Proyecto de referencia para controles de listas, tablas, menús, checkboxes, radiobuttons y calendarios en WPF. Cubre `ListView` con `GridView`, `DataGrid` con columnas configurables, `ComboBox` enlazado a datos, `Menu`, `ContextMenu`, `CheckBox`, `RadioButton` y `Calendar`. Todo bajo el patrón MVVM con CommunityToolkit.Mvvm.

## Objetivos de Aprendizaje
- Mostrar colecciones de objetos en `ListView` con `GridView` y filtrado por categoría
- Configurar columnas de `DataGrid` con tipos distintos (`DataGridTextColumn`, `DataGridCheckBoxColumn`, `DataGridTemplateColumn`)
- Enlazar un `ComboBox` a una lista de opciones con `SelectedItem` en el ViewModel
- Construir menús de aplicación con `Menu` y `MenuItem` enlazados a comandos
- Añadir `ContextMenu` a elementos de lista para acciones contextuales
- Comprender la diferencia entre **CheckBox** (selección múltiple) y **RadioButton** (selección única)
- Usar `Calendar` para seleccionar fechas y procesarlas con formato localized

## Requisitos Funcionales
- RF-01: `ListView` de productos con nombre, precio y categoría usando `GridView`
- RF-02: `ComboBox` de categorías que filtra los productos del `ListView`
- RF-03: `DataGrid` de estudiantes con columnas de texto, número y checkbox (activo/inactivo)
- RF-04: `Menu` principal con opciones: Archivo (Nuevo, Abrir, Guardar, Salir), Ver, Ayuda
- RF-05: `ContextMenu` en la lista de productos con opciones Editar y Eliminar
- RF-06: Barra de estado con el número de elementos mostrados
- RF-07: CheckBox para selección múltiple (varias opciones a la vez)
- RF-08: RadioButton para selección única (solo una prioridad)
- RF-09: Calendar para seleccionar fecha y procesarla en español

## Diferencia entre CheckBox y RadioButton

| Característica | CheckBox | RadioButton |
|---------------|----------|-------------|
| Selección | Múltiple (pueden marcarse varios) | Única (solo uno) |
| Uso típico | Aceptar términos, seleccionar características | Prioridad, tipo, categoría |
| Grupo | No necesita grupo | Necesita mismo `GroupName` |
| Ejemplo | "Quiero pizza", "Quiero bebida" | "Prioridad: Baja, Media, Alta" |

## Arquitectura
**MVVM con CommunityToolkit.Mvvm**

```
MainWindow  ──binding──►  ListasViewModel
                              ├── Productos : ObservableCollection<Producto>
                              ├── ProductosFiltrados : ObservableCollection<Producto>
                              ├── Estudiantes : ObservableCollection<Estudiante>
                              ├── Categorias : ObservableCollection<string>
                              ├── CategoriaSeleccionada
                              ├── OpcionA/B/C : bool (CheckBox)
                              ├── PrioridadSeleccionada : string (RadioButton)
                              └── FechaSeleccionada : DateTime? (Calendar)
```

## Tecnologías
- WPF (.NET 10)
- C# 14
- CommunityToolkit.Mvvm (NuGet)

## Estructura
```
13-ListasMenusTablas/
└── ListasMenusTablas/
    ├── ListasMenusTablas.csproj
    ├── App.xaml
    ├── App.xaml.cs
    ├── Models/
    │   └── Models.cs              # Producto, Estudiante
    ├── ViewModels/
    │   └── ListasViewModel.cs     # Lógica MVVM
    ├── Views/
    │   ├── MainWindow.xaml        # UI con ListView, DataGrid, ComboBox, Menu, CheckBox, RadioButton, Calendar
    │   └── MainWindow.xaml.cs
    └── Converters/
        └── StringToBoolConverter.cs  # Converter para RadioButton
```

## Controles Implementados

### ListView con GridView
- Muestra productos en formato tabular
- Columnas: Nombre, Precio (formato moneda), Categoría
- ContextMenu para editar/eliminar

### ComboBox
- Lista de categorías: Todas, Electrónica, Alimentación, Ropa, Hogar
- Filtra automáticamente el ListView al seleccionar

### DataGrid
- Muestra estudiantes con columnas: Nombre, Apellidos, Edad, Activo (CheckBox), Nota
- Botones de acción en última columna

### Menu
- Archivo: Nuevo, Abrir, Guardar, Salir
- Ver: Actualizar
- Ayuda: Acerca de, Ayuda

### CheckBox (Selección múltiple)
- Tres opciones: Opción A, B, C
- Se pueden marcar varias a la vez
- Muestra texto con las seleccionadas

### RadioButton (Selección única)
- Tres prioridades: Baja, Media, Alta
- Solo una puede estar seleccionada
- Al cambiar, muestra mensaje informativo

### Calendar
- Muestra fecha actual por defecto
- Permite seleccionar cualquier fecha
- Botón "Fecha Actual" para restablecer
- Botón "Procesar Fecha" muestra día de la semana, mes y año en español

## Cómo Ejecutar
```bash
cd 13-ListasMenusTablas/ListasMenusTablas
dotnet run
```

## Notas
- Los `CheckBox` usan propiedades `bool` independientes
- Los `RadioButton` usan el mismo `GroupName` y un Converter para binding bidireccional
- El `Calendar` usa `DateTime?` para permitir valor nulo
- `CultureInfo.CurrentUICulture` obtiene el idioma del sistema para localize
- `DataGrid.CanUserAddRows = False` evita que el usuario inserte filas directamente
- Para el `ContextMenu` en items de lista, usar `DataContext` del item contenedor
