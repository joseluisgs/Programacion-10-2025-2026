Voy a crear un diagrama completo de navegación y funcionalidades para la migración a WPF:

Voy a crear el diagrama completo. Primero analiza las necesidades adicionales:

📊 Diagrama de Navegación y Funcionalidad - WPF GestionAcademica
🗺️ Navegación Principal
┌─────────────────────────────────────────────────────────────────────────────┐
│                              MAIN WINDOW                                    │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │                        MENU BAR                                      │   │
│  │  [Archivo] [Personas] [Informes] [Herramientas] [Ayuda]             │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │  SIDEBAR          │              CONTENT AREA                         │   │
│  │  ┌─────────────┐  │  ┌─────────────────────────────────────────┐    │   │
│  │  │ 📊 Dashboard│  │  │                                          │    │   │
│  │  │ 🎓 Estudiantes│  │  │         FRAME (Navigation)              │    │   │
│  │  │ 👨‍🏫 Docentes  │  │  │                                          │    │   │
│  │  │ 📈 Informes  │  │  │   - DashboardView                       │    │   │
│  │  │ 📊 Gráficos  │  │  │   - EstudiantesView                     │    │   │
│  │  │ 💾 Backups  │  │  │   - DocentesView                        │    │   │
│  │  │ 📥 Import/Exp│  │  │   - InformesView                       │    │   │
│  │  └─────────────┘  │  │   - GraficosView                        │    │   │
│  │                   │  │   - BackupView                          │    │   │
│  │  ┌─────────────┐  │  │   - ImportExportView                    │    │   │
│  │  │ ⚙️ Settings │  │  │   - AboutView                          │    │   │
│  │  │ 🌙 Tema     │  │  └─────────────────────────────────────────┘    │   │
│  │  └─────────────┘  │                                                  │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────────────────┘
📋 Pantallas y Funcionalidades Detalladas
1. SPLASH SCREEN (SplashWindow.xaml)
Elemento	Descripción
Icono	Resources/app-icon.ico
Título	"Gestión Académica"
ProgressBar	Indeterminado mientras carga
Estado	"Cargando servicios...", "Inicializando...", etc.
2. MAIN WINDOW (MainWindow.xaml)
Componente	Funcionalidad
MenuBar	Archivo, Personas, Informes, Herramientas, Ayuda
Sidebar	Navegación rápida con iconos
Frame	Contenedor de vistas
StatusBar	Estado de conexión, usuario, fecha
3. DASHBOARD (DashboardView.xaml)
┌────────────────────────────────────────────────────────────┐
│  🎓 GESTIÓN ACADÉMICA - Dashboard                          │
├────────────────────────────────────────────────────────────┤
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐     │
│  │ 👨‍🎓      │ │ 👨‍🏫      │ │ ✅       │ │ ❌       │     │
│  │Estudiant.| │ Docentes │ │Aprobados │ │Suspensos │     │
│  │   125    │ │    15    │ │   85%    │ │   15%    │     │
│  └──────────┘ └──────────┘ └──────────┘ └──────────┘     │
│                                                            │
│  ┌─────────────────────────────────────────────────────┐   │
│  │           ÚLTIMAS ACCIONES                          │   │
│  │  - Juan Pérez creado hace 2h                        │   │
│  │  - Maria García eliminada hace 5h                   │   │
│  │  - Backup realizado hace 1 día                      │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                            │
│  [🔗 Acceso rápido: Añadir Estudiante | Nuevo Backup]    │
└────────────────────────────────────────────────────────────┘
4. ESTUDIANTES (EstudiantesView.xaml + EstudianteEditView.xaml)
Pantalla	Componentes	Funcionalidad
Listado	DataGrid	Columns: ID, DNI, Nombre, Ciclo, Curso, Nota, Imagen
Filtros	ComboBox(Ciclo), ComboBox(Curso), TextBox(búsqueda)
Ordenación	Click en headers columna
Botones	Añadir, Editar, Eliminar, Ver detalles
Edición/Detalle	Formulario	TextBox(DNI), TextBox(Nombre), TextBox(Apellidos)
Imagen	Click en Image → OpenFileDialog (jpg, png, bmp)
Fecha	DatePicker con validación (>1900, < fecha actual)
Ciclo/Curso	ComboBox con enumerados
Nota	NumericUpDown (0-10) con validación
Validación	ErrorTemplate con mensajes rojos
Botones	Aceptar (habilitado si válido), Cancelar
Validaciones WPF:

DNI: 8 dígitos + letra (Regex)
Nombre: 2-30 caracteres (Length)
Fecha: RangeValidator (1900-Now)
Nota: 0-10 (RangeValidator)
Botones: IDataErrorInfo o ValidationRule
5. DOCENTES (DocentesView.xaml + DocenteEditView.xaml)
Igual estructura que Estudiantes pero con campos de docente:

Especialidad (TextBox)
Experiencia (NumericUpDown, 0-50 años)
Ciclo (ComboBox)
6. INFORMES (InformesView.xaml)
┌────────────────────────────────────────────────────────────┐
│  📈 GENERAR INFORMES                                       │
├────────────────────────────────────────────────────────────┤
│  Tipo de Informe:                                          │
│  ┌────────────────────────────────────────────────────┐    │
│  │ ○ Estudiantes (rendimiento)                       │    │
│  │ ○ Docentes (experiencia)                          │    │
│  │ ○ Listado completo                                │    │
│  │ ○ Filtrado por ciclo [DAM ▼]                      │    │
│  └────────────────────────────────────────────────────┘    │
│                                                            │
│  Opciones:                                                 │
│  ☑ Incluir eliminados    ☑ Con imágenes                   │
│                                                            │
│  Formato de salida:                                        │
│  ○ HTML (.html)                                            │
│  ● PDF (.pdf) ← Predeterminado                             │
│  ○ Both (HTML + PDF)                                       │
│                                                            │
│  [📄 PREVISUALIZAR]  [💾 GENERAR]  [📤 EXPORTAR]          │
└────────────────────────────────────────────────────────────┘
7. GRÁFICOS (GraficosView.xaml) - SCOTTPLOT
┌────────────────────────────────────────────────────────────┐
│  📊 ESTADÍSTICAS Y GRÁFICOS                                │
├────────────────────────────────────────────────────────────┤
│  ┌──────────────────────┐  ┌──────────────────────────┐    │
│  │   📊 NOTAS           │  │   🥧 APROBADOS/SUSPENSOS │    │
│  │   [Barras]          │  │      [Pie Chart]         │    │
│  │   (histograma)      │  │                          │    │
│  │                      │  │   - Aprobados: 85%       │    │
│  │                      │  │   - Suspensos: 15%       │    │
│  └──────────────────────┘  └──────────────────────────┘    │
│                                                            │
│  ┌──────────────────────┐  ┌──────────────────────────┐    │
│  │   📈 CICLOS          │  │   👥 POR EDAD            │    │
│  │   [Barras agrupadas] │  │      [Histograma]        │    │
│  │   DAM | DAW | ASIR   │  │   <18, 18-25, >25        │    │
│  └──────────────────────┘  └──────────────────────────┘    │
│                                                            │
│  [📊 EXPORTAR COMO PNG]  [📄 EXPORTAR COMO PDF]          │
└────────────────────────────────────────────────────────────┘
Gráficos ScottPlot:

Barras: Notas por rangos (0-2, 2-4, 4-6, 6-8, 8-10)
Pie: Porcentaje aprobados vs suspensos
Barras agrupadas: Estudiantes por ciclo
Histograma: Distribución de edades
8. BACKUP (BackupView.xaml)
┌────────────────────────────────────────────────────────────┐
│  💾 COPIAS DE SEGURIDAD                                    │
├────────────────────────────────────────────────────────────┤
│  [💾 CREAR BACKUP]  [📂 RESTAURAR]                         │
│                                                            │
│  ┌─────────────────────────────────────────────────────┐   │
│  │  Backups disponibles                                │   │
│  │  ─────────────────────────────────────────────────  │   │
│  │  1. 2024-01-15-10-30-00-back.zip  (2.5 MB) [R] [D] │   │
│  │  2. 2024-01-10-09-15-00-back.zip  (2.3 MB) [R] [D] │   │
│  │  3. 2024-01-05-08-00-00-back.zip  (2.1 MB) [R] [D] │   │
│  │                                                     │   │
│  │  [R] = Restaurar  [D] = Eliminar                    │   │
│  └─────────────────────────────────────────────────────┘   │
└────────────────────────────────────────────────────────────┘
Funcionalidad:

Crear: SaveFileDialog para elegir destino
Restaurar: OpenFileDialog (filter: *.zip) + confirmación
Listar: DataGrid con fecha, tamaño, acciones
9. IMPORT/EXPORT (ImportExportView.xaml)
┌────────────────────────────────────────────────────────────┐
│  📥 IMPORTAR / 📤 EXPORTAR DATOS                           │
├────────────────────────────────────────────────────────────┤
│  ┌─────────────────────┐   ┌─────────────────────────┐     │
│  │   📥 IMPORTAR       │   │   📤 EXPORTAR          │     │
│  │                     │   │                         │     │
│  │  [Seleccionar]     │   │  [Seleccionar destino] │     │
│  │                     │   │                         │     │
│  │  Formato: JSON ▼   │   │  Formato: [JSON ▼]     │     │
│  │                     │   │           [CSV ▼]       │     │
│  │  ☑ Eliminar datos  │   │           [BIN ▼]       │     │
│  │     antes de       │   │                         │     │
│  │     importar       │   │  [📥 Importar]          │     │
│  │                     │   │  [📤 Exportar]          │     │
│  └─────────────────────┘   └─────────────────────────┘     │
└────────────────────────────────────────────────────────────┘
FileDialogs:

Import: OpenFileDialog (filter: JSON|.json|CSV|.csv|BIN|*.bin)
Export: SaveFileDialog (mismo filter)
10. ACERCA DE (AboutView.xaml)
┌────────────────────────────────────────────────────────────┐
│  ℹ️ ACERCA DE                                              │
├────────────────────────────────────────────────────────────┤
│                                                            │
│        🎓 ICONO APP (128x128)                             │
│                                                            │
│        GESTIÓN ACADÉMICA                                  │
│        Versión 1.0.0                                      │
│                                                            │
│        © 2024 DAW Academy                                 │
│        Jose Luis Garcia                                   │
│                                                            │
│        [🔗 GitHub]  → https://github.com/joseluisgs       │
│                                                            │
│        [Cerrar]                                           │
└────────────────────────────────────────────────────────────┘
Implementación:

HyperlinkButton con NavigateUri
Image del logo GitHub
Ventana modal (ShowDialog)
11. AJUSTES/TEMA (SettingsView.xaml)
┌────────────────────────────────────────────────────────────┐
│  ⚙️ AJUSTES                                               │
├────────────────────────────────────────────────────────────┤
│  Tema:                                                    │
│  ○ Claro    ● Oscuro    ○ Sistema                         │
│                                                            │
│  Notificaciones:                                          │
│  ☑ Mostrar notificaciones                                 │
│  ☑ Confirmar eliminación                                 │
│                                                            │
│  Repositorio: [Memory ▼] (Memory|Json|Dapper|EfCore)     │
│  Storage:    [JSON ▼] (JSON|CSV|BIN)                      │
│                                                            │
│  [💾 GUARDAR]  [↩️ RESTAURAR POR DEFECTO]                 │
└────────────────────────────────────────────────────────────┘
🎨 Temas (Dark/Light)
Recurso	Light Theme	Dark Theme
Background	#FFFFFF	#1E1E1E
Foreground	#000000	#FFFFFF
Primary	#667EEA	#667EEA
Secondary	#764BA2	#764BA2
Success	#28A745	#28A745
Error	#DC3545	#DC3545
Border	#CCCCCC	#3C3C3C
Card	#F5F5F5	#2D2D2D
📊 ViewModels Necesarios
ViewModel	Propiedades	Comandos
SplashViewModel	Estado, Mensaje	-
MainViewModel	CurrentView, IsDarkTheme	NavigateCommand
DashboardViewModel	Estadísticas	RefreshCommand
EstudiantesViewModel	Lista, Filtro, Orden	Add, Edit, Delete, Filter
EstudianteEditViewModel	Estudiante, ValidationErrors	Save, Cancel, SelectImage
DocentesViewModel	Lista, Filtro, Orden	Add, Edit, Delete, Filter
DocenteEditViewModel	Docente, ValidationErrors	Save, Cancel, SelectImage
InformesViewModel	Tipo, Formato, PreviewHtml	GenerateCommand
GraficosViewModel	ChartData	RefreshCommand, ExportCommand
BackupViewModel	ListaBackups, SelectedBackup	Create, Restore, Delete
ImportExportViewModel	Formato, FilePath	ImportCommand, ExportCommand
SettingsViewModel	Theme, Config	SaveCommand
🔧 Métodos Adicionales Necesarios en Servicios
// En IPersonasService - para filtros avanzados
IEnumerable<Estudiante> GetEstudiantesByCiclo(Ciclo ciclo);
IEnumerable<Estudiante> GetEstudiantesByCurso(Curso curso);
IEnumerable<Estudiante> GetEstudiantesAprobados(double notaCorte);
IEnumerable<Docente> GetDocentesByCiclo(Ciclo ciclo);
IEnumerable<Docente> GetDocentesByExperiencia(int minAnios);

// En IReportService - para diferentes formatos
Result<byte[], DomainError> GenerarInformeEstudiantesPdf(...);
Result<byte[], DomainError> GenerarInformeDocentesPdf(...);

// En IImageService - para WPF
Result<BitmapImage, DomainError> GetImage(string fileName); // BitmapImage para XAML
✅ Checklist de Implementación
#	Componente	Estado	Notas
1	Splash Screen	⬜	Icono + ProgressBar
2	Main Window con navegación	⬜	Menu + Sidebar + Frame
3	Dashboard con estadísticas	⬜	Cards + últimas acciones
4	CRUD Estudiantes (DataGrid + Edit)	⬜	Image click, DatePicker, validación
5	CRUD Docentes (DataGrid + Edit)	⬜	Igual que estudiantes
6	Informes (HTML/PDF/Both)	⬜	FileDialog para guardar
7	Gráficos ScottPlot	⬜	Barras, Pie, Histogramas
8	Backup/Restore con FileDialog	⬜	Open/SaveFileDialog
9	Import/Export con formato	⬜	ComboBox formato + FileDialog
10	About con GitHub link	⬜	HyperlinkButton
11	Tema Dark/Light	⬜	ResourceDictionary dinámico
12	Validación UI WPF	⬜	IDataErrorInfo + ValidationRule
🆕 Métodos Nuevos a Añadir
IPersonasService:

IEnumerable<Estudiante> GetEstudiantesFiltrados(Ciclo? ciclo, Curso? curso, double? notaMin);
IEnumerable<Docente> GetDocentesFiltrados(Ciclo? ciclo, int? experienciaMin);
int GetTotalEstudiantes();
int GetTotalDocentes();
int GetAprobadosCount(double notaCorte);
int GetSuspensosCount(double notaCorte);
Dictionary<Ciclo, int> GetEstudiantesPorCiclo();
Dictionary<Ciclo, int> GetDocentesPorCiclo();
IReportService:

Result<byte[], DomainError> GenerarInformeEstudiantesPdf(...);
Result<byte[], DomainError> GenerarInformeDocentesPdf(...);
Result<string, DomainError> GenerarInformeEstudiantesCsv(...);



-----


 El problema: El método _personasService.GetAll() en el servicio tiene un tamaño de página por defecto de 100 (lo cambié antes). Sin embargo, el
     repositorio (IPersonasRepository.GetAll) por defecto devuelve solo las 10 primeras personas si no se especifica lo contrario.
   * La consecuencia: todasPersonas solo contiene los 10 primeros registros (que en el Seed son Estudiantes). Al hacer OfType<Docente>(), la lista sale
     vacía o incompleta.
   * Mi propuesta: El Dashboard no debe usar GetAll() (paginado), sino métodos específicos del servicio que devuelvan el conteo total real filtrando por
     tipo en el repositorio, o un método GetAllWithoutPagination().

  2. 🎨 Análisis de Estética Nativa vs Temas Custom

  Tienes razón, los temas manuales en WPF pueden volverse "feos" y difíciles de mantener. Aquí mi análisis de opciones:

  Opción A: "Nativización" Total (Herencia del Sistema)
  Podemos eliminar los archivos .xaml de temas y forzar a WPF a usar los recursos del sistema.
   * Cómo: Usar DynamicResource apuntando a SystemColors.
   * Resultado: La App tendrá el color exacto de la barra de tareas y ventanas del usuario (Gris/Blanco/Azul según su Windows). Se verá "limpia" pero muy
     estándar.

      Mi recomendación de profesor a profesor:

   1. Arreglar el Dashboard: Debemos modificar el servicio para que el conteo no dependa de una lista paginada. Es un error de lógica de negocio que
      debemos solventar.
   2. Sobre la UI: Yo propongo quitar los temas custom "feos" y dejarla nativa. Si los alumnos están aprendiendo, es mejor que vean cómo WPF se integra con
      el SO sin capas extra de XAML complejo que solo ensucia el proyecto.

  ¿Qué decides?
   * ¿Arreglo el contador del Dashboard usando métodos de conteo real?
   * ¿Elimino los diccionarios de temas y conecto la UI a los colores del sistema?

  Espero tu aprobación para actuar.
▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀
 > haz 1 y 2                                                                                                                                               
▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄
✦ Para realizar estos cambios con rigor, voy a proceder por fases, asegurándome de que el Dashboard sea preciso y la interfaz se integre perfectamente con
  Windows.

  Fase 1: Corrección de Contadores (Lógica de Datos)

  Voy a actualizar el contrato del repositorio y sus implementaciones para obtener el conteo real sin depender de listas paginadas.

   1. IPersonasRepository: Añado CountEstudiantes() y CountDocentes().
   2. Implementaciones: Actualizo los 4 repositorios (Memory, JSON, Dapper, EF).
   3. PersonasService: Añado los métodos de conteo.
   4. DashboardViewModel: Actualizo para usar estos métodos.