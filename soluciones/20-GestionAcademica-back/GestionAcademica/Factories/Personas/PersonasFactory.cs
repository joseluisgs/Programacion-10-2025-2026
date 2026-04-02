using GestionAcademica.Models;
using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Personas;

namespace GestionAcademica.Factories.Personas;

/// <summary>
///     Factoría con datos semilla fijos para registros inmutables de Estudiantes y Docentes.
/// </summary>
public static class PersonasFactory {
    /// <summary>
    ///     Genera la semilla de datos inicial (10 Estudiantes y 5 Docentes).
    /// </summary>
    /// <returns>Enumerable con datos de demostración</returns>
    public static IEnumerable<Persona> Seed() {
        var lista = new List<Persona>();

        lista.Add(new Estudiante {
            Dni = "11111111H", Nombre = "Ana", Apellidos = "López", Email = "ana.lopez@gestionacademica.local", Calificacion = 8.5, Ciclo = Ciclo.DAM,
            Curso = Curso.Primero
        });
        lista.Add(new Estudiante {
            Dni = "22222222J", Nombre = "Pedro", Apellidos = "Ruiz", Email = "pedro.ruiz@gestionacademica.local", Calificacion = 4.2, Ciclo = Ciclo.DAM,
            Curso = Curso.Primero
        });
        lista.Add(new Estudiante {
            Dni = "33333333P", Nombre = "María", Apellidos = "García", Email = "maria.garcia@gestionacademica.local", Calificacion = 9.0, Ciclo = Ciclo.DAM,
            Curso = Curso.Segundo
        });
        lista.Add(new Estudiante {
            Dni = "44444444A", Nombre = "Juan", Apellidos = "Pérez", Email = "juan.perez@gestionacademica.local", Calificacion = 6.5, Ciclo = Ciclo.DAM,
            Curso = Curso.Segundo
        });
        lista.Add(new Estudiante {
            Dni = "55555555K", Nombre = "Elena", Apellidos = "Gómez", Email = "elena.gomez@gestionacademica.local", Calificacion = 7.8, Ciclo = Ciclo.DAW,
            Curso = Curso.Primero
        });
        lista.Add(new Estudiante {
            Dni = "66666666Q", Nombre = "Luis", Apellidos = "Moreno", Email = "luis.moreno@gestionacademica.local", Calificacion = 3.5, Ciclo = Ciclo.DAW,
            Curso = Curso.Primero
        });
        lista.Add(new Estudiante {
            Dni = "77777777V", Nombre = "Sonia", Apellidos = "Ruiz", Email = "sonia.ruiz@gestionacademica.local", Calificacion = 5.0, Ciclo = Ciclo.DAW,
            Curso = Curso.Segundo
        });
        lista.Add(new Estudiante {
            Dni = "88888888E", Nombre = "Jorge", Apellidos = "Sánchez", Email = "jorge.sanchez@gestionacademica.local", Calificacion = 2.0, Ciclo = Ciclo.ASIR,
            Curso = Curso.Primero
        });
        lista.Add(new Estudiante {
            Dni = "99999999X", Nombre = "Laura", Apellidos = "Fernández", Email = "laura.fernandez@gestionacademica.local", Calificacion = 10.0, Ciclo = Ciclo.ASIR,
            Curso = Curso.Segundo
        });
        lista.Add(new Estudiante {
            Dni = "00000000M", Nombre = "Carlos", Apellidos = "Jiménez", Email = "carlos.jimenez@gestionacademica.local", Calificacion = 5.5, Ciclo = Ciclo.ASIR,
            Curso = Curso.Segundo
        });

        lista.Add(new Docente {
            Dni = "12345678Z", Nombre = "Jose Luis", Apellidos = "González", Email = "jose.gonzalez@gestionacademica.local", Experiencia = 15,
            Especialidad = Modulos.Programacion, Ciclo = Ciclo.DAW
        });
        lista.Add(new Docente {
            Dni = "23456789D", Nombre = "Beatriz", Apellidos = "Sánchez", Email = "beatriz.sanchez@gestionacademica.local", Experiencia = 10,
            Especialidad = Modulos.BasesDatos, Ciclo = Ciclo.DAW
        });
        lista.Add(new Docente {
            Dni = "34567890V", Nombre = "Carlos", Apellidos = "Pérez", Email = "carlos.perez@gestionacademica.local", Experiencia = 8, Especialidad = Modulos.Entornos,
            Ciclo = Ciclo.DAM
        });
        lista.Add(new Docente {
            Dni = "45678901H", Nombre = "Marta", Apellidos = "García", Email = "marta.garcia@gestionacademica.local", Experiencia = 12,
            Especialidad = Modulos.LenguajesMarcas, Ciclo = Ciclo.DAW
        });
        lista.Add(new Docente {
            Dni = "56789012L", Nombre = "Raúl", Apellidos = "Martínez", Email = "raul.martinez@gestionacademica.local", Experiencia = 5,
            Especialidad = Modulos.Programacion, Ciclo = Ciclo.ASIR
        });

        return lista;
    }
}