using System.Data;
using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using GestionAcademica.Cache;
using GestionAcademica.Config;
using GestionAcademica.Entity;
using GestionAcademica.Models.Personas;
using GestionAcademica.Repositories.Personas.Base;
using GestionAcademica.Repositories.Personas.Memory;
using GestionAcademica.Repositories.Personas.Json;
using GestionAcademica.Repositories.Personas.Dapper;
using GestionAcademica.Repositories.Personas.EfCore;
using GestionAcademica.Services.Personas;
using GestionAcademica.Services.Backup;
using GestionAcademica.Services.Dialogs;
using GestionAcademica.Services.Report;
using GestionAcademica.Services.Images;
using GestionAcademica.Services.ImportExport;
using GestionAcademica.Storage;
using GestionAcademica.Storage.Common;
using GestionAcademica.Storage.Json;
using GestionAcademica.Storage.Csv;
using GestionAcademica.Storage.Binary;
using GestionAcademica.Validators.Personas;
using GestionAcademica.Validators.Common;
using GestionAcademica.ViewModels;
using GestionAcademica.ViewModels.Dashboard;
using GestionAcademica.ViewModels.Docentes;
using GestionAcademica.ViewModels.Estudiantes;
using GestionAcademica.ViewModels.Informes;
using GestionAcademica.ViewModels.Graficos;
using GestionAcademica.ViewModels.Backup;
using GestionAcademica.ViewModels.ImportExport;

namespace GestionAcademica.Infrastructure;

public static class DependenciesProvider
{
    public static IServiceProvider BuildServiceProvider()
    {
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
        RegisterViewModels(services);
        
        // Construir el proveedor de servicios y devolverlo
        return services.BuildServiceProvider();
    }

    private static void RegisterStorages(IServiceCollection services)
    {
        // Registrar almacenamiento para personas según configuración
        services.AddTransient<IStorage<Persona>>(sp =>
        {
            var storageType = AppConfig.StorageType.ToLower();
            return storageType switch
            {
                "json" => new AcademiaJsonStorage(),
                "csv" => new AcademiaCsvStorage(),
                "bin" or "binary" => new AcademiaBinStorage(),
                _ => new AcademiaJsonStorage()
            };
        });
    }

    private static void RegisterRepositories(IServiceCollection services)
    {
        // Registrar repositorio de personas según configuración
        // Singleton porque el repositorio maneja el estado de las personas,
        // si se registrara como Transient se perdería el estado en cada inyección
        // si fuese como Scoped se compartiría el estado solo dentro de cada scope
        // (ej. petición web) pero no entre ellos, esto no es una pagina web,
        // es una aplicación, por lo que no hay scopes definidos
        // Además, al ser un servicio de larga duración,
        // es importante que el repositorio maneje su propia conexión
        // a la base de datos (en caso de usar Dapper o EF Core)
        // y no se cree una nueva conexión en cada inyección
        services.AddSingleton<IPersonasRepository>(sp =>
        {
            var repoType = AppConfig.RepositoryType.ToLower();
            return repoType switch
            {
                "memory" => new PersonasMemoryRepository(AppConfig.DropData, AppConfig.SeedData),
                "json" => new PersonasJsonRepository(Path.Combine(AppConfig.DataFolder, "academia.json")),
                "dapper" => CreateDapperRepository(),
                "efcore" => CreateEfRepository(),
                _ => new PersonasMemoryRepository(AppConfig.DropData, AppConfig.SeedData)
            };
        });
    }

    private static PersonasDapperRepository CreateDapperRepository()
    {
        // Crear carpeta de datos si no existe
        var dataFolder = AppConfig.DataFolder;
        if (!Directory.Exists(dataFolder))
            Directory.CreateDirectory(dataFolder);
        
        // Crear base de datos si no existe
        var dbPath = Path.Combine(dataFolder, "academia.db");
        var connection = new SqliteConnection($"Data Source={dbPath}");
        // Abrir conexión para crear la base de datos si no existe
        connection.Open(); 
        // Devolver repositorio con conexión abierta, el repositorio se encargará de cerrarla
        return new PersonasDapperRepository(connection, () => connection.Close());
    }

    private static PersonasEfRepository CreateEfRepository()
    {
        // Crear carpeta de datos si no existe
        var dataFolder = AppConfig.DataFolder;
        if (!Directory.Exists(dataFolder))
            Directory.CreateDirectory(dataFolder);
        
        // Crear base de datos si no existe
        var dbPath = Path.Combine(dataFolder, "academia.db");
        var context = new AppDbContext($"Data Source={dbPath}");
        
        // Crear la base de datos si no existe
        return new PersonasEfRepository(context);
    }

    private static void RegisterValidators(IServiceCollection services)
    {
        // Registrar validadores para Persona, Estudiante y Docente
        services.AddTransient<IValidador<Persona>, ValidadorPersona>();
        services.AddTransient<IValidador<Persona>, ValidadorEstudiante>();
        services.AddTransient<IValidador<Persona>, ValidadorDocente>();
    }

    private static void RegisterCaches(IServiceCollection services)
    {
        // Registrar LRU Cache para personas
        services.AddSingleton<ICache<int, Persona>>(sp =>
            new LruCache<int, Persona>(AppConfig.CacheSize));
    }

    private static void RegisterServices(IServiceCollection services)
    {
        // Registrar DialogService como Singleton
        services.AddSingleton<IDialogService, DialogService>();
        
        // Inyectar BackupDirectory en BackupService (por defecto)
        // El directorio puede ser sobrescrito en ejecución con parámetro customBackupDirectory
        services.AddTransient<IBackupService, BackupService>(sp => 
            new BackupService(sp.GetRequiredService<IStorage<Persona>>(), AppConfig.BackupDirectory));
        
        // Inyectar ImageDirectory y extensiones permitidas
        services.AddTransient<IImageService, ImageService>(sp => 
            new ImageService(Path.Combine(AppConfig.DataFolder, "images"), AppConfig.AllowedImageExtensions));
        
        // Inyectar ReportDirectory en ReportService
        services.AddTransient<IReportService, ReportService>(sp => 
            new ReportService(AppConfig.ReportDirectory));

        // Registrar ImportExportService
        services.AddTransient<IImportExportService, ImportExportService>();
        
        // Inyectar IPersonasService
        services.AddScoped<IPersonasService, PersonasService>(sp => 
            new PersonasService(
                sp.GetRequiredService<IPersonasRepository>(),
                sp.GetRequiredService<IValidador<Persona>>(), // ValidadorPersona
                sp.GetServices<IValidador<Persona>>().OfType<ValidadorEstudiante>().First(),
                sp.GetServices<IValidador<Persona>>().OfType<ValidadorDocente>().First(),
                sp.GetRequiredService<ICache<int, Persona>>()
            ));
    }

    private static void RegisterViewModels(IServiceCollection services)
    {
        // Registrar ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<EstudiantesViewModel>();
        services.AddTransient<DocentesViewModel>();
        services.AddTransient<InformesViewModel>();
        services.AddTransient<GraficosViewModel>();
        services.AddTransient<BackupViewModel>();
        services.AddTransient<ImportExportViewModel>();
    }

    private static void CleanData()
    {
         // Limpiar directorios de reports e images si DropData o SeedData están activos
        if (AppConfig.DropData || AppConfig.SeedData)
        {
            CleanDirectory(AppConfig.ReportDirectory);
            CleanDirectory(AppConfig.ImagesDirectory);
        }
    }

    private static void CleanDirectory(string path)
    {
        try
        {
            if (Directory.Exists(path))
            {
                foreach (var file in Directory.GetFiles(path))
                {
                    try { File.Delete(file); }
                    catch { /* Ignorar archivos en uso */ }
                }
                foreach (var dir in Directory.GetDirectories(path))
                {
                    try { Directory.Delete(dir, true); }
                    catch { /* Ignorar directorios en uso */ }
                }
            }
            Directory.CreateDirectory(path);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: No se pudo limpiar directorio {path}: {ex.Message}");
        }
    }
}
