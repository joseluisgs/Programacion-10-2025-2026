// ============================================================
// App.xaml.cs - Punto de entrada de la aplicacion
// ============================================================
//
// =================================================================
// GUIA PARA EL ALUMNO: POR QUE EXISTE App.xaml.cs?
// =================================================================
//
// App.xaml.cs es el code-behind del punto de entrada de la aplicacion.
//
// Aqui podemos:
// - Inicializar servicios globales
// - Configurar manejo de excepciones
// - Establecer recursos globales
// - Cualquier cosa que necesite ejecutarse al inicio
//
// En este caso, no necesitamos nada especial porque:
// - StartupUri en App.xaml indica que ventana abrir
// - El ViewModel se crea en el constructor de MainWindow
//
// MAS ADELANTE:
// En aplicaciones mas complejas, aqui configuraremos:
// - Inyeccion de dependencias
// - Servicios de logging
// - Configuracion global
//

using System.Windows;

namespace WpfBindingsReactividad;

public partial class App : Application
{
}
