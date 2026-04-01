using GestionAcademica.Models.Personas;
using GestionAcademica.Models.Academia;
using GestionAcademica.Validators.Personas;
using GestionAcademica.Errors.Personas;
using NUnit.Framework;
using FluentAssertions;

namespace GestionAcademica.Test.Validators.Personas;

[TestFixture]
public class ValidadorEstudianteTests
{
    [TestFixture]
    public class CasosPositivos
    {
        private ValidadorEstudiante _validador = null!;

        [SetUp]
        public void SetUp()
        {
            _validador = new ValidadorEstudiante();
        }

        [Test]
        public void Validar_EstudianteValido_DeberiaRetornarSuccess()
        {
            // Arrange
            var estudiante = new Estudiante
            {
                Id = 1,
                Dni = "12345678H",
                Nombre = "Juan",
                Apellidos = "Pérez",
                FechaNacimiento = DateTime.Now.AddYears(-20),
                Email = "juan@test.com",
                Calificacion = 8.5,
                Ciclo = Ciclo.DAM,
                Curso = Curso.Primero
            };

            // Act
            var resultado = _validador.Validar(estudiante);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
        }

        [Test]
        public void Validar_CalificacionCero_DeberiaRetornarSuccess()
        {
            // Arrange
            var estudiante = new Estudiante
            {
                Id = 1,
                Dni = "12345678H",
                Nombre = "Juan",
                Apellidos = "Pérez",
                FechaNacimiento = DateTime.Now.AddYears(-20),
                Email = "juan@test.com",
                Calificacion = 0,
                Ciclo = Ciclo.DAM,
                Curso = Curso.Primero
            };

            // Act
            var resultado = _validador.Validar(estudiante);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
        }

        [Test]
        public void Validar_CalificacionMaxima_DeberiaRetornarSuccess()
        {
            // Arrange
            var estudiante = new Estudiante
            {
                Id = 1,
                Dni = "12345678H",
                Nombre = "Juan",
                Apellidos = "Pérez",
                FechaNacimiento = DateTime.Now.AddYears(-20),
                Email = "juan@test.com",
                Calificacion = 10,
                Ciclo = Ciclo.DAM,
                Curso = Curso.Primero
            };

            // Act
            var resultado = _validador.Validar(estudiante);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
        }

        [TestCase(Ciclo.DAM)]
        [TestCase(Ciclo.DAW)]
        [TestCase(Ciclo.ASIR)]
        public void Validar_DiferentesCiclos_DeberiaRetornarSuccess(Ciclo ciclo)
        {
            // Arrange
            var estudiante = new Estudiante
            {
                Id = 1,
                Dni = "12345678H",
                Nombre = "Juan",
                Apellidos = "Pérez",
                FechaNacimiento = DateTime.Now.AddYears(-20),
                Email = "juan@test.com",
                Calificacion = 8.5,
                Ciclo = ciclo,
                Curso = Curso.Primero
            };

            // Act
            var resultado = _validador.Validar(estudiante);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
        }

        [TestCase(Curso.Primero)]
        [TestCase(Curso.Segundo)]
        public void Validar_DiferentesCursos_DeberiaRetornarSuccess(Curso curso)
        {
            // Arrange
            var estudiante = new Estudiante
            {
                Id = 1,
                Dni = "12345678H",
                Nombre = "Juan",
                Apellidos = "Pérez",
                FechaNacimiento = DateTime.Now.AddYears(-20),
                Email = "juan@test.com",
                Calificacion = 8.5,
                Ciclo = Ciclo.DAM,
                Curso = curso
            };

            // Act
            var resultado = _validador.Validar(estudiante);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
        }
    }

    [TestFixture]
    public class CasosNegativos
    {
        private ValidadorEstudiante _validador = null!;

        [SetUp]
        public void SetUp()
        {
            _validador = new ValidadorEstudiante();
        }

        [Test]
        public void Validar_CalificacionNegativa_DeberiaRetornarFailure()
        {
            // Arrange
            var estudiante = new Estudiante
            {
                Id = 1,
                Dni = "12345678H",
                Nombre = "Juan",
                Apellidos = "Pérez",
                FechaNacimiento = DateTime.Now.AddYears(-20),
                Email = "juan@test.com",
                Calificacion = -1,
                Ciclo = Ciclo.DAM,
                Curso = Curso.Primero
            };

            // Act
            var resultado = _validador.Validar(estudiante);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.Validation>();
            (resultado.Error as PersonaError.Validation)?.Errors.Should().Contain("La calificación debe estar entre 0.0 y 10.0.");
            resultado.Error.Message.Should().Be("Se han detectado errores de validación en la entidad.");
        }

        [Test]
        public void Validar_CalificacionMayorDiez_DeberiaRetornarFailure()
        {
            // Arrange
            var estudiante = new Estudiante
            {
                Id = 1,
                Dni = "12345678H",
                Nombre = "Juan",
                Apellidos = "Pérez",
                FechaNacimiento = DateTime.Now.AddYears(-20),
                Email = "juan@test.com",
                Calificacion = 11,
                Ciclo = Ciclo.DAM,
                Curso = Curso.Primero
            };

            // Act
            var resultado = _validador.Validar(estudiante);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.Validation>();
            (resultado.Error as PersonaError.Validation)?.Errors.Should().Contain("La calificación debe estar entre 0.0 y 10.0.");
            resultado.Error.Message.Should().Be("Se han detectado errores de validación en la entidad.");
        }

        [Test]
        public void Validar_CicloInvalido_DeberiaRetornarFailure()
        {
            // Arrange
            var estudiante = new Estudiante
            {
                Id = 1,
                Dni = "12345678H",
                Nombre = "Juan",
                Apellidos = "Pérez",
                FechaNacimiento = DateTime.Now.AddYears(-20),
                Email = "juan@test.com",
                Calificacion = 8.5,
                Ciclo = (Ciclo)999,
                Curso = Curso.Primero
            };

            // Act
            var resultado = _validador.Validar(estudiante);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.Validation>();
            (resultado.Error as PersonaError.Validation)?.Errors.Should().Contain("El ciclo formativo no es válido.");
            resultado.Error.Message.Should().Be("Se han detectado errores de validación en la entidad.");
        }

        [Test]
        public void Validar_CursoInvalido_DeberiaRetornarFailure()
        {
            // Arrange
            var estudiante = new Estudiante
            {
                Id = 1,
                Dni = "12345678H",
                Nombre = "Juan",
                Apellidos = "Pérez",
                FechaNacimiento = DateTime.Now.AddYears(-20),
                Email = "juan@test.com",
                Calificacion = 8.5,
                Ciclo = Ciclo.DAM,
                Curso = (Curso)999
            };

            // Act
            var resultado = _validador.Validar(estudiante);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.Validation>();
            (resultado.Error as PersonaError.Validation)?.Errors.Should().Contain("El curso académico no es válido (Primero o Segundo).");
            resultado.Error.Message.Should().Be("Se han detectado errores de validación en la entidad.");
        }

        [Test]
        public void Validar_TipoNoEstudiante_DeberiaRetornarFailure()
        {
            // Arrange
            var docente = new Docente
            {
                Id = 1,
                Dni = "12345678H",
                Nombre = "Juan",
                Apellidos = "Pérez",
                FechaNacimiento = DateTime.Now.AddYears(-30),
                Email = "juan@test.com",
                Experiencia = 5,
                Especialidad = Modulos.Programacion,
                Ciclo = Ciclo.DAW
            };

            // Act
            var resultado = _validador.Validar(docente);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.Validation>();
            (resultado.Error as PersonaError.Validation)?.Errors.Should().Contain("La entidad proporcionada no es un Estudiante.");
            resultado.Error.Message.Should().Be("Se han detectado errores de validación en la entidad.");
        }
    }
}
