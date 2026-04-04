using System.IO;
using System.Text;
using CSharpFunctionalExtensions;
using SelectPdf;
using GestionAcademica.Config;
using GestionAcademica.Enums;
using GestionAcademica.Errors.Common;
using GestionAcademica.Errors.Report;
using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Informes;
using GestionAcademica.Models.Personas;
using Serilog;

namespace GestionAcademica.Services.Report;

/// <summary>
/// Servicio para la generación de informes.
/// Genera informes en HTML y PDF de estudiantes, docentes y listados.
/// </summary>
public class ReportService : IReportService
{
    private readonly ILogger _logger = Log.ForContext<ReportService>();
    private readonly string _reportDirectory;
    private const string DateFormat = "dd/MM/yyyy";

    public ReportService(string reportDirectory)
    {
        _reportDirectory = reportDirectory;
        _logger.Debug("Inicializando la clase ReportService con directorio {Directory}", _reportDirectory);
    }

    private string FormatDate(DateTime date) => date.ToString(DateFormat);

    public InformeEstudiante GenerarInformeEstudiante(IEnumerable<Estudiante> estudiantes, double notaAprobado, Ciclo? ciclo = null, Curso? curso = null)
    {
        _logger.Information("Generando modelo informe de estudiantes. Ciclo: {Ciclo}, Curso: {Curso}", ciclo, curso);

        var filtered = estudiantes
            .Where(e => (ciclo == null || e.Ciclo == ciclo) && (curso == null || e.Curso == curso))
            .OrderByDescending(e => e.Calificacion)
            .ToList();

        var total = filtered.Count;
        if (total == 0) return new InformeEstudiante();

        return new InformeEstudiante
        {
            PorNota = filtered,
            TotalEstudiantes = total,
            Aprobados = filtered.Count(e => e.Calificacion >= notaAprobado),
            Suspensos = filtered.Count(e => e.Calificacion < notaAprobado),
            NotaMedia = filtered.Average(e => e.Calificacion)
        };
    }

    public InformeDocente GenerarInformeDocente(IEnumerable<Docente> docentes, Ciclo? ciclo = null)
    {
        _logger.Information("Generando modelo informe de docentes. Ciclo: {Ciclo}", ciclo);

        var filtered = docentes
            .Where(d => ciclo == null || d.Ciclo == ciclo)
            .OrderByDescending(d => d.Experiencia)
            .ToList();

        var total = filtered.Count;
        return new InformeDocente
        {
            PorExperiencia = filtered,
            TotalDocentes = total,
            ExperienciaMedia = total > 0 ? filtered.Average(d => d.Experiencia) : 0
        };
    }

    public Result<string, DomainError> GenerarInformeEstudiantesHtml(IEnumerable<Estudiante> estudiantes, bool mostrarEliminado = false, bool mostrarMenoresEdad = false)
    {
        _logger.Information("Generando informe HTML de estudiantes");
        
        try
        {
            var lista = estudiantes.ToList();
            var fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            var notaAprobado = AppConfig.NotaAprobado;
            var aprobados = lista.Count(e => e.Calificacion >= notaAprobado);
            var suspensos = lista.Count(e => e.Calificacion < notaAprobado);
            var media = lista.Count > 0 ? lista.Average(e => e.Calificacion) : 0;
            
            var headers = @"<th>DNI</th><th>Nombre</th><th>Apellidos</th><th>Fecha Nacimiento</th><th>Email</th><th>Mayor de edad</th><th>Ciclo</th><th>Curso</th><th class=""nota"">Nota</th><th class=""nota"">Estado</th>";
            if (mostrarMenoresEdad) headers += "<th>Menor de edad</th>";
            if (mostrarEliminado) headers += "<th>Eliminado</th>";

            var html = $@"
<!DOCTYPE html>
<html lang=""es"">
<head>
    <meta charset=""UTF-8"">
    <title>Informe de Estudiantes</title>
    <style>
        * {{ margin: 0; padding: 0; box-sizing: border-box; }}
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 0; padding: 20px; background-color: #f8f9fa; }}
        .container {{ max-width: 1400px; margin: 0 auto; background: white; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); overflow: hidden; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; }}
        .header h1 {{ font-size: 2.5em; margin-bottom: 10px; }}
        .stats {{ display: flex; justify-content: space-around; padding: 20px; background: #f8f9fa; border-bottom: 3px solid #667eea; }}
        .stat-box {{ text-align: center; padding: 15px; }}
        .stat-number {{ font-size: 2em; font-weight: bold; color: #667eea; }}
        .stat-number.aprobados {{ color: #28a745; }}
        .stat-number.suspensos {{ color: #dc3545; }}
        .stat-label {{ color: #666; margin-top: 5px; }}
        table {{ width: 100%; border-collapse: collapse; }}
        th {{ background-color: #667eea; color: white; padding: 12px; text-align: left; font-weight: 600; font-size: 0.9em; }}
        td {{ padding: 10px; border-bottom: 1px solid #eee; color: #555; font-size: 0.85em; }}
        tr:hover {{ background-color: #f5f5f5; }}
        .aprobado {{ color: #28a745; font-weight: bold; }}
        .suspenso {{ color: #dc3545; font-weight: bold; }}
        .nota {{ text-align: center; }}
        .eliminado {{ color: #dc3545; font-weight: bold; }}
        .footer {{ background-color: #333; color: white; padding: 20px; text-align: center; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>📊 Informe de Estudiantes</h1>
            <p>Fecha: {fecha}</p>
        </div>
        <div class=""stats"">
            <div class=""stat-box"">
                <div class=""stat-number"">{lista.Count}</div>
                <div class=""stat-label"">Total Estudiantes</div>
            </div>
            <div class=""stat-box"">
                <div class=""stat-number aprobados"">{aprobados}</div>
                <div class=""stat-label"">Aprobados</div>
            </div>
            <div class=""stat-box"">
                <div class=""stat-number suspensos"">{suspensos}</div>
                <div class=""stat-label"">Suspensos</div>
            </div>
            <div class=""stat-box"">
                <div class=""stat-number"">{media:F2}</div>
                <div class=""stat-label"">Nota Media</div>
            </div>
        </div>
        <table>
            <thead>
                <tr>
                    {headers}
                </tr>
            </thead>
            <tbody>";

            foreach (var e in lista.OrderByDescending(x => x.Calificacion))
            {
                var estado = e.Calificacion >= notaAprobado ? "aprobado" : "suspenso";
                var estadoTexto = e.Calificacion >= notaAprobado ? "Aprobado" : "Suspenso";
                var mayorEdad = e.IsMayorEdad ? "Sí" : "No";
                var menorEdad = e.IsMayorEdad ? "No" : "Sí";
                var fechaNacimiento = FormatDate(e.FechaNacimiento);
                var eliminado = e.IsDeleted ? "Sí" : "No";

                var row = $@"
                <tr>
                    <td>{e.Dni}</td>
                    <td>{e.Nombre}</td>
                    <td>{e.Apellidos}</td>
                    <td>{fechaNacimiento}</td>
                    <td>{e.Email}</td>
                    <td>{mayorEdad}</td>
                    <td>{e.Ciclo}</td>
                    <td>{e.Curso}</td>
                    <td class=""nota"">{e.Calificacion:F1}</td>
                    <td class=""{estado}"">{estadoTexto}</td>";
                
                if (mostrarMenoresEdad)
                    row += $"<td>{menorEdad}</td>";
                
                if (mostrarEliminado)
                    row += $@"<td class=""eliminado"">{eliminado}</td>";
                
                row += "</tr>";
                html += row;
            }

            html += @"
            </tbody>
        </table>
        <div class=""footer"">
            <p>Sistema de Gestión Académica - DAW Academy</p>
        </div>
    </div>
</body>
</html>";

            _logger.Information("Informe HTML de estudiantes generado correctamente");
            return Result.Success<string, DomainError>(html);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al generar informe HTML de estudiantes");
            return Result.Failure<string, DomainError>(
                ReportErrors.GenerationError(ex.Message));
        }
    }

    public Result<string, DomainError> GenerarInformeDocentesHtml(IEnumerable<Docente> docentes, bool mostrarEliminado = false)
    {
        _logger.Information("Generando informe HTML de docentes");
        
        try
        {
            var lista = docentes.ToList();
            var fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            var media = lista.Count > 0 ? lista.Average(d => d.Experiencia) : 0;
            
            var headers = @"<th>DNI</th><th>Nombre</th><th>Apellidos</th><th>Fecha Nacimiento</th><th>Email</th><th>Especialidad</th><th>Ciclo</th><th class=""experiencia"">Experiencia</th>";
            if (mostrarEliminado) headers += "<th>Eliminado</th>";

            var html = $@"
<!DOCTYPE html>
<html lang=""es"">
<head>
    <meta charset=""UTF-8"">
    <title>Informe de Docentes</title>
    <style>
        * {{ margin: 0; padding: 0; box-sizing: border-box; }}
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 0; padding: 20px; background-color: #f8f9fa; }}
        .container {{ max-width: 1200px; margin: 0 auto; background: white; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); overflow: hidden; }}
        .header {{ background: linear-gradient(135deg, #11998e 0%, #38ef7d 100%); color: white; padding: 30px; text-align: center; }}
        .header h1 {{ font-size: 2.5em; margin-bottom: 10px; }}
        .stats {{ display: flex; justify-content: space-around; padding: 20px; background: #f8f9fa; border-bottom: 3px solid #11998e; }}
        .stat-box {{ text-align: center; padding: 15px; }}
        .stat-number {{ font-size: 2em; font-weight: bold; color: #11998e; }}
        .stat-label {{ color: #666; margin-top: 5px; }}
        table {{ width: 100%; border-collapse: collapse; }}
        th {{ background-color: #11998e; color: white; padding: 12px; text-align: left; font-weight: 600; }}
        td {{ padding: 12px; border-bottom: 1px solid #eee; color: #555; }}
        tr:hover {{ background-color: #f5f5f5; }}
        .experiencia {{ text-align: center; }}
        .eliminado {{ color: #dc3545; font-weight: bold; }}
        .footer {{ background-color: #333; color: white; padding: 20px; text-align: center; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>👨‍🏫 Informe de Docentes</h1>
            <p>Fecha: {fecha}</p>
        </div>
        <div class=""stats"">
            <div class=""stat-box"">
                <div class=""stat-number"">{lista.Count}</div>
                <div class=""stat-label"">Total Docentes</div>
            </div>
            <div class=""stat-box"">
                <div class=""stat-number"">{media:F1}</div>
                <div class=""stat-label"">Años Experiencia Media</div>
            </div>
        </div>
        <table>
            <thead>
                <tr>
                    {headers}
                </tr>
            </thead>
            <tbody>";

            foreach (var d in lista.OrderByDescending(x => x.Experiencia))
            {
                var fechaNacimiento = FormatDate(d.FechaNacimiento);
                var eliminado = d.IsDeleted ? "Sí" : "No";

                var row = $@"
                <tr>
                    <td>{d.Dni}</td>
                    <td>{d.Nombre}</td>
                    <td>{d.Apellidos}</td>
                    <td>{fechaNacimiento}</td>
                    <td>{d.Email}</td>
                    <td>{d.Especialidad}</td>
                    <td>{d.Ciclo}</td>
                    <td class=""experiencia"">{d.Experiencia} años</td>";
                
                if (mostrarEliminado)
                    row += $@"<td class=""eliminado"">{eliminado}</td>";
                
                row += "</tr>";
                html += row;
            }

            html += @"
            </tbody>
        </table>
        <div class=""footer"">
            <p>Sistema de Gestión Académica - DAW Academy</p>
        </div>
    </div>
</body>
</html>";

            _logger.Information("Informe HTML de docentes generado correctamente");
            return Result.Success<string, DomainError>(html);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al generar informe HTML de docentes");
            return Result.Failure<string, DomainError>(
                ReportErrors.GenerationError(ex.Message));
        }
    }

    public Result<string, DomainError> GenerarListadoPersonasHtml(IEnumerable<Persona> personas, bool mostrarEliminado = false, bool mostrarMenoresEdad = false)
    {
        _logger.Information("Generando listado HTML de personas");
        
        try
        {
            var lista = personas.ToList();
            var fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            
            var headers = @"<th>ID</th><th>DNI</th><th>Nombre</th><th>Apellidos</th><th>Fecha Nacimiento</th><th>Email</th><th class=""tipo"">Tipo</th>";
            var tieneEstudiantes = lista.OfType<Estudiante>().Any();
            var mostrarColumnaMenorEdad = mostrarMenoresEdad && tieneEstudiantes;
            if (mostrarColumnaMenorEdad) headers += "<th>Menor de edad</th>";
            if (mostrarEliminado) headers += "<th>Eliminado</th>";

            string html = $@"
<!DOCTYPE html>
<html lang=""es"">
<head>
    <meta charset=""UTF-8"">
    <title>Listado de Personal</title>
    <style>
        * {{ margin: 0; padding: 0; box-sizing: border-box; }}
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 0; padding: 20px; background-color: #f8f9fa; }}
        .container {{ max-width: 1400px; margin: 0 auto; background: white; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); overflow: hidden; }}
        .header {{ background: linear-gradient(135deg, #ee0979 0%, #ff6a00 100%); color: white; padding: 30px; text-align: center; }}
        .header h1 {{ font-size: 2.5em; margin-bottom: 10px; }}
        .stats {{ padding: 20px; background: #f8f9fa; border-bottom: 3px solid #ee0979; text-align: center; }}
        .stat-number {{ font-size: 2em; font-weight: bold; color: #ee0979; }}
        .stat-label {{ color: #666; margin-top: 5px; }}
        table {{ width: 100%; border-collapse: collapse; }}
        th {{ background-color: #ee0979; color: white; padding: 12px; text-align: left; font-weight: 600; }}
        td {{ padding: 12px; border-bottom: 1px solid #eee; color: #555; }}
        tr:hover {{ background-color: #f5f5f5; }}
        .estudiante {{ background-color: #e3f2fd; }}
        .docente {{ background-color: #e8f5e9; }}
        .tipo {{ text-align: center; font-weight: bold; }}
        .eliminado {{ color: #dc3545; font-weight: bold; }}
        .footer {{ background-color: #333; color: white; padding: 20px; text-align: center; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>👥 Listado de Personal</h1>
            <p>Fecha: {fecha}</p>
        </div>
        <div class=""stats"">
            <div class=""stat-number"">{lista.Count}</div>
            <div class=""stat-label"">Total Personas</div>
        </div>
        <table>
            <thead>
                <tr>
                    {headers}
                </tr>
            </thead>
            <tbody>";

            foreach (var p in lista.OrderBy(x => x.Apellidos).ThenBy(x => x.Nombre))
            {
                var claseFila = p is Estudiante ? "estudiante" : "docente";
                var tipoTexto = p is Estudiante ? "🎓 Estudiante" : "👨‍🏫 Docente";
                var fechaNacimiento = FormatDate(p.FechaNacimiento);
                var eliminado = p.IsDeleted ? "Sí" : "No";
                
                var row = $@"
                <tr class=""{claseFila}"">
                    <td>{p.Id}</td>
                    <td>{p.Dni}</td>
                    <td>{p.Nombre}</td>
                    <td>{p.Apellidos}</td>
                    <td>{fechaNacimiento}</td>
                    <td>{p.Email}</td>
                    <td class=""tipo"">{tipoTexto}</td>";
                
                if (mostrarColumnaMenorEdad)
                {
                    if (p is Estudiante e)
                        row += $"<td>{(e.IsMayorEdad ? "No" : "Sí")}</td>";
                    else
                        row += "<td>-</td>";
                }
                
                if (mostrarEliminado)
                    row += $@"<td class=""eliminado"">{eliminado}</td>";
                
                row += "</tr>";
                html += row;
            }

            html += @"
            </tbody>
        </table>
        <div class=""footer"">
            <p>Sistema de Gestión Académica - DAW Academy</p>
        </div>
    </div>
</body>
</html>";

            _logger.Information("Listado HTML de personas generado correctamente");
            return Result.Success<string, DomainError>(html);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al generar listado HTML de personas");
            return Result.Failure<string, DomainError>(
                ReportErrors.GenerationError(ex.Message));
        }
    }

    public Result<bool, DomainError> GuardarInforme(string html, string fileName)
    {
        var directory = _reportDirectory;
        _logger.Information("Guardando informe en directorio {Directory}", directory);
        
        try
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var filePath = Path.Combine(directory, fileName);
            File.WriteAllText(filePath, html, Encoding.UTF8);
            
            _logger.Information("Informe guardado correctamente en {FilePath}", filePath);
            return Result.Success<bool, DomainError>(true);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al guardar informe");
            return Result.Failure<bool, DomainError>(
                ReportErrors.SaveError(ex.Message));
        }
    }

    public Result<bool, DomainError> GuardarInformePdf(string html, string fileName)
    {
        var directory = _reportDirectory;
        _logger.Information("Guardando informe PDF en directorio {Directory}", directory);
        
        try
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var filePath = Path.Combine(directory, fileName);
            
            var converter = new SelectPdf.HtmlToPdf();
            converter.Options.PdfPageSize = SelectPdf.PdfPageSize.A4;
            converter.Options.MarginTop = 10;
            converter.Options.MarginBottom = 10;
            converter.Options.MarginLeft = 10;
            converter.Options.MarginRight = 10;
            
            var doc = converter.ConvertHtmlString(html);
            doc.Save(filePath);
            doc.Close();
            
            _logger.Information("Informe PDF guardado correctamente en {FilePath}", filePath);
            return Result.Success<bool, DomainError>(true);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al guardar informe PDF");
            return Result.Failure<bool, DomainError>(
                ReportErrors.SaveError(ex.Message));
        }
    }
}
