using FluentAssertions;
using GestionAcademica.Models.Personas;
using GestionAcademica.Models.Academia;
using NUnit.Framework;

namespace GestionAcademica.Test.Models.Personas;

[TestFixture]
public class EstudianteTests
{
    [TestFixture]
    public class CasosPositivos
    {
        [Test]
        public void ToString_DeberiaRetornarFormatoCorrecto()
        {
            // Arrange
            var estudiante = new Estudiante
            {
                Id = 1,
                Dni = "11111111H",
                Nombre = "Ana",
                Apellidos = "López",
                Calificacion = 8.5,
                Ciclo = Ciclo.DAM,
                Curso = Curso.Primero
            };

            // Act
            var resultado = estudiante.ToString();

            // Assert
            resultado.Should().Contain("[Estudiante]");
            resultado.Should().Contain("Ana López");
            resultado.Should().Contain("11111111H");
            resultado.Should().Contain("8,50");
        }

        [Test]
        public void CalificacionCualitativa_Suspenso_DeberiaRetornarSuspenso()
        {
            // Arrange
            var estudiante = new Estudiante { Calificacion = 3.0 };

            // Act
            var resultado = estudiante.CalificacionCualitativa;

            // Assert
            resultado.Should().Be("Suspenso");
        }

        [Test]
        public void CalificacionCualitativa_Aprobado_DeberiaRetornarAprobado()
        {
            // Arrange
            var estudiante = new Estudiante { Calificacion = 5.0 };

            // Act
            var resultado = estudiante.CalificacionCualitativa;

            // Assert
            resultado.Should().Be("Aprobado");
        }

        [Test]
        public void CalificacionCualitativa_Notable_DeberiaRetornarNotable()
        {
            // Arrange
            var estudiante = new Estudiante { Calificacion = 7.5 };

            // Act
            var resultado = estudiante.CalificacionCualitativa;

            // Assert
            resultado.Should().Be("Notable");
        }

        [Test]
        public void CalificacionCualitativa_Sobresaliente_DeberiaRetornarSobresaliente()
        {
            // Arrange
            var estudiante = new Estudiante { Calificacion = 9.5 };

            // Act
            var resultado = estudiante.CalificacionCualitativa;

            // Assert
            resultado.Should().Be("Sobresaliente");
        }

        [Test]
        public void NombreCompleto_DeberiaConcatenarNombreYApellidos()
        {
            // Arrange
            var estudiante = new Estudiante { Nombre = "Ana", Apellidos = "López García" };

            // Act
            var resultado = estudiante.NombreCompleto;

            // Assert
            resultado.Should().Be("Ana López García");
        }

        [Test]
        public void IsMayorEdad_Menor18_DeberiaRetornarFalse()
        {
            // Arrange
            var estudiante = new Estudiante { FechaNacimiento = DateTime.Now.AddYears(-16) };

            // Act
            var resultado = estudiante.IsMayorEdad;

            // Assert
            resultado.Should().BeFalse();
        }

        [Test]
        public void IsMayorEdad_Mayor18_DeberiaRetornarTrue()
        {
            // Arrange
            var estudiante = new Estudiante { FechaNacimiento = DateTime.Now.AddYears(-20) };

            // Act
            var resultado = estudiante.IsMayorEdad;

            // Assert
            resultado.Should().BeTrue();
        }

        [Test]
        public void Equals_MismoDni_DeberiaSerIgual()
        {
            // Arrange
            var estudiante1 = new Estudiante { Dni = "12345678Z", Nombre = "Juan" };
            var estudiante2 = new Estudiante { Dni = "12345678Z", Nombre = "Pedro" };

            // Act
            var resultado = estudiante1.Equals(estudiante2);

            // Assert
            resultado.Should().BeTrue();
        }

        [Test]
        public void Equals_DniDiferente_DeberiaSerDistinto()
        {
            // Arrange
            var estudiante1 = new Estudiante { Dni = "12345678Z" };
            var estudiante2 = new Estudiante { Dni = "87654321X" };

            // Act
            var resultado = estudiante1.Equals(estudiante2);

            // Assert
            resultado.Should().BeFalse();
        }

        [Test]
        public void GetHashCode_MismoDni_MismoHashCode()
        {
            // Arrange
            var estudiante1 = new Estudiante { Dni = "12345678Z" };
            var estudiante2 = new Estudiante { Dni = "12345678z" }; // lowercase

            // Act
            var hash1 = estudiante1.GetHashCode();
            var hash2 = estudiante2.GetHashCode();

            // Assert
            hash1.Should().Be(hash2);
        }
    }

    [TestFixture]
    public class CasosNegativos
    {
        [Test]
        public void Equals_Nulo_DeberiaRetornarFalse()
        {
            // Arrange
            var estudiante = new Estudiante { Dni = "12345678Z" };

            // Act
            var resultado = estudiante.Equals(null);

            // Assert
            resultado.Should().BeFalse();
        }
    }
}

[TestFixture]
public class DocenteTests
{
    [TestFixture]
    public class CasosPositivos
    {
        [Test]
        public void ToString_DeberiaRetornarFormatoCorrecto()
        {
            // Arrange
            var docente = new Docente
            {
                Id = 1,
                Dni = "87654321X",
                Nombre = "María",
                Apellidos = "García",
                Experiencia = 10,
                Especialidad = Modulos.Programacion,
                Ciclo = Ciclo.DAW
            };

            // Act
            var resultado = docente.ToString();

            // Assert
            resultado.Should().Contain("[Docente]");
            resultado.Should().Contain("María García");
            resultado.Should().Contain("87654321X");
            resultado.Should().Contain("10");
        }

        [Test]
        public void NombreCompleto_DeberiaConcatenarNombreYApellidos()
        {
            // Arrange
            var docente = new Docente { Nombre = "María", Apellidos = "García López" };

            // Act
            var resultado = docente.NombreCompleto;

            // Assert
            resultado.Should().Be("María García López");
        }
    }
}
