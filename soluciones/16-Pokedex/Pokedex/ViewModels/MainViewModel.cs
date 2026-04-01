// =============================================================================
// VIEWMODEL PRINCIPAL DE POKEDEX
// =============================================================================
// El ViewModel implementa el patrón MVVM (Model-View-ViewModel).
// Maneja la lógica de presentación y comunica View y Model.
// Usa CommunityToolkit.Mvvm para implementar el patrón de forma sencilla:
// - [ObservableProperty]: genera propiedades automáticamente
// - [RelayCommand]: genera comandos para bindings
// - partial void OnXxxChanged: se ejecuta cuando cambia una propiedad
// =============================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpFunctionalExtensions;
using Pokedex.Models;
using Pokedex.Services;
using Serilog;

namespace Pokedex.ViewModels;

/// <summary>
/// ViewModel principal de la aplicación Pokedex.
/// Maneja toda la lógica de presentación de la pantalla principal.
/// </summary>
public partial class MainViewModel(IPokedexService pokedexService) : ObservableObject
{
    // ---------------------------------------------------------
    // CONSTANTES Y DEPENDENCIAS
    // ---------------------------------------------------------
    private const int PageSize = 20;  // Pokemons por página
    private readonly ILogger _logger = Log.ForContext<MainViewModel>();
    private readonly IPokedexService _pokedexService = pokedexService;
    
    // Almacén interno de pokemons (datos completos sin filtrar)
    private List<Pokemon> _todosPokemons = [];
    // Almacén de pokemons después de aplicar filtros
    private List<Pokemon> _pokemonsFiltrados = [];

    // ---------------------------------------------------------
    // PROPIEDADES OBSERVABLES (Bindings con la Vista)
    // [ObservableProperty] genera automáticamente la propiedad pública
    // y un campo privado con el mismo nombre precedido de _
    // ---------------------------------------------------------
    
    /// <summary>Lista de pokemons de la página actual (para DataGrid)</summary>
    [ObservableProperty]
    private ObservableCollection<Pokemon> _pokemons = [];

    /// <summary>Lista de tipos disponibles (para ComboBox)</summary>
    [ObservableProperty]
    private ObservableCollection<string> _types = [];

    /// <summary>Texto de búsqueda</summary>
    [ObservableProperty]
    private string _busqueda = "";

    /// <summary>Tipo seleccionado en el filtro</summary>
    [ObservableProperty]
    private string _tipoSeleccionado = "All";

    /// <summary>Pokemon seleccionado en el DataGrid</summary>
    [ObservableProperty]
    private Pokemon? _pokemonSeleccionado;

    /// <summary>Número de página actual</summary>
    [ObservableProperty]
    private int _paginaActual = 1;

    /// <summary>Total de páginas disponibles</summary>
    [ObservableProperty]
    private int _totalPaginas = 1;

    /// <summary>Total de pokemons después de filtrar</summary>
    [ObservableProperty]
    private int _totalPokemons = 0;

    /// <summary>Mensaje de estado/error</summary>
    [ObservableProperty]
    private string _mensaje = "";

    /// <summary>Indica si hay algún error</summary>
    [ObservableProperty]
    private bool _hayError;

    /// <summary>URL de la imagen del pokemon seleccionado</summary>
    [ObservableProperty]
    private string _imagenPokemon = "";

    /// <summary>Ruta de la imagen por defecto</summary>
    [ObservableProperty]
    private string _imagenDefault = "/Resources/images/sin-image.png";

    /// <summary>Indica si la imagen es válida</summary>
    [ObservableProperty]
    private bool _esImagenValida = true;

    /// <summary>Generación del pokemon seleccionado</summary>
    [ObservableProperty]
    private string _generacion = "";

    /// <summary>Habitat del pokemon seleccionado</summary>
    [ObservableProperty]
    private string _habitat = "";

    /// <summary>Color del pokemon seleccionado</summary>
    [ObservableProperty]
    private string _color = "";

    /// <summary>Indica si el pokemon es legendario</summary>
    [ObservableProperty]
    private bool _esLegendario;

    /// <summary>Indica si el pokemon es mítico</summary>
    [ObservableProperty]
    private bool _esMitico;

    /// <summary>HP del pokemon seleccionado</summary>
    [ObservableProperty]
    private string _hp = "";

    /// <summary>Ataque del pokemon seleccionado</summary>
    [ObservableProperty]
    private string _ataque = "";

    /// <summary>Defensa del pokemon seleccionado</summary>
    [ObservableProperty]
    private string _defensa = "";

    /// <summary>Ataque especial del pokemon seleccionado</summary>
    [ObservableProperty]
    private string _spAtaque = "";

    /// <summary>Defensa especial del pokemon seleccionado</summary>
    [ObservableProperty]
    private string _spDefensa = "";

    /// <summary>Velocidad del pokemon seleccionado</summary>
    [ObservableProperty]
    private string _velocidad = "";

    /// <summary>Evolución previa del pokemon</summary>
    [ObservableProperty]
    private string _evolucionPrevia = "";

    /// <summary>Siguiente evolución del pokemon</summary>
    [ObservableProperty]
    private string _evolucionSiguiente = "";

    /// <summary>Indica si está cargando datos</summary>
    [ObservableProperty]
    private bool _estaCargando;

    /// <summary>Orden de la lista seleccionado</summary>
    [ObservableProperty]
    private string _ordenSeleccionado = "ID";

    /// <summary>Opciones disponibles para ordenación</summary>
    [ObservableProperty]
    private ObservableCollection<string> _opcionesOrden = ["ID", "Nombre", "Tipo", "Generación"];

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    
    /// <summary>
    /// Constructor sin parámetros para diseño XAML.
    /// </summary>
    public MainViewModel() : this(null!)
    {
        CargarDatos();
    }

    // ---------------------------------------------------------
    // MÉTODOS QUE SE EJECUTAN AL CAMBIAR PROPIEDADES
    // partial void OnXxxChanged se genera automáticamente por [ObservableProperty]
    // Se ejecuta automáticamente cuando cambia el valor de la propiedad
    // ---------------------------------------------------------

    /// <summary>Se ejecuta cuando cambia el texto de búsqueda</summary>
    partial void OnBusquedaChanged(string value)
    {
        Filtrar();
    }

    /// <summary>Se ejecuta cuando cambia el tipo seleccionado</summary>
    partial void OnTipoSeleccionadoChanged(string value)
    {
        Filtrar();
    }

    /// <summary>
    /// Se ejecuta cuando cambia el pokemon seleccionado.
    /// Actualiza todos los detalles del pokemon en la UI.
    /// </summary>
    partial void OnPokemonSeleccionadoChanged(Pokemon? value)
    {
        if (value != null)
        {
            // Actualiza la imagen
            ImagenPokemon = value.Thumbnail;
            EsImagenValida = !string.IsNullOrWhiteSpace(value.Thumbnail);
            
            // Actualiza información general
            Generacion = value.Generation;
            Habitat = value.Habitat ?? "Desconocido";
            Color = value.Color ?? "Desconocido";
            EsLegendario = value.IsLegendary;
            EsMitico = value.IsMythical;
            
            // Actualiza estadísticas base
            Hp = value.Base.HP.ToString();
            Ataque = value.Base.Attack.ToString();
            Defensa = value.Base.Defense.ToString();
            SpAtaque = value.Base.SpAttack.ToString();
            SpDefensa = value.Base.SpDefense.ToString();
            Velocidad = value.Base.Speed.ToString();
            
            // Actualiza evoluciones
            var prevEvo = value.PrevEvolution?.FirstOrDefault();
            var nextEvo = value.NextEvolution?.FirstOrDefault();
            EvolucionPrevia = prevEvo?.Name ?? "Ninguna";
            EvolucionSiguiente = nextEvo?.Name ?? "Ninguna";
        }
        else
        {
            // Limpia todos los campos cuando no hay selección
            ImagenPokemon = "/Resources/images/sin-image.png";
            EsImagenValida = false;
            Generacion = "";
            Habitat = "";
            Color = "";
            EsLegendario = false;
            EsMitico = false;
            Hp = "";
            Ataque = "";
            Defensa = "";
            SpAtaque = "";
            SpDefensa = "";
            Velocidad = "";
            EvolucionPrevia = "";
            EvolucionSiguiente = "";
        }
    }

    /// <summary>Se ejecuta cuando cambia la página actual</summary>
    partial void OnPaginaActualChanged(int value)
    {
        CargarPagina();
    }

    /// <summary>Se ejecuta cuando cambia el tipo de ordenación</summary>
    partial void OnOrdenSeleccionadoChanged(string value)
    {
        Filtrar();
    }

    // ---------------------------------------------------------
    // MÉTODOS PRIVADOS DE LÓGICA
    // ---------------------------------------------------------

    /// <summary>
    /// Carga los pokemons desde el archivo por defecto.
    /// Usa Task.Run para no bloquear la UI durante la carga.
    /// </summary>
    private async void CargarDatos()
    {
        EstaCargando = true;
        
        await Task.Run(() =>
        {
            var resultado = _pokedexService.LoadDefault();
            
            resultado.Match(
                onSuccess: _ =>
                {
                    _logger.Information("Pokemons cargados correctamente");
                    _todosPokemons = _pokedexService.GetAll().OrderBy(p => p.Id).ToList();
                    CargarTipos();
                },
                onFailure: error =>
                {
                    _logger.Error("Error al cargar pokemons: {Error}", error);
                    // Dispatcher.Invoke para actualizar la UI desde otro hilo
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Mensaje = error.ToString();
                        HayError = true;
                    });
                }
            );
        });
        
        EstaCargando = false;
        Filtrar();
    }

    /// <summary>Carga la lista de tipos disponibles desde el servicio</summary>
    private void CargarTipos()
    {
        Types = new ObservableCollection<string>(_pokedexService.GetTypes());
    }

    /// <summary>
    /// Filtra los pokemons según los criterios de búsqueda, tipo y orden.
    /// Aplica todos los filtros en cadena.
    /// </summary>
    private void Filtrar()
    {
        // Comienza con todos los pokemons
        var pokemons = _todosPokemons.AsEnumerable();

        // 1. Filtro por tipo
        if (!string.IsNullOrWhiteSpace(TipoSeleccionado) && TipoSeleccionado != "All")
        {
            pokemons = pokemons.Where(p => p.Type.Contains(TipoSeleccionado, StringComparer.OrdinalIgnoreCase));
        }

        // 2. Filtro por búsqueda (por ID o por nombre)
        if (!string.IsNullOrWhiteSpace(Busqueda))
        {
            if (int.TryParse(Busqueda, out int idBuscado))
            {
                // Si es número, busca por ID exacto
                pokemons = pokemons.Where(p => p.Id == idBuscado);
            }
            else
            {
                // Si es texto, busca por nombre (contiene)
                pokemons = pokemons.Where(p => p.Name.Contains(Busqueda, StringComparison.OrdinalIgnoreCase));
            }
        }

        // 3. Ordenación
        pokemons = OrdenSeleccionado switch
        {
            "Nombre" => pokemons.OrderBy(p => p.Name),
            "Tipo" => pokemons.OrderBy(p => p.Type.FirstOrDefault()).ThenBy(p => p.Name),
            "Generación" => pokemons.OrderBy(p => p.Generation).ThenBy(p => p.Id),
            _ => pokemons.OrderBy(p => p.Id)  // Por defecto por ID
        };

        // Guarda los pokemons filtrados para paginación
        _pokemonsFiltrados = pokemons.ToList();
        
        // Actualiza contadores
        if (_pokemonsFiltrados.Count == 0)
        {
            Mensaje = "No se encontraron pokemons";
            Pokemons = [];
        }
        else
        {
            Mensaje = "";
        }
        
        TotalPokemons = _pokemonsFiltrados.Count;
        TotalPaginas = (int)Math.Ceiling((double)TotalPokemons / PageSize);
        if (TotalPaginas == 0) TotalPaginas = 1;
        
        PaginaActual = 1;
        CargarPagina();
    }

    /// <summary>
    /// Carga una página específica de pokemons.
    /// Calcula qué pokemons mostrar según la página actual.
    /// </summary>
    private void CargarPagina()
    {
        if (_pokemonsFiltrados.Count == 0)
        {
            Pokemons = [];
            return;
        }

        // Skip: salta los elementos de páginas anteriores
        // Take: toma los elementos de la página actual
        var pokemons = _pokemonsFiltrados
            .Skip((PaginaActual - 1) * PageSize)
            .Take(PageSize);
        
        Pokemons = new ObservableCollection<Pokemon>(pokemons);
    }

    // ---------------------------------------------------------
    // COMANDOS (Acciones conectadas a botones/menús)
    // [RelayCommand] genera automáticamente el comando
    // Se pueden bindear en XAML con {Binding NombreComandoCommand}
    // ---------------------------------------------------------

    /// <summary>Comando para recargar datos</summary>
    [RelayCommand]
    private void Cargar()
    {
        CargarDatos();
    }

    /// <summary>Comando para limpiar el texto de búsqueda</summary>
    [RelayCommand]
    private void LimpiarBusqueda()
    {
        Busqueda = "";
    }

    /// <summary>Comando para ir a la siguiente página</summary>
    [RelayCommand]
    private void PaginaSiguiente()
    {
        if (PaginaActual < TotalPaginas)
        {
            PaginaActual++;
        }
    }

    /// <summary>Comando para ir a la página anterior</summary>
    [RelayCommand]
    private void PaginaAnterior()
    {
        if (PaginaActual > 1)
        {
            PaginaActual--;
        }
    }

    /// <summary>Comando para importar pokemons desde archivo JSON</summary>
    [RelayCommand]
    private void Importar()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "JSON Files (*.json)|*.json",
            DefaultExt = ".json",
            Title = "Importar Pokemons"
        };

        if (dialog.ShowDialog() == true)
        {
            var resultado = _pokedexService.LoadFromFile(dialog.FileName);
            
            resultado.Match(
                onSuccess: _ =>
                {
                    CargarDatos();
                    MessageBox.Show($"Pokemons importados desde {dialog.FileName}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                },
                onFailure: error =>
                {
                    MessageBox.Show(error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            );
        }
    }

    /// <summary>Comando para exportar pokemons a archivo JSON</summary>
    [RelayCommand]
    private void Exportar()
    {
        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "JSON Files (*.json)|*.json",
            DefaultExt = ".json",
            Title = "Exportar Pokemons"
        };

        if (dialog.ShowDialog() == true)
        {
            var resultado = _pokedexService.SaveToFile(dialog.FileName);
            
            resultado.Match(
                onSuccess: _ =>
                {
                    MessageBox.Show($"Pokemons exportados a {dialog.FileName}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                },
                onFailure: error =>
                {
                    MessageBox.Show(error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            );
        }
    }

    /// <summary>Comando para cerrar la aplicación</summary>
    [RelayCommand]
    private void Cerrar()
    {
        Application.Current.Shutdown();
    }
}
