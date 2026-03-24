# 16-WpfTareasBackground

## Descripción
Demostración práctica del problema de bloqueo de la interfaz de usuario en WPF y sus soluciones. El proyecto muestra el comportamiento de la UI ante operaciones largas ejecutadas en el hilo principal, y aplica progresivamente las soluciones: `Thread` manual, `Task.Run`, `async/await`, `IProgress<T>` y `CancellationToken`.

## Objetivos de Aprendizaje
- Reproducir el problema de UI bloqueada con una operación larga en el hilo principal
- Mover operaciones al hilo secundario con `Thread` y con `Task.Run`
- Usar `Dispatcher.Invoke` y `Dispatcher.BeginInvoke` para actualizar controles desde hilos secundarios
- Aplicar el patrón `async/await` para código limpio y sin `Dispatcher` explícito
- Reportar progreso de operaciones largas con `IProgress<T>`
- Cancelar operaciones en curso con `CancellationToken` y `CancellationTokenSource`

## Requisitos Funcionales
- RF-01: Demo 1 — Botón que ejecuta una operación larga en el hilo UI (reproduce el bloqueo)
- RF-02: Demo 2 — Misma operación pero con `Thread` manual y `Dispatcher.Invoke`
- RF-03: Demo 3 — Misma operación con `Task.Run` y `Dispatcher.Invoke`
- RF-04: Demo 4 — Misma operación con `async/await` (sin Dispatcher explícito)
- RF-05: `ProgressBar` que muestra el avance de la operación usando `IProgress<int>`
- RF-06: Botón "Cancelar" que detiene la operación en curso con `CancellationToken`
- RF-07: Indicador de estado: "Ejecutando...", "Completado", "Cancelado"

## Requisitos No Funcionales

| Código | Requisito | Descripción |
|--------|-----------|-------------|
| RNF-01 | Didáctico | Cada demo debe estar claramente etiquetada con el enfoque utilizado |
| RNF-02 | Seguridad | Gestionar correctamente el `CancellationTokenSource` (dispose) |
| RNF-03 | UI responsiva | Las demos 2-4 no deben bloquear la UI |

## Arquitectura
**MVVM con CommunityToolkit.Mvvm**

```
MainWindow  ──binding──►  BackgroundViewModel
                              ├── EjecutarBloqueadoCommand    (Demo 1)
                              ├── EjecutarThreadCommand       (Demo 2)
                              ├── EjecutarTaskRunCommand      (Demo 3)
                              ├── EjecutarAsyncCommand        (Demo 4 - AsyncRelayCommand)
                              ├── CancelarCommand
                              ├── Progreso : int
                              └── Estado : string
```

## Tecnologías
- WPF (.NET 10)
- C# 14
- JetBrains Rider
- CommunityToolkit.Mvvm (NuGet)

## Estructura Sugerida
```
16-WpfTareasBackground/
└── WpfTareasBackground/
    ├── WpfTareasBackground.csproj
    ├── App.xaml
    ├── App.xaml.cs
    ├── ViewModels/
    │   └── BackgroundViewModel.cs
    └── Views/
        ├── MainWindow.xaml
        └── MainWindow.xaml.cs
```

## Funcionalidades Clave
- Comparación visual lado a lado de los cuatro enfoques
- `ProgressBar` animada conectada a `IProgress<int>` desde el ViewModel
- Gestión del ciclo de vida de `CancellationTokenSource` (creación, cancelación, dispose)
- `AsyncRelayCommand` de CommunityToolkit.Mvvm con soporte de cancelación integrado

## Notas
- Alineado con la teoría de `14-wpf-tareas-background.md`
- `IProgress<T>` captura el `SynchronizationContext` del hilo UI al crearse; sus callbacks se ejecutan automáticamente en el hilo correcto
- `CancellationTokenSource` debe ser eliminado (`Dispose`) cuando ya no se necesite
- `async void` solo es aceptable en manejadores de eventos; en ViewModels usar `async Task` con `AsyncRelayCommand`
