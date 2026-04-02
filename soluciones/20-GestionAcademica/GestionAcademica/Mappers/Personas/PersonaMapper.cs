using System.Globalization;
using GestionAcademica.Dto.Personas;
using GestionAcademica.Entity;
using GestionAcademica.Enums;
using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Personas;
using GestionAcademica.ViewModels.Forms;

namespace GestionAcademica.Mappers.Personas;

public static class PersonaMapper {
    // Formato fecha corta (solo fecha, sin hora) para FechaNacimiento
    private const string DateFormat = "d";
    // Formato fecha con hora para CreatedAt, UpdatedAt, DeletedAt
    private const string DateTimeFormat = "s";
    private static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

    /// <summary>
    ///     Convierte un objeto de tipo Estudiante a su representación en DTO
    /// </summary>
    /// <param name="dto">El DTO a convertir</param>
    /// <returns>Una instancia de Persona (Estudiante o Docente) basada en el DTO</returns>
    /// <exception cref="ArgumentException">Si el tipo de persona en el DTO no es reconocido</exception>
    public static Persona ToModel(this PersonaDto dto) {
        var createdAt = DateTime.Parse(dto.CreatedAt, InvariantCulture);
        var updatedAt = DateTime.Parse(dto.UpdatedAt, InvariantCulture);
        DateTime? deletedAt = string.IsNullOrEmpty(dto.DeletedAt) ? null : DateTime.Parse(dto.DeletedAt, InvariantCulture);
        var fechaNacimiento = DateTime.Parse(dto.FechaNacimiento, InvariantCulture);
        
        return dto.Tipo switch {
            "Estudiante" => new Estudiante {
                Id = dto.Id,
                Dni = dto.Dni,
                Nombre = dto.Nombre,
                Apellidos = dto.Apellidos,
                FechaNacimiento = fechaNacimiento,
                Email = dto.Email,
                Imagen = dto.Imagen,
                Calificacion = double.TryParse(dto.Calificacion, NumberStyles.Any, InvariantCulture, out var calif)
                    ? calif
                    : 0.0,
                Ciclo = Enum.TryParse(dto.Ciclo, out Ciclo ciclo) ? ciclo : Ciclo.DAW,
                Curso = Enum.TryParse(dto.Curso, out Curso curso) ? curso : Curso.Primero,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt,
                IsDeleted = dto.IsDeleted,
                DeletedAt = deletedAt
            },
            "Docente" => new Docente {
                Id = dto.Id,
                Dni = dto.Dni,
                Nombre = dto.Nombre,
                Apellidos = dto.Apellidos,
                FechaNacimiento = fechaNacimiento,
                Email = dto.Email,
                Imagen = dto.Imagen,
                Experiencia = int.TryParse(dto.Experiencia, out var exp) ? exp : 0,
                Especialidad = dto.Especialidad ?? string.Empty,
                Ciclo = Enum.TryParse(dto.Ciclo, out Ciclo ciclo) ? ciclo : Ciclo.DAW,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt,
                IsDeleted = dto.IsDeleted,
                DeletedAt = deletedAt
            },
            _ => throw new ArgumentException($"Tipo de persona desconocido: {dto.Tipo}")
        };
    }

    /// <summary>
    ///     Convierte un objeto de tipo Persona (Estudiante o Docente) a su representación en DTO
    /// </summary>
    /// <param name="persona">La persona a convertir</param>
    /// <returns>Una instancia de PersonaDto basada en el modelo</returns>
    /// <exception cref="ArgumentException">Si el tipo de persona no es reconocido</exception>
    public static PersonaDto ToDto(this Persona persona) {
        return persona switch {
            Estudiante estudiante => new PersonaDto(
                estudiante.Id,
                estudiante.Dni ?? string.Empty,
                estudiante.Nombre ?? string.Empty,
                estudiante.Apellidos ?? string.Empty,
                estudiante.FechaNacimiento.ToString(DateFormat, InvariantCulture),
                estudiante.Email ?? string.Empty,
                estudiante.Imagen,
                "Estudiante",
                null,
                null,
                estudiante.Ciclo.ToString(),
                estudiante.Curso.ToString(),
                estudiante.Calificacion.ToString(InvariantCulture),
                estudiante.CreatedAt.ToString(DateTimeFormat, InvariantCulture),
                estudiante.UpdatedAt.ToString(DateTimeFormat, InvariantCulture),
                estudiante.IsDeleted,
                estudiante.DeletedAt?.ToString(DateTimeFormat, InvariantCulture)
            ),
            Docente docente => new PersonaDto(
                docente.Id,
                docente.Dni ?? string.Empty,
                docente.Nombre ?? string.Empty,
                docente.Apellidos ?? string.Empty,
                docente.FechaNacimiento.ToString(DateFormat, InvariantCulture),
                docente.Email ?? string.Empty,
                docente.Imagen,
                "Docente",
                docente.Experiencia.ToString(),
                docente.Especialidad ?? string.Empty,
                docente.Ciclo.ToString(),
                null,
                null,
                docente.CreatedAt.ToString(DateTimeFormat, InvariantCulture),
                docente.UpdatedAt.ToString(DateTimeFormat, InvariantCulture),
                docente.IsDeleted,
                docente.DeletedAt?.ToString(DateTimeFormat, InvariantCulture)
            ),
            _ => throw new ArgumentException($"Tipo de persona desconocido: {persona.GetType().Name}")
        };
    }

    /// <summary>
    /// Convierte una entidad de base de datos a modelo de dominio.
    /// </summary>
    public static Persona? ToModel(this PersonaEntity? entity)
    {
        if (entity == null) return null;

        return entity.Tipo switch
        {
            "Estudiante" => new Estudiante
            {
                Id = entity.Id,
                Dni = entity.Dni,
                Nombre = entity.Nombre,
                Apellidos = entity.Apellidos,
                FechaNacimiento = entity.FechaNacimiento,
                Email = entity.Email,
                Imagen = entity.Imagen,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                IsDeleted = entity.IsDeleted,
                DeletedAt = entity.DeletedAt,
                Calificacion = entity.Calificacion ?? 0,
                Ciclo = (Ciclo)(entity.Ciclo ?? 0),
                Curso = (Curso)(entity.Curso ?? 0)
            },
            "Docente" => new Docente
            {
                Id = entity.Id,
                Dni = entity.Dni,
                Nombre = entity.Nombre,
                Apellidos = entity.Apellidos,
                FechaNacimiento = entity.FechaNacimiento,
                Email = entity.Email,
                Imagen = entity.Imagen,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                IsDeleted = entity.IsDeleted,
                DeletedAt = entity.DeletedAt,
                Experiencia = entity.Experiencia ?? 0,
                Especialidad = entity.Especialidad ?? string.Empty,
                Ciclo = (Ciclo)(entity.Ciclo ?? 0)
            },
            _ => throw new ArgumentException($"Tipo de persona desconocido: {entity.Tipo}")
        };
    }

    /// <summary>
    /// Convierte una lista de entidades a modelos de dominio.
    /// </summary>
    public static IEnumerable<Persona> ToModel(this IEnumerable<PersonaEntity> entities)
    {
        return entities.Select(ToModel).OfType<Persona>();
    }

    /// <summary>
    /// Convierte modelo de dominio a entidad de base de datos.
    /// </summary>
    public static PersonaEntity ToEntity(this Persona model)
    {
        return model switch
        {
            Estudiante e => new PersonaEntity
            {
                Id = e.Id,
                Dni = e.Dni ?? string.Empty,
                Nombre = e.Nombre ?? string.Empty,
                Apellidos = e.Apellidos ?? string.Empty,
                FechaNacimiento = e.FechaNacimiento,
                Email = e.Email ?? string.Empty,
                Imagen = e.Imagen,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
                IsDeleted = e.IsDeleted,
                DeletedAt = e.DeletedAt,
                Tipo = "Estudiante",
                Calificacion = e.Calificacion,
                Ciclo = (int)e.Ciclo,
                Curso = (int)e.Curso
            },
            Docente d => new PersonaEntity
            {
                Id = d.Id,
                Dni = d.Dni ?? string.Empty,
                Nombre = d.Nombre ?? string.Empty,
                Apellidos = d.Apellidos ?? string.Empty,
                FechaNacimiento = d.FechaNacimiento,
                Email = d.Email ?? string.Empty,
                Imagen = d.Imagen,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt,
                IsDeleted = d.IsDeleted,
                DeletedAt = d.DeletedAt,
                Tipo = "Docente",
                Experiencia = d.Experiencia,
                Especialidad = d.Especialidad ?? string.Empty,
                Ciclo = (int)d.Ciclo
            },
            _ => throw new ArgumentException($"Tipo de persona desconocido: {model.GetType().Name}")
        };
    }

    // ─── MAPPERS PARA FORMDATA ──────────────────────────────────────────────

    /// <summary>
    ///     Convierte un modelo de dominio Estudiante en su FormData de presentación.
    /// </summary>
    /// <param name="model">El estudiante de dominio a convertir.</param>
    /// <returns>Una instancia de EstudianteFormData lista para el binding WPF.</returns>
    public static EstudianteFormData ToFormData(this Estudiante model) => new()
    {
        Id = model.Id,
        Nombre = model.Nombre,
        Apellidos = model.Apellidos,
        Dni = model.Dni,
        Email = model.Email,
        FechaNacimiento = model.FechaNacimiento,
        Imagen = model.Imagen,
        Calificacion = model.Calificacion,
        Ciclo = model.Ciclo,
        Curso = model.Curso,
        CreatedAt = model.CreatedAt,
        UpdatedAt = model.UpdatedAt,
        IsDeleted = model.IsDeleted,
        DeletedAt = model.DeletedAt
    };

    /// <summary>
    ///     Convierte un FormData de Estudiante en su modelo de dominio puro.
    /// </summary>
    /// <param name="formData">El FormData a convertir.</param>
    /// <returns>Una instancia inmutable de Estudiante.</returns>
    public static Estudiante ToModel(this EstudianteFormData formData) => new()
    {
        Id = formData.Id,
        Nombre = formData.Nombre ?? string.Empty,
        Apellidos = formData.Apellidos ?? string.Empty,
        Dni = formData.Dni ?? string.Empty,
        Email = formData.Email ?? string.Empty,
        FechaNacimiento = formData.FechaNacimiento,
        Imagen = formData.Imagen,
        Calificacion = formData.Calificacion,
        Ciclo = formData.Ciclo,
        Curso = formData.Curso,
        CreatedAt = formData.CreatedAt,
        UpdatedAt = formData.UpdatedAt,
        IsDeleted = formData.IsDeleted,
        DeletedAt = formData.DeletedAt
    };

    /// <summary>
    ///     Convierte un modelo de dominio Docente en su FormData de presentación.
    /// </summary>
    /// <param name="model">El docente de dominio a convertir.</param>
    /// <returns>Una instancia de DocenteFormData lista para el binding WPF.</returns>
    public static DocenteFormData ToFormData(this Docente model) => new()
    {
        Id = model.Id,
        Nombre = model.Nombre,
        Apellidos = model.Apellidos,
        Dni = model.Dni,
        Email = model.Email,
        FechaNacimiento = model.FechaNacimiento,
        Imagen = model.Imagen,
        Experiencia = model.Experiencia,
        Especialidad = model.Especialidad,
        Ciclo = model.Ciclo,
        CreatedAt = model.CreatedAt,
        UpdatedAt = model.UpdatedAt,
        IsDeleted = model.IsDeleted,
        DeletedAt = model.DeletedAt
    };

    /// <summary>
    ///     Convierte un FormData de Docente en su modelo de dominio puro.
    /// </summary>
    /// <param name="formData">El FormData a convertir.</param>
    /// <returns>Una instancia inmutable de Docente.</returns>
    public static Docente ToModel(this DocenteFormData formData) => new()
    {
        Id = formData.Id,
        Nombre = formData.Nombre ?? string.Empty,
        Apellidos = formData.Apellidos ?? string.Empty,
        Dni = formData.Dni ?? string.Empty,
        Email = formData.Email ?? string.Empty,
        FechaNacimiento = formData.FechaNacimiento,
        Imagen = formData.Imagen,
        Experiencia = formData.Experiencia,
        Especialidad = formData.Especialidad ?? string.Empty,
        Ciclo = formData.Ciclo,
        CreatedAt = formData.CreatedAt,
        UpdatedAt = formData.UpdatedAt,
        IsDeleted = formData.IsDeleted,
        DeletedAt = formData.DeletedAt
    };
}