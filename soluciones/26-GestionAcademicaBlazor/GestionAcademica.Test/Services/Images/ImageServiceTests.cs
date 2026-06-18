using FluentAssertions;
using GestionAcademica.Errors.Images;
using GestionAcademica.Services.Images;

namespace GestionAcademica.Test.Services.Images;

[TestFixture]
public class ImageServiceTests {
    [SetUp]
    public void SetUp() {
        _tempDir = Path.Combine(Path.GetTempPath(), $"ImageTest_{Guid.NewGuid()}");
        _imagesDir = Path.Combine(_tempDir, "images");
        Directory.CreateDirectory(_imagesDir);

        _allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
        _service = new ImageService(_imagesDir, _allowedExtensions);
    }

    [TearDown]
    public void TearDown() {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    private string _tempDir = null!;
    private string _imagesDir = null!;
    private ImageService _service = null!;
    private string[] _allowedExtensions = null!;

    [TestFixture]
    public class CasosPositivos : ImageServiceTests {
        [Test]
        public async Task SaveImage_ConArchivoValido_DeberiaGuardarYRetornarNombre() {
            // Arrange: PNG con magic numbers válidos
            var testImagePath = Path.Combine(_tempDir, "test.png");
            File.WriteAllBytes(testImagePath, new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });

            // Act
            var resultado = await _service.SaveImageAsync(testImagePath);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().EndWith(".png");
            File.Exists(Path.Combine(_imagesDir, resultado.Value)).Should().BeTrue();
        }

        [Test]
        public async Task DeleteImage_ConArchivoExistente_DeberiaEliminar() {
            // Arrange
            var fileName = "test.jpg";
            var filePath = Path.Combine(_imagesDir, fileName);
            File.WriteAllBytes(filePath, new byte[] { 1, 2, 3 });

            // Act
            var resultado = await _service.DeleteImageAsync(fileName);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            File.Exists(filePath).Should().BeFalse();
        }

        [Test]
        public async Task UpdateImage_ConArchivoExistente_DeberiaActualizar() {
            // Arrange
            var existingFile = "existing.png";
            var existingPath = Path.Combine(_imagesDir, existingFile);
            File.WriteAllBytes(existingPath, new byte[] { 0 });

            var newImage = Path.Combine(_tempDir, "new.png");
            File.WriteAllBytes(newImage, new byte[] { 1, 2, 3 });

            // Act
            var resultado = await _service.UpdateImageAsync(newImage, existingFile);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            File.ReadAllBytes(existingPath).Should().Equal(1, 2, 3);
        }

        [Test]
        public async Task IsValidImage_ConExtensionValida_DeberiaRetornarTrue() {
            // Arrange
            var testPath = Path.Combine(_tempDir, "test.jpg");

            // Act
            var resultado = await _service.IsValidImageAsync(testPath);

            // Assert
            resultado.Should().BeTrue();
        }

        [Test]
        public async Task IsValidImage_ConExtensionMinuscula_DeberiaRetornarTrue() {
            // Arrange
            var testPath = Path.Combine(_tempDir, "test.PNG");

            // Act
            var resultado = await _service.IsValidImageAsync(testPath);

            // Assert
            resultado.Should().BeTrue();
        }

        [Test]
        public async Task SaveImage_ConExtensionJpeg_DeberiaGuardar() {
            // Arrange: JPEG con magic numbers válidos
            var testPath = Path.Combine(_tempDir, "test.jpeg");
            File.WriteAllBytes(testPath, new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 });

            // Act
            var resultado = await _service.SaveImageAsync(testPath);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().EndWith(".jpeg");
        }

        [Test]
        public async Task DeleteImage_DeberiaRetornarTrue_EnCasoExitoso() {
            // Arrange
            var fileName = "imagen.gif";
            var filePath = Path.Combine(_imagesDir, fileName);
            File.WriteAllBytes(filePath, new byte[] { 0x89, 0x50, 0x4E });

            // Act
            var resultado = await _service.DeleteImageAsync(fileName);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().BeTrue();
        }

        [Test]
        public async Task SaveImage_ConBmp_DeberiaGuardarCorrectamente() {
            // Arrange: BMP con magic numbers válidos
            var testPath = Path.Combine(_tempDir, "test.bmp");
            File.WriteAllBytes(testPath, new byte[] { 0x42, 0x4D, 0x00, 0x00 });

            // Act
            var resultado = await _service.SaveImageAsync(testPath);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().EndWith(".bmp");
        }

        [Test]
        public async Task SaveImage_ConGif_DeberiaGuardar() {
            // Arrange: GIF con magic numbers válidos
            var testPath = Path.Combine(_tempDir, "test.gif");
            File.WriteAllBytes(testPath, new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 });

            // Act
            var resultado = await _service.SaveImageAsync(testPath);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().EndWith(".gif");
        }
    }

    [TestFixture]
    public class CasosNegativos : ImageServiceTests {
        [Test]
        public async Task SaveImage_ConArchivoNoExistente_DeberiaRetornarErrorNotFound() {
            // Arrange
            var noExiste = Path.Combine(_tempDir, "no-existe.png");

            // Act
            var resultado = await _service.SaveImageAsync(noExiste);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<ImageError.NotFound>();
            resultado.Error.Message.Should().Contain("no-existe.png");
        }

        [Test]
        public async Task SaveImage_ConExtensionInvalida_DeberiaRetornarError() {
            // Arrange
            var invalidPath = Path.Combine(_tempDir, "test.exe");
            File.WriteAllBytes(invalidPath, new byte[] { 0 });

            // Act
            var resultado = await _service.SaveImageAsync(invalidPath);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<ImageError.InvalidFormat>();
            resultado.Error.Message.Should().Contain(".exe");
        }

        [Test]
        public async Task DeleteImage_ConArchivoNoExistente_DeberiaRetornarErrorNotFound() {
            // Act
            var resultado = await _service.DeleteImageAsync("no-existe.jpg");

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<ImageError.NotFound>();
            resultado.Error.Message.Should().Contain("no-existe.jpg");
        }

        [Test]
        public async Task UpdateImage_ConArchivoOrigenNoExistente_DeberiaRetornarErrorNotFound() {
            // Arrange
            var existingFile = "existing.png";
            var existingPath = Path.Combine(_imagesDir, existingFile);
            File.WriteAllBytes(existingPath, new byte[] { 0 });

            var noExiste = Path.Combine(_tempDir, "no-existe.png");

            // Act
            var resultado = await _service.UpdateImageAsync(noExiste, existingFile);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<ImageError.NotFound>();
            resultado.Error.Message.Should().Contain("no-existe");
        }

        [Test]
        public async Task UpdateImage_ConArchivoDestinoNoExistente_DeberiaRetornarErrorNotFound() {
            // Arrange
            var newImage = Path.Combine(_tempDir, "new.png");
            File.WriteAllBytes(newImage, new byte[] { 0 });

            // Act
            var resultado = await _service.UpdateImageAsync(newImage, "no-existe.png");

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<ImageError.NotFound>();
            resultado.Error.Message.Should().Contain("no-existe");
        }

        [Test]
        public async Task IsValidImage_ConExtensionInvalida_DeberiaRetornarFalse() {
            // Arrange
            var testPath = Path.Combine(_tempDir, "test.exe");

            // Act
            var resultado = await _service.IsValidImageAsync(testPath);

            // Assert
            resultado.Should().BeFalse();
        }

        [Test]
        public async Task SaveImage_ConArchivoSinExtension_DeberiaRetornarError() {
            // Arrange
            var testPath = Path.Combine(_tempDir, "sin-extension");
            File.WriteAllBytes(testPath, new byte[] { 0 });

            // Act
            var resultado = await _service.SaveImageAsync(testPath);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<ImageError.InvalidFormat>();
        }

        [Test]
        public async Task SaveImage_ConExtensionPdf_DeberiaRetornarError() {
            // Arrange
            var testPath = Path.Combine(_tempDir, "documento.pdf");
            File.WriteAllBytes(testPath, new byte[] { 0 });

            // Act
            var resultado = await _service.SaveImageAsync(testPath);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<ImageError.InvalidFormat>();
            resultado.Error.Message.Should().Contain(".pdf");
        }

        [Test]
        public async Task UpdateImage_ConExtensionInvalida_DeberiaRetornarError() {
            // Arrange
            var existingFile = "existing.jpg";
            var existingPath = Path.Combine(_imagesDir, existingFile);
            File.WriteAllBytes(existingPath, new byte[] { 0 });

            var invalidSource = Path.Combine(_tempDir, "test.exe");
            File.WriteAllBytes(invalidSource, new byte[] { 0 });

            // Act
            var resultado = await _service.UpdateImageAsync(invalidSource, existingFile);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<ImageError.InvalidFormat>();
        }

        [Test]
        public async Task DeleteImage_ConRutaInvalida_DeberiaRetornarError() {
            // Act
            var resultado = await _service.DeleteImageAsync("..\\..\\..\\etc/passwd");

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<ImageError.NotFound>();
        }
    }

    [TestFixture]
    public class ValidacionesIntegradas : ImageServiceTests {
        [Test]
        public async Task SaveImage_ConImagenValidaSinExcederTamanio_DeberiaGuardar() {
            // Arrange: PNG con magic numbers válidos
            var testPath = Path.Combine(_tempDir, "test.png");
            File.WriteAllBytes(testPath, new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });

            // Act
            var result = await _service.SaveImageAsync(testPath);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Test]
        public async Task SaveImage_ConImagenValidaSinExcederDimensiones_DeberiaGuardar() {
            // Arrange: PNG con magic numbers válidos
            var testPath = Path.Combine(_tempDir, "test.png");
            File.WriteAllBytes(testPath, new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });

            // Act
            var result = await _service.SaveImageAsync(testPath);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Test]
        public async Task UpdateImage_ConImagenValidaQuePassaTodasLasValidaciones_DeberiaActualizar() {
            // Arrange
            var existingFile = "existing.png";
            var existingPath = Path.Combine(_imagesDir, existingFile);
            File.WriteAllBytes(existingPath, new byte[] { 0 });

            var newImage = Path.Combine(_tempDir, "new.png");
            File.WriteAllBytes(newImage, new byte[] { 1, 2, 3 });

            // Act
            var result = await _service.UpdateImageAsync(newImage, existingFile);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }
    }
}
