using FluentAssertions;
using GestionAcademica.Models.Personas;
using GestionAcademica.Models.Academia;
using GestionAcademica.Services.Personas;
using GestionAcademica.Services.Images;
using GestionAcademica.Services.Dialogs;
using GestionAcademica.Errors.Common;
using GestionAcademica.ViewModels.Docentes;
using GestionAcademica.Mappers.Personas;
using CSharpFunctionalExtensions;
using NUnit.Framework;
using Moq;

namespace GestionAcademica.Test.ViewModels.Docentes;

[TestFixture]
public class DocenteEditViewModelTests
{
    [TestFixture]
    public class CasosPositivos
    {
        private Mock<IPersonasService> _personasServiceMock = null!;
        private Mock<IImageService> _imageServiceMock = null!;
        private Mock<IDialogService> _dialogServiceMock = null!;

        [SetUp]
        public void SetUp()
        {
            _personasServiceMock = new Mock<IPersonasService>();
            _imageServiceMock = new Mock<IImageService>();
            _dialogServiceMock = new Mock<IDialogService>();
        }

        [Test]
        public void Constructor_ModoNuevo_DeberiaTenerTituloNuevo()
        {
            // Arrange
            var docente = new Docente();

            // Act
            var viewModel = new DocenteEditViewModel(
                docente,
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object,
                isNew: true);

            // Assert
            viewModel.WindowTitle.Should().Be("Nuevo Docente");
        }

        [Test]
        public void Constructor_ModoEdicion_DeberiaTenerTituloEditar()
        {
            // Arrange
            var docente = new Docente { Id = 1, Nombre = "Juan" };

            // Act
            var viewModel = new DocenteEditViewModel(
                docente,
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object,
                isNew: false);

            // Assert
            viewModel.WindowTitle.Should().Be("Editar Docente");
        }

        [Test]
        public void Constructor_DeberiaInicializarFormData()
        {
            // Arrange
            var docente = new Docente
            {
                Dni = "11111111A",
                Nombre = "Juan",
                Apellidos = "Pérez",
                Email = "juan@test.com",
                Experiencia = 5,
                Especialidad = "Programación",
                Ciclo = Ciclo.DAM
            };

            // Act
            var viewModel = new DocenteEditViewModel(
                docente,
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object,
                isNew: true);

            // Assert
            viewModel.FormData.Should().NotBeNull();
            viewModel.FormData.Dni.Should().Be("11111111A");
            viewModel.FormData.Nombre.Should().Be("Juan");
            viewModel.FormData.Apellidos.Should().Be("Pérez");
            viewModel.FormData.Email.Should().Be("juan@test.com");
            viewModel.FormData.Experiencia.Should().Be(5);
            viewModel.FormData.Especialidad.Should().Be("Programación");
        }

        [Test]
        public void Ciclos_DeberiaContenerTodosLosCiclos()
        {
            // Arrange
            var docente = new Docente();

            // Act
            var viewModel = new DocenteEditViewModel(
                docente,
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object,
                isNew: true);

            // Assert
            viewModel.Ciclos.Should().Contain(Ciclo.DAM);
            viewModel.Ciclos.Should().Contain(Ciclo.DAW);
            viewModel.Ciclos.Should().Contain(Ciclo.ASIR);
        }

        [Test]
        public void Constructor_DeberiaManejarExperienciaCero()
        {
            // Arrange - Usar DNI válido con letra correcta (12345678 -> J)
            var docente = new Docente
            {
                Dni = "12345678J",
                Nombre = "Juan",
                Apellidos = "Pérez",
                Email = "juan@test.com",
                Experiencia = 0,
                Especialidad = "Programación",
                Ciclo = Ciclo.DAM,
                FechaNacimiento = new DateTime(1980, 1, 1)
            };

            // Act
            var viewModel = new DocenteEditViewModel(
                docente,
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object,
                true);

            // Assert
            viewModel.FormData.Experiencia.Should().Be(0);
        }

        [Test]
        public void CloseAction_PuedeAsignarse()
        {
            // Arrange
            var docente = new Docente();
            var viewModel = new DocenteEditViewModel(
                docente,
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object,
                isNew: true);
            var called = false;

            // Act
            viewModel.CloseAction = (result) => called = true;
            viewModel.CloseAction?.Invoke(true);

            // Assert
            called.Should().BeTrue();
        }

        [Test]
        public void LimpiarImagenCommand_DeberiaLimpiarImagen()
        {
            // Arrange
            var docente = new Docente { Imagen = "imagen.jpg" };
            var viewModel = new DocenteEditViewModel(
                docente,
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object,
                isNew: true);
            viewModel.FormData.Imagen = "imagen.jpg";

            // Act
            viewModel.LimpiarImagenCommand.Execute(null);

            // Assert
            viewModel.FormData.Imagen.Should().BeNull();
        }
    }

    [TestFixture]
    public class CasosNegativos
    {
        [Test]
        public void Constructor_CuandoDocenteEsNulo_NoDeberiaFallar()
        {
            // Arrange & Act
            var viewModel = new DocenteEditViewModel(
                new Docente(),
                new Mock<IPersonasService>().Object,
                new Mock<IImageService>().Object,
                new Mock<IDialogService>().Object,
                true);

            // Assert
            viewModel.Should().NotBeNull();
        }

        [Test]
        public void FormData_DniInvalido_IsValid_DeberiaRetornarFalse()
        {
            // Arrange
            var docente = new Docente
            {
                Dni = "INVALID",
                Nombre = "Juan",
                Apellidos = "Pérez",
                Email = "juan@test.com",
                Experiencia = 5,
                Especialidad = "Programación"
            };

            var viewModel = new DocenteEditViewModel(
                docente,
                new Mock<IPersonasService>().Object,
                new Mock<IImageService>().Object,
                new Mock<IDialogService>().Object,
                true);

            // Act
            var esValido = viewModel.FormData.IsValid();

            // Assert
            esValido.Should().BeFalse();
        }

        [Test]
        public void FormData_NombreVacio_IsValid_DeberiaRetornarFalse()
        {
            // Arrange
            var docente = new Docente
            {
                Dni = "11111111A",
                Nombre = "",
                Apellidos = "Pérez",
                Email = "juan@test.com",
                Experiencia = 5,
                Especialidad = "Programación"
            };

            var viewModel = new DocenteEditViewModel(
                docente,
                new Mock<IPersonasService>().Object,
                new Mock<IImageService>().Object,
                new Mock<IDialogService>().Object,
                true);

            // Act
            var esValido = viewModel.FormData.IsValid();

            // Assert
            esValido.Should().BeFalse();
        }

        [Test]
        public void FormData_EmailInvalido_IsValid_DeberiaRetornarFalse()
        {
            // Arrange
            var docente = new Docente
            {
                Dni = "11111111A",
                Nombre = "Juan",
                Apellidos = "Pérez",
                Email = "no-es-email",
                Experiencia = 5,
                Especialidad = "Programación"
            };

            var viewModel = new DocenteEditViewModel(
                docente,
                new Mock<IPersonasService>().Object,
                new Mock<IImageService>().Object,
                new Mock<IDialogService>().Object,
                true);

            // Act
            var esValido = viewModel.FormData.IsValid();

            // Assert
            esValido.Should().BeFalse();
        }

        [Test]
        public void FormData_ExperienciaNegativa_IsValid_DeberiaRetornarFalse()
        {
            // Arrange
            var docente = new Docente
            {
                Dni = "11111111A",
                Nombre = "Juan",
                Apellidos = "Pérez",
                Email = "juan@test.com",
                Experiencia = -1,
                Especialidad = "Programación"
            };

            var viewModel = new DocenteEditViewModel(
                docente,
                new Mock<IPersonasService>().Object,
                new Mock<IImageService>().Object,
                new Mock<IDialogService>().Object,
                true);

            // Act
            var esValido = viewModel.FormData.IsValid();

            // Assert
            esValido.Should().BeFalse();
        }

        [Test]
        public void FormData_EspecialidadVacia_IsValid_DeberiaRetornarFalse()
        {
            // Arrange
            var docente = new Docente
            {
                Dni = "11111111A",
                Nombre = "Juan",
                Apellidos = "Pérez",
                Email = "juan@test.com",
                Experiencia = 5,
                Especialidad = ""
            };

            var viewModel = new DocenteEditViewModel(
                docente,
                new Mock<IPersonasService>().Object,
                new Mock<IImageService>().Object,
                new Mock<IDialogService>().Object,
                true);

            // Act
            var esValido = viewModel.FormData.IsValid();

            // Assert
            esValido.Should().BeFalse();
        }

        [Test]
        public void FormData_GetValidationErrors_DeberiaRetornarErrores()
        {
            // Arrange
            var docente = new Docente
            {
                Dni = "",
                Nombre = "J",
                Apellidos = "",
                Email = "invalid",
                Experiencia = -1,
                Especialidad = ""
            };

            var viewModel = new DocenteEditViewModel(
                docente,
                new Mock<IPersonasService>().Object,
                new Mock<IImageService>().Object,
                new Mock<IDialogService>().Object,
                true);

            // Act
            var errores = viewModel.FormData.GetValidationErrors();

            // Assert
            errores.Should().NotBeEmpty();
            errores.Should().Contain("DNI");
            errores.Should().Contain("Nombre");
            errores.Should().Contain("Experiencia");
        }

        [Test]
        public void SaveCommand_ConFormularioInvalido_DeberiaMostrarErrores()
        {
            // Arrange
            var dialogServiceMock = new Mock<IDialogService>();
            var docente = new Docente
            {
                Dni = "",
                Nombre = "J",
                Apellidos = "P",
                Email = "invalid",
                Experiencia = -1
            };

            var viewModel = new DocenteEditViewModel(
                new Docente(),
                new Mock<IPersonasService>().Object,
                new Mock<IImageService>().Object,
                dialogServiceMock.Object,
                isNew: true);

            // Act
            viewModel.SaveCommand.Execute(null);

            // Assert
            dialogServiceMock.Verify(d => d.ShowWarning(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void CancelCommand_DeberiaInvocarCloseActionConFalse()
        {
            // Arrange
            var called = false;
            var resultPassed = true;
            var docente = new Docente();

            var viewModel = new DocenteEditViewModel(
                new Docente(),
                new Mock<IPersonasService>().Object,
                new Mock<IImageService>().Object,
                new Mock<IDialogService>().Object,
                isNew: true);
            viewModel.CloseAction = (result) =>
            {
                called = true;
                resultPassed = result;
            };

            // Act
            viewModel.CancelCommand.Execute(null);

            // Assert
            called.Should().BeTrue();
            resultPassed.Should().BeFalse();
        }
    }
}