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
}
