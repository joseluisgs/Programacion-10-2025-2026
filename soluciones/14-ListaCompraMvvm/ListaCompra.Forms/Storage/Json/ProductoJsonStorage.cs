// ============================================================
// ProductoJsonStorage.cs - Storage para formato JSON
// ============================================================
// Implementación de IStorage para guardar/cargar productos en JSON.
//
// CONCEPTOS IMPORTANTES:
//
// 1. JSON STORAGE:
//    - Usa System.Text.Json para serialización
//    - Configuración para formato legible
//    - Convierte Model a DTO para serializar
//
// 2. RESULT:
//    - Salvar: Devuelve Result<bool, DomainError>
//    - Cargar: Devuelve Result<IEnumerable<Producto>, DomainError>
//    - Maneja excepciones y retorna Failure

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;
using ListaCompra.Dto;
using ListaCompra.Errors;
using ListaCompra.Mappers;
using ListaCompra.Models;
using ListaCompra.Storage.Common;
using Serilog;

namespace ListaCompra.Storage.Json;

public class ProductoJsonStorage : IStorage<Producto>
{
    private readonly ILogger _logger = Log.ForContext<ProductoJsonStorage>();

    private readonly JsonSerializerOptions _options = new() 
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() },
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public Result<bool, DomainError> Salvar(IEnumerable<Producto> items, string path)
    {
        try
        {
            _logger.Information("Guardando productos en JSON: {path}", path);
            using var stream = File.Create(path);
            var dtos = items.Select(p => ProductoMapper.ToDto(p)).ToList();
            JsonSerializer.Serialize(stream, dtos, _options);
            return Result.Success<bool, DomainError>(true);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al guardar en JSON: {path}", path);
            return Result.Failure<bool, DomainError>(BackupErrors.CreationError(ex.Message));
        }
    }

    public Result<IEnumerable<Producto>, DomainError> Cargar(string path)
    {
        _logger.Information("Cargando productos desde JSON: {path}", path);
        
        if (!Path.Exists(path))
        {
            _logger.Warning("Archivo no encontrado: {path}", path);
            return Result.Failure<IEnumerable<Producto>, DomainError>(BackupErrors.FileNotFound(path));
        }

        try
        {
            using var stream = File.OpenRead(path);
            var dtos = JsonSerializer.Deserialize<List<ProductoDto>>(stream, _options);

            if (dtos == null)
                return Result.Failure<IEnumerable<Producto>, DomainError>(BackupErrors.InvalidBackupFile("No se pudieron deserializar los datos."));

            return Result.Success<IEnumerable<Producto>, DomainError>(ProductoMapper.ToModel(dtos));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al cargar desde JSON: {path}", path);
            return Result.Failure<IEnumerable<Producto>, DomainError>(BackupErrors.InvalidBackupFile(ex.Message));
        }
    }
}
