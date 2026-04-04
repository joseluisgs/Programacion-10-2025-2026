using FluentAssertions;
using GestionAcademica.Services.Personas;
using GestionAcademica.Services.Backup;
using GestionAcademica.Services.Report;
using GestionAcademica.Services.ImportExport;
using GestionAcademica.Services.Dialogs;
using GestionAcademica.ViewModels;
using NUnit.Framework;
using Moq;

namespace GestionAcademica.Test.ViewModels.Main;

[TestFixture]
public class MainViewModelTests
{
    [TestFixture]
    public class CasosPositivos
    {
        private Mock<IPersonasService> _personasServiceMock = null!;
        private Mock<IBackupService> _backupServiceMock = null!;
        private Mock<IReportService> _reportServiceMock = null!;
        private Mock<IImportExportService> _importExportServiceMock = null!;
        private Mock<IDialogService> _dialogServiceMock = null!;

        [SetUp]
        public void SetUp()
        {
            _personasServiceMock = new Mock<IPersonasService>();
            _backupServiceMock = new Mock<IBackupService>();
            _reportServiceMock = new Mock<IReportService>();
            _importExportServiceMock = new Mock<IImportExportService>();
            _dialogServiceMock = new Mock<IDialogService>();
        }

        [Test]
        public void Constructor_DeberiaInicializarIsDarkThemeVerdadero()
        {
            // Act
            var viewModel = new MainViewModel(
                _personasServiceMock.Object,
                _backupServiceMock.Object,
                _reportServiceMock.Object,
                _importExportServiceMock.Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.IsDarkTheme.Should().BeTrue();
        }

        [Test]
        public void Constructor_DeberiaInicializarStatusMessageListo()
        {
            // Act
            var viewModel = new MainViewModel(
                _personasServiceMock.Object,
                _backupServiceMock.Object,
                _reportServiceMock.Object,
                _importExportServiceMock.Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.StatusMessage.Should().Be("Listo");
        }

        [Test]
        public void Constructor_DeberiaInicializarIsLoadingFalso()
        {
            // Act
            var viewModel = new MainViewModel(
                _personasServiceMock.Object,
                _backupServiceMock.Object,
                _reportServiceMock.Object,
                _importExportServiceMock.Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.IsLoading.Should().BeFalse();
        }

        [Test]
        public void IsDarkTheme_SePuedeCambiarAFalso()
        {
            // Arrange
            var viewModel = new MainViewModel(
                _personasServiceMock.Object,
                _backupServiceMock.Object,
                _reportServiceMock.Object,
                _importExportServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.IsDarkTheme = false;

            // Assert
            viewModel.IsDarkTheme.Should().BeFalse();
        }

        [Test]
        public void IsDarkTheme_SePuedeCambiarAVerdadero()
        {
            // Arrange
            var viewModel = new MainViewModel(
                _personasServiceMock.Object,
                _backupServiceMock.Object,
                _reportServiceMock.Object,
                _importExportServiceMock.Object,
                _dialogServiceMock.Object);
            viewModel.IsDarkTheme = false;

            // Act
            viewModel.IsDarkTheme = true;

            // Assert
            viewModel.IsDarkTheme.Should().BeTrue();
        }

        [Test]
        public void StatusMessage_SePuedeAsignar()
        {
            // Arrange
            var viewModel = new MainViewModel(
                _personasServiceMock.Object,
                _backupServiceMock.Object,
                _reportServiceMock.Object,
                _importExportServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.StatusMessage = "Nuevo estado";

            // Assert
            viewModel.StatusMessage.Should().Be("Nuevo estado");
        }

        [Test]
        public void StatusMessage_SePuedeCambiar()
        {
            // Arrange
            var viewModel = new MainViewModel(
                _personasServiceMock.Object,
                _backupServiceMock.Object,
                _reportServiceMock.Object,
                _importExportServiceMock.Object,
                _dialogServiceMock.Object);
            viewModel.StatusMessage = "Estado 1";

            // Act
            viewModel.StatusMessage = "Estado 2";

            // Assert
            viewModel.StatusMessage.Should().Be("Estado 2");
        }

        [Test]
        public void IsLoading_SePuedeCambiarAVerdadero()
        {
            // Arrange
            var viewModel = new MainViewModel(
                _personasServiceMock.Object,
                _backupServiceMock.Object,
                _reportServiceMock.Object,
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
            var viewModel = new MainViewModel(
                _personasServiceMock.Object,
                _backupServiceMock.Object,
                _reportServiceMock.Object,
                _importExportServiceMock.Object,
                _dialogServiceMock.Object);
            viewModel.IsLoading = true;

            // Act
            viewModel.IsLoading = false;

            // Assert
            viewModel.IsLoading.Should().BeFalse();
        }

        [Test]
        public void OnNavigateRequested_SePuedeSuscribirYDesuscribir()
        {
            // Arrange
            var viewModel = new MainViewModel(
                _personasServiceMock.Object,
                _backupServiceMock.Object,
                _reportServiceMock.Object,
                _importExportServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            void handler(System.Windows.Controls.Page page) { }
            viewModel.OnNavigateRequested += handler;
            viewModel.OnNavigateRequested -= handler;

            // Assert
            Assert.Pass();
        }

        [Test]
        public void MultiplesCambiosPropiedades_DeberianMantenerEstado()
        {
            // Arrange
            var viewModel = new MainViewModel(
                _personasServiceMock.Object,
                _backupServiceMock.Object,
                _reportServiceMock.Object,
                _importExportServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.IsDarkTheme = false;
            viewModel.StatusMessage = "Cargando...";
            viewModel.IsLoading = true;

            // Assert
            viewModel.IsDarkTheme.Should().BeFalse();
            viewModel.StatusMessage.Should().Be("Cargando...");
            viewModel.IsLoading.Should().BeTrue();
        }
    }

    [TestFixture]
    public class CasosNegativos
    {
        [Test]
        public void Constructor_NoDeberiaFallarConMocksBasicos()
        {
            // Arrange & Act
            var viewModel = new MainViewModel(
                new Mock<IPersonasService>().Object,
                new Mock<IBackupService>().Object,
                new Mock<IReportService>().Object,
                new Mock<IImportExportService>().Object,
                new Mock<IDialogService>().Object);

            // Assert
            viewModel.Should().NotBeNull();
        }
    }
}
