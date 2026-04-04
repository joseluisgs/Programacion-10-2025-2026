using GestionAcademica.Dto.Personas;
using GestionAcademica.Entity;
using GestionAcademica.Mappers.Personas;
using GestionAcademica.Models.Personas;
using GestionAcademica.Models.Academia;
using NUnit.Framework;
using FluentAssertions;

namespace GestionAcademica.Test.Mappers.Personas;

[TestFixture]
public class PersonaMapperTests
{
    [TestFixture]
    public class CasosPositivos
    {
        private Estudiante _estudiante = null!;
        private Docente _docente = null!;
        private PersonaDto _dtoEstudiante = null!;
        private PersonaDto _dtoDocente = null!;
        private PersonaEntity _entityEstudiante = null!;
        private PersonaEntity _entityDocente = null!;

        [SetUp]
        public void SetUp()
        {
            _estudiante = new Estudiante
            {
                Id = 1,
                Dni = "12345678H",
                Nombre = "Juan",
                Apellidos = "Pérez",
                FechaNacimiento = new DateTime(2000, 5, 15),
                Email = "juan@test.com",
                Imagen = "foto.jpg",
                Calificacion = 8.5,
                Ciclo = Ciclo.DAM,
                Curso = Curso.Primero,
                CreatedAt = new DateTime(2024, 1, 1, 10, 0, 0),
                UpdatedAt = new DateTime(2024, 1, 2, 12, 0, 0),
                IsDeleted = false,
                DeletedAt = null
            };

            _docente = new Docente
            {
                Id = 2,
                Dni = "87654321Z",
                Nombre = "María",
                Apellidos = "García",
                FechaNacimiento = new DateTime(1985, 3, 20),
                Email = "maria@test.com",
                Imagen = null,
                Experiencia = 10,
                Especialidad = Modulos.Programacion,
                Ciclo = Ciclo.DAW,
                CreatedAt = new DateTime(2024, 1, 1, 10, 0, 0),
                UpdatedAt = new DateTime(2024, 1, 2, 12, 0, 0),
                IsDeleted = false,
                DeletedAt = null
            };

            _dtoEstudiante = new PersonaDto(
                1, "12345678H", "Juan", "Pérez",
                "15/05/2000", "juan@test.com", "foto.jpg",
                "Estudiante", null, null, "DAM", "Primero", "8.5",
                "2024-01-01T10:00:00", "2024-01-02T12:00:00", false, null);

            _dtoDocente = new PersonaDto(
                2, "87654321Z", "María", "García",
                "20/03/1985", "maria@test.com", null,
                "Docente", "10", Modulos.Programacion, "DAW", null, null,
                "2024-01-01T10:00:00", "2024-01-02T12:00:00", false, null);

            _entityEstudiante = new PersonaEntity
            {
                Id = 1,
                Dni = "12345678H",
                Nombre = "Juan",
                Apellidos = "Pérez",
                FechaNacimiento = new DateTime(2000, 5, 15),
                Email = "juan@test.com",
                Imagen = "foto.jpg",
                Tipo = "Estudiante",
                Calificacion = 8.5,
                Ciclo = 0,
                Curso = 1,
                CreatedAt = new DateTime(2024, 1, 1, 10, 0, 0),
                UpdatedAt = new DateTime(2024, 1, 2, 12, 0, 0),
                IsDeleted = false,
                DeletedAt = null
            };

            _entityDocente = new PersonaEntity
            {
                Id = 2,
                Dni = "87654321Z",
                Nombre = "María",
                Apellidos = "García",
                FechaNacimiento = new DateTime(1985, 3, 20),
                Email = "maria@test.com",
                Imagen = null,
                Tipo = "Docente",
                Experiencia = 10,
                Especialidad = Modulos.Programacion,
                Ciclo = 1,
                CreatedAt = new DateTime(2024, 1, 1, 10, 0, 0),
                UpdatedAt = new DateTime(2024, 1, 2, 12, 0, 0),
                IsDeleted = false,
                DeletedAt = null
            };
        }

        [Test]
        public void ToDto_Estudiante_DeberiaConvertirCorrectamente()
        {
            // Act
            var resultado = _estudiante.ToDto();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(1);
            resultado.Dni.Should().Be("12345678H");
            resultado.Nombre.Should().Be("Juan");
            resultado.Apellidos.Should().Be("Pérez");
            resultado.Tipo.Should().Be("Estudiante");
            resultado.Calificacion.Should().Be("8.5");
            resultado.Ciclo.Should().Be("DAM");
            resultado.Curso.Should().Be("Primero");
        }

        [Test]
        public void ToDto_Docente_DeberiaConvertirCorrectamente()
        {
            // Act
            var resultado = _docente.ToDto();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(2);
            resultado.Dni.Should().Be("87654321Z");
            resultado.Nombre.Should().Be("María");
            resultado.Tipo.Should().Be("Docente");
            resultado.Experiencia.Should().Be("10");
            resultado.Especialidad.Should().Be(Modulos.Programacion);
            resultado.Ciclo.Should().Be("DAW");
        }

        [Test]
        public void ToModel_DtoEstudiante_DeberiaConvertirCorrectamente()
        {
            // Arrange - usar fecha en formato ISO para evitar problemas de cultura
            var dto = new PersonaDto(
                1, "12345678H", "Juan", "Pérez",
                "2000-05-15", "juan@test.com", "foto.jpg",
                "Estudiante", null, null, "DAM", "Primero", "8.5",
                "2024-01-01T10:00:00", "2024-01-02T12:00:00", false, null);

            // Act
            var resultado = dto.ToModel();

            // Assert
            resultado.Should().BeOfType<Estudiante>();
            var estudiante = resultado as Estudiante;
            estudiante!.Id.Should().Be(1);
            estudiante.Dni.Should().Be("12345678H");
            estudiante.Nombre.Should().Be("Juan");
            estudiante.Calificacion.Should().Be(8.5);
            estudiante.Ciclo.Should().Be(Ciclo.DAM);
            estudiante.Curso.Should().Be(Curso.Primero);
        }

        [Test]
        public void ToModel_DtoDocente_DeberiaConvertirCorrectamente()
        {
            // Arrange - usar fecha en formato ISO para evitar problemas de cultura
            var dto = new PersonaDto(
                2, "87654321Z", "María", "García",
                "1985-03-20", "maria@test.com", null,
                "Docente", "10", Modulos.Programacion, "DAW", null, null,
                "2024-01-01T10:00:00", "2024-01-02T12:00:00", false, null);

            // Act
            var resultado = dto.ToModel();

            // Assert
            resultado.Should().BeOfType<Docente>();
            var docente = resultado as Docente;
            docente!.Id.Should().Be(2);
            docente.Dni.Should().Be("87654321Z");
            docente.Nombre.Should().Be("María");
            docente.Experiencia.Should().Be(10);
            docente.Especialidad.Should().Be(Modulos.Programacion);
            docente.Ciclo.Should().Be(Ciclo.DAW);
        }

        [Test]
        public void ToModel_EntityEstudiante_DeberiaConvertirCorrectamente()
        {
            // Act
            var resultado = _entityEstudiante.ToModel();

            // Assert
            resultado.Should().BeOfType<Estudiante>();
            var estudiante = resultado as Estudiante;
            estudiante!.Id.Should().Be(1);
            estudiante.Dni.Should().Be("12345678H");
            estudiante.Calificacion.Should().Be(8.5);
            estudiante.Ciclo.Should().Be(Ciclo.DAM);
            estudiante.Curso.Should().Be(Curso.Primero);
        }

        [Test]
        public void ToModel_EntityDocente_DeberiaConvertirCorrectamente()
        {
            // Act
            var resultado = _entityDocente.ToModel();

            // Assert
            resultado.Should().BeOfType<Docente>();
            var docente = resultado as Docente;
            docente!.Id.Should().Be(2);
            docente.Experiencia.Should().Be(10);
            docente.Especialidad.Should().Be(Modulos.Programacion);
            docente.Ciclo.Should().Be(Ciclo.DAW);
        }

        [Test]
        public void ToEntity_Estudiante_DeberiaConvertirCorrectamente()
        {
            // Act
            var resultado = _estudiante.ToEntity();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(1);
            resultado.Dni.Should().Be("12345678H");
            resultado.Tipo.Should().Be("Estudiante");
            resultado.Calificacion.Should().Be(8.5);
            resultado.Ciclo.Should().Be(0);
            resultado.Curso.Should().Be(1);
        }

        [Test]
        public void ToEntity_Docente_DeberiaConvertirCorrectamente()
        {
            // Act
            var resultado = _docente.ToEntity();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(2);
            resultado.Tipo.Should().Be("Docente");
            resultado.Experiencia.Should().Be(10);
            resultado.Especialidad.Should().Be(Modulos.Programacion);
            resultado.Ciclo.Should().Be(1);
        }

        [Test]
        public void ToModel_ListaEntities_DeberiaConvertirTodos()
        {
            // Arrange
            var entities = new List<PersonaEntity> { _entityEstudiante, _entityDocente };

            // Act
            var resultado = entities.ToModel();

            // Assert
            resultado.Should().HaveCount(2);
            resultado.Should().ContainSingle(e => e is Estudiante);
            resultado.Should().ContainSingle(e => e is Docente);
        }

        [Test]
        public void ToModel_EntityNulo_DeberiaRetornarNull()
        {
            // Act
            var resultado = ((PersonaEntity?)null).ToModel();

            // Assert
            resultado.Should().BeNull();
        }

        [Test]
        public void ToDto_ConIsDeleted_DeberiaMantenerEstado()
        {
            // Arrange
            var estudianteEliminado = new Estudiante
            {
                Id = 1,
                Dni = "12345678H",
                Nombre = "Juan",
                Apellidos = "Pérez",
                FechaNacimiento = new DateTime(2000, 5, 15),
                Email = "juan@test.com",
                Calificacion = 8.5,
                Ciclo = Ciclo.DAM,
                Curso = Curso.Primero,
                IsDeleted = true,
                DeletedAt = new DateTime(2024, 6, 1)
            };

            // Act
            var resultado = estudianteEliminado.ToDto();

            // Assert
            resultado.IsDeleted.Should().BeTrue();
            resultado.DeletedAt.Should().Be("2024-06-01T00:00:00");
        }
    }

    [TestFixture]
    public class CasosNegativos
    {
        [Test]
        public void ToModel_DtoEstudiante_ConDatosInvalidos_DeberiaUsarValoresPorDefecto()
        {
            // Arrange - Calificación y Enums inválidos
            var dto = new PersonaDto(
                1, "12345678H", "Juan", "Pérez",
                "2000-05-15", "juan@test.com", null,
                "Estudiante", null, null, "CICLO_INVALIDO", "CURSO_INVALIDO", "CALIF_INVALIDA",
                "2024-01-01T10:00:00", "2024-01-02T12:00:00", false, null);

            // Act
            var resultado = dto.ToModel() as Estudiante;

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Calificacion.Should().Be(0.0);
            resultado.Ciclo.Should().Be(Ciclo.DAW); // Valor por defecto en el mapper
            resultado.Curso.Should().Be(Curso.Primero); // Valor por defecto en el mapper
        }

        [Test]
        public void ToModel_DtoDocente_ConExperienciaInvalida_DeberiaUsarCero()
        {
            // Arrange - Experiencia inválida
            var dto = new PersonaDto(
                2, "87654321Z", "María", "García",
                "1985-03-20", "maria@test.com", null,
                "Docente", "EXPERIENCIA_INVALIDA", Modulos.Programacion, "DAW", null, null,
                "2024-01-01T10:00:00", "2024-01-02T12:00:00", false, null);

            // Act
            var resultado = dto.ToModel() as Docente;

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Experiencia.Should().Be(0);
        }

        [Test]
        public void ToModel_DtoDocente_ConEspecialidadNula_DeberiaUsarStringVacio()
        {
            // Arrange - Especialidad nula
            var dto = new PersonaDto(
                2, "87654321Z", "María", "García",
                "1985-03-20", "maria@test.com", null,
                "Docente", "10", null, "DAW", null, null,
                "2024-01-01T10:00:00", "2024-01-02T12:00:00", false, null);

            // Act
            var resultado = dto.ToModel() as Docente;

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Especialidad.Should().Be(string.Empty);
        }

        [Test]
        public void ToModel_DtoTipoInvalido_DeberiaLanzarExcepcion()
        {
            // Arrange - usar fecha en formato ISO para evitar problemas de parsing
            var dtoInvalido = new PersonaDto(
                1, "12345678H", "Juan", "Pérez",
                "2000-05-15", "juan@test.com", null,
                "TipoInvalido", null, null, "DAM", null, null,
                "2024-01-01T10:00:00", "2024-01-02T12:00:00", false, null);

            // Act
            var action = () => dtoInvalido.ToModel();

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void ToModel_EntityTipoInvalido_DeberiaLanzarExcepcion()
        {
            // Arrange
            var entityInvalido = new PersonaEntity
            {
                Id = 1,
                Dni = "12345678H",
                Nombre = "Juan",
                Apellidos = "Pérez",
                FechaNacimiento = new DateTime(2000, 5, 15),
                Email = "juan@test.com",
                Tipo = "TipoInvalido"
            };

            // Act
            var action = () => entityInvalido.ToModel();

            // Assert
            action.Should().Throw<ArgumentException>();
        }
    }
}
