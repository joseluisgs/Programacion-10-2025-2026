using System.Globalization;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace GestionAcademica.Config;

/// <summary>
///     Clase de configuración que lee desde appsettings.json.
/// </summary>
public class AppConfig {
    static AppConfig() {
        Configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", false, true)
            .Build();
    }

    // Exponemos la configuración para que otros servicios (como Serilog) la usen
    public static IConfiguration Configuration { get; }

    public static double NotaAprobado => Configuration.GetValue<double>("Academica:NotaAprobado");

    public static CultureInfo Locale => CultureInfo.GetCultureInfo("es-ES");

    public static string DataFolder => Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        Configuration.GetValue<string>("Repository:Directory") ?? "data");

    public static string ConnectionString => Configuration.GetValue<string>("Repository:ConnectionString") ??
                                             "Data Source=data/academia.db";

    public static string StorageType => Configuration.GetValue<string>("Storage:Type") ?? "json";

    public static string RepositoryType {
        get {
            var type = Configuration.GetValue<string>("Repository:Type") ?? "memory";
            return type.ToLower() switch {
                "memory" => "memory",
                "json" => "json",
                "dapper" => "dapper",
                "efcore" => "efcore",
                _ => "memory"
            };
        }
    }

    public static string AcademiaFile {
        get {
            var extension = StorageType.ToLower() switch {
                "json" => "json",
                "csv" or "csv-alt" => "csv",
                "bin" => "bin",
                _ => "json"
            };
            return Path.Combine(DataFolder, $"academia.{extension}");
        }
    }

    public static int CacheSize => Configuration.GetValue("Cache:Size", 10);

    public static bool DropData => Configuration.GetValue("Repository:DropData", false);

    public static bool SeedData => Configuration.GetValue("Repository:SeedData", true);

    public static bool UseLogicalDelete => Configuration.GetValue("Repository:UseLogicalDelete", true);

    public static string BackupDirectory => Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        Configuration.GetValue<string>("Backup:Directory") ?? "backup");

    public static string BackupFormat {
        get {
            var format = Configuration.GetValue<string>("Backup:Format") ?? "json";
            return format.ToLower() switch {
                "json" => "json",
                "csv" => "csv",
                "bin" => "bin",
                _ => "json"
            };
        }
    }

    public static bool IsDevelopment => Configuration.GetValue("Development:Enabled", false);

    public static string ReportDirectory => Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        Configuration.GetValue<string>("Reports:Directory") ?? "reports");

    // ====================================================================
    // CONFIGURACIÓN DE LOGGING
    // ====================================================================

    public static bool LogToFile => Configuration.GetValue("Logging:File:Enabled", true);

    public static string LogDirectory => Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        Configuration.GetValue<string>("Logging:File:Directory") ?? "log");

    public static int LogRetainDays => Configuration.GetValue("Logging:File:RetainDays", 7);

    public static string LogLevel => Configuration.GetValue<string>("Logging:File:Level") ?? "Error";

    public static string LogOutputTemplate => Configuration.GetValue<string>("Logging:File:OutputTemplate")
                                              ??
                                              "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";

    // ====================================================================
    // CONFIGURACIÓN DE IMÁGENES
    // ====================================================================

    public static string[] AllowedImageExtensions =>
        Configuration.GetSection("Images:AllowedExtensions").Get<string[]>()
        ?? new[] { ".png", ".jpg", ".jpeg", ".bmp" };

    public static string ImagesDirectory => Path.Combine(
        DataFolder,
        Configuration.GetValue<string>("Images:Directory") ?? "images");
}