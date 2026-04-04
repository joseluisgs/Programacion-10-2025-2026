using GestionAcademica.Errors.Common;

namespace GestionAcademica.Errors.Report;

/// <summary>
/// Contenedor de errores específicos para la generación de informes.
/// </summary>
public abstract record ReportError(string Message) : DomainError(Message)
{
    public sealed record GenerationError(string Details)
        : ReportError($"Error al generar el informe: {Details}");

    public sealed record SaveError(string Details)
        : ReportError($"Error al guardar el informe: {Details}");

    public sealed record DirectoryError(string Details)
        : ReportError($"Error con el directorio de informes: {Details}");
}

public static class ReportErrors
{
    public static DomainError GenerationError(string details) => new ReportError.GenerationError(details);
    public static DomainError SaveError(string details) => new ReportError.SaveError(details);
    public static DomainError DirectoryError(string details) => new ReportError.DirectoryError(details);
}
