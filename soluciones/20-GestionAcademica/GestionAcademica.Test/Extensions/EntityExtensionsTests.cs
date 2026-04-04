using FluentAssertions;
using GestionAcademica.Extensions;
using GestionAcademica.Models.Personas;
using GestionAcademica.Models.Academia;
using NUnit.Framework;

namespace GestionAcademica.Test.Extensions;

[TestFixture]
public class EntityExtensionsTests
{
    [TestFixture]
    public class CasosPositivos
    {
        [Test]
        public void Clone_Estudiante_DeberiaCrearCopiaIndependiente()
        {
            // Arrange
            var estudiante = new Estudiante
            {
                Id = 1,
                Dni = "12345678Z",
                Nombre = "Juan",
                Apellidos = "Pérez",
                Calificacion = 8.5,
                Ciclo = Ciclo.DAM,
                Curso = Curso.Primero,
                IsDeleted = true,
                DeletedAt = DateTime.UtcNow
            };

            // Act
            var copia = estudiante.Clone();

            // Assert
            copia.Should().NotBeNull();
            copia.Id.Should().Be(estudiante.Id);
            copia.Dni.Should().Be(estudiante.Dni);
            copia.Nombre.Should().Be(estudiante.Nombre);
            copia.Apellidos.Should().Be(estudiante.Apellidos);
            copia.Calificacion.Should().Be(estudiante.Calificacion);
            copia.Ciclo.Should().Be(estudiante.Ciclo);
            copia.Curso.Should().Be(estudiante.Curso);
            copia.IsDeleted.Should().BeFalse();
            copia.DeletedAt.Should().BeNull();
        }

        [Test]
        public void Clone_Docente_DeberiaCrearCopiaIndependiente()
        {
            // Arrange
            var docente = new Docente
            {
                Id = 2,
                Dni = "87654321X",
                Nombre = "Ana",
                Apellidos = "García",
                Experiencia = 10,
                Especialidad = Modulos.Programacion,
                Ciclo = Ciclo.DAW,
                IsDeleted = true,
                DeletedAt = DateTime.UtcNow
            };

            // Act
            var copia = docente.Clone();

            // Assert
            copia.Should().NotBeNull();
            copia.Id.Should().Be(docente.Id);
            copia.Dni.Should().Be(docente.Dni);
            copia.Nombre.Should().Be(docente.Nombre);
            copia.Experiencia.Should().Be(docente.Experiencia);
            copia.Especialidad.Should().Be(docente.Especialidad);
            copia.Ciclo.Should().Be(docente.Ciclo);
            copia.IsDeleted.Should().BeFalse();
            copia.DeletedAt.Should().BeNull();
        }

        [Test]
        public void Clone_EstudianteNoEliminado_DeberiaMantenerPropiedades()
        {
            // Arrange
            var estudiante = new Estudiante
            {
                Id = 1,
                Dni = "11111111H",
                Nombre = "Ana",
                Apellidos = "López",
                Calificacion = 9.0,
                Ciclo = Ciclo.DAM,
                Curso = Curso.Segundo
            };

            // Act
            var copia = estudiante.Clone();

            // Assert
            copia.Nombre.Should().Be("Ana");
            copia.Calificacion.Should().Be(9.0);
            copia.IsDeleted.Should().BeFalse();
        }
    }

    [TestFixture]
    public class CasosNegativos
    {
        [Test]
        public void Clone_EstudianteNulo_DeberiaLanzarExcepcion()
        {
            // Arrange
            Estudiante? estudiante = null;

            // Act & Assert
            var act = () => estudiante!.Clone();
            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Clone_DocenteNulo_DeberiaLanzarExcepcion()
        {
            // Arrange
            Docente? docente = null;

            // Act & Assert
            var act = () => docente!.Clone();
            act.Should().Throw<ArgumentNullException>();
        }
    }
}
