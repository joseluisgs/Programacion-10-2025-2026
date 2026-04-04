using Microsoft.EntityFrameworkCore;
using GestionAcademica.Entity;

namespace GestionAcademica.Entity;

/// <summary>
/// Contexto de Entity Framework Core para la base de datos de personas.
/// </summary>
public class AppDbContext : DbContext
{
    public DbSet<PersonaEntity> Personas { get; set; } = null!;

    private readonly string _connectionString;

    public AppDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        _connectionString = "";
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }
    }

    public void EnsureCreated()
    {
        Database.EnsureCreated();
    }
}
