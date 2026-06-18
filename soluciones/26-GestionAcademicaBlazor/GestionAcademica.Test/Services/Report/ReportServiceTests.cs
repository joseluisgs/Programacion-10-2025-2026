using System.Globalization;
using FluentAssertions;
using GestionAcademica.Errors.Report;
using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Personas;
using GestionAcademica.Services.Report;

namespace GestionAcademica.Test.Services.Report;

[TestFixture]
public class ReportServiceTests {
    [SetUp]
    public void SetUp() {
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
    public void TearDown() {
        // Restaurar cultura
        CultureInfo.CurrentCulture = _originalCulture;
        CultureInfo.CurrentUICulture = _originalCulture;

        // Borrar todos los archivos y el directorio temporal
        if (Directory.Exists(_tempDirPath))
            try {
                Directory.Delete(_tempDirPath, true);
            }
            catch {
                /* Ignorar errores de limpieza */
            }
    }

    private ReportService _service = null!;
    private List<string> _tempFiles = null!;
    private CultureInfo _originalCulture = null!;
    private string _tempDirPath = null!;

    [TestFixture]
    public class CasosPositivos : ReportServiceTests {
        [Test]
        public async Task GenerarInformeEstudiantesHtml_DeberiaValidarLógicaDeEdadYFormatoEspañol() {
            // Arrange
            var hoy = DateTime.Today;
            var estudiantes = new List<Estudiante> {
                new() {
                    Dni = "11111111A", Nombre = "Adulto", Apellidos = "Test",
                    FechaNacimiento = hoy.AddYears(-20), Calificacion = 8.5
                },
                new() {
                    Dni = "22222222B", Nombre = "Menor", Apellidos = "Test",
                    FechaNacimiento = hoy.AddYears(-17), Calificacion = 4.0
                }
            };

            // Act
            var resultado = await _service.GenerarInformeEstudiantesHtmlAsync(estudiantes);

            // Assert
            var html = resultado.Value;
            html.Should().Contain("<td>Sí</td>"); // Adulto
            html.Should().Contain("<td>No</td>"); // Menor
            html.Should().Contain("8,5"); // Formato coma
            html.Should().Contain("4,0");
        }

        [Test]
        public async Task GenerarInformeEstudiantesHtml_ConBorradoLogico_DeberiaMostrarColumnaEliminado() {
            // Arrange
            var estudiantes = new List<Estudiante> {
                new() { Dni = "11111111A", IsDeleted = true },
                new() { Dni = "22222222B", IsDeleted = false }
            };

            // Act
            var resultado = await _service.GenerarInformeEstudiantesHtmlAsync(estudiantes, true);

            // Assert
            var html = resultado.Value;
            html.Should().Contain("<th>Eliminado</th>");
            html.Should().Contain("<td class=\"eliminado\">Sí</td>");
            html.Should().Contain("<td class=\"eliminado\">No</td>");
        }

        [Test]
        public async Task GuardarInforme_EnDirectorioTemporal_DeberiaPersistirCorrectamente() {
            // Arrange
            var html = "<html><body>Test en temporal</body></html>";
            var fileName = "informe_test.html";
            var expectedPath = Path.Combine(_tempDirPath, fileName);

            // Act
            var resultado = await _service.GuardarInformeAsync(html, fileName);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            File.Exists(expectedPath).Should().BeTrue();
            File.ReadAllText(expectedPath).Should().Be(html);
        }
    }

    [TestFixture]
    public class CasosNegativos : ReportServiceTests {
        [Test]
        public async Task GuardarInforme_ConNombreInvalido_DeberiaRetornarError() {
            // Act
            var resultado = await _service.GuardarInformeAsync("html", "archivo\0invalido.html");

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<ReportError.SaveError>();
            resultado.Error.Message.Should().Contain("Error al guardar el informe");
        }
    }

    [TestFixture]
    public class GenerarInformeEstudianteModelTests : ReportServiceTests {
        [Test]
        public async Task GenerarInformeEstudiante_ConEstudiantes_DeberiaCalcularEstadisticas() {
            // Arrange
            var estudiantes = new List<Estudiante> {
                new() { Id = 1, Calificacion = 8.0, Ciclo = Ciclo.DAM },
                new() { Id = 2, Calificacion = 4.0, Ciclo = Ciclo.DAM },
                new() { Id = 3, Calificacion = 9.0, Ciclo = Ciclo.DAW }
            };

            // Act
            var resultado = await _service.GenerarInformeEstudianteAsync(estudiantes, 5.0);

            // Assert
            resultado.Should().NotBeNull();
            resultado.TotalEstudiantes.Should().Be(3);
            resultado.Aprobados.Should().Be(2);
            resultado.Suspensos.Should().Be(1);
            resultado.NotaMedia.Should().BeApproximately(7.0, 0.1);
        }

        [Test]
        public async Task GenerarInformeEstudiante_SinEstudiantes_DeberiaRetornarVacio() {
            // Arrange
            var estudiantes = new List<Estudiante>();

            // Act
            var resultado = await _service.GenerarInformeEstudianteAsync(estudiantes, 5.0);

            // Assert
            resultado.Should().NotBeNull();
            resultado.TotalEstudiantes.Should().Be(0);
        }

        [Test]
        public async Task GenerarInformeEstudiante_ConFiltroCiclo_DeberiaFiltrar() {
            // Arrange
            var estudiantes = new List<Estudiante> {
                new() { Id = 1, Calificacion = 8.0, Ciclo = Ciclo.DAM },
                new() { Id = 2, Calificacion = 7.0, Ciclo = Ciclo.DAW }
            };

            // Act
            var resultado = await _service.GenerarInformeEstudianteAsync(estudiantes, 5.0, Ciclo.DAM);

            // Assert
            resultado.TotalEstudiantes.Should().Be(1);
        }
    }

    [TestFixture]
    public class GenerarInformeDocenteModelTests : ReportServiceTests {
        [Test]
        public async Task GenerarInformeDocente_ConDocentes_DeberiaCalcularEstadisticas() {
            // Arrange
            var docentes = new List<Docente> {
                new() { Id = 1, Experiencia = 10, Ciclo = Ciclo.DAM },
                new() { Id = 2, Experiencia = 5, Ciclo = Ciclo.DAW }
            };

            // Act
            var resultado = await _service.GenerarInformeDocenteAsync(docentes);

            // Assert
            resultado.Should().NotBeNull();
            resultado.TotalDocentes.Should().Be(2);
            resultado.ExperienciaMedia.Should().Be(7.5);
        }

        [Test]
        public async Task GenerarInformeDocente_SinDocentes_DeberiaRetornarVacio() {
            // Arrange
            var docentes = new List<Docente>();

            // Act
            var resultado = await _service.GenerarInformeDocenteAsync(docentes);

            // Assert
            resultado.Should().NotBeNull();
            resultado.TotalDocentes.Should().Be(0);
            resultado.ExperienciaMedia.Should().Be(0);
        }
    }

    [TestFixture]
    public class GenerarInformeDocentesHtmlTests : ReportServiceTests {
        [Test]
        public async Task GenerarInformeDocentesHtml_ConDocentes_DeberiaGenerarHtml() {
            // Arrange
            var docentes = new List<Docente> {
                new() { Dni = "12345678Z", Nombre = "Ana", Apellidos = "García", Experiencia = 10, Ciclo = Ciclo.DAM }
            };

            // Act
            var resultado = await _service.GenerarInformeDocentesHtmlAsync(docentes);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().Contain("Informe de Docentes");
            resultado.Value.Should().Contain("12345678Z");
            resultado.Value.Should().Contain("Ana");
        }

        [Test]
        public async Task GenerarInformeDocentesHtml_SinDocentes_DeberiaRetornarHtmlVacio() {
            // Arrange
            var docentes = new List<Docente>();

            // Act
            var resultado = await _service.GenerarInformeDocentesHtmlAsync(docentes);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().Contain("Informe de Docentes");
        }
    }

    [TestFixture]
    public class GenerarListadoPersonasHtmlTests : ReportServiceTests {
        [Test]
        public async Task GenerarListadoPersonasHtml_ConPersonas_DeberiaGenerarHtml() {
            // Arrange
            var personas = new List<Persona> {
                new Estudiante { Dni = "11111111A", Nombre = "Ana", Calificacion = 8.0 },
                new Docente { Dni = "22222222B", Nombre = "Pedro", Experiencia = 5 }
            };

            // Act
            var resultado = await _service.GenerarListadoPersonasHtmlAsync(personas);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().Contain("Listado de Personal");
            resultado.Value.Should().Contain("11111111A");
            resultado.Value.Should().Contain("22222222B");
        }
    }
}