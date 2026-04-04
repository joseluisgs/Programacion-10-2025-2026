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
using ListaCompra.Cache;
using ListaCompra.Config;
using ListaCompra.Repositories;
using ListaCompra.Services;
using ListaCompra.Storage.Common;
using ListaCompra.Storage.Csv;
using ListaCompra.Storage.Json;
using ListaCompra.Validators;
using ListaCompra.Models;
using ListaCompra.FormData.ViewModels;

namespace ListaCompra.Infrastructure;

/// <summary>
/// Proveedor de dependencias de la aplicación.
/// Registra todos los servicios, repositorios, validadores, cache y viewmodels.
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
        var services = new ServiceCollection();
        
        RegisterRepositories(services);
        RegisterValidators(services);
        RegisterCache(services);
        RegisterStorages(services);
        RegisterServices(services);
        RegisterViewModels(services);
        
        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Registra los repositorios (capa de acceso a datos).
    /// </summary>
    private static void RegisterRepositories(IServiceCollection services)
    {
        services.AddSingleton<IProductoRepository, ProductoRepository>();
    }

    /// <summary>
    /// Registra los validadores (reglas de negocio).
    /// </summary>
    private static void RegisterValidators(IServiceCollection services)
    {
        services.AddTransient<IValidador<Producto>, ValidadorProducto>();
    }

    /// <summary>
    /// Registra la caché LRU.
    /// </summary>
    private static void RegisterCache(IServiceCollection services)
    {
        services.AddSingleton<ICache<int, Producto>>(sp => 
            new LruCache<int, Producto>(AppConfig.CacheSize));
    }

    /// <summary>
    /// Registra los storages (JSON y CSV).
    /// </summary>
    private static void RegisterStorages(IServiceCollection services)
    {
        services.AddSingleton<IStorage<Producto>, ProductoJsonStorage>();
        services.AddSingleton<IStorage<Producto>, ProductoCsvStorage>();
    }

    /// <summary>
    /// Registra los servicios (lógica de negocio).
    /// </summary>
    private static void RegisterServices(IServiceCollection services)
    {
        services.AddTransient<IProductoService, ProductoService>();
        services.AddTransient<IBackupService, BackupService>();
    }

    /// <summary>
    /// Registra los ViewModels (patrón MVVM).
    /// </summary>
    private static void RegisterViewModels(IServiceCollection services)
    {
        services.AddTransient<MainViewModel>();
    }
}
