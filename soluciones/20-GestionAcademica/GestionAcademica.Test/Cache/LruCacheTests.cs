using GestionAcademica.Cache;
using NUnit.Framework;
using FluentAssertions;

namespace GestionAcademica.Test.Cache;

[TestFixture]
public class LruCacheTests
{
    [TestFixture]
    public class CasosPositivos
    {
        private LruCache<int, string> _cache = null!;

        [SetUp]
        public void SetUp()
        {
            _cache = new LruCache<int, string>(3);
        }

        [Test]
        public void Add_ConElementoValido_DeberiaGuardarElemento()
        {
            // Arrange (ya hecho en SetUp)

            // Act
            _cache.Add(1, "uno");

            // Assert
            _cache.Get(1).Should().Be("uno");
        }

        [Test]
        public void Add_MultiplesElementos_DeberiaGuardarTodos()
        {
            // Arrange (ya hecho en SetUp)

            // Act
            _cache.Add(1, "uno");
            _cache.Add(2, "dos");
            _cache.Add(3, "tres");

            // Assert
            _cache.Get(1).Should().Be("uno");
            _cache.Get(2).Should().Be("dos");
            _cache.Get(3).Should().Be("tres");
        }

        [Test]
        public void Add_CuandoSuperaCapacidad_DeberiaEliminarMasAntiguo()
        {
            // Arrange (ya hecho en SetUp)

            // Act
            _cache.Add(1, "uno");
            _cache.Add(2, "dos");
            _cache.Add(3, "tres");
            _cache.Add(4, "cuatro");

            // Assert
            _cache.Get(1).Should().BeNull();
            _cache.Get(2).Should().Be("dos");
            _cache.Get(3).Should().Be("tres");
            _cache.Get(4).Should().Be("cuatro");
        }

        [Test]
        public void Get_CuandoExiste_DeberiaActualizarOrdenLRU()
        {
            // Arrange (ya hecho en SetUp)

            // Act
            _cache.Add(1, "uno");
            _cache.Add(2, "dos");
            _cache.Get(1);
            _cache.Add(3, "tres");

            // Assert
            _cache.Get(2).Should().Be("dos");
            _cache.Get(1).Should().Be("uno");
        }

        [Test]
        public void Remove_CuandoExiste_DeberiaEliminarElemento()
        {
            // Arrange (ya hecho en SetUp)

            // Act
            _cache.Add(1, "uno");
            _cache.Add(2, "dos");
            var resultado = _cache.Remove(1);

            // Assert
            resultado.Should().BeTrue();
            _cache.Get(1).Should().BeNull();
            _cache.Get(2).Should().Be("dos");
        }

        [Test]
        public void Remove_CuandoNoExiste_DeberiaDevolverFalse()
        {
            // Arrange (ya hecho en SetUp)

            // Act
            var resultado = _cache.Remove(999);

            // Assert
            resultado.Should().BeFalse();
        }

        [Test]
        public void Get_CuandoNoExiste_DeberiaDevolverNull()
        {
            // Arrange (ya hecho en SetUp)

            // Act
            var resultado = _cache.Get(1);

            // Assert
            resultado.Should().BeNull();
        }

        [Test]
        public void Add_ConClaveExistente_DeberiaReemplazarValor()
        {
            // Arrange (ya hecho en SetUp)

            // Act
            _cache.Add(1, "uno");
            _cache.Add(1, "UNO");

            // Assert
            _cache.Get(1).Should().Be("UNO");
        }
    }

    [TestFixture]
    public class CasosNegativos
    {
        private LruCache<int, string?> _cache = null!;

        [SetUp]
        public void SetUp()
        {
            _cache = new LruCache<int, string?>(3);
        }

        [Test]
        public void Add_ConValorNulo_DeberiaGuardarNull()
        {
            // Arrange (ya hecho en SetUp)

            // Act
            _cache.Add(1, null);

            // Assert
            _cache.Get(1).Should().BeNull();
        }

        [Test]
        public void Constructor_ConCapacidadCero_DeberiaLanzarExcepcion()
        {
            // Arrange & Act
            var action = () => new LruCache<int, string>(0);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Constructor_ConCapacidadNegativa_DeberiaLanzarExcepcion()
        {
            // Arrange & Act
            var action = () => new LruCache<int, string>(-1);

            // Assert
            action.Should().Throw<ArgumentException>();
        }
    }
}
