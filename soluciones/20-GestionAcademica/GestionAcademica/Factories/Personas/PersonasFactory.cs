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

        // Estudiantes con fechas de nacimiento para tener distribución de edades
        lista.Add(new Estudiante {
            Dni = "11111111H", Nombre = "Ana", Apellidos = "López", 
            Email = "ana.lopez@gestionacademica.local", Calificacion = 8.5, 
            Ciclo = Ciclo.DAM, Curso = Curso.Primero,
            FechaNacimiento = new DateTime(2006, 5, 15)  // ~19 años
        });
        lista.Add(new Estudiante {
            Dni = "22222222P", Nombre = "Pedro", Apellidos = "Ruiz", 
            Email = "pedro.ruiz@gestionacademica.local", Calificacion = 4.2, 
            Ciclo = Ciclo.DAM, Curso = Curso.Primero,
            FechaNacimiento = new DateTime(2008, 3, 22)  // ~17 años - menor de 18
        });
        lista.Add(new Estudiante {
            Dni = "33333333P", Nombre = "María", Apellidos = "García", 
            Email = "maria.garcia@gestionacademica.local", Calificacion = 9.0, 
            Ciclo = Ciclo.DAM, Curso = Curso.Segundo,
            FechaNacimiento = new DateTime(2005, 8, 10)  // ~20 años
        });
        lista.Add(new Estudiante {
            Dni = "44444444A", Nombre = "Juan", Apellidos = "Pérez", 
            Email = "juan.perez@gestionacademica.local", Calificacion = 6.5, 
            Ciclo = Ciclo.DAM, Curso = Curso.Segundo,
            FechaNacimiento = new DateTime(2007, 11, 3)  // ~18 años
        });
        lista.Add(new Estudiante {
            Dni = "55555555K", Nombre = "Elena", Apellidos = "Gómez", 
            Email = "elena.gomez@gestionacademica.local", Calificacion = 7.8, 
            Ciclo = Ciclo.DAW, Curso = Curso.Primero,
            FechaNacimiento = new DateTime(2004, 2, 28)  // ~22 años
        });
        lista.Add(new Estudiante {
            Dni = "66666666V", Nombre = "Luis", Apellidos = "Moreno", 
            Email = "luis.moreno@gestionacademica.local", Calificacion = 3.5, 
            Ciclo = Ciclo.DAW, Curso = Curso.Primero,
            FechaNacimiento = new DateTime(2009, 7, 14)  // ~16 años - menor de 18
        });
        lista.Add(new Estudiante {
            Dni = "77777777J", Nombre = "Sonia", Apellidos = "Ruiz", 
            Email = "sonia.ruiz@gestionacademica.local", Calificacion = 5.0, 
            Ciclo = Ciclo.DAW, Curso = Curso.Segundo,
            FechaNacimiento = new DateTime(2003, 12, 1)  // ~23 años
        });
        lista.Add(new Estudiante {
            Dni = "88888888X", Nombre = "Jorge", Apellidos = "Sánchez", 
            Email = "jorge.sanchez@gestionacademica.local", Calificacion = 2.0, 
            Ciclo = Ciclo.ASIR, Curso = Curso.Primero,
            FechaNacimiento = new DateTime(1995, 4, 18)  // ~31 años - mayor de 25
        });
        lista.Add(new Estudiante {
            Dni = "99999999P", Nombre = "Laura", Apellidos = "Fernández", 
            Email = "laura.fernandez@gestionacademica.local", Calificacion = 10.0, 
            Ciclo = Ciclo.ASIR, Curso = Curso.Segundo,
            FechaNacimiento = new DateTime(2002, 9, 25)  // ~23 años
        });
        lista.Add(new Estudiante {
            Dni = "00000000T", Nombre = "Carlos", Apellidos = "Jiménez", 
            Email = "carlos.jimenez@gestionacademica.local", Calificacion = 5.5, 
            Ciclo = Ciclo.ASIR, Curso = Curso.Segundo,
            FechaNacimiento = new DateTime(1998, 1, 10)  // ~28 años - mayor de 25
        });

        lista.Add(new Docente {
            Dni = "12345678P", Nombre = "Jose Luis", Apellidos = "González", 
            Email = "jose.gonzalez@gestionacademica.local", Experiencia = 15,
            Especialidad = Modulos.Programacion, Ciclo = Ciclo.DAW,
            FechaNacimiento = new DateTime(1990, 6, 15)  // ~35 años
        });
        lista.Add(new Docente {
            Dni = "23456789D", Nombre = "Beatriz", Apellidos = "Sánchez", 
            Email = "beatriz.sanchez@gestionacademica.local", Experiencia = 10,
            Especialidad = Modulos.BasesDatos, Ciclo = Ciclo.DAW,
            FechaNacimiento = new DateTime(1985, 3, 22)  // ~40 años
        });
        lista.Add(new Docente {
            Dni = "34567890V", Nombre = "Carlos", Apellidos = "Pérez", 
            Email = "carlos.perez@gestionacademica.local", Experiencia = 8, 
            Especialidad = Modulos.Entornos, Ciclo = Ciclo.DAM,
            FechaNacimiento = new DateTime(1990, 9, 10)  // ~35 años
        });
        lista.Add(new Docente {
            Dni = "45678901Z", Nombre = "Marta", Apellidos = "García", 
            Email = "marta.garcia@gestionacademica.local", Experiencia = 12,
            Especialidad = Modulos.LenguajesMarcas, Ciclo = Ciclo.DAW,
            FechaNacimiento = new DateTime(1982, 11, 3)  // ~43 años
        });
        lista.Add(new Docente {
            Dni = "56789012W", Nombre = "Raúl", Apellidos = "Martínez", 
            Email = "raul.martinez@gestionacademica.local", Experiencia = 5,
            Especialidad = Modulos.Programacion, Ciclo = Ciclo.ASIR,
            FechaNacimiento = new DateTime(1995, 2, 28)  // ~31 años
        });

        return lista;
    }
}