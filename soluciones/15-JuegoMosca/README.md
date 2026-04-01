# Juego de la Mosca - WPF

Aplicación de escritorio desarrollada en WPF (.NET) que implementa el clásico juego de buscar una mosca escondida en un tablero.

## Descripción del Juego

El objetivo del juego es encontrar la posición de una mosca escondida en un tablero bidimensional:

1. **Configuración inicial**: El jugador elige:
   - Dimensión del tablero (3x3 a 10x10)
   - Número de intentos disponibles

2. **Durante el juego**: El jugador selecciona una fila y columna para "golpear"

3. **Posibles resultados**:
   - ✅ **Acertar**: La mosca estaba en esa posición - ¡GANAS!
   - ⚠️ **Casi**: Estás a distancia 1 de la mosca - ¡Se mueve a otra posición!
   - ❌ **No acertar**: Estás lejos de la mosca
   - 💀 **Fin de intentos**: Agotas todos los intentos sin encontrar la mosca

## Arquitectura del Proyecto

Este proyecto sigue los patrones y tecnologías aprendidos en el módulo de Programación:

### Tecnologías Utilizadas

| Tecnología | Descripción |
|------------|-------------|
| **WPF** | Windows Presentation Foundation - Interfaz de usuario |
| **.NET 10** | Framework de desarrollo |
| **MVVM** | Patrón Model-View-ViewModel |
| **CommunityToolkit.Mvvm** | Librería que facilita MVVM |
| **CSharpFunctionalExtensions** | Programación funcional con ROP |
| **Microsoft.Extensions.DependencyInjection** | Inyección de dependencias |
| **Serilog** | Logging |

### Estructura del Proyecto

```
15-JuegoMosca/
├── Models/                  # Modelos de datos
│   └── Mosca.cs            # MoscaPosition, Acertado, MoscaConstants
├── Errors/                  # Manejo de errores
│   └── MoscaErrors.cs      # Errores con ROP
├── ViewModels/              # Lógica de la aplicación
│   ├── MoscaViewModel.cs   # Lógica del juego
│   └── ConfigViewModel.cs  # Configuración inicial
├── Views/                   # Interfaz de usuario (XAML)
│   ├── Config/              # Ventana de configuración
│   ├── Main/               # Ventana principal del juego
│   └── Dialog/             # Diálogos (Acerca de)
├── Infrastructure/          # Configuración técnica
│   └── DependenciesProvider.cs  # Inyección de dependencias
├── Converters/              # Convertidores para bindings
│   └── InverseBooleanConverter.cs
├── Resources/              # Recursos (iconos, imágenes)
│   ├── app-icon.ico        # Icono de la aplicación
│   └── icons/              # Iconos del menú
└── App.xaml                # Punto de entrada
```

## Patrones de Diseño

### MVVM (Model-View-ViewModel)

```
┌─────────────┐     Binding      ┌──────────────┐     Binding      ┌─────────┐
│    View     │ ──────────────► │  ViewModel   │ ──────────────► │  Model  │
│   (XAML)   │ ◄────────────── │  (C#)        │ ◄────────────── │ (C#)    │
└─────────────┘                 └──────────────┘                  └─────────┘
```

- **Model**: Datos del juego (posición, estado)
- **View**: Interfaz XAML
- **ViewModel**: Lógica y estado, conecta Model y View

### ROP (Railway Oriented Programming)

Manejo de errores como un "ferrocarril":

```
                    ┌─────────────────┐
                    │   Result<T, E>   │
                    └────────┬────────┘
                             │
              ┌──────────────┴──────────────┐
              ▼                                 ▼
    ┌──────────────────┐            ┌──────────────────┐
    │  Success (éxito) │          │   Failure (error) │
    └──────────────────┘            └──────────────────┘
```

### Inyección de Dependencias

Las dependencias se "inyectan" desde el exterior:

```csharp
// En lugar de crear dependencias dentro:
public class MiClase
{
    private readonly MiServicio _servicio = new MiServicio(); // ❌ Mal
}

// Las inyectamos:
public class MiClase
{
    private readonly MiServicio _servicio;
    
    public MiClase(MiServicio servicio) // ✅ Bien
    {
        _servicio = servicio;
    }
}
```

## Código Didáctico

### Ejemplo 1: Modelo con Record

```csharp
// Los "records" son clases inmutables que comparan por valor
public sealed record MoscaPosition(int Fila, int Columna);

// Uso:
var pos1 = new MoscaPosition(2, 3);
var pos2 = new MoscaPosition(2, 3);
bool sonIguales = pos1 == pos2; // true (a diferencia de las clases)
```

### Ejemplo 2: ViewModel con CommunityToolkit

```csharp
// [ObservableProperty] genera automáticamente propiedades reactivas
public partial class MiViewModel : ObservableObject
{
    [ObservableProperty]
    private string _miPropiedad = "";
}

// El código generado será algo como:
// public string MiPropiedad { get => _miPropiedad; set => SetProperty(...) }
```

### Ejemplo 3: ROP con CSharpFunctionalExtensions

```csharp
public Result<Acertado, DomainError> Golpear()
{
    if (acertamos)
        return Result.Success<Acertado, DomainError>(acertado);
    
    return Result.Failure<Acertado, DomainError>(error);
}

// Uso con Match:
resultado.Match(
    onSuccess: acierto => Console.WriteLine($"¡Ganado en {acierto.Intentos} intentos!"),
    onFailure: error => Console.WriteLine($"Error: {error.Message}")
);
```

### Ejemplo 4: Comandos en MVVM

```csharp
// [RelayCommand] genera automáticamente un comando
[RelayCommand]
public void MiMetodo()
{
    // Lógica del botón
}

// En XAML se usa así:
// <Button Command="{Binding MiMetodoCommand}" Content="Clic aquí"/>
```

## Cómo Ejecutar

1. **Requisitos**:
   - .NET 10 SDK
   - Windows

2. **Ejecutar**:

```bash
cd 15-JuegoMosca
dotnet run
```

3. **Opcional: Generar ejecutable**:

```bash
dotnet publish -c Release -r win-x64 --self-contained false
```

## Recursos

- Iconos copiados de la versión JavaFX/Kotlin original
- Estilo visual inspirado en aplicaciones de escritorio clásico

## Ejercicios Propuestos

1. **Añadir niveles de dificultad predefinidos** (Fácil, Medio, Difícil)
2. **Guardar estadísticas** (partidas jugadas, ganadas, perdidas)
3. **Mejorar la interfaz** con animaciones
4. **Añadir sonidos** al golpear
5. **Implementar modo multiplayer** (dos jugadores)

## Licencia

Este proyecto es con fines educativos para el módulo de Programación de 1º DAW.
