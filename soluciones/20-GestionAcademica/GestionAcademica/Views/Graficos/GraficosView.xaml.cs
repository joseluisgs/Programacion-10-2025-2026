using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using GestionAcademica.ViewModels.Graficos;
using ScottPlot;
using System.Linq;
using Serilog;

namespace GestionAcademica.Views.Graficos;

/// <summary>
/// Vista de gráficos estadísticos con ScottPlot
/// </summary>
public partial class GraficosView : Page
{
    private readonly GraficosViewModel _viewModel;
    private readonly ILogger _logger = Log.ForContext<GraficosView>();

    public GraficosView()
    {
        InitializeComponent();
        _viewModel = App.ServiceProvider.GetRequiredService<GraficosViewModel>();
        DataContext = _viewModel;

        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
    {
        InitializeCharts();
    }

    /// <summary>
    /// Inicializa los 4 gráficos de ScottPlot con datos reales
    /// </summary>
    private void InitializeCharts()
    {
        try
        {
            _logger.Information("📊 Inicializando gráficos de ScottPlot...");

            // GRÁFICO 1: Notas medias por ciclo (Barras verticales)
            InitializeNotasChart();

            // GRÁFICO 2: Distribución de calificaciones (Pie Chart)
            InitializeDistributionChart();

            // GRÁFICO 3: Personas por ciclo (Barras agrupadas)
            InitializeCiclosChart();

            // GRÁFICO 4: Estudiantes por edad (Barras horizontales)
            InitializeEdadChart();

            _logger.Information("✅ Todos los gráficos inicializados correctamente");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "❌ Error al inicializar gráficos");
        }
    }

    private void InitializeNotasChart()
    {
        try
        {
            var calificaciones = _viewModel.GetCalificacionesData();
            var ciclosLabels = _viewModel.GetCicloLabels();

            NotasChart.Plot.Clear();

            var bars = NotasChart.Plot.Add.Bars(calificaciones);
            foreach (var bar in bars.Bars)
            {
                bar.FillColor = ScottPlot.Color.FromHex("#667EEA");
                bar.LineWidth = 0;
            }

            NotasChart.Plot.YLabel("Nota Media");
            NotasChart.Plot.XLabel("Ciclos");
            NotasChart.Plot.Title("Notas medias por ciclo");
            NotasChart.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
                Enumerable.Range(0, ciclosLabels.Length).Select(i => (double)i).ToArray(),
                ciclosLabels
            );
            NotasChart.Plot.Axes.Bottom.MajorTickStyle.Length = 0;
            NotasChart.Plot.Axes.Title.Label.FontSize = 16;
            NotasChart.Plot.Axes.Title.Label.Bold = true;
            NotasChart.Plot.Axes.Bottom.Label.FontSize = 12;
            NotasChart.Plot.Axes.Left.Label.FontSize = 12;
            NotasChart.Plot.Axes.Bottom.TickLabelStyle.FontSize = 10;
            NotasChart.Plot.Axes.Left.TickLabelStyle.FontSize = 10;
            NotasChart.Plot.Axes.Margins(bottom: 0.05, top: 0.2, left: 0.1, right: 0.1);
            NotasChart.Refresh();

            _logger.Information("✅ Gráfico de notas inicializado");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error en InitializeNotasChart");
        }
    }

    private void InitializeDistributionChart()
    {
        try
        {
            var distribution = _viewModel.GetNotasDistribution();

            DistributionChart.Plot.Clear();

            // Crear slices con colores y etiquetas
            var slices = new List<ScottPlot.PieSlice>
            {
                new() { Value = distribution[0], Label = $"Suspensos ({(int)distribution[0]})", FillColor = ScottPlot.Color.FromHex("#DC3545") },
                new() { Value = distribution[1], Label = $"Aprobados ({(int)distribution[1]})", FillColor = ScottPlot.Color.FromHex("#FFC107") },
                new() { Value = distribution[2], Label = $"Notables ({(int)distribution[2]})",  FillColor = ScottPlot.Color.FromHex("#17A2B8") },
                new() { Value = distribution[3], Label = $"Sobresalientes ({(int)distribution[3]})", FillColor = ScottPlot.Color.FromHex("#28A745") }
            };

            var pie = DistributionChart.Plot.Add.Pie(slices);
            pie.ExplodeFraction = 0.05;
            pie.DonutFraction = 0.5; // Convertir en doughnut chart

            // Leyenda fuera del gráfico
            DistributionChart.Plot.Legend.IsVisible = true;
            DistributionChart.Plot.Legend.Alignment = ScottPlot.Alignment.UpperRight;
            DistributionChart.Plot.Legend.FontSize = 11;

            // Título
            DistributionChart.Plot.Title("Distribución de calificaciones");
            DistributionChart.Plot.Axes.Title.Label.FontSize = 16;
            DistributionChart.Plot.Axes.Title.Label.Bold = true;

            DistributionChart.Plot.Axes.Frameless();
            DistributionChart.Plot.HideGrid();
            DistributionChart.Refresh();

            _logger.Information("✅ Gráfico de distribución (doughnut) inicializado");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error en InitializeDistributionChart");
        }
    }

    private void InitializeCiclosChart()
    {
        try
        {
            var calificaciones = _viewModel.GetCalificacionesData();
            var (docentesValues, _) = _viewModel.GetDocentesPorCiclo();

            CiclosChart.Plot.Clear();

            // Crear barras agrupadas (estudiantes y docentes por ciclo)
            var positions = Enumerable.Range(0, calificaciones.Length).Select(i => (double)i).ToArray();
            var estudiantesBars = positions.Select((pos, i) => new ScottPlot.Bar
            {
                Position = pos - 0.2,
                Value = calificaciones[i],
                FillColor = ScottPlot.Color.FromHex("#667EEA"),
                Label = "Estudiantes"
            }).ToList();

            var docentesBars = positions.Select((pos, i) => new ScottPlot.Bar
            {
                Position = pos + 0.2,
                Value = i < docentesValues.Length ? docentesValues[i] : 0,
                FillColor = ScottPlot.Color.FromHex("#764BA2"),
                Label = "Docentes"
            }).ToList();

            CiclosChart.Plot.Add.Bars(estudiantesBars.ToArray());
            CiclosChart.Plot.Add.Bars(docentesBars.ToArray());

            CiclosChart.Plot.YLabel("Cantidad");
            CiclosChart.Plot.XLabel("Ciclos");
            CiclosChart.Plot.Title("Personas por ciclo");

            // Leyenda fuera del gráfico
            CiclosChart.Plot.Legend.IsVisible = true;
            CiclosChart.Plot.Legend.Alignment = ScottPlot.Alignment.UpperRight;
            CiclosChart.Plot.Legend.FontSize = 11;

            CiclosChart.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
                positions,
                _viewModel.GetCicloLabels()
            );
            CiclosChart.Plot.Axes.Bottom.MajorTickStyle.Length = 0;
            CiclosChart.Plot.Axes.Title.Label.FontSize = 16;
            CiclosChart.Plot.Axes.Title.Label.Bold = true;
            CiclosChart.Plot.Axes.Bottom.Label.FontSize = 12;
            CiclosChart.Plot.Axes.Left.Label.FontSize = 12;
            CiclosChart.Plot.Axes.Bottom.TickLabelStyle.FontSize = 10;
            CiclosChart.Plot.Axes.Left.TickLabelStyle.FontSize = 10;
            CiclosChart.Plot.Axes.Margins(bottom: 0.05, top: 0.2, left: 0.1, right: 0.2);
            CiclosChart.Refresh();

            _logger.Information("✅ Gráfico de ciclos inicializado");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error en InitializeCiclosChart");
        }
    }

    private void InitializeEdadChart()
    {
        try
        {
            var edadData = _viewModel.GetEstudiantesPorEdad();
            var edadValues = edadData.Values.Select(v => (double)v).ToArray();
            var edadLabels = edadData.Keys.ToArray();

            EdadChart.Plot.Clear();

            var bars = EdadChart.Plot.Add.Bars(edadValues);
            foreach (var bar in bars.Bars)
            {
                bar.FillColor = ScottPlot.Color.FromHex("#28A745");
                bar.LineWidth = 0;
            }

            EdadChart.Plot.YLabel("Cantidad de estudiantes");
            EdadChart.Plot.Title("Distribución por edad");
            EdadChart.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
                Enumerable.Range(0, edadLabels.Length).Select(i => (double)i).ToArray(),
                edadLabels
            );
            EdadChart.Plot.Axes.Bottom.MajorTickStyle.Length = 0;
            EdadChart.Plot.Axes.Title.Label.FontSize = 16;
            EdadChart.Plot.Axes.Title.Label.Bold = true;
            EdadChart.Plot.Axes.Bottom.TickLabelStyle.FontSize = 10;
            EdadChart.Plot.Axes.Left.Label.FontSize = 12;
            EdadChart.Plot.Axes.Left.TickLabelStyle.FontSize = 10;
            EdadChart.Plot.Axes.Margins(bottom: 0.05, top: 0.2, left: 0.1, right: 0.1);
            EdadChart.Refresh();

            _logger.Information("✅ Gráfico de edad inicializado");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error en InitializeEdadChart");
        }
    }
}