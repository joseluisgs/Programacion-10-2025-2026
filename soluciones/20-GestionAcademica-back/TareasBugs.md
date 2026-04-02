LISTADO DE TAREAS - REFACTORIZACIÓN GESTIONACADEMICA
🔴 TAREA 1: ELIMINAR TEMAS Y USAR COLORES NATIVOS (CRÍTICO)
Prioridad: 🔴 MÁXIMA
Tiempo estimado: 2-3 horas
Archivos afectados: ~15 archivos

Subtareas:
 1.1 Eliminar Themes/DarkTheme.xaml
 1.2 Eliminar Themes/LightTheme.xaml
 1.3 Eliminar referencia en App.xaml (línea 9)
 1.4 Agregar estilos de botones base en App.xaml
 1.5 Reemplazar colores en MainWindow.xaml
 1.6 Reemplazar colores en DashboardView.xaml
 1.7 Reemplazar colores en EstudiantesView.xaml
 1.8 Reemplazar colores en DocentesView.xaml
 1.9 Reemplazar colores en GraficosView.xaml
 1.10 Reemplazar colores en InformesView.xaml
 1.11 Reemplazar colores en BackupView.xaml
 1.12 Reemplazar colores en ImportExportView.xaml
 1.13 Eliminar métodos de tema en MainWindow.xaml.cs
 1.14 Probar en Windows 11 modo claro
 1.15 Probar en Windows 11 modo oscuro
Búsqueda y reemplazo:

Code
Background="#1E1E1E" → Background="{DynamicResource {x:Static SystemColors.WindowBrushKey}}"
Foreground="White" → Foreground="{DynamicResource {x:Static SystemColors.WindowTextBrushKey}}"
BorderBrush="#3C3C3C" → BorderBrush="{DynamicResource {x:Static SystemColors.ActiveBorderBrushKey}}"
🔴 TAREA 2: CORREGIR BUG DEL DASHBOARD (CRÍTICO)
Prioridad: 🔴 MÁXIMA
Tiempo estimado: 1 hora
Archivos afectados: 3 archivos

Subtareas:
 2.1 Agregar métodos a IPersonasService.cs:
int CountEstudiantes(bool includeDeleted = false);
int CountDocentes(bool includeDeleted = false);
int CountAprobados(double notaCorte, bool includeDeleted = false);
int CountSuspensos(double notaCorte, bool includeDeleted = false);
Dictionary<Ciclo, int> GetEstudiantesPorCiclo(bool includeDeleted = false);
Dictionary<Ciclo, int> GetDocentesPorCiclo(bool includeDeleted = false);
 2.2 Implementar métodos en PersonasService.cs
 2.3 Actualizar DashboardViewModel.cs método LoadStatistics()
 2.4 Probar que los conteos sean correctos
🔴 TAREA 3: ARREGLAR GRÁFICOS SCOTTPLOT (CRÍTICO)
Prioridad: 🔴 MÁXIMA
Tiempo estimado: 2-3 horas
Archivos afectados: 3 archivos

Subtareas:
 3.1 Agregar método GetEstudiantesPorEdad() en GraficosViewModel.cs
 3.2 Actualizar GraficosView.xaml - agregar 2 controles WpfPlot más (4 total)
 3.3 Reescribir GraficosView.xaml.cs con inicialización de los 4 gráficos:
Gráfico 1: Notas medias por ciclo (barras)
Gráfico 2: Distribución de calificaciones (pie)
Gráfico 3: Personas por ciclo (barras agrupadas)
Gráfico 4: Estudiantes por edad (barras)
 3.4 Probar que todos los gráficos se visualicen correctamente
⚠️ TAREA 4: IMPLEMENTAR VALIDACIÓN VISUAL (IDataErrorInfo) (ALTA)
Prioridad: ⚠️ ALTA
Tiempo estimado: 2 horas
Archivos afectados: 2 archivos

Subtareas:
 4.1 Actualizar Estudiante.cs:
Implementar IDataErrorInfo
Agregar validaciones: Nombre, Apellidos, DNI, Email, FechaNacimiento, Calificacion
 4.2 Actualizar Docente.cs:
Implementar IDataErrorInfo
Agregar validaciones: Nombre, Apellidos, DNI, Email, FechaNacimiento, Especialidad, Experiencia
 4.3 Probar validaciones en formularios existentes
⚠️ TAREA 5: CREAR VENTANAS MODALES DE EDICIÓN (ALTA)
Prioridad: ⚠️ ALTA
Tiempo estimado: 4-5 horas
Archivos afectados: 10 archivos (6 nuevos + 4 modificados)

Subtareas:
 5.1 Crear Views/Dialog/EstudianteEditWindow.xaml
 5.2 Crear Views/Dialog/EstudianteEditWindow.xaml.cs
 5.3 Crear Views/Dialog/DocenteEditWindow.xaml
 5.4 Crear Views/Dialog/DocenteEditWindow.xaml.cs
 5.5 Crear ViewModels/Estudiantes/EstudianteEditViewModel.cs
 5.6 Crear ViewModels/Docentes/DocenteEditViewModel.cs
 5.7 Actualizar EstudiantesViewModel.cs - métodos New() y Edit() para abrir modales
 5.8 Actualizar DocentesViewModel.cs - métodos New() y Edit() para abrir modales
 5.9 Actualizar EstudiantesView.xaml - eliminar panel lateral de edición
 5.10 Actualizar DocentesView.xaml - eliminar panel lateral de edición
 5.11 Agregar campos faltantes: Email y FechaNacimiento en ambas ventanas
 5.12 Registrar ViewModels en DI (App.xaml.cs)
 5.13 Probar crear nuevo estudiante
 5.14 Probar editar estudiante existente
 5.15 Probar crear nuevo docente
 5.16 Probar editar docente existente
📘 TAREA 6: CREAR TESTS UNITARIOS (MEDIA)
Prioridad: 📘 MEDIA
Tiempo estimado: 3-4 horas
Archivos afectados: 4 archivos nuevos

Subtareas:
 6.1 Crear proyecto de tests si no existe: GestionAcademica.Tests
 6.2 Agregar referencias: NUnit, Moq, FluentAssertions
 6.3 Crear Models/EstudianteTests.cs con 8 tests:
Test validación nombre vacío
Test validación DNI formato incorrecto
Test validación email formato incorrecto
Test validación calificación fuera de rango (Theory con -1, 11, 15.5)
Test calificación cualitativa (Theory con todas las notas)
Test validación fecha futura
Test validación completa correcta
 6.4 Crear Models/DocenteTests.cs con 3 tests:
Test validación especialidad vacía
Test validación experiencia fuera de rango (Theory con -1, 51, 100)
Test validación completa correcta
 6.5 Crear Services/PersonasServiceTests.cs con 4 tests:
Test CountEstudiantes
Test CountDocentes
Test CountAprobados
Test GetEstudiantesPorCiclo
 6.6 Crear ViewModels/DashboardViewModelTests.cs con 3 tests:
Test LoadStatistics con datos
Test cálculo de porcentajes
Test manejo de cero estudiantes
 6.7 Ejecutar todos los tests y verificar que pasen
📋 TAREA 7: MEJORAS ADICIONALES (OPCIONAL) (BAJA)
Prioridad: 📋 BAJA
Tiempo estimado: 2-3 horas
Archivos afectados: 2-3 archivos

Subtareas:
 7.1 Agregar ComboBox de selección de formato en ImportExportView.xaml
 7.2 Agregar filtros por ciclo en InformesView.xaml
 7.3 Agregar botón "Generar ambos formatos" (PDF + HTML)
 7.4 Separar acciones de Exportar e Importar en menú
 7.5 Separar acciones de Crear y Restaurar Backup en menú
📊 RESUMEN DE PRIORIDADES
Tarea	Prioridad	Tiempo	¿Bloqueante?
1. Colores nativos	🔴 CRÍTICA	2-3h	❌ No
2. Bug Dashboard	🔴 CRÍTICA	1h	✅ Sí (afecta datos)
3. Gráficos ScottPlot	🔴 CRÍTICA	2-3h	✅ Sí (no funcionan)
4. Validación visual	⚠️ ALTA	2h	❌ No
5. Ventanas modales	⚠️ ALTA	4-5h	❌ No
6. Tests unitarios	📘 MEDIA	3-4h	❌ No
7. Mejoras adicionales	📋 BAJA	2-3h	❌ No
Tiempo total estimado: 16-23 horas

🎯 ORDEN RECOMENDADO DE EJECUCIÓN
SPRINT 1: BUGS CRÍTICOS (4-7 horas)
✅ Tarea 2: Bug Dashboard → 1 hora
✅ Tarea 3: Gráficos ScottPlot → 2-3 horas
✅ Tarea 1: Colores nativos → 2-3 horas
SPRINT 2: FUNCIONALIDAD (6-7 horas)
✅ Tarea 4: Validación visual → 2 horas
✅ Tarea 5: Ventanas modales → 4-5 horas
SPRINT 3: CALIDAD (3-4 horas)
✅ Tarea 6: Tests unitarios → 3-4 horas
SPRINT 4: EXTRAS OPCIONALES (2-3 horas)
⚪ Tarea 7: Mejoras adicionales → 2-3 horas
💡 RECOMENDACIÓN
Si quieres hacer un PR inmediato con impacto visible:

Hacer solo TAREA 1 + TAREA 2 (3-4 horas) → PR funcional con colores nativos y Dashboard corregido
Si quieres completar lo crítico:

Hacer SPRINT 1 completo (Tareas 1, 2, 3) → PR con todos los bugs corregidos
Si quieres la refactorización completa profesional:

Hacer SPRINT 1 + SPRINT 2 (Tareas 1-5) → PR completo sin tests
Si quieres entrega 100% profesional:

Hacer SPRINT 1 + SPRINT 2 + SPRINT 3 (Tareas 1-6) → PR con todo + tests
🚀 ¿QUÉ QUIERES HACER?
Dime cuál de estas opciones prefieres y procedo a crear el PR con esas tareas específicas:

A) Solo lo crítico inmediato (Tareas 1 + 2) → 3-4 horas
B) Todos los bugs (Sprint 1: Tareas 1, 2, 3) → 4-7 horas
C) Bugs + Funcionalidad (Sprints 1+2: Tareas 1-5) → 10-14 horas
D) TODO completo con tests (Sprints 1+2+3: Tareas 1-6) → 13-18 horas