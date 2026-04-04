// ============================================================
// ProductoRepository.cs - Repositorio de productos con SQLite y Dapper
// ============================================================
// Implementación del repositorio usando SQLite como base de datos
// y Dapper como ORM.
//
// CONCEPTOS IMPORTANTES:
//
// 1. REPOSITORIO:
//    - Patrón de acceso a datos
//    - Abstrae la fuente de datos
//    - CRUD: Create, Read, Update, Delete
//    - No usa Result<T, Error> (devuelve nullable)
//
// 2. DAPPER (MICRO-ORM):
//    - ORM ligero y rápido
//    - Consultas SQL con parámetros
//    - Query<T>: Ejecuta SELECT y mapea a objetos
//    - Execute: Ejecuta INSERT, UPDATE, DELETE
//
// 3. SQLITE:
//    - Base de datos embebida (archivo .db)
//    - No requiere servidor
//    - Ideal para aplicaciones de escritorio
//    - Microsoft.Data.Sqlite para conexión
//
// 4. ENTITY Y MAPPER:
//    - ProductoEntity: Representa la tabla en BD
//    - ProductoMapper: Convierte Entity <-> Model
//    - Separa la lógica de mapeo
//
// 5. SEED DATA:
//    - Datos iniciales de ejemplo
//    - Se carga si SeedData=true en appsettings.json
//    - Se borra si DropData=true
//    - ProductosFactory proporciona los datos

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;
using Dapper;
using ListaCompra.Config;
using ListaCompra.Entity;
using ListaCompra.Factories.Productos;
using ListaCompra.Mappers;
using ListaCompra.Models;
using Serilog;

namespace ListaCompra.Repositories;

public class ProductoRepository : IProductoRepository
{
    private readonly ILogger _logger = Log.ForContext<ProductoRepository>();
    private readonly string _connectionString;

    public ProductoRepository() : this(AppConfig.DropData, AppConfig.SeedData) { }

    private ProductoRepository(bool dropData, bool seedData)
    {
        _logger.Debug("Inicializando repositorio SQLite");
        _connectionString = AppConfig.ConnectionString;
        EnsureDataFolder();
        EnsureTable();

        if (dropData)
        {
            _logger.Warning("Borrando datos...");
            DeleteAll();
        }
        
        if (dropData || seedData)
        {
            _logger.Information("Cargando datos de semilla...");
            foreach (var producto in ProductosFactory.Seed())
            {
                Create(producto);
            }
            _logger.Information("SeedData completado");
        }
    }

    private SqliteConnection CreateConnection() => new(_connectionString);

    private void EnsureDataFolder()
    {
        if (!Directory.Exists(AppConfig.DataFolder))
        {
            Directory.CreateDirectory(AppConfig.DataFolder);
        }
    }

    private void EnsureTable()
    {
        using var connection = CreateConnection();
        connection.Open();
        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Productos (
                Id INTEGER PRIMARY KEY,
                Nombre TEXT NOT NULL,
                Cantidad INTEGER NOT NULL,
                Precio REAL NOT NULL,
                EstaComprado INTEGER NOT NULL DEFAULT 0,
                CreatedAt TEXT NOT NULL,
                UpdatedAt TEXT NOT NULL
            )");
    }

    public IEnumerable<Producto> GetAll()
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Productos";
        var entities = connection.Query<ProductoEntity>(sql).ToList();
        return ProductoMapper.ToModel(entities);
    }

    public Producto? GetById(int id)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Productos WHERE Id = @Id";
        var entity = connection.QueryFirstOrDefault<ProductoEntity>(sql, new { Id = id });
        return ProductoMapper.ToModel(entity);
    }

    public IEnumerable<Producto> GetByNombre(string nombre)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Productos WHERE Nombre LIKE @Nombre";
        var entities = connection.Query<ProductoEntity>(sql, new { Nombre = $"%{nombre}%" }).ToList();
        return ProductoMapper.ToModel(entities);
    }

    public Producto? Create(Producto model)
    {
        model = model with
        {
            Id = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var entity = ProductoMapper.ToEntity(model);

        using var connection = CreateConnection();
        var sql = @"INSERT INTO Productos (Nombre, Cantidad, Precio, EstaComprado, CreatedAt, UpdatedAt)
                    VALUES (@Nombre, @Cantidad, @Precio, @EstaComprado, @CreatedAt, @UpdatedAt);
                    SELECT last_insert_rowid();";

        entity.Id = connection.ExecuteScalar<int>(sql, new
        {
            entity.Nombre,
            entity.Cantidad,
            entity.Precio,
            EstaComprado = entity.EstaComprado ? 1 : 0,
            entity.CreatedAt,
            entity.UpdatedAt
        });

        return GetById(entity.Id);
    }

    public Producto? Update(int id, Producto model)
    {
        var existing = GetById(id);
        if (existing == null) return null;

        model = model with
        {
            Id = id,
            CreatedAt = existing.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        var entity = ProductoMapper.ToEntity(model);

        using var connection = CreateConnection();
        var sql = @"UPDATE Productos SET 
                    Nombre = @Nombre, Cantidad = @Cantidad, Precio = @Precio, 
                    EstaComprado = @EstaComprado, UpdatedAt = @UpdatedAt
                    WHERE Id = @Id";

        connection.Execute(sql, new
        {
            Id = id,
            entity.Nombre,
            entity.Cantidad,
            entity.Precio,
            EstaComprado = entity.EstaComprado ? 1 : 0,
            entity.UpdatedAt
        });

        return GetById(id);
    }

    public Producto? Delete(int id, bool logical = true)
    {
        var existing = GetById(id);
        if (existing == null) return null;

        using var connection = CreateConnection();
        
        if (logical)
        {
            var sql = "UPDATE Productos SET EstaComprado = 1, UpdatedAt = @UpdatedAt WHERE Id = @Id";
            connection.Execute(sql, new { Id = id, UpdatedAt = DateTime.UtcNow });
        }
        else
        {
            var sql = "DELETE FROM Productos WHERE Id = @Id";
            connection.Execute(sql, new { Id = id });
        }

        return existing;
    }

    public bool DeleteAll()
    {
        using var connection = CreateConnection();
        connection.Execute("DELETE FROM Productos");
        return true;
    }
}
