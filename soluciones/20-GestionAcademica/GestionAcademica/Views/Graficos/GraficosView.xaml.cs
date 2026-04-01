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
        _viewModel.Initialize();
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
            NotasChart.Plot.Title("Notas medias por ciclo");
            NotasChart.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
                Enumerable.Range(0, ciclosLabels.Length).Select(i => (double)i).ToArray(),
                ciclosLabels
            );
            NotasChart.Plot.Axes.Margins(bottom: 0);
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
            string[] labels = { "Suspensos\n(<5)", "Aprobados\n(5-7)", "Notables\n(7-9)", "Sobresalientes\n(≥9)" };

            DistributionChart.Plot.Clear();

            var pie = DistributionChart.Plot.Add.Pie(distribution);
            pie.ExplodeFraction = 0.05;

            for (int i = 0; i < pie.Slices.Count && i < labels.Length; i++)
            {
                pie.Slices[i].Label = labels[i];
                pie.Slices[i].LabelStyle.FontSize = 12;
                pie.Slices[i].LabelStyle.Bold = true;
            }

            // Colores personalizados
            if (pie.Slices.Count >= 4)
            {
                pie.Slices[0].FillColor = ScottPlot.Color.FromHex("#DC3545"); // Rojo para suspensos
                pie.Slices[1].FillColor = ScottPlot.Color.FromHex("#FFC107"); // Amarillo para aprobados
                pie.Slices[2].FillColor = ScottPlot.Color.FromHex("#17A2B8"); // Azul para notables
                pie.Slices[3].FillColor = ScottPlot.Color.FromHex("#28A745"); // Verde para sobresalientes
            }

            DistributionChart.Plot.Axes.Frameless();
            DistributionChart.Plot.HideGrid();
            DistributionChart.Refresh();

            _logger.Information("✅ Gráfico de distribución inicializado");
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
            var (docentesValues, docentesLabels) = _viewModel.GetDocentesPorCiclo();

            CiclosChart.Plot.Clear();

            // Crear barras de estudiantes
            var positions = Enumerable.Range(0, calificaciones.Length).Select(i => (double)i).ToArray();
            var estudiantesBars = positions.Select((pos, i) => new ScottPlot.Bar
            {
                Position = pos - 0.2,
                Value = calificaciones[i],
                FillColor = ScottPlot.Color.FromHex("#667EEA"),
                Label = "Estudiantes"
            }).ToList();

            // Crear barras de docentes
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
            CiclosChart.Plot.Title("Personas por ciclo");
            CiclosChart.Plot.ShowLegend(Alignment.UpperRight);
            CiclosChart.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
                positions,
                _viewModel.GetCicloLabels()
            );
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
            EdadChart.Plot.Axes.Margins(bottom: 0);
            EdadChart.Refresh();

            _logger.Information("✅ Gráfico de edad inicializado");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error en InitializeEdadChart");
        }
    }
}