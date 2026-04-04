using GestionAcademica.Models.Personas;
using GestionAcademica.Models.Academia;
using GestionAcademica.Validators.Personas;
using GestionAcademica.Errors.Personas;
using NUnit.Framework;
using FluentAssertions;

namespace GestionAcademica.Test.Validators.Personas;

[TestFixture]
public class ValidadorDocenteTests
{
    [TestFixture]
    public class CasosPositivos
    {
        private ValidadorDocente _validador = null!;

        [SetUp]
        public void SetUp()
        {
            _validador = new ValidadorDocente();
        }

        [Test]
        public void Validar_DocenteValido_DeberiaRetornarSuccess()
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
        public void Validar_ExperienciaValida_DeberiaRetornarSuccess()
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
                Especialidad = Modulos.BasesDatos,
                Ciclo = Ciclo.DAM
            };

            // Act
            var resultado = _validador.Validar(docente);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
        }

        [Test]
        public void Validar_ExperienciaCero_DeberiaRetornarSuccess()
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
                Experiencia = 0,
                Especialidad = Modulos.Entornos,
                Ciclo = Ciclo.ASIR
            };

            // Act
            var resultado = _validador.Validar(docente);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
        }

        [TestCase(Modulos.Programacion)]
        [TestCase(Modulos.BasesDatos)]
        [TestCase(Modulos.Entornos)]
        [TestCase(Modulos.LenguajesMarcas)]
        public void Validar_DiferentesEspecialidades_DeberiaRetornarSuccess(string especialidad)
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
                Experiencia = 10,
                Especialidad = especialidad,
                Ciclo = Ciclo.DAW
            };

            // Act
            var resultado = _validador.Validar(docente);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
        }
    }

    [TestFixture]
    public class CasosNegativos
    {
        private ValidadorDocente _validador = null!;

        [SetUp]
        public void SetUp()
        {
            _validador = new ValidadorDocente();
        }

        [Test]
        public void Validar_ExperienciaNegativa_DeberiaRetornarFailure()
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
                Experiencia = -1,
                Especialidad = Modulos.Programacion,
                Ciclo = Ciclo.DAW
            };

            // Act
            var resultado = _validador.Validar(docente);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.Validation>();
            (resultado.Error as PersonaError.Validation)?.Errors.Should().Contain("Los años de experiencia no pueden ser negativos.");
            resultado.Error.Message.Should().Contain("Se han detectado errores de validación en la entidad:");
        }

        [Test]
        public void Validar_TipoNoDocente_DeberiaRetornarFailure()
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
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.Validation>();
            (resultado.Error as PersonaError.Validation)?.Errors.Should().Contain("La entidad proporcionada no es un Docente.");
            resultado.Error.Message.Should().Contain("Se han detectado errores de validación en la entidad:");
        }

        [Test]
        public void Validar_EspecialidadVacia_DeberiaRetornarFailure()
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
                Experiencia = 10,
                Especialidad = "",
                Ciclo = Ciclo.DAW
            };

            // Act
            var resultado = _validador.Validar(docente);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.Validation>();
            (resultado.Error as PersonaError.Validation)?.Errors.Should().Contain("La especialidad o módulo docente debe estar definida.");
        }

        [Test]
        public void Validar_EspecialidadNula_DeberiaRetornarFailure()
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
                Experiencia = 10,
                Especialidad = null!,
                Ciclo = Ciclo.DAW
            };

            // Act
            var resultado = _validador.Validar(docente);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.Validation>();
            (resultado.Error as PersonaError.Validation)?.Errors.Should().Contain("La especialidad o módulo docente debe estar definida.");
        }

        [Test]
        public void Validar_CicloInvalido_DeberiaRetornarFailure()
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
                Experiencia = 10,
                Especialidad = Modulos.Programacion,
                Ciclo = (Ciclo)999
            };

            // Act
            var resultado = _validador.Validar(docente);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.Validation>();
            (resultado.Error as PersonaError.Validation)?.Errors.Should().Contain("El ciclo asignado no es un ciclo oficial válido.");
        }
    }
}
