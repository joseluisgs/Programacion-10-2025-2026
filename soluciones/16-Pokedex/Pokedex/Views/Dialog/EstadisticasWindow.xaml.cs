using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using Pokedex.Models;
using Pokedex.ViewModels;
using Serilog;

namespace Pokedex.Views.Dialog;

public partial class EstadisticasWindow : Window
{
    private readonly MainViewModel _vm;
    private readonly ILogger _logger = Log.ForContext<EstadisticasWindow>();

    public EstadisticasWindow(MainViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        
        _logger.Information("🔨 EstadisticasWindow Constructor - Pokemons: " + (_vm.TodosPokemons?.Count ?? 0));
        
        Loaded += OnLoaded;
    }

    private void ResetZoom_Click(object sender, RoutedEventArgs e)
    {
        ResetChartZoom(TiposChart);
        ResetChartZoom(GeneracionesChart);
        ResetChartZoom(PokemonStatsChart);
    }

    private void ResetChartZoom(LiveChartsCore.SkiaSharpView.WPF.CartesianChart chart)
    {
        if (chart?.XAxes != null)
        {
            foreach (var axis in chart.XAxes)
            {
                axis.MinLimit = null;
                axis.MaxLimit = null;
            }
        }
        if (chart?.YAxes != null)
        {
            foreach (var axis in chart.YAxes)
            {
                axis.MinLimit = null;
                axis.MaxLimit = null;
            }
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        InitializeCharts();
    }

    private void InitializeCharts()
    {
        try
        {
            var pokemons = _vm.TodosPokemons;
            if (pokemons == null || pokemons.Count == 0) return;

            InitializeTiposChart(pokemons);
            InitializeGeneracionesChart(pokemons);
            InitializePokemonStatsChart(pokemons);
            InitializeLegendariosChart(pokemons);
        }
        catch (Exception ex)
        {
            _logger.Information($"❌ Error: {ex.Message}");
        }
    }

    private void InitializeTiposChart(IReadOnlyList<Pokemon> pokemons)
    {
        var tipos = pokemons
            .SelectMany(p => p.Type)
            .GroupBy(t => t)
            .OrderByDescending(g => g.Count())
            .Take(10)
            .ToList();

        if (tipos.Count == 0) return;

        var labels = tipos.Select(t => t.Key).ToArray();
        var values = tipos.Select(t => (double)t.Count()).ToArray();

        TiposChart.Series = new ISeries[] {
            new ColumnSeries<double> {
                Values = values,
                Fill = new SolidColorPaint(SKColor.Parse("#3B82C6")),
                Name = "Cantidad"
            }
        };

        TiposChart.XAxes = new Axis[] {
            new Axis {
                Labels = labels,
                LabelsRotation = 20,
                TextSize = 12,
                LabelsPaint = new SolidColorPaint(SKColor.Parse("#CCCCCC"))
            }
        };

        TiposChart.YAxes = new Axis[] {
            new Axis {
                MinLimit = 0,
                TextSize = 12,
                LabelsPaint = new SolidColorPaint(SKColor.Parse("#CCCCCC")),
                Labeler = value => $"{value:F0}"
            }
        };
    }

    private void InitializeGeneracionesChart(IReadOnlyList<Pokemon> pokemons)
    {
        var generaciones = pokemons
            .GroupBy(p => p.Generation)
            .OrderBy(g => g.Key)
            .ToList();

        if (generaciones.Count == 0) return;

        var labels = generaciones.Select(g => ToRoman(g.Key)).ToArray();
        var values = generaciones.Select(g => (double)g.Count()).ToArray();

        GeneracionesChart.Series = new ISeries[] {
            new ColumnSeries<double> {
                Values = values,
                Fill = new SolidColorPaint(SKColor.Parse("#E3350C")),
                Name = "Cantidad"
            }
        };

        GeneracionesChart.XAxes = new Axis[] {
            new Axis {
                Labels = labels,
                TextSize = 14,
                LabelsPaint = new SolidColorPaint(SKColor.Parse("#CCCCCC"))
            }
        };

        GeneracionesChart.YAxes = new Axis[] {
            new Axis {
                MinLimit = 0,
                TextSize = 12,
                LabelsPaint = new SolidColorPaint(SKColor.Parse("#CCCCCC")),
                Labeler = value => $"{value:F0}"
            }
        };
    }

    private void InitializePokemonStatsChart(IReadOnlyList<Pokemon> pokemons)
    {
        var top10 = pokemons
            .Select(p => new { Name = p.Name, Total = p.Base.HP + p.Base.Attack + p.Base.Defense + p.Base.SpAttack + p.Base.SpDefense + p.Base.Speed })
            .OrderByDescending(p => p.Total)
            .Take(10)
            .ToList();

        var labels = top10.Select(p => p.Name).ToArray();
        var values = top10.Select(p => (double)p.Total).ToArray();

        PokemonStatsChart.Series = new ISeries[] {
            new ColumnSeries<double> {
                Values = values,
                Fill = new SolidColorPaint(SKColor.Parse("#FFD700")),
                Name = "Stats Totales"
            }
        };

        PokemonStatsChart.XAxes = new Axis[] {
            new Axis {
                Labels = labels,
                LabelsRotation = 30,
                TextSize = 10,
                LabelsPaint = new SolidColorPaint(SKColor.Parse("#CCCCCC"))
            }
        };

        PokemonStatsChart.YAxes = new Axis[] {
            new Axis {
                MinLimit = 0,
                TextSize = 12,
                LabelsPaint = new SolidColorPaint(SKColor.Parse("#CCCCCC")),
                Labeler = value => $"{value:F0}"
            }
        };
    }

    private void InitializeLegendariosChart(IReadOnlyList<Pokemon> pokemons)
    {
        var legendarios = pokemons.Count(p => p.IsLegendary);
        var miticos = pokemons.Count(p => p.IsMythical);
        var normales = pokemons.Count(p => !p.IsLegendary && !p.IsMythical);
        var total = normales + legendarios + miticos;

        var pctNormal = total > 0 ? normales * 100.0 / total : 0;
        var pctLegendario = total > 0 ? legendarios * 100.0 / total : 0;
        var pctMitico = total > 0 ? miticos * 100.0 / total : 0;

        LegendariosChart.Series = new ISeries[] {
            new PieSeries<double> {
                Values = new double[] { normales },
                Name = $"Normal: {normales} ({pctNormal:F1}%)",
                Fill = new SolidColorPaint(SKColor.Parse("#3B82C6"))
            },
            new PieSeries<double> {
                Values = new double[] { legendarios },
                Name = $"Legendario: {legendarios} ({pctLegendario:F1}%)",
                Fill = new SolidColorPaint(SKColor.Parse("#FFD700"))
            },
            new PieSeries<double> {
                Values = new double[] { miticos },
                Name = $"Mítico: {miticos} ({pctMitico:F1}%)",
                Fill = new SolidColorPaint(SKColor.Parse("#9C27B0"))
            }
        };

        LegendariosChart.InitialRotation = -90;
    }

    private static string ToRoman(string generation)
    {
        var match = System.Text.RegularExpressions.Regex.Match(generation, @"\d+");
        if (match.Success && int.TryParse(match.Value, out int genNum))
        {
            return genNum switch
            {
                1 => "I", 2 => "II", 3 => "III", 4 => "IV", 5 => "V",
                6 => "VI", 7 => "VII", 8 => "VIII", 9 => "IX", 10 => "X",
                _ => genNum.ToString()
            };
        }
        return generation.ToUpper().Replace("GEN ", "");
    }
}