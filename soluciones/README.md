# Soluciones — Proyectos WPF

Directorio de proyectos prácticos de la asignatura. Cada carpeta contiene un proyecto WPF independiente con su propio `README.md` que describe los objetivos, requisitos y arquitectura.

## Índice de proyectos

| # | Proyecto | Arquitectura | Descripción breve |
|---|----------|--------------|-------------------|
| 01 | [WpfHolaMundo](./01-WpfHolaMundo/) | MVC (code-behind) | Ventana con botón que cambia texto — introducción a WPF |
| 02 | [WpfCalculadora](./02-WpfCalculadora/) | MVC (code-behind) | Calculadora con operaciones aritméticas básicas |
| 03 | [WpfFormularioRegistro](./03-WpfFormularioRegistro/) | MVC (code-behind) | Formulario con validación y múltiples tipos de controles |
| 04 | [WpfListaCompra](./04-WpfListaCompra/) | MVC → MVVM | Lista dinámica con `ObservableCollection` y `ListBox` |
| 05 | [WpfGaleriaImagenes](./05-WpfGaleriaImagenes/) | MVC (code-behind) | Galería con `OpenFileDialog` y `WrapPanel` |
| 06 | [WpfGestorContactos](./06-WpfGestorContactos/) | MVVM | CRUD de contactos con `DataGrid` y CommunityToolkit.Mvvm |
| 07 | [WpfGestionProductos](./07-WpfGestionProductos/) | MVVM completo | CRUD completo, multi-ventana, servicios y validación |

## Progresión pedagógica

```
01 → 02 → 03          Fundamentos WPF y code-behind
          ↓
          04           Transición MVC → MVVM / ObservableCollection
          ↓
     05 ← 04           Diálogos del sistema y layouts avanzados
          ↓
          06           MVVM con CommunityToolkit.Mvvm
          ↓
          07           MVVM completo: servicios, navegación y validación
```

## Tecnologías comunes
- WPF (.NET 10)
- C# 14
- JetBrains Rider
- CommunityToolkit.Mvvm (proyectos 06 y 07)
