// ============================================================
// App.xaml.cs - Punto de entrada de la aplicación WPF
// ============================================================
// Este archivo es el punto de entrada de la aplicación WPF.
//
// CONCEPTOS IMPORTANTES:
//
// 1. WPF (Windows Presentation Foundation):
//    - Framework de Microsoft para crear aplicaciones de escritorio
//    - Usa XAML para definir la interfaz gráfica
//    - Permite separar la UI del código (code-behind)
//
// 2. Application:
//    - Clase base para toda aplicación WPF
//    - Controla el ciclo de vida de la aplicación
//    - Tiene eventos como OnStartup, OnExit
//
// 3. Inyección de Dependencias (DI):
//    - Patrón de diseño que permite gestionar dependencias
//    - DependenciesProvider registra los servicios
//    - ServiceProvider proporciona las instancias
//
// PUNTO DE ENTRADA:
// 1. Se crea el objeto Application (Main() generado automáticamente)
// 2. Se ejecuta OnStartup() <- aquí configuramos todo
// 3. Se crea el ServiceProvider con las dependencias
// 4. Se abre la ventana principal (MainWindow)
// 5. Cuando se cierra la última ventana, se ejecuta OnExit()

using System;
using System.Diagnostics;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ListaCompra.Infrastructure;
using ListaCompra.Views.Main;

namespace ListaCompra;

/// <summary>
/// Clase principal de la aplicación WPF.
/// Gestiona el ciclo de vida de la aplicación y la inyección de dependencias.
/// </summary>
public partial class App : Application
{
    // ============================================================
    // SERVICE PROVIDER ESTÁTICO
    // ============================================================
    // Se hace público y estático para que todas las ventanas puedan
    // acceder a los servicios registrados sin necesidad de recibirlos por constructor.
    //
    // Ejemplo de uso en una ventana:
    //   var servicio = App.ServiceProvider.GetRequiredService<IProductoService>();
    //
    /// <summary>
    /// Proveedor de servicios con todas las dependencias registradas.
    /// Acceso global desde cualquier parte de la aplicación.
    /// </summary>
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    /// <summary>
    /// Constructor de la aplicación.
    /// Se ejecuta al crear el objeto Application.
    /// </summary>
    public App()
    {
        // Debug: Mostrar mensaje al crear la aplicación
        Debug.WriteLine("🔵 [App] Constructor - Objeto Application creado");
    }

    /// <summary>
    /// Se ejecuta cuando la aplicación va a iniciar.
    /// Aquí configuramos la inyección de dependencias y mostramos la ventana principal.
    /// </summary>
    protected override void OnStartup(StartupEventArgs e)
    {
        // Llamar al método base primero (importante)
        base.OnStartup(e);
        
        Debug.WriteLine("🔵 [App] OnStartup - La aplicación está iniciando");
        
        // ============================================================
        // INYECCIÓN DE DEPENDENCIAS (DI)
        // ============================================================
        // Crear el ServiceProvider con todas las dependencias registradas.
        // DependenciesProvider.BuildServiceProvider() devuelve un IServiceProvider
        // que contiene todas las instancias de servicios, repositorios y validadores.
        //
        // Registro de dependencias:
        // - IProductoRepository -> ProductoRepository (AddSingleton)
        // - IValidador<Producto> -> ValidadorProducto (AddTransient)
        // - IProductoService -> ProductoService (AddTransient)
        ServiceProvider = DependenciesProvider.BuildServiceProvider();
        
        Debug.WriteLine("✅ [App] ServiceProvider creado con todas las dependencias");
        
        // ============================================================
        // ABRIR VENTANA PRINCIPAL
        // ============================================================
        // Crear y mostrar la ventana principal de la aplicación.
        // No usamos StartupUri en App.xaml porque queremos controlar
        // la creación después de configurar la inyección de dependencias.
        var mainWindow = new MainWindow();
        mainWindow.Show();
    }

    /// <summary>
    /// Se ejecuta cuando la aplicación termina.
    /// Liberamos recursos aquí.
    /// </summary>
    protected override void OnExit(ExitEventArgs e)
    {
        Debug.WriteLine("🔴 [App] OnExit - La aplicación está terminando");
        
        // Dispose del ServiceProvider para liberar recursos
        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
        
        base.OnExit(e);
    }
}

// ============================================================
// NOTA IMPORTANTE: Punto de entrada en WPF
// ============================================================
//
// En WPF con XAML, NO necesitamos un método Main() explícito.
// El compilador genera automáticamente el punto de entrada.
//
// CÓMO FUNCIONA:
//
// 1. El archivo .csproj define:
//      <OutputType>WinExe</OutputType>   <- Tipo de salida (ventana)
//      <UseWPF>true</UseWPF>            <- Usar WPF
//
// 2. App.xaml define:
//    <Application x:Class="ListaCompra.App">
//      <- No usamos StartupUri porque creamos la ventana manualmente
//    </Application>
//
// 3. App.xaml.cs:
//    - Hereda de Application
//    - Tiene OnStartup() que se ejecuta al iniciar
//    - Tiene OnExit() que se ejecuta al terminar
//
// 4. MainWindow.xaml:
//    - Define la interfaz gráfica en XAML
//    - MainWindow.xaml.cs contiene el código (event handlers, etc.)
//
// DIFERENCIA CON WINFORMS:
// - WPF: Usa XAML para la UI, más moderno y flexible
// - WinForms: Código nativo para la UI, más simple pero menos flexible
// - Ambos usan el mismo patrón de código (Application, Window, etc.)