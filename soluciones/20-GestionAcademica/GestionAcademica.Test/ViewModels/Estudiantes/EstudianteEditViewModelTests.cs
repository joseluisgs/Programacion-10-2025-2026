using FluentAssertions;
using GestionAcademica.Models.Personas;
using GestionAcademica.Models.Academia;
using GestionAcademica.Services.Personas;
using GestionAcademica.Services.Images;
using GestionAcademica.Services.Dialogs;
using GestionAcademica.Errors.Common;
using GestionAcademica.ViewModels.Estudiantes;
using GestionAcademica.Mappers.Personas;
using CSharpFunctionalExtensions;
using NUnit.Framework;
using Moq;

namespace GestionAcademica.Test.ViewModels.Estudiantes;

[TestFixture]
public class EstudianteEditViewModelTests
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
            var estudiante = new Estudiante();

            // Act
            var viewModel = new EstudianteEditViewModel(
                estudiante,
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object,
                isNew: true);

            // Assert
            viewModel.WindowTitle.Should().Be("Nuevo Estudiante");
        }

        [Test]
        public void Constructor_ModoEdicion_DeberiaTenerTituloEditar()
        {
            // Arrange
            var estudiante = new Estudiante { Id = 1, Nombre = "Juan" };

            // Act
            var viewModel = new EstudianteEditViewModel(
                estudiante,
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object,
                isNew: false);

            // Assert
            viewModel.WindowTitle.Should().Be("Editar Estudiante");
        }

        [Test]
        public void Constructor_DeberiaInicializarFormData()
        {
            // Arrange
            var estudiante = new Estudiante
            {
                Dni = "11111111A",
                Nombre = "Juan",
                Apellidos = "Pérez",
                Email = "juan@test.com"
            };

            // Act
            var viewModel = new EstudianteEditViewModel(
                estudiante,
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
        }

        [Test]
        public void Ciclos_DeberiaContenerTodosLosCiclos()
        {
            // Arrange
            var estudiante = new Estudiante();

            // Act
            var viewModel = new EstudianteEditViewModel(
                estudiante,
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
        public void Cursos_DeberiaContenerTodosLosCursos()
        {
            // Arrange
            var estudiante = new Estudiante();

            // Act
            var viewModel = new EstudianteEditViewModel(
                estudiante,
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object,
                isNew: true);

            // Assert
            viewModel.Cursos.Should().Contain(Curso.Primero);
            viewModel.Cursos.Should().Contain(Curso.Segundo);
        }

        [Test]
        public void FormData_ConValoresValidos_IsValid_DeberiaRetornarTrue()
        {
            // Arrange
            var estudiante = new Estudiante
            {
                Dni = "11111111H",
                Nombre = "Juan",
                Apellidos = "Pérez",
                Email = "juan@test.com",
                Calificacion = 7.5,
                Ciclo = Ciclo.DAM,
                Curso = Curso.Primero,
                FechaNacimiento = new DateTime(2000, 1, 1)
            };

            var viewModel = new EstudianteEditViewModel(
                estudiante,
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object,
                isNew: true);

            // Act
            var esValido = viewModel.FormData.IsValid();

            // Assert
            esValido.Should().BeTrue();
        }

        [Test]
        public void FormData_ToModel_DeberiaMapearCorrectamente()
        {
            // Arrange
            var estudiante = new Estudiante
            {
                Dni = "11111111A",
                Nombre = "Juan",
                Apellidos = "Pérez",
                Email = "juan@test.com",
                Calificacion = 8.5,
                Ciclo = Ciclo.DAW,
                Curso = Curso.Segundo
            };

            var viewModel = new EstudianteEditViewModel(
                estudiante,
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object,
                isNew: true);

            // Act
            var modelo = viewModel.FormData.ToModel();

            // Assert
            modelo.Dni.Should().Be("11111111A");
            modelo.Nombre.Should().Be("Juan");
            modelo.Apellidos.Should().Be("Pérez");
            modelo.Email.Should().Be("juan@test.com");
            modelo.Calificacion.Should().Be(8.5);
            modelo.Ciclo.Should().Be(Ciclo.DAW);
            modelo.Curso.Should().Be(Curso.Segundo);
        }

        [Test]
        public void CloseAction_PuedeAsignarse()
        {
            // Arrange
            var estudiante = new Estudiante();
            var viewModel = new EstudianteEditViewModel(
                estudiante,
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
            var estudiante = new Estudiante { Imagen = "imagen.jpg" };
            var viewModel = new EstudianteEditViewModel(
                estudiante,
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
        public void Constructor_CuandoEstudianteEsNulo_NoDeberiaFallar()
        {
            // Arrange & Act
            var viewModel = new EstudianteEditViewModel(
                new Estudiante(),
                new Mock<IPersonasService>().Object,
                new Mock<IImageService>().Object,
                new Mock<IDialogService>().Object,
                isNew: true);

            // Assert
            viewModel.Should().NotBeNull();
        }

        [Test]
        public void FormData_DniInvalido_IsValid_DeberiaRetornarFalse()
        {
            // Arrange
            var estudiante = new Estudiante
            {
                Dni = "INVALID",
                Nombre = "Juan",
                Apellidos = "Pérez",
                Email = "juan@test.com"
            };

            var viewModel = new EstudianteEditViewModel(
                estudiante,
                new Mock<IPersonasService>().Object,
                new Mock<IImageService>().Object,
                new Mock<IDialogService>().Object,
                isNew: true);

            // Act
            var esValido = viewModel.FormData.IsValid();

            // Assert
            esValido.Should().BeFalse();
        }

        [Test]
        public void FormData_NombreVacio_IsValid_DeberiaRetornarFalse()
        {
            // Arrange
            var estudiante = new Estudiante
            {
                Dni = "11111111A",
                Nombre = "",
                Apellidos = "Pérez",
                Email = "juan@test.com"
            };

            var viewModel = new EstudianteEditViewModel(
                estudiante,
                new Mock<IPersonasService>().Object,
                new Mock<IImageService>().Object,
                new Mock<IDialogService>().Object,
                isNew: true);

            // Act
            var esValido = viewModel.FormData.IsValid();

            // Assert
            esValido.Should().BeFalse();
        }

        [Test]
        public void FormData_EmailInvalido_IsValid_DeberiaRetornarFalse()
        {
            // Arrange
            var estudiante = new Estudiante
            {
                Dni = "11111111A",
                Nombre = "Juan",
                Apellidos = "Pérez",
                Email = "no-es-email"
            };

            var viewModel = new EstudianteEditViewModel(
                estudiante,
                new Mock<IPersonasService>().Object,
                new Mock<IImageService>().Object,
                new Mock<IDialogService>().Object,
                isNew: true);

            // Act
            var esValido = viewModel.FormData.IsValid();

            // Assert
            esValido.Should().BeFalse();
        }

        [Test]
        public void FormData_CalificacionFueraDeRango_IsValid_DeberiaRetornarFalse()
        {
            // Arrange
            var estudiante = new Estudiante
            {
                Dni = "11111111A",
                Nombre = "Juan",
                Apellidos = "Pérez",
                Email = "juan@test.com",
                Calificacion = 15.0
            };

            var viewModel = new EstudianteEditViewModel(
                estudiante,
                new Mock<IPersonasService>().Object,
                new Mock<IImageService>().Object,
                new Mock<IDialogService>().Object,
                isNew: true);

            // Act
            var esValido = viewModel.FormData.IsValid();

            // Assert
            esValido.Should().BeFalse();
        }

        [Test]
        public void FormData_GetValidationErrors_DeberiaRetornarErrores()
        {
            // Arrange
            var estudiante = new Estudiante
            {
                Dni = "",
                Nombre = "J",
                Apellidos = "",
                Email = "invalid",
                Calificacion = -1
            };

            var viewModel = new EstudianteEditViewModel(
                estudiante,
                new Mock<IPersonasService>().Object,
                new Mock<IImageService>().Object,
                new Mock<IDialogService>().Object,
                isNew: true);

            // Act
            var errores = viewModel.FormData.GetValidationErrors();

            // Assert
            errores.Should().NotBeEmpty();
            errores.Should().Contain("DNI");
            errores.Should().Contain("Nombre");
        }

        [Test]
        public void SaveCommand_ConFormularioInvalido_DeberiaMostrarErrores()
        {
            // Arrange
            var dialogServiceMock = new Mock<IDialogService>();
            var estudiante = new Estudiante
            {
                Dni = "",
                Nombre = "J",
                Apellidos = "P",
                Email = "invalid"
            };

            var viewModel = new EstudianteEditViewModel(
                new Estudiante(),
                new Mock<IPersonasService>().Object,
                new Mock<IImageService>().Object,
                dialogServiceMock.Object,
                isNew: true);
            viewModel.FormData.Dni = "";
            viewModel.FormData.Nombre = "J";

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
            var estudiante = new Estudiante();

            var viewModel = new EstudianteEditViewModel(
                new Estudiante(),
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