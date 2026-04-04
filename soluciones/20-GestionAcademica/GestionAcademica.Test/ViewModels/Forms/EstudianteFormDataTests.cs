using FluentAssertions;
using GestionAcademica.Models.Academia;
using GestionAcademica.ViewModels.Forms;
using NUnit.Framework;

namespace GestionAcademica.Test.ViewModels.Forms;

/// <summary>
/// Tests para la validación de EstudianteFormData (IDataErrorInfo).
/// </summary>
[TestFixture]
public class EstudianteFormDataTests
{
    private EstudianteFormData ConstruirFormDataValido() => new()
    {
        Nombre        = "Juan",
        Apellidos     = "Pérez García",
        Dni           = "12345678Z",
        Email         = "juan@test.com",
        Calificacion  = 7.5,
        Ciclo         = Ciclo.DAW,
        Curso         = Curso.Primero,
        FechaNacimiento = DateTime.Today.AddYears(-20)
    };

    // ===================================================================
    // IsValid – casos positivos
    // ===================================================================

    [TestFixture]
    public class IsValidCasosPositivos : EstudianteFormDataTests
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
        public void IsValid_ConCalificacionCero_DeberiaRetornarTrue()
        {
            var formData = ConstruirFormDataValido();
            formData.Calificacion = 0;
            formData.IsValid().Should().BeTrue();
        }

        [Test]
        public void IsValid_ConCalificacionMaxima_DeberiaRetornarTrue()
        {
            var formData = ConstruirFormDataValido();
            formData.Calificacion = 10;
            formData.IsValid().Should().BeTrue();
        }
    }

    // ===================================================================
    // IDataErrorInfo – validación de Nombre
    // ===================================================================

    [TestFixture]
    public class ValidacionNombreTests : EstudianteFormDataTests
    {
        [Test]
        public void Nombre_Vacio_DeberiaRetornarError()
        {
            var formData = new EstudianteFormData { Nombre = "" };
            formData["Nombre"].Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Nombre_UnCaracter_DeberiaRetornarError()
        {
            var formData = new EstudianteFormData { Nombre = "A" };
            formData["Nombre"].Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Nombre_DosCaracteres_DeberiaSerValido()
        {
            var formData = new EstudianteFormData { Nombre = "Al" };
            formData["Nombre"].Should().BeNullOrEmpty();
        }

        [Test]
        public void Nombre_TreintaYUnCaracteres_DeberiaRetornarError()
        {
            var formData = new EstudianteFormData { Nombre = new string('A', 31) };
            formData["Nombre"].Should().NotBeNullOrEmpty();
        }
    }

    // ===================================================================
    // IDataErrorInfo – validación de Apellidos
    // ===================================================================

    [TestFixture]
    public class ValidacionApellidosTests : EstudianteFormDataTests
    {
        [Test]
        public void Apellidos_Vacio_DeberiaRetornarError()
        {
            var formData = new EstudianteFormData { Apellidos = "" };
            formData["Apellidos"].Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Apellidos_Valido_DeberiaRetornarVacio()
        {
            var formData = new EstudianteFormData { Apellidos = "García" };
            formData["Apellidos"].Should().BeNullOrEmpty();
        }
    }

    // ===================================================================
    // IDataErrorInfo – validación de DNI
    // ===================================================================

    [TestFixture]
    public class ValidacionDniTests : EstudianteFormDataTests
    {
        [Test]
        public void Dni_Vacio_DeberiaRetornarError()
        {
            var formData = new EstudianteFormData { Dni = "" };
            formData["Dni"].Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Dni_SoloNumeros_DeberiaRetornarError()
        {
            var formData = new EstudianteFormData { Dni = "12345678" };
            formData["Dni"].Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Dni_LetraMinuscula_DeberiaRetornarVacio()
        {
            // La validación acepta minúsculas porque las convierte a mayúsculas antes de validar
            var formData = new EstudianteFormData { Dni = "12345678z" };
            formData["Dni"].Should().BeNullOrEmpty();
        }

        [Test]
        public void Dni_FormatoValido_DeberiaRetornarVacio()
        {
            var formData = new EstudianteFormData { Dni = "12345678Z" };
            formData["Dni"].Should().BeNullOrEmpty();
        }

        [Test]
        public void Dni_OtroFormatoValido_DeberiaRetornarVacio()
        {
            var formData = new EstudianteFormData { Dni = "00000001R" };
            formData["Dni"].Should().BeNullOrEmpty();
        }
    }

    // ===================================================================
    // IDataErrorInfo – validación de Email
    // ===================================================================

    [TestFixture]
    public class ValidacionEmailTests : EstudianteFormDataTests
    {
        [Test]
        public void Email_Vacio_DeberiaRetornarError()
        {
            var formData = new EstudianteFormData { Email = "" };
            formData["Email"].Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Email_SinArroba_DeberiaRetornarError()
        {
            var formData = new EstudianteFormData { Email = "correo-sin-arroba.com" };
            formData["Email"].Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Email_Valido_DeberiaRetornarVacio()
        {
            var formData = new EstudianteFormData { Email = "usuario@dominio.com" };
            formData["Email"].Should().BeNullOrEmpty();
        }
    }

    // ===================================================================
    // IDataErrorInfo – validación de FechaNacimiento
    // ===================================================================

    [TestFixture]
    public class ValidacionFechaNacimientoTests : EstudianteFormDataTests
    {
        [Test]
        public void FechaNacimiento_Futura_DeberiaRetornarError()
        {
            var formData = new EstudianteFormData { FechaNacimiento = DateTime.Today.AddDays(1) };
            formData["FechaNacimiento"].Should().NotBeNullOrEmpty();
        }

        [Test]
        public void FechaNacimiento_AnteDe1900_DeberiaRetornarError()
        {
            var formData = new EstudianteFormData { FechaNacimiento = new DateTime(1899, 12, 31) };
            formData["FechaNacimiento"].Should().NotBeNullOrEmpty();
        }

        [Test]
        public void FechaNacimiento_Valida_DeberiaRetornarVacio()
        {
            var formData = new EstudianteFormData { FechaNacimiento = DateTime.Today.AddYears(-18) };
            formData["FechaNacimiento"].Should().BeNullOrEmpty();
        }
    }

    // ===================================================================
    // IDataErrorInfo – validación de Calificacion
    // ===================================================================

    [TestFixture]
    public class ValidacionCalificacionTests : EstudianteFormDataTests
    {
        [Test]
        public void Calificacion_Negativa_DeberiaRetornarError()
        {
            var formData = new EstudianteFormData { Calificacion = -0.1 };
            formData["Calificacion"].Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Calificacion_MayorQueDiez_DeberiaRetornarError()
        {
            var formData = new EstudianteFormData { Calificacion = 10.1 };
            formData["Calificacion"].Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Calificacion_EnRangoValido_DeberiaRetornarVacio()
        {
            var formData = new EstudianteFormData { Calificacion = 5.0 };
            formData["Calificacion"].Should().BeNullOrEmpty();
        }
    }

    // ===================================================================
    // IDataErrorInfo – campo desconocido
    // ===================================================================

    [Test]
    public void Indexer_CampoDesconocido_DeberiaRetornarNull()
    {
        var formData = ConstruirFormDataValido();
        formData["CampoQueNoExiste"].Should().BeNull();
    }

    // ===================================================================
    // Error property (IDataErrorInfo global)
    // ===================================================================

    [Test]
    public void Error_Siempre_DeberiaRetornarCadenaVacia()
    {
        var formData = new EstudianteFormData();
        formData.Error.Should().BeEmpty();
    }
}
