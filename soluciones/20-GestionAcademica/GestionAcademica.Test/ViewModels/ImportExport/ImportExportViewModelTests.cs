using FluentAssertions;
using GestionAcademica.Models.Personas;
using GestionAcademica.Models.Academia;
using GestionAcademica.Services.Personas;
using GestionAcademica.Services.ImportExport;
using GestionAcademica.Services.Dialogs;
using GestionAcademica.ViewModels.ImportExport;
using NUnit.Framework;
using Moq;

namespace GestionAcademica.Test.ViewModels.ImportExport;

[TestFixture]
public class ImportExportViewModelTests
{
    [TestFixture]
    public class CasosPositivos
    {
        private Mock<IPersonasService> _personasServiceMock = null!;
        private Mock<IImportExportService> _importExportServiceMock = null!;
        private Mock<IDialogService> _dialogServiceMock = null!;

        [SetUp]
        public void SetUp()
        {
            _personasServiceMock = new Mock<IPersonasService>();
            _importExportServiceMock = new Mock<IImportExportService>();
            _dialogServiceMock = new Mock<IDialogService>();
        }

        [Test]
        public void PropiedadesPorDefecto_DeberianTenerValoresIniciales()
        {
            // Act
            var viewModel = new ImportExportViewModel(
                _personasServiceMock.Object,
                _importExportServiceMock.Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.IsLoading.Should().BeFalse();
            viewModel.SustituirDatos.Should().BeFalse();
            viewModel.StatusMessage.Should().BeNullOrEmpty();
        }

        [Test]
        public void IsLoading_SePuedeCambiarAVerdadero()
        {
            // Arrange
            var viewModel = new ImportExportViewModel(
                _personasServiceMock.Object,
                _importExportServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.IsLoading = true;

            // Assert
            viewModel.IsLoading.Should().BeTrue();
        }

        [Test]
        public void IsLoading_SePuedeCambiarAFalso()
        {
            // Arrange
            var viewModel = new ImportExportViewModel(
                _personasServiceMock.Object,
                _importExportServiceMock.Object,
                _dialogServiceMock.Object);
            viewModel.IsLoading = true;

            // Act
            viewModel.IsLoading = false;

            // Assert
            viewModel.IsLoading.Should().BeFalse();
        }

        [Test]
        public void SustituirDatos_SePuedeCambiarAVerdadero()
        {
            // Arrange
            var viewModel = new ImportExportViewModel(
                _personasServiceMock.Object,
                _importExportServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.SustituirDatos = true;

            // Assert
            viewModel.SustituirDatos.Should().BeTrue();
        }

        [Test]
        public void SustituirDatos_SePuedeCambiarAFalso()
        {
            // Arrange
            var viewModel = new ImportExportViewModel(
                _personasServiceMock.Object,
                _importExportServiceMock.Object,
                _dialogServiceMock.Object);
            viewModel.SustituirDatos = true;

            // Act
            viewModel.SustituirDatos = false;

            // Assert
            viewModel.SustituirDatos.Should().BeFalse();
        }

        [Test]
        public void StatusMessage_SePuedeAsignar()
        {
            // Arrange
            var viewModel = new ImportExportViewModel(
                _personasServiceMock.Object,
                _importExportServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.StatusMessage = "Exportando...";

            // Assert
            viewModel.StatusMessage.Should().Be("Exportando...");
        }

        [Test]
        public void StatusMessage_SePuedeCambiar()
        {
            // Arrange
            var viewModel = new ImportExportViewModel(
                _personasServiceMock.Object,
                _importExportServiceMock.Object,
                _dialogServiceMock.Object);
            viewModel.StatusMessage = "Exportando...";

            // Act
            viewModel.StatusMessage = "Completado";

            // Assert
            viewModel.StatusMessage.Should().Be("Completado");
        }
    }

    [TestFixture]
    public class CasosNegativos
    {
        [Test]
        public void Constructor_NoDeberiaFallarConMocksBasicos()
        {
            // Arrange & Act
            var viewModel = new ImportExportViewModel(
                new Mock<IPersonasService>().Object,
                new Mock<IImportExportService>().Object,
                new Mock<IDialogService>().Object);

            // Assert
            viewModel.Should().NotBeNull();
            viewModel.IsLoading.Should().BeFalse();
        }
    }
}
