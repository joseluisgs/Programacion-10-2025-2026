// ============================================================
// AppConfig.cs - Configuración centralizada de la aplicación
// ============================================================
// Lee la configuración desde appsettings.json usando
// Microsoft.Extensions.Configuration.
//
// CONCEPTOS IMPORTANTES:
//
// 1. CONFIGURACIÓN CENTRALIZADA:
//    - Un único lugar para todas las opciones de configuración
//    - Se lee del archivo appsettings.json
//    - Permite cambiar valores sin recompilar
//
// 2. IConfiguration:
//    - Interfaz de Microsoft para acceder a configuración
//    - GetValue<T>(key, default): Obtiene valor con默认值
//    - Soporta secciones anidadas (Repository:Directory)
//
// 3. PROPIEDADES ESTÁTICAS:
//    - Todas las propiedades son estáticas (no necesita instancia)
//    - Se inicializan en el constructor estático
//    - Se accede directamente: AppConfig.CacheSize
//
// 4. APPSETTINGS.JSON:
//    - Archivo JSON con la configuración
//    - Se carga al iniciar la aplicación
//    - Sections: Repository, Cache, Logging

using System;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog.Events;

namespace ListaCompra.Config;

/// <summary>
/// Clase de configuración que lee desde appsettings.json.
/// </summary>
public class AppConfig
{
    private static readonly IConfiguration Config;
    
    static AppConfig()
    {
        Config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }

    public static CultureInfo Locale => CultureInfo.GetCultureInfo("es-ES");

    public static string DataFolder => Path.Combine(
        Environment.CurrentDirectory, 
        Config.GetValue<string>("Repository:Directory") ?? "data");

    public static string ConnectionString => Config.GetValue<string>("Repository:ConnectionString") 
        ?? $"Data Source={Path.Combine(DataFolder, "listacompra.db")}";

    public static bool DropData => Config.GetValue<bool>("Repository:DropData", false);

    public static bool SeedData => Config.GetValue<bool>("Repository:SeedData", true);

    public static int CacheSize => Config.GetValue<int>("Cache:Size", 100);

    public static bool CacheEnabled => Config.GetValue<bool>("Cache:Enabled", true);

    public static bool LogToFile => Config.GetValue<bool>("Logging:File:Enabled", true);

    public static string LogDirectory => Path.Combine(
        Environment.CurrentDirectory,
        Config.GetValue<string>("Logging:File:Directory") ?? "logs");

    public static int LogRetainDays => Config.GetValue<int>("Logging:File:RetainDays", 7);

    public static LogEventLevel LogLevel => Config.GetValue<string>("Logging:File:Level") switch
    {
        "Debug" => LogEventLevel.Debug,
        "Information" => LogEventLevel.Information,
        "Warning" => LogEventLevel.Warning,
        "Error" => LogEventLevel.Error,
        _ => LogEventLevel.Information
    };

    public static bool LogToConsole => Config.GetValue<bool>("Logging:Console:Enabled", true);

    public static LogEventLevel LogConsoleLevel => Config.GetValue<string>("Logging:Console:Level") switch
    {
        "Debug" => LogEventLevel.Debug,
        "Information" => LogEventLevel.Information,
        "Warning" => LogEventLevel.Warning,
        "Error" => LogEventLevel.Error,
        _ => LogEventLevel.Debug
    };
}
