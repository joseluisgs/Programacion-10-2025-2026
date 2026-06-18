using FluentAssertions;
using GestionAcademica.Back.Models.Academia;
using GestionAcademica.Errors.Storage;
using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Personas;
using GestionAcademica.Storage.Binary;

namespace GestionAcademica.Test.Storage.Binary;

[TestFixture]
public class AcademiaBinStorageTests {
    [SetUp]
    public void SetUp() {
        _storage = new AcademiaBinStorage();
        _tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.bin");
    }

    [TearDown]
    public void TearDown() {
        if (File.Exists(_tempPath)) File.Delete(_tempPath);
    }

    private AcademiaBinStorage _storage = null!;
    private string _tempPath = null!;

    [TestFixture]
    public class CasosPositivos {
        [SetUp]
        public void SetUp() {
            _storage = new AcademiaBinStorage();
            _tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.bin");
        }

        [TearDown]
        public void TearDown() {
            if (File.Exists(_tempPath)) File.Delete(_tempPath);
        }

        private AcademiaBinStorage _storage = null!;
        private string _tempPath = null!;

        [Test]
        public async Task Salvar_ConDatosValidos_DeberiaGuardarCorrectamente() {
            // Arrange
            var personas = new List<Persona> {
                new Estudiante {
                    Id = 1, Dni = "12345678A", Nombre = "Juan", Apellidos = "Pérez", Email = "juan@test.com",
                    Calificacion = 8.5, Ciclo = Ciclo.DAM, Curso = Curso.Primero
                },
                new Docente {
                    Id = 2, Dni = "87654321B", Nombre = "Ana", Apellidos = "García", Email = "ana@test.com",
                    Experiencia = 10, Especialidad = Modulos.Programacion, Ciclo = Ciclo.DAW
                }
            };

            // Act
            var resultado = await _storage.SalvarAsync(personas, _tempPath);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            File.Exists(_tempPath).Should().BeTrue();
        }

        [Test]
        public async Task Cargar_ConArchivoExistente_DeberiaRetornarDatos() {
            // Arrange
            var personas = new List<Persona> {
                new Docente {
                    Id = 2, Dni = "87654321B", Nombre = "Ana", Apellidos = "García", Email = "ana@test.com",
                    Experiencia = 10, Especialidad = Modulos.Programacion, Ciclo = Ciclo.DAW
                }
            };
            await _storage.SalvarAsync(personas, _tempPath);

            // Act
            var resultado = await _storage.CargarAsync(_tempPath);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().HaveCount(1);
            resultado.Value.First().Dni.Should().Be("87654321B");
            resultado.Value.First().Should().BeOfType<Docente>();
            (resultado.Value.First() as Docente)!.Experiencia.Should().Be(10);
        }
    }

    [TestFixture]
    public class CasosNegativos {
        [SetUp]
        public void SetUp() {
            _storage = new AcademiaBinStorage();
            _tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.bin");
        }

        [TearDown]
        public void TearDown() {
            if (File.Exists(_tempPath)) File.Delete(_tempPath);
        }

        private AcademiaBinStorage _storage = null!;
        private string _tempPath = null!;

        [Test]
        public async Task Cargar_CuandoArchivoNoExiste_DeberiaRetornarError() {
            // Arrange & Act
            var resultado = await _storage.CargarAsync("ruta/inexistente.bin");

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<StorageError.FileNotFound>();
            (resultado.Error as StorageError.FileNotFound)?.FilePath.Should().Be("ruta/inexistente.bin");
            resultado.Error.Message.Should().Contain("ruta/inexistente.bin");
        }

        [Test]
        public async Task Cargar_CuandoArchivoNoEsBinarioValido_DeberiaRetornarError() {
            // Arrange
            File.WriteAllText(_tempPath, "Este no es un archivo binario válido serializado");

            // Act
            var resultado = await _storage.CargarAsync(_tempPath);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<StorageError.InvalidFormat>();
            resultado.Error.Message.Should().Contain("formato del archivo es inválido");
        }

        [Test]
        public async Task Salvar_EnRutaInvalida_DeberiaRetornarError() {
            // Arrange
            var personas = new List<Persona>();

            // Act
            var resultado = await _storage.SalvarAsync(personas, "/ruta/invalida/archivo.bin");

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<StorageError.WriteError>();
            resultado.Error.Message.Should().Contain("Error al escribir");
        }
    }

    [TestFixture]
    public class CasosMixtos {
        [SetUp]
        public void SetUp() {
            _storage = new AcademiaBinStorage();
            _tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.bin");
        }

        [TearDown]
        public void TearDown() {
            if (File.Exists(_tempPath)) File.Delete(_tempPath);
        }

        private AcademiaBinStorage _storage = null!;
        private string _tempPath = null!;

        [Test]
        public async Task SalvarYLeer_RoundTrip_DeberiaMantenerDatos() {
            // Arrange
            var original = new List<Persona> {
                new Estudiante {
                    Id = 1, Dni = "11111111H", Nombre = "Ana", Apellidos = "López",
                    Email = "ana@test.com", Calificacion = 8.5, Ciclo = Ciclo.DAM, Curso = Curso.Primero
                },
                new Docente {
                    Id = 2, Dni = "22222222J", Nombre = "Pedro", Apellidos = "García",
                    Email = "pedro@test.com", Experiencia = 10, Especialidad = Modulos.Programacion, Ciclo = Ciclo.DAW
                }
            };

            // Act
            await _storage.SalvarAsync(original, _tempPath);
            var resultado = await _storage.CargarAsync(_tempPath);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().HaveCount(2);

            var estudiante = resultado.Value.First() as Estudiante;
            estudiante!.Dni.Should().Be("11111111H");

            var docente = resultado.Value.Last() as Docente;
            docente!.Dni.Should().Be("22222222J");
        }

        [Test]
        public async Task Salvar_ListaVacia_DeberiaCrearArchivoVacio() {
            // Arrange
            var personas = new List<Persona>();

            // Act
            var resultado = await _storage.SalvarAsync(personas, _tempPath);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            File.Exists(_tempPath).Should().BeTrue();
        }

        [Test]
        public async Task SalvarYLeer_MultiplesVeces_DeberiaMantenerConsistencia() {
            // Arrange
            var personas = new List<Persona> {
                new Estudiante { Id = 1, Dni = "11111111H", Nombre = "Ana", Calificacion = 9.0 }
            };

            // Act - Guardar y cargar múltiples veces
            await _storage.SalvarAsync(personas, _tempPath);
            var resultado1 = await _storage.CargarAsync(_tempPath);

            await _storage.SalvarAsync(resultado1.Value, _tempPath);
            var resultado2 = await _storage.CargarAsync(_tempPath);

            // Assert
            resultado1.IsSuccess.Should().BeTrue();
            resultado2.IsSuccess.Should().BeTrue();
            resultado2.Value.Should().HaveCount(1);
            resultado2.Value.First().Dni.Should().Be("11111111H");
        }
    }
}
