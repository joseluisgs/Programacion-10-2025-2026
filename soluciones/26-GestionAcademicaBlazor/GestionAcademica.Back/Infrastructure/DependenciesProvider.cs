using System.IO;
using GestionAcademica.Cache;
using GestionAcademica.Config;
using GestionAcademica.Entity;
using GestionAcademica.Models.Personas;
using GestionAcademica.Repositories.Personas.Base;
using GestionAcademica.Repositories.Personas.Dapper;
using GestionAcademica.Repositories.Personas.EfCore;
using GestionAcademica.Repositories.Personas.Json;
using GestionAcademica.Repositories.Personas.Memory;
using GestionAcademica.Services.Backup;
using GestionAcademica.Services.Dialogs;
using GestionAcademica.Services.Images;
using GestionAcademica.Services.ImportExport;
using GestionAcademica.Services.Personas;
using GestionAcademica.Services.Report;
using GestionAcademica.Storage.Binary;
using GestionAcademica.Storage.Common;
using GestionAcademica.Storage.Csv;
using GestionAcademica.Storage.Json;
using GestionAcademica.Validators.Common;
using GestionAcademica.Validators.Personas;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;

namespace GestionAcademica.Infrastructure;

/// <summary>
///     Proveedor de dependencias centralizado para toda la aplicación.
///     Configura la inyección de dependencias registrando repositorios, servicios, validadores y caches.
/// </summary>
public static class DependenciesProvider {
    /// <summary>
    ///     Construye y configura el contenedor de inyección de dependencias.
    /// </summary>
    /// <param name="configureAdditional">Callback opcional para registrar servicios adicionales.</param>
    /// <returns>Proveedor de servicios configurado.</returns>
    public static IServiceProvider BuildServiceProvider(Action<IServiceCollection>? configureAdditional = null) {
        // Crear colección de servicios
        var services = new ServiceCollection();

        // Limpiamos datos si es necesario antes de registrar dependencias.
        CleanData();

        // Registrar dependencias
        RegisterCaches(services);
        RegisterValidators(services);
        RegisterStorages(services);
        RegisterRepositories(services);
        RegisterServices(services);

        // Permitir extensión con servicios adicionales
        configureAdditional?.Invoke(services);

        // Construir el proveedor de servicios y devolverlo
        return services.BuildServiceProvider();
    }

    /// <summary>
    ///     Registra el almacenamiento (Storage) según la configuración de appsettings.json.
    /// </summary>
    private static void RegisterStorages(IServiceCollection services) {
        // Registrar almacenamiento para personas según configuración
        services.AddTransient<IStorage<Persona>>(sp => {
            var storageType = AppConfig.StorageType.ToLower();
            return storageType switch {
                "json" => new AcademiaJsonStorage(),
                "csv" => new AcademiaCsvStorage(),
                "bin" or "binary" => new AcademiaBinStorage(),
                _ => new AcademiaJsonStorage()
            };
        });
    }

    /// <summary>
    ///     Registra el repositorio de personas según la configuración de appsettings.json.
    ///     Permite intercambiar entre Memory, JSON, Dapper y EFCore.
    /// </summary>
    private static void RegisterRepositories(IServiceCollection services) {
        // Registrar repositorio de personas según configuración
        services.AddSingleton<IPersonasRepository>(sp => {
            var repoType = AppConfig.RepositoryType.ToLower();
            return repoType switch {
                "memory" => new PersonasMemoryRepository(AppConfig.DropData, AppConfig.SeedData),
                "json" => PersonasJsonRepository.CreateAsync(
                    Path.Combine(AppConfig.DataFolder, "academia.json"),
                    AppConfig.DropData,
                    AppConfig.SeedData).GetAwaiter().GetResult(),
                "dapper" => CreateDapperRepository(AppConfig.DropData, AppConfig.SeedData),
                "efcore" => CreateEfRepository(AppConfig.DropData, AppConfig.SeedData),
                _ => new PersonasMemoryRepository(AppConfig.DropData, AppConfig.SeedData)
            };
        });
    }

    /// <summary>
    ///     Crea el repositorio Dapper con conexión SQLite.
    /// </summary>
    private static PersonasDapperRepository CreateDapperRepository(bool dropData, bool seedData) {
        // Crear carpeta de datos si no existe
        var dataFolder = AppConfig.DataFolder;
        if (!Directory.Exists(dataFolder))
            Directory.CreateDirectory(dataFolder);

        var dbPath = Path.Combine(dataFolder, "academia.db");
        var connection = new SqliteConnection($"Data Source={dbPath}");
        connection.Open();
        return new PersonasDapperRepository(connection, () => connection.Close(), dropData, seedData);
    }

    /// <summary>
    ///     Crea el repositorio Entity Framework Core con SQLite.
    ///     Ahora pasa la connection string en lugar de un AppDbContext,
    ///     porque PersonasEfRepository crea sus propios contextos por operación
    ///     (patrón DbContextFactory educativo).
    /// </summary>
    private static PersonasEfRepository CreateEfRepository(bool dropData, bool seedData) {
        // Crear carpeta de datos si no existe
        var dataFolder = AppConfig.DataFolder;
        if (!Directory.Exists(dataFolder))
            Directory.CreateDirectory(dataFolder);

        var dbPath = Path.Combine(dataFolder, "academia.db");
        var connectionString = $"Data Source={dbPath}";
        return new PersonasEfRepository(connectionString, dropData, seedData);
    }

    /// <summary>
    ///     Registra los validadores para personas, estudiantes y docentes.
    /// </summary>
    private static void RegisterValidators(IServiceCollection services) {
        // Validador base para personas (campos comunes)
        services.AddTransient<IValidador<Persona>, ValidadorPersona>();
        // Validador específico para estudiantes
        services.AddTransient<IValidador<Estudiante>, ValidadorEstudiante>();
        // Validador específico para docentes
        services.AddTransient<IValidador<Docente>, ValidadorDocente>();
    }

    /// <summary>
    ///     Registra la caché LRU para optimizar lecturas por ID y DNI.
    /// </summary>
    private static void RegisterCaches(IServiceCollection services) {
        // Caché LRU para búsquedas por ID (int)
        services.AddSingleton<ICache<int, Persona>>(sp =>
            new LruCache<int, Persona>(AppConfig.CacheSize));

        // Caché LRU para búsquedas por DNI (string)
        services.AddSingleton<ICache<string, Persona>>(sp =>
            new LruCache<string, Persona>(AppConfig.CacheSize));
    }

    /// <summary>
    ///     Registra todos los servicios de la aplicación.
    /// </summary>
    private static void RegisterServices(IServiceCollection services) {
        // Servicio de diálogos para mostrar mensajes al usuario
        services.AddSingleton<IDialogService, DialogService>();

        // Servicio de backup (usa el storage configurado)
        services.AddTransient<IBackupService, BackupService>(sp =>
            new BackupService(sp.GetRequiredService<IStorage<Persona>>(), AppConfig.BackupDirectory));

        // Servicio de gestión de imágenes
        services.AddTransient<IImageService, ImageService>(sp =>
            new ImageService(Path.Combine(AppConfig.DataFolder, "images"), AppConfig.AllowedImageExtensions));

        // Servicio de generación de informes HTML
        services.AddTransient<IReportService, ReportService>(sp =>
            new ReportService(AppConfig.ReportDirectory));

        // Servicio de importación/exportación de datos
        services.AddTransient<IImportExportService, ImportExportService>();

        // Servicio principal de personas (inyecta todas las dependencias)
        services.AddScoped<IPersonasService, PersonasService>(sp =>
            new PersonasService(
                sp.GetRequiredService<IPersonasRepository>(),
                sp.GetRequiredService<IValidador<Persona>>(),
                sp.GetRequiredService<IValidador<Estudiante>>(),
                sp.GetRequiredService<IValidador<Docente>>(),
                sp.GetRequiredService<ICache<int, Persona>>(),
                sp.GetRequiredService<ICache<string, Persona>>(),
                sp.GetRequiredService<IImageService>()
            ));
    }

    /// <summary>
    ///     Limpia los directorios de informes e imágenes si se requiere DropData o SeedData.
    /// </summary>
    private static void CleanData() {
        if (AppConfig.DropData || AppConfig.SeedData) {
            CleanDirectory(AppConfig.ReportDirectory);
            CleanDirectory(AppConfig.ImagesDirectory);
        }
    }

    /// <summary>
    ///     Limpia un directorio eliminando todos sus archivos y subdirectorios.
    /// </summary>
    private static void CleanDirectory(string path) {
        try {
            if (Directory.Exists(path)) {
                // Eliminar todos los archivos
                foreach (var file in Directory.GetFiles(path))
                    try { File.Delete(file); } catch { }

                // Eliminar todos los subdirectorios
                foreach (var dir in Directory.GetDirectories(path))
                    try { Directory.Delete(dir, true); } catch { }
            }

            // Recrear el directorio vacío
            Directory.CreateDirectory(path);
        }
        catch (Exception ex) {
            Console.WriteLine($"Warning: No se pudo limpiar directorio {path}: {ex.Message}");
        }
    }
}