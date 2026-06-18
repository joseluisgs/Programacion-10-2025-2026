// ============================================================
// ProductoCsvStorage.cs - Storage para formato CSV
// ============================================================
// Implementación de IStorage para guardar/cargar productos en CSV.
//
// CONCEPTOS IMPORTANTES:
//
// 1. CSV STORAGE:
//    - Usa punto y coma (;) como delimitador
//    - Primera línea: cabecera
//    - Formato: Id;Nombre;Cantidad;Precio;EstaComprado
//
// 2. LINQ:
//    - File.ReadLines(): Lee líneas sin cargar todo en memoria
//    - Skip(1): Omite la cabecera
//    - Select(): Transforma cada línea
//    - Split(';'): Separa por delimitador
//    - Method chaining: Encadenar operaciones LINQ

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CSharpFunctionalExtensions;
using ListaCompra.Dto;
using ListaCompra.Errors;
using ListaCompra.Mappers;
using ListaCompra.Models;
using ListaCompra.Storage.Common;
using Serilog;

namespace ListaCompra.Storage.Csv;

public class ProductoCsvStorage : IStorage<Producto>
{
    private readonly ILogger _logger = Log.ForContext<ProductoCsvStorage>();

    public Result<bool, DomainError> Salvar(IEnumerable<Producto> items, string path)
    {
        try
        {
            _logger.Information("Guardando productos en CSV: {path}", path);
            
            using var writer = new StreamWriter(path, false, Encoding.UTF8);
            writer.WriteLine("Id;Nombre;Cantidad;Precio;EstaComprado");

            foreach (var producto in items)
            {
                var dto = ProductoMapper.ToDto(producto);
                writer.WriteLine($"{dto.Id};{dto.Nombre};{dto.Cantidad};{dto.Precio};{dto.EstaComprado}");
            }

            return Result.Success<bool, DomainError>(true);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al guardar en CSV: {path}", path);
            return Result.Failure<bool, DomainError>(BackupErrors.CreationError(ex.Message));
        }
    }

    public Result<IEnumerable<Producto>, DomainError> Cargar(string path)
    {
        _logger.Information("Cargando productos desde CSV: {path}", path);
        
        if (!Path.Exists(path))
        {
            _logger.Warning("Archivo no encontrado: {path}", path);
            return Result.Failure<IEnumerable<Producto>, DomainError>(BackupErrors.FileNotFound(path));
        }

        try
        {
            var productos = File.ReadLines(path, Encoding.UTF8)
                .Skip(1)
                .Select(linea => linea.Split(';'))
                .Select(campos => new ProductoDto(
                    int.Parse(campos[0]),
                    campos[1],
                    int.Parse(campos[2]),
                    decimal.Parse(campos[3]),
                    bool.TryParse(campos[4], out var comprado) && comprado
                ))
                .ToList()
                .Select(dto => ProductoMapper.ToModel(dto));

            return Result.Success<IEnumerable<Producto>, DomainError>(productos);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al cargar desde CSV: {path}", path);
            return Result.Failure<IEnumerable<Producto>, DomainError>(BackupErrors.InvalidBackupFile(ex.Message));
        }
    }
}
