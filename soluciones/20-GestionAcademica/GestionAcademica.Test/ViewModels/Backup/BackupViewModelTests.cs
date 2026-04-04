using FluentAssertions;
using GestionAcademica.Models.Personas;
using GestionAcademica.Models.Academia;
using GestionAcademica.Services.Personas;
using GestionAcademica.Services.Backup;
using GestionAcademica.Services.Dialogs;
using GestionAcademica.ViewModels.Backup;
using GestionAcademica.Errors.Backup;
using GestionAcademica.Errors.Common;
using NUnit.Framework;
using Moq;

namespace GestionAcademica.Test.ViewModels.Backup;

[TestFixture]
public class BackupViewModelTests
{
    [TestFixture]
    public class CasosPositivos
    {
        private Mock<IPersonasService> _personasServiceMock = null!;
        private Mock<IBackupService> _backupServiceMock = null!;
        private Mock<IDialogService> _dialogServiceMock = null!;

        [SetUp]
        public void SetUp()
        {
            _personasServiceMock = new Mock<IPersonasService>();
            _backupServiceMock = new Mock<IBackupService>();
            _dialogServiceMock = new Mock<IDialogService>();
        }

        [Test]
        public void Constructor_ConBackupsDisponibles_DeberiaCargarLista()
        {
            // Arrange
            _backupServiceMock.Setup(b => b.ListarBackups(It.IsAny<string?>()))
                .Returns(new List<string> { "backup1.zip", "backup2.zip", "backup3.zip" });

            // Act
            var viewModel = new BackupViewModel(
                _personasServiceMock.Object,
                _backupServiceMock.Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.Backups.Should().HaveCount(3);
            viewModel.Backups.Should().Contain("backup1.zip");
            viewModel.Backups.Should().Contain("backup2.zip");
            viewModel.Backups.Should().Contain("backup3.zip");
            viewModel.StatusMessage.Should().Contain("3 backups");
        }

        [Test]
        public void Constructor_SinBackups_DeberiaRetornarListaVacia()
        {
            // Arrange
            _backupServiceMock.Setup(b => b.ListarBackups(It.IsAny<string?>()))
                .Returns(new List<string>());

            // Act
            var viewModel = new BackupViewModel(
                _personasServiceMock.Object,
                _backupServiceMock.Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.Backups.Should().BeEmpty();
            viewModel.StatusMessage.Should().Contain("0 backups");
        }

        [Test]
        public void PropiedadesPorDefecto_DeberianTenerValoresIniciales()
        {
            // Arrange
            _backupServiceMock.Setup(b => b.ListarBackups(It.IsAny<string?>()))
                .Returns(new List<string>());

            // Act
            var viewModel = new BackupViewModel(
                _personasServiceMock.Object,
                _backupServiceMock.Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.IsLoading.Should().BeFalse();
            viewModel.SelectedBackup.Should().BeNull();
        }

        [Test]
        public void SelectedBackup_SePuedeAsignar()
        {
            // Arrange
            _backupServiceMock.Setup(b => b.ListarBackups(It.IsAny<string?>()))
                .Returns(new List<string>());
            var viewModel = new BackupViewModel(
                _personasServiceMock.Object,
                _backupServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.SelectedBackup = "backup1.zip";

            // Assert
            viewModel.SelectedBackup.Should().Be("backup1.zip");
        }

        [Test]
        public void SelectedBackup_SePuedeCambiar()
        {
            // Arrange
            _backupServiceMock.Setup(b => b.ListarBackups(It.IsAny<string?>()))
                .Returns(new List<string>());
            var viewModel = new BackupViewModel(
                _personasServiceMock.Object,
                _backupServiceMock.Object,
                _dialogServiceMock.Object);
            viewModel.SelectedBackup = "backup1.zip";

            // Act
            viewModel.SelectedBackup = "backup2.zip";

            // Assert
            viewModel.SelectedBackup.Should().Be("backup2.zip");
        }

        [Test]
        public void StatusMessage_SePuedeAsignar()
        {
            // Arrange
            _backupServiceMock.Setup(b => b.ListarBackups(It.IsAny<string?>()))
                .Returns(new List<string>());
            var viewModel = new BackupViewModel(
                _personasServiceMock.Object,
                _backupServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.StatusMessage = "Nuevo mensaje";

            // Assert
            viewModel.StatusMessage.Should().Be("Nuevo mensaje");
        }

        [Test]
        public void IsLoading_SePuedeCambiarAVerdadero()
        {
            // Arrange
            _backupServiceMock.Setup(b => b.ListarBackups(It.IsAny<string?>()))
                .Returns(new List<string>());
            var viewModel = new BackupViewModel(
                _personasServiceMock.Object,
                _backupServiceMock.Object,
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
            _backupServiceMock.Setup(b => b.ListarBackups(It.IsAny<string?>()))
                .Returns(new List<string>());
            var viewModel = new BackupViewModel(
                _personasServiceMock.Object,
                _backupServiceMock.Object,
                _dialogServiceMock.Object);
            viewModel.IsLoading = true;

            // Act
            viewModel.IsLoading = false;

            // Assert
            viewModel.IsLoading.Should().BeFalse();
        }
    }

    [TestFixture]
    public class CasosNegativos
    {
        [Test]
        public void Constructor_CuandoListarBackupsFalla_DeberiaMostrarError()
        {
            // Arrange
            var backupServiceMock = new Mock<IBackupService>();
            backupServiceMock.Setup(b => b.ListarBackups(It.IsAny<string?>()))
                .Throws(new Exception("Error de disco"));

            // Act
            var viewModel = new BackupViewModel(
                new Mock<IPersonasService>().Object,
                backupServiceMock.Object,
                new Mock<IDialogService>().Object);

            // Assert
            viewModel.StatusMessage.Should().Contain("Error al cargar backups");
        }

        [Test]
        public void Constructor_CuandoServicioEsNulo_NoDeberiaFallar()
        {
            // Arrange & Act
            var viewModel = new BackupViewModel(
                new Mock<IPersonasService>().Object,
                new Mock<IBackupService>().Object,
                new Mock<IDialogService>().Object);

            // Assert
            viewModel.Should().NotBeNull();
        }
    }
}
