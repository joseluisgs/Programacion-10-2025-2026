using FluentAssertions;
using GestionAcademica.Config;
using GestionAcademica.Enums;
using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Personas;
using GestionAcademica.Services.Dialogs;
using GestionAcademica.Services.Images;
using GestionAcademica.Services.Personas;
using GestionAcademica.ViewModels.Docentes;
using Moq;

namespace GestionAcademica.Test.ViewModels.Docentes;

[TestFixture]
public class DocentesViewModelTests {
    [TestFixture]
    public class CasosPositivos {
        [SetUp]
        public void SetUp() {
            _personasServiceMock = new Mock<IPersonasService>();
            _imageServiceMock = new Mock<IImageService>();
            _dialogServiceMock = new Mock<IDialogService>();
        }

        private Mock<IPersonasService> _personasServiceMock = null!;
        private Mock<IImageService> _imageServiceMock = null!;
        private Mock<IDialogService> _dialogServiceMock = null!;

        [Test]
        public async Task Constructor_ConDocentes_DeberiaCargarLista() {
            // Arrange
            var docentes = new List<Docente> {
                new() { Id = 1, Dni = "11111111A", Nombre = "Juan", Apellidos = "Pérez", Ciclo = Ciclo.DAM },
                new() { Id = 2, Dni = "22222222B", Nombre = "Ana", Apellidos = "García", Ciclo = Ciclo.DAW }
            };
            _personasServiceMock.Setup(s =>
                    s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(docentes);

            // Act
            var viewModel = new DocentesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.Docentes.Should().HaveCount(2);
            viewModel.StatusMessage.Should().Contain("docentes");
        }

        [Test]
        public async Task Constructor_SinDocentes_DeberiaRetornarListaVacia() {
            // Arrange
            _personasServiceMock.Setup(s =>
                    s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(new List<Docente>());

            // Act
            var viewModel = new DocentesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.Docentes.Should().BeEmpty();
            viewModel.StatusMessage.Should().Contain("0 docentes");
        }

        [Test]
        public async Task PropiedadesIniciales_DeberianTenerValoresPorDefecto() {
            // Arrange
            _personasServiceMock.Setup(s =>
                    s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(new List<Docente>());

            // Act
            var viewModel = new DocentesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.SearchText.Should().BeEmpty();
            viewModel.CicloSeleccionado.Should().Be("Todos");
            viewModel.MostrarEliminados.Should().BeFalse();
            viewModel.IsLoading.Should().BeFalse();
            viewModel.SelectedDocente.Should().BeNull();
            viewModel.OrdenActual.Should().Be(TipoOrdenamiento.Dni);
            viewModel.PaginaActual.Should().Be(1);
            viewModel.TamanoPagina.Should().Be(10);
            viewModel.TotalRegistros.Should().Be(0);
            viewModel.TotalPaginas.Should().Be(1);
        }

        [Test]
        public async Task Ciclos_DeberiaContenerTodosLosCiclos() {
            // Arrange
            _personasServiceMock.Setup(s =>
                    s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(new List<Docente>());

            // Act
            var viewModel = new DocentesViewModel(
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
        public async Task SearchText_CuandoCambia_DeberiaFiltrarDocentes() {
            // Arrange
            var docentes = new List<Docente> {
                new() { Id = 1, Dni = "11111111A", Nombre = "Juan", Apellidos = "Pérez" },
                new() { Id = 2, Dni = "22222222B", Nombre = "Ana", Apellidos = "García" }
            };
            _personasServiceMock.Setup(s =>
                    s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(docentes);

            var viewModel = new DocentesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.SearchText = "Juan";

            // Assert
            viewModel.Docentes.Should().HaveCount(1);
            viewModel.Docentes.First().Nombre.Should().Be("Juan");
        }

        [Test]
        public async Task SearchText_ConTextoVacio_DeberiaMostrarTodos() {
            // Arrange
            var docentes = new List<Docente> {
                new() { Id = 1, Dni = "11111111A", Nombre = "Juan", Apellidos = "Pérez" },
                new() { Id = 2, Dni = "22222222B", Nombre = "Ana", Apellidos = "García" }
            };
            _personasServiceMock.Setup(s =>
                    s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(docentes);

            var viewModel = new DocentesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);
            viewModel.SearchText = "Juan";

            // Act
            viewModel.SearchText = "";

            // Assert
            viewModel.Docentes.Should().HaveCount(2);
        }

        [Test]
        public async Task CicloSeleccionado_CuandoCambia_DeberiaFiltrarPorCiclo() {
            // Arrange
            var docentes = new List<Docente> {
                new() { Id = 1, Dni = "11111111A", Nombre = "Juan", Ciclo = Ciclo.DAM },
                new() { Id = 2, Dni = "22222222B", Nombre = "Ana", Ciclo = Ciclo.DAW }
            };
            _personasServiceMock.Setup(s =>
                    s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(docentes);

            var viewModel = new DocentesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.CicloSeleccionado = "DAM";

            // Assert
            viewModel.Docentes.Should().HaveCount(1);
            viewModel.Docentes.First().Ciclo.Should().Be(Ciclo.DAM);
        }

        [Test]
        public async Task SelectedDocente_SePuedeAsignar() {
            // Arrange
            var docentes = new List<Docente> {
                new() { Id = 1, Dni = "11111111A", Nombre = "Juan" },
                new() { Id = 2, Dni = "22222222B", Nombre = "Ana" }
            };
            _personasServiceMock.Setup(s =>
                    s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(docentes);

            var viewModel = new DocentesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.SelectedDocente = viewModel.Docentes.First();

            // Assert
            viewModel.SelectedDocente.Should().NotBeNull();
            viewModel.SelectedDocente!.Nombre.Should().Be("Juan");
        }

        [Test]
        public async Task MostrarEliminados_CuandoCambia_DeberiaRecargar() {
            // Arrange
            var callCount = 0;
            _personasServiceMock.Setup(s =>
                    s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(new List<Docente>())
                .Callback(() => callCount++);

            var viewModel = new DocentesViewModel(
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
        public async Task OrderBy_CuandoSeEjecuta_DeberiaOrdenar() {
            // Arrange
            var docentes = new List<Docente> {
                new() { Id = 2, Dni = "22222222B", Nombre = "Ana" },
                new() { Id = 1, Dni = "11111111A", Nombre = "Juan" }
            };
            _personasServiceMock.Setup(s =>
                    s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(docentes);

            var viewModel = new DocentesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.OrderByCommand.Execute(TipoOrdenamiento.Nombre);

            // Assert
            viewModel.Docentes.First().Nombre.Should().Be("Ana");
        }

        [Test]
        public async Task LoadCommand_CuandoSeEjecuta_DeberiaRecargar() {
            // Arrange
            var callCount = 0;
            _personasServiceMock.Setup(s =>
                    s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(new List<Docente>())
                .Callback(() => callCount++);

            var viewModel = new DocentesViewModel(
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
        public async Task UsaBorradoLogico_DeberiaRetornarConfiguracion() {
            // Arrange
            _personasServiceMock.Setup(s =>
                    s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(new List<Docente>());

            // Act
            var viewModel = new DocentesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.UsaBorradoLogico.Should().Be(AppConfig.UseLogicalDelete);
        }

        [Test]
        public async Task TamanosPagina_DeberiaContenerOpcionesValidas() {
            // Arrange
            _personasServiceMock.Setup(s =>
                    s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(new List<Docente>());

            // Act
            var viewModel = new DocentesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.TamanosPagina.Should().ContainInOrder(5, 10, 25, 50);
        }

        [Test]
        public async Task PaginaAnteriorCommand_CuandoPaginaEs1_NoDeberiaEjecutarse() {
            // Arrange
            _personasServiceMock.Setup(s =>
                    s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(new List<Docente>());

            var viewModel = new DocentesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.PaginaAnteriorCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public async Task PaginaSiguienteCommand_CuandoPaginaEsUltima_NoDeberiaEjecutarse() {
            // Arrange
            _personasServiceMock.Setup(s =>
                    s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(new List<Docente>());

            var viewModel = new DocentesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.PaginaSiguienteCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public async Task PaginaAnterior_CuandoSeEjecuta_DeberiaDecrementarPagina() {
            // Arrange
            var docentes = GenerarDocentes(25);
            _personasServiceMock.Setup(s =>
                    s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(docentes);

            var viewModel = new DocentesViewModel(
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
        public async Task PaginaSiguiente_CuandoSeEjecuta_DeberiaIncrementarPagina() {
            // Arrange
            var docentes = GenerarDocentes(25);
            _personasServiceMock.Setup(s =>
                    s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(docentes);

            var viewModel = new DocentesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.PaginaSiguienteCommand.Execute(null);

            // Assert
            viewModel.PaginaActual.Should().Be(2);
        }

        [Test]
        public async Task PrimeraPagina_CuandoSeEjecuta_DeberiaIrAPrimera() {
            // Arrange
            var docentes = GenerarDocentes(50);
            _personasServiceMock.Setup(s =>
                    s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(docentes);

            var viewModel = new DocentesViewModel(
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
        public async Task UltimaPagina_CuandoSeEjecuta_DeberiaIrAUltima() {
            // Arrange
            var docentes = GenerarDocentes(25);
            _personasServiceMock.Setup(s =>
                    s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(docentes);

            var viewModel = new DocentesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.UltimaPaginaCommand.Execute(null);

            // Assert
            viewModel.PaginaActual.Should().Be(3);
        }

        [Test]
        public async Task CambiarTamanoPagina_CuandoSeEjecuta_DeberiaCambiarTamanoYReiniciarPagina() {
            // Arrange
            var docentes = GenerarDocentes(25);
            _personasServiceMock.Setup(s =>
                    s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(docentes);

            var viewModel = new DocentesViewModel(
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
        public async Task Paginacion_Con25Registros_DeberiaCalcular3PaginasDe10() {
            // Arrange
            var docentes = GenerarDocentes(25);
            _personasServiceMock.Setup(s =>
                    s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(docentes);

            // Act
            var viewModel = new DocentesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.TotalRegistros.Should().Be(25);
            viewModel.TotalPaginas.Should().Be(3);
            viewModel.PaginaActual.Should().Be(1);
            viewModel.Docentes.Should().HaveCount(10);
        }

        [Test]
        public async Task Paginacion_Pagina2_DeberiaMostrarSiguientes10() {
            // Arrange
            var docentes = GenerarDocentes(25);
            _personasServiceMock.Setup(s =>
                    s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(docentes);

            var viewModel = new DocentesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.PaginaSiguienteCommand.Execute(null);

            // Assert
            viewModel.PaginaActual.Should().Be(2);
            viewModel.Docentes.Should().HaveCount(10);
        }

        [Test]
        public async Task Paginacion_Pagina3_DeberiaMostrarUltimos5() {
            // Arrange
            var docentes = GenerarDocentes(25);
            _personasServiceMock.Setup(s =>
                    s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(docentes);

            var viewModel = new DocentesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.PaginaActual = 3;

            // Assert
            viewModel.Docentes.Should().HaveCount(5);
        }

        [Test]
        public async Task Paginacion_ConTamano25_DeberiaMostrarTodosEnUnaPagina() {
            // Arrange
            var docentes = GenerarDocentes(25);
            _personasServiceMock.Setup(s =>
                    s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(docentes);

            var viewModel = new DocentesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.CambiarTamanoPaginaCommand.Execute(25);

            // Assert
            viewModel.TotalPaginas.Should().Be(1);
            viewModel.Docentes.Should().HaveCount(25);
        }

        [Test]
        public async Task Paginacion_ConTamano5_DeberiaCalcular5Paginas() {
            // Arrange
            var docentes = GenerarDocentes(25);
            _personasServiceMock.Setup(s =>
                    s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(docentes);

            var viewModel = new DocentesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Act
            viewModel.CambiarTamanoPaginaCommand.Execute(5);

            // Assert
            viewModel.TotalPaginas.Should().Be(5);
            viewModel.Docentes.Should().HaveCount(5);
        }

        [Test]
        public async Task StatusMessage_DeberiaMostrarPaginaActualYTotal() {
            // Arrange
            var docentes = GenerarDocentes(25);
            _personasServiceMock.Setup(s =>
                    s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(docentes);

            // Act
            var viewModel = new DocentesViewModel(
                _personasServiceMock.Object,
                _imageServiceMock.Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.StatusMessage.Should().Contain("Página 1/3");
            viewModel.StatusMessage.Should().Contain("25 docentes");
        }

        private List<Docente> GenerarDocentes(int cantidad) {
            var lista = new List<Docente>();
            var letras = "TRWAGMYFPDXBNJZSQVHLCKE";
            for (var i = 1; i <= cantidad; i++) {
                var dniNumerico = i * 11111111 % 100000000;
                var letra = letras[dniNumerico % 23];
                lista.Add(new Docente {
                    Id = i,
                    Dni = $"{dniNumerico:D8}{letra}",
                    Nombre = $"Docente{i}",
                    Apellidos = $"Apellido{i}",
                    Ciclo = Ciclo.DAM
                });
            }

            return lista;
        }
    }

    [TestFixture]
    public class CasosNegativos {
        [SetUp]
        public void SetUp() {
            _personasServiceMock = new Mock<IPersonasService>();
            _imageServiceMock = new Mock<IImageService>();
            _dialogServiceMock = new Mock<IDialogService>();
        }

        private Mock<IPersonasService> _personasServiceMock = null!;
        private Mock<IImageService> _imageServiceMock = null!;
        private Mock<IDialogService> _dialogServiceMock = null!;

        [Test]
        public async Task Constructor_CuandoGetDocentesFalla_DeberiaManejarError() {
            // Arrange
            _personasServiceMock.Setup(s =>
                    s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(),
                        It.IsAny<bool>()))
                .ThrowsAsync(new Exception("Error de base de datos"));

            // Act
            var viewModel = new DocentesViewModel(
                _personasServiceMock.Object,
                new Mock<IImageService>().Object,
                _dialogServiceMock.Object);

            // Assert
            viewModel.StatusMessage.Should().Contain("Error");
            _dialogServiceMock.Verify(d => d.ShowError(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task Constructor_CuandoServiciosSonNulos_NoDeberiaFallar() {
            // Arrange & Act
            var viewModel = new DocentesViewModel(
                new Mock<IPersonasService>().Object,
                new Mock<IImageService>().Object,
                new Mock<IDialogService>().Object);

            // Assert
            viewModel.Should().NotBeNull();
        }

        [Test]
        public async Task SearchText_ConCaracteresEspeciales_NoDeberiaFallar() {
            // Arrange
            _personasServiceMock.Setup(s =>
                    s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(new List<Docente>());

            var viewModel = new DocentesViewModel(
                _personasServiceMock.Object,
                new Mock<IImageService>().Object,
                new Mock<IDialogService>().Object);

            // Act & Assert
            viewModel.Invoking(v => v.SearchText = "test<>'\"").Should().NotThrow();
        }

        [Test]
        public async Task CicloSeleccionado_ConValorInvalido_DeberiaManejarlo() {
            // Arrange
            var _personasServiceMock = new Mock<IPersonasService>();
            _personasServiceMock.Setup(s =>
                    s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(new List<Docente>());

            var viewModel = new DocentesViewModel(
                _personasServiceMock.Object,
                new Mock<IImageService>().Object,
                new Mock<IDialogService>().Object);

            // Act
            viewModel.CicloSeleccionado = "CicloInvalido";

            // Assert
            viewModel.Docentes.Should().BeEmpty();
        }

        [Test]
        public async Task DeleteCommand_CuandoNoHaySeleccion_EditCommandNoSePuedeEjecutar() {
            // Arrange
            var _personasServiceMock = new Mock<IPersonasService>();
            _personasServiceMock.Setup(s =>
                    s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(new List<Docente>());

            var viewModel = new DocentesViewModel(
                _personasServiceMock.Object,
                new Mock<IImageService>().Object,
                new Mock<IDialogService>().Object);

            // Assert
            viewModel.EditCommand.CanExecute(null).Should().BeFalse();
            viewModel.DeleteCommand.CanExecute(null).Should().BeFalse();
            viewModel.ViewCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public async Task SearchText_Cambio_DeberiaFiltrarEnMemoria() {
            // Arrange
            var docentes = new List<Docente> { new() { Id = 1, Dni = "11111111A" } };
            _personasServiceMock.Setup(s =>
                    s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .ReturnsAsync(docentes);

            var viewModel = new DocentesViewModel(
                _personasServiceMock.Object,
                new Mock<IImageService>().Object,
                new Mock<IDialogService>().Object);

            // Act
            viewModel.SearchText = "11111111A";

            // Assert - La búsqueda se hace en memoria
            viewModel.Docentes.Should().HaveCount(1);
        }

        [Test]
        public async Task MostrarEliminados_Cambio_DeberiaRecargar() {
            // Arrange
            var docentesEliminados = new List<Docente> { new() { Id = 1, IsDeleted = true } };
            _personasServiceMock.Setup(s =>
                    s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), 1, int.MaxValue, false))
                .ReturnsAsync(docentesEliminados);

            var viewModel = new DocentesViewModel(
                _personasServiceMock.Object,
                new Mock<IImageService>().Object,
                new Mock<IDialogService>().Object);

            // Act
            viewModel.MostrarEliminados = false;

            // Assert
            _personasServiceMock.Verify(s =>
                s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), 1, int.MaxValue, false), Times.Once);
        }

        [Test]
        public async Task CicloSeleccionado_Cambio_DeberiaFiltrarEnMemoria() {
            // Arrange
            var docentesDAW = new List<Docente> { new() { Id = 1, Ciclo = Ciclo.DAW } };
            _personasServiceMock.Setup(s =>
                    s.GetDocentesOrderByAsync(It.IsAny<TipoOrdenamiento>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .ReturnsAsync(docentesDAW);

            var viewModel = new DocentesViewModel(
                _personasServiceMock.Object,
                new Mock<IImageService>().Object,
                new Mock<IDialogService>().Object);

            // Act
            viewModel.CicloSeleccionado = "DAW";

            // Assert - El filtrado por ciclo se hace en memoria
            viewModel.Docentes.Should().AllSatisfy(e => e.Ciclo.Should().Be(Ciclo.DAW));
        }
    }
}
