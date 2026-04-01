using GestionAcademica.Models.Personas;
using GestionAcademica.Models.Academia;
using GestionAcademica.Validators.Personas;
using GestionAcademica.Errors.Personas;
using NUnit.Framework;
using FluentAssertions;

namespace GestionAcademica.Test.Validators.Personas;

[TestFixture]
public class ValidadorPersonaTests
{
    [TestFixture]
    public class CasosPositivos
    {
        private ValidadorPersona _validador = null!;

        [SetUp]
        public void SetUp()
        {
            _validador = new ValidadorPersona();
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
        public void Validar_DocenteValido_DeberiaRetornarSuccess()
        {
            // Arrange
            var docente = new Docente
            {
                Id = 1,
                Dni = "87654321Z",
                Nombre = "María",
                Apellidos = "García",
                FechaNacimiento = DateTime.Now.AddYears(-30),
                Email = "maria@test.com",
                Experiencia = 10,
                Especialidad = Modulos.Programacion,
                Ciclo = Ciclo.DAW
            };

            // Act
            var resultado = _validador.Validar(docente);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
        }

        [Test]
        public void Validar_ConImagenValida_DeberiaRetornarSuccess()
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
                Imagen = "foto.png",
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
        public void Validar_ConImagenJpg_DeberiaRetornarSuccess()
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
                Imagen = "foto.jpg",
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
        public void Validar_EstudianteMayorEdad_DeberiaRetornarSuccess()
        {
            // Arrange
            var estudiante = new Estudiante
            {
                Id = 1,
                Dni = "12345678H",
                Nombre = "Juan",
                Apellidos = "Pérez",
                FechaNacimiento = DateTime.Now.AddYears(-18),
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
        public void Validar_ConCaracteresEspeciales_DeberiaRetornarSuccess()
        {
            // Arrange
            var estudiante = new Estudiante
            {
                Id = 1,
                Dni = "12345678H",
                Nombre = "José Luis",
                Apellidos = "García-Smith",
                FechaNacimiento = DateTime.Now.AddYears(-20),
                Email = "jose@test.com",
                Calificacion = 8.5,
                Ciclo = Ciclo.DAM,
                Curso = Curso.Primero
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
        private ValidadorPersona _validador = null!;

        [SetUp]
        public void SetUp()
        {
            _validador = new ValidadorPersona();
        }

        [Test]
        public void Validar_NombreVacio_DeberiaRetornarFailure()
        {
            // Arrange
            var estudiante = new Estudiante
            {
                Id = 1,
                Dni = "12345678H",
                Nombre = "",
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
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.Validation>();
            (resultado.Error as PersonaError.Validation)?.Errors.Should().Contain("El nombre es obligatorio (mín. 2 car.).");
            resultado.Error.Message.Should().Be("Se han detectado errores de validación en la entidad.");
        }

        [Test]
        public void Validar_NombreCorto_DeberiaRetornarFailure()
        {
            // Arrange
            var estudiante = new Estudiante
            {
                Id = 1,
                Dni = "12345678H",
                Nombre = "A",
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
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.Validation>();
            (resultado.Error as PersonaError.Validation)?.Errors.Should().Contain("El nombre es obligatorio (mín. 2 car.).");
            resultado.Error.Message.Should().Be("Se han detectado errores de validación en la entidad.");
        }

        [Test]
        public void Validar_ApellidosVacios_DeberiaRetornarFailure()
        {
            // Arrange
            var estudiante = new Estudiante
            {
                Id = 1,
                Dni = "12345678H",
                Nombre = "Juan",
                Apellidos = "",
                FechaNacimiento = DateTime.Now.AddYears(-20),
                Email = "juan@test.com",
                Calificacion = 8.5,
                Ciclo = Ciclo.DAM,
                Curso = Curso.Primero
            };

            // Act
            var resultado = _validador.Validar(estudiante);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.Validation>();
            (resultado.Error as PersonaError.Validation)?.Errors.Should().Contain("Los apellidos son obligatorios (mín. 2 car.).");
            resultado.Error.Message.Should().Be("Se han detectado errores de validación en la entidad.");
        }

        [Test]
        public void Validar_FechaFutura_DeberiaRetornarFailure()
        {
            // Arrange
            var estudiante = new Estudiante
            {
                Id = 1,
                Dni = "12345678H",
                Nombre = "Juan",
                Apellidos = "Pérez",
                FechaNacimiento = DateTime.UtcNow.AddYears(1).AddDays(1),
                Email = "juan@test.com",
                Calificacion = 8.5,
                Ciclo = Ciclo.DAM,
                Curso = Curso.Primero
            };

            // Act
            var resultado = _validador.Validar(estudiante);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.Validation>();
            (resultado.Error as PersonaError.Validation)?.Errors.Should().Contain("La fecha de nacimiento no puede ser futura.");
            resultado.Error.Message.Should().Be("Se han detectado errores de validación en la entidad.");
        }

        [Test]
        public void Validar_EmailInvalido_DeberiaRetornarFailure()
        {
            // Arrange
            var estudiante = new Estudiante
            {
                Id = 1,
                Dni = "12345678H",
                Nombre = "Juan",
                Apellidos = "Pérez",
                FechaNacimiento = DateTime.Now.AddYears(-20),
                Email = "email-invalido",
                Calificacion = 8.5,
                Ciclo = Ciclo.DAM,
                Curso = Curso.Primero
            };

            // Act
            var resultado = _validador.Validar(estudiante);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.Validation>();
            (resultado.Error as PersonaError.Validation)?.Errors.Should().Contain("El email no es válido.");
            resultado.Error.Message.Should().Be("Se han detectado errores de validación en la entidad.");
        }

        [Test]
        public void Validar_ImagenExtensionInvalida_DeberiaRetornarFailure()
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
                Imagen = "foto.gif",
                Calificacion = 8.5,
                Ciclo = Ciclo.DAM,
                Curso = Curso.Primero
            };

            // Act
            var resultado = _validador.Validar(estudiante);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.Validation>();
            (resultado.Error as PersonaError.Validation)?.Errors.Should().Contain("La imagen debe ser png, jpg, jpeg o bmp.");
            resultado.Error.Message.Should().Be("Se han detectado errores de validación en la entidad.");
        }

        [Test]
        public void Validar_EmailVacio_DeberiaRetornarFailure()
        {
            // Arrange
            var estudiante = new Estudiante
            {
                Id = 1,
                Dni = "12345678H",
                Nombre = "Juan",
                Apellidos = "Pérez",
                FechaNacimiento = DateTime.Now.AddYears(-20),
                Email = "",
                Calificacion = 8.5,
                Ciclo = Ciclo.DAM,
                Curso = Curso.Primero
            };

            // Act
            var resultado = _validador.Validar(estudiante);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.Validation>();
            (resultado.Error as PersonaError.Validation)?.Errors.Should().Contain("El email no es válido.");
            resultado.Error.Message.Should().Be("Se han detectado errores de validación en la entidad.");
        }

        [Test]
        public void Validar_EmailSinArroba_DeberiaRetornarFailure()
        {
            // Arrange
            var estudiante = new Estudiante
            {
                Id = 1,
                Dni = "12345678H",
                Nombre = "Juan",
                Apellidos = "Pérez",
                FechaNacimiento = DateTime.Now.AddYears(-20),
                Email = "emailtest.com",
                Calificacion = 8.5,
                Ciclo = Ciclo.DAM,
                Curso = Curso.Primero
            };

            // Act
            var resultado = _validador.Validar(estudiante);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.Validation>();
            (resultado.Error as PersonaError.Validation)?.Errors.Should().Contain("El email no es válido.");
            resultado.Error.Message.Should().Be("Se han detectado errores de validación en la entidad.");
        }
    }
}
