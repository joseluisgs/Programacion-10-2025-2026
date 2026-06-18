using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Personas;
using GestionAcademica.ViewModels.Docentes;
using GestionAcademica.ViewModels.Estudiantes;
using GestionAcademica.ViewModels.Forms;

namespace GestionAcademica.Mappers.Personas;

public static class PersonaMapper {
    public static EstudianteFormData ToFormData(this Estudiante model) {
        return new EstudianteFormData {
            Id = model.Id,
            Nombre = model.Nombre,
            Apellidos = model.Apellidos,
            Dni = model.Dni,
            Email = model.Email,
            FechaNacimiento = model.FechaNacimiento,
            Imagen = model.Imagen,
            Calificacion = model.Calificacion,
            Ciclo = model.Ciclo,
            Curso = model.Curso,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt,
            IsDeleted = model.IsDeleted,
            DeletedAt = model.DeletedAt
        };
    }

    public static Estudiante ToModel(this EstudianteFormData formData) {
        return new Estudiante {
            Id = formData.Id,
            Nombre = formData.Nombre ?? string.Empty,
            Apellidos = formData.Apellidos ?? string.Empty,
            Dni = formData.Dni ?? string.Empty,
            Email = formData.Email ?? string.Empty,
            FechaNacimiento = formData.FechaNacimiento,
            Imagen = formData.Imagen,
            Calificacion = formData.Calificacion,
            Ciclo = formData.Ciclo,
            Curso = formData.Curso,
            CreatedAt = formData.CreatedAt,
            UpdatedAt = formData.UpdatedAt,
            IsDeleted = formData.IsDeleted,
            DeletedAt = formData.DeletedAt
        };
    }

    public static DocenteFormData ToFormData(this Docente model) {
        return new DocenteFormData {
            Id = model.Id,
            Nombre = model.Nombre,
            Apellidos = model.Apellidos,
            Dni = model.Dni,
            Email = model.Email,
            FechaNacimiento = model.FechaNacimiento,
            Imagen = model.Imagen,
            Experiencia = model.Experiencia,
            Especialidad = model.Especialidad,
            Ciclo = model.Ciclo,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt,
            IsDeleted = model.IsDeleted,
            DeletedAt = model.DeletedAt
        };
    }

    public static Docente ToModel(this DocenteFormData formData) {
        return new Docente {
            Id = formData.Id,
            Nombre = formData.Nombre ?? string.Empty,
            Apellidos = formData.Apellidos ?? string.Empty,
            Dni = formData.Dni ?? string.Empty,
            Email = formData.Email ?? string.Empty,
            FechaNacimiento = formData.FechaNacimiento,
            Imagen = formData.Imagen,
            Experiencia = formData.Experiencia,
            Especialidad = formData.Especialidad ?? string.Empty,
            Ciclo = formData.Ciclo,
            CreatedAt = formData.CreatedAt,
            UpdatedAt = formData.UpdatedAt,
            IsDeleted = formData.IsDeleted,
            DeletedAt = formData.DeletedAt
        };
    }

    public static EstudianteItemViewModel ToItemViewModel(this Estudiante model) {
        return new EstudianteItemViewModel {
            Id = model.Id,
            Dni = model.Dni,
            Nombre = model.Nombre,
            Apellidos = model.Apellidos,
            Email = model.Email,
            FechaNacimiento = model.FechaNacimiento,
            Imagen = model.Imagen,
            Calificacion = model.Calificacion,
            Ciclo = model.Ciclo,
            Curso = model.Curso,
            IsDeleted = model.IsDeleted
        };
    }

    public static DocenteItemViewModel ToItemViewModel(this Docente model) {
        return new DocenteItemViewModel {
            Id = model.Id,
            Dni = model.Dni,
            Nombre = model.Nombre,
            Apellidos = model.Apellidos,
            Email = model.Email,
            FechaNacimiento = model.FechaNacimiento,
            Imagen = model.Imagen,
            Experiencia = model.Experiencia,
            Especialidad = model.Especialidad,
            Ciclo = model.Ciclo,
            IsDeleted = model.IsDeleted
        };
    }

    public static void UpdateFromFormData(this EstudianteItemViewModel item, EstudianteFormData form) {
        item.Dni = form.Dni;
        item.Nombre = form.Nombre;
        item.Apellidos = form.Apellidos;
        item.Email = form.Email;
        item.FechaNacimiento = form.FechaNacimiento;
        item.Imagen = form.Imagen;
        item.Calificacion = form.Calificacion;
        item.Ciclo = form.Ciclo;
        item.Curso = form.Curso;
        item.IsDeleted = form.IsDeleted;
    }

    public static void UpdateFromFormData(this DocenteItemViewModel item, DocenteFormData form) {
        item.Dni = form.Dni;
        item.Nombre = form.Nombre;
        item.Apellidos = form.Apellidos;
        item.Email = form.Email;
        item.FechaNacimiento = form.FechaNacimiento;
        item.Imagen = form.Imagen;
        item.Experiencia = form.Experiencia;
        item.Especialidad = form.Especialidad;
        item.Ciclo = form.Ciclo;
        item.IsDeleted = form.IsDeleted;
    }

    public static Estudiante ToModel(this EstudianteItemViewModel item) {
        return new Estudiante {
            Id = item.Id,
            Dni = item.Dni,
            Nombre = item.Nombre,
            Apellidos = item.Apellidos,
            Email = item.Email,
            FechaNacimiento = item.FechaNacimiento,
            Imagen = item.Imagen,
            Calificacion = item.Calificacion,
            Ciclo = item.Ciclo,
            Curso = item.Curso,
            IsDeleted = item.IsDeleted
        };
    }

    public static Docente ToModel(this DocenteItemViewModel item) {
        return new Docente {
            Id = item.Id,
            Dni = item.Dni,
            Nombre = item.Nombre,
            Apellidos = item.Apellidos,
            Email = item.Email,
            FechaNacimiento = item.FechaNacimiento,
            Imagen = item.Imagen,
            Experiencia = item.Experiencia,
            Especialidad = item.Especialidad,
            Ciclo = item.Ciclo,
            IsDeleted = item.IsDeleted
        };
    }
}