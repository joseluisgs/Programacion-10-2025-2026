using FluentAssertions;
using GestionAcademica.Back.Models.Academia;
using GestionAcademica.Entity;
using GestionAcademica.Errors.Personas;
using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Personas;
using GestionAcademica.Repositories.Personas.EfCore;
using Microsoft.Data.Sqlite;

namespace GestionAcademica.Test.Repositories.Personas.EfCore;

/// <summary>
/// Tests para PersonasEfRepository.
/// Configuración de tests: Se utiliza SQLite en memoria (:memory:) para cada clase de test.
/// La conexión se mantiene abierta durante toda la clase para compartir la BD en memoria.
/// 
/// Parámetros de configuración:
/// - dropData: true  → Borra cualquier dato existente al inicio (BD limpia)
/// - seedData: false → No carga datos de semilla (datos de prueba)
/// 
/// Esta configuración es ideal para tests porque:
/// 1. SQLite real en memoria (más realista que InMemory)
/// 2. Soporta foreign keys, transacciones, SQL real
/// 3. Cada clase de test tiene su propia BD independiente
/// 4. Los IDs siempre empiezan desde 1 en cada test
/// </summary>

[TestFixture]
public class PersonasEfRepositoryTests {
    [TestFixture]
    public class CasosPositivos {
        private string _dbFile = null!;

        [SetUp]
        public void SetUp() {
            _dbFile = Path.GetTempFileName();
            _repository = new PersonasEfRepository($"Data Source={_dbFile}", dropData: true, seedData: false);
        }

        [TearDown]
        public void TearDown() {
            SqliteConnection.ClearAllPools();
            if (File.Exists(_dbFile)) File.Delete(_dbFile);
        }

        private PersonasEfRepository _repository = null!;

        [Test]
        public async Task Create_EstudianteValido_DeberiaCrearCorrectamente() {
            // Arrange
            var estudiante = new Estudiante {
                Dni = "12345678H",
                Nombre = "Juan",
                Apellidos = "Pérez",
                Email = "juan@test.com",
                Calificacion = 8.5,
                Ciclo = Ciclo.DAM,
                Curso = Curso.Primero
            };

            // Act
            var resultado = await _repository.CreateAsync(estudiante);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Id.Should().Be(1);
        }

        [Test]
        public async Task Create_DocenteValido_DeberiaCrearCorrectamente() {
            // Arrange
            var docente = new Docente {
                Dni = "87654321Z",
                Nombre = "María",
                Apellidos = "García",
                Email = "maria@test.com",
                Experiencia = 10,
                Especialidad = Modulos.Programacion,
                Ciclo = Ciclo.DAW
            };

            // Act
            var resultado = await _repository.CreateAsync(docente);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Id.Should().Be(1);
        }

        [Test]
        public async Task GetById_CuandoExiste_DeberiaRetornarPersona() {
            // Arrange
            var estudiante = new Estudiante {
                Dni = "12345678H", Nombre = "Juan", Apellidos = "Pérez", Email = "juan@test.com", Calificacion = 8.5,
                Ciclo = Ciclo.DAM, Curso = Curso.Primero
            };
            await _repository.CreateAsync(estudiante);

            // Act
            var resultado = await _repository.GetByIdAsync(1);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Id.Should().Be(1);
        }

        [Test]
        public async Task GetByDni_CuandoExiste_DeberiaRetornarPersona() {
            // Arrange
            var estudiante = new Estudiante {
                Dni = "12345678H", Nombre = "Juan", Apellidos = "Pérez", Email = "juan@test.com", Calificacion = 8.5,
                Ciclo = Ciclo.DAM, Curso = Curso.Primero
            };
            await _repository.CreateAsync(estudiante);

            // Act
            var resultado = await _repository.GetByDniAsync("12345678H");

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Dni.Should().Be("12345678H");
        }

        [Test]
        public async Task GetByEmail_CuandoExiste_DeberiaRetornarPersona() {
            // Arrange
            var estudiante = new Estudiante {
                Dni = "12345678H", Nombre = "Juan", Apellidos = "Pérez", Email = "juan@test.com", Calificacion = 8.5,
                Ciclo = Ciclo.DAM, Curso = Curso.Primero
            };
            await _repository.CreateAsync(estudiante);

            // Act
            var resultado = await _repository.GetByEmailAsync("juan@test.com");

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Email.Should().Be("juan@test.com");
        }

        [Test]
        public async Task ExisteDni_CuandoExiste_DeberiaRetornarTrue() {
            // Arrange
            var estudiante = new Estudiante {
                Dni = "12345678H", Nombre = "Juan", Apellidos = "Pérez", Email = "juan@test.com", Calificacion = 8.5,
                Ciclo = Ciclo.DAM, Curso = Curso.Primero
            };
            await _repository.CreateAsync(estudiante);

            // Act
            var resultado = await _repository.ExisteDniAsync("12345678H");

            // Assert
            resultado.Should().BeTrue();
        }

        [Test]
        public async Task ExisteEmail_CuandoExiste_DeberiaRetornarTrue() {
            // Arrange
            var estudiante = new Estudiante {
                Dni = "12345678H", Nombre = "Juan", Apellidos = "Pérez", Email = "juan@test.com", Calificacion = 8.5,
                Ciclo = Ciclo.DAM, Curso = Curso.Primero
            };
            await _repository.CreateAsync(estudiante);

            // Act
            var resultado = await _repository.ExisteEmailAsync("juan@test.com");

            // Assert
            resultado.Should().BeTrue();
        }

        [Test]
        public async Task GetAll_SinParametros_DeberiaRetornarTodos() {
            // Arrange
            await _repository.CreateAsync(new Estudiante {
                Dni = "11111111H", Nombre = "Ana", Apellidos = "López", Email = "ana@test.com", Calificacion = 8.5,
                Ciclo = Ciclo.DAM, Curso = Curso.Primero
            });
            await _repository.CreateAsync(new Estudiante {
                Dni = "22222222J", Nombre = "Pedro", Apellidos = "Ruiz", Email = "pedro@test.com", Calificacion = 5.0,
                Ciclo = Ciclo.DAW, Curso = Curso.Segundo
            });

            // Act
            var resultado = await _repository.GetAllAsync();

            // Assert
            resultado.Should().HaveCount(2);
        }

        [Test]
        public async Task GetAll_ConPaginacion_DeberiaRetornarPagina() {
            // Arrange
            for (var i = 1; i <= 5; i++)
                await _repository.CreateAsync(new Estudiante {
                    Dni = $"{i:D8}H", Nombre = $"Nombre{i}", Apellidos = "Apellido", Email = $"test{i}@test.com",
                    Calificacion = 5.0, Ciclo = Ciclo.DAM, Curso = Curso.Primero
                });

            // Act
            var resultado = await _repository.GetAllAsync(1, 3);

            // Assert
            resultado.Should().HaveCount(3);
        }

        [Test]
        public async Task GetAll_SinIncluirBorrados_DeberiaRetornarSoloActivos() {
            // Arrange
            await _repository.CreateAsync(new Estudiante {
                Dni = "11111111H", Nombre = "Ana", Apellidos = "López", Email = "ana@test.com", Calificacion = 8.5,
                Ciclo = Ciclo.DAM, Curso = Curso.Primero
            });
            var p2 = (await _repository.CreateAsync(new Estudiante {
                Dni = "22222222J", Nombre = "Pedro", Apellidos = "Ruiz", Email = "pedro@test.com", Calificacion = 5.0,
                Ciclo = Ciclo.DAW, Curso = Curso.Segundo
            })).Value;
            await _repository.DeleteAsync(p2.Id);

            // Act
            var resultado = await _repository.GetAllAsync(includeDeleted: false);

            // Assert
            resultado.Should().HaveCount(1);
            resultado.First().Dni.Should().Be("11111111H");
        }

        [Test]
        public async Task Update_ConDatosValidos_DeberiaActualizar() {
            // Arrange
            var estudiante = new Estudiante {
                Dni = "12345678H", Nombre = "Juan", Apellidos = "Pérez", Email = "juan@test.com", Calificacion = 8.5,
                Ciclo = Ciclo.DAM, Curso = Curso.Primero
            };
            var created = await _repository.CreateAsync(estudiante);
            created.IsSuccess.Should().BeTrue();
            created.Value.Id.Should().Be(1);

            var existing = await _repository.GetByIdAsync(1);
            existing.Should().NotBeNull();
            existing!.Nombre.Should().Be("Juan");

            var actualizado = new Estudiante {
                Dni = "12345678H", Nombre = "Juan Updated", Apellidos = "Pérez Updated", Email = "juanupdated@test.com",
                Calificacion = 9.0, Ciclo = Ciclo.DAW, Curso = Curso.Segundo
            };

            // Act
            var resultado = await _repository.UpdateAsync(1, actualizado);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Nombre.Should().Be("Juan Updated");
        }

        [Test]
        public async Task Delete_Logico_CuandoExiste_DeberiaRetornarPersona() {
            // Arrange
            var estudiante = new Estudiante {
                Dni = "12345678H", Nombre = "Juan", Apellidos = "Pérez", Email = "juan@test.com", Calificacion = 8.5,
                Ciclo = Ciclo.DAM, Curso = Curso.Primero
            };
            await _repository.CreateAsync(estudiante);

            // Act
            var resultado = await _repository.DeleteAsync(1);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.IsDeleted.Should().BeTrue();
        }

        [Test]
        public async Task Delete_Fisico_CuandoExiste_DeberiaRetornarPersona() {
            // Arrange
            var estudiante = new Estudiante {
                Dni = "12345678H", Nombre = "Juan", Apellidos = "Pérez", Email = "juan@test.com", Calificacion = 8.5,
                Ciclo = Ciclo.DAM, Curso = Curso.Primero
            };
            await _repository.CreateAsync(estudiante);

            // Act
            var resultado = await _repository.DeleteAsync(1, false);

            // Assert
            resultado.Should().NotBeNull();
            (await _repository.GetByIdAsync(1)).Should().BeNull();
        }

        [Test]
        public async Task DeleteAll_CuandoHayDatos_DeberiaEliminarTodos() {
            // Arrange
            await _repository.CreateAsync(new Estudiante {
                Dni = "11111111H", Nombre = "Ana", Apellidos = "López", Email = "ana@test.com", Calificacion = 8.5,
                Ciclo = Ciclo.DAM, Curso = Curso.Primero
            });
            await _repository.CreateAsync(new Estudiante {
                Dni = "22222222J", Nombre = "Pedro", Apellidos = "Ruiz", Email = "pedro@test.com", Calificacion = 5.0,
                Ciclo = Ciclo.DAW, Curso = Curso.Segundo
            });

            // Act
            var resultado = await _repository.DeleteAllAsync();

            // Assert
            resultado.Should().BeTrue();
            (await _repository.GetAllAsync()).Should().BeEmpty();
        }
    }

    [TestFixture]
    public class CasosNegativos {
        private string _dbFile = null!;

        [SetUp]
        public void SetUp() {
            _dbFile = Path.GetTempFileName();
            _repository = new PersonasEfRepository($"Data Source={_dbFile}", dropData: true, seedData: false);
        }

        [TearDown]
        public void TearDown() {
            SqliteConnection.ClearAllPools();
            if (File.Exists(_dbFile)) File.Delete(_dbFile);
        }

        private PersonasEfRepository _repository = null!;

        [Test]
        public async Task Create_ConDniExistente_DeberiaRetornarFailure() {
            // Arrange
            var estudiante1 = new Estudiante {
                Dni = "12345678H", Nombre = "Juan", Apellidos = "Pérez", Email = "juan1@test.com", Calificacion = 8.5,
                Ciclo = Ciclo.DAM, Curso = Curso.Primero
            };
            var estudiante2 = new Estudiante {
                Dni = "12345678H", Nombre = "Pedro", Apellidos = "García", Email = "juan2@test.com", Calificacion = 5.0,
                Ciclo = Ciclo.DAW, Curso = Curso.Segundo
            };
            await _repository.CreateAsync(estudiante1);

            // Act
            var resultado = await _repository.CreateAsync(estudiante2);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.DniAlreadyExists>();
            (resultado.Error as PersonaError.DniAlreadyExists)?.Dni.Should().Be("12345678H");
            resultado.Error.Message.Should().Contain("12345678H");
        }

        [Test]
        public async Task Create_ConEmailExistente_DeberiaRetornarFailure() {
            // Arrange
            var estudiante1 = new Estudiante {
                Dni = "11111111H", Nombre = "Juan", Apellidos = "Pérez", Email = "juan@test.com", Calificacion = 8.5,
                Ciclo = Ciclo.DAM, Curso = Curso.Primero
            };
            var estudiante2 = new Estudiante {
                Dni = "22222222J", Nombre = "Pedro", Apellidos = "García", Email = "juan@test.com", Calificacion = 5.0,
                Ciclo = Ciclo.DAW, Curso = Curso.Segundo
            };
            await _repository.CreateAsync(estudiante1);

            // Act
            var resultado = await _repository.CreateAsync(estudiante2);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.EmailAlreadyExists>();
            (resultado.Error as PersonaError.EmailAlreadyExists)?.Email.Should().Be("juan@test.com");
            resultado.Error.Message.Should().Contain("juan@test.com");
        }

        [Test]
        public async Task GetById_CuandoNoExiste_DeberiaRetornarNull() {
            // Act
            var resultado = await _repository.GetByIdAsync(999);

            // Assert
            resultado.Should().BeNull();
        }

        [Test]
        public async Task GetByDni_CuandoNoExiste_DeberiaRetornarNull() {
            // Act
            var resultado = await _repository.GetByDniAsync("99999999Z");

            // Assert
            resultado.Should().BeNull();
        }

        [Test]
        public async Task GetByEmail_CuandoNoExiste_DeberiaRetornarNull() {
            // Act
            var resultado = await _repository.GetByEmailAsync("noexiste@test.com");

            // Assert
            resultado.Should().BeNull();
        }

        [Test]
        public async Task ExisteDni_CuandoNoExiste_DeberiaRetornarFalse() {
            // Act
            var resultado = await _repository.ExisteDniAsync("99999999Z");

            // Assert
            resultado.Should().BeFalse();
        }

        [Test]
        public async Task ExisteEmail_CuandoNoExiste_DeberiaRetornarFalse() {
            // Act
            var resultado = await _repository.ExisteEmailAsync("noexiste@test.com");

            // Assert
            resultado.Should().BeFalse();
        }

        [Test]
        public async Task Update_CuandoNoExiste_DeberiaRetornarFailure() {
            // Arrange
            var estudiante = new Estudiante {
                Dni = "12345678H", Nombre = "Juan", Apellidos = "Pérez", Email = "juan@test.com", Calificacion = 8.5,
                Ciclo = Ciclo.DAM, Curso = Curso.Primero
            };

            // Act
            var resultado = await _repository.UpdateAsync(999, estudiante);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.NotFound>();
            (resultado.Error as PersonaError.NotFound)?.Id.Should().Be("999");
            resultado.Error.Message.Should().Contain("999");
        }

        [Test]
        public async Task Update_ConDniExistenteEnOtro_DeberiaRetornarFailure() {
            // Arrange
            var estudiante1 = new Estudiante {
                Dni = "11111111H", Nombre = "Juan", Apellidos = "Pérez", Email = "juan1@test.com", Calificacion = 8.5,
                Ciclo = Ciclo.DAM, Curso = Curso.Primero
            };
            var estudiante2 = new Estudiante {
                Dni = "22222222J", Nombre = "Pedro", Apellidos = "García", Email = "juan2@test.com", Calificacion = 5.0,
                Ciclo = Ciclo.DAW, Curso = Curso.Segundo
            };
            await _repository.CreateAsync(estudiante1);
            await _repository.CreateAsync(estudiante2);

            // Act
            var resultado = await _repository.UpdateAsync(2, estudiante1);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.DniAlreadyExists>();
            (resultado.Error as PersonaError.DniAlreadyExists)?.Dni.Should().Be("11111111H");
        }

        [Test]
        public async Task Update_ConEmailExistenteEnOtro_DeberiaRetornarFailure() {
            // Arrange
            var estudiante1 = new Estudiante {
                Dni = "11111111H", Nombre = "Juan", Apellidos = "Pérez", Email = "juan@test.com", Calificacion = 8.5,
                Ciclo = Ciclo.DAM, Curso = Curso.Primero
            };
            var estudiante2 = new Estudiante {
                Dni = "22222222J", Nombre = "Pedro", Apellidos = "García", Email = "pedro@test.com", Calificacion = 5.0,
                Ciclo = Ciclo.DAW, Curso = Curso.Segundo
            };
            await _repository.CreateAsync(estudiante1);
            await _repository.CreateAsync(estudiante2);

            var actualizado = new Estudiante {
                Dni = "22222222J", Nombre = "Pedro", Apellidos = "García", Email = "juan@test.com", Calificacion = 5.0,
                Ciclo = Ciclo.DAW, Curso = Curso.Segundo
            };

            // Act
            var resultado = await _repository.UpdateAsync(2, actualizado);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.EmailAlreadyExists>();
            (resultado.Error as PersonaError.EmailAlreadyExists)?.Email.Should().Be("juan@test.com");
        }

        [Test]
        public async Task Delete_CuandoNoExiste_DeberiaRetornarNull() {
            // Act
            var resultado = await _repository.DeleteAsync(999);

            // Assert
            resultado.Should().BeNull();
        }

        [Test]
        public async Task Restore_CuandoNoExiste_DeberiaRetornarFailure() {
            // Act
            var resultado = await _repository.RestoreAsync(999);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<PersonaError.NotFound>();
        }
    }

    [TestFixture]
    public class CasosMixtos {
        private string _dbFile = null!;

        [SetUp]
        public void SetUp() {
            _dbFile = Path.GetTempFileName();
            _repository = new PersonasEfRepository($"Data Source={_dbFile}", dropData: true, seedData: false);
        }

        [TearDown]
        public void TearDown() {
            SqliteConnection.ClearAllPools();
            if (File.Exists(_dbFile)) File.Delete(_dbFile);
        }

        private PersonasEfRepository _repository = null!;

        [Test]
        public async Task Restore_CuandoEliminadoLogicamente_DeberiaRestaurar() {
            // Arrange
            var estudiante = new Estudiante {
                Dni = "12345678H",
                Nombre = "Juan",
                Apellidos = "Pérez",
                Email = "juan@test.com",
                Calificacion = 8.5,
                Ciclo = Ciclo.DAM,
                Curso = Curso.Primero
            };
            var creada = (await _repository.CreateAsync(estudiante)).Value;
            await _repository.DeleteAsync(creada.Id);

            // Act
            var resultado = await _repository.RestoreAsync(creada.Id);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.IsDeleted.Should().BeFalse();
            resultado.Value.DeletedAt.Should().BeNull();
        }

        [Test]
        public async Task CountEstudiantes_SinEliminados_DeberiaContarSoloActivos() {
            // Arrange
            await _repository.CreateAsync(new Estudiante {
                Dni = "11111111H", Nombre = "Ana", Apellidos = "López", Email = "ana@test.com", Calificacion = 8.5,
                Ciclo = Ciclo.DAM, Curso = Curso.Primero
            });
            var p2 = (await _repository.CreateAsync(new Estudiante {
                Dni = "22222222J", Nombre = "Pedro", Apellidos = "Ruiz", Email = "pedro@test.com", Calificacion = 5.0,
                Ciclo = Ciclo.DAW, Curso = Curso.Segundo
            })).Value;
            await _repository.CreateAsync(new Docente {
                Dni = "33333333P", Nombre = "Ana", Apellidos = "García", Email = "maria@test.com", Experiencia = 10,
                Especialidad = Modulos.Programacion, Ciclo = Ciclo.DAW
            });
            await _repository.DeleteAsync(p2.Id);

            // Act
            var resultado = await _repository.CountEstudiantesAsync();

            // Assert
            resultado.Should().Be(1);
        }

        [Test]
        public async Task CountEstudiantes_IncluyendoEliminados_DeberiaContarTodos() {
            // Arrange
            await _repository.CreateAsync(new Estudiante {
                Dni = "11111111H", Nombre = "Ana", Apellidos = "López", Email = "ana@test.com", Calificacion = 8.5,
                Ciclo = Ciclo.DAM, Curso = Curso.Primero
            });
            var p2 = (await _repository.CreateAsync(new Estudiante {
                Dni = "22222222J", Nombre = "Pedro", Apellidos = "Ruiz", Email = "pedro@test.com", Calificacion = 5.0,
                Ciclo = Ciclo.DAW, Curso = Curso.Segundo
            })).Value;
            await _repository.DeleteAsync(p2.Id);

            // Act
            var resultado = await _repository.CountEstudiantesAsync(true);

            // Assert
            resultado.Should().Be(2);
        }

        [Test]
        public async Task CountDocentes_SinEliminados_DeberiaContarSoloActivos() {
            // Arrange
            await _repository.CreateAsync(new Docente {
                Dni = "33333333P", Nombre = "Ana", Apellidos = "García", Email = "ana@test.com", Experiencia = 10,
                Especialidad = Modulos.Programacion, Ciclo = Ciclo.DAW
            });
            var p2 = (await _repository.CreateAsync(new Docente {
                Dni = "44444444A", Nombre = "Pedro", Apellidos = "Ruiz", Email = "pedro@test.com", Experiencia = 5,
                Especialidad = Modulos.BasesDatos, Ciclo = Ciclo.DAM
            })).Value;
            await _repository.DeleteAsync(p2.Id);

            // Act
            var resultado = await _repository.CountDocentesAsync();

            // Assert
            resultado.Should().Be(1);
        }

        [Test]
        public async Task CountDocentes_IncluyendoEliminados_DeberiaContarTodos() {
            // Arrange
            await _repository.CreateAsync(new Docente {
                Dni = "33333333P", Nombre = "Ana", Apellidos = "García", Email = "ana@test.com", Experiencia = 10,
                Especialidad = Modulos.Programacion, Ciclo = Ciclo.DAW
            });
            var p2 = (await _repository.CreateAsync(new Docente {
                Dni = "44444444A", Nombre = "Pedro", Apellidos = "Ruiz", Email = "pedro@test.com", Experiencia = 5,
                Especialidad = Modulos.BasesDatos, Ciclo = Ciclo.DAM
            })).Value;
            await _repository.DeleteAsync(p2.Id);

            // Act
            var resultado = await _repository.CountDocentesAsync(true);

            // Assert
            resultado.Should().Be(2);
        }

        [Test]
        public async Task GetEstudiantes_DeberiaRetornarSoloEstudiantes() {
            // Arrange
            await _repository.CreateAsync(new Estudiante {
                Dni = "11111111H", Nombre = "Ana", Apellidos = "López", Email = "ana@test.com", Calificacion = 8.5,
                Ciclo = Ciclo.DAM, Curso = Curso.Primero
            });
            await _repository.CreateAsync(new Docente {
                Dni = "33333333P", Nombre = "Ana", Apellidos = "García", Email = "maria@test.com", Experiencia = 10,
                Especialidad = Modulos.Programacion, Ciclo = Ciclo.DAW
            });

            // Act
            var resultado = await _repository.GetEstudiantesAsync(1, 10, false);

            // Assert
            resultado.Should().HaveCount(1);
            resultado.First().Should().BeOfType<Estudiante>();
        }

        [Test]
        public async Task GetDocentes_DeberiaRetornarSoloDocentes() {
            // Arrange
            await _repository.CreateAsync(new Estudiante {
                Dni = "11111111H", Nombre = "Ana", Apellidos = "López", Email = "ana@test.com", Calificacion = 8.5,
                Ciclo = Ciclo.DAM, Curso = Curso.Primero
            });
            await _repository.CreateAsync(new Docente {
                Dni = "33333333P", Nombre = "Ana", Apellidos = "García", Email = "maria@test.com", Experiencia = 10,
                Especialidad = Modulos.Programacion, Ciclo = Ciclo.DAW
            });

            // Act
            var resultado = await _repository.GetDocentesAsync(1, 10, false);

            // Assert
            resultado.Should().HaveCount(1);
            resultado.First().Should().BeOfType<Docente>();
        }

        [Test]
        public async Task DeleteAll_DeberiaVaciarRepositorio() {
            // Arrange
            await _repository.CreateAsync(new Estudiante {
                Dni = "11111111H", Nombre = "Ana", Apellidos = "López", Email = "ana@test.com", Calificacion = 8.5,
                Ciclo = Ciclo.DAM, Curso = Curso.Primero
            });
            await _repository.CreateAsync(new Docente {
                Dni = "33333333P", Nombre = "Ana", Apellidos = "García", Email = "maria@test.com", Experiencia = 10,
                Especialidad = Modulos.Programacion, Ciclo = Ciclo.DAW
            });

            // Act
            var resultado = await _repository.DeleteAllAsync();

            // Assert
            resultado.Should().BeTrue();
            (await _repository.GetAllAsync()).Should().BeEmpty();
        }

        [Test]
        public async Task GetEstudiantesOrderBy_Paginacion_DeberiaOrdenarPorDni() {
            // Arrange
            await _repository.CreateAsync(new Estudiante { Dni = "C", Nombre = "Carlos", Apellidos = "Gómez", Email = "carlos@test.com", Calificacion = 7.0, Ciclo = Ciclo.DAM, Curso = Curso.Primero });
            await _repository.CreateAsync(new Estudiante { Dni = "A", Nombre = "Ana", Apellidos = "López", Email = "ana@test.com", Calificacion = 9.0, Ciclo = Ciclo.DAW, Curso = Curso.Segundo });
            await _repository.CreateAsync(new Estudiante { Dni = "B", Nombre = "Belén", Apellidos = "Pérez", Email = "belen@test.com", Calificacion = 8.0, Ciclo = Ciclo.ASIR, Curso = Curso.Primero });

            // Act
            var resultado = (await _repository.GetEstudiantesOrderByAsync("dni", 1, 2, true)).ToList();

            // Assert
            resultado.Should().HaveCount(2);
            resultado[0].Dni.Should().Be("A");
            resultado[1].Dni.Should().Be("B");
        }

        [Test]
        public async Task GetDocentesOrderBy_Paginacion_DeberiaOrdenarPorExperiencia() {
            // Arrange
            await _repository.CreateAsync(new Docente { Dni = "C", Nombre = "Carlos", Apellidos = "Gómez", Email = "carlos@test.com", Experiencia = 5, Especialidad = "BD", Ciclo = Ciclo.DAM });
            await _repository.CreateAsync(new Docente { Dni = "A", Nombre = "Ana", Apellidos = "López", Email = "ana@test.com", Experiencia = 15, Especialidad = "PROG", Ciclo = Ciclo.DAW });
            await _repository.CreateAsync(new Docente { Dni = "B", Nombre = "Belén", Apellidos = "Pérez", Email = "belen@test.com", Experiencia = 10, Especialidad = "RED", Ciclo = Ciclo.ASIR });

            // Act
            var resultado = (await _repository.GetDocentesOrderByAsync("experiencia", 1, 10, true)).ToList();

            // Assert
            resultado.Should().HaveCount(3);
            resultado[0].Dni.Should().Be("A");
        }
    }
}
