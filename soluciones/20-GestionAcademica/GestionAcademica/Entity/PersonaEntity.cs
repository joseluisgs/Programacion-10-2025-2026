using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionAcademica.Entity;

/// <summary>
/// Entidad de base de datos para Personas (una sola tabla con todos los campos).
/// </summary>
[Table("Personas")]
public class PersonaEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(9)]
    public string Dni { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Apellidos { get; set; } = string.Empty;

    [Column(TypeName = "datetime2")]
    public DateTime FechaNacimiento { get; set; }

    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Imagen { get; set; }

    [Required]
    [MaxLength(20)]
    public string Tipo { get; set; } = "Persona";

    public int? Experiencia { get; set; }

    [MaxLength(100)]
    public string? Especialidad { get; set; }

    public int? Ciclo { get; set; }

    public int? Curso { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public double? Calificacion { get; set; }

    [Column(TypeName = "datetime2")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "datetime2")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime2")]
    public DateTime? DeletedAt { get; set; }
}
