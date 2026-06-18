using System.Windows;
using System.Windows.Controls;
using GestionAcademica.ViewModels.Graficos;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SkiaSharp;

namespace GestionAcademica.Views.Graficos;

public partial class GraficosView : Page {
    private readonly ILogger _logger = Log.ForContext<GraficosView>();
    private readonly GraficosViewModel _viewModel;

    public GraficosView() {
        InitializeComponent();
        _viewModel = App.ServiceProvider.GetRequiredService<GraficosViewModel>();
        DataContext = _viewModel;

        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e) {
        InitializeCharts();
    }

    private void InitializeCharts() {
        try {
            _logger.Information("📊 Inicializando gráficos con LiveCharts2...");

            InitializeNotasChart();
            InitializeDistributionChart();
            InitializeCiclosChart();
            InitializeEdadChart();
            InitializeTasaAprobadosChart();
            InitializeCursosChart();

            _logger.Information("✅ Todos los gráficos inicializados correctamente");
        }
        catch (Exception ex) {
            _logger.Error(ex, "❌ Error al inicializar gráficos");
        }
    }

    private void InitializeNotasChart() {
        try {
            var calificaciones = _viewModel.GetCalificacionesData();
            var ciclosLabels = _viewModel.GetCicloLabels();

            NotasChart.Series = new ISeries[] {
                new ColumnSeries<double> {
                    Values = calificaciones,
                    Fill = new SolidColorPaint(SKColor.Parse("#667EEA")),
                    Name = "Nota Media"
                }
            };

            NotasChart.XAxes = new Axis[] {
                new Axis {
                    Labels = ciclosLabels,
                    LabelsRotation = 0,
                    TextSize = 12,
                    LabelsPaint = new SolidColorPaint(SKColor.Parse("#333333")),
                    SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#E5E5E5")) { StrokeThickness = 1 }
                }
            };

            NotasChart.YAxes = new Axis[] {
                new Axis {
                    Name = "Nota Media",
                    NameTextSize = 14,
                    TextSize = 12,
                    LabelsPaint = new SolidColorPaint(SKColor.Parse("#333333")),
                    SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#E5E5E5")) { StrokeThickness = 1 },
                    Labeler = value => $"{value:F2}"
                }
            };

            _logger.Information("✅ Gráfico de notas inicializado");
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error en InitializeNotasChart");
        }
    }

    private void InitializeDistributionChart() {
        try {
            var distribution = _viewModel.GetNotasDistribution();
            var total = distribution.Sum();
            var labels = new[] { "Suspensos", "Aprobados", "Notables", "Sobresalientes" };
            var labelData = distribution.Select((v, i) => {
                var pct = total > 0 ? v / total * 100 : 0;
                return $"{labels[i]}: {(int)v} ({pct:F1}%)";
            }).ToArray();

            DistributionChart.Series = new ISeries[] {
                new PieSeries<double> {
                    Values = new[] { distribution[0] },
                    Name = labelData[0],
                    Fill = new SolidColorPaint(SKColor.Parse("#EF4444"))
                },
                new PieSeries<double> {
                    Values = new[] { distribution[1] },
                    Name = labelData[1],
                    Fill = new SolidColorPaint(SKColor.Parse("#F59E0B"))
                },
                new PieSeries<double> {
                    Values = new[] { distribution[2] },
                    Name = labelData[2],
                    Fill = new SolidColorPaint(SKColor.Parse("#3B82F6"))
                },
                new PieSeries<double> {
                    Values = new[] { distribution[3] },
                    Name = labelData[3],
                    Fill = new SolidColorPaint(SKColor.Parse("#10B981"))
                }
            };

            _logger.Information("✅ Gráfico de distribución (doughnut) inicializado");
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error en InitializeDistributionChart");
        }
    }

    private void InitializeCiclosChart() {
        try {
            var (estudiantesValues, estudiantesLabels) = _viewModel.GetEstudiantesCantidadPorCiclo();
            var (docentesValues, docentesLabels) = _viewModel.GetDocentesPorCiclo();

            var ciclos = estudiantesLabels.ToList();
            var estudiantes = new double[ciclos.Count];
            var docentes = new double[ciclos.Count];

            for (int i = 0; i < ciclos.Count; i++) {
                estudiantes[i] = estudiantesValues[i];
                var idx = Array.IndexOf(docentesLabels, ciclos[i]);
                docentes[i] = idx >= 0 ? docentesValues[idx] : 0;
            }

            CiclosChart.Series = new ISeries[] {
                new ColumnSeries<double> {
                    Values = estudiantes,
                    Name = "Estudiantes",
                    Fill = new SolidColorPaint(SKColor.Parse("#667EEA"))
                },
                new ColumnSeries<double> {
                    Values = docentes,
                    Name = "Docentes",
                    Fill = new SolidColorPaint(SKColor.Parse("#764BA2"))
                }
            };

            CiclosChart.XAxes = new Axis[] {
                new Axis {
                    Labels = ciclos,
                    LabelsRotation = 0,
                    TextSize = 12,
                    LabelsPaint = new SolidColorPaint(SKColor.Parse("#333333")),
                    SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#E5E5E5")) { StrokeThickness = 1 }
                }
            };

            CiclosChart.YAxes = new Axis[] {
                new Axis {
                    Name = "Cantidad",
                    NameTextSize = 14,
                    TextSize = 12,
                    LabelsPaint = new SolidColorPaint(SKColor.Parse("#333333")),
                    SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#E5E5E5")) { StrokeThickness = 1 },
                    Labeler = value => $"{value:F0}"
                }
            };

            _logger.Information("✅ Gráfico de ciclos inicializado");
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error en InitializeCiclosChart");
        }
    }

    private void InitializeEdadChart() {
        try {
            var edadData = _viewModel.GetEstudiantesPorEdad();
            var edadValues = edadData.Values.Select(v => (double)v).ToArray();
            var edadLabels = edadData.Keys.ToArray();

            EdadChart.Series = new ISeries[] {
                new ColumnSeries<double> {
                    Values = edadValues,
                    Fill = new SolidColorPaint(SKColor.Parse("#28A745")),
                    Name = "Estudiantes"
                }
            };

            EdadChart.XAxes = new Axis[] {
                new Axis {
                    Labels = edadLabels,
                    LabelsRotation = 0,
                    TextSize = 10,
                    LabelsPaint = new SolidColorPaint(SKColor.Parse("#333333")),
                    SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#E5E5E5")) { StrokeThickness = 1 }
                }
            };

            EdadChart.YAxes = new Axis[] {
                new Axis {
                    Name = "Cantidad de estudiantes",
                    NameTextSize = 14,
                    TextSize = 12,
                    LabelsPaint = new SolidColorPaint(SKColor.Parse("#333333")),
                    SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#E5E5E5")) { StrokeThickness = 1 },
                    Labeler = value => $"{value:F0}"
                }
            };

            _logger.Information("✅ Gráfico de edad inicializado");
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error en InitializeEdadChart");
        }
    }

    private void InitializeTasaAprobadosChart() {
        try {
            var (aprobados, suspensos, ciclos) = _viewModel.GetTasaAprobadosPorCiclo();

            TasaAprobadosChart.Series = new ISeries[] {
                new StackedColumnSeries<double> {
                    Values = aprobados,
                    Name = "Aprobados %",
                    Fill = new SolidColorPaint(SKColor.Parse("#10B981")),
                    MaxBarWidth = 50
                },
                new StackedColumnSeries<double> {
                    Values = suspensos,
                    Name = "Suspensos %",
                    Fill = new SolidColorPaint(SKColor.Parse("#EF4444")),
                    MaxBarWidth = 50
                }
            };

            TasaAprobadosChart.XAxes = new Axis[] {
                new Axis {
                    Labels = ciclos,
                    LabelsRotation = 0,
                    TextSize = 12,
                    LabelsPaint = new SolidColorPaint(SKColor.Parse("#333333")),
                    SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#E5E5E5")) { StrokeThickness = 1 }
                }
            };

            TasaAprobadosChart.YAxes = new Axis[] {
                new Axis {
                    Name = "Porcentaje",
                    NameTextSize = 14,
                    MinLimit = 0,
                    MaxLimit = 100,
                    TextSize = 12,
                    LabelsPaint = new SolidColorPaint(SKColor.Parse("#333333")),
                    SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#E5E5E5")) { StrokeThickness = 1 },
                    Labeler = value => $"{value:F2}%"
                }
            };

            _logger.Information("✅ Gráfico de tasa de aprobados inicializado");
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error en InitializeTasaAprobadosChart");
        }
    }

    private void InitializeCursosChart() {
        try {
            var (values, labels) = _viewModel.GetExperienciaMediaDocentes();

            CursosChart.Series = new ISeries[] {
                new ColumnSeries<double> {
                    Values = values,
                    Name = "Experiencia Media (años)",
                    Fill = new SolidColorPaint(SKColor.Parse("#F59E0B")),
                    MaxBarWidth = 50
                }
            };

            CursosChart.XAxes = new Axis[] {
                new Axis {
                    Labels = labels,
                    TextSize = 12,
                    LabelsPaint = new SolidColorPaint(SKColor.Parse("#333333")),
                    SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#E5E5E5")) { StrokeThickness = 1 }
                }
            };

            CursosChart.YAxes = new Axis[] {
                new Axis {
                    Name = "Años de experiencia",
                    NameTextSize = 14,
                    MinLimit = 0,
                    TextSize = 12,
                    LabelsPaint = new SolidColorPaint(SKColor.Parse("#333333")),
                    SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#E5E5E5")) { StrokeThickness = 1 },
                    Labeler = value => $"{value:F1}"
                }
            };

            _logger.Information("✅ Gráfico de experiencia media docentes inicializado");
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error en InitializeCursosChart");
        }
    }
}