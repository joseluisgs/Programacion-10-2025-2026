// ============================================================
// DependenciesProvider.cs - Proveedor de dependencias con DI automática
// ============================================================
// Implementa el patrón de Inyección de Dependencias (DI) usando
// Microsoft.Extensions.DependencyInjection.
//
// CONCEPTOS IMPORTANTES:
//
// 1. INYECCIÓN DE DEPENDENCIAS (DI):
//    - Patrón de diseño que gestiona las dependencias entre objetos
//    - En lugar de crear dependencias manualmente, las injectamos
//    - Facilita el testing y el mantenimiento
//
// 2. IServiceCollection:
//    - Colección donde registramos todos los servicios
//    - Cada registro especifica: interfaz -> implementación
//    - Define el ciclo de vida (Singleton, Transient, Scoped)
//
// 3. IServiceProvider:
//    - Proveedor que crea las instancias de los servicios registrados
//    - GetRequiredService<T>(): Obtiene una instancia del tipo solicitado
//    - GetService<T>(): Similar pero devuelve null si no existe
//
// 4. TIPOS DE REGISTRO:
//    - AddSingleton: Una sola instancia para toda la aplicación
//    - AddTransient: Nueva instancia cada vez que se solicita
//    - AddScoped: Una instancia por ámbito (no usado en WPF)
//
// USO EN LA APLICACIÓN:
// 1. App.xaml.cs llama a DependenciesProvider.BuildServiceProvider()
// 2. El ServiceProvider se guarda en App.ServiceProvider (público)
// 3. Las ventanas acceden a App.ServiceProvider.GetRequiredService<T>()
// 4. El ServiceProvider crea automáticamente las instancias con sus dependencias

using System;
using Microsoft.Extensions.DependencyInjection;
using ListaCompra.Repositories;
using ListaCompra.Services;
using ListaCompra.Validators;
using ListaCompra.Models;

namespace ListaCompra.Infrastructure;

/// <summary>
/// Proveedor de dependencias de la aplicación.
/// Clase estática que registra todos los servicios, repositorios y validadores.
/// </summary>
public static class DependenciesProvider
{
    /// <summary>
    /// Construye el ServiceProvider con todas las dependencias registradas.
    /// Debe llamarse al inicio de la aplicación (en App.OnStartup).
    /// </summary>
    /// <returns>IServiceProvider con todas las dependencias registradas.</returns>
    public static IServiceProvider BuildServiceProvider()
    {
        // IServiceCollection: coleção de servicios a registrar
        // Aquí definimos qué clases instanciar para cada interfaz
        var services = new ServiceCollection();
        
        // Registrar las dependencias en orden (repositorios, validadores, servicios)
        RegisterRepositories(services);
        RegisterValidators(services);
        RegisterServices(services);
        
        // BuildServiceProvider(): Compila la colección y crea el proveedor
        // A partir de aquí podemos obtener instancias con GetRequiredService<T>
        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Registra los repositorios (capa de acceso a datos).
    /// </summary>
    private static void RegisterRepositories(IServiceCollection services)
    {
        // AddSingleton: Crea una única instancia que se comparte en toda la aplicación.
        // Ideal para repositorios en memoria (como este caso).
        // El repositorio guarda los datos durante toda la ejecución de la app.
        services.AddSingleton<IProductoRepository, ProductoRepository>();
    }

    /// <summary>
    /// Registra los validadores (reglas de negocio).
    /// </summary>
    private static void RegisterValidators(IServiceCollection services)
    {
        // AddTransient: Crea una nueva instancia cada vez que se solicita.
        // Ideal para validadores porque pueden tener estado específico de cada operación.
        // Cada vez que GetRequiredService<IValidador<Producto>> se llama,
        // se crea una nueva instancia de ValidadorProducto.
        services.AddTransient<IValidador<Producto>, ValidadorProducto>();
    }

    /// <summary>
    /// Registra los servicios (lógica de negocio).
    /// </summary>
    private static void RegisterServices(IServiceCollection services)
    {
        // AddTransient: Nueva instancia cada vez que se solicita.
        // El servicio recibe el repositorio y el validador por inyección automáticamente.
        // Cuando se crea ProductoService, DI le pasa:
        //   - IProductoRepository (del singleton registrado)
        //   - IValidador<Producto> (del transient registrado)
        services.AddTransient<IProductoService, ProductoService>();
    }
}

// ============================================================
// RESUMEN: FLUJO DE LA INYECCIÓN DE DEPENDENCIAS
// ============================================================
//
//    App.xaml.cs (OnStartup)
//           |
//           v
//    DependenciesProvider.BuildServiceProvider()
//           |
//           v
//    IServiceProvider (contiene todas las dependencias)
//           |
//           v
//    App.ServiceProvider = ... (acceso estático global)
//           |
//           v
//    MainWindow (accede via App.ServiceProvider.GetRequiredService)
//           |
//           v
//    IProductoService (creado con sus dependencias)
//           |
//           v
//    IProductoRepository + IValidador<Producto>