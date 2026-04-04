using FluentAssertions;
using GestionAcademica.Models.Personas;
using GestionAcademica.Models.Academia;
using GestionAcademica.Services.Personas;
using GestionAcademica.ViewModels.Dashboard;
using NUnit.Framework;
using Moq;

namespace GestionAcademica.Test.ViewModels.Dashboard;

[TestFixture]
public class DashboardViewModelTests
{
    [TestFixture]
    public class CasosPositivos
    {
        private Mock<IPersonasService> _serviceMock = null!;

        [SetUp]
        public void SetUp()
        {
            _serviceMock = new Mock<IPersonasService>();
        }

        [Test]
        public void LoadStatistics_ConEstudiantesYDocentes_DeberiaCalcularEstadisticas()
        {
            // Arrange
            _serviceMock.Setup(s => s.CountEstudiantes(false)).Returns(10);
            _serviceMock.Setup(s => s.CountDocentes(false)).Returns(5);
            _serviceMock.Setup(s => s.CountAprobados(It.IsAny<double>(), false)).Returns(7);
            _serviceMock.Setup(s => s.CountSuspensos(It.IsAny<double>(), false)).Returns(3);
            _serviceMock.Setup(s => s.GetEstudiantesPorCiclo(false)).Returns(new Dictionary<Ciclo, int>
            {
                { Ciclo.DAM, 5 },
                { Ciclo.DAW, 3 },
                { Ciclo.ASIR, 2 }
            });
            _serviceMock.Setup(s => s.GetDocentesPorCiclo(false)).Returns(new Dictionary<Ciclo, int>
            {
                { Ciclo.DAM, 2 },
                { Ciclo.DAW, 2 },
                { Ciclo.ASIR, 1 }
            });

            // Act
            var viewModel = new DashboardViewModel(_serviceMock.Object);

            // Assert
            viewModel.TotalEstudiantes.Should().Be(10);
            viewModel.TotalDocentes.Should().Be(5);
            viewModel.PorcentajeAprobados.Should().Be(70);
            viewModel.PorcentajeSuspensos.Should().Be(30);
            viewModel.TotalDAM.Should().Be(7);
            viewModel.TotalDAW.Should().Be(5);
            viewModel.TotalASIR.Should().Be(3);
            viewModel.MensajeEstado.Should().Contain("Estudiantes: 10");
        }

        [Test]
        public void LoadStatistics_SinEstudiantes_DeberiaRetornarCero()
        {
            // Arrange
            _serviceMock.Setup(s => s.CountEstudiantes(false)).Returns(0);
            _serviceMock.Setup(s => s.CountDocentes(false)).Returns(0);
            _serviceMock.Setup(s => s.CountAprobados(It.IsAny<double>(), false)).Returns(0);
            _serviceMock.Setup(s => s.CountSuspensos(It.IsAny<double>(), false)).Returns(0);
            _serviceMock.Setup(s => s.GetEstudiantesPorCiclo(false)).Returns(new Dictionary<Ciclo, int>());
            _serviceMock.Setup(s => s.GetDocentesPorCiclo(false)).Returns(new Dictionary<Ciclo, int>());

            // Act
            var viewModel = new DashboardViewModel(_serviceMock.Object);

            // Assert
            viewModel.TotalEstudiantes.Should().Be(0);
            viewModel.PorcentajeAprobados.Should().Be(0);
            viewModel.PorcentajeSuspensos.Should().Be(0);
        }
    }
}
