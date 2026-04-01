using System.Globalization;
using CSharpFunctionalExtensions;
using FluentAssertions;
using GestionAcademica.Models.Personas;
using GestionAcademica.Models.Academia;
using GestionAcademica.Services.Backup;
using GestionAcademica.Errors.Backup;
using GestionAcademica.Storage.Common;
using GestionAcademica.Errors.Common;
using NUnit.Framework;
using Moq;

namespace GestionAcademica.Test.Services.Backup;

[TestFixture]
public class BackupServiceTests
{
    private string _tempDir = null!;
    private string _backupDir = null!;
    private string _imagesDir = null!;
    private BackupService _service = null!;
    private Mock<IStorage<Persona>> _storageMock = null!;

    [SetUp]
    public void SetUp()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"BackupTest_{Guid.NewGuid()}");
        _backupDir = Path.Combine(_tempDir, "backups");
        _imagesDir = Path.Combine(_tempDir, "images");
        Directory.CreateDirectory(_backupDir);
        Directory.CreateDirectory(_imagesDir);
        
        _storageMock = new Mock<IStorage<Persona>>();
        
        _service = new BackupService(_storageMock.Object, _backupDir);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    [TestFixture]
    public class CasosPositivos : BackupServiceTests
    {
        [Test]
        public void RealizarBackup_ConListaVacia_DeberiaRetornarErrorCreationError()
        {
            // Arrange
            var personas = new List<Persona>();

            // Act
            var resultado = _service.RealizarBackup(personas);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<BackupError.CreationError>();
            resultado.Error.Message.Should().Contain("No hay datos para respaldar");
        }

        [Test]
        public void RealizarBackup_ConPersonasSinImagenes_DeberiaCrearZipConData()
        {
            // Arrange
            var personas = new List<Persona>
            {
                new Estudiante { Id = 1, Dni = "11111111A", Nombre = "Test", Apellidos = "Test", Calificacion = 8.5 }
            };
            
            _storageMock.Setup(s => s.Salvar(It.IsAny<IEnumerable<Persona>>(), It.IsAny<string>()))
                .Returns(Result.Success<bool, DomainError>(true));

            // Act
            var resultado = _service.RealizarBackup(personas);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().EndWith(".zip");
            File.Exists(resultado.Value).Should().BeTrue();
            _storageMock.Verify(s => s.Salvar(personas, It.Is<string>(p => p.EndsWith("personas.json"))), Times.Once);
        }

        [Test]
        public void RealizarBackup_ConPersonasConImagenes_DeberiaCopiarImagenesAZip()
        {
            // Arrange - crear imagen en la ruta que espera el servicio
            var imagen = "foto1.png";
            var dataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            var imagesFolder = Path.Combine(dataFolder, "images");
            Directory.CreateDirectory(imagesFolder);
            var imagenPath = Path.Combine(imagesFolder, imagen);
            File.WriteAllBytes(imagenPath, new byte[] { 1, 2, 3 });

            var personas = new List<Persona>
            {
                new Estudiante { Id = 1, Dni = "11111111A", Nombre = "Test", Imagen = imagen }
            };
            
            _storageMock.Setup(s => s.Salvar(It.IsAny<IEnumerable<Persona>>(), It.IsAny<string>()))
                .Returns(Result.Success<bool, DomainError>(true));

            // Act
            var resultado = _service.RealizarBackup(personas, _backupDir);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            
            // Verificar que la imagen está en el ZIP
            var zipPath = resultado.Value;
            using var zip = System.IO.Compression.ZipFile.OpenRead(zipPath);
            var imgEntry = zip.GetEntry($"img/{imagen}");
            imgEntry.Should().NotBeNull();
        }

        [Test]
        public void RealizarBackup_ConDirectorioCustom_DeberiaCrearEnEseDirectorio()
        {
            // Arrange
            var customDir = Path.Combine(_tempDir, "custom-backup");
            Directory.CreateDirectory(customDir);
            
            var personas = new List<Persona>
            {
                new Estudiante { Id = 1, Dni = "11111111A", Nombre = "Test" }
            };
            
            _storageMock.Setup(s => s.Salvar(It.IsAny<IEnumerable<Persona>>(), It.IsAny<string>()))
                .Returns(Result.Success<bool, DomainError>(true));

            // Act
            var resultado = _service.RealizarBackup(personas, customDir);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().StartWith(customDir);
            File.Exists(resultado.Value).Should().BeTrue();
        }

        [Test]
        public void RestaurarBackup_ConArchivoInexistente_DeberiaRetornarErrorFileNotFound()
        {
            // Arrange
            var archivoInexistente = Path.Combine(_tempDir, "no_existe.zip");

            // Act
            var resultado = _service.RestaurarBackup(archivoInexistente);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<BackupError.FileNotFound>();
            resultado.Error.Message.Should().Contain("no_existe.zip");
        }

        [Test]
        public void RestaurarBackup_ConZipValido_DeberiaRetornarPersonas()
        {
            // Arrange
            var personas = new List<Persona>
            {
                new Estudiante { Id = 1, Dni = "11111111A", Nombre = "Test", Calificacion = 8.5 }
            };
            
            _storageMock.Setup(s => s.Cargar(It.IsAny<string>()))
                .Returns(Result.Success<IEnumerable<Persona>, DomainError>(personas));

            // Crear ZIP manualmente
            var zipPath = Path.Combine(_backupDir, "test-back.zip");
            using (var zip = System.IO.Compression.ZipFile.Open(zipPath, System.IO.Compression.ZipArchiveMode.Create))
            {
                zip.CreateEntry("data/personas.json");
            }

            // Act
            var resultado = _service.RestaurarBackup(zipPath);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().HaveCount(1);
        }

        [Test]
        public void RestaurarBackup_ConImagenes_DeberiaRestaurarImagenes()
        {
            // Arrange
            var imagen = "foto1.png";
            var personas = new List<Persona>
            {
                new Estudiante { Id = 1, Dni = "11111111A", Nombre = "Test", Imagen = imagen }
            };
            
            var imagesDirRestore = Path.Combine(_tempDir, "restored-images");
            Directory.CreateDirectory(imagesDirRestore);
            
            _storageMock.Setup(s => s.Cargar(It.IsAny<string>()))
                .Returns(Result.Success<IEnumerable<Persona>, DomainError>(personas));

            // Crear ZIP con imagen
            var zipPath = Path.Combine(_backupDir, "test-back.zip");
            using (var zip = System.IO.Compression.ZipFile.Open(zipPath, System.IO.Compression.ZipArchiveMode.Create))
            {
                zip.CreateEntry("data/personas.json");
                var imgEntry = zip.CreateEntry("img/foto1.png");
                using var imgStream = imgEntry.Open();
                imgStream.Write(new byte[] { 1, 2, 3 }, 0, 3);
            }

            // Act
            var resultado = _service.RestaurarBackup(zipPath, imagesDirRestore);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            var restoredImagePath = Path.Combine(imagesDirRestore, "images", imagen);
            File.Exists(restoredImagePath).Should().BeTrue();
        }

        [Test]
        public void ListarBackups_SinBackups_DeberiaRetornarListaVacia()
        {
            // Act
            var resultado = _service.ListarBackups();

            // Assert
            resultado.Should().BeEmpty();
        }

        [Test]
        public void ListarBackups_ConUnBackup_DeberiaRetornarEseBackup()
        {
            // Arrange - crear un backup primero (sin imágenes)
            var persona = new Estudiante { Id = 1, Dni = "11111111A", Nombre = "Test", Imagen = null };
            _storageMock.Setup(s => s.Salvar(It.IsAny<IEnumerable<Persona>>(), It.IsAny<string>()))
                .Returns(Result.Success<bool, DomainError>(true));

            // Crear un backup
            var backup = _service.RealizarBackup(new[] { persona }, _backupDir);
            backup.IsSuccess.Should().BeTrue("El backup debe crearse exitosamente");

            // Act - Verificar ListarBackups
            var resultado = _service.ListarBackups().ToList();

            // Assert
            resultado.Should().HaveCount(1);
            resultado[0].Should().Be(backup.Value);
        }

        [Test]
        public void ListarBackups_ConDirectorioCustom_DeberiaBuscarEnEseDirectorio()
        {
            // Arrange
            var customDir = Path.Combine(_tempDir, "custom-list");
            Directory.CreateDirectory(customDir);
            
            var persona = new Estudiante { Id = 1, Dni = "A" };
            _storageMock.Setup(s => s.Salvar(It.IsAny<IEnumerable<Persona>>(), It.IsAny<string>()))
                .Returns(Result.Success<bool, DomainError>(true));
                
            _service.RealizarBackup(new[] { persona }, customDir);

            // Act - Sin directorio custom (debería estar vacío)
            var resultadoDefault = _service.ListarBackups().ToList();
            
            // Act - Con directorio custom
            var resultadoCustom = _service.ListarBackups(customDir).ToList();

            // Assert
            resultadoDefault.Should().BeEmpty();
            resultadoCustom.Should().HaveCount(1);
        }

        [Test]
        public void RestaurarBackupSistema_ConCallbackExitoso_DeberiaRetornarContador()
        {
            // Arrange
            var persona = new Estudiante { Id = 1, Dni = "11111111A", Nombre = "Test", Calificacion = 5.0 };
            var personas = new List<Persona> { persona };
            
            _storageMock.Setup(s => s.Cargar(It.IsAny<string>()))
                .Returns(Result.Success<IEnumerable<Persona>, DomainError>(personas));

            var zipPath = Path.Combine(_backupDir, "test-back.zip");
            using (var zip = System.IO.Compression.ZipFile.Open(zipPath, System.IO.Compression.ZipArchiveMode.Create))
            {
                zip.CreateEntry("data/personas.json");
            }

            Func<Persona, Result<Persona, DomainError>> callback = p => Result.Success<Persona, DomainError>(p);

            // Act
            var resultado = _service.RestaurarBackupSistema(zipPath, callback);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().Be(1);
        }

        [Test]
        public void RealizarBackup_ConVariasImagenes_DeberiaCopiarTodas()
        {
            // Arrange - crear imágenes en la ruta que espera el servicio
            var img1 = "img1.png";
            var img2 = "img2.jpg";
            var dataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            var imagesFolder = Path.Combine(dataFolder, "images");
            Directory.CreateDirectory(imagesFolder);
            File.WriteAllBytes(Path.Combine(imagesFolder, img1), new byte[] { 1 });
            File.WriteAllBytes(Path.Combine(imagesFolder, img2), new byte[] { 2 });

            var personas = new List<Persona>
            {
                new Estudiante { Id = 1, Dni = "11111111A", Imagen = img1 },
                new Estudiante { Id = 2, Dni = "22222222B", Imagen = img2 }
            };
            
            _storageMock.Setup(s => s.Salvar(It.IsAny<IEnumerable<Persona>>(), It.IsAny<string>()))
                .Returns(Result.Success<bool, DomainError>(true));

            // Act
            var resultado = _service.RealizarBackup(personas, _backupDir);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            using var zip = System.IO.Compression.ZipFile.OpenRead(resultado.Value);
            zip.GetEntry("img/img1.png").Should().NotBeNull();
            zip.GetEntry("img/img2.jpg").Should().NotBeNull();
        }
    }

    [TestFixture]
    public class CasosNegativos : BackupServiceTests
    {
        [Test]
        public void RealizarBackup_SinDirectorio_DeberiaLanzarExcepcion()
        {
            // Arrange - servicio sin directorio por defecto
            var serviceSinDirectorio = new BackupService(_storageMock.Object, null);
            var personas = new List<Persona> { new Estudiante { Id = 1, Dni = "A" } };

            // Act & Assert
            serviceSinDirectorio.Invoking(s => s.RealizarBackup(personas, null))
                .Should().Throw<InvalidOperationException>()
                .WithMessage("*directorio de backup*");
        }

        [Test]
        public void RestaurarBackup_ConArchivoCorrupto_DeberiaRetornarErrorInvalidBackupFile()
        {
            // Arrange
            var zipPath = Path.Combine(_backupDir, "corrupto.zip");
            File.WriteAllText(zipPath, "Esto no es un ZIP válido");

            // Act
            var resultado = _service.RestaurarBackup(zipPath);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<BackupError.InvalidBackupFile>();
        }

        [Test]
        public void RestaurarBackup_ConZipSinDataJson_DeberiaRetornarErrorInvalidBackupFile()
        {
            // Arrange
            var zipPath = Path.Combine(_backupDir, "sin-data.zip");
            using (var zip = System.IO.Compression.ZipFile.Open(zipPath, System.IO.Compression.ZipArchiveMode.Create))
            {
                zip.CreateEntry("otro-archivo.txt");
            }

            // Act
            var resultado = _service.RestaurarBackup(zipPath);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<BackupError.InvalidBackupFile>();
            resultado.Error.Message.Should().Contain("datos válidos");
        }

        [Test]
        public void RealizarBackup_ConErrorDeDirectorio_DeberiaRetornarErrorDirectoryError()
        {
            // Arrange - directorio inválido
            var service = new BackupService(_storageMock.Object, "C:\\:invalido\\ruta");
            var personas = new List<Persona> { new Estudiante { Id = 1, Dni = "A", Nombre = "T" } };

            // Act
            var resultado = service.RealizarBackup(personas, "C:\\:invalido\\ruta");

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<BackupError.DirectoryError>();
            resultado.Error.Message.Should().Contain("directorio");
        }

        [Test]
        public void RealizarBackup_ConErrorDeEscritura_DeberiaRetornarError()
        {
            // Arrange
            var personas = new List<Persona> { new Estudiante { Id = 1, Dni = "A" } };
            var error = new BackupError.CreationError("Error de escritura");
            
            _storageMock.Setup(s => s.Salvar(It.IsAny<IEnumerable<Persona>>(), It.IsAny<string>()))
                .Returns(Result.Failure<bool, DomainError>(error));

            // Act
            var resultado = _service.RealizarBackup(personas, _backupDir);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<BackupError.CreationError>();
        }

        [Test]
        public void RestaurarBackupSistema_ConCallbackFallido_DeberiaRetornarError()
        {
            // Arrange
            var persona = new Estudiante { Id = 1, Dni = "11111111A", Nombre = "Test" };
            var personas = new List<Persona> { persona };
            
            _storageMock.Setup(s => s.Cargar(It.IsAny<string>()))
                .Returns(Result.Success<IEnumerable<Persona>, DomainError>(personas));

            var zipPath = Path.Combine(_backupDir, "test.zip");
            using (var zip = System.IO.Compression.ZipFile.Open(zipPath, System.IO.Compression.ZipArchiveMode.Create))
            {
                zip.CreateEntry("data/personas.json");
            }

            Func<Persona, Result<Persona, DomainError>> callback = p => 
                Result.Failure<Persona, DomainError>(BackupErrors.CreationError("Error al crear persona"));

            // Act
            var resultado = _service.RestaurarBackupSistema(zipPath, callback);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Message.Should().Contain("crear persona");
        }

        [Test]
        public void RestaurarBackup_ConZipVacio_DeberiaRetornarError()
        {
            // Arrange
            var zipPath = Path.Combine(_backupDir, "vacio.zip");
            using (var zip = System.IO.Compression.ZipFile.Open(zipPath, System.IO.Compression.ZipArchiveMode.Create))
            {
                // Zip vacío sin entradas
            }

            // Act
            var resultado = _service.RestaurarBackup(zipPath);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<BackupError.InvalidBackupFile>();
        }

        [Test]
        public void RestaurarBackup_ConImagenNoEncontrada_DeberiaContinuar()
        {
            // Arrange - persona con imagen que no existe en el ZIP
            var personas = new List<Persona>
            {
                new Estudiante { Id = 1, Dni = "11111111A", Imagen = "no-existe.png" }
            };
            
            _storageMock.Setup(s => s.Cargar(It.IsAny<string>()))
                .Returns(Result.Success<IEnumerable<Persona>, DomainError>(personas));

            var zipPath = Path.Combine(_backupDir, "test.zip");
            using (var zip = System.IO.Compression.ZipFile.Open(zipPath, System.IO.Compression.ZipArchiveMode.Create))
            {
                zip.CreateEntry("data/personas.json");
                // No hay img/no-existe.png
            }

            // Act
            var resultado = _service.RestaurarBackup(zipPath);

            // Assert - debe seguir funcionando, solo avisa con warning
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().HaveCount(1);
        }
    }
}