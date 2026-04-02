// ============================================================
// ProductoRepository.cs - Repositorio de productos con SQLite
// ============================================================
// Implementación del repositorio usando SQLite.
//
// CARACTERÍSTICAS:
// - Persistencia en archivo SQLite
// - Borrado lógico (soft delete)
// - Seed data al iniciar si está configurado

using GestionProductos.Config;
using GestionProductos.Models;
using Microsoft.Data.Sqlite;

namespace GestionProductos.Repositories;

/// <summary>
/// Repositorio de productos implementado con SQLite.
/// </summary>
public class ProductoRepository : IProductoRepository
{
    private readonly string _connectionString;

    public ProductoRepository()
    {
        _connectionString = AppConfig.ConnectionString;
        Initialize();
        SeedData();
    }

    /// <summary>
    /// Inicializa la base de datos: crea tablas y borra datos si DropData=true.
    /// </summary>
    private void Initialize()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Productos (
                Id INTEGER PRIMARY KEY,
                Nombre TEXT NOT NULL,
                Descripcion TEXT,
                Categoria TEXT NOT NULL,
                Precio REAL NOT NULL,
                Stock INTEGER NOT NULL DEFAULT 0,
                Activo INTEGER NOT NULL DEFAULT 1,
                FechaCreacion TEXT NOT NULL,
                FechaActualizacion TEXT NOT NULL
            )";
        command.ExecuteNonQuery();

        if (AppConfig.DropData)
        {
            DeleteAll();
        }
    }

    /// <summary>
    /// Carga datos de ejemplo (seed) solo si SeedData=true y no hay datos.
    /// </summary>
    private void SeedData()
    {
        if (AppConfig.SeedData && !GetAll().Any())
        {
            var productos = new[]
            {
                new Producto(0, "Portátil HP 15s", "Portátil 15.6\" Intel Core i5", "Electrónica", 549.99m, 25),
                new Producto(0, "Ratón Inalámbrico", "Ratón ergonómico wireless", "Electrónica", 19.99m, 150),
                new Producto(0, "Teclado Mecánico", "Teclado RGB switches rojos", "Electrónica", 79.99m, 60),
                new Producto(0, "Monitor 24\"", "Monitor Full HD 60Hz", "Electrónica", 149.99m, 40),
                new Producto(0, "Webcam HD", "Webcam 1080p con micrófono", "Electrónica", 49.99m, 80),
                new Producto(0, "Silla Gaming", "Silla acolchada con soporte lumbar", "Muebles", 199.99m, 30),
                new Producto(0, "Escritorio", "Escritorio 120x60cm madera", "Muebles", 129.99m, 20),
                new Producto(0, "Lámpara LED", "Lámpara de escritorio regulable", "Iluminación", 24.99m, 100),
                new Producto(0, "Auriculares", "Auriculares cancelación ruido", "Electrónica", 129.99m, 75),
                new Producto(0, "Pendrive 64GB", "USB 3.0 alta velocidad", "Almacenamiento", 12.99m, 200),
                new Producto(0, "Disco SSD 500GB", "SSD interno SATA III", "Almacenamiento", 59.99m, 90),
                new Producto(0, "Cable HDMI 2m", "Cable HDMI 4K compatible", "Accesorios", 8.99m, 300),
                new Producto(0, "Hub USB 7 puertos", "Hub alimentado USB 3.0", "Accesorios", 29.99m, 120),
                new Producto(0, "Alfombrilla Ratón", "Alfombrilla 90x40cm gaming", "Accesorios", 14.99m, 180),
                new Producto(0, "Webcam 4K", "Webcam profesional streaming", "Electrónica", 89.99m, 45)
            };

            foreach (var producto in productos)
            {
                Create(producto);
            }
        }
    }

    /// <summary>
    /// Borra todos los registros.
    /// </summary>
    private void DeleteAll()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Productos";
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Obtiene todos los productos.
    /// </summary>
    public IEnumerable<Producto> GetAll()
    {
        var productos = new List<Producto>();
        
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Productos ORDER BY Nombre";
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            productos.Add(MapProducto(reader));
        }
        
        return productos;
    }

    /// <summary>
    /// Obtiene un producto por ID.
    /// </summary>
    public Producto? GetById(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Productos WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);
        
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return MapProducto(reader);
        }
        
        return null;
    }

    /// <summary>
    /// Crea un nuevo producto.
    /// </summary>
    public Producto? Create(Producto producto)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Productos (Nombre, Descripcion, Categoria, Precio, Stock, Activo, FechaCreacion, FechaActualizacion)
            VALUES (@Nombre, @Descripcion, @Categoria, @Precio, @Stock, @Activo, @FechaCreacion, @FechaActualizacion);
            SELECT last_insert_rowid();";
        
        producto.FechaCreacion = DateTime.Now;
        producto.FechaActualizacion = DateTime.Now;
        
        command.Parameters.AddWithValue("@Nombre", producto.Nombre);
        command.Parameters.AddWithValue("@Descripcion", producto.Descripcion ?? "");
        command.Parameters.AddWithValue("@Categoria", producto.Categoria);
        command.Parameters.AddWithValue("@Precio", producto.Precio);
        command.Parameters.AddWithValue("@Stock", producto.Stock);
        command.Parameters.AddWithValue("@Activo", producto.Activo ? 1 : 0);
        command.Parameters.AddWithValue("@FechaCreacion", producto.FechaCreacion.ToString("o"));
        command.Parameters.AddWithValue("@FechaActualizacion", producto.FechaActualizacion.ToString("o"));
        
        producto.Id = Convert.ToInt32(command.ExecuteScalar());
        
        return producto;
    }

    /// <summary>
    /// Actualiza un producto existente.
    /// </summary>
    public Producto? Update(int id, Producto producto)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Productos 
            SET Nombre = @Nombre, Descripcion = @Descripcion, Categoria = @Categoria,
                Precio = @Precio, Stock = @Stock, Activo = @Activo, FechaActualizacion = @FechaActualizacion
            WHERE Id = @Id";
        
        producto.FechaActualizacion = DateTime.Now;
        
        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@Nombre", producto.Nombre);
        command.Parameters.AddWithValue("@Descripcion", producto.Descripcion ?? "");
        command.Parameters.AddWithValue("@Categoria", producto.Categoria);
        command.Parameters.AddWithValue("@Precio", producto.Precio);
        command.Parameters.AddWithValue("@Stock", producto.Stock);
        command.Parameters.AddWithValue("@Activo", producto.Activo ? 1 : 0);
        command.Parameters.AddWithValue("@FechaActualizacion", producto.FechaActualizacion.ToString("o"));
        
        var rowsAffected = command.ExecuteNonQuery();
        
        if (rowsAffected == 0)
            return null;
        
        producto.Id = id;
        return producto;
    }

    /// <summary>
    /// Elimina un producto (borrado lógico).
    /// </summary>
    public Producto? Delete(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        var command = connection.CreateCommand();
        command.CommandText = "UPDATE Productos SET Activo = 0, FechaActualizacion = @Fecha WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@Fecha", DateTime.Now.ToString("o"));
        
        var rowsAffected = command.ExecuteNonQuery();
        
        if (rowsAffected == 0)
            return null;
        
        return GetById(id);
    }

    /// <summary>
    /// Busca productos por nombre.
    /// </summary>
    public IEnumerable<Producto> GetByNombre(string nombre)
    {
        var productos = new List<Producto>();
        
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Productos WHERE Nombre LIKE @Nombre ORDER BY Nombre";
        command.Parameters.AddWithValue("@Nombre", $"%{nombre}%");
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            productos.Add(MapProducto(reader));
        }
        
        return productos;
    }

    /// <summary>
    /// Busca productos por categoría.
    /// </summary>
    public IEnumerable<Producto> GetByCategoria(string categoria)
    {
        var productos = new List<Producto>();
        
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Productos WHERE Categoria = @Categoria ORDER BY Nombre";
        command.Parameters.AddWithValue("@Categoria", categoria);
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            productos.Add(MapProducto(reader));
        }
        
        return productos;
    }

    /// <summary>
    /// Obtiene solo productos activos.
    /// </summary>
    public IEnumerable<Producto> GetActivos()
    {
        var productos = new List<Producto>();
        
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Productos WHERE Activo = 1 ORDER BY Nombre";
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            productos.Add(MapProducto(reader));
        }
        
        return productos;
    }

    /// <summary>
    /// Búsqueda general por criterios.
    /// </summary>
    public IEnumerable<Producto> Search(string criterio)
    {
        var productos = new List<Producto>();
        
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT * FROM Productos 
            WHERE Activo = 1 AND (Nombre LIKE @Criterio OR Descripcion LIKE @Criterio OR Categoria LIKE @Criterio)
            ORDER BY Nombre";
        command.Parameters.AddWithValue("@Criterio", $"%{criterio}%");
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            productos.Add(MapProducto(reader));
        }
        
        return productos;
    }

    /// <summary>
    /// Mapea un registro de la BD a un objeto Producto.
    /// </summary>
    private static Producto MapProducto(SqliteDataReader reader)
    {
        return new Producto(
            reader.GetInt32(reader.GetOrdinal("Id")),
            reader.GetString(reader.GetOrdinal("Nombre")),
            reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? "" : reader.GetString(reader.GetOrdinal("Descripcion")),
            reader.GetString(reader.GetOrdinal("Categoria")),
            reader.GetDecimal(reader.GetOrdinal("Precio")),
            reader.GetInt32(reader.GetOrdinal("Stock")),
            reader.GetInt32(reader.GetOrdinal("Activo")) == 1
        );
    }
}