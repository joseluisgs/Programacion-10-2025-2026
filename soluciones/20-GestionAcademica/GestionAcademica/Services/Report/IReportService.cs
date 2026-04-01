using CSharpFunctionalExtensions;
using GestionAcademica.Enums;
using GestionAcademica.Errors.Common;
using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Informes;
using GestionAcademica.Models.Personas;

namespace GestionAcademica.Services.Report;

public interface IReportService
{
    // Métodos de modelo informe
    InformeEstudiante GenerarInformeEstudiante(IEnumerable<Estudiante> estudiantes, double notaAprobado, Ciclo? ciclo = null, Curso? curso = null);
    InformeDocente GenerarInformeDocente(IEnumerable<Docente> docentes, Ciclo? ciclo = null);

    // Métodos HTML
    Result<string, DomainError> GenerarInformeEstudiantesHtml(
        IEnumerable<Estudiante> estudiantes,
        bool mostrarEliminado = false);

    Result<string, DomainError> GenerarInformeDocentesHtml(
        IEnumerable<Docente> docentes,
        bool mostrarEliminado = false);

    Result<string, DomainError> GenerarListadoPersonasHtml(
        IEnumerable<Persona> personas,
        bool mostrarEliminado = false);

    Result<bool, DomainError> GuardarInforme(string html, string fileName);

    Result<bool, DomainError> GuardarInformePdf(string html, string fileName);
}
