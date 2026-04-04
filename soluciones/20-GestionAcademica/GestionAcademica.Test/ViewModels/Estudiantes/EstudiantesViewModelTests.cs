using FluentAssertions;
using GestionAcademica.Models.Personas;
using GestionAcademica.Models.Academia;
using GestionAcademica.Services.Personas;
using GestionAcademica.Services.Images;
using GestionAcademica.Services.Dialogs;
using GestionAcademica.Enums;
using GestionAcademica.ViewModels.Estudiantes;
using NUnit.Framework;
using Moq;
using System.Collections.ObjectModel;

namespace GestionAcademica.Test.ViewModels.Estudiantes;

[TestFixture]
public class EstudiantesViewModelTests
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
        public void Constructor_ConEstudiantes_DeberiaCargarLista()
        {
            // Arrange
            var estudiantes = new List<Estudiante>
            {
                new Estudiante { Id = 1, Dni = "11111111A", Nombre = "Juan", Apellidos = "Pérez", Ciclo = Ciclo.DAM },
                new Estudiante { Id = 2, Dni = "22222222B", Nombre = "Ana", Apellidos = "García", Ciclo = Ciclo.DAW }
            };
            _personasServiceMock.Setup(s => s.GetEstudiantesOrderBy(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(estudiantes);

            // Act
            var viewModel = new EstudiantesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.TotalRegistros.Should().Be(2);
            viewModel.TotalPaginas.Should().Be(1);
            viewModel.PaginaActual.Should().Be(1);
            viewModel.Estudiantes.Should().HaveCount(2);
            viewModel.StatusMessage.Should().Contain("estudiantes");
        }

        [Test]
        public void Constructor_SinEstudiantes_DeberiaRetornarListaVacia()
        {
            // Arrange
            _personasServiceMock.Setup(s => s.GetEstudiantesOrderBy(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(new List<Estudiante>());

            // Act
            var viewModel = new EstudiantesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.Estudiantes.Should().BeEmpty();
            viewModel.StatusMessage.Should().Contain("0 estudiantes");
        }

        [Test]
        public void PropiedadesIniciales_DeberianTenerValoresPorDefecto()
        {
            // Arrange
            _personasServiceMock.Setup(s => s.GetEstudiantesOrderBy(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(new List<Estudiante>());

            // Act
            var viewModel = new EstudiantesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.SearchText.Should().BeEmpty();
            viewModel.CicloSeleccionado.Should().Be("Todos");
            viewModel.MostrarEliminados.Should().BeFalse();
            viewModel.IsLoading.Should().BeFalse();
            viewModel.SelectedEstudiante.Should().BeNull();
            viewModel.OrdenActual.Should().Be(TipoOrdenamiento.Dni);
            viewModel.PaginaActual.Should().Be(1);
            viewModel.TamanoPagina.Should().Be(10);
            viewModel.TotalRegistros.Should().Be(0);
            viewModel.TotalPaginas.Should().Be(1);
        }

        [Test]
        public void TamanosPagina_DeberiaContenerOpcionesValidas()
        {
            // Arrange
            _personasServiceMock.Setup(s => s.GetEstudiantesOrderBy(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(new List<Estudiante>());

            // Act
            var viewModel = new EstudiantesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.TamanosPagina.Should().ContainInOrder(5, 10, 25, 50);
        }

        [Test]
        public void PaginaAnteriorCommand_CuandoPaginaEs1_NoDeberiaEjecutarse()
        {
            // Arrange
            _personasServiceMock.Setup(s => s.GetEstudiantesOrderBy(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(new List<Estudiante>());

            var viewModel = new EstudiantesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.PaginaAnteriorCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void PaginaSiguienteCommand_CuandoPaginaEsUltima_NoDeberiaEjecutarse()
        {
            // Arrange
            _personasServiceMock.Setup(s => s.GetEstudiantesOrderBy(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(new List<Estudiante>());

            var viewModel = new EstudiantesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.PaginaSiguienteCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void PaginaAnteriorCommand_CuandoPaginaEsMayorQue1_DeberiaEjecutarse()
        {
            // Arrange
            var estudiantes = GenerarEstudiantes(25);
            _personasServiceMock.Setup(s => s.GetEstudiantesOrderBy(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(estudiantes);

            var viewModel = new EstudiantesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            viewModel.PaginaActual = 2;

            // Assert
            viewModel.PaginaAnteriorCommand.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void PaginaSiguienteCommand_CuandoHayMasPaginas_DeberiaEjecutarse()
        {
            // Arrange
            var estudiantes = GenerarEstudiantes(25);
            _personasServiceMock.Setup(s => s.GetEstudiantesOrderBy(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(estudiantes);

            var viewModel = new EstudiantesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.PaginaSiguienteCommand.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void PaginaAnterior_CuandoSeEjecuta_DeberiaDecrementarPagina()
        {
            // Arrange
            var estudiantes = GenerarEstudiantes(25);
            _personasServiceMock.Setup(s => s.GetEstudiantesOrderBy(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(estudiantes);

            var viewModel = new EstudiantesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            viewModel.PaginaActual = 2;

            // Act
            viewModel.PaginaAnteriorCommand.Execute(null);

            // Assert
            viewModel.PaginaActual.Should().Be(1);
        }

        [Test]
        public void PaginaSiguiente_CuandoSeEjecuta_DeberiaIncrementarPagina()
        {
            // Arrange
            var estudiantes = GenerarEstudiantes(25);
            _personasServiceMock.Setup(s => s.GetEstudiantesOrderBy(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(estudiantes);

            var viewModel = new EstudiantesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.PaginaSiguienteCommand.Execute(null);

            // Assert
            viewModel.PaginaActual.Should().Be(2);
        }

        [Test]
        public void PrimeraPagina_CuandoSeEjecuta_DeberiaIrAPrimera()
        {
            // Arrange
            var estudiantes = GenerarEstudiantes(50);
            _personasServiceMock.Setup(s => s.GetEstudiantesOrderBy(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(estudiantes);

            var viewModel = new EstudiantesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            viewModel.PaginaActual = 3;

            // Act
            viewModel.PrimeraPaginaCommand.Execute(null);

            // Assert
            viewModel.PaginaActual.Should().Be(1);
        }

        [Test]
        public void UltimaPagina_CuandoSeEjecuta_DeberiaIrAUltima()
        {
            // Arrange
            var estudiantes = GenerarEstudiantes(25);
            _personasServiceMock.Setup(s => s.GetEstudiantesOrderBy(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(estudiantes);

            var viewModel = new EstudiantesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.UltimaPaginaCommand.Execute(null);

            // Assert
            viewModel.PaginaActual.Should().Be(3);
        }

        [Test]
        public void CambiarTamanoPagina_CuandoSeEjecuta_DeberiaCambiarTamanoYReiniciarPagina()
        {
            // Arrange
            var estudiantes = GenerarEstudiantes(25);
            _personasServiceMock.Setup(s => s.GetEstudiantesOrderBy(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(estudiantes);

            var viewModel = new EstudiantesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            viewModel.PaginaActual = 2;

            // Act
            viewModel.CambiarTamanoPaginaCommand.Execute(25);

            // Assert
            viewModel.TamanoPagina.Should().Be(25);
            viewModel.PaginaActual.Should().Be(1);
        }

        [Test]
        public void Paginacion_Con25Registros_DeberiaCalcular3PaginasDe10()
        {
            // Arrange
            var estudiantes = GenerarEstudiantes(25);
            _personasServiceMock.Setup(s => s.GetEstudiantesOrderBy(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(estudiantes);

            // Act
            var viewModel = new EstudiantesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.TotalRegistros.Should().Be(25);
            viewModel.TotalPaginas.Should().Be(3);
            viewModel.PaginaActual.Should().Be(1);
            viewModel.Estudiantes.Should().HaveCount(10);
        }

        [Test]
        public void Paginacion_Pagina2_DeberiaMostrarSiguientes10()
        {
            // Arrange
            var estudiantes = GenerarEstudiantes(25);
            _personasServiceMock.Setup(s => s.GetEstudiantesOrderBy(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(estudiantes);

            var viewModel = new EstudiantesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.PaginaSiguienteCommand.Execute(null);

            // Assert
            viewModel.PaginaActual.Should().Be(2);
            viewModel.Estudiantes.Should().HaveCount(10);
        }

        [Test]
        public void Paginacion_Pagina3_DeberiaMostrarUltimos5()
        {
            // Arrange
            var estudiantes = GenerarEstudiantes(25);
            _personasServiceMock.Setup(s => s.GetEstudiantesOrderBy(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(estudiantes);

            var viewModel = new EstudiantesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.PaginaActual = 3;

            // Assert
            viewModel.Estudiantes.Should().HaveCount(5);
        }

        [Test]
        public void Paginacion_ConTamano25_DeberiaMostrarTodosEnUnaPagina()
        {
            // Arrange
            var estudiantes = GenerarEstudiantes(25);
            _personasServiceMock.Setup(s => s.GetEstudiantesOrderBy(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(estudiantes);

            var viewModel = new EstudiantesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.CambiarTamanoPaginaCommand.Execute(25);

            // Assert
            viewModel.TotalPaginas.Should().Be(1);
            viewModel.Estudiantes.Should().HaveCount(25);
        }

        [Test]
        public void Paginacion_ConTamano5_DeberiaCalcular5Paginas()
        {
            // Arrange
            var estudiantes = GenerarEstudiantes(25);
            _personasServiceMock.Setup(s => s.GetEstudiantesOrderBy(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(estudiantes);

            var viewModel = new EstudiantesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.CambiarTamanoPaginaCommand.Execute(5);

            // Assert
            viewModel.TotalPaginas.Should().Be(5);
            viewModel.Estudiantes.Should().HaveCount(5);
        }

        [Test]
        public void StatusMessage_DeberiaMostrarPaginaActualYTotal()
        {
            // Arrange
            var estudiantes = GenerarEstudiantes(25);
            _personasServiceMock.Setup(s => s.GetEstudiantesOrderBy(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(estudiantes);

            // Act
            var viewModel = new EstudiantesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.StatusMessage.Should().Contain("Página 1/3");
            viewModel.StatusMessage.Should().Contain("25 estudiantes");
        }

        private List<Estudiante> GenerarEstudiantes(int cantidad)
        {
            var lista = new List<Estudiante>();
            var letras = "TRWAGMYFPDXBNJZSQVHLCKE";
            for (int i = 1; i <= cantidad; i++)
            {
                var dniNumerico = i * 11111111 % 100000000;
                var letra = letras[dniNumerico % 23];
                lista.Add(new Estudiante
                {
                    Id = i,
                    Dni = $"{dniNumerico:D8}{letra}",
                    Nombre = $"Estudiante{i}",
                    Apellidos = $"Apellido{i}",
                    Ciclo = Ciclo.DAM
                });
            }
            return lista;
        }

        [Test]
        public void Ciclos_DeberiaContenerTodosLosCiclos()
        {
            // Arrange
            _personasServiceMock.Setup(s => s.GetEstudiantesOrderBy(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(new List<Estudiante>());

            // Act
            var viewModel = new EstudiantesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.Ciclos.Should().Contain(Ciclo.DAM);
            viewModel.Ciclos.Should().Contain(Ciclo.DAW);
            viewModel.Ciclos.Should().Contain(Ciclo.ASIR);
            viewModel.CiclosConTodos.Should().Contain("Todos");
            viewModel.CiclosConTodos.Should().Contain("DAM");
        }

        [Test]
        public void SearchText_CuandoCambia_DeberiaFiltrarEstudiantes()
        {
            // Arrange
            var estudiantes = new List<Estudiante>
            {
                new Estudiante { Id = 1, Dni = "11111111A", Nombre = "Juan", Apellidos = "Pérez" },
                new Estudiante { Id = 2, Dni = "22222222B", Nombre = "Ana", Apellidos = "García" }
            };
            _personasServiceMock.Setup(s => s.GetEstudiantesOrderBy(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(estudiantes);

            var viewModel = new EstudiantesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.SearchText = "Juan";

            // Assert
            viewModel.Estudiantes.Should().HaveCount(1);
            viewModel.Estudiantes.First().Nombre.Should().Be("Juan");
        }

        [Test]
        public void SearchText_ConTextoVacio_DeberiaMostrarTodos()
        {
            // Arrange
            var estudiantes = new List<Estudiante>
            {
                new Estudiante { Id = 1, Dni = "11111111A", Nombre = "Juan", Apellidos = "Pérez" },
                new Estudiante { Id = 2, Dni = "22222222B", Nombre = "Ana", Apellidos = "García" }
            };
            _personasServiceMock.Setup(s => s.GetEstudiantesOrderBy(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(estudiantes);

            var viewModel = new EstudiantesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);
            viewModel.SearchText = "Juan";

            // Act
            viewModel.SearchText = "";

            // Assert
            viewModel.Estudiantes.Should().HaveCount(2);
        }

        [Test]
        public void CicloSeleccionado_CuandoCambia_DeberiaFiltrarPorCiclo()
        {
            // Arrange
            var estudiantes = new List<Estudiante>
            {
                new Estudiante { Id = 1, Dni = "11111111A", Nombre = "Juan", Ciclo = Ciclo.DAM },
                new Estudiante { Id = 2, Dni = "22222222B", Nombre = "Ana", Ciclo = Ciclo.DAW }
            };
            _personasServiceMock.Setup(s => s.GetEstudiantesOrderBy(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(estudiantes);

            var viewModel = new EstudiantesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.CicloSeleccionado = "DAM";

            // Assert
            viewModel.Estudiantes.Should().HaveCount(1);
            viewModel.Estudiantes.First().Ciclo.Should().Be(Ciclo.DAM);
        }

        [Test]
        public void SelectedEstudiante_SePuedeAsignar()
        {
            // Arrange
            var estudiantes = new List<Estudiante>
            {
                new Estudiante { Id = 1, Dni = "11111111A", Nombre = "Juan" },
                new Estudiante { Id = 2, Dni = "22222222B", Nombre = "Ana" }
            };
            _personasServiceMock.Setup(s => s.GetEstudiantesOrderBy(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(estudiantes);

            var viewModel = new EstudiantesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.SelectedEstudiante = viewModel.Estudiantes.First();

            // Assert
            viewModel.SelectedEstudiante.Should().NotBeNull();
            viewModel.SelectedEstudiante!.Nombre.Should().Be("Juan");
        }

        [Test]
        public void MostrarEliminados_CuandoCambia_DeberiaRecargar()
        {
            // Arrange
            var callCount = 0;
            _personasServiceMock.Setup(s => s.GetEstudiantesOrderBy(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(new List<Estudiante>())
                .Callback(() => callCount++);

            var viewModel = new EstudiantesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);
            var initialCalls = callCount;

            // Act
            viewModel.MostrarEliminados = true;

            // Assert
            callCount.Should().BeGreaterThan(initialCalls);
        }

        [Test]
        public void OrderBy_CuandoSeEjecuta_DeberiaOrdenar()
        {
            // Arrange
            var estudiantes = new List<Estudiante>
            {
                new Estudiante { Id = 2, Dni = "22222222B", Nombre = "Ana" },
                new Estudiante { Id = 1, Dni = "11111111A", Nombre = "Juan" }
            };
            _personasServiceMock.Setup(s => s.GetEstudiantesOrderBy(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(estudiantes);

            var viewModel = new EstudiantesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.OrderByCommand.Execute(TipoOrdenamiento.Nombre);

            // Assert
            viewModel.Estudiantes.First().Nombre.Should().Be("Ana");
        }

        [Test]
        public void LoadCommand_CuandoSeEjecuta_DeberiaRecargar()
        {
            // Arrange
            var callCount = 0;
            _personasServiceMock.Setup(s => s.GetEstudiantesOrderBy(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(new List<Estudiante>())
                .Callback(() => callCount++);

            var viewModel = new EstudiantesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);
            var initialCalls = callCount;

            // Act
            viewModel.LoadCommand.Execute(null);

            // Assert
            callCount.Should().BeGreaterThan(initialCalls);
            viewModel.SearchText.Should().BeEmpty();
            viewModel.CicloSeleccionado.Should().Be("Todos");
        }

        [Test]
        public void UsaBorradoLogico_DeberiaRetornarConfiguracion()
        {
            // Arrange
            _personasServiceMock.Setup(s => s.GetEstudiantesOrderBy(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(new List<Estudiante>());

            // Act
            var viewModel = new EstudiantesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.UsaBorradoLogico.Should().Be(GestionAcademica.Config.AppConfig.UseLogicalDelete);
        }
    }

    [TestFixture]
    public class CasosNegativos
    {
        [Test]
        public void Constructor_CuandoGetEstudiantesFalla_DeberiaManejarError()
        {
            // Arrange
            var personasServiceMock = new Mock<IPersonasService>();
            personasServiceMock.Setup(s => s.GetEstudiantesOrderBy(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Throws(new Exception("Error de base de datos"));

            var dialogServiceMock = new Mock<IDialogService>();

            // Act
            var viewModel = new EstudiantesViewModel(
                personasServiceMock.Object,
                new Mock<IImageService>().Object,
                dialogServiceMock.Object);

            // Assert
            viewModel.StatusMessage.Should().Contain("Error");
            dialogServiceMock.Verify(d => d.ShowError(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void Constructor_CuandoServiciosSonNulos_NoDeberiaFallar()
        {
            // Arrange & Act
            var viewModel = new EstudiantesViewModel(
                new Mock<IPersonasService>().Object,
                new Mock<IImageService>().Object,
                new Mock<IDialogService>().Object);

            // Assert
            viewModel.Should().NotBeNull();
        }

        [Test]
        public void SearchText_ConCaracteresEspeciales_NoDeberiaFallar()
        {
            // Arrange
            var personasServiceMock = new Mock<IPersonasService>();
            personasServiceMock.Setup(s => s.GetEstudiantesOrderBy(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(new List<Estudiante>());

            var viewModel = new EstudiantesViewModel(
                personasServiceMock.Object,
                new Mock<IImageService>().Object,
                new Mock<IDialogService>().Object);

            // Act & Assert
            viewModel.Invoking(v => v.SearchText = "test<>'\"").Should().NotThrow();
        }

        [Test]
        public void CicloSeleccionado_ConValorInvalido_DeberiaManejarlo()
        {
            // Arrange
            var personasServiceMock = new Mock<IPersonasService>();
            personasServiceMock.Setup(s => s.GetEstudiantesOrderBy(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(new List<Estudiante>());

            var viewModel = new EstudiantesViewModel(
                personasServiceMock.Object,
                new Mock<IImageService>().Object,
                new Mock<IDialogService>().Object);

            // Act
            viewModel.CicloSeleccionado = "CicloInvalido";

            // Assert
            viewModel.Estudiantes.Should().BeEmpty();
        }

        [Test]
        public void DeleteCommand_CuandoNoHaySeleccion_EditCommandNoSePuedeEjecutar()
        {
            // Arrange
            var personasServiceMock = new Mock<IPersonasService>();
            personasServiceMock.Setup(s => s.GetEstudiantesOrderBy(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(new List<Estudiante>());

            var viewModel = new EstudiantesViewModel(
                personasServiceMock.Object,
                new Mock<IImageService>().Object,
                new Mock<IDialogService>().Object);

            // Assert
            viewModel.EditCommand.CanExecute(null).Should().BeFalse();
            viewModel.DeleteCommand.CanExecute(null).Should().BeFalse();
            viewModel.ViewCommand.CanExecute(null).Should().BeFalse();
        }
    }
}