using Microsoft.Extensions.DependencyInjection;
using TemasEstilos.Services;
using TemasEstilos.ViewModels;
using TemasEstilos.Views;

namespace TemasEstilos.Infrastructure;

/// <summary>
/// Proveedor de dependencias para la aplicación.
/// </summary>
public static class DependenciesProvider
{
    /// <summary>
    /// Configura los servicios de la aplicación.
    /// </summary>
    /// <param name="services">Colección de servicios</param>
    /// <returns>Colección de servicios con las dependencias registradas</returns>
    public static IServiceCollection ConfigureServices(IServiceCollection services)
    {
        // Singleton: una sola instancia para toda la aplicación
        // El servicio de temas necesita mantener estado (tema actual)
        services.AddSingleton<ITemasService, TemasService>();
        
        // Transient: nueva instancia cada vez que se solicita
        services.AddTransient<MainViewModel>();
        services.AddTransient<MainWindow>();
        
        return services;
    }
}