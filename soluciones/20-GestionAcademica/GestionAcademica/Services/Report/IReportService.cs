using CSharpFunctionalExtensions;
using GestionAcademica.Enums;
using GestionAcademica.Errors.Common;
using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Informes;
using GestionAcademica.Models.Personas;

namespace GestionAcademica.Services.Report;

/// <summary>
/// Define el contrato para la generación de informes del sistema.
/// </summary>
public interface IReportService
{
    /// <summary>
    /// Genera un informe estadístico de estudiantes.
    /// </summary>
    /// <param name="estudiantes">Enumerable de estudiantes.</param>
    /// <param name="notaAprobado">Nota mínima para aprobar.</param>
    /// <param name="ciclo">Filtro opcional por ciclo.</param>
    /// <param name="curso">Filtro opcional por curso.</param>
    /// <returns>Informe con estadísticas de estudiantes.</returns>
    InformeEstudiante GenerarInformeEstudiante(IEnumerable<Estudiante> estudiantes, double notaAprobado, Ciclo? ciclo = null, Curso? curso = null);

    /// <summary>
    /// Genera un informe estadístico de docentes.
    /// </summary>
    /// <param name="docentes">Enumerable de docentes.</param>
    /// <param name="ciclo">Filtro opcional por ciclo.</param>
    /// <returns>Informe con estadísticas de docentes.</returns>
    InformeDocente GenerarInformeDocente(IEnumerable<Docente> docentes, Ciclo? ciclo = null);

    /// <summary>
    /// Genera un informe HTML de estudiantes.
    /// </summary>
    /// <param name="estudiantes">Enumerable de estudiantes.</param>
    /// <param name="mostrarEliminado">Indica si incluye estudiantes eliminados.</param>
    /// <param name="mostrarMenoresEdad">Indica si incluye menores de edad.</param>
    /// <returns>
    /// Result con el HTML generado o error <see cref="Errors.Report.ReportErrors.GenerationError(string)"/>.
    /// </returns>
    Result<string, DomainError> GenerarInformeEstudiantesHtml(
        IEnumerable<Estudiante> estudiantes,
        bool mostrarEliminado = false,
        bool mostrarMenoresEdad = false);

    /// <summary>
    /// Genera un informe HTML de docentes.
    /// </summary>
    /// <param name="docentes">Enumerable de docentes.</param>
    /// <param name="mostrarEliminado">Indica si incluye docentes eliminados.</param>
    /// <returns>
    /// Result con el HTML generado o error <see cref="Errors.Report.ReportErrors.GenerationError(string)"/>.
    /// </returns>
    Result<string, DomainError> GenerarInformeDocentesHtml(
        IEnumerable<Docente> docentes,
        bool mostrarEliminado = false);

    /// <summary>
    /// Genera un listado HTML de todas las personas.
    /// </summary>
    /// <param name="personas">Enumerable de personas.</param>
    /// <param name="mostrarEliminado">Indica si incluye eliminados.</param>
    /// <param name="mostrarMenoresEdad">Indica si incluye menores de edad.</param>
    /// <returns>
    /// Result con el HTML generado o error <see cref="Errors.Report.ReportErrors.GenerationError(string)"/>.
    /// </returns>
    Result<string, DomainError> GenerarListadoPersonasHtml(
        IEnumerable<Persona> personas,
        bool mostrarEliminado = false,
        bool mostrarMenoresEdad = false);

    /// <summary>
    /// Guarda el informe HTML en un archivo.
    /// </summary>
    /// <param name="html">Contenido HTML.</param>
    /// <param name="fileName">Nombre del archivo.</param>
    /// <returns>
    /// Result con true si se guardó correctamente o error <see cref="Errors.Report.ReportErrors.StorageError(string)"/>.
    /// </returns>
    Result<bool, DomainError> GuardarInforme(string html, string fileName);

    /// <summary>
    /// Convierte el informe HTML a PDF y lo guarda.
    /// </summary>
    /// <param name="html">Contenido HTML.</param>
    /// <param name="fileName">Nombre del archivo PDF.</param>
    /// <returns>
    /// Result con true si se guardó correctamente o error <see cref="Errors.Report.ReportErrors.GenerationError(string)"/> o <see cref="Errors.Report.ReportErrors.StorageError(string)"/>.
    /// </returns>
    Result<bool, DomainError> GuardarInformePdf(string html, string fileName);
}
