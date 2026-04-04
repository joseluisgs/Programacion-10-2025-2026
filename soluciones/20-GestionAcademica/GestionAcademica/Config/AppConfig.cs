using System.Globalization;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace GestionAcademica.Config;

/// <summary>
/// Clase de configuración que lee desde appsettings.json.
/// </summary>
public class AppConfig
{
    private static readonly IConfiguration Config;
    
    // Exponemos la configuración para que otros servicios (como Serilog) la usen
    public static IConfiguration Configuration => Config;
    
    static AppConfig()
    {
        Config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }

    public static double NotaAprobado => Config.GetValue<double>("Academica:NotaAprobado");

    public static CultureInfo Locale => CultureInfo.GetCultureInfo("es-ES");

    public static string DataFolder => Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, 
        Config.GetValue<string>("Repository:Directory") ?? "data");

    public static string ConnectionString => Config.GetValue<string>("Repository:ConnectionString") ?? "Data Source=data/academia.db";

    public static string StorageType => Config.GetValue<string>("Storage:Type") ?? "json";

    public static string RepositoryType
    {
        get
        {
            var type = Config.GetValue<string>("Repository:Type") ?? "memory";
            return type.ToLower() switch
            {
                "memory" => "memory",
                "json" => "json",
                "dapper" => "dapper",
                "efcore" => "efcore",
                _ => "memory"
            };
        }
    }

    public static string AcademiaFile
    {
        get
        {
            var extension = StorageType.ToLower() switch
            {
                "json" => "json",
                "csv" or "csv-alt" => "csv",
                "bin" => "bin",
                _ => "json"
            };
            return Path.Combine(DataFolder, $"academia.{extension}");
        }
    }

    public static int CacheSize => Config.GetValue<int>("Cache:Size", 10);

    public static bool DropData => Config.GetValue<bool>("Repository:DropData", false);

    public static bool SeedData => Config.GetValue<bool>("Repository:SeedData", true);

    public static bool UseLogicalDelete => Config.GetValue<bool>("Repository:UseLogicalDelete", true);

    public static string BackupDirectory => Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, 
        Config.GetValue<string>("Backup:Directory") ?? "backup");

    public static string BackupFormat
    {
        get
        {
            var format = Config.GetValue<string>("Backup:Format") ?? "json";
            return format.ToLower() switch
            {
                "json" => "json",
                "csv" => "csv",
                "bin" => "bin",
                _ => "json"
            };
        }
    }

    public static bool IsDevelopment => Config.GetValue<bool>("Development:Enabled", false);

    public static string ReportDirectory => Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, 
        Config.GetValue<string>("Reports:Directory") ?? "reports");

    // ====================================================================
    // CONFIGURACIÓN DE LOGGING
    // ====================================================================
    
    public static bool LogToFile => Config.GetValue<bool>("Logging:File:Enabled", true);

    public static string LogDirectory => Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        Config.GetValue<string>("Logging:File:Directory") ?? "log");

    public static int LogRetainDays => Config.GetValue<int>("Logging:File:RetainDays", 7);

    public static string LogLevel => Config.GetValue<string>("Logging:File:Level") ?? "Error";

    public static string LogOutputTemplate => Config.GetValue<string>("Logging:File:OutputTemplate") 
        ?? "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";

    // ====================================================================
    // CONFIGURACIÓN DE IMÁGENES
    // ====================================================================
    
    public static string[] AllowedImageExtensions => Config.GetSection("Images:AllowedExtensions").Get<string[]>() 
        ?? new[] { ".png", ".jpg", ".jpeg", ".bmp" };

    public static string ImagesDirectory => Path.Combine(
        DataFolder, 
        Config.GetValue<string>("Images:Directory") ?? "images");
}
