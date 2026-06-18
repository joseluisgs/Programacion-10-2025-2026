using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using StarWars.Config;
using StarWars.Factories;
using StarWars.Models;
using System.Text;
using Application = System.Windows.Application;

namespace StarWars.ViewModels;

/// <summary>
/// ViewModel principal del juego Star Wars.
/// </summary>
/// <remarks>
/// Este ViewModel gestionatoda la lógica del juego:
/// - Control del estado del juego (en ejecución, terminado)
/// - Gestión de la matriz de droides en el cuadrante
/// - Ejecución del bucle principal del juego con async/await
/// - Actualización de la UI mediante propiedades observables
/// - Uso de comandos de CommunityToolkit.Mvvm
/// 
/// Se inyecta la dependencia DroideGenerator mediante el constructor,
/// siguiendo los principios de Inyección de Dependencias (DI).
/// </remarks>
public partial class MainViewModel(DroideGenerator droideGenerator) : ObservableObject
{
    // ============================================
    // ATRIBUTOS PRIVADOS (ESTADO INTERNO)
    // ============================================
    
    /// <summary>
    /// Logger específico para esta clase.
    /// Permite registrar eventos con contexto enriquecido.
    /// </summary>
    /// <remarks>
    /// Log.ForContext<MainViewModel>() crea un logger que incluye
    /// el nombre de la clase en cada mensaje, útil para debugging.
    /// </remarks>
    private readonly ILogger _logger = Log.ForContext<MainViewModel>();
    
    /// <summary>
    /// Generador de droides inyectado mediante DI.
    /// Se usa para crear instancias aleatorias de droides.
    /// </summary>
    private readonly DroideGenerator _droideGenerator = droideGenerator;

    /// <summary>
    /// Matriz bidimensional que representa el cuadrante del juego.
    /// Cada celda puede contener un droide o null (espacio vacío).
    /// </summary>
    /// <remarks>
    /// El tamaño de la matriz viene determinado por la propiedad Dimension.
    /// Se limpia y recolorea en cada intervalo de movimiento.
    /// </remarks>
    private Droide?[,]? _matriz;
    
    /// <summary>
    /// Array de todos los droides del juego (vivos y muertos).
    /// Se crea al iniciar la simulación y no cambia de tamaño.
    /// </summary>
    private Droide[]? _enemigos;
    
    /// <summary>
    /// Contador de disparos realizados en la simulación actual.
    /// Se incrementa cada vez que se ejecuta un ciclo del bucle.
    /// </summary>
    private int _numDisparos = 0;
    
    /// <summary>
    /// Contador de disparos que alcanzaron un droide.
    /// Se incrementa cuando hay un droide en la posición disparada.
    /// </summary>
    private int _numAciertos = 0;

    // ============================================
    // PROPIEDADES OBSERVABLES (PARA BINDING)
    // ============================================
    // Las propiedades con [ObservableProperty] generan automáticamente
    // una propiedad pública que notifica a la UI cuando cambia su valor.
    // El campo privado se nombra con _camelCase.
    
    /// <summary>
    /// Log de operaciones del juego.
    /// Muestra los eventos que ocurren durante la simulación.
    /// Se bindea en modo OneWay a un TextBox en la Vista.
    /// </summary>
    [ObservableProperty]
    private string _operacion = "";

    /// <summary>
    /// Representación visual del cuadrante (matriz de droides).
    /// Se bindea en modo OneWay a un TextBox para mostrar el mapa.
    /// </summary>
    [ObservableProperty]
    private string _cuadrante = "";

    /// <summary>
    /// Dimensión del mapa (5-9).
    /// Valor por defecto definido en Configuration.
    /// Se bindea TwoWay con los sliders de la UI.
    /// </summary>
    [ObservableProperty]
    private int _dimension = Configuration.DefaultDimension;

    /// <summary>
    /// Número de droides en el juego (5-30).
    /// Valor por defecto definido en Configuration.
    /// Se bindea TwoWay con los sliders de la UI.
    /// </summary>
    [ObservableProperty]
    private int _numDroides = Configuration.DefaultDroides;

    /// <summary>
    /// Tiempo máximo de la simulación en segundos (1-3).
    /// Valor por defecto definido en Configuration.
    /// Se bindea TwoWay con los sliders de la UI.
    /// </summary>
    [ObservableProperty]
    private int _tiempoMax = Configuration.DefaultTiempo;

    /// <summary>
    /// Contador de disparos realizados.
    /// Se bindea OneWay a un TextBox para mostrar al usuario.
    /// </summary>
    [ObservableProperty]
    private int _disparos = 0;

    /// <summary>
    /// Contador de aciertos (disparos que alcanzaron un droide).
    /// Se bindea OneWay a un TextBox para mostrar al usuario.
    /// </summary>
    [ObservableProperty]
    private int _aciertos = 0;

    /// <summary>
    /// Contador de droides muertos.
    /// Se bindea OneWay a un TextBox para mostrar al usuario.
    /// </summary>
    [ObservableProperty]
    private int _muertos = 0;

    /// <summary>
    /// Indica si la simulación está en ejecución.
    /// Se usa para:
    /// - Deshabilitar los sliders y botón (usando InverseBooleanConverter)
    /// - Mostrar/ocultar elementos de la UI
    /// </summary>
    /// <remarks>
    /// Cuando IsRunning=true, los controles deben estar deshabilitados.
    /// El InverseBooleanConverter invierte este valor para la propiedad IsEnabled.
    /// </remarks>
    [ObservableProperty]
    private bool _isRunning = false;

    /// <summary>
    /// Porcentaje de progreso de la simulación (0-100).
    /// Se bindea OneWay a un ProgressBar para mostrar el avance.
    /// </summary>
    [ObservableProperty]
    private double _progreso = 0.0;

    // ============================================
    // COMANDOS (ACCIONES DEL USUARIO)
    // ============================================
    // [RelayCommand] genera automáticamente un comando público
    // que puede bindearse a botones en la UI.
    
    /// <summary>
    /// Comando para iniciar la simulación del juego.
    /// </summary>
    /// <remarks>
    /// Este comando:
    /// - Se bindea al botón "Comenzar" en la UI
    /// - Está deshabilitado cuando IsRunning=true (via InverseBooleanConverter)
    /// - Ejecuta el método IniciarAsync de forma asíncrona
    /// - Permite que la UI responda mientras el juego está en curso
    /// </remarks>
    [RelayCommand]
    private async Task IniciarAsync()
    {
        // Registro del inicio de la simulación
        _logger.Information("Iniciando simulación");
        
        // ============================================
        // INICIALIZACIÓN DEL JUEGO
        // ============================================
        
        // Marcar el juego como en ejecución
        IsRunning = true;
        
        // Reiniciar el progreso a 0
        Progreso = 0.0;
        
        // Reiniciar contadores
        _numDisparos = 0;
        _numAciertos = 0;

        // Limpiar las áreas de texto
        Operacion = "";
        Cuadrante = "";

        // Crear la matriz del cuadrante según la dimensión seleccionada
        // Ejemplo: Dimension=5 crea una matriz 5x5
        _matriz = new Droide?[Dimension, Dimension];
        
        // Crear el array de enemigos según el número de droides seleccionado
        _enemigos = new Droide[NumDroides];
        
        // Generar cada droide usando el generador inyectado
        // Cada droide se crea con características aleatorias dentro de rangos
        for (int i = 0; i < NumDroides; i++)
        {
            _enemigos[i] = _droideGenerator.RandomDroide();
        }

        // Mostrar información inicial en el log de operaciones
        AddOperacion("Iniciando simulación...");
        AddOperacion($"Tamaño del mapa: {Dimension}x{Dimension}");
        AddOperacion($"Número de enemigos: {NumDroides}");
        AddOperacion($"Tiempo máximo: {TiempoMax} segundos");

        // Colocar los droides en posiciones aleatorias del cuadrante
        ColocarEnemigos();
        
        // Mostrar el estado inicial del mapa
        ImprimirMapa();

        // ============================================
        // BUCLE PRINCIPAL DEL JUEGO
        // ============================================
        
        // Variable para controlar el tiempo acumulado (en milisegundos)
        var time = 0;
        
        // Bucle que se ejecuta mientras:
        // - Queden enemigos vivos (GetEnemigosVivos() > 0)
        // - No se haya agotado el tiempo (time < TiempoMax * 1000)
        // - La tarea no sea cancelada (usa await, así que puede interrumplirse)
        while (GetEnemigosVivos() > 0 && time < TiempoMax * 1000)
        {
            // Registrar el tiempo actual en el log
            AddOperacion($"\nTiempo: {time} ms");
            AddOperacion($"Enemigos vivos: {GetEnemigosVivos()}");

            // ============================================
            // REUBICACIÓN DE DROIDES
            // ============================================
            
            // Cada Configuration.IntervaloMovimiento ms (300ms), los droides se reubican
            // Esto ocurre solo después del primer intervalo (time > 0)
            if (time % Configuration.IntervaloMovimiento == 0 && time > 0)
            {
                AddOperacion("Enemigos se reubican en el espacio...");
                ColocarEnemigos();
            }

            // ============================================
            // ESPERA ANTES DEL DISPARO
            // ============================================
            
            // Esperar el intervalo de disparo (100ms)
            // Esto permite que la UI se actualice y el usuario vea el progreso
            await Task.Delay(Configuration.IntervaloDisparo);
            
            // Incrementar el tiempo
            time += Configuration.IntervaloDisparo;

            // ============================================
            // DISPARO
            // ============================================
            
            // Calcular el valor del disparo (normal o crítico)
            var shotValue = DarDisparo();
            
            // Incrementar el contador de disparos
            _numDisparos++;

            // Elegir una posición aleatoria en el mapa para disparar
            var rnd = new Random();
            var row = rnd.Next(0, Dimension);  // Fila aleatoria
            var col = rnd.Next(0, Dimension);   // Columna aleatoria

            // Obtener el droide en esa posición (si existe)
            var enemy = _matriz?[row, col];
            
            // Verificar si hay un droide en la posición disparada
            if (enemy != null)
            {
                // ============================================
                // EL DISPARO ALCANZÓ UN DROIDE
                // ============================================
                
                AddOperacion($"Tiene un enemigo en: {row + 1}, {col + 1}");
                _numAciertos++;  // Incrementar aciertos

                // Variable para almacenar el daño efectivo (después de defenses)
                var effectiveDamage = 0;
                
                AddOperacion("Enemigo: " + enemy);

                if (enemy is IDefensa droideDefensa)
                {
                    AddOperacion("El enemigo está intentando defenderse...");
                    effectiveDamage = droideDefensa.Defender(shotValue);
                }

                if (enemy is IEscudo droideEscudo)
                {
                    AddOperacion($"El enemigo está usando su escudo: {droideEscudo.Shield}");
                    effectiveDamage = droideEscudo.UsarEscudo(shotValue);
                }

                if (enemy is IMovimiento droideMovimiento)
                {
                    AddOperacion("El enemigo está intentando escapar...");
                    if (!droideMovimiento.Moverse())
                    {
                        AddOperacion("El enemigo no ha podido escapar.");
                        effectiveDamage = shotValue;
                    }
                }

                // ============================================
                // APLICAR DAÑO AL DROIDE
                // ============================================
                
                // Reducir la energía del droide por el daño efectivo
                enemy.MaxEnergy -= effectiveDamage;
                
                AddOperacion($"Daño efectivo: {effectiveDamage}");
                AddOperacion("Estado del enemigo tras el ataque: " + enemy);

                // ============================================
                // ESPECIAL: SW6969 (POSIBLE EXPLOSIÓN)
                // ============================================
                
                // Si el droide es un SW6969, tiene probabilidad de explotar
                if (enemy is Sw6969 sw6969)
                {
                    AddOperacion("El enemigo es muy inestable, puede explotar.");
                    // Llamar al método de explosión (30% de probabilidad)
                    sw6969.Explotar();
                }
            }
            else
            {
                // ============================================
                // EL DISPARO NO ALCANZÓ NADA
                // ============================================
                
                AddOperacion("¡Disparo en el vacío!");
            }

            // ============================================
            // ACTUALIZAR LA UI
            // ============================================
            
            // Actualizar los contadores en la UI (usan Dispatcher para hilo seguro)
            ActualizarUI();
            
            // Volver a dibujar el mapa con las nuevas posiciones/estados
            ImprimirMapa();

            // Actualizar la barra de progreso
            // Progreso = (tiempo actual / tiempo máximo) * 100
            Progreso = (double)time / (TiempoMax * 1000) * 100;
        }

        // ============================================
        // FIN DE LA SIMULACIÓN
        // ============================================
        
        // Marcar el juego como terminado
        IsRunning = false;
        Progreso = 100;
        AddOperacion("\n=== SIMULACIÓN TERMINADA ===");
        var rendimiento = _numDisparos > 0 ? Math.Round((double)_numAciertos / _numDisparos * 100, 2) : 0;
        AddOperacion($"Disparos: {_numDisparos}");
        AddOperacion($"Aciertos: {_numAciertos}");
        AddOperacion($"Rendimiento: {rendimiento}%");
        AddOperacion($"Enemigos muertos: {GetEnemigosMuertos()}");
        AddOperacion($"Enemigos vivos: {GetEnemigosVivos()}");
        _logger.Information("Simulación finalizada");
        MostrarAlertaFin();
    }

    /// <summary>
    /// Coloca los droides vivos en posiciones aleatorias del cuadrante.
    /// </summary>
    /// <remarks>
    /// Este método:
    /// 1. Limpia la matriz (pone todas las celdas a null)
    /// 2. Cuenta cuántos enemigos vivos hay
    /// 3. Coloca cada enemigo vivo en una posición aleatoria vacía
    /// 
    /// Se llama:
    /// - Al inicio del juego
    /// - Cada Configuration.IntervaloMovimiento ms (300ms)
    /// </remarks>
    private void ColocarEnemigos()
    {
        // Verificar que las estructuras estén inicializadas
        if (_matriz == null || _enemigos == null) return;

        // Paso 1: Limpiar la matriz (todas las celdas a null)
        for (int row = 0; row < Dimension; row++)
        {
            for (int col = 0; col < Dimension; col++)
            {
                _matriz[row, col] = null;
            }
        }

        // Paso 2: Obtener el número de enemigos vivos
        var leftEnemies = GetEnemigosVivos();
        
        // Paso 3: Calcular cuántos podemos colocar (no más que celdas hay)
        var maxEnemiesToStore = Math.Min(Dimension * Dimension, leftEnemies);
        
        // Variables de control del bucle
        int storedEnemies = 0;
        int enemiesIndex = 0;
        var rnd = new Random();

        // Paso 4: Colocar cada enemigo vivo
        while (storedEnemies < maxEnemiesToStore)
        {
            // Buscar el siguiente enemigo vivo
            while (enemiesIndex < _enemigos.Length && !_enemigos[enemiesIndex].IsAlive)
            {
                enemiesIndex++;
            }

            // Intentamos colocar el droide en una posición aleatoria
            bool isStored = false;
            do
            {
                // Generar posición aleatoria
                var row = rnd.Next(0, Dimension);
                var col = rnd.Next(0, Dimension);
                
                // Solo colocar si la celda está vacía
                if (_matriz[row, col] == null)
                {
                    _matriz[row, col] = _enemigos[enemiesIndex];
                    storedEnemies++;
                    isStored = true;
                    enemiesIndex++;
                }
            } while (!isStored);  // Repetir hasta encontrar celda vacía
        }
    }

    /// <summary>
    /// Obtiene el número de droides vivos.
    /// </summary>
    /// <returns>Contador de droides con energía > 0</returns>
    private int GetEnemigosVivos()
    {
        if (_enemigos == null) return 0;
        return _enemigos.Count(d => d.IsAlive);  // Cuenta los que tienen IsAlive=true
    }

    /// <summary>
    /// Obtiene el número de droides muertos.
    /// </summary>
    /// <returns>Contador de droides con energía = 0</returns>
    private int GetEnemigosMuertos()
    {
        if (_enemigos == null) return 0;
        return _enemigos.Count(d => !d.IsAlive);  // Cuenta los que tienen IsAlive=false
    }

    /// <summary>
    /// Genera un valor de disparo (normal o crítico).
    /// </summary>
    /// <returns>Daño del disparo (25 normal, 50 crítico)</returns>
    /// <remarks>
    /// Hay un Configuration.ProbDisparoCritico (15%) de probabilidad de crítico.
    /// El disparo crítico hace el doble de daño (50 vs 25).
    /// </remarks>
    private int DarDisparo()
    {
        // Generar número aleatorio entre 0 y 100
        if (new Random().Next(0, 101) <= Configuration.ProbDisparoCritico)
        {
            // ¡Disparo crítico! (15% de probabilidad)
            AddOperacion("¡Has conseguido un disparo crítico!");
            return Configuration.DañoDisparoCritico;  // 50 puntos
        }
        
        // Disparo normal (85% de probabilidad)
        AddOperacion("Disparo normal.");
        return Configuration.DañoDisparoNormal;  // 25 puntos
    }

    /// <summary>
    /// Genera la representación textual del cuadrante.
    /// </summary>
    /// <remarks>
    /// Recorre toda la matriz y genera un string con formato:
    /// [      ] para celdas vacías
    /// [  SÍMBOLO  ] para celdas con droide
    /// 
    /// Este string se bindea a un TextBox en la UI para mostrar el mapa.
    /// </remarks>
    private void ImprimirMapa()
    {
        if (_matriz == null) return;
        
        // StringBuilder es más eficiente que concatenar strings en un bucle
        var sb = new StringBuilder();
        
        // Recorrer cada fila
        for (int row = 0; row < Dimension; row++)
        {
            // Recorrer cada columna de esa fila
            for (int col = 0; col < Dimension; col++)
            {
                // Obtener el droide en esta posición
                var droid = _matriz[row, col];
                
                // Generar la representación de la celda
                // Si hay droide: [  🔴  ]
                // Si está vacío: [      ]
                sb.Append(droid == null ? "[      ]" : $"[  {droid.Simbolo}  ]");
            }
            sb.AppendLine();
        }
        
        // Asignar el resultado a la propiedad cuadrante
        // Esto dispara PropertyChanged, actualizando la UI
        Cuadrante = sb.ToString();
    }

    /// <summary>
    /// Actualiza los contadores de la UI de forma segura.
    /// </summary>
    /// <remarks>
    /// Este método se llama desde un hilo secundario (el bucle async).
    /// WPF requiere que las actualizaciones de UI se hagan en el hilo principal.
    /// Dispatcher.Invoke() asegura que el código se ejecute en el hilo correcto.
    /// 
    /// Alternativas más modernas:
    /// - ObservableProperty ya maneja el cambio de hilo para propiedades simples
    /// - Aquí usamos Dispatcher por compatibilidad y para actualizar varias propiedades
    /// </remarks>
    private void ActualizarUI()
    {
        // Dispatcher.Invoke() ejecuta el código en el hilo de la UI
        Application.Current.Dispatcher.Invoke(() =>
        {
            // Actualizar los contadores (estas propiedades notifying a la UI)
            Disparos = _numDisparos;
            Aciertos = _numAciertos;
            Muertos = GetEnemigosMuertos();
        });
    }

    /// <summary>
    /// Añade un mensaje al log de operaciones.
    /// </summary>
    /// <param name="mensaje">Mensaje a añadir</param>
    /// <remarks>
    /// Este método:
    /// 1. Añade el mensaje a la propiedad Operacion (actualiza el TextBox)
    /// 2. También lo escribe en la consola (para debugging)
    /// </remarks>
    public event Action<string, string>? MostrarAlerta;

    private void MostrarAlertaFin()
    {
        var rendimiento = _numDisparos > 0 ? Math.Round((double)_numAciertos / _numDisparos * 100, 2) : 0;
        var msg = $"Simulación terminada\n\n" +
                  $"Disparos: {_numDisparos}\n" +
                  $"Aciertos: {_numAciertos}\n" +
                  $"Rendimiento: {rendimiento}%\n" +
                  $"Enemigos muertos: {GetEnemigosMuertos()}\n" +
                  $"Enemigos vivos: {GetEnemigosVivos()}";
        MostrarAlerta?.Invoke("Simulación Terminada", msg);
    }

    private void AddOperacion(string mensaje)
    {
        // Concatenar el mensaje con un salto de línea
        Operacion += mensaje + "\n";
        
        // También escribir en consola para debugging
        Console.WriteLine(mensaje);
    }
}