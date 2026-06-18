using CSharpFunctionalExtensions;
using GestionAcademica.Errors.Common;
using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Informes;
using GestionAcademica.Models.Personas;

namespace GestionAcademica.Services.Report;

/// <summary>
///     Define el contrato para la generación de informes del sistema.
/// </summary>
public interface IReportService {
    /// <summary>
    ///     Genera un informe estadístico de estudiantes.
    /// </summary>
    Task<InformeEstudiante> GenerarInformeEstudianteAsync(IEnumerable<Estudiante> estudiantes, double notaAprobado,
        Ciclo? ciclo = null, Curso? curso = null);

    /// <summary>
    ///     Genera un informe estadístico de docentes.
    /// </summary>
    Task<InformeDocente> GenerarInformeDocenteAsync(IEnumerable<Docente> docentes, Ciclo? ciclo = null);

    /// <summary>
    ///     Genera un informe HTML de estudiantes.
    /// </summary>
    Task<Result<string, DomainError>> GenerarInformeEstudiantesHtmlAsync(
        IEnumerable<Estudiante> estudiantes,
        bool mostrarEliminado = false,
        bool mostrarMenoresEdad = false);

    /// <summary>
    ///     Genera un informe HTML de docentes.
    /// </summary>
    Task<Result<string, DomainError>> GenerarInformeDocentesHtmlAsync(
        IEnumerable<Docente> docentes,
        bool mostrarEliminado = false);

    /// <summary>
    ///     Genera un listado HTML de todas las personas.
    /// </summary>
    Task<Result<string, DomainError>> GenerarListadoPersonasHtmlAsync(
        IEnumerable<Persona> personas,
        bool mostrarEliminado = false,
        bool mostrarMenoresEdad = false);

    /// <summary>
    ///     Guarda el informe HTML en un archivo.
    /// </summary>
    Task<Result<bool, DomainError>> GuardarInformeAsync(string html, string fileName);

    /// <summary>
    ///     Convierte el informe HTML a PDF y lo guarda.
    /// </summary>
    Task<Result<bool, DomainError>> GuardarInformePdfAsync(string html, string fileName);
}