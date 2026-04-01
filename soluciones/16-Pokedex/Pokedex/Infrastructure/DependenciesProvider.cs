using System;
using Microsoft.Extensions.DependencyInjection;
using Pokedex.Cache;
using Pokedex.Config;
using Pokedex.Models;
using Pokedex.Repositories;
using Pokedex.Services;
using Pokedex.Storage;
using Pokedex.Storage.Json;
using Pokedex.Validators;
using Pokedex.ViewModels;

namespace Pokedex.Infrastructure;

public static class DependenciesProvider
{
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

    private static void RegisterRepositories(IServiceCollection services)
    {
        services.AddSingleton<IPokedexRepository, PokedexRepository>();
    }

    private static void RegisterValidators(IServiceCollection services)
    {
        services.AddTransient<IValidador<Pokemon>, ValidadorPokemon>();
    }

    private static void RegisterCache(IServiceCollection services)
    {
        services.AddSingleton<ICache<int, Pokemon>>(sp => 
            new LruCache<int, Pokemon>(AppConfig.CacheSize));
    }

    private static void RegisterStorages(IServiceCollection services)
    {
        services.AddSingleton<IPokedexStorage, PokedexJsonStorage>();
    }

    private static void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IPokedexService, PokedexService>();
    }

    private static void RegisterViewModels(IServiceCollection services)
    {
        services.AddTransient<MainViewModel>();
    }
}
