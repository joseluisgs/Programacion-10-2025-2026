using FluentAssertions;
using GestionAcademica.Enums;
using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Personas;
using GestionAcademica.Services.Personas;
using GestionAcademica.ViewModels.Graficos;
using Moq;

namespace GestionAcademica.Test.ViewModels.Graficos;

[TestFixture]
public class GraficosViewModelTests {
    [TestFixture]
    public class CasosPositivos {
        [SetUp]
        public void SetUp() {
            _serviceMock = new Mock<IPersonasService>();
        }

        private Mock<IPersonasService> _serviceMock = null!;

        [Test]
        public void LoadStatistics_ConEstudiantesYDocentes_DeberiaCalcularEstadisticas() {
            // Arrange
            var estudiantes = new List<Estudiante> {
                new() { Id = 1, Calificacion = 8.0, Ciclo = Ciclo.DAM },
                new() { Id = 2, Calificacion = 4.0, Ciclo = Ciclo.DAM },
                new() { Id = 3, Calificacion = 9.5, Ciclo = Ciclo.DAW }
            };
            var docentes = new List<Docente> {
                new() { Id = 1, Ciclo = Ciclo.DAM }
            };
            _serviceMock.Setup(s => s.GetEstudiantesOrderBy(TipoOrdenamiento.Dni, 1, 1000, false)).Returns(estudiantes);
            _serviceMock.Setup(s => s.GetDocentesOrderBy(TipoOrdenamiento.Dni, 1, 1000, false)).Returns(docentes);

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
        public void GetCalificacionesData_DeberiaRetornarMediasPorCiclo() {
            // Arrange
            var estudiantes = new List<Estudiante> {
                new() { Id = 1, Calificacion = 8.0, Ciclo = Ciclo.DAM },
                new() { Id = 2, Calificacion = 4.0, Ciclo = Ciclo.DAM },
                new() { Id = 3, Calificacion = 9.0, Ciclo = Ciclo.DAW }
            };
            _serviceMock.Setup(s => s.GetEstudiantesOrderBy(TipoOrdenamiento.Dni, 1, 1000, false)).Returns(estudiantes);
            var viewModel = new GraficosViewModel(_serviceMock.Object);

            // Act
            var resultado = viewModel.GetCalificacionesData();

            // Assert
            resultado.Should().NotBeEmpty();
            resultado.Should().HaveCount(2);
        }

        [Test]
        public void GetNotasDistribution_DeberiaRetornar4Categorias() {
            // Arrange
            var estudiantes = new List<Estudiante> {
                new() { Id = 1, Calificacion = 3.0 },
                new() { Id = 2, Calificacion = 6.0 },
                new() { Id = 3, Calificacion = 8.0 },
                new() { Id = 4, Calificacion = 10.0 }
            };
            _serviceMock.Setup(s => s.GetEstudiantesOrderBy(TipoOrdenamiento.Dni, 1, 1000, false)).Returns(estudiantes);
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
        public void GetEstudiantesPorEdad_DeberiaRetornar3Categorias() {
            // Arrange
            var estudiantes = new List<Estudiante> {
                new() { Id = 1, FechaNacimiento = DateTime.Now.AddYears(-16) },
                new() { Id = 2, FechaNacimiento = DateTime.Now.AddYears(-20) },
                new() { Id = 3, FechaNacimiento = DateTime.Now.AddYears(-30) }
            };
            _serviceMock.Setup(s => s.GetEstudiantesOrderBy(TipoOrdenamiento.Dni, 1, 1000, false)).Returns(estudiantes);
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
        public void GetDocentesPorCiclo_DeberiaRetornarDocentesAgrupados() {
            // Arrange
            var docentes = new List<Docente> {
                new() { Id = 1, Ciclo = Ciclo.DAM },
                new() { Id = 2, Ciclo = Ciclo.DAM },
                new() { Id = 3, Ciclo = Ciclo.DAW }
            };
            _serviceMock.Setup(s => s.GetDocentesOrderBy(TipoOrdenamiento.Dni, 1, 1000, false)).Returns(docentes);
            var viewModel = new GraficosViewModel(_serviceMock.Object);

            // Act
            var resultado = viewModel.GetDocentesPorCiclo();

            // Assert
            resultado.values.Should().NotBeEmpty();
            resultado.labels.Should().NotBeEmpty();
        }
    }

    [TestFixture]
    public class CasosPositivosAdicionales {
        [SetUp]
        public void SetUp() {
            _serviceMock = new Mock<IPersonasService>();
        }

        private Mock<IPersonasService> _serviceMock = null!;

        [Test]
        public void GetTasaAprobadosPorCiclo_DeberiaRetornarPorcentajes() {
            // Arrange
            var estudiantes = new List<Estudiante> {
                new() { Id = 1, Calificacion = 8.0, Ciclo = Ciclo.DAM },
                new() { Id = 2, Calificacion = 4.0, Ciclo = Ciclo.DAM },
                new() { Id = 3, Calificacion = 9.0, Ciclo = Ciclo.DAW },
                new() { Id = 4, Calificacion = 3.0, Ciclo = Ciclo.DAW }
            };
            _serviceMock.Setup(s => s.GetEstudiantesOrderBy(TipoOrdenamiento.Dni, 1, 1000, false)).Returns(estudiantes);
            var viewModel = new GraficosViewModel(_serviceMock.Object);

            // Act
            var resultado = viewModel.GetTasaAprobadosPorCiclo();

            // Assert
            resultado.aprobados.Should().HaveCount(2);
            resultado.suspensos.Should().HaveCount(2);
            resultado.ciclos.Should().HaveCount(2);
            resultado.aprobados[0].Should().Be(50); // DAM: 1 de 2
            resultado.suspensos[0].Should().Be(50);
            resultado.aprobados[1].Should().Be(50); // DAW: 1 de 2
            resultado.suspensos[1].Should().Be(50);
        }

        [Test]
        public void GetEstudiantesPorCurso_DeberiaRetornarPrimeroYSegundo() {
            // Arrange
            var estudiantes = new List<Estudiante> {
                new() { Id = 1, Ciclo = Ciclo.DAM, Curso = Curso.Primero },
                new() { Id = 2, Ciclo = Ciclo.DAM, Curso = Curso.Primero },
                new() { Id = 3, Ciclo = Ciclo.DAM, Curso = Curso.Segundo },
                new() { Id = 4, Ciclo = Ciclo.DAW, Curso = Curso.Segundo }
            };
            _serviceMock.Setup(s => s.GetEstudiantesOrderBy(TipoOrdenamiento.Dni, 1, 1000, false)).Returns(estudiantes);
            var viewModel = new GraficosViewModel(_serviceMock.Object);

            // Act
            var resultado = viewModel.GetEstudiantesPorCurso();

            // Assert
            resultado.primero.Should().Be(2);
            resultado.segundo.Should().Be(2);
            resultado.ciclos.Should().HaveCount(2);
        }

        [Test]
        public void GetEstudiantesCantidadPorCiclo_DeberiaRetornarCantidades() {
            // Arrange
            var estudiantes = new List<Estudiante> {
                new() { Id = 1, Ciclo = Ciclo.DAM },
                new() { Id = 2, Ciclo = Ciclo.DAM },
                new() { Id = 3, Ciclo = Ciclo.DAW }
            };
            _serviceMock.Setup(s => s.GetEstudiantesOrderBy(TipoOrdenamiento.Dni, 1, 1000, false)).Returns(estudiantes);
            var viewModel = new GraficosViewModel(_serviceMock.Object);

            // Act
            var resultado = viewModel.GetEstudiantesCantidadPorCiclo();

            // Assert
            resultado.values.Should().HaveCount(2);
            resultado.values[0].Should().Be(2);
            resultado.values[1].Should().Be(1);
            resultado.labels.Should().Contain("DAM");
            resultado.labels.Should().Contain("DAW");
        }
    }

    [TestFixture]
    public class CasosVacios {
        [Test]
        public void LoadStatistics_SinEstudiantes_DeberiaRetornarCero() {
            // Arrange
            var serviceMock = new Mock<IPersonasService>();
            serviceMock.Setup(s => s.GetEstudiantesOrderBy(TipoOrdenamiento.Dni, 1, 1000, false))
                .Returns(new List<Estudiante>());
            serviceMock.Setup(s => s.GetDocentesOrderBy(TipoOrdenamiento.Dni, 1, 1000, false))
                .Returns(new List<Docente>());

            // Act
            var viewModel = new GraficosViewModel(serviceMock.Object);

            // Assert
            viewModel.TotalEstudiantes.Should().Be(0);
            viewModel.TotalDocentes.Should().Be(0);
        }

        [Test]
        public void GetNotasDistribution_SinEstudiantes_DeberiaRetornarCeros() {
            // Arrange
            var serviceMock = new Mock<IPersonasService>();
            serviceMock.Setup(s => s.GetEstudiantesOrderBy(TipoOrdenamiento.Dni, 1, 1000, false))
                .Returns(new List<Estudiante>());
            var viewModel = new GraficosViewModel(serviceMock.Object);

            // Act
            var resultado = viewModel.GetNotasDistribution();

            // Assert
            resultado.Should().HaveCount(4);
            resultado.Should().AllBeEquivalentTo(0);
        }
    }
}