using System.Globalization;
using GestionAcademica.Models.Personas;
using GestionAcademica.Models.Academia;
using GestionAcademica.Services.Report;
using GestionAcademica.Errors.Report;
using NUnit.Framework;
using FluentAssertions;

namespace GestionAcademica.Test.Services.Report;

[TestFixture]
public class ReportServiceTests
{
    private ReportService _service = null!;
    private List<string> _tempFiles = null!;
    private CultureInfo _originalCulture = null!;
    private string _tempDirPath = null!;

    [SetUp]
    public void SetUp()
    {
        // Crear un subdirectorio temporal único para esta ejecución de tests
        _tempDirPath = Path.Combine(Path.GetTempPath(), $"ReportTests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempDirPath);

        _service = new ReportService(_tempDirPath);
        _tempFiles = new List<string>();
        
        // Forzar cultura española para validación de formatos decimales y fechas
        _originalCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("es-ES");
        CultureInfo.CurrentUICulture = new CultureInfo("es-ES");
    }

    [TearDown]
    public void TearDown()
    {
        // Restaurar cultura
        CultureInfo.CurrentCulture = _originalCulture;
        CultureInfo.CurrentUICulture = _originalCulture;

        // Borrar todos los archivos y el directorio temporal
        if (Directory.Exists(_tempDirPath))
        {
            try { Directory.Delete(_tempDirPath, true); } catch { /* Ignorar errores de limpieza */ }
        }
    }

    [TestFixture]
    public class CasosPositivos : ReportServiceTests
    {
        [Test]
        public void GenerarInformeEstudiantesHtml_DeberiaValidarLógicaDeEdadYFormatoEspañol()
        {
            // Arrange
            var hoy = DateTime.Today;
            var estudiantes = new List<Estudiante>
            {
                new Estudiante { 
                    Dni = "11111111A", Nombre = "Adulto", Apellidos = "Test", 
                    FechaNacimiento = hoy.AddYears(-20), Calificacion = 8.5 
                },
                new Estudiante { 
                    Dni = "22222222B", Nombre = "Menor", Apellidos = "Test", 
                    FechaNacimiento = hoy.AddYears(-17), Calificacion = 4.0 
                }
            };

            // Act
            var resultado = _service.GenerarInformeEstudiantesHtml(estudiantes);

            // Assert
            var html = resultado.Value;
            html.Should().Contain("<td>Sí</td>"); // Adulto
            html.Should().Contain("<td>No</td>"); // Menor
            html.Should().Contain("8,5"); // Formato coma
            html.Should().Contain("4,0");
        }

        [Test]
        public void GenerarInformeEstudiantesHtml_ConBorradoLogico_DeberiaMostrarColumnaEliminado()
        {
            // Arrange
            var estudiantes = new List<Estudiante>
            {
                new Estudiante { Dni = "11111111A", IsDeleted = true },
                new Estudiante { Dni = "22222222B", IsDeleted = false }
            };

            // Act
            var resultado = _service.GenerarInformeEstudiantesHtml(estudiantes, mostrarEliminado: true);

            // Assert
            var html = resultado.Value;
            html.Should().Contain("<th>Eliminado</th>");
            html.Should().Contain("<td class=\"eliminado\">Sí</td>");
            html.Should().Contain("<td class=\"eliminado\">No</td>");
        }

        [Test]
        public void GuardarInforme_EnDirectorioTemporal_DeberiaPersistirCorrectamente()
        {
            // Arrange
            var html = "<html><body>Test en temporal</body></html>";
            var fileName = "informe_test.html";
            var expectedPath = Path.Combine(_tempDirPath, fileName);

            // Act
            var resultado = _service.GuardarInforme(html, fileName);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            File.Exists(expectedPath).Should().BeTrue();
            File.ReadAllText(expectedPath).Should().Be(html);
        }
    }

    [TestFixture]
    public class CasosNegativos : ReportServiceTests
    {
        [Test]
        public void GuardarInforme_ConNombreInvalido_DeberiaRetornarError()
        {
            // Act
            var resultado = _service.GuardarInforme("html", "archivo\0invalido.html");

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<ReportError.SaveError>();
            resultado.Error.Message.Should().Contain("Error al guardar el informe");
        }
    }

    [TestFixture]
    public class GenerarInformeEstudianteModelTests : ReportServiceTests
    {
        [Test]
        public void GenerarInformeEstudiante_ConEstudiantes_DeberiaCalcularEstadisticas()
        {
            // Arrange
            var estudiantes = new List<Estudiante>
            {
                new Estudiante { Id = 1, Calificacion = 8.0, Ciclo = Ciclo.DAM },
                new Estudiante { Id = 2, Calificacion = 4.0, Ciclo = Ciclo.DAM },
                new Estudiante { Id = 3, Calificacion = 9.0, Ciclo = Ciclo.DAW }
            };

            // Act
            var resultado = _service.GenerarInformeEstudiante(estudiantes, 5.0);

            // Assert
            resultado.Should().NotBeNull();
            resultado.TotalEstudiantes.Should().Be(3);
            resultado.Aprobados.Should().Be(2);
            resultado.Suspensos.Should().Be(1);
            resultado.NotaMedia.Should().BeApproximately(7.0, 0.1);
        }

        [Test]
        public void GenerarInformeEstudiante_SinEstudiantes_DeberiaRetornarVacio()
        {
            // Arrange
            var estudiantes = new List<Estudiante>();

            // Act
            var resultado = _service.GenerarInformeEstudiante(estudiantes, 5.0);

            // Assert
            resultado.Should().NotBeNull();
            resultado.TotalEstudiantes.Should().Be(0);
        }

        [Test]
        public void GenerarInformeEstudiante_ConFiltroCiclo_DeberiaFiltrar()
        {
            // Arrange
            var estudiantes = new List<Estudiante>
            {
                new Estudiante { Id = 1, Calificacion = 8.0, Ciclo = Ciclo.DAM },
                new Estudiante { Id = 2, Calificacion = 7.0, Ciclo = Ciclo.DAW }
            };

            // Act
            var resultado = _service.GenerarInformeEstudiante(estudiantes, 5.0, Ciclo.DAM);

            // Assert
            resultado.TotalEstudiantes.Should().Be(1);
        }
    }

    [TestFixture]
    public class GenerarInformeDocenteModelTests : ReportServiceTests
    {
        [Test]
        public void GenerarInformeDocente_ConDocentes_DeberiaCalcularEstadisticas()
        {
            // Arrange
            var docentes = new List<Docente>
            {
                new Docente { Id = 1, Experiencia = 10, Ciclo = Ciclo.DAM },
                new Docente { Id = 2, Experiencia = 5, Ciclo = Ciclo.DAW }
            };

            // Act
            var resultado = _service.GenerarInformeDocente(docentes);

            // Assert
            resultado.Should().NotBeNull();
            resultado.TotalDocentes.Should().Be(2);
            resultado.ExperienciaMedia.Should().Be(7.5);
        }

        [Test]
        public void GenerarInformeDocente_SinDocentes_DeberiaRetornarVacio()
        {
            // Arrange
            var docentes = new List<Docente>();

            // Act
            var resultado = _service.GenerarInformeDocente(docentes);

            // Assert
            resultado.Should().NotBeNull();
            resultado.TotalDocentes.Should().Be(0);
            resultado.ExperienciaMedia.Should().Be(0);
        }
    }

    [TestFixture]
    public class GenerarInformeDocentesHtmlTests : ReportServiceTests
    {
        [Test]
        public void GenerarInformeDocentesHtml_ConDocentes_DeberiaGenerarHtml()
        {
            // Arrange
            var docentes = new List<Docente>
            {
                new Docente { Dni = "12345678Z", Nombre = "Ana", Apellidos = "García", Experiencia = 10, Ciclo = Ciclo.DAM }
            };

            // Act
            var resultado = _service.GenerarInformeDocentesHtml(docentes);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().Contain("Informe de Docentes");
            resultado.Value.Should().Contain("12345678Z");
            resultado.Value.Should().Contain("Ana");
        }

        [Test]
        public void GenerarInformeDocentesHtml_SinDocentes_DeberiaRetornarHtmlVacio()
        {
            // Arrange
            var docentes = new List<Docente>();

            // Act
            var resultado = _service.GenerarInformeDocentesHtml(docentes);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().Contain("Informe de Docentes");
        }
    }

    [TestFixture]
    public class GenerarListadoPersonasHtmlTests : ReportServiceTests
    {
        [Test]
        public void GenerarListadoPersonasHtml_ConPersonas_DeberiaGenerarHtml()
        {
            // Arrange
            var personas = new List<Persona>
            {
                new Estudiante { Dni = "11111111A", Nombre = "Ana", Calificacion = 8.0 },
                new Docente { Dni = "22222222B", Nombre = "Pedro", Experiencia = 5 }
            };

            // Act
            var resultado = _service.GenerarListadoPersonasHtml(personas);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().Contain("Listado de Personal");
            resultado.Value.Should().Contain("11111111A");
            resultado.Value.Should().Contain("22222222B");
        }
    }
}
