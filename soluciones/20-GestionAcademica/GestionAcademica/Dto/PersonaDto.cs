namespace GestionAcademica.Dto.Personas;

/// <summary>
/// Objeto de Transferencia de Datos para Personas.
/// Se utiliza para序列化 y deserialización de datos de estudiantes y docentes.
/// </summary>
public record PersonaDto(
    int Id,
    string Dni,
    string Nombre,
    string Apellidos,
    string FechaNacimiento,
    string Email,
    string? Imagen,
    string Tipo,
    string? Experiencia,
    string? Especialidad,
    string Ciclo,
    string? Curso,
    string? Calificacion,
    string CreatedAt,
    string UpdatedAt,
    bool IsDeleted,
    string? DeletedAt
)
{
    public PersonaDto() : this(0, "", "", "", "", "", null, "", null, null, "", null, null, "", "", false, null) { }
}
