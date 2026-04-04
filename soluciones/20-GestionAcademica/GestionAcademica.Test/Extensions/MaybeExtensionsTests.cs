using CSharpFunctionalExtensions;
using FluentAssertions;
using GestionAcademica.Errors.Common;
using GestionAcademica.Errors.Personas;
using GestionAcademica.Extensions;
using EstudianteModel = GestionAcademica.Models.Personas.Estudiante;
using NUnit.Framework;

namespace GestionAcademica.Test.Extensions;

[TestFixture]
public class MaybeExtensionsTests
{
    [TestFixture]
    public class CasosPositivos
    {
        [Test]
        public void ToResult_ConValor_DeberiaRetornarSuccess()
        {
            // Arrange
            var persona = new EstudianteModel { Id = 1, Nombre = "Ana" };
            Maybe<EstudianteModel> maybe = persona;

            // Act
            var resultado = maybe.ToResult(new PersonaError.NotFound("1"));

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().Be(persona);
        }

        [Test]
        public void ToResult_ConErrorFactoryYValor_DeberiaRetornarSuccess()
        {
            // Arrange
            var persona = new EstudianteModel { Id = 1, Nombre = "Ana" };
            Maybe<EstudianteModel> maybe = persona;

            // Act
            var resultado = maybe.ToResult(() => new PersonaError.NotFound("1"));

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().Be(persona);
        }

        [Test]
        public void ToResult_SinValor_DeberiaRetornarFailure()
        {
            // Arrange
            Maybe<EstudianteModel> maybe = Maybe<EstudianteModel>.None;
            var error = new PersonaError.NotFound("99");

            // Act
            var resultado = maybe.ToResult(error);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().Be(error);
        }

        [Test]
        public void ToResult_SinValorYErrorFactory_DeberiaRetornarFailure()
        {
            // Arrange
            Maybe<EstudianteModel> maybe = Maybe<EstudianteModel>.None;

            // Act
            var resultado = maybe.ToResult(() => new PersonaError.NotFound("99"));

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.NotFound>();
        }
    }

    [TestFixture]
    public class CasosNegativos
    {
        [Test]
        public void ToResult_ConNulo_DeberiaRetornarFailure()
        {
            // Arrange
            Maybe<EstudianteModel> maybe = Maybe<EstudianteModel>.None;

            // Act
            var resultado = maybe.ToResult(new PersonaError.NotFound("0"));

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.NotFound>();
        }
    }
}
