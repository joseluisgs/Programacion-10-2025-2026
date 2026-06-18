using FluentAssertions;
using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Personas;
using GestionAcademica.Services.ImportExport;

namespace GestionAcademica.Test.Services.ImportExport;

[TestFixture]
public class ImportExportServiceTests
{
    [SetUp]
    public void SetUp()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"ImportExportTest_{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempDir);
        _service = new ImportExportService();
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    private string _tempDir = null!;
    private ImportExportService _service = null!;

    private static List<Persona> CrearPersonas() => new()
    {
        new Estudiante
        {
            Id = 1, Dni = "11111111A", Nombre = "Test1", Apellidos = "Apellido1",
            Email = "t1@test.com", FechaNacimiento = new DateTime(2000, 1, 1),
            Ciclo = Ciclo.DAM, Curso = Curso.Primero, Calificacion = 7.5
        },
        new Docente
        {
            Id = 2, Dni = "22222222B", Nombre = "Test2", Apellidos = "Apellido2",
            Email = "t2@test.com", FechaNacimiento = new DateTime(1985, 5, 15),
            Ciclo = Ciclo.DAW, Experiencia = 10, Especialidad = "Programación"
        }
    };

    [TestFixture]
    public class Exportar : ImportExportServiceTests
    {
        [Test]
        public async Task Json_ConPersonas_DeberiaCrearArchivoValido()
        {
            var personas = CrearPersonas();
            var path = Path.Combine(_tempDir, "export.json");

            var resultado = await _service.ExportarDatosAsync(personas, path);

            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().Be(2);
            File.Exists(path).Should().BeTrue();
            var contenido = File.ReadAllText(path);
            contenido.Should().Contain("11111111A");
            contenido.Should().Contain("22222222B");
        }

        [Test]
        public async Task Csv_ConPersonas_DeberiaCrearArchivoValido()
        {
            var personas = CrearPersonas();
            var path = Path.Combine(_tempDir, "export.csv");

            var resultado = await _service.ExportarDatosAsync(personas, path);

            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().Be(2);
            File.Exists(path).Should().BeTrue();
            var contenido = File.ReadAllText(path);
            contenido.Should().Contain("11111111A");
            contenido.Should().Contain("22222222B");
            contenido.Should().StartWith("Id;Dni;Nombre");
        }

        [Test]
        public async Task ConListaVacia_DeberiaRetornarCero()
        {
            var path = Path.Combine(_tempDir, "empty.json");

            var resultado = await _service.ExportarDatosAsync(new List<Persona>(), path);

            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().Be(0);
            File.Exists(path).Should().BeTrue();
        }
    }

    [TestFixture]
    public class Importar : ImportExportServiceTests
    {
        [Test]
        public async Task Json_RoundTrip_DeberiaRetornarPersonas()
        {
            var originales = CrearPersonas();
            var path = Path.Combine(_tempDir, "roundtrip.json");
            await _service.ExportarDatosAsync(originales, path);

            var resultado = await _service.ImportarDatosAsync(path);

            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().HaveCount(2);
            resultado.Value.First().Dni.Should().Be("11111111A");
        }

        [Test]
        public async Task Csv_RoundTrip_DeberiaRetornarPersonas()
        {
            var originales = CrearPersonas();
            var path = Path.Combine(_tempDir, "roundtrip.csv");
            await _service.ExportarDatosAsync(originales, path);

            var resultado = await _service.ImportarDatosAsync(path);

            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().HaveCount(2);
            resultado.Value.First().Dni.Should().Be("11111111A");
        }

        [Test]
        public async Task ArchivoInexistente_DeberiaRetornarError()
        {
            var path = Path.Combine(_tempDir, "no-existe.json");

            var resultado = await _service.ImportarDatosAsync(path);

            resultado.IsFailure.Should().BeTrue();
        }
    }
}
