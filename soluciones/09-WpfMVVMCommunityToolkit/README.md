# 09-WpfMVVMCommunityToolkit

## Descripción
Reimplementación del proyecto `08-WpfMVVMBasico` utilizando **CommunityToolkit.Mvvm**. El proyecto aplica las mismas funcionalidades del contador pero aprovechando los generadores de código `[ObservableProperty]` y `[RelayCommand]` del toolkit, eliminando el código repetitivo. Incluye una tabla comparativa entre ambos enfoques.

## Objetivos de Aprendizaje
- Instalar y configurar CommunityToolkit.Mvvm en un proyecto WPF
- Usar `[ObservableProperty]` para generar automáticamente propiedades con notificación de cambios
- Usar `[RelayCommand]` para generar comandos a partir de métodos
- Heredar de `ObservableObject` en lugar de implementar `INotifyPropertyChanged` manualmente
- Comparar el código resultante con la implementación manual del proyecto 08
- Apreciar el uso de Source Generators de C# para reducir el código repetitivo

## Requisitos Funcionales
- RF-01: Mismas funcionalidades que `08-WpfMVVMBasico` (contador con Incrementar, Decrementar, Reiniciar)
- RF-02: El botón "Decrementar" se deshabilita cuando el contador vale 0
- RF-03: La UI se actualiza automáticamente mediante binding

## Requisitos No Funcionales

| Código | Requisito | Descripción |
|--------|-----------|-------------|
| RNF-01 | Dependencias | CommunityToolkit.Mvvm vía NuGet |
| RNF-02 | Código mínimo | El ViewModel debe ser más corto que el equivalente manual |
| RNF-03 | Arquitectura | Separación estricta Model-View-ViewModel |

## Arquitectura
**MVVM con CommunityToolkit.Mvvm**

```
MainWindow.xaml  ──DataContext──►  ContadorViewModel : ObservableObject
    ↑                                    ↓ [ObservableProperty]
    └───── binding ──────────────── _contador → Contador (generado)
    └───── Command ──────────── IncrementarCommand (generado por [RelayCommand])
```

## Tecnologías
- WPF (.NET 10)
- C# 14
- JetBrains Rider
- CommunityToolkit.Mvvm (NuGet)

## Estructura Sugerida
```
09-WpfMVVMCommunityToolkit/
└── WpfMVVMCommunityToolkit/
    ├── WpfMVVMCommunityToolkit.csproj
    ├── App.xaml
    ├── App.xaml.cs
    ├── ViewModels/
    │   └── ContadorViewModel.cs     ← ObservableObject + [ObservableProperty] + [RelayCommand]
    └── Views/
        ├── MainWindow.xaml
        └── MainWindow.xaml.cs
```

## Funcionalidades Clave
- ViewModel en ~20 líneas frente a ~80 del proyecto 08
- `[ObservableProperty]` genera propiedad pública `Contador` con `PropertyChanged`
- `[RelayCommand(CanExecute = nameof(PuedeDecrementar))]` enlaza comando y `CanExecute`
- `OnContadorChanged` (partial method) para ejecutar lógica al cambiar la propiedad

## Tabla comparativa: Manual vs CommunityToolkit

| Aspecto | 08-WpfMVVMBasico (Manual) | 09-WpfMVVMCommunityToolkit |
|---------|--------------------------|----------------------------|
| Notificación de cambios | `INotifyPropertyChanged` manual | `ObservableObject` + `[ObservableProperty]` |
| Comandos | `RelayCommand` propio | `[RelayCommand]` generado |
| Líneas de código (ViewModel) | ~80 | ~20 |
| Dependencias externas | Ninguna | CommunityToolkit.Mvvm |
| Facilidad de mantenimiento | Media | Alta |
| Comprensión del mecanismo | Alta (ves todo) | Requiere conocer el toolkit |

## Notas
- Alineado con la teoría de `08-wpf-arquitectura-mvvm.md`
- Instalar el toolkit: `dotnet add package CommunityToolkit.Mvvm`
- Los campos con `[ObservableProperty]` deben ser `private` y en `camelCase` con guion bajo: `_contador`
- Usar `partial class` en el ViewModel para que el generador de código funcione correctamente
