using System.Globalization;
using CSharpFunctionalExtensions;
using FluentAssertions;
using GestionAcademica.Services.Images;
using GestionAcademica.Errors.Images;
using NUnit.Framework;

namespace GestionAcademica.Test.Services.Images;

[TestFixture]
public class ImageServiceTests
{
    private string _tempDir = null!;
    private string _imagesDir = null!;
    private ImageService _service = null!;
    private string[] _allowedExtensions = null!;

    [SetUp]
    public void SetUp()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"ImageTest_{Guid.NewGuid()}");
        _imagesDir = Path.Combine(_tempDir, "images");
        Directory.CreateDirectory(_imagesDir);
        
        _allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
        _service = new ImageService(_imagesDir, _allowedExtensions);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    [TestFixture]
    public class CasosPositivos : ImageServiceTests
    {
        [Test]
        public void SaveImage_ConArchivoValido_DeberiaGuardarYRetornarNombre()
        {
            // Arrange
            var testImagePath = Path.Combine(_tempDir, "test.png");
            File.WriteAllBytes(testImagePath, new byte[] { 0, 1, 2 });

            // Act
            var resultado = _service.SaveImage(testImagePath);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().EndWith(".png");
            File.Exists(Path.Combine(_imagesDir, resultado.Value)).Should().BeTrue();
        }

        [Test]
        public void DeleteImage_ConArchivoExistente_DeberiaEliminar()
        {
            // Arrange
            var fileName = "test.jpg";
            var filePath = Path.Combine(_imagesDir, fileName);
            File.WriteAllBytes(filePath, new byte[] { 1, 2, 3 });

            // Act
            var resultado = _service.DeleteImage(fileName);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            File.Exists(filePath).Should().BeFalse();
        }

        [Test]
        public void UpdateImage_ConArchivoExistente_DeberiaActualizar()
        {
            // Arrange
            var existingFile = "existing.png";
            var existingPath = Path.Combine(_imagesDir, existingFile);
            File.WriteAllBytes(existingPath, new byte[] { 0 });

            var newImage = Path.Combine(_tempDir, "new.png");
            File.WriteAllBytes(newImage, new byte[] { 1, 2, 3 });

            // Act
            var resultado = _service.UpdateImage(newImage, existingFile);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            File.ReadAllBytes(existingPath).Should().Equal(new byte[] { 1, 2, 3 });
        }

        [Test]
        public void IsValidImage_ConExtensionValida_DeberiaRetornarTrue()
        {
            // Arrange
            var testPath = Path.Combine(_tempDir, "test.jpg");

            // Act
            var resultado = _service.IsValidImage(testPath);

            // Assert
            resultado.Should().BeTrue();
        }

        [Test]
        public void IsValidImage_ConExtensionMinuscula_DeberiaRetornarTrue()
        {
            // Arrange
            var testPath = Path.Combine(_tempDir, "test.PNG");

            // Act
            var resultado = _service.IsValidImage(testPath);

            // Assert
            resultado.Should().BeTrue();
        }

        [Test]
        public void SaveImage_ConExtensionJpeg_DeberiaGuardar()
        {
            // Arrange
            var testPath = Path.Combine(_tempDir, "test.jpeg");
            File.WriteAllBytes(testPath, new byte[] { 0 });

            // Act
            var resultado = _service.SaveImage(testPath);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().EndWith(".jpeg");
        }

        [Test]
        public void DeleteImage_DeberiaRetornarTrue_EnCasoExitoso()
        {
            // Arrange
            var fileName = "imagen.gif";
            var filePath = Path.Combine(_imagesDir, fileName);
            File.WriteAllBytes(filePath, new byte[] { 0x89, 0x50, 0x4E });

            // Act
            var resultado = _service.DeleteImage(fileName);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().BeTrue();
        }

        [Test]
        public void SaveImage_ConBmp_DeberiaGuardarCorrectamente()
        {
            // Arrange
            var testPath = Path.Combine(_tempDir, "test.bmp");
            File.WriteAllBytes(testPath, new byte[] { 0 });

            // Act
            var resultado = _service.SaveImage(testPath);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().EndWith(".bmp");
        }

        [Test]
        public void SaveImage_ConGif_DeberiaGuardar()
        {
            // Arrange
            var testPath = Path.Combine(_tempDir, "test.gif");
            File.WriteAllBytes(testPath, new byte[] { 0 });

            // Act
            var resultado = _service.SaveImage(testPath);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().EndWith(".gif");
        }
    }

    [TestFixture]
    public class CasosNegativos : ImageServiceTests
    {
        [Test]
        public void SaveImage_ConArchivoNoExistente_DeberiaRetornarErrorNotFound()
        {
            // Arrange
            var noExiste = Path.Combine(_tempDir, "no-existe.png");

            // Act
            var resultado = _service.SaveImage(noExiste);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<ImageError.NotFound>();
            resultado.Error.Message.Should().Contain("no-existe.png");
        }

        [Test]
        public void SaveImage_ConExtensionInvalida_DeberiaRetornarError()
        {
            // Arrange
            var invalidPath = Path.Combine(_tempDir, "test.exe");
            File.WriteAllBytes(invalidPath, new byte[] { 0 });

            // Act
            var resultado = _service.SaveImage(invalidPath);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<ImageError.InvalidFormat>();
            resultado.Error.Message.Should().Contain(".exe");
        }

        [Test]
        public void DeleteImage_ConArchivoNoExistente_DeberiaRetornarErrorNotFound()
        {
            // Act
            var resultado = _service.DeleteImage("no-existe.jpg");

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<ImageError.NotFound>();
            resultado.Error.Message.Should().Contain("no-existe.jpg");
        }

        [Test]
        public void UpdateImage_ConArchivoOrigenNoExistente_DeberiaRetornarErrorNotFound()
        {
            // Arrange
            var existingFile = "existing.png";
            var existingPath = Path.Combine(_imagesDir, existingFile);
            File.WriteAllBytes(existingPath, new byte[] { 0 });

            var noExiste = Path.Combine(_tempDir, "no-existe.png");

            // Act
            var resultado = _service.UpdateImage(noExiste, existingFile);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<ImageError.NotFound>();
            resultado.Error.Message.Should().Contain("no-existe");
        }

        [Test]
        public void UpdateImage_ConArchivoDestinoNoExistente_DeberiaRetornarErrorNotFound()
        {
            // Arrange
            var newImage = Path.Combine(_tempDir, "new.png");
            File.WriteAllBytes(newImage, new byte[] { 0 });

            // Act
            var resultado = _service.UpdateImage(newImage, "no-existe.png");

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<ImageError.NotFound>();
            resultado.Error.Message.Should().Contain("no-existe");
        }

        [Test]
        public void IsValidImage_ConExtensionInvalida_DeberiaRetornarFalse()
        {
            // Arrange
            var testPath = Path.Combine(_tempDir, "test.exe");

            // Act
            var resultado = _service.IsValidImage(testPath);

            // Assert
            resultado.Should().BeFalse();
        }

        [Test]
        public void SaveImage_ConArchivoSinExtension_DeberiaRetornarError()
        {
            // Arrange
            var testPath = Path.Combine(_tempDir, "sin-extension");
            File.WriteAllBytes(testPath, new byte[] { 0 });

            // Act
            var resultado = _service.SaveImage(testPath);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<ImageError.InvalidFormat>();
        }

        [Test]
        public void SaveImage_ConExtensionPdf_DeberiaRetornarError()
        {
            // Arrange
            var testPath = Path.Combine(_tempDir, "documento.pdf");
            File.WriteAllBytes(testPath, new byte[] { 0 });

            // Act
            var resultado = _service.SaveImage(testPath);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<ImageError.InvalidFormat>();
            resultado.Error.Message.Should().Contain(".pdf");
        }

        [Test]
        public void UpdateImage_ConExtensionInvalida_DeberiaRetornarError()
        {
            // Arrange
            var existingFile = "existing.jpg";
            var existingPath = Path.Combine(_imagesDir, existingFile);
            File.WriteAllBytes(existingPath, new byte[] { 0 });

            var invalidSource = Path.Combine(_tempDir, "test.exe");
            File.WriteAllBytes(invalidSource, new byte[] { 0 });

            // Act
            var resultado = _service.UpdateImage(invalidSource, existingFile);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<ImageError.InvalidFormat>();
        }

        [Test]
        public void DeleteImage_ConRutaInvalida_DeberiaRetornarError()
        {
            // Act
            var resultado = _service.DeleteImage("..\\..\\..\\etc/passwd");

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<ImageError.NotFound>();
        }
    }

    [TestFixture]
    public class ValidacionesIntegradas : ImageServiceTests
    {
        [Test]
        public void SaveImage_ConImagenValidaSinExcederTamanio_DeberiaGuardar()
        {
            // Arrange
            var testPath = Path.Combine(_tempDir, "test.png");
            File.WriteAllBytes(testPath, new byte[] { 0 });

            // Act
            var result = _service.SaveImage(testPath);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Test]
        public void SaveImage_ConImagenValidaSinExcederDimensiones_DeberiaGuardar()
        {
            // Arrange
            var testPath = Path.Combine(_tempDir, "test.png");
            File.WriteAllBytes(testPath, new byte[] { 0 });

            // Act
            var result = _service.SaveImage(testPath);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Test]
        public void UpdateImage_ConImagenValidaQuePassaTodasLasValidaciones_DeberiaActualizar()
        {
            // Arrange
            var existingFile = "existing.png";
            var existingPath = Path.Combine(_imagesDir, existingFile);
            File.WriteAllBytes(existingPath, new byte[] { 0 });

            var newImage = Path.Combine(_tempDir, "new.png");
            File.WriteAllBytes(newImage, new byte[] { 1, 2, 3 });

            // Act
            var result = _service.UpdateImage(newImage, existingFile);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }
    }
}