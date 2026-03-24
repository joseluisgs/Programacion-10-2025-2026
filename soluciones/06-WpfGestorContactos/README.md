# WPF Gestor de Contactos — MVVM

## Descripción
Aplicación de gestión de contactos que implementa el patrón MVVM completo con la biblioteca **CommunityToolkit.Mvvm**. El estudiante construye un sistema de CRUD (crear, leer, actualizar y eliminar) sobre una colección de contactos, mostrándolos en un `DataGrid` y gestionando las operaciones mediante comandos (`ICommand`/`RelayCommand`).

## Objetivos de Aprendizaje
- Aplicar el patrón MVVM separando Model, View y ViewModel en capas independientes
- Usar `ObservableObject` y `[ObservableProperty]` de CommunityToolkit.Mvvm para notificación automática de cambios
- Definir y enlazar comandos con `[RelayCommand]` desde el ViewModel
- Mostrar y editar datos tabulares con `DataGrid` mediante `ItemsSource` y binding bidireccional
- Gestionar el elemento seleccionado con `SelectedItem` vinculado al ViewModel
- Filtrar la lista de contactos en tiempo real mediante una propiedad de búsqueda

## Requisitos Funcionales
- Listado de contactos en un `DataGrid` con columnas: nombre, apellidos, teléfono y correo electrónico
- Formulario lateral o inferior para introducir o editar los datos de un contacto
- Botón "Nuevo" que limpia el formulario para añadir un contacto
- Botón "Guardar" que añade o actualiza el contacto seleccionado (con validación básica)
- Botón "Eliminar" que borra el contacto seleccionado tras confirmación
- Campo de búsqueda que filtra el `DataGrid` por nombre o correo en tiempo real
- Los botones "Guardar" y "Eliminar" deben deshabilitarse automáticamente cuando no procede su uso (CanExecute)

## Arquitectura
**MVVM con CommunityToolkit.Mvvm**

```
View  ──binding──►  ViewModel  ──usa──►  Model
(XAML)             (lógica de           (datos puros)
                    presentación)
```

- **Model**: clase `Contacto` con propiedades de datos.
- **ViewModel**: `ContactosViewModel` hereda de `ObservableObject`; expone la colección, el contacto seleccionado y los comandos.
- **View**: `MainWindow.xaml` sin lógica en el code-behind (solo asignación del `DataContext`).

## Tecnologías
- WPF (.NET 10)
- C# 14
- JetBrains Rider
- CommunityToolkit.Mvvm (NuGet)

## Estructura Sugerida
```
06-WpfGestorContactos/
└── WpfGestorContactos/
    ├── WpfGestorContactos.csproj
    ├── App.xaml
    ├── App.xaml.cs
    ├── Models/
    │   └── Contacto.cs
    ├── ViewModels/
    │   └── ContactosViewModel.cs
    └── Views/
        ├── MainWindow.xaml
        └── MainWindow.xaml.cs    ← solo DataContext = new ContactosViewModel()
```

## Notas
- Instalar CommunityToolkit.Mvvm vía NuGet: `dotnet add package CommunityToolkit.Mvvm`.
- Las propiedades generadas por `[ObservableProperty]` siguen la convención `_camelCase` (campo) → `PascalCase` (propiedad pública).
- Para el filtrado en tiempo real, usar `CollectionViewSource` o una propiedad `ContactosFiltrados` que devuelva la colección filtrada.
- Evitar poner lógica de negocio en el code-behind de la View; si se necesita abrir un diálogo, delegar mediante un servicio o evento.
