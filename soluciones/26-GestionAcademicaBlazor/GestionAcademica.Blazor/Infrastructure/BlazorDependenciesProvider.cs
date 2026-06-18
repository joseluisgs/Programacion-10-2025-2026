using GestionAcademica.Blazor.Services;
using GestionAcademica.Infrastructure;
using GestionAcademica.Services.Dialogs;
using GestionAcademica.Services.Images;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace GestionAcademica.Blazor.Infrastructure;

public static class BlazorDependenciesProvider
{
    public static IServiceProvider BuildServiceProvider()
    {
        Log.Information("Configurando servicios Blazor (Back + Blazor)...");

        var serviceProvider = DependenciesProvider.BuildServiceProvider(services =>
        {
            services.AddSingleton<IImageService, BlazorImageService>();
            Log.Information("Servicios Blazor registrados");
        });

        Log.Information("Servicios configurados correctamente");
        return serviceProvider;
    }
}
