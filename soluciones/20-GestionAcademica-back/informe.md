📋 INFORME FINAL - ANÁLISIS EN PROFUNDIDAD
🔴 PROBLEMA 1: FORMULARIOS CON SCROLLVIEW - ESPACIOS INSUFICIENTES
Confirmación del problema:
EstudiantesView.xaml (líneas 154-273):

XML
<Border Grid.Column="1" Width="450" ... >
    <ScrollViewer VerticalScrollBarVisibility="Auto"> <!-- línea 161 -->
DocentesView.xaml (líneas 114-143):

XML
<Border Grid.Column="1" Width="450" ... >
    <ScrollViewer VerticalScrollBarVisibility="Auto"> <!-- línea 117 -->
Análisis:
✅ Sí hay ScrollViewer
❌ Pero 450px es demasiado estrecho para 8+ campos
❌ El usuario tiene que hacer scroll vertical → mala UX
❌ El modelo Estudiante.cs tiene los campos (líneas en Persona.cs):
Nombre, Apellidos, DNI, Email, FechaNacimiento, Imagen
Calificacion, Ciclo, Curso
Pero en el formulario faltan Email y FechaNacimiento
SOLUCIÓN RECOMENDADA: VENTANAS MODALES
Ventajas:

✅ Espacio completo de pantalla (600-800px de ancho)
✅ Sin scroll necesario
✅ Enfoque total en la edición
✅ Separación clara de contextos (listado vs edición)
Implementación:

Crear EstudianteEditWindow.xaml:
XML
<Window x:Class="GestionAcademica.Views.Dialog.EstudianteEditWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="{Binding WindowTitle}"
        Width="700" Height="650"
        WindowStartupLocation="CenterOwner"
        ResizeMode="NoResize"
        ShowInTaskbar="False">
    <Grid Margin="30">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>
        
        <!-- Header con imagen -->
        <Border Grid.Row="0" Height="140" ...>
            <!-- Imagen circular grande -->
        </Border>
        
        <!-- Formulario SIN ScrollViewer -->
        <Grid Grid.Row="1" Margin="0,20,0,0">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="15"/>
                <ColumnDefinition Width="*"/>
            </Grid.ColumnDefinitions>
            
            <!-- Columna izquierda -->
            <StackPanel Grid.Column="0">
                <!-- Nombre, Apellidos, DNI, Email -->
            </StackPanel>
            
            <!-- Columna derecha -->
            <StackPanel Grid.Column="2">
                <!-- FechaNacimiento, Ciclo, Curso, Calificación -->
            </StackPanel>
        </Grid>
        
        <!-- Botones -->
        <StackPanel Grid.Row="2" Orientation="Horizontal" HorizontalAlignment="Right" Margin="0,20,0,0">
            <Button Content="Guardar" Command="{Binding SaveCommand}" MinWidth="120" Margin="0,0,10,0"/>
            <Button Content="Cancelar" Command="{Binding CancelCommand}" MinWidth="120"/>
        </StackPanel>
    </Grid>
</Window>
Abrir desde EstudiantesViewModel:
C#
[RelayCommand]
private void Edit()
{
    if (SelectedEstudiante == null) return;
    
    var editWindow = new EstudianteEditWindow();
    var editViewModel = new EstudianteEditViewModel(SelectedEstudiante, _personasService);
    editWindow.DataContext = editViewModel;
    editWindow.Owner = Application.Current.MainWindow;
    
    if (editWindow.ShowDialog() == true)
    {
        LoadEstudiantes(); // Recargar lista
    }
}
🔴 PROBLEMA 2: COLORES HARDCODEADOS - NO USA TEMA DEL SISTEMA
Evidencia CONFIRMADA:
GraficosView.xaml línea 6:

XML
Background="{DynamicResource BackgroundBrush}"
Pero ese recurso NO EXISTE en colores nativos, viene de DarkTheme.xaml

TODOS los demás usan colores hardcodeados:

XML
<!-- EstudiantesView.xaml línea 5 -->
Background="#1E1E1E"

<!-- DocentesView.xaml línea 5 -->
Background="#1E1E1E"
SOLUCIÓN: USAR COLORES NATIVOS DE WINDOWS 11
1. ELIMINAR Themes/DarkTheme.xaml y Themes/LightTheme.xaml

2. ELIMINAR de App.xaml línea 9:

XML
<ResourceDictionary Source="Themes/DarkTheme.xaml"/>
3. BUSCAR Y REEMPLAZAR en TODOS los archivos .xaml:

ANTES (Hardcoded)	DESPUÉS (Sistema Nativo)
Background="#1E1E1E"	Background="{DynamicResource {x:Static SystemColors.WindowBrushKey}}"
Foreground="White"	Foreground="{DynamicResource {x:Static SystemColors.WindowTextBrushKey}}"
BorderBrush="#3C3C3C"	BorderBrush="{DynamicResource {x:Static SystemColors.ActiveBorderBrushKey}}"
Background="#252525" (Cards)	Background="{DynamicResource {x:Static SystemColors.ControlBrushKey}}"
Background="#667EEA" (Botón primario)	Background="{DynamicResource {x:Static SystemColors.HotTrackBrushKey}}"
4. ELIMINAR el método ApplyTheme() de MainWindow.xaml.cs (líneas 176-203)

5. RESULTADO:

✅ La app respetará automáticamente el tema de Windows 11 (claro/oscuro)
✅ Los colores cambiarán según la configuración del usuario
✅ Aspecto nativo y profesional
✅ Sin mantenimiento de temas custom
🔴 PROBLEMA 3: DASHBOARD - CONTEO INCORRECTO (CRÍTICO)
Confirmación del bug en DashboardViewModel.cs línea 51:
C#
var todasPersonas = _personasService.GetAll().ToList();
El problema: GetAll() sin parámetros usa pageSize=10 por defecto (según IPersonasService.cs)

SOLUCIÓN: AGREGAR MÉTODOS DE CONTEO
1. Extender IPersonasService.cs:

C#
public interface IPersonasService
{
    // ... métodos existentes ...
    
    // NUEVOS MÉTODOS DE CONTEO
    int CountEstudiantes(bool includeDeleted = false);
    int CountDocentes(bool includeDeleted = false);
    int CountAprobados(double notaCorte = 5.0, bool includeDeleted = false);
    int CountSuspensos(double notaCorte = 5.0, bool includeDeleted = false);
    Dictionary<Ciclo, int> GetEstudiantesPorCiclo(bool includeDeleted = false);
    Dictionary<Ciclo, int> GetDocentesPorCiclo(bool includeDeleted = false);
}
2. Implementar en PersonasService.cs:

C#
public int CountEstudiantes(bool includeDeleted = false)
{
    return repository.GetEstudiantes(1, int.MaxValue, includeDeleted).Count();
}

public int CountDocentes(bool includeDeleted = false)
{
    return repository.GetDocentes(1, int.MaxValue, includeDeleted).Count();
}

public int CountAprobados(double notaCorte = 5.0, bool includeDeleted = false)
{
    return repository.GetEstudiantes(1, int.MaxValue, includeDeleted)
        .Count(e => e.Calificacion >= notaCorte);
}

public int CountSuspensos(double notaCorte = 5.0, bool includeDeleted = false)
{
    return repository.GetEstudiantes(1, int.MaxValue, includeDeleted)
        .Count(e => e.Calificacion < notaCorte);
}

public Dictionary<Ciclo, int> GetEstudiantesPorCiclo(bool includeDeleted = false)
{
    return repository.GetEstudiantes(1, int.MaxValue, includeDeleted)
        .GroupBy(e => e.Ciclo)
        .ToDictionary(g => g.Key, g => g.Count());
}
3. Actualizar DashboardViewModel.cs líneas 50-70:

C#
private void LoadStatistics()
{
    try
    {
        // USAR MÉTODOS DE CONTEO
        TotalEstudiantes = _personasService.CountEstudiantes(false);
        TotalDocentes = _personasService.CountDocentes(false);

        var notaCorte = GestionAcademica.Config.AppConfig.NotaAprobado;
        var aprobados = _personasService.CountAprobados(notaCorte, false);
        var suspensos = _personasService.CountSuspensos(notaCorte, false);

        if (TotalEstudiantes > 0)
        {
            PorcentajeAprobados = Math.Round((double)aprobados / TotalEstudiantes * 100, 1);
            PorcentajeSuspensos = Math.Round((double)suspensos / TotalEstudiantes * 100, 1);
        }

        var estudiantesPorCiclo = _personasService.GetEstudiantesPorCiclo(false);
        var docentesPorCiclo = _personasService.GetDocentesPorCiclo(false);

        TotalDAM = estudiantesPorCiclo.GetValueOrDefault(Ciclo.DAM) + docentesPorCiclo.GetValueOrDefault(Ciclo.DAM);
        TotalDAW = estudiantesPorCiclo.GetValueOrDefault(Ciclo.DAW) + docentesPorCiclo.GetValueOrDefault(Ciclo.DAW);
        TotalASIR = estudiantesPorCiclo.GetValueOrDefault(Ciclo.ASIR) + docentesPorCiclo.GetValueOrDefault(Ciclo.ASIR);

        MensajeEstado = "Datos cargados correctamente";
        _logger.Information("📊 Dashboard cargado correctamente");
    }
    catch (Exception ex)
    {
        _logger.Error(ex, "❌ Error al cargar estadísticas");
        MensajeEstado = "Error al cargar datos";
    }
}
🔴 PROBLEMA 4: GRÁFICOS NO SE VEN - NO HAY INICIALIZACIÓN
Confirmación del problema:
GraficosView.xaml líneas 60-61:

XML
<wpf:WpfPlot Grid.Column="0" x:Name="NotasChart" Margin="5"/>
<wpf:WpfPlot Grid.Column="1" x:Name="DistributionChart" Margin="5"/>
Pero GraficosView.xaml.cs solo tiene 17 líneas y NO inicializa ScottPlot:

C#
public GraficosView()
{
    InitializeComponent();
    _viewModel = App.ServiceProvider.GetRequiredService<GraficosViewModel>();
    DataContext = _viewModel;
}
// NO HAY CÓDIGO QUE LLAME A NotasChart.Plot.Add(...) !!!
SOLUCIÓN: INICIALIZAR GRÁFICOS EN CODE-BEHIND
Actualizar GraficosView.xaml.cs:

C#
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using GestionAcademica.ViewModels.Graficos;
using ScottPlot;
using ScottPlot.Plottables;

namespace GestionAcademica.Views.Graficos;

public partial class GraficosView : Page
{
    private readonly GraficosViewModel _viewModel;

    public GraficosView()
    {
        InitializeComponent();
        _viewModel = App.ServiceProvider.GetRequiredService<GraficosViewModel>();
        DataContext = _viewModel;
        
        Loaded += OnLoaded; // IMPORTANTE: esperar a que cargue
    }

    private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
    {
        InitializeCharts();
    }

    private void InitializeCharts()
    {
        // GRÁFICO 1: Notas por ciclo (Barras)
        var calificaciones = _viewModel.GetCalificacionesData();
        var ciclosLabels = _viewModel.GetCicloLabels();

        NotasChart.Plot.Clear();
        var barPlot = NotasChart.Plot.Add.Bars(calificaciones);
        NotasChart.Plot.XAxis.SetTicks(ciclosLabels);
        NotasChart.Plot.YLabel("Nota Media");
        NotasChart.Plot.XLabel("Ciclo");
        NotasChart.Plot.Title("Notas medias por ciclo");
        NotasChart.Refresh();

        // GRÁFICO 2: Distribución (Pie Chart)
        var distribution = _viewModel.GetNotasDistribution();
        string[] labels = { "Suspensos (<5)", "Aprobados (5-7)", "Notables (7-9)", "Sobresalientes (≥9)" };

        DistributionChart.Plot.Clear();
        var pie = DistributionChart.Plot.Add.Pie(distribution);
        pie.SliceLabels = labels;
        pie.ShowSliceLabels = true;
        DistributionChart.Plot.Title("Distribución de notas");
        DistributionChart.Plot.Axes.Frameless();
        DistributionChart.Refresh();
    }
}
Agregar 2 gráficos más en GraficosView.xaml:

XML
<!-- Después de la línea 62, agregar: -->
<Grid Grid.Row="2">
    <Grid.RowDefinitions>
        <RowDefinition Height="*"/>
        <RowDefinition Height="*"/>
    </Grid.RowDefinitions>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="*"/>
        <ColumnDefinition Width="*"/>
    </Grid.ColumnDefinitions>

    <wpf:WpfPlot Grid.Row="0" Grid.Column="0" x:Name="NotasChart" Margin="5"/>
    <wpf:WpfPlot Grid.Row="0" Grid.Column="1" x:Name="DistributionChart" Margin="5"/>
    <wpf:WpfPlot Grid.Row="1" Grid.Column="0" x:Name="CiclosChart" Margin="5"/>
    <wpf:WpfPlot Grid.Row="1" Grid.Column="1" x:Name="EdadChart" Margin="5"/>
</Grid>
Agregar método en GraficosViewModel.cs:

C#
public Dictionary<string, int> GetEstudiantesPorEdad()
{
    var estudiantes = _personasService.GetEstudiantesOrderBy(TipoOrdenamiento.Dni, 1, 1000, false).ToList();
    var hoy = DateTime.Now;
    
    return new Dictionary<string, int>
    {
        ["<18"] = estudiantes.Count(e => (hoy - e.FechaNacimiento).TotalDays / 365 < 18),
        ["18-25"] = estudiantes.Count(e => {
            var edad = (hoy - e.FechaNacimiento).TotalDays / 365;
            return edad >= 18 && edad < 25;
        }),
        [">25"] = estudiantes.Count(e => (hoy - e.FechaNacimiento).TotalDays / 365 >= 25)
    };
}
🔴 PROBLEMA 5: IMPORT/EXPORT - NO PERMITE ELEGIR FORMATO
Confirmación del problema:
ImportExportView.xaml NO tiene ComboBox para elegir formato

Revisando el archivo, probablemente tenga botones fijos para cada formato, pero debería tener:

SOLUCIÓN: AGREGAR SELECCIÓN DE FORMATO
XML
<Page x:Class="GestionAcademica.Views.ImportExport.ImportExportView"
      ...>
    <Grid Margin="30">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="*"/>
            <ColumnDefinition Width="30"/>
            <ColumnDefinition Width="*"/>
        </Grid.ColumnDefinitions>
        
        <!-- IMPORTAR -->
        <GroupBox Grid.Column="0" Header="📥 IMPORTAR">
            <StackPanel Margin="15">
                <TextBlock Text="Formato:" Margin="0,0,0,5"/>
                <ComboBox x:Name="ImportFormatCombo" SelectedIndex="0" Margin="0,0,0,15">
                    <ComboBoxItem Content="JSON (.json)"/>
                    <ComboBoxItem Content="CSV (.csv)"/>
                    <ComboBoxItem Content="Binary (.bin)"/>
                </ComboBox>
                
                <CheckBox Content="Eliminar datos antes de importar" 
                          IsChecked="{Binding DeleteBeforeImport}" 
                          Margin="0,0,0,15"/>
                
                <Button Content="Seleccionar archivo" Command="{Binding ImportCommand}" 
                        MinWidth="150" MinHeight="40"/>
            </StackPanel>
        </GroupBox>
        
        <!-- EXPORTAR -->
        <GroupBox Grid.Column="2" Header="📤 EXPORTAR">
            <StackPanel Margin="15">
                <TextBlock Text="Formato:" Margin="0,0,0,5"/>
                <ComboBox x:Name="ExportFormatCombo" SelectedIndex="0" Margin="0,0,0,15">
                    <ComboBoxItem Content="JSON (.json)"/>
                    <ComboBoxItem Content="CSV (.csv)"/>
                    <ComboBoxItem Content="Binary (.bin)"/>
                </ComboBox>
                
                <CheckBox Content="Incluir eliminados" 
                          IsChecked="{Binding IncludeDeleted}" 
                          Margin="0,0,0,15"/>
                
                <Button Content="Exportar" Command="{Binding ExportCommand}" 
                        MinWidth="150" MinHeight="40"/>
            </StackPanel>
        </GroupBox>
    </Grid>
</Page>
🔴 PROBLEMA 6: INFORMES - FORMATO LIMITADO
Confirmación:
InformesView.xaml líneas 35-51: ✅ SÍ tiene botones para PDF y HTML ✅ Separado por Estudiantes y Docentes

Pero falta:

Opción de "Ambos formatos"
Filtros (por ciclo, etc.)
MEJORA SUGERIDA:
XML
<!-- Agregar ComboBox de filtros antes del Grid de reportes -->
<StackPanel Grid.Row="1" Margin="0,0,0,20">
    <TextBlock Text="Filtrar por ciclo:" Margin="0,0,0,5"/>
    <ComboBox x:Name="FiltroCombo" SelectedIndex="0" MinWidth="200">
        <ComboBoxItem Content="Todos"/>
        <ComboBoxItem Content="DAM"/>
        <ComboBoxItem Content="DAW"/>
        <ComboBoxItem Content="ASIR"/>
    </ComboBox>
    
    <CheckBox Content="Mostrar eliminados" IsChecked="{Binding MostrarEliminados}" Margin="0,10,0,0"/>
</StackPanel>

<!-- En los GroupBox, agregar botón "Ambos" -->
<Button Content="Generar PDF + HTML" Command="{Binding GenerarAmbosEstudiantesCommand}" 
        Style="{DynamicResource SecondaryButton}" Margin="0,10,0,0"/>
🔴 PROBLEMA 7: VALIDACIÓN VISUAL AUSENTE
Confirmación:
Estudiante.cs y Docente.cs NO implementan IDataErrorInfo

C#
// Línea 10 de Estudiante.cs
public sealed record Estudiante : Persona, IEstudiar {
    // NO tiene IDataErrorInfo
}
SOLUCIÓN: IMPLEMENTAR VALIDACIÓN VISUAL
1. Actualizar Estudiante.cs:

C#
using System.ComponentModel;
using System.Text.RegularExpressions;

public sealed record Estudiante : Persona, IEstudiar, IDataErrorInfo
{
    public double Calificacion { get; init; }
    public Ciclo Ciclo { get; init; }
    public Curso Curso { get; init; }

    // Implementación de IDataErrorInfo
    public string Error => null!;

    public string this[string columnName]
    {
        get
        {
            return columnName switch
            {
                nameof(Nombre) when string.IsNullOrWhiteSpace(Nombre) 
                    => "El nombre es obligatorio",
                nameof(Nombre) when Nombre.Length < 2 || Nombre.Length > 30 
                    => "El nombre debe tener entre 2 y 30 caracteres",
                    
                nameof(Apellidos) when string.IsNullOrWhiteSpace(Apellidos) 
                    => "Los apellidos son obligatorios",
                nameof(Apellidos) when Apellidos.Length < 2 || Apellidos.Length > 50 
                    => "Los apellidos deben tener entre 2 y 50 caracteres",
                    
                nameof(Dni) when string.IsNullOrWhiteSpace(Dni) 
                    => "El DNI es obligatorio",
                nameof(Dni) when !Regex.IsMatch(Dni, @"^\d{8}[A-Z]$") 
                    => "El DNI debe tener 8 dígitos y una letra mayúscula",
                    
                nameof(Email) when string.IsNullOrWhiteSpace(Email) 
                    => "El email es obligatorio",
                nameof(Email) when !Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$") 
                    => "El formato del email es inválido",
                    
                nameof(FechaNacimiento) when FechaNacimiento.Year < 1900 
                    => "La fecha debe ser posterior a 1900",
                nameof(FechaNacimiento) when FechaNacimiento > DateTime.Now 
                    => "La fecha no puede ser futura",
                    
                nameof(Calificacion) when Calificacion < 0 || Calificacion > 10 
                    => "La nota debe estar entre 0 y 10",
                    
                _ => null!
            };
        }
    }
    
    // ... resto del código
}
2. Agregar ErrorTemplate en XAML (EstudiantesView.xaml):

XML
<TextBox Text="{Binding EditingEstudiante.Nombre, UpdateSourceTrigger=PropertyChanged, ValidatesOnDataErrors=True}" 
         Background="#1E1E1E" Foreground="White" Margin="0,0,0,15" Padding="10">
    <Validation.ErrorTemplate>
        <ControlTemplate>
            <StackPanel>
                <AdornedElementPlaceholder x:Name="placeholder"/>
                <TextBlock Text="{Binding [0].ErrorContent}" 
                           Foreground="Red" 
                           FontSize="11" 
                           Margin="0,2,0,0"/>
            </StackPanel>
        </ControlTemplate>
    </Validation.ErrorTemplate>
</TextBox>
3. Deshabilitar botón "Guardar" si hay errores:

XML
<Button Content="Guardar" Command="{Binding SaveCommand}" 
        IsEnabled="{Binding IsValid}" 
        MinWidth="120"/>
C#
// En EstudiantesViewModel
public bool IsValid => EditingEstudiante != null && 
                       string.IsNullOrEmpty(((IDataErrorInfo)EditingEstudiante)["Nombre"]) &&
                       string.IsNullOrEmpty(((IDataErrorInfo)EditingEstudiante)["Apellidos"]) &&
                       // ... validar todos los campos
                       true;
✅ PLAN DE ACCIÓN FINAL - PRIORIZADO
FASE 1: CRÍTICO - TEMAS Y COLORES NATIVOS (2-3 horas)
Eliminar Themes/DarkTheme.xaml y Themes/LightTheme.xaml
Eliminar referencia en App.xaml línea 9
Buscar y reemplazar colores hardcodeados en TODOS los .xaml
Eliminar método ApplyTheme() de MainWindow.xaml.cs
Probar en Windows 11 modo claro y oscuro
FASE 2: CRÍTICO - FORMULARIOS MODALES (4-5 horas)
Crear EstudianteEditWindow.xaml con diseño a 2 columnas
Crear DocenteEditWindow.xaml con diseño a 2 columnas
Agregar campos faltantes: Email, FechaNacimiento
Modificar ViewModels para abrir ventanas modales con ShowDialog()
Eliminar paneles laterales de EstudiantesView.xaml y DocentesView.xaml
FASE 3: CRÍTICO - CORREGIR DASHBOARD (1 hora)
Agregar métodos de conteo a IPersonasService
Implementar en PersonasService
Actualizar DashboardViewModel para usar los nuevos métodos
FASE 4: ALTO - ARREGLAR GRÁFICOS (3-4 horas)
Actualizar GraficosView.xaml.cs con inicialización de ScottPlot
Agregar 2 controles WpfPlot adicionales en XAML
Implementar método GetEstudiantesPorEdad() en ViewModel
Crear gráfico de ciclos y gráfico de edad
Probar que se visualicen correctamente
FASE 5: MEDIO - VALIDACIÓN VISUAL (3-4 horas)
Implementar IDataErrorInfo en Estudiante.cs
Implementar IDataErrorInfo en Docente.cs
Agregar ErrorTemplate en XAML de formularios
Agregar propiedad IsValid en ViewModels
Deshabilitar botón "Guardar" si hay errores
FASE 6: BAJO - MEJORAR IMPORT/EXPORT E INFORMES (2 horas)
Agregar ComboBox de formato en ImportExportView.xaml
Agregar filtros en InformesView.xaml
Agregar botón "Generar ambos formatos"
📊 RESUMEN EJECUTIVO
Problema	Gravedad	Tiempo	Estado
Temas hardcodeados	🔴 CRÍTICO	2-3h	Por hacer
Formularios con scroll	🔴 CRÍTICO	4-5h	Por hacer
Dashboard conteo incorrecto	🔴 CRÍTICO	1h	Por hacer
Gráficos no se ven	⚠️ ALTO	3-4h	Por hacer
Validación visual ausente	⚠️ MEDIO	3-4h	Por hacer
Import/Export limitado	ℹ️ BAJO	2h	Por hacer
Tiempo total estimado: 15-21 horas de trabajo