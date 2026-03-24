# 15-WpfExpedientesAcademicos

## Descripción
Aplicación de gestión de expedientes académicos con CRUD completo de estudiantes, asignaturas y calificaciones. Es el proyecto de mayor complejidad de la serie antes del proyecto final, integrando múltiples ventanas, validación exhaustiva, `DataGrid` con edición inline y un repositorio en memoria con interfaz abstracta.

## Objetivos de Aprendizaje
- Diseñar un sistema multi-entidad (Estudiante, Asignatura, Calificación) con relaciones entre ellas
- Implementar CRUD completo con `DataGrid` editable y ventanas de detalle modales
- Aplicar validación con `INotifyDataErrorInfo` en los modelos de dominio
- Estructurar el proyecto en capas: Models, ViewModels, Views, Services/Repositories
- Navegar entre ventanas usando un servicio de diálogo inyectado en los ViewModels
- Calcular estadísticas derivadas (media, nota máxima/mínima) en tiempo real

## Requisitos Funcionales
- RF-01: CRUD de estudiantes: nombre, apellidos, DNI, fecha de nacimiento, correo
- RF-02: CRUD de asignaturas: código, nombre, créditos, profesor
- RF-03: Gestión de calificaciones: asignar nota (0-10) a un estudiante en una asignatura
- RF-04: Vista resumen por estudiante: lista de asignaturas con notas y media calculada
- RF-05: Vista resumen por asignatura: lista de estudiantes matriculados con sus notas
- RF-06: Filtrado de estudiantes por nombre, DNI o asignatura
- RF-07: Validación completa en todos los formularios con mensajes de error
- RF-08: Confirmación al eliminar cualquier entidad

## Requisitos No Funcionales

| Código | Requisito | Descripción |
|--------|-----------|-------------|
| RNF-01 | Arquitectura | Capas definidas: Models, ViewModels, Views, Services |
| RNF-02 | Validación | `INotifyDataErrorInfo` en todas las entidades editables |
| RNF-03 | Desacoplamiento | Los ViewModels no referencian clases de WPF; usan servicios inyectados |

## Arquitectura
**MVVM completo con servicios y repositorios**

```
Views  ──binding──►  ViewModels  ──usa──►  Services  ──usa──►  Repositories
                                                                (en memoria)
```

- `IDialogService` abstrae la apertura de ventanas modales desde el ViewModel.
- `IEstudianteRepository`, `IAsignaturaRepository`, `ICalificacionRepository` abstraen el acceso a datos.
- Los ViewModels reciben las dependencias por constructor (DI manual).

## Tecnologías
- WPF (.NET 10)
- C# 14
- JetBrains Rider
- CommunityToolkit.Mvvm (NuGet)

## Estructura Sugerida
```
15-WpfExpedientesAcademicos/
└── WpfExpedientesAcademicos/
    ├── WpfExpedientesAcademicos.csproj
    ├── App.xaml
    ├── App.xaml.cs
    ├── Models/
    │   ├── Estudiante.cs
    │   ├── Asignatura.cs
    │   └── Calificacion.cs
    ├── Repositories/
    │   ├── IEstudianteRepository.cs
    │   ├── EstudianteRepository.cs
    │   ├── IAsignaturaRepository.cs
    │   └── AsignaturaRepository.cs
    ├── Services/
    │   ├── IDialogService.cs
    │   └── DialogService.cs
    ├── ViewModels/
    │   ├── EstudiantesViewModel.cs
    │   ├── AsignaturasViewModel.cs
    │   └── CalificacionesViewModel.cs
    └── Views/
        ├── MainWindow.xaml
        ├── EstudianteDetalleWindow.xaml
        └── AsignaturaDetalleWindow.xaml
```

## Funcionalidades Clave
- `TabControl` en la ventana principal para navegar entre Estudiantes, Asignaturas y Calificaciones
- `DataGrid` con edición inline para calificaciones
- Estadísticas en tiempo real con propiedades calculadas en el ViewModel
- Servicio de diálogo que abstrae `new VentanaDetalle().ShowDialog()`

## Notas
- Alineado con la teoría de `09-wpf-mvvm-bindings-reactividad.md`
- Para la DI manual, instanciar repositorios y servicios en `App.xaml.cs` y pasarlos al ViewModel principal
- `INotifyDataErrorInfo` permite múltiples errores por propiedad y validación asíncrona
- Los repositorios en memoria pueden evolucionar a JSON o SQLite cambiando solo la implementación
