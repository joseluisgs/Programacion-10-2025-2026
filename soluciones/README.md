# Soluciones — Proyectos Prácticos

Directorio de proyectos prácticos de la asignatura.

## Índice de proyectos

| # | Proyecto | Arquitectura | Descripción breve |
|---|----------|--------------|-------------------|
| 01 | [PatronObserver](./01-PatronObserver/) | Consola | Patrón Observer: interfaces, eventos, EventAggregator |
| 02 | [IntroWinForms](./02-IntroWinForms/) | WinForms | Eventos, Anchor/Dock, validación |
| 03 | [IntroWPF](./03-IntroWPF/) | WPF | Primera ventana XAML, ciclo de vida, Hot Reload |
| 04 | [LayoutsComponentes](./04-LayoutsComponentes/) | WPF | Grid, StackPanel, DockPanel, componentes |
| 05 | [ListaCompra](./05-ListaCompra/) | MVC → MVVM | ObservableCollection y ListBox |
| 06 | [FormularioValidacion](./06-FormularioValidacion/) | MVVM | IDataErrorInfo y ValidationRules |
| 07 | [NavegacionVentanas](./07-NavegacionVentanas/) | MVC + MVVM | ShowDialog, Show, paso de datos |
| 08 | [Dialogos](./08-Dialogos/) | WPF | OpenFileDialog, SaveFileDialog |
| 09 | [GestionProductos](./09-GestionProductos/) | MVVM | CRUD completo, multi-ventana |
| 10 | [MVVMBasico](./10-MVVMBasico/) | MVVM manual | INotifyPropertyChanged, ICommand |
| 11 | [MVVMCommunityToolkit](./11-MVVMCommunityToolkit/) | MVVM + Toolkit | [ObservableProperty], [RelayCommand] |
| 12 | [BindingsReactividad](./12-BindingsReactividad/) | MVVM | Modos binding, UpdateSourceTrigger, IValueConverter, **FormData + IDataErrorInfo** |
| 13 | [ListasMenusTablas](./13-ListasMenusTablas/) | MVVM | ListView, DataGrid, ComboBox, Menu |
| 14 | [ListaCompraMvvm](./14-ListaCompraMvvm/) | MVVM | Repository, DI, ObservableCollection, **FormData + IDataErrorInfo** |
| 15 | [JuegoMosca](./15-JuegoMosca/) | MVVM reactivo | DispatcherTimer, puntuación |
| 16 | [Pokedex](./16-Pokedex/) | MVVM | JSON local, DataTemplate, búsqueda |
| 17 | [TemasEstilos](./17-TemasEstilos/) | WPF | Material Design, temas dinámicos |
| 18 | [TareasBackground](./18-TareasBackground/) | MVVM + Background | Thread, Task, async/await |
| 19 | [StarWars](./19-StarWars/) | MVVM + API REST | SWAPI, búsqueda, cancelación |
| 20 | [GestionAcademica](./20-GestionAcademica/) | MVVM profesional | CRUD completo, múltiples repositories, imágenes, ScottPlot, NUnit |
| 21 | [GestionAcademicaReactive](./21-GestionAcademicaReactive/) | MVVM reactivo | **Versión 2.0**: más reactiva y optimizada con ItemViewModels |
| 22 | [ListaTareasBlazor](./22-ListaTareasBlazor/) | Blazor Server | Introducción a Blazor |
| 23 | [ListaTareasMAUI](./23-ListaTareasMAUI/) | .NET MAUI | Introducción a MAUI |
| 24 | [ListaTareasAvalonia](./24-ListaTareasAvalonia/) | Avalonia UI | Framework multiplataforma |
| 25 | [ListaTareasOpenSilver](./25-ListaTareasOpenSilver/) | OpenSilver | WPF en navegador (WebAssembly) |
| 26 | [GestionAcademicaBlazor](./26-GestionAcademicaBlazor/) | Blazor Server | Port a Blazor Server con async/await, Clean Architecture |

* = Proyecto opcional

## Progresión pedagógica

```
01              Patrón Observer (consola)
   ↓
02              WinForms (introducción)
   ↓
03              WPF intro
   ↓
04              Layouts y Componentes WPF
   ↓
05 → 06        Transición MVC → MVVM
   ↓
07 → 08        Navegación y Diálogos
   ↓
09 → 10        MVVM manual vs Toolkit
   ↓
11 → 12        Bindings y Reactividad + FormData/IDataErrorInfo
   ↓
13 → 14        Listas, tablas y CRUD + FormData/IDataErrorInfo
   ↓
15 → 16        Juegos y API REST
   ↓
17 → 18        Repositorios y Background Tasks
   ↓
19 → 20        Gestión Académica v1 (múltiples repositories)
   ↓
21              Gestión Académica v2 (reactiva y optimizada)
   ↓
26              Gestión Académica Blazor (async/await, Blazor Server)

========================================
       PROYECTOS OPCIONALES
   ↓
22 → 25         Blazor + MAUI + Avalonia + OpenSilver
```
