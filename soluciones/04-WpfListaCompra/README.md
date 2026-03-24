# WPF Lista de la Compra

## Descripción
Aplicación para gestionar una lista de la compra. El usuario puede añadir artículos, marcarlos como comprados y eliminarlos de la lista. El proyecto sirve de puente entre el enfoque MVC con code-behind y el patrón MVVM: comienza con una implementación directa en code-behind y evoluciona hacia el uso de `ObservableCollection` y bindings básicos.

## Objetivos de Aprendizaje
- Utilizar `ListBox` para mostrar una colección dinámica de elementos
- Añadir y eliminar elementos de una colección en tiempo de ejecución
- Introducir `ObservableCollection<T>` y entender por qué notifica cambios a la UI
- Comprender el atributo `ItemsSource` y el binding básico de colecciones
- Gestionar el elemento seleccionado con `SelectedItem` / `SelectedIndex`
- Dar los primeros pasos hacia la separación de lógica y presentación (transición a MVVM)

## Requisitos Funcionales
- Campo de texto para introducir el nombre del artículo
- Botón "Añadir" que agrega el artículo a la lista (no se permiten nombres vacíos ni duplicados)
- `ListBox` que muestra todos los artículos de la lista
- Botón "Eliminar seleccionado" que borra el artículo actualmente seleccionado
- Botón "Marcar como comprado" que tacha o destaca visualmente el artículo seleccionado
- Botón "Limpiar lista" que elimina todos los artículos tras confirmación con `MessageBox`
- Contador de artículos totales y artículos pendientes visible en la interfaz

## Arquitectura
**MVC (Code-Behind) con transición hacia MVVM**

La versión inicial gestiona la `ObservableCollection` directamente en `MainWindow.xaml.cs`. Como evolución, se extrae la lógica a una clase `ListaCompraViewModel` y se conecta mediante `DataContext`, introduciendo así los conceptos de binding y separación de responsabilidades.

## Tecnologías
- WPF (.NET 10)
- C# 14
- JetBrains Rider

## Estructura Sugerida
```
04-WpfListaCompra/
└── WpfListaCompra/
    ├── WpfListaCompra.csproj
    ├── App.xaml
    ├── App.xaml.cs
    ├── MainWindow.xaml
    ├── MainWindow.xaml.cs
    ├── Models/
    │   └── Articulo.cs           ← clase con Nombre e IsComprado
    └── ViewModels/               ← (fase 2, transición MVVM)
        └── ListaCompraViewModel.cs
```

## Notas
- Comenzar con la lista gestionada íntegramente en el code-behind y después refactorizar.
- `ObservableCollection<T>` debe estar definida antes de asignarse como `ItemsSource`; no reasignarla en cada actualización.
- Para tachar visualmente artículos comprados, se puede usar un `DataTemplate` con un `TextDecoration` condicional o simplemente cambiar el color del texto.
- El contador de pendientes se puede actualizar cada vez que cambia la colección suscribiéndose al evento `CollectionChanged`.
