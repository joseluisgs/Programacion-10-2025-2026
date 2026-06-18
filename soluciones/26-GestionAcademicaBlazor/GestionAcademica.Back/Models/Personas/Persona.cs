namespace GestionAcademica.Models.Personas;

/// <summary>
///     Clase base abstracta para representar una persona en el sistema.
/// </summary>
/// <remarks>
///     Shared por estudiantes y docentes. Utiliza record para inmutabilidad y comparación por DNI.
/// </remarks>
public abstract record Persona {
    /// <summary>Identificador único de la persona.</summary>
    public int Id { get; set; }
    /// <summary>DNI de la persona (formato español: 8 dígitos + letra).</summary>
    public string Dni { get; set; } = string.Empty;
    /// <summary>Nombre de la persona.</summary>
    public string Nombre { get; set; } = string.Empty;
    /// <summary>Apellidos de la persona.</summary>
    public string Apellidos { get; set; } = string.Empty;
    /// <summary>Fecha de nacimiento de la persona.</summary>
    public DateTime FechaNacimiento { get; set; }
    /// <summary>Correo electrónico de la persona.</summary>
    public string Email { get; set; } = string.Empty;
    /// <summary>Ruta relativa de la imagen de perfil (nullable).</summary>
    public string? Imagen { get; set; }
    /// <summary>Fecha y hora de creación del registro.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    /// <summary>Fecha y hora de última modificación.</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    /// <summary>Indica si la persona está dada de baja (borrado lógico).</summary>
    public bool IsDeleted { get; set; } = false;
    /// <summary>Fecha y hora de eliminación (nullable si no está eliminada).</summary>
    public DateTime? DeletedAt { get; set; } = null;

    /// <summary>
    ///     Indica si la persona es mayor de edad (>= 18 años).
    /// </summary>
    public bool IsMayorEdad => CalcularEdad() >= 18;

    /// <summary>
    ///     Nombre completo concatenando nombre y apellidos.
    /// </summary>
    public string NombreCompleto => $"{Nombre} {Apellidos}";

    /// <summary>
    ///     Compara personas por DNI (ignorando mayúsculas/minúsculas).
    /// </summary>
    public virtual bool Equals(Persona? other) {
        return other is not null && string.Equals(Dni, other.Dni, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Calcula la edad actual basada en la fecha de nacimiento.
    /// </summary>
    private int CalcularEdad() {
        var hoy = DateTime.Today;
        var edad = hoy.Year - FechaNacimiento.Year;
        if (FechaNacimiento.Date > hoy.AddYears(-edad))
            edad--;
        return edad;
    }

    /// <summary>
    ///     HashCode basado en el DNI en minúsculas.
    /// </summary>
    public override int GetHashCode() {
        return HashCode.Combine(Dni.ToLowerInvariant());
    }
}