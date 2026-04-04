using FluentAssertions;
using GestionAcademica.Models.Personas;
using GestionAcademica.Models.Academia;
using GestionAcademica.Services.Personas;
using GestionAcademica.ViewModels.Graficos;
using NUnit.Framework;
using Moq;

namespace GestionAcademica.Test.ViewModels.Graficos;

[TestFixture]
public class GraficosViewModelTests
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
            var estudiantes = new List<Estudiante>
            {
                new Estudiante { Id = 1, Calificacion = 8.0, Ciclo = Ciclo.DAM },
                new Estudiante { Id = 2, Calificacion = 4.0, Ciclo = Ciclo.DAM },
                new Estudiante { Id = 3, Calificacion = 9.5, Ciclo = Ciclo.DAW }
            };
            var docentes = new List<Docente>
            {
                new Docente { Id = 1, Ciclo = Ciclo.DAM }
            };
            _serviceMock.Setup(s => s.GetEstudiantesOrderBy(GestionAcademica.Enums.TipoOrdenamiento.Dni, 1, 1000, false)).Returns(estudiantes);
            _serviceMock.Setup(s => s.GetDocentesOrderBy(GestionAcademica.Enums.TipoOrdenamiento.Dni, 1, 1000, false)).Returns(docentes);

            // Act
            var viewModel = new GraficosViewModel(_serviceMock.Object);

            // Assert
            viewModel.TotalEstudiantes.Should().Be(3);
            viewModel.TotalDocentes.Should().Be(1);
            viewModel.EstudiantesAprobados.Should().Be(2);
            viewModel.EstudiantesSuspensos.Should().Be(1);
            viewModel.EstudiantesSobresaliente.Should().Be(1);
            viewModel.StatusMessage.Should().Contain("Estadísticas cargadas");
        }

        [Test]
        public void GetCalificacionesData_DeberiaRetornarMediasPorCiclo()
        {
            // Arrange
            var estudiantes = new List<Estudiante>
            {
                new Estudiante { Id = 1, Calificacion = 8.0, Ciclo = Ciclo.DAM },
                new Estudiante { Id = 2, Calificacion = 4.0, Ciclo = Ciclo.DAM },
                new Estudiante { Id = 3, Calificacion = 9.0, Ciclo = Ciclo.DAW }
            };
            _serviceMock.Setup(s => s.GetEstudiantesOrderBy(GestionAcademica.Enums.TipoOrdenamiento.Dni, 1, 1000, false)).Returns(estudiantes);
            var viewModel = new GraficosViewModel(_serviceMock.Object);

            // Act
            var resultado = viewModel.GetCalificacionesData();

            // Assert
            resultado.Should().NotBeEmpty();
            resultado.Should().HaveCount(2);
        }

        [Test]
        public void GetNotasDistribution_DeberiaRetornar4Categorias()
        {
            // Arrange
            var estudiantes = new List<Estudiante>
            {
                new Estudiante { Id = 1, Calificacion = 3.0 },
                new Estudiante { Id = 2, Calificacion = 6.0 },
                new Estudiante { Id = 3, Calificacion = 8.0 },
                new Estudiante { Id = 4, Calificacion = 10.0 }
            };
            _serviceMock.Setup(s => s.GetEstudiantesOrderBy(GestionAcademica.Enums.TipoOrdenamiento.Dni, 1, 1000, false)).Returns(estudiantes);
            var viewModel = new GraficosViewModel(_serviceMock.Object);

            // Act
            var resultado = viewModel.GetNotasDistribution();

            // Assert
            resultado.Should().HaveCount(4);
            resultado[0].Should().Be(1); // Suspenso
            resultado[1].Should().Be(1); // Aprobado
            resultado[2].Should().Be(1); // Notable
            resultado[3].Should().Be(1); // Sobresaliente
        }

        [Test]
        public void GetEstudiantesPorEdad_DeberiaRetornar3Categorias()
        {
            // Arrange
            var estudiantes = new List<Estudiante>
            {
                new Estudiante { Id = 1, FechaNacimiento = DateTime.Now.AddYears(-16) },
                new Estudiante { Id = 2, FechaNacimiento = DateTime.Now.AddYears(-20) },
                new Estudiante { Id = 3, FechaNacimiento = DateTime.Now.AddYears(-30) }
            };
            _serviceMock.Setup(s => s.GetEstudiantesOrderBy(GestionAcademica.Enums.TipoOrdenamiento.Dni, 1, 1000, false)).Returns(estudiantes);
            var viewModel = new GraficosViewModel(_serviceMock.Object);

            // Act
            var resultado = viewModel.GetEstudiantesPorEdad();

            // Assert
            resultado.Should().ContainKey("Menores de 18");
            resultado.Should().ContainKey("18-25 años");
            resultado.Should().ContainKey("Mayores de 25");
            resultado["Menores de 18"].Should().Be(1);
            resultado["18-25 años"].Should().Be(1);
            resultado["Mayores de 25"].Should().Be(1);
        }

        [Test]
        public void GetDocentesPorCiclo_DeberiaRetornarDocentesAgrupados()
        {
            // Arrange
            var docentes = new List<Docente>
            {
                new Docente { Id = 1, Ciclo = Ciclo.DAM },
                new Docente { Id = 2, Ciclo = Ciclo.DAM },
                new Docente { Id = 3, Ciclo = Ciclo.DAW }
            };
            _serviceMock.Setup(s => s.GetDocentesOrderBy(GestionAcademica.Enums.TipoOrdenamiento.Dni, 1, 1000, false)).Returns(docentes);
            var viewModel = new GraficosViewModel(_serviceMock.Object);

            // Act
            var resultado = viewModel.GetDocentesPorCiclo();

            // Assert
            resultado.values.Should().NotBeEmpty();
            resultado.labels.Should().NotBeEmpty();
        }
    }

    [TestFixture]
    public class CasosVacios
    {
        [Test]
        public void LoadStatistics_SinEstudiantes_DeberiaRetornarCero()
        {
            // Arrange
            var serviceMock = new Mock<IPersonasService>();
            serviceMock.Setup(s => s.GetEstudiantesOrderBy(GestionAcademica.Enums.TipoOrdenamiento.Dni, 1, 1000, false)).Returns(new List<Estudiante>());
            serviceMock.Setup(s => s.GetDocentesOrderBy(GestionAcademica.Enums.TipoOrdenamiento.Dni, 1, 1000, false)).Returns(new List<Docente>());

            // Act
            var viewModel = new GraficosViewModel(serviceMock.Object);

            // Assert
            viewModel.TotalEstudiantes.Should().Be(0);
            viewModel.TotalDocentes.Should().Be(0);
        }

        [Test]
        public void GetNotasDistribution_SinEstudiantes_DeberiaRetornarCeros()
        {
            // Arrange
            var serviceMock = new Mock<IPersonasService>();
            serviceMock.Setup(s => s.GetEstudiantesOrderBy(GestionAcademica.Enums.TipoOrdenamiento.Dni, 1, 1000, false)).Returns(new List<Estudiante>());
            var viewModel = new GraficosViewModel(serviceMock.Object);

            // Act
            var resultado = viewModel.GetNotasDistribution();

            // Assert
            resultado.Should().HaveCount(4);
            resultado.Should().AllBeEquivalentTo(0);
        }
    }
}
