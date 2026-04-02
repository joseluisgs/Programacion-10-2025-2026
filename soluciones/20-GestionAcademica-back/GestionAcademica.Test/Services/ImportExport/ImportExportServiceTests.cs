using System.Globalization;
using CSharpFunctionalExtensions;
using FluentAssertions;
using GestionAcademica.Models.Personas;
using GestionAcademica.Services.ImportExport;
using GestionAcademica.Storage.Common;
using GestionAcademica.Errors.Common;
using NUnit.Framework;
using Moq;

namespace GestionAcademica.Test.Services.ImportExport;

[TestFixture]
public class ImportExportServiceTests
{
    private string _tempDir = null!;
    private ImportExportService _service = null!;
    private Mock<IStorage<Persona>> _storageMock = null!;

    [SetUp]
    public void SetUp()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"ImportExportTest_{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempDir);
        
        _storageMock = new Mock<IStorage<Persona>>();
        _service = new ImportExportService(_storageMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    [TestFixture]
    public class CasosPositivos : ImportExportServiceTests
    {
        [Test]
        public void ExportarDatos_ConPersonas_DeberiaRetornarContador()
        {
            // Arrange
            var personas = new List<Persona>
            {
                new Estudiante { Id = 1, Dni = "11111111A", Nombre = "Test1" },
                new Estudiante { Id = 2, Dni = "22222222B", Nombre = "Test2" },
                new Estudiante { Id = 3, Dni = "33333333C", Nombre = "Test3" }
            };
            
            var path = Path.Combine(_tempDir, "export.json");
            _storageMock.Setup(s => s.Salvar(It.IsAny<IEnumerable<Persona>>(), It.IsAny<string>()))
                .Returns(Result.Success<bool, DomainError>(true));

            // Act
            var resultado = _service.ExportarDatos(personas, path);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().Be(3);
        }

        [Test]
        public void ImportarDatos_ConArchivo_DeberiaRetornarPersonas()
        {
            // Arrange
            var personas = new List<Persona>
            {
                new Estudiante { Id = 1, Dni = "11111111A", Nombre = "Test" }
            };
            var path = Path.Combine(_tempDir, "import.json");
            
            _storageMock.Setup(s => s.Cargar(path))
                .Returns(Result.Success<IEnumerable<Persona>, DomainError>(personas));

            // Act
            var resultado = _service.ImportarDatos(path);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().HaveCount(1);
            resultado.Value.First().Dni.Should().Be("11111111A");
        }

        [Test]
        public void ExportarDatosSistema_DeberiaLLamarExportarDatosConRutaVacia()
        {
            // Arrange
            var personas = new List<Persona> { new Estudiante { Id = 1, Dni = "A" } };
            _storageMock.Setup(s => s.Salvar(It.IsAny<IEnumerable<Persona>>(), It.IsAny<string>()))
                .Returns(Result.Success<bool, DomainError>(true));

            // Act
            var resultado = _service.ExportarDatosSistema(personas);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            _storageMock.Verify(s => s.Salvar(It.IsAny<IEnumerable<Persona>>(), string.Empty), Times.Once);
        }

        [Test]
        public void ImportarDatosSistema_ConRuta_DeberiaLLamarImportarDatos()
        {
            // Arrange
            var path = Path.Combine(_tempDir, "test.json");
            var personas = new List<Persona> { new Estudiante { Id = 1 } };
            
            _storageMock.Setup(s => s.Cargar(path))
                .Returns(Result.Success<IEnumerable<Persona>, DomainError>(personas));

            // Act
            var resultado = _service.ImportarDatosSistema(path);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            _storageMock.Verify(s => s.Cargar(path), Times.Once);
        }

        [Test]
        public void ExportarDatos_ConListaVacia_DeberiaRetornarCero()
        {
            // Arrange
            var personas = new List<Persona>();
            var path = Path.Combine(_tempDir, "empty.json");
            
            _storageMock.Setup(s => s.Salvar(It.IsAny<IEnumerable<Persona>>(), It.IsAny<string>()))
                .Returns(Result.Success<bool, DomainError>(true));

            // Act
            var resultado = _service.ExportarDatos(personas, path);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().Be(0);
        }
    }

    [TestFixture]
    public class CasosNegativos : ImportExportServiceTests
    {
        [Test]
        public void ImportarDatos_ConError_DeberiaRetornarError()
        {
            // Arrange
            var path = Path.Combine(_tempDir, "no-existe.json");
            var error = new TestError("File not found");
            
            _storageMock.Setup(s => s.Cargar(path))
                .Returns(Result.Failure<IEnumerable<Persona>, DomainError>(error));

            // Act
            var resultado = _service.ImportarDatos(path);

            // Assert
            resultado.IsFailure.Should().BeTrue();
        }

        [Test]
        public void ExportarDatos_ConError_DeberiaRetornarError()
        {
            // Arrange
            var personas = new List<Persona> { new Estudiante { Id = 1 } };
            var path = Path.Combine(_tempDir, "error.json");
            var error = new TestError("Write error");
            
            _storageMock.Setup(s => s.Salvar(It.IsAny<IEnumerable<Persona>>(), It.IsAny<string>()))
                .Returns(Result.Failure<bool, DomainError>(error));

            // Act
            var resultado = _service.ExportarDatos(personas, path);

            // Assert
            resultado.IsFailure.Should().BeTrue();
        }

        private record TestError(string Message) : DomainError(Message);
    }
}