using GestionAcademica.Models.Personas;
using GestionAcademica.Models.Academia;
using GestionAcademica.Storage.Binary;
using GestionAcademica.Errors.Storage;
using NUnit.Framework;
using FluentAssertions;

namespace GestionAcademica.Test.Storage.Binary;

[TestFixture]
public class AcademiaBinStorageTests
{
    private AcademiaBinStorage _storage = null!;
    private string _tempPath = null!;

    [SetUp]
    public void SetUp()
    {
        _storage = new AcademiaBinStorage();
        _tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.bin");
    }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(_tempPath))
        {
            File.Delete(_tempPath);
        }
    }

    [TestFixture]
    public class CasosPositivos
    {
        private AcademiaBinStorage _storage = null!;
        private string _tempPath = null!;

        [SetUp]
        public void SetUp()
        {
            _storage = new AcademiaBinStorage();
            _tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.bin");
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_tempPath))
            {
                File.Delete(_tempPath);
            }
        }

        [Test]
        public void Salvar_ConDatosValidos_DeberiaGuardarCorrectamente()
        {
            // Arrange
            var personas = new List<Persona>
            {
                new Estudiante { Id = 1, Dni = "12345678A", Nombre = "Juan", Apellidos = "Pérez", Email = "juan@test.com", Calificacion = 8.5, Ciclo = Ciclo.DAM, Curso = Curso.Primero },
                new Docente { Id = 2, Dni = "87654321B", Nombre = "Ana", Apellidos = "García", Email = "ana@test.com", Experiencia = 10, Especialidad = Modulos.Programacion, Ciclo = Ciclo.DAW }
            };

            // Act
            var resultado = _storage.Salvar(personas, _tempPath);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            File.Exists(_tempPath).Should().BeTrue();
        }

        [Test]
        public void Cargar_ConArchivoExistente_DeberiaRetornarDatos()
        {
            // Arrange
            var personas = new List<Persona>
            {
                new Docente { Id = 2, Dni = "87654321B", Nombre = "Ana", Apellidos = "García", Email = "ana@test.com", Experiencia = 10, Especialidad = Modulos.Programacion, Ciclo = Ciclo.DAW }
            };
            _storage.Salvar(personas, _tempPath);

            // Act
            var resultado = _storage.Cargar(_tempPath);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().HaveCount(1);
            resultado.Value.First().Dni.Should().Be("87654321B");
            resultado.Value.First().Should().BeOfType<Docente>();
            (resultado.Value.First() as Docente)!.Experiencia.Should().Be(10);
        }
    }

    [TestFixture]
    public class CasosNegativos
    {
        private AcademiaBinStorage _storage = null!;
        private string _tempPath = null!;

        [SetUp]
        public void SetUp()
        {
            _storage = new AcademiaBinStorage();
            _tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.bin");
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_tempPath))
            {
                File.Delete(_tempPath);
            }
        }

        [Test]
        public void Cargar_CuandoArchivoNoExiste_DeberiaRetornarError()
        {
            // Arrange & Act
            var resultado = _storage.Cargar("ruta/inexistente.bin");

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<StorageError.FileNotFound>();
            (resultado.Error as StorageError.FileNotFound)?.FilePath.Should().Be("ruta/inexistente.bin");
            resultado.Error.Message.Should().Contain("ruta/inexistente.bin");
        }

        [Test]
        public void Cargar_CuandoArchivoNoEsBinarioValido_DeberiaRetornarError()
        {
            // Arrange
            File.WriteAllText(_tempPath, "Este no es un archivo binario válido serializado");

            // Act
            var resultado = _storage.Cargar(_tempPath);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<StorageError.InvalidFormat>();
            resultado.Error.Message.Should().Contain("formato del archivo es inválido");
        }
    }
}
