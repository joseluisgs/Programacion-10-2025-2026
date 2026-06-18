# Ejercicios de Construcción: Interfaces Gráficas con .NET

- [Ejercicios de Construcción: Interfaces Gráficas con .NET](#ejercicios-de-construcción-interfaces-gráficas-con-net)
  - [Ejercicio 1: Calculadora con WPF](#ejercicio-1-calculadora-con-wpf)
  - [Ejercicio 2: Gestor de Tareas con MVVM](#ejercicio-2-gestor-de-tareas-con-mvvm)
  - [Ejercicio 3: Registro de Alumnos con WPF MVVM](#ejercicio-3-registro-de-alumnos-con-wpf-mvvm)
  - [Ejercicio 4: Gestión de Inventario con WPF MVC](#ejercicio-4-gestión-de-inventario-con-wpf-mvc)
  - [Ejercicio 5: Aplicación de Contactos con WPF y SQLite](#ejercicio-5-aplicación-de-contactos-con-wpf-y-sqlite)

---

## Ejercicio 1: Calculadora con WPF

Crea una calculadora básica usando **WPF** con **MVVM**.

### Requisitos

1. **UI:** Interface con botones numéricos (0-9), operaciones (+, -, *, /), clear y equals.
2. **Patrón MVVM:** Usa ViewModel con propiedades observable y comandos.
3. **Estilos:** Aplica estilos coherentes con triggers para estados de hover/pressed.
4. **Excepciones:** Maneja división por cero y overflow.

### Modelo

```csharp
public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string _display = "0";
    
    [ObservableProperty]
    private string _operacion = "";
    
    [ObservableProperty]
    private double _primerOperando;
    
    [ObservableProperty]
    private bool _nuevaOperacion = true;
    
    [RelayCommand]
    private void Numero(string numero) { /* ... */ }
    
    [RelayCommand]
    private void Operacion(string op) { /* ... */ }
    
    [RelayCommand]
    private void Calcular() { /* ... */ }
    
    [RelayCommand]
    private void Clear() { /* ... */ }
}
```

### Criterios de Evaluación

- ✅ La calculadora funciona correctamente
- ✅ Usa MVVM con CommunityToolkit.Mvvm
- ✅ Estilos con triggers
- ✅ Código limpio y separado

---

## Ejercicio 2: Gestor de Tareas con MVVM

Crea una aplicación WPF para gestionar tareas pendientes.

### Modelo de Datos

```csharp
public class Tarea
{
    public required int Id { get; init; }
    public required string Titulo { get; set; }
    public string? Descripcion { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public bool Completada { get; set; }
    public Prioridad Prioridad { get; set; }
}

public enum Prioridad { Baja, Media, Alta }
```

### Requisitos

1. **Lista de tareas:** Muestra todas las tareas con su estado (completada o no).
2. **CRUD completo:** Crear, leer, actualizar, eliminar tareas.
3. **Filtros:** Filtrar por: todas, pendientes, completadas.
4. **Ordenación:** Ordenar por fecha, prioridad o título.
5. **Persistencia:** Guarda en JSON usando System.Text.Json.
6. **MVVM:** Implementa con CommunityToolkit.Mvvm.

### UI Sugerida

- `ListBox` con `DataTemplate` para mostrar tareas
- `ComboBox` para filtros
- `DataGrid` o `ListView` alternativo
- Diálogos para crear/editar tareas

### Criterios de Evaluación

- ✅ CRUD completo funcional
- ✅ Filtros y ordenación
- ✅ Persistencia en JSON
- ✅ MVVM con bindings
- ✅ Estilos y temas

---

## Ejercicio 3: Registro de Alumnos con WPF MVVM

Crea una aplicación WPF usando el patrón **MVVM** para gestionar un registro de alumnos.

### Modelo de Datos

```csharp
public class Alumno
{
    public required int Id { get; init; }
    public required string Nombre { get; set; }
    public required string Apellidos { get; set; }
    public required string Email { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public string Curso { get; set; } = "";
}

public enum Curso { DAM1, DAM2, DAW1, DAW2, ASIR1, ASIR2 }
```

### Requisitos

1. **Listado:** Muestra los alumnos en un DataGrid.
2. **Alta:** Formulario para añadir nuevo alumno.
3. **Edición:** Selecciona un alumno para editar.
4. **Eliminación:** Elimina con confirmación.
5. **Validación:** Email obligatorio y formato válido.
6. **Persistencia:** Guarda en JSON.

### Estructura MVVM

```
ViewModels/
    MainViewModel.cs      - Lista de alumnos, comandos
    AlumnoEditorViewModel.cs - Formulario de edición
Models/
    Alumno.cs
Views/
    MainWindow.xaml
    AlumnoEditorDialog.xaml
Services/
    IAlumnoRepository.cs
    AlumnoRepository.cs
```

### Código Sugerido (ViewModel)

```csharp
public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Alumno> _alumnos = new();
    
    [ObservableProperty]
    private Alumno? _alumnoSeleccionado;
    
    [ObservableProperty]
    private string _filtro = "";
    
    public IRelayCommand NuevoCommand { get; }
    public IRelayCommand EditarCommand { get; }
    public IRelayCommand EliminarCommand { get; }
    
    public MainViewModel()
    {
        CargarAlumnos();
    }
    
    private void CargarAlumnos()
    {
        // Cargar desde JSON
    }
}
```

### Código XAML (DataGrid)

```xml
<DataGrid ItemsSource="{Binding Alumnos}" 
          SelectedItem="{Binding AlumnoSeleccionado}"
          AutoGenerateColumns="False">
    <DataGrid.Columns>
        <DataGridTextColumn Header="Nombre" Binding="{Binding Nombre}"/>
        <DataGridTextColumn Header="Apellidos" Binding="{Binding Apellidos}"/>
        <DataGridTextColumn Header="Email" Binding="{Binding Email}"/>
        <DataGridTextColumn Header="Curso" Binding="{Binding Curso}"/>
    </DataGrid.Columns>
</DataGrid>
```

### Criterios de Evaluación

- ✅ MVVM con CommunityToolkit.Mvvm
- ✅ DataGrid con bindings
- ✅ CRUD completo
- ✅ Validación
- ✅ Persistencia en JSON

---

## Ejercicio 4: Gestión de Inventario con WPF MVC

Crea una aplicación WPF usando el patrón **MVC** (code-behind).

### Modelo de Datos

```csharp
public class Producto
{
    public required int Id { get; init; }
    public required string Nombre { get; set; }
    public string Descripcion { get; set; } = "";
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string Categoria { get; set; } = "";
}
```

### Requisitos

1. **Ventana principal:** Muestra un DataGrid con el inventario.
2. **Alta de producto:** Botón para añadir nuevo producto.
3. **Edición:** Doble clic en fila para editar.
4. **Baja:** Botón para eliminar con confirmación.
5. **Búsqueda:** TextBox para buscar por nombre.
6. **Total:** Muestra el valor total del inventario.

### Estructura MVC

```
MainWindow.xaml     - Vista
MainWindow.xaml.cs  - Controlador (code-behind)
Producto.cs         - Modelo
Inventario.json     - Persistencia
```

### Código Sugerido (Code-Behind)

```csharp
public partial class MainWindow : Window
{
    private List<Producto> _productos = new();
    
    public MainWindow()
    {
        InitializeComponent();
        CargarProductos();
    }
    
    private void CargarProductos()
    {
        // Cargar desde JSON
        var json = File.ReadAllText("inventario.json");
        _productos = JsonSerializer.Deserialize<List<Producto>>(json);
        dgvProductos.ItemsSource = _productos;
    }
    
    private void btnNuevo_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new ProductoDialog();
        if (dialog.ShowDialog() == true)
        {
            _productos.Add(dialog.Producto);
            GuardarProductos();
        }
    }
}
```

### Criterios de Evaluación

- ✅ Uso de DataGrid
- ✅ CRUD con code-behind (MVC)
- ✅ Persistencia en JSON
- ✅ Búsqueda y filtros
- ✅ Layout con Grid/StackPanel

---

## Ejercicio 5: Gestión de Citas de ITV

Crea una aplicación WPF con **MVVM** para gestionar citas de una ITV.

### Modelo de Datos

```csharp
public class Cita
{
    public required int Id { get; init; }
    public required string Matricula { get; set; }
    public required string Propietario { get; set; }
    public required DateTime Fecha { get; set; }
    public string? Telefono { get; set; }
    public EstadoCita Estado { get; set; }
    public string? Observaciones { get; set; }
}

public enum EstadoCita { Pendiente, Completada, Cancelada }
```

### Requisitos

1. **Calendario:** Muestra las citas por fecha.
2. **Alta de cita:** Formulario con matrícula, propietario, fecha, teléfono.
3. **Estados:** Cambiar estado (Pendiente → Completada / Cancelada).
4. **Búsqueda:** Buscar por matrícula o propietario.
5. **Citas de hoy:** Resalta las citas del día actual.
6. **Persistencia:** Guarda en JSON.

### UI Sugerida

- `Calendar` o `DatePicker` para seleccionar fecha
- `DataGrid` para listado de citas
- `ComboBox` para estados
- `TextBox` para búsqueda

### Estructura MVVM

```
ViewModels/
    MainViewModel.cs
    CitaEditorViewModel.cs
Models/
    Cita.cs
Views/
    MainWindow.xaml
    CitaEditorDialog.xaml
```

### Código Sugerido

```csharp
public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Cita> _citas = new();
    
    [ObservableProperty]
    private ObservableCollection<Cita> _citasFiltradas = new();
    
    [ObservableProperty]
    private DateTime _fechaSeleccionada = DateTime.Today;
    
    [ObservableProperty]
    private string _busqueda = "";
    
    [RelayCommand]
    private void FiltrarPorFecha()
    {
        CitasFiltradas = new ObservableCollection<Cita>(
            Citas.Where(c => c.Fecha.Date == FechaSeleccionada.Date));
    }
    
    [RelayCommand]
    private void Buscar()
    {
        CitasFiltradas = new ObservableCollection<Cita>(
            Citas.Where(c => c.Matricula.Contains(Busqueda) || 
                           c.Propietario.Contains(Busqueda)));
    }
}
```

### Criterios de Evaluación

- ✅ CRUD completo de citas
- ✅ Filtrado por fecha
- ✅ Cambio de estados
- ✅ Búsqueda
- ✅ MVVM con CommunityToolkit.Mvvm

---

## Ejercicio 6 (Opcional): Aplicación de Contactos con WPF y SQLite

*(Ejercicio adicional para quien quiera profundizar en base de datos)*

Crea una aplicación WPF con **MVVM** que use **SQLite** como base de datos.

### Modelo de Datos

```csharp
public class Contacto
{
    public int Id { get; set; }
    public required string Nombre { get; set; }
    public required string Apellidos { get; set; }
    public string? Telefono { get; set; }
    public required string Email { get; set; }
    public string Categoria { get; set; } = "Trabajo";
}
```

### Requisitos

1. **Base de datos SQLite:** Crea la tabla Contactos.
2. **Listado de contactos:** Muestra en DataGrid.
3. **CRUD completo:** Alta, edición, eliminación.
4. **Búsqueda:** Filtro en tiempo real.
5. **Categorías:** Filtra por: Trabajo, Personal, Familia, Otros.

### Criterios de Evaluación

- ✅ SQLite con Dapper
- ✅ MVVM con CommunityToolkit.Mvvm
- ✅ CRUD completo
- ✅ Búsqueda en tiempo real
