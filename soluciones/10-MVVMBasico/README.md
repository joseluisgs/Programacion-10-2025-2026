# 10-WpfMVVMBasico - MVVM desde Cero

## Descripcion
Implementacion manual del patron MVVM desde cero, sin librerias externas. El proyecto construye paso a paso `INotifyPropertyChanged` y un `RelayCommand` propio, aplicandolos a un contador simple con botones de incremento, decremento y reinicio. Su proposito es comprender la mecanica interna antes de usar toolkits.

## Objetivos de Aprendizaje
- Implementar `INotifyPropertyChanged` manualmente en una clase ViewModel
- Crear un `RelayCommand` propio que implemente `ICommand`
- Enlazar propiedades del ViewModel a controles de la vista mediante binding
- Entender el rol de `PropertyChanged` en la actualizacion automatica de la UI
- Comparar la implementacion manual con la generada por CommunityToolkit.Mvvm
- Seguir el flujo de datos: View -> Command -> ViewModel -> PropertyChanged -> View

## Requisitos Funcionales
- RF-01: Ventana con un contador numerico visible en pantalla
- RF-02: Boton "Incrementar" que suma 1 al contador
- RF-03: Boton "Decrementar" que resta 1 al contador (minimo 0)
- RF-04: Boton "Reiniciar" que vuelve el contador a 0
- RF-05: El boton "Decrementar" se deshabilita cuando el contador vale 0 (`CanExecute`)
- RF-06: La UI se actualiza automaticamente mediante binding sin codigo en el code-behind

## Requisitos No Funcionales

| Codigo | Requisito | Descripcion |
|--------|-----------|-------------|
| RNF-01 | Sin dependencias externas | Ningun paquete NuGet de MVVM; todo implementado a mano |
| RNF-02 | Codigo educativo | El codigo debe ser claro y estar bien comentado para uso didactico |
| RNF-03 | Arquitectura | Separacion estricta Model-View-ViewModel |

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

## Estructura del Proyecto
```
10-MVVMBasico/
└── WpfMVVMBasico/
    ├── WpfMVVMBasico.csproj
    ├── App.xaml
    ├── App.xaml.cs
    ├── Commands/
    │   └── RelayCommand.cs          <- ICommand manual
    ├── ViewModels/
    │   └── ContadorViewModel.cs     <- INotifyPropertyChanged manual
    └── Views/
        └── Main/
            ├── MainWindow.xaml           <- Bindings a propiedades y comandos
            └── MainWindow.xaml.cs        <- solo DataContext = new ContadorViewModel()
```

## Conceptos Clave

### 1. INotifyPropertyChanged
```csharp
public class ContadorViewModel : INotifyPropertyChanged
{
    private int _contador;
    
    public int Contador
    {
        get => _contador;
        set
        {
            if (_contador != value)
            {
                _contador = value;
                OnPropertyChanged();  // Notifica a la UI que cambio
            }
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
```

### 2. ICommand / RelayCommand
```csharp
public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;
    
    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }
    
    public bool CanExecute(object? parameter) => _canExecute == null || _canExecute();
    
    public void Execute(object? parameter) => _execute();
    
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
```

### 3. Binding en XAML
```xml
<!-- Binding a propiedad: se actualiza cuando cambia -->
<TextBlock Text="{Binding Contador}"/>

<!-- Binding a comando: ejecuta accion -->
<Button Content="Incrementar" Command="{Binding IncrementarCommand}"/>

<!-- CanExecute: se habilita/deshabilita automaticamente -->
<Button Content="Decrementar" Command="{Binding DecrementarCommand}"/>
```

## Flujo de Datos Completo

```
1. Usuario hace clic en boton "Incrementar"
   └─> XAML: Command="{Binding IncrementarCommand}"

2. WPF llama a CanExecute() para verificar si el boton debe estar habilitado
   └─> Si CanExecute() devuelve false, el boton se deshabilita automaticamente

3. Si el boton esta habilitado, WPF llama a Execute()
   └─> Execute() llama a _execute()

4. _execute() ejecuta la logica (ej: Contador++)
   └─> ViewModel cambia el valor de la propiedad

5. ViewModel dispara PropertyChanged para la propiedad
   └─> La UI detecta el cambio y actualiza el valor en pantalla

6. ViewModel dispara CanExecuteChanged
   └─> La UI vuelve a llamar a CanExecute()
   └─> Se actualiza el estado de los botones
```

## Comparacion con CommunityToolkit.Mvvm

### Version Manual (este proyecto)
```csharp
// ViewModel
private int _contador;
public int Contador
{
    get => _contador;
    set { _contador = value; OnPropertyChanged(); }
}

public RelayCommand IncrementarCommand { get; }
public ContadorViewModel()
{
    IncrementarCommand = new RelayCommand(() => Contador++);
}

// XAML
<TextBlock Text="{Binding Contador}"/>
<Button Command="{Binding IncrementarCommand}"/>
```

### Version con CommunityToolkit.Mvvm
```csharp
// ViewModel
[ObservableProperty]
private int _contador;

[RelayCommand]
private void Incrementar() => Contador++;

// XAML (igual)
<TextBlock Text="{Binding Contador}"/>
<Button Command="{Binding IncrementarCommand}"/>
```

El resultado es identico, pero con CommunityToolkit se escribe menos codigo.

## Tecnologias
- WPF (.NET 10)
- C# 14
- Sin paquetes NuGet externos

## Como Ejecutar
```bash
cd WpfMVVMBasico
dotnet run
```

## Notas
- Este proyecto es el punto de partida para entender por que toolkits como CommunityToolkit.Mvvm son utiles
- Comparar directamente con el proyecto 09-GestionProductos para apreciar la reduccion de codigo repetitivo
- El code-behind (MainWindow.xaml.cs) SOLO tiene una linea: DataContext = new ContadorViewModel()
- No hay ninguna logica en el code-behind, todo esta en el ViewModel

## Ejercicios Propuestos
1. Agregar un boton "Duplicar" que multiplique el contador por 2
2. Agregar una propiedad "EsPar" que muestre si el contador es par/impar
3. Agregar validacion para no incrementar mas de 100
4. Agregar un ListBox con el historial de operaciones
