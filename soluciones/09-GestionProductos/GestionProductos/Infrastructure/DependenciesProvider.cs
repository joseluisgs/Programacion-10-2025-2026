// ============================================================
// DependenciesProvider.cs - Proveedor de dependencias
// ============================================================
// Registra todas las dependencias con inyección automática.
//
// REGISTROS:
// - IProductoRepository -> ProductoRepository (Singleton)
// - IValidador<Producto> -> ValidadorProducto (Transient)
// - IProductoService -> ProductoService (Transient)

using Microsoft.Extensions.DependencyInjection;
using GestionProductos.Repositories;
using GestionProductos.Services;
using GestionProductos.Validators;
using GestionProductos.Models;

namespace GestionProductos.Infrastructure;

/// <summary>
/// Proveedor de dependencias.
/// </summary>
public static class DependenciesProvider
{
    /// <summary>
    /// Construye el ServiceProvider.
    /// </summary>
    public static IServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        
        RegisterRepositories(services);
        RegisterValidators(services);
        RegisterServices(services);
        
        return services.BuildServiceProvider();
    }

    private static void RegisterRepositories(IServiceCollection services)
    {
        // Singleton: una sola instancia para toda la app
        services.AddSingleton<IProductoRepository, ProductoRepository>();
    }

    private static void RegisterValidators(IServiceCollection services)
    {
        // Transient: nueva instancia cada vez
        services.AddTransient<IValidador<Producto>, ValidadorProducto>();
    }

    private static void RegisterServices(IServiceCollection services)
    {
        // Transient: nueva instancia cada vez
        services.AddTransient<IProductoService, ProductoService>();
    }
}