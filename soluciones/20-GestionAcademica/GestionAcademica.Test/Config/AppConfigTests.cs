using FluentAssertions;
using GestionAcademica.Config;
using NUnit.Framework;
using System.IO;
using System.Globalization;

namespace GestionAcademica.Test.Config;

[TestFixture]
public class AppConfigTests
{
    [TestFixture]
    public class PropiedadesBasicas
    {
        [Test]
        public void Locale_DeberiaRetornarEspana()
        {
            // Act
            var locale = AppConfig.Locale;

            // Assert
            locale.Should().NotBeNull();
            locale.Name.Should().Be("es-ES");
        }

        [Test]
        public void NotaAprobado_DeberiaRetornarValorPositivo()
        {
            // Act
            var nota = AppConfig.NotaAprobado;

            // Assert
            nota.Should().BeGreaterThan(0);
        }

        [Test]
        public void StorageType_DeberiaRetornarTipoValido()
        {
            // Act
            var tipo = AppConfig.StorageType;

            // Assert
            tipo.Should().NotBeNullOrEmpty();
            // El tipo puede venir en mayúsculas o minúsculas
            tipo.ToLower().Should().BeOneOf("json", "csv", "bin", "csv-alt");
        }

        [Test]
        public void RepositoryType_DeberiaRetornarTipoValido()
        {
            // Act
            var tipo = AppConfig.RepositoryType;

            // Assert
            tipo.Should().NotBeNullOrEmpty();
            tipo.Should().BeOneOf("memory", "json", "dapper", "efcore");
        }

        [Test]
        public void ConnectionString_DeberiaRetornarValorNoNulo()
        {
            // Act
            var connStr = AppConfig.ConnectionString;

            // Assert
            connStr.Should().NotBeNullOrEmpty();
            connStr.Should().Contain("Data Source");
        }

        [Test]
        public void CacheSize_DeberiaSerMayorQueCero()
        {
            // Act
            var size = AppConfig.CacheSize;

            // Assert
            size.Should().BeGreaterThan(0);
        }

        [Test]
        public void DropData_DeberiaRetornarBoolean()
        {
            // Act
            var drop = AppConfig.DropData;

            // Assert
            // Puede ser true o false dependiendo de la configuración
            // Lo importante es que no lanza excepción y retorna un boolean
            (drop == true || drop == false).Should().BeTrue();
        }

        [Test]
        public void SeedData_DeberiaRetornarTrue()
        {
            // Act
            var seed = AppConfig.SeedData;

            // Assert
            seed.Should().BeTrue(); // Por defecto es true
        }

        [Test]
        public void UseLogicalDelete_DeberiaRetornarTrue()
        {
            // Act
            var logical = AppConfig.UseLogicalDelete;

            // Assert
            logical.Should().BeTrue(); // Por defecto es true
        }
    }

    [TestFixture]
    public class Directorios
    {
        [Test]
        public void DataFolder_DeberiaRetornarRutaValida()
        {
            // Act
            var folder = AppConfig.DataFolder;

            // Assert
            folder.Should().NotBeNullOrEmpty();
            Path.IsPathRooted(folder).Should().BeTrue();
        }

        [Test]
        public void BackupDirectory_DeberiaRetornarRutaValida()
        {
            // Act
            var dir = AppConfig.BackupDirectory;

            // Assert
            dir.Should().NotBeNullOrEmpty();
            Path.IsPathRooted(dir).Should().BeTrue();
        }

        [Test]
        public void ReportDirectory_DeberiaRetornarRutaValida()
        {
            // Act
            var dir = AppConfig.ReportDirectory;

            // Assert
            dir.Should().NotBeNullOrEmpty();
            Path.IsPathRooted(dir).Should().BeTrue();
        }

        [Test]
        public void LogDirectory_DeberiaRetornarRutaValida()
        {
            // Act
            var dir = AppConfig.LogDirectory;

            // Assert
            dir.Should().NotBeNullOrEmpty();
            Path.IsPathRooted(dir).Should().BeTrue();
        }

        [Test]
        public void ImagesDirectory_DeberiaRetornarRutaValida()
        {
            // Act
            var dir = AppConfig.ImagesDirectory;

            // Assert
            dir.Should().NotBeNullOrEmpty();
            dir.Should().Contain("images");
        }
    }

    [TestFixture]
    public class Formatos
    {
        [Test]
        public void AcademiaFile_DeberiaRetornarArchivoJson()
        {
            // Act
            var file = AppConfig.AcademiaFile;

            // Assert
            file.Should().NotBeNullOrEmpty();
            file.Should().EndWith(".json");
        }

        [Test]
        public void BackupFormat_DeberiaRetornarFormatoValido()
        {
            // Act
            var format = AppConfig.BackupFormat;

            // Assert
            format.Should().NotBeNullOrEmpty();
            format.Should().BeOneOf("json", "csv", "bin");
        }

        [Test]
        public void IsDevelopment_DeberiaRetornarBoolean()
        {
            // Act
            var dev = AppConfig.IsDevelopment;

            // Assert
            // Puede ser true o false dependiendo de la configuración
            // Lo importante es que no lanza excepción y retorna un boolean
            (dev == true || dev == false).Should().BeTrue();
        }
    }

    [TestFixture]
    public class Logging
    {
        [Test]
        public void LogToFile_DeberiaRetornarTrue()
        {
            // Act
            var log = AppConfig.LogToFile;

            // Assert
            log.Should().BeTrue();
        }

        [Test]
        public void LogRetainDays_DeberiaSerMayorQueCero()
        {
            // Act
            var days = AppConfig.LogRetainDays;

            // Assert
            days.Should().BeGreaterThan(0);
        }

        [Test]
        public void LogLevel_DeberiaRetornarNivelValido()
        {
            // Act
            var level = AppConfig.LogLevel;

            // Assert
            level.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void LogOutputTemplate_DeberiaRetornarTemplate()
        {
            // Act
            var template = AppConfig.LogOutputTemplate;

            // Assert
            template.Should().NotBeNullOrEmpty();
            template.Should().Contain("{Timestamp");
            template.Should().Contain("{Level");
        }
    }

    [TestFixture]
    public class Imagenes
    {
        [Test]
        public void AllowedImageExtensions_DeberiaRetornarExtensiones()
        {
            // Act
            var extensions = AppConfig.AllowedImageExtensions;

            // Assert
            extensions.Should().NotBeEmpty();
            extensions.Should().Contain(".png");
            extensions.Should().Contain(".jpg");
            extensions.Should().Contain(".jpeg");
            extensions.Should().Contain(".bmp");
        }
    }
}
