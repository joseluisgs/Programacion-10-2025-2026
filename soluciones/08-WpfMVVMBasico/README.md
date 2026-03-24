# 08-WpfMVVMBasico

## Descripción
Implementación manual del patrón MVVM desde cero, sin librerías externas. El proyecto construye paso a paso `INotifyPropertyChanged` y un `RelayCommand` propio, aplicándolos a un contador simple con botones de incremento, decremento y reinicio. Su propósito es comprender la mecánica interna antes de usar toolkits.

## Objetivos de Aprendizaje
- Implementar `INotifyPropertyChanged` manualmente en una clase ViewModel
- Crear un `RelayCommand` propio que implemente `ICommand`
- Enlazar propiedades del ViewModel a controles de la vista mediante binding
- Entender el rol de `PropertyChanged` en la actualización automática de la UI
- Comparar la implementación manual con la generada por CommunityToolkit.Mvvm (ver proyecto 09)
- Seguir el flujo de datos: View → Command → ViewModel → PropertyChanged → View

## Requisitos Funcionales
- RF-01: Ventana con un contador numérico visible en pantalla
- RF-02: Botón "Incrementar" que suma 1 al contador
- RF-03: Botón "Decrementar" que resta 1 al contador (mínimo 0)
- RF-04: Botón "Reiniciar" que vuelve el contador a 0
- RF-05: El botón "Decrementar" se deshabilita cuando el contador vale 0 (`CanExecute`)
- RF-06: La UI se actualiza automáticamente mediante binding sin código en el code-behind

## Requisitos No Funcionales

| Código | Requisito | Descripción |
|--------|-----------|-------------|
| RNF-01 | Sin dependencias externas | Ningún paquete NuGet de MVVM; todo implementado a mano |
| RNF-02 | Código educativo | El código debe ser claro y estar bien comentado para uso didáctico |
| RNF-03 | Arquitectura | Separación estricta Model-View-ViewModel |

## Arquitectura
**MVVM manual (sin toolkit)**

```
MainWindow.xaml  ──DataContext──►  ContadorViewModel
    ↑                                    ↓ INotifyPropertyChanged
    └───── binding ──────────────── Contador (propiedad)
    └───── Command ──────────── IncrementarCommand (RelayCommand)
```

- `ContadorViewModel` implementa `INotifyPropertyChanged` directamente.
- `RelayCommand` implementa `ICommand` con delegados `Action` y `Func<bool>` para `Execute` y `CanExecute`.

## Tecnologías
- WPF (.NET 10)
- C# 14
- JetBrains Rider

## Estructura Sugerida
```
08-WpfMVVMBasico/
└── WpfMVVMBasico/
    ├── WpfMVVMBasico.csproj
    ├── App.xaml
    ├── App.xaml.cs
    ├── Commands/
    │   └── RelayCommand.cs          ← ICommand manual
    ├── ViewModels/
    │   └── ContadorViewModel.cs     ← INotifyPropertyChanged manual
    └── Views/
        ├── MainWindow.xaml
        └── MainWindow.xaml.cs       ← solo DataContext = new ContadorViewModel()
```

## Funcionalidades Clave
- `RelayCommand` genérico reutilizable con `Execute(object?)` y `CanExecute(object?)`
- `ContadorViewModel` con propiedad `Contador` que notifica cambios
- Invocación de `CanExecuteChanged` al cambiar el valor del contador para refrescar el estado del botón "Decrementar"
- `DataContext` asignado en el constructor de la ventana

## Notas
- Alineado con la teoría de `08-wpf-arquitectura-mvvm.md`
- Este proyecto es el punto de partida para entender por qué toolkits como CommunityToolkit.Mvvm son útiles
- Comparar directamente con el proyecto `09-WpfMVVMCommunityToolkit` para apreciar la reducción de código repetitivo
