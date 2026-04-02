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

namespace JuegoMosca.Infrastructure;

/// <summary>
/// Clase estática que configura el contenedor de inyección de dependencias.
/// Aquí registramos todas las clases que serán "inyectadas".
/// </summary>
public static class DependenciesProvider
{
    /// <summary>
    /// Construye y configura el ServiceProvider.
    /// El ServiceProvider es el objeto que gestiona las dependencias.
    /// </summary>
    public static IServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        
        // ---------------------------------------------------------------------
        // REGISTRO DE SERVICIOS
        // ---------------------------------------------------------------------
        
        // Singleton: Una sola instancia para toda la aplicación
        // MoscaViewModel se comparte entre ConfigWindow y MainWindow
        services.AddSingleton<MoscaViewModel>();
        
        // Transient: Se crea una nueva instancia cada vez que se necesita
        services.AddTransient<ConfigViewModel>();
        
        // Transient: Se crea una nueva ventana cada vez que se necesita
        services.AddTransient<ConfigWindow>();
        
        // Construimos el proveedor de servicios
        return services.BuildServiceProvider();
    }
}
