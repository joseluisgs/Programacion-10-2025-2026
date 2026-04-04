using System.Text;
using GestionAcademica.Models.Personas;
using GestionAcademica.Models.Academia;
using GestionAcademica.Storage.Csv;
using GestionAcademica.Errors.Storage;
using NUnit.Framework;
using FluentAssertions;

namespace GestionAcademica.Test.Storage.Csv;

[TestFixture]
public class AcademiaCsvStorageTests
{
    private AcademiaCsvStorage _storage = null!;
    private string _tempPath = null!;

    [SetUp]
    public void SetUp()
    {
        _storage = new AcademiaCsvStorage();
        _tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.csv");
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
        private AcademiaCsvStorage _storage = null!;
        private string _tempPath = null!;

        [SetUp]
        public void SetUp()
        {
            _storage = new AcademiaCsvStorage();
            _tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.csv");
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
            (resultado.Value.First() as Estudiante)!.Calificacion.Should().Be(8.5);
        }
    }

    [TestFixture]
    public class CasosNegativos
    {
        private AcademiaCsvStorage _storage = null!;
        private string _tempPath = null!;

        [SetUp]
        public void SetUp()
        {
            _storage = new AcademiaCsvStorage();
            _tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.csv");
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
            var resultado = _storage.Cargar("ruta/inexistente.csv");

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<StorageError.FileNotFound>();
            (resultado.Error as StorageError.FileNotFound)?.FilePath.Should().Be("ruta/inexistente.csv");
            resultado.Error.Message.Should().Contain("ruta/inexistente.csv");
        }

        [Test]
        public void Cargar_CuandoArchivoTieneFormatoInvalido_DeberiaRetornarError()
        {
            // Arrange - usar separador incorrecto (el servicio espera ;)
            // Escribimos con comma pero el servicio espera punto y coma
            using var writer = new StreamWriter(_tempPath, false, Encoding.UTF8);
            writer.WriteLine("Id;Dni;Nombre;Apellidos;Email;Telefono;Imagen;FechaNacimiento;Ciclo;Curso;Tipo;Calificacion;Experiencia;IsDeleted;Direccion");
            writer.WriteLine("1;A;Juan;Perez;juan@test.com;123;img.png;2020-01-01;DAM;1;Estudiante;7.5;2024-01-01;false;Calle 1");
            writer.WriteLine("2;B;Ana;Garcia;ana@test.com;456;img2.jpg;2019-01-01;DAW;2;Docente;;2024-01-01;false;Calle 2");

            // Act
            var resultado = _storage.Cargar(_tempPath);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<StorageError.InvalidFormat>();
        }

        [Test]
        public void Salvar_EnRutaInvalida_DeberiaRetornarError()
        {
            // Arrange
            var personas = new List<Persona>();

            // Act
            var resultado = _storage.Salvar(personas, "/ruta/invalida/archivo.csv");

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<StorageError.WriteError>();
            resultado.Error.Message.Should().Contain("Error al escribir");
        }
    }

    [TestFixture]
    public class CasosMixtos
    {
        private AcademiaCsvStorage _storage = null!;
        private string _tempPath = null!;

        [SetUp]
        public void SetUp()
        {
            _storage = new AcademiaCsvStorage();
            _tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.csv");
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
                    Email = "ana@test.com", Calificacion = 8.5, Ciclo = Ciclo.DAM, Curso = Curso.Primero
                },
                new Docente 
                { 
                    Id = 2, Dni = "22222222J", Nombre = "Pedro", Apellidos = "García",
                    Email = "pedro@test.com", Experiencia = 10, Especialidad = Modulos.Programacion, Ciclo = Ciclo.DAW
                }
            };

            // Act
            _storage.Salvar(original, _tempPath);
            var resultado = _storage.Cargar(_tempPath);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().HaveCount(2);
            
            var estudiante = resultado.Value.First() as Estudiante;
            estudiante!.Dni.Should().Be("11111111H");
            
            var docente = resultado.Value.Last() as Docente;
            docente!.Dni.Should().Be("22222222J");
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
}
