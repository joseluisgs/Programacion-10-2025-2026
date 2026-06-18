using Microsoft.EntityFrameworkCore;

namespace GestionAcademica.Entity;

/// <summary>
///     Contexto de Entity Framework Core para la base de datos de personas.
/// </summary>
public class AppDbContext : DbContext {
    private readonly string _connectionString;

    public AppDbContext(string connectionString) {
        _connectionString = connectionString;
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
        _connectionString = "";
    }

    public DbSet<PersonaEntity> Personas { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        if (!optionsBuilder.IsConfigured) optionsBuilder.UseSqlite(_connectionString);
    }

    public void EnsureCreated() {
        Database.EnsureCreated();
    }
}