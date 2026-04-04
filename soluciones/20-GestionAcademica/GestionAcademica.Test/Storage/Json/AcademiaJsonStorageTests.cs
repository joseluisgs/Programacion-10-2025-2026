using GestionAcademica.Models.Personas;
using GestionAcademica.Models.Academia;
using GestionAcademica.Storage.Json;
using GestionAcademica.Errors.Storage;
using NUnit.Framework;
using FluentAssertions;

namespace GestionAcademica.Test.Storage.Json;

[TestFixture]
public class AcademiaJsonStorageTests
{
    private AcademiaJsonStorage _storage = null!;
    private string _tempPath = null!;

    [SetUp]
    public void SetUp()
    {
        _storage = new AcademiaJsonStorage();
        _tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");
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
        private AcademiaJsonStorage _storage = null!;
        private string _tempPath = null!;

        [SetUp]
        public void SetUp()
        {
            _storage = new AcademiaJsonStorage();
            _tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");
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
                new Estudiante { Id = 1, Dni = "12345678A", Nombre = "Juan", Apellidos = "Pérez", Email = "juan@test.com", Calificacion = 8.5, Ciclo = Ciclo.DAM, Curso = Curso.Primero }
            };
            _storage.Salvar(personas, _tempPath);

            // Act
            var resultado = _storage.Cargar(_tempPath);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().HaveCount(1);
            resultado.Value.First().Dni.Should().Be("12345678A");
            resultado.Value.First().Should().BeOfType<Estudiante>();
        }

        [Test]
        public void Salvar_ListaVacia_DeberiaCrearArchivoVacio()
        {
            // Arrange
            var personas = new List<Persona>();

            // Act
            var resultado = _storage.Salvar(personas, _tempPath);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            File.Exists(_tempPath).Should().BeTrue();
        }
    }

    [TestFixture]
    public class CasosNegativos
    {
        private AcademiaJsonStorage _storage = null!;
        private string _tempPath = null!;

        [SetUp]
        public void SetUp()
        {
            _storage = new AcademiaJsonStorage();
            _tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");
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
            var resultado = _storage.Cargar("ruta/inexistente.json");

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<StorageError.FileNotFound>();
            (resultado.Error as StorageError.FileNotFound)?.FilePath.Should().Be("ruta/inexistente.json");
            resultado.Error.Message.Should().Contain("ruta/inexistente.json");
        }

        [Test]
        public void Salvar_EnRutaInvalida_DeberiaRetornarError()
        {
            // Arrange
            var personas = new List<Persona>();

            // Act
            var resultado = _storage.Salvar(personas, "/ruta/invalida/archivo.json");

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<StorageError.WriteError>();
            resultado.Error.Message.Should().Contain("Error al escribir");
        }
    }

    [TestFixture]
    public class CasosMixtos
    {
        private AcademiaJsonStorage _storage = null!;
        private string _tempPath = null!;

        [SetUp]
        public void SetUp()
        {
            _storage = new AcademiaJsonStorage();
            _tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");
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
        public void SalvarYLeer_RoundTrip_DeberiaMantenerDatos()
        {
            // Arrange
            var original = new List<Persona>
            {
                new Estudiante 
                { 
                    Id = 1, Dni = "11111111H", Nombre = "Ana", Apellidos = "López",
                    Email = "ana@test.com", Calificacion = 8.5, Ciclo = Ciclo.DAM, Curso = Curso.Primero,
                    FechaNacimiento = new DateTime(2000, 5, 15), Imagen = "foto.png",
                    IsDeleted = false, CreatedAt = DateTime.UtcNow
                },
                new Docente 
                { 
                    Id = 2, Dni = "22222222J", Nombre = "Pedro", Apellidos = "García",
                    Email = "pedro@test.com", Experiencia = 10, Especialidad = Modulos.Programacion, Ciclo = Ciclo.DAW,
                    FechaNacimiento = new DateTime(1985, 3, 20), Imagen = "profesor.jpg",
                    IsDeleted = false, CreatedAt = DateTime.UtcNow
                }
            };

            // Act
            var saveResult = _storage.Salvar(original, _tempPath);
            var loadResult = _storage.Cargar(_tempPath);

            // Assert
            saveResult.IsSuccess.Should().BeTrue();
            loadResult.IsSuccess.Should().BeTrue();
            loadResult.Value.Should().HaveCount(2);
            
            var estudiante = loadResult.Value.First() as Estudiante;
            estudiante.Should().NotBeNull();
            estudiante!.Dni.Should().Be("11111111H");
            estudiante.Nombre.Should().Be("Ana");
            estudiante.Calificacion.Should().Be(8.5);
            
            var docente = loadResult.Value.Last() as Docente;
            docente.Should().NotBeNull();
            docente!.Dni.Should().Be("22222222J");
            docente.Experiencia.Should().Be(10);
        }

        [Test]
        public void Salvar_ConEstudianteEliminado_DeberiaMantenerEstadoEliminado()
        {
            // Arrange
            var personas = new List<Persona>
            {
                new Estudiante 
                { 
                    Id = 1, Dni = "12345678H", Nombre = "Eliminado", 
                    IsDeleted = true, DeletedAt = DateTime.UtcNow 
                }
            };

            // Act
            _storage.Salvar(personas, _tempPath);
            var resultado = _storage.Cargar(_tempPath);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.First().IsDeleted.Should().BeTrue();
            resultado.Value.First().DeletedAt.Should().NotBeNull();
        }
    }
}
