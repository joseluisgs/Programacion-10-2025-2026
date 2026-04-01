// ============================================================
// BackupService.cs - Implementación del servicio de backup
// ============================================================
// Servicio para importar/exportar productos.
// Usa el storage apropiado según la extensión del archivo.
//
// CONCEPTOS IMPORTANTES:
//
// 1. BACKUP SERVICE:
//    - Coordina diferentes storages
//    -自动 selecciona storage por extensión
//    - .json -> ProductoJsonStorage
//    - .csv -> ProductoCsvStorage
//
// 2. INYECCIÓN DE DEPENDENCIAS:
//    - Recibe storages por constructor
//    - No necesita conocer la implementación
//
// 3. ROP:
//    - Exportar: Result<bool, DomainError>
//    - Importar: Result<IEnumerable<Producto>, DomainError>
//    - Errores específicos de backup

using System;
using System.Collections.Generic;
using System.IO;
using CSharpFunctionalExtensions;
using ListaCompra.Errors;
using ListaCompra.Models;
using ListaCompra.Storage.Csv;
using ListaCompra.Storage.Common;
using ListaCompra.Storage.Json;
using Serilog;

namespace ListaCompra.Services;

public class BackupService : IBackupService
{
    private readonly ILogger _logger = Log.ForContext<BackupService>();
    private readonly IStorage<Producto> _jsonStorage;
    private readonly IStorage<Producto> _csvStorage;

    public BackupService(ProductoJsonStorage jsonStorage, ProductoCsvStorage csvStorage)
    {
        _logger.Debug("BackupService inicializado");
        _jsonStorage = jsonStorage;
        _csvStorage = csvStorage;
    }

    public Result<bool, DomainError> Exportar(IEnumerable<Producto> productos, string path)
    {
        _logger.Information("Exportando productos a: {path}", path);
        
        var extension = Path.GetExtension(path).ToLowerInvariant();
        
        var storage = GetStorage(extension);
        if (storage == null)
        {
            _logger.Error("Extensión no soportada: {extension}", extension);
            return Result.Failure<bool, DomainError>(BackupErrors.CreationError($"Extensión no soportada: {extension}"));
        }

        return storage.Salvar(productos, path);
    }

    public Result<IEnumerable<Producto>, DomainError> Importar(string path)
    {
        _logger.Information("Importando productos desde: {path}", path);
        
        var extension = Path.GetExtension(path).ToLowerInvariant();
        
        var storage = GetStorage(extension);
        if (storage == null)
        {
            _logger.Error("Extensión no soportada: {extension}", extension);
            return Result.Failure<IEnumerable<Producto>, DomainError>(BackupErrors.InvalidBackupFile($"Extensión no soportada: {extension}"));
        }

        return storage.Cargar(path);
    }

    private IStorage<Producto>? GetStorage(string extension)
    {
        return extension switch
        {
            ".json" => _jsonStorage,
            ".csv" => _csvStorage,
            _ => null
        };
    }
}
