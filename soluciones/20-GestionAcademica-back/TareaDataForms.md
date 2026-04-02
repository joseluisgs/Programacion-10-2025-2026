🏗️ PROPUESTA ARQUITECTÓNICA COMPLETA
📊 ANÁLISIS DEL CÓDIGO ACTUAL
He analizado tu proyecto y encontré:

✅ Lo que está BIEN:
Mappers con extensiones - Patrón consistente con ToModel(), ToEntity(), ToDto()
ViewModels MVVM limpios - Usan ObservableObject del MVVM Toolkit
Servicios desacoplados - IPersonasService, IImageService con DI
Records inmutables - Modelos de dominio puros
CultureInfo invariante - Para fechas y números
⚠️ PROBLEMAS DE UNIFORMIDAD DETECTADOS:
Problema	Ubicación	Impacto
ViewModels editan modelos directamente	EstudiantesViewModel, DocentesViewModel	Alto - viola separación
Propiedades mutables en ViewModels	EditingEstudiante, EditingDocente	Medio - records se modifican con with
NO hay capa de FormData	Todo el proyecto	Alto - lógica UI mezclada
Inconsistencia en nombres	Algunos usan EditingX, otros no	Bajo - naming
🎯 SOLUCIÓN PROPUESTA
ARQUITECTURA DE 3 CAPAS:
Code
┌─────────────────────────────────────┐
│  View (XAML)                        │
│  - EstudianteEditWindow.xaml        │
└──────────────┬──────────────────────┘
               │ Binding
┌──────────────▼──────────────────────┐
│  ViewModel                          │
│  - EstudianteEditViewModel          │
│    • Contiene EstudianteFormData    │
│    • Comandos (Save, Cancel)        │
└──────────────┬──────────────────────┘
               │ Usa
┌──────────────▼──────────────────────┐
│  FormData (CON IDataErrorInfo)      │
│  - EstudianteFormData               │
│    • Validación UI                  │
│    • INotifyPropertyChanged         │
│    • ToModel() / FromModel()        │
└──────────────┬──────────────────────┘
               │ Mapea
┌──────────────▼──────────────────────┐
│  Model (Dominio PURO)               │
│  - Estudiante (record)              │
│    • SIN IDataErrorInfo             │
│    • Lógica de negocio              │
└──────────────┬─────────────��────────┘
               │ Persiste
┌──────────────▼──────────────────────┐
│  Entity (Base de Datos)             │
│  - PersonaEntity                    │
└─────────────────────────────────────┘
📁 ESTRUCTURA DE ARCHIVOS
Code
GestionAcademica/
├── Models/
│   └── Personas/
│       ├── Persona.cs                    ← PURO (sin IDataErrorInfo)
│       ├── Estudiante.cs                 ← PURO
│       └── Docente.cs                    ← PURO
│
├── ViewModels/
│   ├── Forms/                            ← NUEVA CARPETA
│   │   ├── EstudianteFormData.cs         ← CON IDataErrorInfo
│   │   └── DocenteFormData.cs            ← CON IDataErrorInfo
│   │
│   ├── Estudiantes/
│   │   ├── EstudiantesViewModel.cs       ← Lista (sin cambios)
│   │   └── EstudianteEditViewModel.cs    ← NUEVO (modal)
│   │
│   └── Docentes/
│       ├── DocentesViewModel.cs          ← Lista (sin cambios)
│       └── DocenteEditViewModel.cs       ← NUEVO (modal)
│
├── Mappers/
│   └── Personas/
│       └── PersonaMapper.cs              ← EXTENDIDO con FormData
│
└── Views/
    └── Dialog/                            ← NUEVA CARPETA
        ├── EstudianteEditWindow.xaml
        ├── EstudianteEditWindow.xaml.cs
        ├── DocenteEditWindow.xaml
        └── DocenteEditWindow.xaml.cs
💻 IMPLEMENTACIÓN DETALLADA
Voy a crear el PULL REQUEST CORRECTO con esta estructura. Te doy un preview del código:

1. EstudianteFormData.cs (NUEVO)
C#
using System.ComponentModel;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using GestionAcademica.Models.Academia;
using GestionAcademica.Models.Personas;

namespace GestionAcademica.ViewModels.Forms;

/// <summary>
/// FormData para validación de Estudiante en la UI.
/// Separa la lógica de presentación del modelo de dominio.
/// </summary>
public partial class EstudianteFormData : ObservableObject, IDataErrorInfo
{
    [ObservableProperty]
    private int _id;
    
    [ObservableProperty]
    private string _nombre = string.Empty;
    
    [ObservableProperty]
    private string _apellidos = string.Empty;
    
    [ObservableProperty]
    private string _dni = string.Empty;
    
    [ObservableProperty]
    private string _email = string.Empty;
    
    [ObservableProperty]
    private DateTime _fechaNacimiento = DateTime.Now.AddYears(-18);
    
    [ObservableProperty]
    private string? _imagen;
    
    [ObservableProperty]
    private double _calificacion;
    
    [ObservableProperty]
    private Ciclo _ciclo;
    
    [ObservableProperty]
    private Curso _curso;

    // IDataErrorInfo para validación
    public string Error => null!;

    public string this[string columnName]
    {
        get
        {
            return columnName switch
            {
                nameof(Nombre) when string.IsNullOrWhiteSpace(Nombre)
                    => "El nombre es obligatorio",
                nameof(Nombre) when Nombre.Length < 2 || Nombre.Length > 30
                    => "El nombre debe tener entre 2 y 30 caracteres",

                nameof(Apellidos) when string.IsNullOrWhiteSpace(Apellidos)
                    => "Los apellidos son obligatorios",
                nameof(Apellidos) when Apellidos.Length < 2 || Apellidos.Length > 50
                    => "Los apellidos deben tener entre 2 y 50 caracteres",

                nameof(Dni) when string.IsNullOrWhiteSpace(Dni)
                    => "El DNI es obligatorio",
                nameof(Dni) when !Regex.IsMatch(Dni, @"^\d{8}[A-Z]$")
                    => "El DNI debe tener 8 dígitos y una letra mayúscula (ej: 12345678Z)",

                nameof(Email) when string.IsNullOrWhiteSpace(Email)
                    => "El email es obligatorio",
                nameof(Email) when !Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")
                    => "El formato del email es inválido",

                nameof(FechaNacimiento) when FechaNacimiento.Year < 1900
                    => "La fecha debe ser posterior a 1900",
                nameof(FechaNacimiento) when FechaNacimiento > DateTime.Now
                    => "La fecha no puede ser futura",

                nameof(Calificacion) when Calificacion < 0 || Calificacion > 10
                    => "La nota debe estar entre 0 y 10",

                _ => null!
            };
        }
    }
}
2. Extensiones en PersonaMapper.cs
C#
// AGREGAR al archivo existente PersonaMapper.cs (línea 220+)

/// <summary>
/// Convierte EstudianteFormData a Estudiante (modelo de dominio).
/// </summary>
public static Estudiante ToModel(this EstudianteFormData formData)
{
    return new Estudiante
    {
        Id = formData.Id,
        Nombre = formData.Nombre,
        Apellidos = formData.Apellidos,
        Dni = formData.Dni,
        Email = formData.Email,
        FechaNacimiento = formData.FechaNacimiento,
        Imagen = formData.Imagen,
        Calificacion = formData.Calificacion,
        Ciclo = formData.Ciclo,
        Curso = formData.Curso,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        IsDeleted = false
    };
}

/// <summary>
/// Convierte Estudiante a EstudianteFormData.
/// </summary>
public static EstudianteFormData ToFormData(this Estudiante model)
{
    return new EstudianteFormData
    {
        Id = model.Id,
        Nombre = model.Nombre,
        Apellidos = model.Apellidos,
        Dni = model.Dni,
        Email = model.Email,
        FechaNacimiento = model.FechaNacimiento,
        Imagen = model.Imagen,
        Calificacion = model.Calificacion,
        Ciclo = model.Ciclo,
        Curso = model.Curso
    };
}

/// <summary>
/// Convierte DocenteFormData a Docente (modelo de dominio).
/// </summary>
public static Docente ToModel(this DocenteFormData formData)
{
    return new Docente
    {
        Id = formData.Id,
        Nombre = formData.Nombre,
        Apellidos = formData.Apellidos,
        Dni = formData.Dni,
        Email = formData.Email,
        FechaNacimiento = formData.FechaNacimiento,
        Imagen = formData.Imagen,
        Experiencia = formData.Experiencia,
        Especialidad = formData.Especialidad,
        Ciclo = formData.Ciclo,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        IsDeleted = false
    };
}

/// <summary>
/// Convierte Docente a DocenteFormData.
/// </summary>
public static DocenteFormData ToFormData(this Docente model)
{
    return new DocenteFormData
    {
        Id = model.Id,
        Nombre = model.Nombre,
        Apellidos = model.Apellidos,
        Dni = model.Dni,
        Email = model.Email,
        FechaNacimiento = model.FechaNacimiento,
        Imagen = model.Imagen,
        Experiencia = model.Experiencia,
        Especialidad = model.Especialidad,
        Ciclo = model.Ciclo
    };
}
📋 CAMBIOS EN OTROS ARCHIVOS
Problemas de uniformidad encontrados:
Archivo	Problema	Solución
EstudiantesViewModel.cs	Usa EditingEstudiante mutable	Eliminar, usar ventana modal de confirmación
DocentesViewModel.cs	Usa EditingDocente mutable	Eliminar, usar ventana modal de confirmación
MainViewModel.cs	Tiene CambiarTema() que ya no existe	Eliminar método
Todos los ViewModels	Usan ObservableObject correctamente	✅ OK
Todos los Mappers	Usan extensiones consistentemente	✅ OK
