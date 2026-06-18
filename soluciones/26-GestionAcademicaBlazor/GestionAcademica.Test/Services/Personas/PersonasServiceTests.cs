using System.Collections;
using CSharpFunctionalExtensions;
using FluentAssertions;
using GestionAcademica.Cache;
using GestionAcademica.Enums;
using GestionAcademica.Errors.Common;
using GestionAcademica.Errors.Personas;
using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Personas;
using GestionAcademica.Repositories.Personas.Base;
using GestionAcademica.Services.Images;
using GestionAcademica.Services.Personas;
using GestionAcademica.Validators.Common;
using Moq;

namespace GestionAcademica.Test.Services.Personas;

[TestFixture]
public class PersonasServiceTests {
    [SetUp]
    public void SetUp() {
        _repositoryMock = new Mock<IPersonasRepository>();
        _valPersonaMock = new Mock<IValidador<Persona>>();
        _valEstudianteMock = new Mock<IValidador<Estudiante>>();
        _valDocenteMock = new Mock<IValidador<Docente>>();
        _cacheByIdMock = new Mock<ICache<int, Persona>>();
        _cacheByDniMock = new Mock<ICache<string, Persona>>();
        _imageServiceMock = new Mock<IImageService>();

        _valPersonaMock.Setup(v => v.Validar(It.IsAny<Persona>()))
            .Returns((Persona p) => Result.Success<Persona, DomainError>(p));
        _valEstudianteMock.Setup(v => v.Validar(It.IsAny<Estudiante>()))
            .Returns((Estudiante e) => Result.Success<Estudiante, DomainError>(e));
        _valDocenteMock.Setup(v => v.Validar(It.IsAny<Docente>()))
            .Returns((Docente d) => Result.Success<Docente, DomainError>(d));

        _service = new PersonasService(
            _repositoryMock.Object,
            _valPersonaMock.Object,
            _valEstudianteMock.Object,
            _valDocenteMock.Object,
            _cacheByIdMock.Object,
            _cacheByDniMock.Object,
            _imageServiceMock.Object
        );
    }

    private PersonasService _service = null!;
    private Mock<IPersonasRepository> _repositoryMock = null!;
    private Mock<IValidador<Persona>> _valPersonaMock = null!;
    private Mock<IValidador<Estudiante>> _valEstudianteMock = null!;
    private Mock<IValidador<Docente>> _valDocenteMock = null!;
    private Mock<ICache<int, Persona>> _cacheByIdMock = null!;
    private Mock<ICache<string, Persona>> _cacheByDniMock = null!;
    private Mock<IImageService> _imageServiceMock = null!;

    [TestFixture]
    public class CasosPositivos : PersonasServiceTests {
        [Test]
        public async Task GetAll_SinParametros_DeberiaRetornarTodasLasPersonas() {
            // Arrange
            var personas = new List<Persona> {
                new Estudiante { Id = 1, Dni = "11111111A", Nombre = "Juan" },
                new Docente { Id = 2, Dni = "22222222B", Nombre = "Ana" }
            };
            _repositoryMock.Setup(r => r.GetAllAsync(1, 10, true)).Returns(Task.FromResult<IEnumerable<Persona>>(personas));

            // Act
            var resultado = (await _service.GetAllAsync()).ToList();

            // Assert
            resultado.Should().HaveCount(2);
            _repositoryMock.Verify(r => r.GetAllAsync(1, 10, true), Times.Once);
        }

        [Test]
        public async Task GetAll_ConPaginacion_DeberiaRetornarPersonasPaginadas() {
            // Arrange
            var personas = new List<Persona> { new Estudiante { Id = 1, Dni = "A" } };
            _repositoryMock.Setup(r => r.GetAllAsync(2, 5, false)).Returns(Task.FromResult<IEnumerable<Persona>>(personas));

            // Act
            var resultado = (await _service.GetAllAsync(2, 5, false)).ToList();

            // Assert
            resultado.Should().HaveCount(1);
            _repositoryMock.Verify(r => r.GetAllAsync(2, 5, false), Times.Once);
        }

        [Test]
        public async Task GetById_ConCache_DeberiaRetornarDeCache() {
            // Arrange
            var persona = new Estudiante { Id = 1, Dni = "11111111A", Nombre = "Juan" };
            _cacheByIdMock.Setup(c => c.Get(1)).Returns(persona);

            // Act
            var resultado = await _service.GetByIdAsync(1);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Nombre.Should().Be("Juan");
            _cacheByIdMock.Verify(c => c.Get(1), Times.Once);
            _repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task GetById_SinCache_DeberiaBuscarEnRepositorioYAgregarACache() {
            // Arrange
            var persona = new Estudiante { Id = 1, Dni = "11111111A", Nombre = "Juan" };
            _cacheByIdMock.Setup(c => c.Get(1)).Returns((Persona?)null);
            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(persona);

            // Act
            var resultado = await _service.GetByIdAsync(1);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Nombre.Should().Be("Juan");
            _cacheByIdMock.Verify(c => c.Get(1), Times.Once);
            _cacheByIdMock.Verify(c => c.Add(1, persona), Times.Once);
            _repositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [Test]
        public async Task GetByDni_ConPersonaExistente_DeberiaRetornarPersona() {
            // Arrange
            var persona = new Estudiante { Id = 1, Dni = "11111111A" };
            _repositoryMock.Setup(r => r.GetByDniAsync("11111111A")).ReturnsAsync(persona);

            // Act
            var resultado = await _service.GetByDniAsync("11111111A");

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Dni.Should().Be("11111111A");
            _repositoryMock.Verify(r => r.GetByDniAsync("11111111A"), Times.Once);
        }

        [Test]
        public async Task Save_ConEstudianteValido_DeberiaGuardarCorrectamente() {
            // Arrange
            var estudiante = new Estudiante
                { Dni = "11111111A", Nombre = "Juan", Apellidos = "Pérez", Calificacion = 8.5 };

            _valPersonaMock.Setup(v => v.Validar(It.IsAny<Persona>()))
                .Returns((Persona p) => Result.Success<Persona, DomainError>(p));
            _valEstudianteMock.Setup(v => v.Validar(It.IsAny<Estudiante>()))
                .Returns((Estudiante e) => Result.Success<Estudiante, DomainError>(e));
            _repositoryMock.Setup(r => r.ExisteDniAsync(It.IsAny<string>())).ReturnsAsync(false);
            _repositoryMock.Setup(r => r.ExisteEmailAsync(It.IsAny<string>())).ReturnsAsync(false);
            _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Persona>()))
                .ReturnsAsync((Persona p) => Result.Success<Persona, DomainError>(p));

            // Act
            var resultado = await _service.SaveAsync(estudiante);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Persona>()), Times.Once);
            _repositoryMock.Verify(r => r.ExisteDniAsync(It.IsAny<string>()), Times.Once);
            _repositoryMock.Verify(r => r.ExisteEmailAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task Save_ConDocenteValido_DeberiaGuardarCorrectamente() {
            // Arrange
            var docente = new Docente { Dni = "22222222B", Nombre = "Ana", Apellidos = "García", Experiencia = 5 };

            _valPersonaMock.Setup(v => v.Validar(It.IsAny<Persona>()))
                .Returns((Persona p) => Result.Success<Persona, DomainError>(p));
            _valDocenteMock.Setup(v => v.Validar(It.IsAny<Docente>()))
                .Returns((Docente d) => Result.Success<Docente, DomainError>(d));
            _repositoryMock.Setup(r => r.ExisteDniAsync(It.IsAny<string>())).ReturnsAsync(false);
            _repositoryMock.Setup(r => r.ExisteEmailAsync(It.IsAny<string>())).ReturnsAsync(false);
            _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Persona>()))
                .ReturnsAsync((Persona p) => Result.Success<Persona, DomainError>(p));

            // Act
            var resultado = await _service.SaveAsync(docente);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Persona>()), Times.Once);
        }

        [Test]
        public async Task Update_ConPersonaExistente_DeberiaActualizarYLimpiarCache() {
            // Arrange
            var existente = new Estudiante { Id = 1, Dni = "11111111A", Nombre = "Juan" };
            var actualizada = new Estudiante { Id = 1, Dni = "11111111A", Nombre = "Juan Actualizado" };

            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existente);
            _repositoryMock.Setup(r => r.GetByDniAsync(It.IsAny<string>())).ReturnsAsync((Persona?)null);
            _repositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((Persona?)null);
            _valPersonaMock.Setup(v => v.Validar(It.IsAny<Persona>()))
                .Returns((Persona p) => Result.Success<Persona, DomainError>(p));
            _valEstudianteMock.Setup(v => v.Validar(It.IsAny<Estudiante>()))
                .Returns((Estudiante e) => Result.Success<Estudiante, DomainError>(e));
            _repositoryMock.Setup(r => r.UpdateAsync(1, It.IsAny<Persona>()))
                .ReturnsAsync((int id, Persona p) => Result.Success<Persona, DomainError>(p));

            // Act
            var resultado = await _service.UpdateAsync(1, actualizada);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            _cacheByIdMock.Verify(c => c.Remove(1), Times.Once);
            _cacheByDniMock.Verify(c => c.Remove("11111111A"), Times.Once);
            _repositoryMock.Verify(r => r.UpdateAsync(1, It.IsAny<Persona>()), Times.Once);
        }

        [Test]
        public async Task Delete_ConPersonaExistente_DeberiaEliminarYLimpiarCache() {
            // Arrange
            var persona = new Estudiante { Id = 1, Dni = "11111111A" };
            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(persona);
            _repositoryMock.Setup(r => r.DeleteAsync(1, true)).ReturnsAsync(persona);

            // Act
            var resultado = await _service.DeleteAsync(1);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            _cacheByIdMock.Verify(c => c.Remove(1), Times.Once);
            _cacheByDniMock.Verify(c => c.Remove("11111111A"), Times.Once);
            _repositoryMock.Verify(r => r.DeleteAsync(1, true), Times.Once);
            _repositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [Test]
        public async Task DeleteAll_DeberiaLlamarRepository() {
            // Arrange
            _repositoryMock.Setup(r => r.DeleteAllAsync()).ReturnsAsync(true);

            // Act
            var resultado = await _service.DeleteAllAsync();

            // Assert
            resultado.Should().BeTrue();
            _repositoryMock.Verify(r => r.DeleteAllAsync(), Times.Once);
        }

        [Test]
        public async Task GetEstudiantesOrderBy_DeberiaRetornarSoloEstudiantes() {
            // Arrange
            var estudiantes = new List<Estudiante> {
                new() { Id = 1, Dni = "A", Calificacion = 7.5 },
                new() { Id = 2, Dni = "B", Calificacion = 8.5 }
            };
            _repositoryMock.Setup(r => r.GetEstudiantesOrderByAsync("dni", 1, 10, true))
                .ReturnsAsync(estudiantes);

            // Act
            var resultado = (await _service.GetEstudiantesOrderByAsync()).ToList();

            // Assert
            resultado.Should().HaveCount(2);
            resultado.Should().AllBeOfType<Estudiante>();
        }

        [Test]
        public async Task GetEstudiantesOrderBy_ConOrdenPorNota_DeberiaLlamarRepository() {
            // Arrange
            var estudiantes = new List<Estudiante> { new() { Id = 1, Calificacion = 9.0 } };
            _repositoryMock.Setup(r => r.GetEstudiantesOrderByAsync("nota", 1, 10, true))
                .ReturnsAsync(estudiantes);

            // Act
            var resultado = (await _service.GetEstudiantesOrderByAsync(TipoOrdenamiento.Nota)).ToList();

            // Assert
            resultado.Should().HaveCount(1);
        }

        [Test]
        public async Task GetEstudiantesOrderBy_ConOrdenPorCiclo_DeberiaLlamarRepository() {
            // Arrange
            var estudiantes = new List<Estudiante> { new() { Id = 1, Ciclo = Ciclo.DAM } };
            _repositoryMock.Setup(r => r.GetEstudiantesOrderByAsync("ciclo", 1, 5, false))
                .ReturnsAsync(estudiantes);

            // Act
            var resultado = (await _service.GetEstudiantesOrderByAsync(TipoOrdenamiento.Ciclo, 1, 5, false)).ToList();

            // Assert
            resultado.Should().HaveCount(1);
        }

        [Test]
        public async Task GetEstudiantesOrderBy_ConOrdenPorCurso_DeberiaLlamarRepository() {
            // Arrange
            var estudiantes = new List<Estudiante> { new() { Id = 1, Curso = Curso.Primero } };
            _repositoryMock.Setup(r => r.GetEstudiantesOrderByAsync("curso", 1, 10, true))
                .ReturnsAsync(estudiantes);

            // Act
            var resultado = (await _service.GetEstudiantesOrderByAsync(TipoOrdenamiento.Curso)).ToList();

            // Assert
            resultado.Should().HaveCount(1);
        }

        [Test]
        public async Task GetDocentesOrderBy_DeberiaRetornarSoloDocentes() {
            // Arrange
            var docentes = new List<Docente> {
                new() { Id = 2, Dni = "B", Experiencia = 3 },
                new() { Id = 3, Dni = "C", Experiencia = 7 }
            };
            _repositoryMock.Setup(r => r.GetDocentesOrderByAsync("dni", 1, 10, true))
                .ReturnsAsync(docentes);

            // Act
            var resultado = (await _service.GetDocentesOrderByAsync()).ToList();

            // Assert
            resultado.Should().HaveCount(2);
            resultado.Should().AllBeOfType<Docente>();
        }

        [Test]
        public async Task GetDocentesOrderBy_ConOrdenPorExperiencia_DeberiaLlamarRepository() {
            // Arrange
            var docentes = new List<Docente> { new() { Id = 1, Experiencia = 5 } };
            _repositoryMock.Setup(r => r.GetDocentesOrderByAsync("experiencia", 1, 10, true))
                .ReturnsAsync(docentes);

            // Act
            var resultado = (await _service.GetDocentesOrderByAsync(TipoOrdenamiento.Experiencia)).ToList();

            // Assert
            resultado.Should().HaveCount(1);
        }

        [Test]
        public async Task GetDocentesOrderBy_ConOrdenPorCiclo_DeberiaLlamarRepository() {
            // Arrange
            var docentes = new List<Docente> { new() { Id = 1, Ciclo = Ciclo.DAW } };
            _repositoryMock.Setup(r => r.GetDocentesOrderByAsync("ciclo", 1, 5, false))
                .ReturnsAsync(docentes);

            // Act
            var resultado = (await _service.GetDocentesOrderByAsync(TipoOrdenamiento.Ciclo, 1, 5, false)).ToList();

            // Assert
            resultado.Should().HaveCount(1);
        }

        [Test]
        public async Task GetDocentesOrderBy_ConOrdenPorModulo_DeberiaLlamarRepository() {
            // Arrange
            var docentes = new List<Docente> { new() { Id = 1, Especialidad = "Programación" } };
            _repositoryMock.Setup(r => r.GetDocentesOrderByAsync("modulo", 1, 10, true))
                .ReturnsAsync(docentes);

            // Act
            var resultado = (await _service.GetDocentesOrderByAsync(TipoOrdenamiento.Modulo)).ToList();

            // Assert
            resultado.Should().HaveCount(1);
        }
    }

    [TestFixture]
    public class CasosNegativos : PersonasServiceTests {
        [Test]
        public async Task GetById_ConPersonaNoExistente_DeberiaRetornarErrorNotFound() {
            // Arrange
            _cacheByIdMock.Setup(c => c.Get(1)).Returns((Persona?)null);
            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Persona?)null);

            // Act
            var resultado = await _service.GetByIdAsync(1);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.NotFound>();
            resultado.Error.Message.Should().Contain("1");
            _cacheByIdMock.Verify(c => c.Get(1), Times.Once);
            _repositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [Test]
        public async Task GetByDni_ConPersonaNoExistente_DeberiaRetornarErrorNotFound() {
            // Arrange
            _repositoryMock.Setup(r => r.GetByDniAsync("99999999Z")).ReturnsAsync((Persona?)null);

            // Act
            var resultado = await _service.GetByDniAsync("99999999Z");

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.NotFound>();
            resultado.Error.Message.Should().Contain("99999999Z");
            _repositoryMock.Verify(r => r.GetByDniAsync("99999999Z"), Times.Once);
        }

        [Test]
        public async Task Save_ConDniDuplicado_DeberiaRetornarErrorDniAlreadyExists() {
            // Arrange
            var estudiante = new Estudiante { Dni = "11111111A", Nombre = "Juan" };

            _valPersonaMock.Setup(v => v.Validar(It.IsAny<Persona>()))
                .Returns((Persona p) => Result.Success<Persona, DomainError>(p));
            _repositoryMock.Setup(r => r.ExisteDniAsync("11111111A")).ReturnsAsync(true);

            // Act
            var resultado = await _service.SaveAsync(estudiante);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.DniAlreadyExists>();
            resultado.Error.Message.Should().Contain("11111111A");
            _repositoryMock.Verify(r => r.ExisteDniAsync("11111111A"), Times.Once);
            _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Persona>()), Times.Never);
        }

        [Test]
        public async Task Save_ConEmailDuplicado_DeberiaRetornarErrorEmailAlreadyExists() {
            // Arrange
            var estudiante = new Estudiante { Dni = "11111111A", Email = "juan@test.com" };

            _valPersonaMock.Setup(v => v.Validar(It.IsAny<Persona>()))
                .Returns((Persona p) => Result.Success<Persona, DomainError>(p));
            _repositoryMock.Setup(r => r.ExisteDniAsync(It.IsAny<string>())).ReturnsAsync(false);
            _repositoryMock.Setup(r => r.ExisteEmailAsync("juan@test.com")).ReturnsAsync(true);

            // Act
            var resultado = await _service.SaveAsync(estudiante);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.EmailAlreadyExists>();
            resultado.Error.Message.Should().Contain("juan@test.com");
            _repositoryMock.Verify(r => r.ExisteEmailAsync("juan@test.com"), Times.Once);
            _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Persona>()), Times.Never);
        }

        [Test]
        public async Task Save_ConValidacionFallida_DeberiaRetornarErrorValidation() {
            // Arrange
            var estudiante = new Estudiante { Dni = "11111111A", Nombre = "Juan" };
            var error = new PersonaError.Validation(new[] { "El nombre no puede estar vacío" });

            _valPersonaMock.Setup(v => v.Validar(It.IsAny<Persona>()))
                .Returns(Result.Failure<Persona, DomainError>(error));

            // Act
            var resultado = await _service.SaveAsync(estudiante);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.Validation>();
            resultado.Error.Message.Should().Contain("validación");
            _valPersonaMock.Verify(v => v.Validar(estudiante), Times.Once);
            _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Persona>()), Times.Never);
        }

        [Test]
        public async Task Update_ConPersonaNoExistente_DeberiaRetornarErrorNotFound() {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Persona?)null);

            // Act
            var resultado = await _service.UpdateAsync(999, new Estudiante { Dni = "A" });

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.NotFound>();
            resultado.Error.Message.Should().Contain("999");
            _repositoryMock.Verify(r => r.GetByIdAsync(999), Times.Once);
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<int>(), It.IsAny<Persona>()), Times.Never);
        }

        [Test]
        public async Task Delete_ConPersonaNoExistente_DeberiaRetornarErrorNotFound() {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Persona?)null);

            // Act
            var resultado = await _service.DeleteAsync(999);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.NotFound>();
            resultado.Error.Message.Should().Contain("999");
            _repositoryMock.Verify(r => r.GetByIdAsync(999), Times.Once);
            _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
        }

        [Test]
        public async Task Update_ConDniDuplicadoEnOtraPersona_DeberiaRetornarError() {
            // Arrange
            var existente = new Estudiante { Id = 1, Dni = "11111111A", Nombre = "Juan" };
            var actualizada = new Estudiante { Id = 1, Dni = "22222222B" };
            var otraPersona = new Estudiante { Id = 2, Dni = "22222222B" };

            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existente);
            _valPersonaMock.Setup(v => v.Validar(It.IsAny<Persona>()))
                .Returns(Result.Success<Persona, DomainError>(actualizada));
            _repositoryMock.Setup(r => r.GetByDniAsync("22222222B")).ReturnsAsync(otraPersona);

            // Act
            var resultado = await _service.UpdateAsync(1, actualizada);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.DniAlreadyExists>();
            resultado.Error.Message.Should().Contain("22222222B");
        }

        [Test]
        public async Task Update_ConEmailDuplicadoEnOtraPersona_DeberiaRetornarError() {
            // Arrange
            var existente = new Estudiante { Id = 1, Dni = "11111111A", Email = "juan@test.com" };
            var actualizada = new Estudiante { Id = 1, Dni = "11111111A", Email = "otro@test.com" };
            var otraPersona = new Estudiante { Id = 2, Email = "otro@test.com" };

            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existente);
            _valPersonaMock.Setup(v => v.Validar(It.IsAny<Persona>()))
                .Returns(Result.Success<Persona, DomainError>(actualizada));
            _valEstudianteMock.Setup(v => v.Validar(It.IsAny<Estudiante>()))
                .Returns((Estudiante e) => Result.Success<Estudiante, DomainError>(e));
            _repositoryMock.Setup(r => r.GetByDniAsync(It.IsAny<string>())).ReturnsAsync((Persona?)null);
            _repositoryMock.Setup(r => r.GetByEmailAsync("otro@test.com")).ReturnsAsync(otraPersona);

            // Act
            var resultado = await _service.UpdateAsync(1, actualizada);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.EmailAlreadyExists>();
            resultado.Error.Message.Should().Contain("otro@test.com");
        }

        [Test]
        public async Task Restore_ConPersonaExistenteEliminada_DeberiaRestaurar() {
            // Arrange
            _repositoryMock.Setup(r => r.RestoreAsync(1))
                .ReturnsAsync(Result.Success<Persona, DomainError>(new Estudiante
                    { Id = 1, Dni = "11111111A", IsDeleted = false }));

            // Act
            var resultado = await _service.RestoreAsync(1);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            _repositoryMock.Verify(r => r.RestoreAsync(1), Times.Once);
        }

        [Test]
        public async Task Restore_ConPersonaNoExistente_DeberiaRetornarError() {
            // Arrange
            _repositoryMock.Setup(r => r.RestoreAsync(999))
                .ReturnsAsync(Result.Failure<Persona, DomainError>(PersonaErrors.NotFound("999")));

            // Act
            var resultado = await _service.RestoreAsync(999);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.NotFound>();
        }

        [Test]
        public async Task CountEstudiantes_DeberiaRetornarConteoCorrecto() {
            // Arrange
            _repositoryMock.Setup(r => r.CountEstudiantesAsync(false)).ReturnsAsync(5);

            // Act
            var resultado = await _service.CountEstudiantesAsync();

            // Assert
            resultado.Should().Be(5);
            _repositoryMock.Verify(r => r.CountEstudiantesAsync(false), Times.Once);
        }

        [Test]
        public async Task CountDocentes_DeberiaRetornarConteoCorrecto() {
            // Arrange
            _repositoryMock.Setup(r => r.CountDocentesAsync(false)).ReturnsAsync(3);

            // Act
            var resultado = await _service.CountDocentesAsync();

            // Assert
            resultado.Should().Be(3);
            _repositoryMock.Verify(r => r.CountDocentesAsync(false), Times.Once);
        }

        [Test]
        public async Task CountAprobados_DeberiaRetornarConteoCorrecto() {
            // Arrange
            var estudiantes = new List<Estudiante> {
                new() { Id = 1, Calificacion = 8.5 },
                new() { Id = 2, Calificacion = 3.0 },
                new() { Id = 3, Calificacion = 6.0 }
            };
            _repositoryMock.Setup(r => r.GetEstudiantesAsync(1, int.MaxValue, false)).ReturnsAsync(estudiantes);

            // Act
            var resultado = await _service.CountAprobadosAsync(5);

            // Assert
            resultado.Should().Be(2);
        }

        [Test]
        public async Task CountSuspensos_DeberiaRetornarConteoCorrecto() {
            // Arrange
            var estudiantes = new List<Estudiante> {
                new() { Id = 1, Calificacion = 8.5 },
                new() { Id = 2, Calificacion = 3.0 },
                new() { Id = 3, Calificacion = 4.5 }
            };
            _repositoryMock.Setup(r => r.GetEstudiantesAsync(1, int.MaxValue, false)).ReturnsAsync(estudiantes);

            // Act
            var resultado = await _service.CountSuspensosAsync(5);

            // Assert
            resultado.Should().Be(2);
        }

        [Test]
        public async Task GetEstudiantesPorCiclo_DeberiaRetornarDiccionarioCorrecto() {
            // Arrange
            var estudiantes = new List<Estudiante> {
                new() { Id = 1, Ciclo = Ciclo.DAM },
                new() { Id = 2, Ciclo = Ciclo.DAM },
                new() { Id = 3, Ciclo = Ciclo.DAW }
            };
            _repositoryMock.Setup(r => r.GetEstudiantesAsync(1, int.MaxValue, false)).ReturnsAsync(estudiantes);

            // Act
            var resultado = await _service.GetEstudiantesPorCicloAsync();

            // Assert
            resultado.Should().ContainKey(Ciclo.DAM);
            resultado[Ciclo.DAM].Should().Be(2);
            resultado.Should().ContainKey(Ciclo.DAW);
            resultado[Ciclo.DAW].Should().Be(1);
        }

        [Test]
        public async Task GetDocentesPorCiclo_DeberiaRetornarDiccionarioCorrecto() {
            // Arrange
            var docentes = new List<Docente> {
                new() { Id = 1, Ciclo = Ciclo.DAM },
                new() { Id = 2, Ciclo = Ciclo.DAW },
                new() { Id = 3, Ciclo = Ciclo.DAW }
            };
            _repositoryMock.Setup(r => r.GetDocentesAsync(1, int.MaxValue, false)).ReturnsAsync(docentes);

            // Act
            var resultado = await _service.GetDocentesPorCicloAsync();

            // Assert
            resultado.Should().ContainKey(Ciclo.DAM);
            resultado[Ciclo.DAM].Should().Be(1);
            resultado.Should().ContainKey(Ciclo.DAW);
            resultado[Ciclo.DAW].Should().Be(2);
        }
    }
}
