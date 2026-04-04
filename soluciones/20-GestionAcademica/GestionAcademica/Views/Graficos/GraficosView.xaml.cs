using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using GestionAcademica.ViewModels.Graficos;
using ScottPlot;
using System.Linq;
using Serilog;

namespace GestionAcademica.Views.Graficos
{
    /// <summary>
    /// Vista de gráficos estadísticos con ScottPlot.
    /// </summary>
    public partial class GraficosView : Page
    {
        private readonly GraficosViewModel _viewModel;
        private readonly ILogger _logger = Log.ForContext<GraficosView>();

        /// <summary>
        /// Inicializa la vista de gráficos y configura el ViewModel correspondiente.
        /// </summary>
        public GraficosView()
        {
            InitializeComponent();
            _viewModel = App.ServiceProvider.GetRequiredService<GraficosViewModel>();
            DataContext = _viewModel;

            Loaded += OnLoaded;
        }

        /// <summary>
        /// Maneja el evento de carga completa de la página para inicializar los gráficos.
        /// </summary>
        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            InitializeCharts();
        }

        /// <summary>
        /// Inicializa los 4 gráficos de ScottPlot con datos reales.
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

            var barData = calificaciones.Select((v, i) => new ScottPlot.Bar
            {
                Position = i,
                Value = v,
                FillColor = ScottPlot.Color.FromHex("#667EEA"),
                LineWidth = 0
            }).ToArray();

            NotasChart.Plot.Add.Bars(barData);

            NotasChart.Plot.YLabel("Nota Media");
            NotasChart.Plot.XLabel("Ciclos");
            NotasChart.Plot.Title("Notas medias por ciclo");
            NotasChart.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
                Enumerable.Range(0, ciclosLabels.Length)
                    .Select(i => new ScottPlot.Tick((double)i, ciclosLabels[i]))
                    .ToArray()
            );
            NotasChart.Plot.Axes.Bottom.MajorTickStyle.Length = 0;
            NotasChart.Plot.Axes.Title.Label.FontSize = 16;
            NotasChart.Plot.Axes.Title.Label.Bold = true;
            NotasChart.Plot.Axes.Bottom.Label.FontSize = 12;
            NotasChart.Plot.Axes.Left.Label.FontSize = 12;
            NotasChart.Plot.Axes.Bottom.TickLabelStyle.FontSize = 10;
            NotasChart.Plot.Axes.Left.TickLabelStyle.FontSize = 10;
            NotasChart.Plot.Axes.Margins(bottom: 0, top: 0.05, left: 0.1, right: 0.1);
            NotasChart.Plot.Axes.Bottom.IsVisible = true;
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
                new() { Value = distribution[0], Label = $"Suspensos ({(int)distribution[0]})", FillColor = ScottPlot.Color.FromHex("#EF4444") },
                new() { Value = distribution[1], Label = $"Aprobados ({(int)distribution[1]})", FillColor = ScottPlot.Color.FromHex("#F59E0B") },
                new() { Value = distribution[2], Label = $"Notables ({(int)distribution[2]})",  FillColor = ScottPlot.Color.FromHex("#3B82F6") },
                new() { Value = distribution[3], Label = $"Sobresalientes ({(int)distribution[3]})", FillColor = ScottPlot.Color.FromHex("#10B981") }
            };

            var pie = DistributionChart.Plot.Add.Pie(slices);
            pie.DonutFraction = 0.5;
            pie.ExplodeFraction = 0.02;

            // Leyenda a la derecha con fuente grande y legible
            DistributionChart.Plot.Legend.IsVisible = true;
            DistributionChart.Plot.Legend.Alignment = ScottPlot.Alignment.MiddleRight;
            DistributionChart.Plot.Legend.FontSize = 14;

            // Título grande y en negrita
            DistributionChart.Plot.Title("Distribución de calificaciones");
            DistributionChart.Plot.Axes.Title.Label.FontSize = 18;
            DistributionChart.Plot.Axes.Title.Label.Bold = true;

            DistributionChart.Plot.Axes.Frameless();
            DistributionChart.Plot.HideGrid();

            // Margen derecho amplio para que la leyenda no solape el gráfico
            DistributionChart.Plot.Axes.Margins(bottom: 0.05, top: 0.05, left: 0.05, right: 0.35);

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
            var (estudiantesValues, estudiantesLabels) = _viewModel.GetEstudiantesCantidadPorCiclo();
            var (docentesValues, docentesLabels) = _viewModel.GetDocentesPorCiclo();

            CiclosChart.Plot.Clear();

            var positions = Enumerable.Range(0, estudiantesLabels.Length).Select(i => (double)i).ToArray();

            var estudiantesBars = positions.Select((pos, i) => new ScottPlot.Bar
            {
                Position = pos - 0.2,
                Value = estudiantesValues[i],
                FillColor = ScottPlot.Color.FromHex("#667EEA"),
                Label = "Estudiantes"
            }).ToArray();

            var docentesBars = positions.Select((pos, i) =>
            {
                var ciclo = estudiantesLabels[i];
                var idx = Array.IndexOf(docentesLabels, ciclo);
                return new ScottPlot.Bar
                {
                    Position = pos + 0.2,
                    Value = idx >= 0 ? docentesValues[idx] : 0,
                    FillColor = ScottPlot.Color.FromHex("#764BA2"),
                    Label = "Docentes"
                };
            }).ToArray();

            CiclosChart.Plot.Add.Bars(estudiantesBars);
            CiclosChart.Plot.Add.Bars(docentesBars);

            CiclosChart.Plot.YLabel("Cantidad");
            CiclosChart.Plot.XLabel("Ciclos");
            CiclosChart.Plot.Title("Personas por ciclo");

            CiclosChart.Plot.Legend.IsVisible = true;
            CiclosChart.Plot.Legend.Alignment = ScottPlot.Alignment.UpperRight;
            CiclosChart.Plot.Legend.FontSize = 11;

            CiclosChart.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
                positions.Select((pos, i) => new ScottPlot.Tick(pos, estudiantesLabels[i])).ToArray()
            );
            CiclosChart.Plot.Axes.Bottom.MajorTickStyle.Length = 0;
            CiclosChart.Plot.Axes.Title.Label.FontSize = 16;
            CiclosChart.Plot.Axes.Title.Label.Bold = true;
            CiclosChart.Plot.Axes.Bottom.Label.FontSize = 12;
            CiclosChart.Plot.Axes.Left.Label.FontSize = 12;
            CiclosChart.Plot.Axes.Bottom.TickLabelStyle.FontSize = 10;
            CiclosChart.Plot.Axes.Left.TickLabelStyle.FontSize = 10;
            CiclosChart.Plot.Axes.Margins(bottom: 0, top: 0.1, left: 0.1, right: 0.2);
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

            var barData = edadValues.Select((v, i) => new ScottPlot.Bar
            {
                Position = i,
                Value = v,
                FillColor = ScottPlot.Color.FromHex("#28A745"),
                LineWidth = 0
            }).ToArray();

            EdadChart.Plot.Add.Bars(barData);

            EdadChart.Plot.YLabel("Cantidad de estudiantes");
            EdadChart.Plot.Title("Distribución por edad");
            EdadChart.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
                Enumerable.Range(0, edadLabels.Length)
                    .Select(i => new ScottPlot.Tick((double)i, edadLabels[i]))
                    .ToArray()
            );
            EdadChart.Plot.Axes.Bottom.MajorTickStyle.Length = 0;
            EdadChart.Plot.Axes.Title.Label.FontSize = 16;
            EdadChart.Plot.Axes.Title.Label.Bold = true;
            EdadChart.Plot.Axes.Bottom.TickLabelStyle.FontSize = 10;
            EdadChart.Plot.Axes.Left.Label.FontSize = 12;
            EdadChart.Plot.Axes.Left.TickLabelStyle.FontSize = 10;
            EdadChart.Plot.Axes.Margins(bottom: 0, top: 0.1, left: 0.1, right: 0.1);
            EdadChart.Refresh();

            _logger.Information("✅ Gráfico de edad inicializado");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error en InitializeEdadChart");
        }
    }
}
}