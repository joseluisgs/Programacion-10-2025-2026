using FluentAssertions;
using GestionAcademica.Models.Academia;
using GestionAcademica.ViewModels.Forms;
using NUnit.Framework;

namespace GestionAcademica.Test.ViewModels.Forms;

/// <summary>
/// Tests para la validación de DocenteFormData (IDataErrorInfo).
/// </summary>
[TestFixture]
public class DocenteFormDataTests
{
    private DocenteFormData ConstruirFormDataValido() => new()
    {
        Nombre        = "Ana",
        Apellidos     = "García López",
        Dni           = "87654321X",
        Email         = "ana@test.com",
        Experiencia   = 10,
        Especialidad  = "Programación",
        Ciclo         = Ciclo.DAW,
        FechaNacimiento = DateTime.Today.AddYears(-35)
    };

    // ===================================================================
    // IsValid – casos positivos
    // ===================================================================

    [TestFixture]
    public class IsValidCasosPositivos : DocenteFormDataTests
    {
        [Test]
        public void IsValid_ConDatosCompletos_DeberiaRetornarTrue()
        {
            // Arrange
            var formData = ConstruirFormDataValido();

            // Act
            var result = formData.IsValid();

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValid_ConExperienciaCero_DeberiaRetornarTrue()
        {
            var formData = ConstruirFormDataValido();
            formData.Experiencia = 0;
            formData.IsValid().Should().BeTrue();
        }

        [Test]
        public void IsValid_ConExperienciaMaxima_DeberiaRetornarTrue()
        {
            var formData = ConstruirFormDataValido();
            formData.Experiencia = 50;
            formData.IsValid().Should().BeTrue();
        }
    }

    // ===================================================================
    // IDataErrorInfo – validación de Nombre
    // ===================================================================

    [TestFixture]
    public class ValidacionNombreTests : DocenteFormDataTests
    {
        [Test]
        public void Nombre_Vacio_DeberiaRetornarError()
        {
            var formData = new DocenteFormData { Nombre = "" };
            formData["Nombre"].Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Nombre_UnCaracter_DeberiaRetornarError()
        {
            var formData = new DocenteFormData { Nombre = "A" };
            formData["Nombre"].Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Nombre_DosCaracteres_DeberiaSerValido()
        {
            var formData = new DocenteFormData { Nombre = "Al" };
            formData["Nombre"].Should().BeNullOrEmpty();
        }

        [Test]
        public void Nombre_TreintaYUnCaracteres_DeberiaRetornarError()
        {
            var formData = new DocenteFormData { Nombre = new string('A', 31) };
            formData["Nombre"].Should().NotBeNullOrEmpty();
        }
    }

    // ===================================================================
    // IDataErrorInfo – validación de Apellidos
    // ===================================================================

    [TestFixture]
    public class ValidacionApellidosTests : DocenteFormDataTests
    {
        [Test]
        public void Apellidos_Vacio_DeberiaRetornarError()
        {
            var formData = new DocenteFormData { Apellidos = "" };
            formData["Apellidos"].Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Apellidos_UnCaracter_DeberiaRetornarError()
        {
            var formData = new DocenteFormData { Apellidos = "A" };
            formData["Apellidos"].Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Apellidos_CincuentaYUnCaracteres_DeberiaRetornarError()
        {
            var formData = new DocenteFormData { Apellidos = new string('A', 51) };
            formData["Apellidos"].Should().NotBeNullOrEmpty();
        }
    }

    // ===================================================================
    // IDataErrorInfo – validación de DNI
    // ===================================================================

    [TestFixture]
    public class ValidacionDniTests : DocenteFormDataTests
    {
        [Test]
        public void Dni_Vacio_DeberiaRetornarError()
        {
            var formData = new DocenteFormData { Dni = "" };
            formData["Dni"].Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Dni_SoloNumeros_DeberiaRetornarError()
        {
            var formData = new DocenteFormData { Dni = "12345678" };
            formData["Dni"].Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Dni_FormatoValido_DeberiaRetornarVacio()
        {
            var formData = new DocenteFormData { Dni = "12345678Z" };
            formData["Dni"].Should().BeNullOrEmpty();
        }
    }

    // ===================================================================
    // IDataErrorInfo – validación de Email
    // ===================================================================

    [TestFixture]
    public class ValidacionEmailTests : DocenteFormDataTests
    {
        [Test]
        public void Email_Vacio_DeberiaRetornarError()
        {
            var formData = new DocenteFormData { Email = "" };
            formData["Email"].Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Email_SinArroba_DeberiaRetornarError()
        {
            var formData = new DocenteFormData { Email = "test.com" };
            formData["Email"].Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Email_FormatoValido_DeberiaRetornarVacio()
        {
            var formData = new DocenteFormData { Email = "test@test.com" };
            formData["Email"].Should().BeNullOrEmpty();
        }
    }

    // ===================================================================
    // IDataErrorInfo – validación de Fecha de Nacimiento
    // ===================================================================

    [TestFixture]
    public class ValidacionFechaNacimientoTests : DocenteFormDataTests
    {
        [Test]
        public void FechaNacimiento_Futura_DeberiaRetornarError()
        {
            var formData = new DocenteFormData { FechaNacimiento = DateTime.Now.AddDays(1) };
            formData["FechaNacimiento"].Should().NotBeNullOrEmpty();
        }

        [Test]
        public void FechaNacimiento_Anterior1900_DeberiaRetornarError()
        {
            var formData = new DocenteFormData { FechaNacimiento = new DateTime(1899, 1, 1) };
            formData["FechaNacimiento"].Should().NotBeNullOrEmpty();
        }

        [Test]
        public void FechaNacimiento_Hoy_DeberiaRetornarVacio()
        {
            var formData = new DocenteFormData { FechaNacimiento = DateTime.Today };
            formData["FechaNacimiento"].Should().BeNullOrEmpty();
        }
    }

    // ===================================================================
    // IDataErrorInfo – validación de Experiencia
    // ===================================================================

    [TestFixture]
    public class ValidacionExperienciaTests : DocenteFormDataTests
    {
        [Test]
        public void Experiencia_Negativa_DeberiaRetornarError()
        {
            var formData = new DocenteFormData { Experiencia = -1 };
            formData["Experiencia"].Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Experiencia_Mayor50_DeberiaRetornarError()
        {
            var formData = new DocenteFormData { Experiencia = 51 };
            formData["Experiencia"].Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Experiencia_Valida_DeberiaRetornarVacio()
        {
            var formData = new DocenteFormData { Experiencia = 10 };
            formData["Experiencia"].Should().BeNullOrEmpty();
        }
    }

    // ===================================================================
    // IDataErrorInfo – validación de Especialidad
    // ===================================================================

    [TestFixture]
    public class ValidacionEspecialidadTests : DocenteFormDataTests
    {
        [Test]
        public void Especialidad_Vacia_DeberiaRetornarError()
        {
            var formData = new DocenteFormData { Especialidad = "" };
            formData["Especialidad"].Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Especialidad_Nula_DeberiaRetornarError()
        {
            var formData = new DocenteFormData { Especialidad = null! };
            formData["Especialidad"].Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Especialidad_Valida_DeberiaRetornarVacio()
        {
            var formData = new DocenteFormData { Especialidad = "Programación" };
            formData["Especialidad"].Should().BeNullOrEmpty();
        }
    }

    // ===================================================================
    // GetValidationErrors
    // ===================================================================

    [TestFixture]
    public class GetValidationErrorsTests : DocenteFormDataTests
    {
        [Test]
        public void GetValidationErrors_ConErrores_DeberiaRetornarTodosLosErrores()
        {
            // Arrange
            var formData = new DocenteFormData();

            // Act
            var errores = formData.GetValidationErrors();

            // Assert
            errores.Should().Contain("Nombre");
            errores.Should().Contain("Apellidos");
            errores.Should().Contain("DNI");
            errores.Should().Contain("Email");
            errores.Should().Contain("Especialidad");
        }

        [Test]
        public void GetValidationErrors_SinErrores_DeberiaRetornarVacio()
        {
            // Arrange
            var formData = ConstruirFormDataValido();

            // Act
            var errores = formData.GetValidationErrors();

            // Assert
            errores.Should().BeEmpty();
        }
    }
}
