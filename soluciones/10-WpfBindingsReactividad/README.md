# 10-WpfBindingsReactividad

## Descripción
Exploración profunda del sistema de binding y reactividad de WPF. El proyecto demuestra los cuatro modos de binding (`OneWay`, `TwoWay`, `OneTime`, `OneWayToSource`), el `UpdateSourceTrigger` y la construcción de formularios completamente reactivos donde la UI refleja el estado del ViewModel en tiempo real.

## Objetivos de Aprendizaje
- Diferenciar los modos de binding: `OneWay`, `TwoWay`, `OneTime`, `OneWayToSource`
- Controlar cuándo se actualiza la fuente con `UpdateSourceTrigger` (`PropertyChanged`, `LostFocus`, `Explicit`)
- Usar `StringFormat` y `Converter` en bindings para transformar datos en la vista
- Enlazar propiedades de controles entre sí (`ElementName` binding)
- Aplicar `MultiBinding` para combinar varias propiedades en una expresión
- Construir un formulario reactivo que actualiza un resumen en tiempo real

## Requisitos Funcionales
- RF-01: Panel de demostración de cada modo de binding con ejemplos visuales e interactivos
- RF-02: Slider y TextBox enlazados bidireccionalmente (mueven el mismo valor)
- RF-03: TextBox con `UpdateSourceTrigger=PropertyChanged` vs `LostFocus`: ver diferencia visual
- RF-04: Formulario de perfil de usuario cuyo resumen se actualiza en tiempo real
- RF-05: Ejemplo de `MultiBinding` que concatena nombre y apellidos
- RF-06: Ejemplo de `Converter` (p. ej. booleano a visibilidad, número a color)

## Requisitos No Funcionales

| Código | Requisito | Descripción |
|--------|-----------|-------------|
| RNF-01 | Didáctico | Cada demo debe estar etiquetada con el concepto que ilustra |
| RNF-02 | MVVM | Todo el estado está en el ViewModel; la vista no tiene lógica |

## Arquitectura
**MVVM con CommunityToolkit.Mvvm**

Cada sección de la ventana principal actúa como una demo independiente con su propia parte del ViewModel.

## Tecnologías
- WPF (.NET 10)
- C# 14
- JetBrains Rider
- CommunityToolkit.Mvvm (NuGet)

## Estructura Sugerida
```
10-WpfBindingsReactividad/
└── WpfBindingsReactividad/
    ├── WpfBindingsReactividad.csproj
    ├── App.xaml
    ├── App.xaml.cs
    ├── Converters/
    │   ├── BoolToVisibilityConverter.cs
    │   └── NumberToColorConverter.cs
    ├── ViewModels/
    │   └── BindingDemoViewModel.cs
    └── Views/
        ├── MainWindow.xaml
        └── MainWindow.xaml.cs
```

## Funcionalidades Clave
- Tabla visual con los cuatro modos de binding y su comportamiento
- Demostración en vivo de `UpdateSourceTrigger` con dos TextBox sincronizados
- Formulario de perfil reactivo (nombre, edad, ciudad → resumen automático)
- Conversores de tipo `IValueConverter` reutilizables

## Notas
- Alineado con la teoría de `09-wpf-mvvm-bindings-reactividad.md`
- `OneWayToSource` actualiza solo la fuente, no la UI; útil para controles de solo escritura
- `UpdateSourceTrigger=Explicit` requiere llamar a `BindingExpression.UpdateSource()` desde código
- Los `IValueConverter` deben registrarse como recursos en XAML antes de usarlos en bindings
