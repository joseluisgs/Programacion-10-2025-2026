// =============================================================================
// INYECCIÓN DE DEPENDENCIAS
// =============================================================================
// La inyección de dependencias permite que las clases reciban sus dependencias
// desde el exterior en lugar de crearlas internamente.
// =============================================================================

using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TareasBackground.ViewModels;
using TareasBackground.Views;

namespace TareasBackground.Infrastructure;

/// <summary>
/// Clase que configura el contenedor de inyección de dependencias.
/// </summary>
public static class DependenciesProvider
{
    /// <summary>
    /// Construye el ServiceProvider con todas las dependencias registradas.
    /// </summary>
    public static IServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        
        // ViewModels - Singleton para compartir estado
        services.AddSingleton<MainViewModel>();
        
        // Ventanas - Transient para crear nueva instancia cada vez
        services.AddTransient<MainWindow>();
        
        return services.BuildServiceProvider();
    }
}
