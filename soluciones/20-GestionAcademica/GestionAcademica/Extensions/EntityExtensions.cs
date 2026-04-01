using GestionAcademica.Models.Personas;

namespace GestionAcademica.Extensions;

/// <summary>
///     Métodos de extensión para clonar modelos de dominio, evitando la copia manual de propiedades.
/// </summary>
public static class EntityExtensions
{
    /// <summary>Crea una copia independiente del estudiante preservando todos sus valores excepto IsDeleted.</summary>
    public static Estudiante Clone(this Estudiante source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return source with { IsDeleted = false, DeletedAt = null };
    }

    /// <summary>Crea una copia independiente del docente preservando todos sus valores excepto IsDeleted.</summary>
    public static Docente Clone(this Docente source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return source with { IsDeleted = false, DeletedAt = null };
    }
}
