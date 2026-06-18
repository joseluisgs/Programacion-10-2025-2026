using GestionAcademica.Services.Dialogs;
using GestionAcademica.ViewModels;
using GestionAcademica.ViewModels.Dashboard;
using GestionAcademica.ViewModels.Docentes;
using GestionAcademica.ViewModels.Estudiantes;
using GestionAcademica.ViewModels.Backup;
using GestionAcademica.ViewModels.Graficos;
using GestionAcademica.ViewModels.Informes;
using GestionAcademica.ViewModels.ImportExport;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace GestionAcademica.Infrastructure.FrontDependenciesProvider;

public static class FrontDependenciesProvider {
    public static IServiceProvider BuildServiceProvider() {
        Log.Information("Configurando servicios (Back + Front)...");

        var serviceProvider = GestionAcademica.Infrastructure.DependenciesProvider.BuildServiceProvider(services => {
            services.AddSingleton<IDialogService, WpfDialogService>();
            RegisterViewModels(services);
            Log.Information("ViewModels registradas desde Front");
        });

        Log.Information("Servicios configurados correctamente");

        return serviceProvider;
    }

    private static void RegisterViewModels(IServiceCollection services) {
        // ViewModel principal de la aplicación
        services.AddTransient<MainViewModel>();
        // ViewModel del Dashboard (página de inicio con estadísticas)
        services.AddTransient<DashboardViewModel>();
        // ViewModels de Docentes (listado y edición)
        services.AddTransient<DocentesViewModel>();
        services.AddTransient<DocenteEditViewModel>();
        // ViewModels de Estudiantes (listado y edición)
        services.AddTransient<EstudiantesViewModel>();
        services.AddTransient<EstudianteEditViewModel>();
        // ViewModel de Backup (gestión de copias de seguridad)
        services.AddTransient<BackupViewModel>();
        // ViewModel de Gráficos (visualización de estadísticas)
        services.AddTransient<GraficosViewModel>();
        // ViewModel de Informes (generación de reportes)
        services.AddTransient<InformesViewModel>();
        // ViewModel de Import/Export (importación y exportación de datos)
        services.AddTransient<ImportExportViewModel>();
    }
}