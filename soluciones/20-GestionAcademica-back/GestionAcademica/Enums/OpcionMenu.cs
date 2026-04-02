namespace GestionAcademica.Enums;

/// <summary>
///     Opciones del menú principal organizadas jerárquicamente.
/// </summary>
public enum OpcionMenu
{
    Salir = 0,

    // Bloque General (1-4)
    ListarTodas = 1,
    BuscarDni = 2,
    BuscarId = 3,
    ListarTodasHtml = 4,

    // Bloque Estudiantes (5-10)
    ListarEstudiantes = 5,
    AnadirEstudiante = 6,
    ActualizarEstudiante = 7,
    EliminarEstudiante = 8,
    InformeEstudiantes = 9,
    InformeEstudiantesHtml = 10,

    // Bloque Docentes (11-16)
    ListarDocentes = 11,
    AnadirDocente = 12,
    ActualizarDocente = 13,
    EliminarDocente = 14,
    InformeDocentes = 15,
    InformeDocentesHtml = 16,

    // Importar/Exportar (17-18)
    ImportarDatos = 17,
    ExportarDatos = 18,

    // Backup (19-20)
    RealizarBackup = 19,
    RestaurarBackup = 20,


}