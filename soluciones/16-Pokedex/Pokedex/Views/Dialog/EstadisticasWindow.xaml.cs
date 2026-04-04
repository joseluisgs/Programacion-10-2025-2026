using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ScottPlot;
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
        ContentRendered += (s, e) => _logger.Information("🔔 ContentRendered fired");
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _logger.Information("🔔 OnLoaded called");
        InitializeCharts();
    }

    private void InitializeCharts()
    {
        try
        {
            _logger.Information("🔄 InitializeCharts START");

            var pokemons = _vm.TodosPokemons;
            _logger.Information($"📋 _vm.TodosPokemons is null: {pokemons == null}");

            if (pokemons == null || pokemons.Count == 0)
            {
                _logger.Information("⚠️ No pokemons data available");
                return;
            }

            _logger.Information($"✅ Found {pokemons.Count} pokemons");

            // Debug: ver primeros 3 pokemons
            _logger.Information("--- Sample Pokemons ---");
            for (int i = 0; i < Math.Min(3, pokemons.Count); i++)
            {
                var p = pokemons[i];
                _logger.Information($"  [{i}] {p.Name}: Types={string.Join(",", p.Type)}, HP={p.Base.HP}, Height={p.Height}, Weight={p.Weight}, Gen={p.Generation}");
            }
            _logger.Information("------------------------");

            InitializeTiposChart(pokemons);
            InitializeStatsTipoChart(pokemons);
            InitializeAlturaPesoChart(pokemons);
            InitializeGeneracionesChart(pokemons);
            InitializePokemonStatsChart(pokemons);
            InitializeLegendariosChart(pokemons);

            _logger.Information("✅ All charts initialized END");
        }
        catch (Exception ex)
        {
            _logger.Information($"❌ Error initializing charts: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private void InitializeTiposChart(IReadOnlyList<Pokemon> pokemons)
    {
        try
        {
            _logger.Information("📊 InitializeTiposChart START");

            if (PlotTipos == null) { _logger.Information("  ❌ PlotTipos is null!"); return; }
            _logger.Information("  ✅ PlotTipos NOT null");

            // Procesar tipos
            var tipos = pokemons
                .SelectMany(p => p.Type)
                .GroupBy(t => t)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .ToList();

            _logger.Information($"  📊 Found {tipos.Count} tipos");
            foreach (var t in tipos)
            {
                _logger.Information($"    - {t.Key}: {t.Count()}");
            }

            if (tipos.Count == 0)
            {
                _logger.Information("  ⚠️ No tipos found, skipping chart");
                return;
            }

            PlotTipos.Plot.Clear();
            _logger.Information("  ✅ Plot cleared");

            var colores = new[] { "#E3350C", "#3B82C4", "#FFCB05", "#4CAF50", "#9C27B0", "#FF9800", "#00BCD4", "#E91E63", "#8BC34A", "#795548" };

            var barras = tipos.Select((t, i) => new Bar
            {
                Position = i,
                Value = t.Count(),
                FillColor = Color.FromHex(colores[i % colores.Length]),
                LineWidth = 0
            }).ToArray();

            _logger.Information($"  📊 Created {barras.Length} bars");
            foreach (var b in barras)
            {
                _logger.Information($"    Bar position={b.Position}, value={b.Value}");
            }

            var added = PlotTipos.Plot.Add.Bars(barras);
            _logger.Information($"  ✅ Added bars to plot");

            PlotTipos.Plot.YLabel("Cantidad");
            PlotTipos.Plot.Title("Top 10 Tipos");

            PlotTipos.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
                tipos.Select((t, i) => new ScottPlot.Tick((double)i, t.Key)).ToArray());
            PlotTipos.Plot.Axes.Bottom.MajorTickStyle.Length = 0;
            PlotTipos.Plot.Axes.Title.Label.FontSize = 14;
            PlotTipos.Plot.Axes.Title.Label.Bold = true;
            PlotTipos.Plot.Axes.Bottom.Label.FontSize = 11;
            PlotTipos.Plot.Axes.Left.Label.FontSize = 11;
            PlotTipos.Plot.Axes.Bottom.TickLabelStyle.FontSize = 9;
            PlotTipos.Plot.Axes.Left.TickLabelStyle.FontSize = 9;
            PlotTipos.Plot.Axes.Margins(bottom: 0, top: 0.1);
            PlotTipos.Plot.Axes.Bottom.IsVisible = true;

            PlotTipos.Refresh();
            _logger.Information("  ✅ TiposChart Refreshed END");
        }
        catch (Exception ex)
        {
            _logger.Information($"❌ Error TiposChart: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private void InitializeStatsTipoChart(IReadOnlyList<Pokemon> pokemons)
    {
        try
        {
            _logger.Information("📊 InitializeStatsTipoChart START");

            if (PlotStatsTipo == null) { _logger.Information("  ❌ PlotStatsTipo is null!"); return; }

            var tipos = pokemons
                .SelectMany(p => p.Type)
                .Distinct()
                .OrderBy(t => t)
                .Take(6)
                .ToList();

            _logger.Information($"  Found {tipos.Count} tipos: {string.Join(", ", tipos)}");

            if (tipos.Count == 0) { _logger.Information("  ⚠️ No tipos"); return; }

            PlotStatsTipo.Plot.Clear();

            var stats = new[] { "HP", "ATK", "DEF", "Sp.Atk", "Sp.Def", "SPD" };
            var colores = new[] { "#E3350C", "#3B82C4", "#FFCB05", "#4CAF50", "#9C27B0", "#FF9800" };
            var xs = Enumerable.Range(0, tipos.Count).Select(i => (double)i).ToArray();

            for (int s = 0; s < stats.Length; s++)
            {
                var valores = tipos.Select(tipo =>
                {
                    var pts = pokemons.Where(p => p.Type.Contains(tipo)).ToList();
                    var avg = s switch
                    {
                        0 => pts.Average(p => p.Base.HP),
                        1 => pts.Average(p => p.Base.Attack),
                        2 => pts.Average(p => p.Base.Defense),
                        3 => pts.Average(p => p.Base.SpAttack),
                        4 => pts.Average(p => p.Base.SpDefense),
                        _ => pts.Average(p => p.Base.Speed)
                    };
                    _logger.Information($"    {stats[s]} for {tipo}: {avg:F1}");
                    return avg;
                }).ToArray();

                var scatter = PlotStatsTipo.Plot.Add.Scatter(xs, valores);
                scatter.LineWidth = 2;
                scatter.Color = Color.FromHex(colores[s]);
                scatter.MarkerSize = 6;
                scatter.MarkerShape = MarkerShape.FilledCircle;
                scatter.LegendText = stats[s];
            }

            PlotStatsTipo.Plot.Title("Stats Medios por Tipo");
            PlotStatsTipo.Plot.Legend.IsVisible = true;
            PlotStatsTipo.Plot.Legend.Alignment = Alignment.UpperLeft;
            PlotStatsTipo.Plot.Legend.FontSize = 11;
            PlotStatsTipo.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
                tipos.Select((t, i) => new ScottPlot.Tick((double)i, t)).ToArray());
            PlotStatsTipo.Plot.Axes.Bottom.MajorTickStyle.Length = 0;
            PlotStatsTipo.Plot.Axes.Title.Label.FontSize = 14;
            PlotStatsTipo.Plot.Axes.Title.Label.Bold = true;
            PlotStatsTipo.Plot.Axes.Margins(bottom: 0.05, top: 0.15);
            PlotStatsTipo.Refresh();
            _logger.Information("  ✅ StatsTipoChart END");
        }
        catch (Exception ex)
        {
            _logger.Information($"❌ Error StatsTipoChart: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private void InitializeAlturaPesoChart(IReadOnlyList<Pokemon> pokemons)
    {
        try
        {
            _logger.Information("📊 InitializeAlturaPesoChart START");

            if (PlotAlturaPeso == null) { _logger.Information("  ❌ PlotAlturaPeso is null!"); return; }

            var alturas = pokemons.Select(p => p.Height).ToArray();
            var pesos = pokemons.Select(p => p.Weight).ToArray();

            _logger.Information($"  altura count: {alturas.Length}, peso count: {pesos.Length}");
            _logger.Information($"  Sample heights: {string.Join(", ", alturas.Take(5))}");
            _logger.Information($"  Sample weights: {string.Join(", ", pesos.Take(5))}");

            var scatter = PlotAlturaPeso.Plot.Add.Scatter(alturas, pesos, Color.FromHex("#3B82C4"));
            scatter.MarkerSize = 4;
            scatter.MarkerShape = MarkerShape.FilledCircle;
            scatter.LineWidth = 0;

            PlotAlturaPeso.Plot.Title("Altura vs Peso");
            PlotAlturaPeso.Plot.XLabel("Altura (m)");
            PlotAlturaPeso.Plot.YLabel("Peso (kg)");
            PlotAlturaPeso.Plot.Axes.Title.Label.FontSize = 14;
            PlotAlturaPeso.Plot.Axes.Title.Label.Bold = true;
            PlotAlturaPeso.Refresh();
            _logger.Information("  ✅ AlturaPesoChart END");
        }
        catch (Exception ex)
        {
            _logger.Information($"❌ Error AlturaPesoChart: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private void InitializeGeneracionesChart(IReadOnlyList<Pokemon> pokemons)
    {
        try
        {
            _logger.Information("📊 InitializeGeneracionesChart START");

            if (PlotGeneraciones == null) { _logger.Information("  ❌ PlotGeneraciones is null!"); return; }

            var generaciones = pokemons
                .GroupBy(p => p.Generation)
                .OrderBy(g => g.Key)
                .ToList();

            _logger.Information($"  Found {generaciones.Count} generaciones");
            foreach (var g in generaciones)
            {
                _logger.Information($"    Gen {g.Key}: {g.Count()} pokemons");
            }

            if (generaciones.Count == 0) { _logger.Information("  ⚠️ No generaciones"); return; }

            PlotGeneraciones.Plot.Clear();

            var genColores = new[]
            {
                "#E3350C", "#3B82C4", "#FFCB05", "#4CAF50", "#9C27B0",
                "#FF9800", "#00BCD4", "#E91E63", "#8BC34A", "#795548"
            };

            var barras = generaciones.Select((g, i) => new Bar
            {
                Position = i,
                Value = g.Count(),
                FillColor = Color.FromHex(genColores[i % genColores.Length]),
                LineWidth = 0
            }).ToArray();

            PlotGeneraciones.Plot.Add.Bars(barras);
            PlotGeneraciones.Plot.Title("Pokemons por Generación");
            PlotGeneraciones.Plot.YLabel("Cantidad");
            PlotGeneraciones.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
                generaciones.Select((g, i) => new ScottPlot.Tick((double)i, ToRoman(g.Key))).ToArray());
            PlotGeneraciones.Plot.Axes.Bottom.MajorTickStyle.Length = 0;
            PlotGeneraciones.Plot.Axes.Title.Label.FontSize = 14;
            PlotGeneraciones.Plot.Axes.Title.Label.Bold = true;
            PlotGeneraciones.Plot.Axes.Margins(bottom: 0, top: 0.1);
            PlotGeneraciones.Refresh();
            _logger.Information("  ✅ GeneracionesChart END");
        }
        catch (Exception ex)
        {
            _logger.Information($"❌ Error GeneracionesChart: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private void InitializePokemonStatsChart(IReadOnlyList<Pokemon> pokemons)
    {
        try
        {
            _logger.Information("📊 InitializeTopStatsChart START");

            if (PlotPokemonStats == null) { _logger.Information("  ❌ PlotPokemonStats is null!"); return; }

            PlotPokemonStats.Plot.Clear();

            var top10 = pokemons
                .Select(p => new 
                { 
                    Name = p.Name, 
                    Total = p.Base.HP + p.Base.Attack + p.Base.Defense + 
                            p.Base.SpAttack + p.Base.SpDefense + p.Base.Speed 
                })
                .OrderByDescending(p => p.Total)
                .Take(10)
                .ToList();

            _logger.Information($"  Top 10 by total stats:");
            foreach (var p in top10)
            {
                _logger.Information($"    {p.Name}: {p.Total}");
            }

            var colores = new[]
            {
                "#FFD700", "#C0C0C0", "#CD7F32", "#E3350C", "#3B82C4",
                "#FFCB05", "#4CAF50", "#9C27B0", "#FF9800", "#00BCD4"
            };

            var barras = top10.Select((p, i) => new Bar
            {
                Position = i,
                Value = p.Total,
                FillColor = Color.FromHex(colores[i]),
                LineWidth = 0
            }).ToArray();

            PlotPokemonStats.Plot.Add.Bars(barras);
            PlotPokemonStats.Plot.Title("Top 10 Pokemons por Stats Totales");
            PlotPokemonStats.Plot.YLabel("Stats Totales");
            
            PlotPokemonStats.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
                top10.Select((p, i) => new ScottPlot.Tick((double)i, p.Name)).ToArray());
            PlotPokemonStats.Plot.Axes.Bottom.MajorTickStyle.Length = 0;
            PlotPokemonStats.Plot.Axes.Title.Label.FontSize = 14;
            PlotPokemonStats.Plot.Axes.Title.Label.Bold = true;
            PlotPokemonStats.Plot.Axes.Bottom.TickLabelStyle.FontSize = 9;
            PlotPokemonStats.Plot.Axes.Left.Label.FontSize = 11;
            PlotPokemonStats.Plot.Axes.Margins(bottom: 0.25, top: 0.1);
            
            PlotPokemonStats.Refresh();
            _logger.Information("  ✅ TopStatsChart END");
        }
        catch (Exception ex)
        {
            _logger.Information($"❌ Error TopStatsChart: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private void InitializeLegendariosChart(IReadOnlyList<Pokemon> pokemons)
    {
        try
        {
            _logger.Information("📊 InitializeLegendariosChart START");

            if (PlotLegendarios == null) { _logger.Information("  ❌ PlotLegendarios is null!"); return; }

            var legendarios = pokemons.Count(p => p.IsLegendary);
            var miticos = pokemons.Count(p => p.IsMythical);
            var normales = pokemons.Count(p => !p.IsLegendary && !p.IsMythical);

            _logger.Information($"  Normal: {normales}, Legendary: {legendarios}, Mythical: {miticos}");

            PlotLegendarios.Plot.Clear();

            var slices = new List<PieSlice>
            {
                new() { Value = normales, Label = $"Normal ({normales})", FillColor = Color.FromHex("#3B82C4") },
                new() { Value = legendarios, Label = $"Legendario ({legendarios})", FillColor = Color.FromHex("#FFCB05") },
                new() { Value = miticos, Label = $"Mítico ({miticos})", FillColor = Color.FromHex("#9C27B0") }
            };

            var pie = PlotLegendarios.Plot.Add.Pie(slices);
            pie.DonutFraction = 0.4;
            pie.ExplodeFraction = 0.02;

            PlotLegendarios.Plot.Title("Categoría Especial");
            PlotLegendarios.Plot.Legend.IsVisible = true;
            PlotLegendarios.Plot.Legend.Alignment = Alignment.MiddleRight;
            PlotLegendarios.Plot.Legend.FontSize = 12;
            PlotLegendarios.Plot.Axes.Title.Label.FontSize = 14;
            PlotLegendarios.Plot.Axes.Title.Label.Bold = true;
            PlotLegendarios.Plot.Axes.Frameless();
            PlotLegendarios.Plot.HideGrid();
            PlotLegendarios.Plot.Axes.Margins(bottom: 0.05, top: 0.05, left: 0.05, right: 0.35);
            PlotLegendarios.Refresh();
            _logger.Information("  ✅ LegendariosChart END");
        }
        catch (Exception ex)
        {
            _logger.Information($"❌ Error LegendariosChart: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private static string ToRoman(string generation)
    {
        var match = System.Text.RegularExpressions.Regex.Match(generation, @"\d+", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
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
