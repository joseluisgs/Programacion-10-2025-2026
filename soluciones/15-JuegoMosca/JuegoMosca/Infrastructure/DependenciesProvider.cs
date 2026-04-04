// =============================================================================
// INYECCIÓN DE DEPENDENCIAS
// =============================================================================
// La inyección de dependencias (DI) es un patrón que nos permite
// proporcionar las dependencias (objetos que necesita una clase)
// desde "afuera" en lugar de crearlas dentro.
//
// BENEFICIOS:
// - Desacoplamiento: Las clases no crean sus dependencias
// - Testabilidad: Podemos reemplazar dependencias con mocks
// - Flexibilidad: Podemos cambiar implementaciones fácilmente
// =============================================================================

using System;
using Microsoft.Extensions.DependencyInjection;
using JuegoMosca.ViewModels;
using JuegoMosca.Views.Config;
using JuegoMosca.Views.Main;

namespace JuegoMosca.Infrastructure;

public static class DependenciesProvider
{
    public static IServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        
        services.AddSingleton<MoscaViewModel>();
        services.AddTransient<ConfigViewModel>();
        services.AddTransient<ConfigWindow>();
        services.AddTransient<MainWindow>();
        
        return services.BuildServiceProvider();
    }
}
