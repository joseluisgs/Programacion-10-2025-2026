# 13-WpfJuegoMosca

## Descripción
Juego interactivo de la mosca implementado con MVVM reactivo. El jugador debe hacer clic en botones que representan una mosca, la cual cambia de posición aleatoriamente tras cada acierto. El proyecto combina temporizadores, animaciones simples, puntuación y gestión del tiempo dentro del patrón MVVM.

## Objetivos de Aprendizaje
- Usar `DispatcherTimer` en el ViewModel para actualizar la UI en el hilo principal
- Gestionar posiciones dinámicas de controles mediante binding a propiedades del ViewModel
- Implementar lógica de juego (puntuación, tiempo restante, estado) en el ViewModel
- Aplicar transiciones o animaciones simples usando `Storyboard` o `VisualStateManager`
- Demostrar reactividad completa: la UI responde automáticamente a cambios del ViewModel

## Requisitos Funcionales
- RF-01: Tablero de juego con una cuadrícula de botones (5×5 o similar) que representan posibles posiciones de la mosca
- RF-02: La mosca aparece en un botón aleatorio; al hacer clic en él, suma puntos y se mueve
- RF-03: Temporizador de cuenta atrás (p. ej. 30 segundos) visible en pantalla
- RF-04: Marcador de puntos que se incrementa por cada acierto
- RF-05: Botón "Iniciar" para comenzar y "Reiniciar" para volver a empezar
- RF-06: Cuando el tiempo se agota, mostrar la puntuación final y deshabilitar el tablero
- RF-07: Nivel de dificultad opcional: la mosca se mueve más rápido a mayor puntuación

## Requisitos No Funcionales

| Código | Requisito | Descripción |
|--------|-----------|-------------|
| RNF-01 | MVVM puro | Ninguna lógica de juego en el code-behind |
| RNF-02 | Fluidez | La UI no debe bloquearse; el temporizador corre en el hilo UI via `DispatcherTimer` |

## Arquitectura
**MVVM reactivo con CommunityToolkit.Mvvm**

```
MainWindow  ──binding──►  JuegoViewModel
                              ├── PosicionMoscaActual
                              ├── Puntuacion
                              ├── TiempoRestante
                              ├── EstaEnJuego
                              ├── IniciarCommand
                              ├── ReiniciarCommand
                              └── ClickBotónCommand(posicion)
```

## Tecnologías
- WPF (.NET 10)
- C# 14
- JetBrains Rider
- CommunityToolkit.Mvvm (NuGet)

## Estructura Sugerida
```
13-WpfJuegoMosca/
└── WpfJuegoMosca/
    ├── WpfJuegoMosca.csproj
    ├── App.xaml
    ├── App.xaml.cs
    ├── Models/
    │   └── EstadoJuego.cs
    ├── ViewModels/
    │   └── JuegoViewModel.cs
    └── Views/
        ├── MainWindow.xaml
        └── MainWindow.xaml.cs
```

## Funcionalidades Clave
- Tablero generado dinámicamente con `ItemsControl` y `UniformGrid`
- `DispatcherTimer` encapsulado en el ViewModel para el contador de tiempo
- `[RelayCommand]` parametrizado para el comando de clic en cada celda
- Binding de `Background` de cada botón para resaltar la posición de la mosca
- Nivel de dificultad ajustable mediante binding a un `Slider`

## Notas
- Alineado con la teoría de `09-wpf-mvvm-bindings-reactividad.md`
- `DispatcherTimer` ejecuta su callback en el hilo UI, por lo que no necesita `Dispatcher.Invoke`
- Para el tablero dinámico, usar `ItemsSource` con una colección de celdas y un `DataTemplate` por celda
- La posición de la mosca puede ser un índice entero que se compara en el `DataTemplate` con un `IValueConverter`
