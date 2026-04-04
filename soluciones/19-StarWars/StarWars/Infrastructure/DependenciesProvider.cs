using Microsoft.Extensions.DependencyInjection;
using StarWars.Factories;
using StarWars.ViewModels;
using StarWars.Views.Main;

namespace StarWars.Infrastructure;

public static class DependenciesProvider
{
    public static IServiceCollection ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<DroideGenerator>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<MainWindow>();
        return services;
    }
}
