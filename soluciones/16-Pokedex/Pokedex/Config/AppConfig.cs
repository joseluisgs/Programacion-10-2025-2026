// =============================================================================
// CONFIGURACIÓN DE LA APLICACIÓN
// =============================================================================
// Lee la configuración desde appsettings.json usando Microsoft.Extensions.Configuration.
// Proporciona propiedades estáticas para acceder a la configuración en cualquier parte.
// =============================================================================

using System;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog.Events;

namespace Pokedex.Config;

/// <summary>
/// Clase estática que proporciona acceso a la configuración de la aplicación.
/// Lee valores de appsettings.json en tiempo de ejecución.
/// </summary>
public class AppConfig
{
    // Configuración cargada desde JSON (solo lectura, se inicializa una vez)
    private static readonly IConfiguration Config;
    
    // ---------------------------------------------------------
    // CONSTRUCTOR ESTÁTICO:
    // Se ejecuta una sola vez cuando se accede por primera vez a la clase.
    // Configura el proveedor de configuración para leer appsettings.json
    // ---------------------------------------------------------
    static AppConfig()
    {
        Config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)  // Directorio base de la app
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }

    /// <summary>Cultura/localización española</summary>
    public static CultureInfo Locale => CultureInfo.GetCultureInfo("es-ES");

    /// <summary>Directorio donde se encuentran los datos</summary>
    public static string DataFolder => Path.Combine(
        Environment.CurrentDirectory, 
        Config.GetValue<string>("Repository:Directory") ?? "data");

    /// <summary>Ruta completa al archivo de pokemons</summary>
    public static string PokedexFile => Path.Combine(
        DataFolder,
        Config.GetValue<string>("Repository:FileName") ?? "pokedex.json");

    /// <summary>Indica si debe cargar datos de ejemplo</summary>
    public static bool SeedData => Config.GetValue<bool>("Repository:SeedData", true);

    /// <summary>Indica si debe eliminar datos existentes</summary>
    public static bool DropData => Config.GetValue<bool>("Repository:DropData", false);

    /// <summary>Tamaño máximo del caché LRU</summary>
    public static int CacheSize => Config.GetValue<int>("Cache:Size", 10);

    /// <summary>Indica si el caché está habilitado</summary>
    public static bool CacheEnabled => Config.GetValue<bool>("Cache:Enabled", true);

    /// <summary>Indica si debe escribir logs a archivo</summary>
    public static bool LogToFile => Config.GetValue<bool>("Logging:File:Enabled", true);

    /// <summary>Directorio donde se guardan los logs</summary>
    public static string LogDirectory => Path.Combine(
        Environment.CurrentDirectory,
        Config.GetValue<string>("Logging:File:Directory") ?? "logs");

    /// <summary>Días que se conservan los archivos de log</summary>
    public static int LogRetainDays => Config.GetValue<int>("Logging:File:RetainDays", 7);

    /// <summary>Nivel de log para archivo</summary>
    public static LogEventLevel LogLevel => Config.GetValue<string>("Logging:File:Level") switch
    {
        "Debug" => LogEventLevel.Debug,
        "Information" => LogEventLevel.Information,
        "Warning" => LogEventLevel.Warning,
        "Error" => LogEventLevel.Error,
        _ => LogEventLevel.Information
    };

    /// <summary>Indica si debe escribir logs a consola</summary>
    public static bool LogToConsole => Config.GetValue<bool>("Logging:Console:Enabled", true);

    /// <summary>Nivel de log para consola</summary>
    public static LogEventLevel LogConsoleLevel => Config.GetValue<string>("Logging:Console:Level") switch
    {
        "Debug" => LogEventLevel.Debug,
        "Information" => LogEventLevel.Information,
        "Warning" => LogEventLevel.Warning,
        "Error" => LogEventLevel.Error,
        _ => LogEventLevel.Debug
    };
}
