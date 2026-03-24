# 05 - Reactividad en WPF: Notificación de Cambios

## 1. ¿Qué es la Reactividad?

La **reactividad** en el contexto de interfaces gráficas se refiere a la capacidad de la UI de **actualizarse automáticamente** cuando los datos subyacentes cambian. En WPF, esto se logra mediante el sistema de **data binding** (vinculación de datos) y notificaciones de cambios de propiedad.

### 1.1 El Problema sin Reactividad

```csharp
// Modelo sin reactividad
public class Persona
{
    public string Nombre { get; set; }
    public int Edad { get; set; }
}

// En el code-behind
Persona persona = new() { Nombre = "Ana", Edad = 25 };
txtNombre.Text = persona.Nombre; // Sincronización manual

// Si cambiamos el modelo...
persona.Nombre = "Ana García";
// ¡La UI NO se actualiza automáticamente!
txtNombre.Text = persona.Nombre; // Tenemos que actualizar manualmente
```

**Problemas:**

❌ Sincronización manual propensa a errores  
❌ Código repetitivo y difícil de mantener  
❌ Difícil mantener consistencia entre modelo y vista  
❌ No escala en aplicaciones complejas  

### 1.2 La Solución: Data Binding

```csharp
// Con data binding (simplificado)
txtNombre.SetBinding(TextBox.TextProperty, new Binding("Nombre") { Source = persona });

// Ahora, cuando cambie persona.Nombre, ¡la UI se actualiza sola!
// (Siempre que Persona implemente INotifyPropertyChanged)
```

---

## 2. INotifyPropertyChanged: El Contrato de Notificación

### 2.1 La Interfaz

**INotifyPropertyChanged** es una interfaz del namespace `System.ComponentModel` que define un único evento:

```csharp
public interface INotifyPropertyChanged
{
    event PropertyChangedEventHandler? PropertyChanged;
}

// Firma del delegado del evento
public delegate void PropertyChangedEventHandler(
    object? sender, 
    PropertyChangedEventArgs e
);
```

### 2.2 Implementación Manual (Forma Clásica)

```csharp
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ModeloReactivo;

public class Persona : INotifyPropertyChanged
{
    private string _nombre = "";
    private int _edad;
    
    public string Nombre
    {
        get => _nombre;
        set
        {
            if (_nombre != value)
            {
                _nombre = value;
                OnPropertyChanged(); // Notifica el cambio
            }
        }
    }
    
    public int Edad
    {
        get => _edad;
        set
        {
            if (_edad != value)
            {
                _edad = value;
                OnPropertyChanged();
            }
        }
    }
    
    // Evento requerido por la interfaz
    public event PropertyChangedEventHandler? PropertyChanged;
    
    // Método helper para disparar el evento
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
```

**Atributo `[CallerMemberName]`:** El compilador automáticamente pasa el nombre de la propiedad que llamó al método.

### 2.3 Uso en XAML

```xml
<Window x:Class="EjemploReactividad.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Persona Editor" Height="200" Width="400">
    
    <StackPanel Margin="20">
        <TextBlock Text="Nombre:" />
        <TextBox Text="{Binding Nombre, UpdateSourceTrigger=PropertyChanged}" 
                 Margin="0,5,0,15" />
        
        <TextBlock Text="Edad:" />
        <Slider Minimum="0" Maximum="120" Value="{Binding Edad}" 
                Margin="0,5,0,15" />
        
        <TextBlock>
            <Run Text="Hola, me llamo " />
            <Run Text="{Binding Nombre}" FontWeight="Bold" />
            <Run Text=" y tengo " />
            <Run Text="{Binding Edad}" FontWeight="Bold" />
            <Run Text=" años." />
        </TextBlock>
    </StackPanel>
</Window>
```

```csharp
// Code-behind
namespace EjemploReactividad;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        // Establecer el DataContext
        DataContext = new Persona { Nombre = "Ana", Edad = 25 };
    }
}
```

**Resultado:** Al mover el slider o escribir en el TextBox, el texto inferior se actualiza automáticamente. ¡Reactividad en acción!

---

## 3. Mejora: Método SetProperty Genérico

Para evitar repetir el patrón en cada propiedad:

```csharp
public class ModeloBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    // Método genérico para establecer propiedad
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;
        
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
```

**Uso:**

```csharp
public class Persona : ModeloBase
{
    private string _nombre = "";
    private int _edad;
    
    public string Nombre
    {
        get => _nombre;
        set => SetProperty(ref _nombre, value);
    }
    
    public int Edad
    {
        get => _edad;
        set => SetProperty(ref _edad, value);
    }
}
```

**Ventajas:**

✅ Menos código boilerplate  
✅ Consistencia en la implementación  
✅ Evita errores (olvido de notificar)  

---

## 4. CommunityToolkit.Mvvm: La Forma Moderna

### 4.1 Instalación

```bash
dotnet add package CommunityToolkit.Mvvm
```

### 4.2 ObservableObject

La clase base `ObservableObject` ya implementa `INotifyPropertyChanged`:

```csharp
using CommunityToolkit.Mvvm.ComponentModel;

namespace ModeloModerno;

public partial class Persona : ObservableObject
{
    private string _nombre = "";
    private int _edad;
    
    public string Nombre
    {
        get => _nombre;
        set => SetProperty(ref _nombre, value);
    }
    
    public int Edad
    {
        get => _edad;
        set => SetProperty(ref _edad, value);
    }
}
```

### 4.3 Source Generators: [ObservableProperty]

Con **source generators** (C# 9+), el código se simplifica aún más:

```csharp
using CommunityToolkit.Mvvm.ComponentModel;

namespace ModeloModerno;

public partial class Persona : ObservableObject
{
    [ObservableProperty]
    private string _nombre = "";
    
    [ObservableProperty]
    private int _edad;
}
```

**¿Qué hace el generador?**

El compilador genera automáticamente las propiedades públicas:

```csharp
// Generado automáticamente:
public string Nombre
{
    get => _nombre;
    set => SetProperty(ref _nombre, value);
}

public int Edad
{
    get => _edad;
    set => SetProperty(ref _edad, value);
}
```

**Convención de nombres:**

- Campo privado: `_nombreCamelCase`
- Propiedad generada: `NombrePascalCase`

### 4.4 Propiedades Calculadas

```csharp
public partial class Persona : ObservableObject
{
    [ObservableProperty]
    private string _nombre = "";
    
    [ObservableProperty]
    private string _apellido = "";
    
    [ObservableProperty]
    private int _edad;
    
    // Propiedad calculada (se actualiza cuando Nombre o Apellido cambian)
    public string NombreCompleto => $"{Nombre} {Apellido}";
    
    // Notificar manualmente propiedades dependientes
    partial void OnNombreChanged(string value)
    {
        OnPropertyChanged(nameof(NombreCompleto));
    }
    
    partial void OnApellidoChanged(string value)
    {
        OnPropertyChanged(nameof(NombreCompleto));
    }
    
    // Propiedad calculada: es mayor de edad
    public bool EsMayorDeEdad => Edad >= 18;
    
    partial void OnEdadChanged(int value)
    {
        OnPropertyChanged(nameof(EsMayorDeEdad));
    }
}
```

**Uso en XAML:**

```xml
<StackPanel>
    <TextBlock>
        <Run Text="Nombre completo: " />
        <Run Text="{Binding NombreCompleto}" FontWeight="Bold" />
    </TextBlock>
    
    <TextBlock>
        <Run Text="Es mayor de edad: " />
        <Run Text="{Binding EsMayorDeEdad}" FontWeight="Bold" />
    </TextBlock>
</StackPanel>
```

### 4.5 Atributo [NotifyPropertyChangedFor]

Alternativa más concisa para propiedades dependientes:

```csharp
public partial class Persona : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NombreCompleto))]
    private string _nombre = "";
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NombreCompleto))]
    private string _apellido = "";
    
    public string NombreCompleto => $"{Nombre} {Apellido}";
}
```

---

## 5. ObservableCollection<T>: Colecciones Reactivas

### 5.1 El Problema con List<T>

```csharp
public class ViewModel : ObservableObject
{
    private List<string> _items = [];
    
    public List<string> Items
    {
        get => _items;
        set => SetProperty(ref _items, value);
    }
}

// En code-behind
viewModel.Items.Add("Nuevo item");
// ¡La UI NO se actualiza! Porque modificamos el contenido, no la referencia
```

### 5.2 ObservableCollection<T>

`ObservableCollection<T>` notifica automáticamente cuando se añaden, eliminan o modifican elementos:

```csharp
using System.Collections.ObjectModel;

public partial class TareasViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<string> _tareas = [];
    
    public TareasViewModel()
    {
        Tareas.Add("Comprar leche");
        Tareas.Add("Hacer ejercicio");
        Tareas.Add("Estudiar WPF");
    }
    
    public void AgregarTarea(string tarea)
    {
        Tareas.Add(tarea); // ¡La UI se actualiza automáticamente!
    }
}
```

**XAML con ListBox:**

```xml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="*" />
        <RowDefinition Height="Auto" />
    </Grid.RowDefinitions>
    
    <!-- Lista reactiva -->
    <ListBox Grid.Row="0" ItemsSource="{Binding Tareas}" />
    
    <!-- Añadir tarea -->
    <StackPanel Grid.Row="1" Orientation="Horizontal" Margin="10">
        <TextBox x:Name="txtNuevaTarea" Width="300" Margin="0,0,10,0" />
        <Button Content="Añadir" Click="BtnAnadir_Click" />
    </StackPanel>
</Grid>
```

```csharp
private void BtnAnadir_Click(object sender, RoutedEventArgs e)
{
    if (string.IsNullOrWhiteSpace(txtNuevaTarea.Text)) return;
    
    var viewModel = (TareasViewModel)DataContext;
    viewModel.AgregarTarea(txtNuevaTarea.Text);
    txtNuevaTarea.Clear();
}
```

### 5.3 ObservableCollection con Objetos Complejos

```csharp
public partial class Tarea : ObservableObject
{
    [ObservableProperty]
    private string _titulo = "";
    
    [ObservableProperty]
    private bool _completada;
    
    [ObservableProperty]
    private DateTime _fechaCreacion = DateTime.Now;
}

public partial class TareasViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Tarea> _tareas = [];
    
    public void AgregarTarea(string titulo)
    {
        Tareas.Add(new Tarea { Titulo = titulo });
    }
    
    public void EliminarTarea(Tarea tarea)
    {
        Tareas.Remove(tarea);
    }
    
    public void MarcarCompletada(Tarea tarea)
    {
        tarea.Completada = !tarea.Completada;
    }
}
```

**XAML con ItemTemplate:**

```xml
<ListBox ItemsSource="{Binding Tareas}">
    <ListBox.ItemTemplate>
        <DataTemplate>
            <StackPanel Orientation="Horizontal" Margin="5">
                <CheckBox IsChecked="{Binding Completada}" 
                          VerticalAlignment="Center" Margin="0,0,10,0" />
                <TextBlock Text="{Binding Titulo}" 
                           VerticalAlignment="Center" FontSize="14">
                    <TextBlock.Style>
                        <Style TargetType="TextBlock">
                            <Style.Triggers>
                                <DataTrigger Binding="{Binding Completada}" Value="True">
                                    <Setter Property="TextDecorations" Value="Strikethrough" />
                                    <Setter Property="Foreground" Value="Gray" />
                                </DataTrigger>
                            </Style.Triggers>
                        </Style>
                    </TextBlock.Style>
                </TextBlock>
                <TextBlock Text="{Binding FechaCreacion, StringFormat='{}{0:dd/MM/yyyy}'}" 
                           Foreground="Gray" Margin="10,0,0,0" 
                           VerticalAlignment="Center" FontSize="10" />
            </StackPanel>
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>
```

---

## 6. Ejemplo Completo: Lista de Contactos

### 6.1 Modelo

```csharp
using CommunityToolkit.Mvvm.ComponentModel;

namespace ContactosApp;

public partial class Contacto : ObservableObject
{
    [ObservableProperty]
    private string _nombre = "";
    
    [ObservableProperty]
    private string _email = "";
    
    [ObservableProperty]
    private string _telefono = "";
    
    [ObservableProperty]
    private bool _favorito;
}
```

### 6.2 ViewModel

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace ContactosApp;

public partial class ContactosViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Contacto> _contactos = [];
    
    [ObservableProperty]
    private Contacto? _contactoSeleccionado;
    
    public ContactosViewModel()
    {
        // Datos de ejemplo
        Contactos.Add(new Contacto
        {
            Nombre = "Ana García",
            Email = "ana@ejemplo.com",
            Telefono = "123456789",
            Favorito = true
        });
        
        Contactos.Add(new Contacto
        {
            Nombre = "Carlos López",
            Email = "carlos@ejemplo.com",
            Telefono = "987654321"
        });
    }
    
    public void AgregarContacto(string nombre, string email, string telefono)
    {
        Contactos.Add(new Contacto
        {
            Nombre = nombre,
            Email = email,
            Telefono = telefono
        });
    }
    
    public void EliminarContactoSeleccionado()
    {
        if (ContactoSeleccionado != null)
        {
            Contactos.Remove(ContactoSeleccionado);
            ContactoSeleccionado = null;
        }
    }
}
```

### 6.3 XAML

```xml
<Window x:Class="ContactosApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Gestor de Contactos" Height="500" Width="700">
    
    <Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="*" />
            <ColumnDefinition Width="2*" />
        </Grid.ColumnDefinitions>
        
        <!-- Lista de contactos -->
        <Grid Grid.Column="0" Margin="10">
            <Grid.RowDefinitions>
                <RowDefinition Height="Auto" />
                <RowDefinition Height="*" />
            </Grid.RowDefinitions>
            
            <TextBlock Text="Contactos" FontSize="18" FontWeight="Bold" 
                       Margin="0,0,0,10" />
            
            <ListBox Grid.Row="1" 
                     ItemsSource="{Binding Contactos}" 
                     SelectedItem="{Binding ContactoSeleccionado}">
                <ListBox.ItemTemplate>
                    <DataTemplate>
                        <StackPanel>
                            <TextBlock Text="{Binding Nombre}" FontWeight="Bold" />
                            <TextBlock Text="{Binding Email}" FontSize="10" 
                                       Foreground="Gray" />
                        </StackPanel>
                    </DataTemplate>
                </ListBox.ItemTemplate>
            </ListBox>
        </Grid>
        
        <!-- Detalles del contacto -->
        <Grid Grid.Column="1" Margin="10">
            <Grid.RowDefinitions>
                <RowDefinition Height="Auto" />
                <RowDefinition Height="*" />
                <RowDefinition Height="Auto" />
            </Grid.RowDefinitions>
            
            <TextBlock Text="Detalles" FontSize="18" FontWeight="Bold" 
                       Margin="0,0,0,20" />
            
            <StackPanel Grid.Row="1" DataContext="{Binding ContactoSeleccionado}">
                <TextBlock Text="Nombre:" FontWeight="Bold" />
                <TextBox Text="{Binding Nombre, UpdateSourceTrigger=PropertyChanged}" 
                         Margin="0,5,0,15" />
                
                <TextBlock Text="Email:" FontWeight="Bold" />
                <TextBox Text="{Binding Email, UpdateSourceTrigger=PropertyChanged}" 
                         Margin="0,5,0,15" />
                
                <TextBlock Text="Teléfono:" FontWeight="Bold" />
                <TextBox Text="{Binding Telefono, UpdateSourceTrigger=PropertyChanged}" 
                         Margin="0,5,0,15" />
                
                <CheckBox Content="Marcar como favorito" 
                          IsChecked="{Binding Favorito}" />
            </StackPanel>
            
            <StackPanel Grid.Row="2" Orientation="Horizontal" 
                        HorizontalAlignment="Right">
                <Button Content="Nuevo" Width="100" Height="35" 
                        Click="BtnNuevo_Click" Margin="0,0,10,0" />
                <Button Content="Eliminar" Width="100" Height="35" 
                        Click="BtnEliminar_Click" />
            </StackPanel>
        </Grid>
    </Grid>
</Window>
```

### 6.4 Code-Behind

```csharp
namespace ContactosApp;

public partial class MainWindow : Window
{
    private readonly ContactosViewModel _viewModel;
    
    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new ContactosViewModel();
        DataContext = _viewModel;
    }
    
    private void BtnNuevo_Click(object sender, RoutedEventArgs e)
    {
        // Diálogo simple para añadir contacto
        var dialogo = new NuevoContactoDialog();
        if (dialogo.ShowDialog() == true)
        {
            _viewModel.AgregarContacto(
                dialogo.Nombre,
                dialogo.Email,
                dialogo.Telefono
            );
        }
    }
    
    private void BtnEliminar_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.ContactoSeleccionado == null)
        {
            MessageBox.Show("Selecciona un contacto primero.", "Aviso",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        var resultado = MessageBox.Show(
            $"¿Eliminar el contacto '{_viewModel.ContactoSeleccionado.Nombre}'?",
            "Confirmar eliminación",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question
        );
        
        if (resultado == MessageBoxResult.Yes)
        {
            _viewModel.EliminarContactoSeleccionado();
        }
    }
}
```

---

## 7. Evolución del Código: Comparativa

### 7.1 Sin Reactividad (Imperativo)

```csharp
// ❌ Código antiguo y frágil
txtNombre.TextChanged += (s, e) =>
{
    lblSaludo.Content = $"Hola, {txtNombre.Text}!";
};
```

### 7.2 Con INotifyPropertyChanged Manual

```csharp
// ⚠️ Funcional pero verboso
public class Persona : INotifyPropertyChanged
{
    private string _nombre = "";
    
    public string Nombre
    {
        get => _nombre;
        set
        {
            if (_nombre != value)
            {
                _nombre = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Nombre)));
            }
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
}
```

### 7.3 Con CommunityToolkit.Mvvm

```csharp
// ✅ Moderno, conciso, mantenible
public partial class Persona : ObservableObject
{
    [ObservableProperty]
    private string _nombre = "";
}
```

---

## 8. Comparación: Manual vs. CommunityToolkit

| Aspecto | Manual | CommunityToolkit.Mvvm |
|---------|--------|----------------------|
| **Líneas de código** | ~15 por propiedad | ~2 por propiedad |
| **Propensión a errores** | Alta (olvidar notificar) | Baja (automatizado) |
| **Legibilidad** | Baja (boilerplate) | Alta (declarativo) |
| **Rendimiento** | Similar | Similar |
| **Soporte IDE** | Básico | Excelente (IntelliSense) |
| **Curva de aprendizaje** | Moderada | Baja (una vez entendido) |

---

## 9. UpdateSourceTrigger: Cuándo Notificar

En bindings de XAML, `UpdateSourceTrigger` controla **cuándo** se actualiza la fuente:

```xml
<!-- Por defecto: al perder foco -->
<TextBox Text="{Binding Nombre}" />

<!-- Al cambiar cada carácter -->
<TextBox Text="{Binding Nombre, UpdateSourceTrigger=PropertyChanged}" />

<!-- Explícitamente (raro) -->
<TextBox Text="{Binding Nombre, UpdateSourceTrigger=Explicit}" />

<!-- Al perder foco (explícito) -->
<TextBox Text="{Binding Nombre, UpdateSourceTrigger=LostFocus}" />
```

| Valor | Cuándo se actualiza |
|-------|---------------------|
| `Default` | Comportamiento por defecto del control |
| `PropertyChanged` | Con cada cambio (cada tecla en TextBox) |
| `LostFocus` | Al perder el foco |
| `Explicit` | Solo con `BindingExpression.UpdateSource()` |

---

## 10. Resumen

| Concepto | Descripción |
|----------|-------------|
| `INotifyPropertyChanged` | Interfaz para notificar cambios de propiedad |
| `PropertyChanged` | Evento que se dispara al cambiar una propiedad |
| `OnPropertyChanged()` | Método helper para disparar el evento |
| `ObservableObject` | Clase base de CommunityToolkit que implementa INPC |
| `[ObservableProperty]` | Atributo que genera propiedades reactivas |
| `ObservableCollection<T>` | Colección que notifica cambios automáticamente |
| `UpdateSourceTrigger` | Controla cuándo se sincroniza el binding |

---

## 11. Ejercicios Propuestos

1. **Calculadora de IMC**: Crea un modelo `Persona` con Peso y Altura. Añade una propiedad calculada `IMC` que se actualice automáticamente.

2. **Lista de Compras**: Implementa una lista de compras con `ObservableCollection`. Permite marcar productos como comprados y calcular el total.

3. **Conversor de Temperatura**: Modelo con temperatura en Celsius. Propiedades calculadas para Fahrenheit y Kelvin que se actualicen automáticamente.

4. **Temporizador**: Crea un ViewModel con una propiedad `Segundos` que se incremente cada segundo usando `DispatcherTimer`. La UI debe actualizarse automáticamente.

---

## 12. Referencias

- [INotifyPropertyChanged Interface](https://learn.microsoft.com/dotnet/api/system.componentmodel.inotifypropertychanged)
- [ObservableCollection](https://learn.microsoft.com/dotnet/api/system.collections.objectmodel.observablecollection-1)
- [CommunityToolkit.Mvvm Documentation](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/)
- [Data Binding Overview](https://learn.microsoft.com/dotnet/desktop/wpf/data/)

Ver ejemplos completos en `/soluciones/05-wpf-reactividad/`

---

*Documento elaborado para el módulo de Programación del ciclo formativo 1º DAW · Curso 2025-2026*
