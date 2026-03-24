# Soluciones — Proyectos WPF

Directorio de proyectos prácticos de la asignatura. Cada carpeta contiene un proyecto WPF independiente con su propio `README.md` que describe los objetivos, requisitos y arquitectura.

## Índice de proyectos

| # | Proyecto | Arquitectura | Descripción breve |
|---|----------|--------------|-------------------|
| 01 | [WpfHolaMundo](./01-WpfHolaMundo/) | MVC (code-behind) | Ventana con botón que cambia texto — introducción a WPF |
| 02 | [WpfCalculadora](./02-WpfCalculadora/) | MVC (code-behind) | Calculadora con operaciones aritméticas básicas |
| 03 | [WpfFormularioRegistro](./03-WpfFormularioRegistro/) | MVC (code-behind) | Formulario con validación y múltiples tipos de controles |
| 04 | [WpfListaCompra](./04-WpfListaCompra/) | MVC → MVVM | Lista dinámica con `ObservableCollection` y `ListBox` |
| 05 | [WpfFormularioValidacion](./05-WpfFormularioValidacion/) | MVVM | Formulario con validación MVVM: `IDataErrorInfo` y `ValidationRules` |
| 06 | [WpfNavigacionVentanas](./06-WpfNavigacionVentanas/) | MVC + MVVM | Navegación entre ventanas: `ShowDialog`, `Show`, paso de datos, ViewModel compartido |
| 07 | [WpfDialogos](./07-WpfDialogos/) | MVC (code-behind) | Diálogos del sistema: `OpenFileDialog`, galería de imágenes con `WrapPanel` |
| — | [WpfGestionProductos](./07-WpfGestionProductos/) | MVVM completo | CRUD completo, multi-ventana, servicios y validación *(proyecto complementario)* |
| 08 | [WpfMVVMBasico](./08-WpfMVVMBasico/) | MVVM manual | `INotifyPropertyChanged` y `ICommand` implementados desde cero |
| 09 | [WpfMVVMCommunityToolkit](./09-WpfMVVMCommunityToolkit/) | MVVM + Toolkit | Mismo contador que 08 pero con `[ObservableProperty]` y `[RelayCommand]` |
| 10 | [WpfBindingsReactividad](./10-WpfBindingsReactividad/) | MVVM | Modos de binding, `UpdateSourceTrigger`, `IValueConverter`, formulario reactivo |
| 11 | [WpfListasMenusTablas](./11-WpfListasMenusTablas/) | MVVM | `ListView`, `DataGrid`, `ComboBox`, `Menu` y `ContextMenu` con binding |
| 12 | [WpfGestorContactos](./12-WpfGestorContactos/) | MVVM | CRUD de contactos con `DataGrid` y CommunityToolkit.Mvvm |
| 13 | [WpfJuegoMosca](./13-WpfJuegoMosca/) | MVVM reactivo | Juego de la mosca con `DispatcherTimer`, puntuación y niveles |
| 14 | [WpfPokedex](./14-WpfPokedex/) | MVVM + API REST | Consumo de PokéAPI con `HttpClient`, `async/await` y caché de imágenes |
| 15 | [WpfExpedientesAcademicos](./15-WpfExpedientesAcademicos/) | MVVM + Repositorios | CRUD multi-entidad, validación completa, navegación multi-ventana |
| 16 | [WpfTareasBackground](./16-WpfTareasBackground/) | MVVM + Background | UI bloqueada vs `Thread`/`Task`/`async`, `IProgress<T>`, `CancellationToken` |
| 17 | [WpfStarWars](./17-WpfStarWars/) | MVVM + API REST + BG | SWAPI con búsqueda asíncrona, caché de resultados y cancelación |
| 18 | [WpfGestionAcademicaPro](./18-WpfGestionAcademicaPro/) | MVVM profesional | **Proyecto final**: CRUD, imágenes, ScottPlot, informes HTML, NUnit + Moq, DI |

## Progresión pedagógica

```
01 → 02 → 03              Fundamentos WPF y code-behind
              ↓
              04           Transición MVC → MVVM / ObservableCollection
              ↓
         05 → 06           MVVM con validación y navegación entre ventanas
              ↓
              07           Diálogos del sistema y layouts avanzados
              ↓
         08 → 09           MVVM manual vs CommunityToolkit.Mvvm
              ↓
        10 → 11            Bindings avanzados, listas, tablas y menús
              ↓
             12            MVVM con CommunityToolkit: CRUD de contactos
              ↓
             13            MVVM reactivo con DispatcherTimer
              ↓
        14 → 15            API REST asíncrona + CRUD completo multi-entidad
              ↓
        16 → 17            Background tasks: Thread, Task, async/await, IProgress<T>
              ↓
             18            Proyecto final integrador (profesional)
```

## Tecnologías comunes
- WPF (.NET 10)
- C# 14
- JetBrains Rider
- CommunityToolkit.Mvvm (proyectos 06 y siguientes)
- ScottPlot (proyecto 18)
- NUnit + Moq (proyecto 18)
