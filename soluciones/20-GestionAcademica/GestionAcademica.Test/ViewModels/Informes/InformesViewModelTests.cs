using FluentAssertions;
using GestionAcademica.Enums;
using GestionAcademica.Models.Academia;
using GestionAcademica.Services.Personas;
using GestionAcademica.Services.Dialogs;
using GestionAcademica.Services.Report;
using GestionAcademica.ViewModels.Informes;
using NUnit.Framework;
using Moq;

namespace GestionAcademica.Test.ViewModels.Informes;

[TestFixture]
public class InformesViewModelTests
{
    [TestFixture]
    public class CasosPositivos
    {
        private Mock<IPersonasService> _personasServiceMock = null!;
        private Mock<IReportService> _reportServiceMock = null!;
        private Mock<IDialogService> _dialogServiceMock = null!;

        [SetUp]
        public void SetUp()
        {
            _personasServiceMock = new Mock<IPersonasService>();
            _reportServiceMock = new Mock<IReportService>();
            _dialogServiceMock = new Mock<IDialogService>();
        }

        [Test]
        public void Ciclos_DeberiaContenerDAM()
        {
            // Arrange
            var viewModel = new InformesViewModel(
                _personasServiceMock.Object,
                _reportServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            var ciclos = viewModel.Ciclos.ToList();

            // Assert
            ciclos.Should().Contain(Ciclo.DAM);
        }

        [Test]
        public void Ciclos_DeberiaContenerDAW()
        {
            // Arrange
            var viewModel = new InformesViewModel(
                _personasServiceMock.Object,
                _reportServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            var ciclos = viewModel.Ciclos.ToList();

            // Assert
            ciclos.Should().Contain(Ciclo.DAW);
        }

        [Test]
        public void Ciclos_DeberiaContenerASIR()
        {
            // Arrange
            var viewModel = new InformesViewModel(
                _personasServiceMock.Object,
                _reportServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            var ciclos = viewModel.Ciclos.ToList();

            // Assert
            ciclos.Should().Contain(Ciclo.ASIR);
        }

        [Test]
        public void Cursos_DeberiaContenerPrimero()
        {
            // Arrange
            var viewModel = new InformesViewModel(
                _personasServiceMock.Object,
                _reportServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            var cursos = viewModel.Cursos.ToList();

            // Assert
            cursos.Should().Contain(Curso.Primero);
        }

        [Test]
        public void Cursos_DeberiaContenerSegundo()
        {
            // Arrange
            var viewModel = new InformesViewModel(
                _personasServiceMock.Object,
                _reportServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            var cursos = viewModel.Cursos.ToList();

            // Assert
            cursos.Should().Contain(Curso.Segundo);
        }

        [Test]
        public void PropiedadesPorDefecto_IsGeneratingFalso()
        {
            // Arrange
            var viewModel = new InformesViewModel(
                _personasServiceMock.Object,
                _reportServiceMock.Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.IsGenerating.Should().BeFalse();
        }

        [Test]
        public void PropiedadesPorDefecto_MostrarEliminadosFalso()
        {
            // Arrange
            var viewModel = new InformesViewModel(
                _personasServiceMock.Object,
                _reportServiceMock.Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.MostrarEliminados.Should().BeFalse();
        }

        [Test]
        public void PropiedadesPorDefecto_MostrarMenoresEdadFalso()
        {
            // Arrange
            var viewModel = new InformesViewModel(
                _personasServiceMock.Object,
                _reportServiceMock.Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.MostrarMenoresEdad.Should().BeFalse();
        }

        [Test]
        public void PropiedadesPorDefecto_NotaAprobado5()
        {
            // Arrange
            var viewModel = new InformesViewModel(
                _personasServiceMock.Object,
                _reportServiceMock.Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.NotaAprobado.Should().Be(5.0);
        }

        [Test]
        public void PropiedadesPorDefecto_SelectedCicloNulo()
        {
            // Arrange
            var viewModel = new InformesViewModel(
                _personasServiceMock.Object,
                _reportServiceMock.Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.SelectedCiclo.Should().BeNull();
        }

        [Test]
        public void PropiedadesPorDefecto_SelectedCursoNulo()
        {
            // Arrange
            var viewModel = new InformesViewModel(
                _personasServiceMock.Object,
                _reportServiceMock.Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.SelectedCurso.Should().BeNull();
        }

        [Test]
        public void SelectedCiclo_SePuedeAsignarDAM()
        {
            // Arrange
            var viewModel = new InformesViewModel(
                _personasServiceMock.Object,
                _reportServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.SelectedCiclo = Ciclo.DAM;

            // Assert
            viewModel.SelectedCiclo.Should().Be(Ciclo.DAM);
        }

        [Test]
        public void SelectedCiclo_SePuedeCambiarADAW()
        {
            // Arrange
            var viewModel = new InformesViewModel(
                _personasServiceMock.Object,
                _reportServiceMock.Object,
                _dialogServiceMock.Object);
            viewModel.SelectedCiclo = Ciclo.DAM;

            // Act
            viewModel.SelectedCiclo = Ciclo.DAW;

            // Assert
            viewModel.SelectedCiclo.Should().Be(Ciclo.DAW);
        }

        [Test]
        public void SelectedCurso_SePuedeAsignarPrimero()
        {
            // Arrange
            var viewModel = new InformesViewModel(
                _personasServiceMock.Object,
                _reportServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.SelectedCurso = Curso.Primero;

            // Assert
            viewModel.SelectedCurso.Should().Be(Curso.Primero);
        }

        [Test]
        public void SelectedCurso_SePuedeCambiarASegundo()
        {
            // Arrange
            var viewModel = new InformesViewModel(
                _personasServiceMock.Object,
                _reportServiceMock.Object,
                _dialogServiceMock.Object);
            viewModel.SelectedCurso = Curso.Primero;

            // Act
            viewModel.SelectedCurso = Curso.Segundo;

            // Assert
            viewModel.SelectedCurso.Should().Be(Curso.Segundo);
        }

        [Test]
        public void MostrarEliminados_SePuedeCambiarAVerdadero()
        {
            // Arrange
            var viewModel = new InformesViewModel(
                _personasServiceMock.Object,
                _reportServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.MostrarEliminados = true;

            // Assert
            viewModel.MostrarEliminados.Should().BeTrue();
        }

        [Test]
        public void MostrarMenoresEdad_SePuedeCambiarAVerdadero()
        {
            // Arrange
            var viewModel = new InformesViewModel(
                _personasServiceMock.Object,
                _reportServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.MostrarMenoresEdad = true;

            // Assert
            viewModel.MostrarMenoresEdad.Should().BeTrue();
        }

        [Test]
        public void NotaAprobado_SePuedeCambiarA7()
        {
            // Arrange
            var viewModel = new InformesViewModel(
                _personasServiceMock.Object,
                _reportServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.NotaAprobado = 7.0;

            // Assert
            viewModel.NotaAprobado.Should().Be(7.0);
        }

        [Test]
        public void IsGenerating_SePuedeCambiarAVerdadero()
        {
            // Arrange
            var viewModel = new InformesViewModel(
                _personasServiceMock.Object,
                _reportServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.IsGenerating = true;

            // Assert
            viewModel.IsGenerating.Should().BeTrue();
        }

        [Test]
        public void StatusMessage_SePuedeAsignar()
        {
            // Arrange
            var viewModel = new InformesViewModel(
                _personasServiceMock.Object,
                _reportServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.StatusMessage = "Generando informe...";

            // Assert
            viewModel.StatusMessage.Should().Be("Generando informe...");
        }
    }

    [TestFixture]
    public class CasosNegativos
    {
        [Test]
        public void Constructor_NoDeberiaFallarConMocksBasicos()
        {
            // Arrange & Act
            var viewModel = new InformesViewModel(
                new Mock<IPersonasService>().Object,
                new Mock<IReportService>().Object,
                new Mock<IDialogService>().Object);

            // Assert
            viewModel.Should().NotBeNull();
        }
    }
}
