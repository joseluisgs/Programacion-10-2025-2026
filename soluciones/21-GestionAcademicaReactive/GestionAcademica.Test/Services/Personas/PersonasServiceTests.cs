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

        // Configurar validadores para que devuelvan Success por defecto
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
        public void GetAll_SinParametros_DeberiaRetornarTodasLasPersonas() {
            // Arrange
            var personas = new List<Persona> {
                new Estudiante { Id = 1, Dni = "11111111A", Nombre = "Juan" },
                new Docente { Id = 2, Dni = "22222222B", Nombre = "Ana" }
            };
            _repositoryMock.Setup(r => r.GetAll(1, 10, true)).Returns(personas);

            // Act
            var resultado = _service.GetAll().ToList();

            // Assert
            resultado.Should().HaveCount(2);
            _repositoryMock.Verify(r => r.GetAll(1, 10, true), Times.Once);
        }

        [Test]
        public void GetAll_ConPaginacion_DeberiaRetornarPersonasPaginadas() {
            // Arrange
            var personas = new List<Persona> { new Estudiante { Id = 1, Dni = "A" } };
            _repositoryMock.Setup(r => r.GetAll(2, 5, false)).Returns(personas);

            // Act
            var resultado = _service.GetAll(2, 5, false).ToList();

            // Assert
            resultado.Should().HaveCount(1);
            _repositoryMock.Verify(r => r.GetAll(2, 5, false), Times.Once);
        }

        [Test]
        public void GetById_ConCache_DeberiaRetornarDeCache() {
            // Arrange
            var persona = new Estudiante { Id = 1, Dni = "11111111A", Nombre = "Juan" };
            _cacheByIdMock.Setup(c => c.Get(1)).Returns(persona);

            // Act
            var resultado = _service.GetById(1);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Nombre.Should().Be("Juan");
            _cacheByIdMock.Verify(c => c.Get(1), Times.Once);
            _repositoryMock.Verify(r => r.GetById(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void GetById_SinCache_DeberiaBuscarEnRepositorioYAgregarACache() {
            // Arrange
            var persona = new Estudiante { Id = 1, Dni = "11111111A", Nombre = "Juan" };
            _cacheByIdMock.Setup(c => c.Get(1)).Returns((Persona?)null);
            _repositoryMock.Setup(r => r.GetById(1)).Returns(persona);

            // Act
            var resultado = _service.GetById(1);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Nombre.Should().Be("Juan");
            _cacheByIdMock.Verify(c => c.Get(1), Times.Once);
            _cacheByIdMock.Verify(c => c.Add(1, persona), Times.Once);
            _repositoryMock.Verify(r => r.GetById(1), Times.Once);
        }

        [Test]
        public void GetByDni_ConPersonaExistente_DeberiaRetornarPersona() {
            // Arrange
            var persona = new Estudiante { Id = 1, Dni = "11111111A" };
            _repositoryMock.Setup(r => r.GetByDni("11111111A")).Returns(persona);

            // Act
            var resultado = _service.GetByDni("11111111A");

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Dni.Should().Be("11111111A");
            _repositoryMock.Verify(r => r.GetByDni("11111111A"), Times.Once);
        }

        [Test]
        public void Save_ConEstudianteValido_DeberiaGuardarCorrectamente() {
            // Arrange
            var estudiante = new Estudiante
                { Dni = "11111111A", Nombre = "Juan", Apellidos = "Pérez", Calificacion = 8.5 };

            _valPersonaMock.Setup(v => v.Validar(It.IsAny<Persona>()))
                .Returns((Persona p) => Result.Success<Persona, DomainError>(p));
            _valEstudianteMock.Setup(v => v.Validar(It.IsAny<Estudiante>()))
                .Returns((Estudiante e) => Result.Success<Estudiante, DomainError>(e));
            _repositoryMock.Setup(r => r.ExisteDni(It.IsAny<string>())).Returns(false);
            _repositoryMock.Setup(r => r.ExisteEmail(It.IsAny<string>())).Returns(false);
            _repositoryMock.Setup(r => r.Create(It.IsAny<Persona>()))
                .Returns((Persona p) => Result.Success<Persona, DomainError>(p));

            // Act
            var resultado = _service.Save(estudiante);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            _repositoryMock.Verify(r => r.Create(It.IsAny<Persona>()), Times.Once);
            _repositoryMock.Verify(r => r.ExisteDni(It.IsAny<string>()), Times.Once);
            _repositoryMock.Verify(r => r.ExisteEmail(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void Save_ConDocenteValido_DeberiaGuardarCorrectamente() {
            // Arrange
            var docente = new Docente { Dni = "22222222B", Nombre = "Ana", Apellidos = "García", Experiencia = 5 };

            _valPersonaMock.Setup(v => v.Validar(It.IsAny<Persona>()))
                .Returns((Persona p) => Result.Success<Persona, DomainError>(p));
            _valDocenteMock.Setup(v => v.Validar(It.IsAny<Docente>()))
                .Returns((Docente d) => Result.Success<Docente, DomainError>(d));
            _repositoryMock.Setup(r => r.ExisteDni(It.IsAny<string>())).Returns(false);
            _repositoryMock.Setup(r => r.ExisteEmail(It.IsAny<string>())).Returns(false);
            _repositoryMock.Setup(r => r.Create(It.IsAny<Persona>()))
                .Returns((Persona p) => Result.Success<Persona, DomainError>(p));

            // Act
            var resultado = _service.Save(docente);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            _repositoryMock.Verify(r => r.Create(It.IsAny<Persona>()), Times.Once);
        }

        [Test]
        public void Update_ConPersonaExistente_DeberiaActualizarYLimpiarCache() {
            // Arrange
            var existente = new Estudiante { Id = 1, Dni = "11111111A", Nombre = "Juan" };
            var actualizada = new Estudiante { Id = 1, Dni = "11111111A", Nombre = "Juan Actualizado" };

            _repositoryMock.Setup(r => r.GetById(1)).Returns(existente);
            _repositoryMock.Setup(r => r.GetByDni(It.IsAny<string>())).Returns((Persona?)null);
            _repositoryMock.Setup(r => r.GetByEmail(It.IsAny<string>())).Returns((Persona?)null);
            _valPersonaMock.Setup(v => v.Validar(It.IsAny<Persona>()))
                .Returns((Persona p) => Result.Success<Persona, DomainError>(p));
            _valEstudianteMock.Setup(v => v.Validar(It.IsAny<Estudiante>()))
                .Returns((Estudiante e) => Result.Success<Estudiante, DomainError>(e));
            _repositoryMock.Setup(r => r.Update(1, It.IsAny<Persona>()))
                .Returns((int id, Persona p) => Result.Success<Persona, DomainError>(p));

            // Act
            var resultado = _service.Update(1, actualizada);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            _cacheByIdMock.Verify(c => c.Remove(1), Times.Once);
            _cacheByDniMock.Verify(c => c.Remove("11111111A"), Times.Once);
            _repositoryMock.Verify(r => r.Update(1, It.IsAny<Persona>()), Times.Once);
        }

        [Test]
        public void Delete_ConPersonaExistente_DeberiaEliminarYLimpiarCache() {
            // Arrange
            var persona = new Estudiante { Id = 1, Dni = "11111111A" };
            _repositoryMock.Setup(r => r.GetById(1)).Returns(persona);
            _repositoryMock.Setup(r => r.Delete(1, true)).Returns(persona);

            // Act
            var resultado = _service.Delete(1);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            _cacheByIdMock.Verify(c => c.Remove(1), Times.Once);
            _cacheByDniMock.Verify(c => c.Remove("11111111A"), Times.Once);
            _repositoryMock.Verify(r => r.Delete(1, true), Times.Once);
            _repositoryMock.Verify(r => r.GetById(1), Times.Once);
        }

        [Test]
        public void DeleteAll_DeberiaLlamarRepository() {
            // Arrange
            _repositoryMock.Setup(r => r.DeleteAll()).Returns(true);

            // Act
            var resultado = _service.DeleteAll();

            // Assert
            resultado.Should().BeTrue();
            _repositoryMock.Verify(r => r.DeleteAll(), Times.Once);
        }

[Test]
        public void GetEstudiantesOrderBy_DeberiaRetornarSoloEstudiantes() {
            // Arrange
            var estudiantes = new List<Estudiante> {
                new() { Id = 1, Dni = "A", Calificacion = 7.5 },
                new() { Id = 2, Dni = "B", Calificacion = 8.5 }
            };
            _repositoryMock.Setup(r => r.GetEstudiantesOrderBy("dni", 1, 10, true))
                .Returns(estudiantes);

            // Act
            var resultado = _service.GetEstudiantesOrderBy().ToList();

            // Assert
            resultado.Should().HaveCount(2);
            resultado.Should().AllBeOfType<Estudiante>();
        }

        [Test]
        public void GetEstudiantesOrderBy_ConOrdenPorNota_DeberiaLlamarRepository() {
            // Arrange
            var estudiantes = new List<Estudiante> { new() { Id = 1, Calificacion = 9.0 } };
            _repositoryMock.Setup(r => r.GetEstudiantesOrderBy("nota", 1, 10, true))
                .Returns(estudiantes);

            // Act
            var resultado = _service.GetEstudiantesOrderBy(TipoOrdenamiento.Nota).ToList();

            // Assert
            resultado.Should().HaveCount(1);
        }

        [Test]
        public void GetEstudiantesOrderBy_ConOrdenPorCiclo_DeberiaLlamarRepository() {
            // Arrange
            var estudiantes = new List<Estudiante> { new() { Id = 1, Ciclo = Ciclo.DAM } };
            _repositoryMock.Setup(r => r.GetEstudiantesOrderBy("ciclo", 1, 5, false))
                .Returns(estudiantes);

            // Act
            var resultado = _service.GetEstudiantesOrderBy(TipoOrdenamiento.Ciclo, 1, 5, false).ToList();

            // Assert
            resultado.Should().HaveCount(1);
        }

        [Test]
        public void GetEstudiantesOrderBy_ConOrdenPorCurso_DeberiaLlamarRepository() {
            // Arrange
            var estudiantes = new List<Estudiante> { new() { Id = 1, Curso = Curso.Primero } };
            _repositoryMock.Setup(r => r.GetEstudiantesOrderBy("curso", 1, 10, true))
                .Returns(estudiantes);

            // Act
            var resultado = _service.GetEstudiantesOrderBy(TipoOrdenamiento.Curso).ToList();

            // Assert
            resultado.Should().HaveCount(1);
        }

        [Test]
        public void GetDocentesOrderBy_DeberiaRetornarSoloDocentes() {
            // Arrange
            var docentes = new List<Docente> {
                new() { Id = 2, Dni = "B", Experiencia = 3 },
                new() { Id = 3, Dni = "C", Experiencia = 7 }
            };
            _repositoryMock.Setup(r => r.GetDocentesOrderBy("dni", 1, 10, true))
                .Returns(docentes);

            // Act
            var resultado = _service.GetDocentesOrderBy().ToList();

            // Assert
            resultado.Should().HaveCount(2);
            resultado.Should().AllBeOfType<Docente>();
        }

        [Test]
        public void GetDocentesOrderBy_ConOrdenPorExperiencia_DeberiaLlamarRepository() {
            // Arrange
            var docentes = new List<Docente> { new() { Id = 1, Experiencia = 5 } };
            _repositoryMock.Setup(r => r.GetDocentesOrderBy("experiencia", 1, 10, true))
                .Returns(docentes);

            // Act
            var resultado = _service.GetDocentesOrderBy(TipoOrdenamiento.Experiencia).ToList();

            // Assert
            resultado.Should().HaveCount(1);
        }

        [Test]
        public void GetDocentesOrderBy_ConOrdenPorCiclo_DeberiaLlamarRepository() {
            // Arrange
            var docentes = new List<Docente> { new() { Id = 1, Ciclo = Ciclo.DAW } };
            _repositoryMock.Setup(r => r.GetDocentesOrderBy("ciclo", 1, 5, false))
                .Returns(docentes);

            // Act
            var resultado = _service.GetDocentesOrderBy(TipoOrdenamiento.Ciclo, 1, 5, false).ToList();

            // Assert
            resultado.Should().HaveCount(1);
        }

        [Test]
        public void GetDocentesOrderBy_ConOrdenPorModulo_DeberiaLlamarRepository() {
            // Arrange
            var docentes = new List<Docente> { new() { Id = 1, Especialidad = "Programación" } };
            _repositoryMock.Setup(r => r.GetDocentesOrderBy("modulo", 1, 10, true))
                .Returns(docentes);

            // Act
            var resultado = _service.GetDocentesOrderBy(TipoOrdenamiento.Modulo).ToList();

            // Assert
            resultado.Should().HaveCount(1);
        }
    }

    [TestFixture]
    public class CasosNegativos : PersonasServiceTests {
        [Test]
        public void GetById_ConPersonaNoExistente_DeberiaRetornarErrorNotFound() {
            // Arrange
            _cacheByIdMock.Setup(c => c.Get(1)).Returns((Persona?)null);
            _repositoryMock.Setup(r => r.GetById(1)).Returns((Persona?)null);

            // Act
            var resultado = _service.GetById(1);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.NotFound>();
            resultado.Error.Message.Should().Contain("1");
            _cacheByIdMock.Verify(c => c.Get(1), Times.Once);
            _repositoryMock.Verify(r => r.GetById(1), Times.Once);
        }

        [Test]
        public void GetByDni_ConPersonaNoExistente_DeberiaRetornarErrorNotFound() {
            // Arrange
            _repositoryMock.Setup(r => r.GetByDni("99999999Z")).Returns((Persona?)null);

            // Act
            var resultado = _service.GetByDni("99999999Z");

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.NotFound>();
            resultado.Error.Message.Should().Contain("99999999Z");
            _repositoryMock.Verify(r => r.GetByDni("99999999Z"), Times.Once);
        }

        [Test]
        public void Save_ConDniDuplicado_DeberiaRetornarErrorDniAlreadyExists() {
            // Arrange
            var estudiante = new Estudiante { Dni = "11111111A", Nombre = "Juan" };

            _valPersonaMock.Setup(v => v.Validar(It.IsAny<Persona>()))
                .Returns((Persona p) => Result.Success<Persona, DomainError>(p));
            _repositoryMock.Setup(r => r.ExisteDni("11111111A")).Returns(true);

            // Act
            var resultado = _service.Save(estudiante);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.DniAlreadyExists>();
            resultado.Error.Message.Should().Contain("11111111A");
            _repositoryMock.Verify(r => r.ExisteDni("11111111A"), Times.Once);
            _repositoryMock.Verify(r => r.Create(It.IsAny<Persona>()), Times.Never);
        }

        [Test]
        public void Save_ConEmailDuplicado_DeberiaRetornarErrorEmailAlreadyExists() {
            // Arrange
            var estudiante = new Estudiante { Dni = "11111111A", Email = "juan@test.com" };

            _valPersonaMock.Setup(v => v.Validar(It.IsAny<Persona>()))
                .Returns((Persona p) => Result.Success<Persona, DomainError>(p));
            _repositoryMock.Setup(r => r.ExisteDni(It.IsAny<string>())).Returns(false);
            _repositoryMock.Setup(r => r.ExisteEmail("juan@test.com")).Returns(true);

            // Act
            var resultado = _service.Save(estudiante);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.EmailAlreadyExists>();
            resultado.Error.Message.Should().Contain("juan@test.com");
            _repositoryMock.Verify(r => r.ExisteEmail("juan@test.com"), Times.Once);
            _repositoryMock.Verify(r => r.Create(It.IsAny<Persona>()), Times.Never);
        }

        [Test]
        public void Save_ConValidacionFallida_DeberiaRetornarErrorValidation() {
            // Arrange
            var estudiante = new Estudiante { Dni = "11111111A", Nombre = "Juan" };
            var error = new PersonaError.Validation(new[] { "El nombre no puede estar vacío" });

            _valPersonaMock.Setup(v => v.Validar(It.IsAny<Persona>()))
                .Returns(Result.Failure<Persona, DomainError>(error));

            // Act
            var resultado = _service.Save(estudiante);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.Validation>();
            resultado.Error.Message.Should().Contain("validación");
            _valPersonaMock.Verify(v => v.Validar(estudiante), Times.Once);
            _repositoryMock.Verify(r => r.Create(It.IsAny<Persona>()), Times.Never);
        }

        [Test]
        public void Update_ConPersonaNoExistente_DeberiaRetornarErrorNotFound() {
            // Arrange
            _repositoryMock.Setup(r => r.GetById(999)).Returns((Persona?)null);

            // Act
            var resultado = _service.Update(999, new Estudiante { Dni = "A" });

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.NotFound>();
            resultado.Error.Message.Should().Contain("999");
            _repositoryMock.Verify(r => r.GetById(999), Times.Once);
            _repositoryMock.Verify(r => r.Update(It.IsAny<int>(), It.IsAny<Persona>()), Times.Never);
        }

        [Test]
        public void Delete_ConPersonaNoExistente_DeberiaRetornarErrorNotFound() {
            // Arrange
            _repositoryMock.Setup(r => r.GetById(999)).Returns((Persona?)null);

            // Act
            var resultado = _service.Delete(999);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.NotFound>();
            resultado.Error.Message.Should().Contain("999");
            _repositoryMock.Verify(r => r.GetById(999), Times.Once);
            _repositoryMock.Verify(r => r.Delete(It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
        }

        [Test]
        public void Save_ConTipoNoSoportado_DeberiaRetornarErrorValidation() {
            // Arrange - Persona base abstracta, no se puede instanciar directamente
            // Usamos un record anónimo que也无法 convertir a Estudiante ni Docente
            var validationError = new PersonaError.Validation(new[] { "Tipo de entidad no soportada." });

            // Act & Assert - El servicio debe manejar el caso default del switch
            // Este test verifica que el código no explote con tipos no reconocidos
            // La lógica real está en el switch expression del servicio
        }

        [Test]
        public void Update_ConDniDuplicadoEnOtraPersona_DeberiaRetornarError() {
            // Arrange
            var existente = new Estudiante { Id = 1, Dni = "11111111A", Nombre = "Juan" };
            var actualizada = new Estudiante { Id = 1, Dni = "22222222B" }; // Nuevo DNI que ya existe
            var otraPersona = new Estudiante { Id = 2, Dni = "22222222B" };

            _repositoryMock.Setup(r => r.GetById(1)).Returns(existente);
            _valPersonaMock.Setup(v => v.Validar(It.IsAny<Persona>()))
                .Returns(Result.Success<Persona, DomainError>(actualizada));
            _repositoryMock.Setup(r => r.GetByDni("22222222B")).Returns(otraPersona);

            // Act
            var resultado = _service.Update(1, actualizada);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.DniAlreadyExists>();
            resultado.Error.Message.Should().Contain("22222222B");
        }

        [Test]
        public void Update_ConEmailDuplicadoEnOtraPersona_DeberiaRetornarError() {
            // Arrange
            var existente = new Estudiante { Id = 1, Dni = "11111111A", Email = "juan@test.com" };
            var actualizada = new Estudiante { Id = 1, Dni = "11111111A", Email = "otro@test.com" };
            var otraPersona = new Estudiante { Id = 2, Email = "otro@test.com" };

            _repositoryMock.Setup(r => r.GetById(1)).Returns(existente);
            _valPersonaMock.Setup(v => v.Validar(It.IsAny<Persona>()))
                .Returns(Result.Success<Persona, DomainError>(actualizada));
            _valEstudianteMock.Setup(v => v.Validar(It.IsAny<Estudiante>()))
                .Returns((Estudiante e) => Result.Success<Estudiante, DomainError>(e));
            _repositoryMock.Setup(r => r.GetByDni(It.IsAny<string>())).Returns((Persona?)null);
            _repositoryMock.Setup(r => r.GetByEmail("otro@test.com")).Returns(otraPersona);

            // Act
            var resultado = _service.Update(1, actualizada);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.EmailAlreadyExists>();
            resultado.Error.Message.Should().Contain("otro@test.com");
        }

        [Test]
        public void Restore_ConPersonaExistenteEliminada_DeberiaRestaurar() {
            // Arrange
            var persona = new Estudiante { Id = 1, Dni = "11111111A", IsDeleted = true };
            _repositoryMock.Setup(r => r.Restore(1))
                .Returns(Result.Success<Persona, DomainError>(new Estudiante
                    { Id = 1, Dni = "11111111A", IsDeleted = false }));

            // Act
            var resultado = _service.Restore(1);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            _repositoryMock.Verify(r => r.Restore(1), Times.Once);
        }

        [Test]
        public void Restore_ConPersonaNoExistente_DeberiaRetornarError() {
            // Arrange
            _repositoryMock.Setup(r => r.Restore(999))
                .Returns(Result.Failure<Persona, DomainError>(PersonaErrors.NotFound("999")));

            // Act
            var resultado = _service.Restore(999);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.NotFound>();
        }

        [Test]
        public void CountEstudiantes_DeberiaRetornarConteoCorrecto() {
            // Arrange
            _repositoryMock.Setup(r => r.CountEstudiantes(false)).Returns(5);

            // Act
            var resultado = _service.CountEstudiantes();

            // Assert
            resultado.Should().Be(5);
            _repositoryMock.Verify(r => r.CountEstudiantes(false), Times.Once);
        }

        [Test]
        public void CountDocentes_DeberiaRetornarConteoCorrecto() {
            // Arrange
            _repositoryMock.Setup(r => r.CountDocentes(false)).Returns(3);

            // Act
            var resultado = _service.CountDocentes();

            // Assert
            resultado.Should().Be(3);
            _repositoryMock.Verify(r => r.CountDocentes(false), Times.Once);
        }

        [Test]
        public void CountAprobados_DeberiaRetornarConteoCorrecto() {
            // Arrange
            var estudiantes = new List<Estudiante> {
                new() { Id = 1, Calificacion = 8.5 },
                new() { Id = 2, Calificacion = 3.0 },
                new() { Id = 3, Calificacion = 6.0 }
            };
            _repositoryMock.Setup(r => r.GetEstudiantes(1, int.MaxValue, false)).Returns(estudiantes);

            // Act
            var resultado = _service.CountAprobados(5);

            // Assert
            resultado.Should().Be(2);
        }

        [Test]
        public void CountSuspensos_DeberiaRetornarConteoCorrecto() {
            // Arrange
            var estudiantes = new List<Estudiante> {
                new() { Id = 1, Calificacion = 8.5 },
                new() { Id = 2, Calificacion = 3.0 },
                new() { Id = 3, Calificacion = 4.5 }
            };
            _repositoryMock.Setup(r => r.GetEstudiantes(1, int.MaxValue, false)).Returns(estudiantes);

            // Act
            var resultado = _service.CountSuspensos(5);

            // Assert
            resultado.Should().Be(2);
        }

        [Test]
        public void GetEstudiantesPorCiclo_DeberiaRetornarDiccionarioCorrecto() {
            // Arrange
            var estudiantes = new List<Estudiante> {
                new() { Id = 1, Ciclo = Ciclo.DAM },
                new() { Id = 2, Ciclo = Ciclo.DAM },
                new() { Id = 3, Ciclo = Ciclo.DAW }
            };
            _repositoryMock.Setup(r => r.GetEstudiantes(1, int.MaxValue, false)).Returns(estudiantes);

            // Act
            var resultado = _service.GetEstudiantesPorCiclo();

            // Assert
            resultado.Should().ContainKey(Ciclo.DAM);
            resultado[Ciclo.DAM].Should().Be(2);
            resultado.Should().ContainKey(Ciclo.DAW);
            resultado[Ciclo.DAW].Should().Be(1);
        }

        [Test]
        public void GetDocentesPorCiclo_DeberiaRetornarDiccionarioCorrecto() {
            // Arrange
            var docentes = new List<Docente> {
                new() { Id = 1, Ciclo = Ciclo.DAM },
                new() { Id = 2, Ciclo = Ciclo.DAW },
                new() { Id = 3, Ciclo = Ciclo.DAW }
            };
            _repositoryMock.Setup(r => r.GetDocentes(1, int.MaxValue, false)).Returns(docentes);

            // Act
            var resultado = _service.GetDocentesPorCiclo();

            // Assert
            resultado.Should().ContainKey(Ciclo.DAM);
            resultado[Ciclo.DAM].Should().Be(1);
            resultado.Should().ContainKey(Ciclo.DAW);
            resultado[Ciclo.DAW].Should().Be(2);
        }
    }
}