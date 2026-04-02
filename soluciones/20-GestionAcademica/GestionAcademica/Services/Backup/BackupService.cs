using System.IO;
using System.IO.Compression;
using CSharpFunctionalExtensions;
using GestionAcademica.Config;
using GestionAcademica.Errors.Backup;
using GestionAcademica.Errors.Common;
using GestionAcademica.Models.Personas;
using GestionAcademica.Storage.Common;
using Serilog;

namespace GestionAcademica.Services.Backup;

/// <summary>
/// Servicio para la gestión de copias de seguridad.
/// Permite crear, restaurar y eliminar backups del sistema.
/// </summary>
public class BackupService(
    IStorage<Persona> storage,
    string? defaultBackupDirectory = null
) : IBackupService
{
    private readonly string? _defaultBackupDirectory = defaultBackupDirectory;
    private readonly ILogger _logger = Log.ForContext<BackupService>();

    public Result<string, DomainError> RealizarBackup(
        IEnumerable<Persona> personas, 
        string? customBackupDirectory = null)
    {
        var backDirectory = customBackupDirectory ?? _defaultBackupDirectory 
            ?? throw new InvalidOperationException("No se ha especificado un directorio de backup.");

        _logger.Information("Iniciando proceso de backup en: {dir}", backDirectory);

        var personasList = personas.ToList();
        if (personasList.Count == 0)
        {
            _logger.Warning("No hay datos para respaldar.");
            return Result.Failure<string, DomainError>(BackupErrors.CreationError("No hay datos para respaldar."));
        }

        try
        {
            Directory.CreateDirectory(backDirectory);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al crear el directorio de backup: {dir}", backDirectory);
            return Result.Failure<string, DomainError>(BackupErrors.DirectoryError($"No se pudo crear el directorio: {backDirectory}"));
        }

        var tempDir = Path.Combine(Path.GetTempPath(), $"backup-{Guid.NewGuid()}");
        var dataDir = Path.Combine(tempDir, "data");
        var imgDir = Path.Combine(tempDir, "img");
        
        Directory.CreateDirectory(dataDir);
        Directory.CreateDirectory(imgDir);

        try
        {
            // 1. Guardar datos en data/
            var jsonPath = Path.Combine(dataDir, "personas.json");
            var salvarResult = storage.Salvar(personasList, jsonPath);
            if (salvarResult.IsFailure)
            {
                _logger.Error("Error al serializar los datos.");
                return Result.Failure<string, DomainError>(BackupErrors.CreationError("Error al serializar los datos."));
            }

            // 2. Copiar imágenes de personas que tengan imagen
            var imagenesCopiadas = 0;
            foreach (var persona in personasList)
            {
                if (!string.IsNullOrWhiteSpace(persona.Imagen))
                {
                    var sourceImagePath = GetImageSourcePath(persona.Imagen);
                    if (File.Exists(sourceImagePath))
                    {
                        try
                        {
                            var destImagePath = Path.Combine(imgDir, persona.Imagen);
                            File.Copy(sourceImagePath, destImagePath, true);
                            imagenesCopiadas++;
                            _logger.Debug("Imagen copiada: {img}", persona.Imagen);
                        }
                        catch (Exception ex)
                        {
                            _logger.Warning(ex, "Error al copiar imagen {img}", persona.Imagen);
                        }
                    }
                    else
                    {
                        _logger.Warning("Imagen no encontrada: {img} para persona {dni}", persona.Imagen, persona.Dni);
                    }
                }
            }
            _logger.Information("Copiadas {count} imágenes al backup.", imagenesCopiadas);

            // 3. Crear ZIP
            var fecha = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            var zipPath = Path.Combine(backDirectory, $"{fecha}-back.zip");

            try
            {
                ZipFile.CreateFromDirectory(tempDir, zipPath);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error al crear el archivo ZIP.");
                return Result.Failure<string, DomainError>(BackupErrors.CreationError("Error al comprimir el backup."));
            }

            _logger.Information("Backup creado correctamente: {zipPath}", zipPath);
            return Result.Success<string, DomainError>(zipPath);
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
                _logger.Debug("Directorio temporal limpiado.");
            }
        }
    }

    public Result<IEnumerable<Persona>, DomainError> RestaurarBackup(
        string archivoBackup,
        string? customImagesDirectory = null)
    {
        _logger.Information("Iniciando restauración desde: {archivo}", archivoBackup);

        if (!File.Exists(archivoBackup))
        {
            _logger.Warning("Archivo de backup no encontrado: {path}", archivoBackup);
            return Result.Failure<IEnumerable<Persona>, DomainError>(BackupErrors.FileNotFound(archivoBackup));
        }

        var tempDir = Path.Combine(Path.GetTempPath(), $"restore-{Guid.NewGuid()}");
        Directory.CreateDirectory(tempDir);

        try
        {
            try
            {
                ZipFile.ExtractToDirectory(archivoBackup, tempDir);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error al extraer el archivo ZIP.");
                return Result.Failure<IEnumerable<Persona>, DomainError>(BackupErrors.InvalidBackupFile("No se pudo extraer el archivo ZIP."));
            }

            var dataDir = Path.Combine(tempDir, "data");
            var imgDir = Path.Combine(tempDir, "img");

            var jsonPath = Path.Combine(dataDir, "personas.json");
            if (!File.Exists(jsonPath))
            {
                _logger.Warning("El archivo de backup no contiene datos válidos (personas.json no encontrado).");
                return Result.Failure<IEnumerable<Persona>, DomainError>(BackupErrors.InvalidBackupFile("El archivo de backup no contiene datos válidos."));
            }

            var cargarResult = storage.Cargar(jsonPath);
            if (cargarResult.IsFailure)
            {
                _logger.Error("Error al deserializar los datos del backup.");
                return Result.Failure<IEnumerable<Persona>, DomainError>(BackupErrors.InvalidBackupFile("El archivo de backup contiene datos corruptos."));
            }

            var personas = cargarResult.Value.ToList();

            // Restaurar imágenes si existe la carpeta img
            if (Directory.Exists(imgDir))
            {
                var imagenesRestauradas = 0;
                foreach (var persona in personas)
                {
                    if (!string.IsNullOrWhiteSpace(persona.Imagen))
                    {
                        var sourceImagePath = Path.Combine(imgDir, persona.Imagen);
                        if (File.Exists(sourceImagePath))
                        {
                            try
                            {
                                var destImagePath = GetImageDestPath(persona.Imagen, customImagesDirectory);
                                Directory.CreateDirectory(Path.GetDirectoryName(destImagePath)!);
                                File.Copy(sourceImagePath, destImagePath, true);
                                imagenesRestauradas++;
                                _logger.Debug("Imagen restaurada: {img}", persona.Imagen);
                            }
                            catch (Exception ex)
                            {
                                _logger.Warning(ex, "Error al restaurar imagen {img}", persona.Imagen);
                            }
                        }
                    }
                }
                _logger.Information("Restauradas {count} imágenes.", imagenesRestauradas);
            }

            _logger.Information("Datos extraídos del backup correctamente.");
            return Result.Success<IEnumerable<Persona>, DomainError>(personas);
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
                _logger.Debug("Directorio temporal limpiado.");
            }
        }
    }

    public IEnumerable<string> ListarBackups(string? customBackupDirectory = null)
    {
        var backDirectory = customBackupDirectory ?? _defaultBackupDirectory;
        if (backDirectory == null || !Directory.Exists(backDirectory)) 
            return Enumerable.Empty<string>();

        return Directory.GetFiles(backDirectory, "*.zip")
            .OrderByDescending(f => File.GetCreationTime(f));
    }

    public Result<string, DomainError> RealizarBackupSistema(IEnumerable<Persona> personas)
    {
        return RealizarBackup(personas);
    }

    public Result<int, DomainError> RestaurarBackupSistema(
        string archivoBackup, 
        Func<Persona, Result<Persona, DomainError>> createCallback,
        string? customImagesDirectory = null)
    {
        // Limpiar directorio de imágenes antes de restaurar
        CleanImagesDirectory(customImagesDirectory);

        return RestaurarBackup(archivoBackup, customImagesDirectory)
            .Bind(personas =>
            {
                var contador = 0;
                DomainError? primerError = null;
                
                foreach (var p in personas)
                {
                    var result = createCallback(p);
                    if (result.IsSuccess)
                        contador++;
                    else if (primerError == null)
                        primerError = result.Error;
                }
                
                if (primerError != null && contador == 0)
                    return Result.Failure<int, DomainError>(primerError);
                    
                _logger.Information("Restauración completada. Total registros: {count}", contador);
                return Result.Success<int, DomainError>(contador);
            });
    }

    private void CleanImagesDirectory(string? customDirectory = null)
    {
        try
        {
            var imagesDir = customDirectory != null 
                ? Path.Combine(customDirectory, "images")
                : Path.Combine(GetDataFolder(), "images");
                
            if (Directory.Exists(imagesDir))
            {
                foreach (var file in Directory.GetFiles(imagesDir))
                {
                    try { File.Delete(file); }
                    catch { /* Ignorar archivos en uso */ }
                }
                foreach (var dir in Directory.GetDirectories(imagesDir))
                {
                    try { Directory.Delete(dir, true); }
                    catch { /* Ignorar directorios en uso */ }
                }
                _logger.Information("Directorio de imágenes limpiado: {dir}", imagesDir);
            }
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "No se pudo limpiar directorio de imágenes");
        }
    }

    private static string GetImageSourcePath(string imageName)
    {
        var dataFolder = GetDataFolder();
        return Path.Combine(dataFolder, "images", imageName);
    }

    private static string GetImageDestPath(string imageName, string? customDirectory = null)
    {
        var baseDir = customDirectory ?? GetDataFolder();
        return Path.Combine(baseDir, "images", imageName);
    }

    private static string GetDataFolder()
    {
        return AppConfig.DataFolder;
    }
}