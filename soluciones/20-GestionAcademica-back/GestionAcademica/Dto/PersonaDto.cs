namespace GestionAcademica.Dto.Personas;

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
