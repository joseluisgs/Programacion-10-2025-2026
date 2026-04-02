// ============================================================
// AppConfig.cs - Configuración de la aplicación
// ============================================================
// Lee la configuración desde appsettings.json
//
// PROPIEDADES:
// - DatabaseType: Tipo de base de datos (Sqlite)
// - ConnectionString: Cadena de conexión a la BD
// - DataFolder: Carpeta donde se guarda la BD
// - DropData: Si true, borra los datos al iniciar
// - SeedData: Si true, carga datos de ejemplo

using System.IO;
using Microsoft.Extensions.Configuration;

namespace GestionProductos.Config;

/// <summary>
/// Clase de configuración que lee desde appsettings.json.
/// </summary>
public class AppConfig
{
    private static readonly IConfiguration Config;
    
    static AppConfig()
    {
        // Construir configuración desde appsettings.json
        Config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }

    /// <summary>
    /// Tipo de base de datos.
    /// </summary>
    public static string DatabaseType => Config.GetValue<string>("Database:Type") ?? "Sqlite";

    /// <summary>
    /// Cadena de conexión a la base de datos.
    /// </summary>
    public static string ConnectionString => Config.GetValue<string>("Database:ConnectionString") 
        ?? "Data Source=data/productos.db";

    /// <summary>
    /// Carpeta donde se guardan los datos.
    /// </summary>
    public static string DataFolder
    {
        get
        {
            var folder = Path.GetDirectoryName(ConnectionString) ?? "data";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            return folder;
        }
    }

    /// <summary>
    /// Si true, borra los datos al iniciar.
    /// </summary>
    public static bool DropData => Config.GetValue<bool>("Repository:DropData", false);

    /// <summary>
    /// Si true, carga datos de ejemplo (seed).
    /// </summary>
    public static bool SeedData => Config.GetValue<bool>("Repository:SeedData", true);

    /// <summary>
    /// Nivel de logging.
    /// </summary>
    public static string LogLevel => Config.GetValue<string>("Logging:Level") ?? "Information";
}