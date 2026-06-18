using FluentAssertions;
using GestionAcademica.Models.Academia;
using GestionAcademica.Services.Personas;
using GestionAcademica.ViewModels.Dashboard;
using Moq;

namespace GestionAcademica.Test.ViewModels.Dashboard;

[TestFixture]
public class DashboardViewModelTests {
    [TestFixture]
    public class CasosPositivos {
        [SetUp]
        public void SetUp() {
            _serviceMock = new Mock<IPersonasService>();
        }

        private Mock<IPersonasService> _serviceMock = null!;

        [Test]
        public async Task Constructor_ConEstudiantesYDocentes_DeberiaCalcularEstadisticas() {
            // Arrange
            _serviceMock.Setup(s => s.CountEstudiantesAsync(false)).ReturnsAsync(10);
            _serviceMock.Setup(s => s.CountDocentesAsync(false)).ReturnsAsync(5);
            _serviceMock.Setup(s => s.CountAprobadosAsync(It.IsAny<double>(), false)).ReturnsAsync(7);
            _serviceMock.Setup(s => s.CountSuspensosAsync(It.IsAny<double>(), false)).ReturnsAsync(3);
            _serviceMock.Setup(s => s.GetEstudiantesPorCicloAsync(false)).ReturnsAsync(new Dictionary<Ciclo, int> {
                { Ciclo.DAM, 5 },
                { Ciclo.DAW, 3 },
                { Ciclo.ASIR, 2 }
            });
            _serviceMock.Setup(s => s.GetDocentesPorCicloAsync(false)).ReturnsAsync(new Dictionary<Ciclo, int> {
                { Ciclo.DAM, 2 },
                { Ciclo.DAW, 2 },
                { Ciclo.ASIR, 1 }
            });

            // Act
            var viewModel = new DashboardViewModel(_serviceMock.Object);
            await Task.Delay(100);

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
        public async Task Constructor_SinEstudiantes_DeberiaRetornarCero() {
            // Arrange
            _serviceMock.Setup(s => s.CountEstudiantesAsync(false)).ReturnsAsync(0);
            _serviceMock.Setup(s => s.CountDocentesAsync(false)).ReturnsAsync(0);
            _serviceMock.Setup(s => s.CountAprobadosAsync(It.IsAny<double>(), false)).ReturnsAsync(0);
            _serviceMock.Setup(s => s.CountSuspensosAsync(It.IsAny<double>(), false)).ReturnsAsync(0);
            _serviceMock.Setup(s => s.GetEstudiantesPorCicloAsync(false)).ReturnsAsync(new Dictionary<Ciclo, int>());
            _serviceMock.Setup(s => s.GetDocentesPorCicloAsync(false)).ReturnsAsync(new Dictionary<Ciclo, int>());

            // Act
            var viewModel = new DashboardViewModel(_serviceMock.Object);
            await Task.Delay(100);

            // Assert
            viewModel.TotalEstudiantes.Should().Be(0);
            viewModel.PorcentajeAprobados.Should().Be(0);
            viewModel.PorcentajeSuspensos.Should().Be(0);
        }
    }
}
