using GestionAcademica.Models.Personas;
using GestionAcademica.Models.Academia;
using GestionAcademica.Repositories.Personas.Json;
using GestionAcademica.Errors.Personas;
using NUnit.Framework;
using FluentAssertions;

namespace GestionAcademica.Test.Repositories.Personas.Json;

[TestFixture]
public class PersonasJsonRepositoryTests
{
    private string _tempFile = null!;

    [SetUp]
    public void SetUp()
    {
        _tempFile = Path.GetTempFileName();
    }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(_tempFile))
            File.Delete(_tempFile);
    }

    [TestFixture]
    public class CasosPositivos
    {
        private string _tempFile = null!;
        private PersonasJsonRepository _repository = null!;

        [SetUp]
        public void SetUp()
        {
            _tempFile = Path.GetTempFileName();
            _repository = new PersonasJsonRepository(_tempFile);
            _repository.DeleteAll();
        }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(_tempFile))
            File.Delete(_tempFile);
    }

        [Test]
        public void Create_EstudianteValido_DeberiaCrearCorrectamente()
        {
            // Arrange
            var estudiante = new Estudiante
            {
                Dni = "12345678H",
                Nombre = "Juan",
                Apellidos = "Pérez",
                Email = "juan@test.com",
                Calificacion = 8.5,
                Ciclo = Ciclo.DAM,
                Curso = Curso.Primero
            };

            // Act
            var resultado = _repository.Create(estudiante);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Id.Should().Be(1);
        }

        [Test]
        public void Create_DocenteValido_DeberiaCrearCorrectamente()
        {
            // Arrange
            var docente = new Docente
            {
                Dni = "87654321Z",
                Nombre = "María",
                Apellidos = "García",
                Email = "maria@test.com",
                Experiencia = 10,
                Especialidad = Modulos.Programacion,
                Ciclo = Ciclo.DAW
            };

            // Act
            var resultado = _repository.Create(docente);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Id.Should().Be(1);
        }

        [Test]
        public void GetById_CuandoExiste_DeberiaRetornarPersona()
        {
            // Arrange
            var estudiante = new Estudiante { Dni = "12345678H", Nombre = "Juan", Apellidos = "Pérez", Email = "juan@test.com", Calificacion = 8.5, Ciclo = Ciclo.DAM, Curso = Curso.Primero };
            _repository.Create(estudiante);

            // Act
            var resultado = _repository.GetById(1);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Id.Should().Be(1);
        }

        [Test]
        public void GetByDni_CuandoExiste_DeberiaRetornarPersona()
        {
            // Arrange
            var estudiante = new Estudiante { Dni = "12345678H", Nombre = "Juan", Apellidos = "Pérez", Email = "juan@test.com", Calificacion = 8.5, Ciclo = Ciclo.DAM, Curso = Curso.Primero };
            _repository.Create(estudiante);

            // Act
            var resultado = _repository.GetByDni("12345678H");

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Dni.Should().Be("12345678H");
        }

        [Test]
        public void GetByEmail_CuandoExiste_DeberiaRetornarPersona()
        {
            // Arrange
            var estudiante = new Estudiante { Dni = "12345678H", Nombre = "Juan", Apellidos = "Pérez", Email = "juan@test.com", Calificacion = 8.5, Ciclo = Ciclo.DAM, Curso = Curso.Primero };
            _repository.Create(estudiante);

            // Act
            var resultado = _repository.GetByEmail("juan@test.com");

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Email.Should().Be("juan@test.com");
        }

        [Test]
        public void ExisteDni_CuandoExiste_DeberiaRetornarTrue()
        {
            // Arrange
            var estudiante = new Estudiante { Dni = "12345678H", Nombre = "Juan", Apellidos = "Pérez", Email = "juan@test.com", Calificacion = 8.5, Ciclo = Ciclo.DAM, Curso = Curso.Primero };
            _repository.Create(estudiante);

            // Act
            var resultado = _repository.ExisteDni("12345678H");

            // Assert
            resultado.Should().BeTrue();
        }

        [Test]
        public void ExisteEmail_CuandoExiste_DeberiaRetornarTrue()
        {
            // Arrange
            var estudiante = new Estudiante { Dni = "12345678H", Nombre = "Juan", Apellidos = "Pérez", Email = "juan@test.com", Calificacion = 8.5, Ciclo = Ciclo.DAM, Curso = Curso.Primero };
            _repository.Create(estudiante);

            // Act
            var resultado = _repository.ExisteEmail("juan@test.com");

            // Assert
            resultado.Should().BeTrue();
        }

        [Test]
        public void GetAll_SinParametros_DeberiaRetornarTodos()
        {
            // Arrange
            _repository.Create(new Estudiante { Dni = "11111111H", Nombre = "Ana", Apellidos = "López", Email = "ana@test.com", Calificacion = 8.5, Ciclo = Ciclo.DAM, Curso = Curso.Primero });
            _repository.Create(new Estudiante { Dni = "22222222J", Nombre = "Pedro", Apellidos = "Ruiz", Email = "pedro@test.com", Calificacion = 5.0, Ciclo = Ciclo.DAW, Curso = Curso.Segundo });

            // Act
            var resultado = _repository.GetAll();

            // Assert
            resultado.Should().HaveCount(2);
        }

        [Test]
        public void GetAll_ConPaginacion_DeberiaRetornarPagina()
        {
            // Arrange
            for (int i = 1; i <= 5; i++)
            {
                _repository.Create(new Estudiante { Dni = $"{i:D8}H", Nombre = $"Nombre{i}", Apellidos = "Apellido", Email = $"test{i}@test.com", Calificacion = 5.0, Ciclo = Ciclo.DAM, Curso = Curso.Primero });
            }

            // Act
            var resultado = _repository.GetAll(page: 1, pageSize: 3);

            // Assert
            resultado.Should().HaveCount(3);
        }

        [Test]
        public void GetAll_SinIncluirBorrados_DeberiaRetornarSoloActivos()
        {
            // Arrange
            _repository.Create(new Estudiante { Dni = "11111111H", Nombre = "Ana", Apellidos = "López", Email = "ana@test.com", Calificacion = 8.5, Ciclo = Ciclo.DAM, Curso = Curso.Primero });
            var p2 = _repository.Create(new Estudiante { Dni = "22222222J", Nombre = "Pedro", Apellidos = "Ruiz", Email = "pedro@test.com", Calificacion = 5.0, Ciclo = Ciclo.DAW, Curso = Curso.Segundo }).Value;
            _repository.Delete(p2.Id, isLogical: true);

            // Act
            var resultado = _repository.GetAll(includeDeleted: false);

            // Assert
            resultado.Should().HaveCount(1);
            resultado.First().Dni.Should().Be("11111111H");
        }

        [Test]
        public void Update_ConDatosValidos_DeberiaActualizar()
        {
            // Arrange
            var estudiante = new Estudiante { Dni = "12345678H", Nombre = "Juan", Apellidos = "Pérez", Email = "juan@test.com", Calificacion = 8.5, Ciclo = Ciclo.DAM, Curso = Curso.Primero };
            _repository.Create(estudiante);

            var actualizado = new Estudiante { Dni = "12345678H", Nombre = "Juan Updated", Apellidos = "Pérez Updated", Email = "juanupdated@test.com", Calificacion = 9.0, Ciclo = Ciclo.DAW, Curso = Curso.Segundo };

            // Act
            var resultado = _repository.Update(1, actualizado);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Nombre.Should().Be("Juan Updated");
        }

        [Test]
        public void Delete_Logico_CuandoExiste_DeberiaRetornarPersona()
        {
            // Arrange
            var estudiante = new Estudiante { Dni = "12345678H", Nombre = "Juan", Apellidos = "Pérez", Email = "juan@test.com", Calificacion = 8.5, Ciclo = Ciclo.DAM, Curso = Curso.Primero };
            _repository.Create(estudiante);

            // Act
            var resultado = _repository.Delete(1, isLogical: true);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.IsDeleted.Should().BeTrue();
        }

        [Test]
        public void Delete_Fisico_CuandoExiste_DeberiaRetornarPersona()
        {
            // Arrange
            var estudiante = new Estudiante { Dni = "12345678H", Nombre = "Juan", Apellidos = "Pérez", Email = "juan@test.com", Calificacion = 8.5, Ciclo = Ciclo.DAM, Curso = Curso.Primero };
            _repository.Create(estudiante);

            // Act
            var resultado = _repository.Delete(1, isLogical: false);

            // Assert
            resultado.Should().NotBeNull();
            _repository.GetById(1).Should().BeNull();
        }

        [Test]
        public void DeleteAll_CuandoHayDatos_DeberiaEliminarTodos()
        {
            // Arrange
            _repository.Create(new Estudiante { Dni = "11111111H", Nombre = "Ana", Apellidos = "López", Email = "ana@test.com", Calificacion = 8.5, Ciclo = Ciclo.DAM, Curso = Curso.Primero });
            _repository.Create(new Estudiante { Dni = "22222222J", Nombre = "Pedro", Apellidos = "Ruiz", Email = "pedro@test.com", Calificacion = 5.0, Ciclo = Ciclo.DAW, Curso = Curso.Segundo });

            // Act
            var resultado = _repository.DeleteAll();

            // Assert
            resultado.Should().BeTrue();
            _repository.GetAll().Should().BeEmpty();
        }
    }

    [TestFixture]
    public class CasosNegativos
    {
        private string _tempFile = null!;
        private PersonasJsonRepository _repository = null!;

        [SetUp]
        public void SetUp()
        {
            _tempFile = Path.GetTempFileName();
            _repository = new PersonasJsonRepository(_tempFile);
            _repository.DeleteAll();
        }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(_tempFile))
            File.Delete(_tempFile);
    }

        [Test]
        public void Create_ConDniExistente_DeberiaRetornarFailure()
        {
            // Arrange
            var estudiante1 = new Estudiante { Dni = "12345678H", Nombre = "Juan", Apellidos = "Pérez", Email = "juan1@test.com", Calificacion = 8.5, Ciclo = Ciclo.DAM, Curso = Curso.Primero };
            var estudiante2 = new Estudiante { Dni = "12345678H", Nombre = "Pedro", Apellidos = "García", Email = "juan2@test.com", Calificacion = 5.0, Ciclo = Ciclo.DAW, Curso = Curso.Segundo };
            _repository.Create(estudiante1);

            // Act
            var resultado = _repository.Create(estudiante2);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.DniAlreadyExists>();
            (resultado.Error as PersonaError.DniAlreadyExists)?.Dni.Should().Be("12345678H");
            resultado.Error.Message.Should().Contain("12345678H");
        }

        [Test]
        public void Create_ConEmailExistente_DeberiaRetornarFailure()
        {
            // Arrange
            var estudiante1 = new Estudiante { Dni = "11111111H", Nombre = "Juan", Apellidos = "Pérez", Email = "juan@test.com", Calificacion = 8.5, Ciclo = Ciclo.DAM, Curso = Curso.Primero };
            var estudiante2 = new Estudiante { Dni = "22222222J", Nombre = "Pedro", Apellidos = "García", Email = "juan@test.com", Calificacion = 5.0, Ciclo = Ciclo.DAW, Curso = Curso.Segundo };
            _repository.Create(estudiante1);

            // Act
            var resultado = _repository.Create(estudiante2);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.EmailAlreadyExists>();
            (resultado.Error as PersonaError.EmailAlreadyExists)?.Email.Should().Be("juan@test.com");
            resultado.Error.Message.Should().Contain("juan@test.com");
        }

        [Test]
        public void GetById_CuandoNoExiste_DeberiaRetornarNull()
        {
            // Act
            var resultado = _repository.GetById(999);

            // Assert
            resultado.Should().BeNull();
        }

        [Test]
        public void GetByDni_CuandoNoExiste_DeberiaRetornarNull()
        {
            // Act
            var resultado = _repository.GetByDni("99999999Z");

            // Assert
            resultado.Should().BeNull();
        }

        [Test]
        public void GetByEmail_CuandoNoExiste_DeberiaRetornarNull()
        {
            // Act
            var resultado = _repository.GetByEmail("noexiste@test.com");

            // Assert
            resultado.Should().BeNull();
        }

        [Test]
        public void ExisteDni_CuandoNoExiste_DeberiaRetornarFalse()
        {
            // Act
            var resultado = _repository.ExisteDni("99999999Z");

            // Assert
            resultado.Should().BeFalse();
        }

        [Test]
        public void ExisteEmail_CuandoNoExiste_DeberiaRetornarFalse()
        {
            // Act
            var resultado = _repository.ExisteEmail("noexiste@test.com");

            // Assert
            resultado.Should().BeFalse();
        }

        [Test]
        public void Update_CuandoNoExiste_DeberiaRetornarFailure()
        {
            // Arrange
            var estudiante = new Estudiante { Dni = "12345678H", Nombre = "Juan", Apellidos = "Pérez", Email = "juan@test.com", Calificacion = 8.5, Ciclo = Ciclo.DAM, Curso = Curso.Primero };

            // Act
            var resultado = _repository.Update(999, estudiante);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.NotFound>();
            (resultado.Error as PersonaError.NotFound)?.Id.Should().Be("999");
            resultado.Error.Message.Should().Contain("999");
        }

        [Test]
        public void Update_ConDniExistenteEnOtro_DeberiaRetornarFailure()
        {
            // Arrange
            var estudiante1 = new Estudiante { Dni = "11111111H", Nombre = "Juan", Apellidos = "Pérez", Email = "juan1@test.com", Calificacion = 8.5, Ciclo = Ciclo.DAM, Curso = Curso.Primero };
            var estudiante2 = new Estudiante { Dni = "22222222J", Nombre = "Pedro", Apellidos = "García", Email = "juan2@test.com", Calificacion = 5.0, Ciclo = Ciclo.DAW, Curso = Curso.Segundo };
            _repository.Create(estudiante1);
            _repository.Create(estudiante2);

            // Act
            var resultado = _repository.Update(2, estudiante1);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.DniAlreadyExists>();
            (resultado.Error as PersonaError.DniAlreadyExists)?.Dni.Should().Be("11111111H");
        }

        [Test]
        public void Update_ConEmailExistenteEnOtro_DeberiaRetornarFailure()
        {
            // Arrange
            var estudiante1 = new Estudiante { Dni = "11111111H", Nombre = "Juan", Apellidos = "Pérez", Email = "juan@test.com", Calificacion = 8.5, Ciclo = Ciclo.DAM, Curso = Curso.Primero };
            var estudiante2 = new Estudiante { Dni = "22222222J", Nombre = "Pedro", Apellidos = "García", Email = "pedro@test.com", Calificacion = 5.0, Ciclo = Ciclo.DAW, Curso = Curso.Segundo };
            _repository.Create(estudiante1);
            _repository.Create(estudiante2);

            var actualizado = new Estudiante { Dni = "22222222J", Nombre = "Pedro", Apellidos = "García", Email = "juan@test.com", Calificacion = 5.0, Ciclo = Ciclo.DAW, Curso = Curso.Segundo };

            // Act
            var resultado = _repository.Update(2, actualizado);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.EmailAlreadyExists>();
            (resultado.Error as PersonaError.EmailAlreadyExists)?.Email.Should().Be("juan@test.com");
        }

        [Test]
        public void Delete_CuandoNoExiste_DeberiaRetornarNull()
        {
            // Act
            var resultado = _repository.Delete(999);

            // Assert
            resultado.Should().BeNull();
        }

        [Test]
        public void Restore_CuandoNoExiste_DeberiaRetornarFailure()
        {
            // Act
            var resultado = _repository.Restore(999);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.NotFound>();
        }
    }

    [TestFixture]
    public class CasosMixtos
    {
        private string _tempFile = null!;
        private PersonasJsonRepository _repository = null!;

        [SetUp]
        public void SetUp()
        {
            _tempFile = Path.GetTempFileName();
            _repository = new PersonasJsonRepository(_tempFile);
            _repository.DeleteAll();
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_tempFile))
                File.Delete(_tempFile);
        }

        [Test]
        public void Restore_CuandoEliminadoLogicamente_DeberiaRestaurar()
        {
            // Arrange
            var estudiante = new Estudiante
            {
                Dni = "12345678H",
                Nombre = "Juan",
                Apellidos = "Pérez",
                Email = "juan@test.com",
                Calificacion = 8.5,
                Ciclo = Ciclo.DAM,
                Curso = Curso.Primero
            };
            var creada = _repository.Create(estudiante).Value;
            _repository.Delete(creada.Id, isLogical: true);

            // Act
            var resultado = _repository.Restore(creada.Id);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.IsDeleted.Should().BeFalse();
            resultado.Value.DeletedAt.Should().BeNull();
        }

        [Test]
        public void CountEstudiantes_SinEliminados_DeberiaContarSoloActivos()
        {
            // Arrange
            _repository.Create(new Estudiante { Dni = "11111111H", Nombre = "Ana", Apellidos = "López", Email = "ana@test.com", Calificacion = 8.5, Ciclo = Ciclo.DAM, Curso = Curso.Primero });
            var p2 = _repository.Create(new Estudiante { Dni = "22222222J", Nombre = "Pedro", Apellidos = "Ruiz", Email = "pedro@test.com", Calificacion = 5.0, Ciclo = Ciclo.DAW, Curso = Curso.Segundo }).Value;
            _repository.Create(new Docente { Dni = "33333333P", Nombre = "Ana", Apellidos = "García", Email = "maria@test.com", Experiencia = 10, Especialidad = Modulos.Programacion, Ciclo = Ciclo.DAW });
            _repository.Delete(p2.Id, isLogical: true);

            // Act
            var resultado = _repository.CountEstudiantes(false);

            // Assert
            resultado.Should().Be(1);
        }

        [Test]
        public void CountEstudiantes_IncluyendoEliminados_DeberiaContarTodos()
        {
            // Arrange
            _repository.Create(new Estudiante { Dni = "11111111H", Nombre = "Ana", Apellidos = "López", Email = "ana@test.com", Calificacion = 8.5, Ciclo = Ciclo.DAM, Curso = Curso.Primero });
            var p2 = _repository.Create(new Estudiante { Dni = "22222222J", Nombre = "Pedro", Apellidos = "Ruiz", Email = "pedro@test.com", Calificacion = 5.0, Ciclo = Ciclo.DAW, Curso = Curso.Segundo }).Value;
            _repository.Delete(p2.Id, isLogical: true);

            // Act
            var resultado = _repository.CountEstudiantes(true);

            // Assert
            resultado.Should().Be(2);
        }

        [Test]
        public void CountDocentes_SinEliminados_DeberiaContarSoloActivos()
        {
            // Arrange
            _repository.Create(new Docente { Dni = "33333333P", Nombre = "Ana", Apellidos = "García", Email = "ana@test.com", Experiencia = 10, Especialidad = Modulos.Programacion, Ciclo = Ciclo.DAW });
            var p2 = _repository.Create(new Docente { Dni = "44444444A", Nombre = "Pedro", Apellidos = "Ruiz", Email = "pedro@test.com", Experiencia = 5, Especialidad = Modulos.BasesDatos, Ciclo = Ciclo.DAM }).Value;
            _repository.Delete(p2.Id, isLogical: true);

            // Act
            var resultado = _repository.CountDocentes(false);

            // Assert
            resultado.Should().Be(1);
        }

        [Test]
        public void CountDocentes_IncluyendoEliminados_DeberiaContarTodos()
        {
            // Arrange
            _repository.Create(new Docente { Dni = "33333333P", Nombre = "Ana", Apellidos = "García", Email = "ana@test.com", Experiencia = 10, Especialidad = Modulos.Programacion, Ciclo = Ciclo.DAW });
            var p2 = _repository.Create(new Docente { Dni = "44444444A", Nombre = "Pedro", Apellidos = "Ruiz", Email = "pedro@test.com", Experiencia = 5, Especialidad = Modulos.BasesDatos, Ciclo = Ciclo.DAM }).Value;
            _repository.Delete(p2.Id, isLogical: true);

            // Act
            var resultado = _repository.CountDocentes(true);

            // Assert
            resultado.Should().Be(2);
        }

        [Test]
        public void GetEstudiantes_DeberiaRetornarSoloEstudiantes()
        {
            // Arrange
            _repository.Create(new Estudiante { Dni = "11111111H", Nombre = "Ana", Apellidos = "López", Email = "ana@test.com", Calificacion = 8.5, Ciclo = Ciclo.DAM, Curso = Curso.Primero });
            _repository.Create(new Docente { Dni = "33333333P", Nombre = "Ana", Apellidos = "García", Email = "maria@test.com", Experiencia = 10, Especialidad = Modulos.Programacion, Ciclo = Ciclo.DAW });

            // Act
            var resultado = _repository.GetEstudiantes(1, 10, false);

            // Assert
            resultado.Should().HaveCount(1);
            resultado.First().Should().BeOfType<Estudiante>();
        }

        [Test]
        public void GetDocentes_DeberiaRetornarSoloDocentes()
        {
            // Arrange
            _repository.Create(new Estudiante { Dni = "11111111H", Nombre = "Ana", Apellidos = "López", Email = "ana@test.com", Calificacion = 8.5, Ciclo = Ciclo.DAM, Curso = Curso.Primero });
            _repository.Create(new Docente { Dni = "33333333P", Nombre = "Ana", Apellidos = "García", Email = "maria@test.com", Experiencia = 10, Especialidad = Modulos.Programacion, Ciclo = Ciclo.DAW });

            // Act
            var resultado = _repository.GetDocentes(1, 10, false);

            // Assert
            resultado.Should().HaveCount(1);
            resultado.First().Should().BeOfType<Docente>();
        }
    }
}
