using FluentAssertions;
using GestionAcademica.Services.Images;
using NUnit.Framework;

namespace GestionAcademica.Test.Services.Images;

/// <summary>
/// Tests para los nuevos métodos de validación de tamaño y dimensiones de ImageService.
/// </summary>
[TestFixture]
public class ImageServiceValidationTests
{
    private string _tempDir = null!;
    private string _imagesDir = null!;
    private ImageService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"ImageValidationTest_{Guid.NewGuid()}");
        _imagesDir = Path.Combine(_tempDir, "images");
        Directory.CreateDirectory(_tempDir);
        Directory.CreateDirectory(_imagesDir);

        _service = new ImageService(_imagesDir, new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" });
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    // ===================================================================
    // ValidateImageSize tests
    // ===================================================================

    [TestFixture]
    public class ValidateImageSizeTests : ImageServiceValidationTests
    {
        [Test]
        public void ValidateImageSize_ConArchivoPequeno_DeberiaRetornarTrue()
        {
            // Arrange
            var testFile = Path.Combine(_tempDir, "pequeno.png");
            File.WriteAllBytes(testFile, new byte[1024]); // 1 KB

            // Act
            var result = _service.ValidateImageSize(testFile, 5_242_880);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void ValidateImageSize_ConArchivoGrande_DeberiaRetornarFalse()
        {
            // Arrange
            var testFile = Path.Combine(_tempDir, "grande.png");
            File.WriteAllBytes(testFile, new byte[10_000_000]); // 10 MB

            // Act
            var result = _service.ValidateImageSize(testFile, 5_242_880);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void ValidateImageSize_ConArchivoExactamenteAlLimite_DeberiaRetornarTrue()
        {
            // Arrange
            var testFile = Path.Combine(_tempDir, "limite.png");
            File.WriteAllBytes(testFile, new byte[5_242_880]); // Exactamente 5 MB

            // Act
            var result = _service.ValidateImageSize(testFile, 5_242_880);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void ValidateImageSize_ConArchivoUnByteEncimaDeLimite_DeberiaRetornarFalse()
        {
            // Arrange
            var testFile = Path.Combine(_tempDir, "encima.png");
            File.WriteAllBytes(testFile, new byte[5_242_881]); // 5 MB + 1 byte

            // Act
            var result = _service.ValidateImageSize(testFile, 5_242_880);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void ValidateImageSize_ConArchivoNoExistente_DeberiaRetornarFalse()
        {
            // Act
            var result = _service.ValidateImageSize(Path.Combine(_tempDir, "no-existe.png"));

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void ValidateImageSize_ConLimitePersonalizado_DeberiaRespetarLimite()
        {
            // Arrange
            var testFile = Path.Combine(_tempDir, "test.png");
            File.WriteAllBytes(testFile, new byte[500]); // 500 bytes

            // Act
            var resultBajoLimite = _service.ValidateImageSize(testFile, 1024);
            var resultEncimaDeLimite = _service.ValidateImageSize(testFile, 100);

            // Assert
            resultBajoLimite.Should().BeTrue();
            resultEncimaDeLimite.Should().BeFalse();
        }
    }

    // ===================================================================
    // ValidateImageDimensions tests
    // ===================================================================

    [TestFixture]
    public class ValidateImageDimensionsTests : ImageServiceValidationTests
    {
        [Test]
        public void ValidateImageDimensions_ConArchivoNoExistente_DeberiaRetornarFalse()
        {
            // Act
            var result = _service.ValidateImageDimensions(Path.Combine(_tempDir, "no-existe.png"));

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void ValidateImageDimensions_ConPngPequeno_DeberiaRetornarTrue()
        {
            // Arrange: PNG with 100x100 px header
            var testFile = Path.Combine(_tempDir, "100x100.png");
            File.WriteAllBytes(testFile, CreatePngHeader(100, 100));

            // Act
            var result = _service.ValidateImageDimensions(testFile, 4096, 4096);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void ValidateImageDimensions_ConPngGrande_DeberiaRetornarFalse()
        {
            // Arrange: PNG with 5000x5000 px header (exceeds 4096)
            var testFile = Path.Combine(_tempDir, "5000x5000.png");
            File.WriteAllBytes(testFile, CreatePngHeader(5000, 5000));

            // Act
            var result = _service.ValidateImageDimensions(testFile, 4096, 4096);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void ValidateImageDimensions_ConPngExactamenteAlLimite_DeberiaRetornarTrue()
        {
            // Arrange: PNG with exactly 4096x4096 px
            var testFile = Path.Combine(_tempDir, "4096x4096.png");
            File.WriteAllBytes(testFile, CreatePngHeader(4096, 4096));

            // Act
            var result = _service.ValidateImageDimensions(testFile, 4096, 4096);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void ValidateImageDimensions_ConBmpPequeno_DeberiaRetornarTrue()
        {
            // Arrange: BMP with 200x150 px
            var testFile = Path.Combine(_tempDir, "200x150.bmp");
            File.WriteAllBytes(testFile, CreateBmpHeader(200, 150));

            // Act
            var result = _service.ValidateImageDimensions(testFile, 4096, 4096);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void ValidateImageDimensions_ConGifPequeno_DeberiaRetornarTrue()
        {
            // Arrange: GIF with 320x240 px
            var testFile = Path.Combine(_tempDir, "320x240.gif");
            File.WriteAllBytes(testFile, CreateGifHeader(320, 240));

            // Act
            var result = _service.ValidateImageDimensions(testFile, 4096, 4096);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void ValidateImageDimensions_ConArchivoPequenioSinCabecera_DeberiaRetornarTrue()
        {
            // Arrange: file too small to contain a valid header
            // ValidateImageDimensions is lenient when dimensions cannot be determined
            var testFile = Path.Combine(_tempDir, "truncado.png");
            File.WriteAllBytes(testFile, new byte[] { 0x89, 0x50 }); // Only 2 bytes

            // Act
            var result = _service.ValidateImageDimensions(testFile, 4096, 4096);

            // Assert: lenient - returns true when dimensions cannot be determined
            result.Should().BeTrue();
        }

        [Test]
        public void ValidateImageDimensions_ConLimitePersonalizado_DeberiaRespetarLimite()
        {
            // Arrange: PNG with 500x300 px
            var testFile = Path.Combine(_tempDir, "500x300.png");
            File.WriteAllBytes(testFile, CreatePngHeader(500, 300));

            // Act
            var resultBajoLimite = _service.ValidateImageDimensions(testFile, 1920, 1080);
            var resultEncimaDeLimite = _service.ValidateImageDimensions(testFile, 400, 400);

            // Assert
            resultBajoLimite.Should().BeTrue();
            resultEncimaDeLimite.Should().BeFalse();
        }
    }

    // ===================================================================
    // SaveImage integration tests - size and dimension validation in SaveImage
    // ===================================================================

    [TestFixture]
    public class SaveImageValidacionIntegradaTests : ImageServiceValidationTests
    {
        [Test]
        public void SaveImage_ConArchivoGrande_DeberiaRetornarErrorFileSizeTooLarge()
        {
            // Arrange
            var bigFile = Path.Combine(_tempDir, "grande.png");
            File.WriteAllBytes(bigFile, new byte[10_000_000]); // 10 MB > 5 MB limit

            // Act
            var result = _service.SaveImage(bigFile);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeOfType<GestionAcademica.Errors.Images.ImageError.FileSizeTooLarge>();
            result.Error.Message.Should().Contain("grande.png");
        }

        [Test]
        public void SaveImage_ConPngExcedeDimensiones_DeberiaRetornarErrorDimensionsTooLarge()
        {
            // Arrange: PNG with 5000x5000 px (exceeds 4096x4096)
            var bigImageFile = Path.Combine(_tempDir, "5000x5000.png");
            File.WriteAllBytes(bigImageFile, CreatePngHeader(5000, 5000));

            // Act
            var result = _service.SaveImage(bigImageFile);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeOfType<GestionAcademica.Errors.Images.ImageError.DimensionsTooLarge>();
            result.Error.Message.Should().Contain("5000");
        }
    }

    // ===================================================================
    // Helper methods for creating minimal image headers in tests
    // ===================================================================

    /// <summary>
    /// Creates the minimal PNG header bytes (signature + IHDR chunk) with the specified dimensions.
    /// The resulting file is not a valid PNG image, but our dimension reader only needs the header.
    /// </summary>
    private static byte[] CreatePngHeader(int width, int height)
    {
        var header = new byte[33];

        // PNG signature
        header[0] = 0x89; header[1] = 0x50; header[2] = 0x4E; header[3] = 0x47;
        header[4] = 0x0D; header[5] = 0x0A; header[6] = 0x1A; header[7] = 0x0A;

        // IHDR chunk length (13 bytes)
        header[8] = 0; header[9] = 0; header[10] = 0; header[11] = 13;

        // IHDR type
        header[12] = 0x49; header[13] = 0x48; header[14] = 0x44; header[15] = 0x52;

        // Width (big-endian)
        header[16] = (byte)(width >> 24); header[17] = (byte)(width >> 16);
        header[18] = (byte)(width >> 8);  header[19] = (byte)width;

        // Height (big-endian)
        header[20] = (byte)(height >> 24); header[21] = (byte)(height >> 16);
        header[22] = (byte)(height >> 8);  header[23] = (byte)height;

        // Bit depth (8), color type (2 = RGB), compression (0), filter (0), interlace (0)
        header[24] = 8; header[25] = 2; header[26] = 0; header[27] = 0; header[28] = 0;

        // CRC (4 bytes, zeroed - not validated by our reader)
        header[29] = 0; header[30] = 0; header[31] = 0; header[32] = 0;

        return header;
    }

    /// <summary>
    /// Creates a minimal BMP header with the specified dimensions.
    /// </summary>
    private static byte[] CreateBmpHeader(int width, int height)
    {
        var header = new byte[54]; // Standard BMP header size

        // Signature "BM"
        header[0] = 0x42; header[1] = 0x4D;

        // File size (placeholder)
        header[2] = 54; header[3] = 0; header[4] = 0; header[5] = 0;

        // Reserved
        header[6] = 0; header[7] = 0; header[8] = 0; header[9] = 0;

        // Pixel data offset (54)
        header[10] = 54; header[11] = 0; header[12] = 0; header[13] = 0;

        // BITMAPINFOHEADER size (40)
        header[14] = 40; header[15] = 0; header[16] = 0; header[17] = 0;

        // Width (little-endian)
        header[18] = (byte)width;        header[19] = (byte)(width >> 8);
        header[20] = (byte)(width >> 16); header[21] = (byte)(width >> 24);

        // Height (little-endian)
        header[22] = (byte)height;        header[23] = (byte)(height >> 8);
        header[24] = (byte)(height >> 16); header[25] = (byte)(height >> 24);

        // Color planes (1)
        header[26] = 1; header[27] = 0;

        // Bits per pixel (24)
        header[28] = 24; header[29] = 0;

        return header;
    }

    /// <summary>
    /// Creates a minimal GIF header with the specified dimensions.
    /// </summary>
    private static byte[] CreateGifHeader(int width, int height)
    {
        var header = new byte[13];

        // GIF signature "GIF89a"
        header[0] = 0x47; header[1] = 0x49; header[2] = 0x46;
        header[3] = 0x38; header[4] = 0x39; header[5] = 0x61;

        // Width (little-endian)
        header[6] = (byte)width;  header[7] = (byte)(width >> 8);

        // Height (little-endian)
        header[8] = (byte)height; header[9] = (byte)(height >> 8);

        // Packed field, background color index, aspect ratio
        header[10] = 0; header[11] = 0; header[12] = 0;

        return header;
    }
}
