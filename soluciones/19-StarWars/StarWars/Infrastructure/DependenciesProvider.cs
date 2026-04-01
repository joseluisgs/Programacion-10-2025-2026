using Microsoft.Extensions.DependencyInjection;
using StarWars.Factories;
using StarWars.ViewModels;

namespace StarWars.Infrastructure;

/// <summary>
/// Proveedor de dependencias para la aplicación.
/// </summary>
/// <remarks>
/// Configura la inyección de dependencias usando Microsoft.Extensions.DependencyInjection.
/// Registra los servicios disponibles en el contenedor:
/// - DroideGenerator como Singleton (una sola instancia compartida)
/// - MainViewModel como Transient (nueva instancia cada vez que se solicita)
/// </remarks>
public static class DependenciesProvider
{
    /// <summary>
    /// Configura los servicios de la aplicación.
    /// </summary>
    /// <param name="services">Colección de servicios donde registrar las dependencias</param>
    /// <returns>La colección de servicios con las dependencias registradas</returns>
    public static IServiceCollection ConfigureServices(IServiceCollection services)
    {
        // Singleton: se crea una sola vez y se comparte en toda la aplicación
        // Útil para servicios sin estado que no necesitan recrearse
        services.AddSingleton<DroideGenerator>();
        
        // Transient: se crea una nueva instancia cada vez que se solicita
        // Útil para ViewModels que necesitan mantener estado independiente
        services.AddTransient<MainViewModel>();
        
        return services;
    }
}
