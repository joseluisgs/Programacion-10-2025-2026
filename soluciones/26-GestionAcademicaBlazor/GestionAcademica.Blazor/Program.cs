// PUNTO DE ENTRADA DE LA APLICACION BLAZOR
// Blazor es un framework de ASP.NET Core que permite crear interfaces de usuario interactivas
// usando C# en lugar de JavaScript. El codigo se ejecuta en el servidor y la UI se actualiza
// mediante SignalR (WebSockets).

using ApexCharts;
using GestionAcademica.Blazor.Components;
using GestionAcademica.Blazor.Infrastructure;
using GestionAcademica.Blazor.Services;
using GestionAcademica.Config;
using GestionAcademica.Services.Backup;
using GestionAcademica.Services.Dialogs;
using GestionAcademica.Services.Images;
using GestionAcademica.Services.ImportExport;
using GestionAcademica.Services.Personas;
using GestionAcademica.Services.Report;
using Serilog;

// IMPORTANTE: Serilog se configura ANTES del builder porque queremos capturar errores
// incluso durante el arranque de la aplicacion. Se lee la configuracion desde appsettings.json.
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(AppConfig.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

try
{
    Log.Information("Iniciando aplicacion Blazor");

    var builder = WebApplication.CreateBuilder(args);

    // INTEGRAR SERILOG CON ASP.NET CORE
    // Sin UseSerilog(), el pipeline de ILogger<T> de ASP.NET Core usa el console provider
    // nativo (formato "info: Microsoft...") en lugar de Serilog, y los logs de servicios
    // Blazor (BlazorDialogService, BlazorImageService) pueden no aparecer en consola.
    builder.Host.UseSerilog();

    // HABILITAR STATIC WEB ASSETS PARA ENTORNOS NO-DESARROLLO
    // UseStaticWebAssets() permite que MapStaticAssets() funcione correctamente
    // en Production/Staging sin necesidad de publicar la aplicacion.
    // Sin esta llamada, MapStaticAssets() busca los assets en wwwroot/ en lugar
    // de usar el sistema de Static Web Assets del SDK, causando FileNotFoundException
    // para _framework/blazor.web.js (que solo existe en el cache de NuGet).
    builder.WebHost.UseStaticWebAssets();

    // HABILITAR RAZOR COMPONENTS CON RENDER MODE INTERACTIVO EN SERVIDOR
    // AddInteractiveServerComponents(): Configura Blazor Server, donde la UI se renderiza en el
    // servidor y se envia al navegador via SignalR. Esto permite que el codigo C# maneje eventos
    // (clicks, formularios) sin necesidad de JavaScript, manteniendo una conexion permanente
    // con el servidor a traves de un circuito SignalR.
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    // Registrar ApexCharts: servicio opcional para opciones globales y locales de graficos.
    // Si no se registra, los graficos usaran configuracion por defecto.
    builder.Services.AddApexCharts();

    // PUENTE DE INYECCION DE DEPENDENCIAS (Bridge Pattern)
    // BlazorDependenciesProvider.BuildServiceProvider() construye el contenedor DI de la capa Back
    // (GestionAcademica.Back) que contiene PersonasService, BackupService, etc., y registra las
    // implementaciones Blazor de IDialogService (BlazorDialogService) e IImageService (BlazorImageService).
    var backSp = BlazorDependenciesProvider.BuildServiceProvider();

    // Los servicios se resuelven del contenedor Back (backSp) y se registran en el contenedor
    // ASP.NET (builder.Services) para que Blazor pueda inyectarlos en los componentes mediante
    // la directiva @inject. Sin este registro, @inject no encontraria las dependencias.
    //
    // IDialogService se registra como Scoped en lugar de Singleton para poder inyectar
    // IJSRuntime (que es scoped por circuito SignalR) y asi hacer ShowConfirmation()
    // funcione correctamente con javascript sincrono (IJSInProcessRuntime).
    builder.Services.AddScoped<IDialogService, BlazorDialogService>();
    builder.Services.AddSingleton(backSp.GetRequiredService<IImageService>());
    builder.Services.AddSingleton(backSp.GetRequiredService<IPersonasService>());
    builder.Services.AddSingleton(backSp.GetRequiredService<IBackupService>());
    builder.Services.AddSingleton(backSp.GetRequiredService<IReportService>());
    builder.Services.AddSingleton(backSp.GetRequiredService<IImportExportService>());

    // STATECONTAINER: Singleton que permite comunicación entre páginas.
    // Cuando una página modifica datos (crear/editar/eliminar), publica un evento
    // y las demás páginas se refrescan automáticamente (auto-refresh).
    builder.Services.AddSingleton<StateContainer>();

    var app = builder.Build();

    // LIMPIAR DIRECTORIOS TEMPORALES EN WWWROOT
    // DependenciesProvider.CleanData() solo limpia las rutas del Back (BaseDirectory/reports/).
    // Pero Informes.razor, Backup.razor e ImportExport.razor guardan en wwwroot/ (subcarpetas
    // reports, backups, exports, uploads), que se acumulan sin limpiarse nunca.
    // Las imagenes subidas SOLO se limpian si DropData esta activo, no con SeedData.
    var wwwDirs = new List<string> { "reports", "backups", "exports", "uploads" };
    if (AppConfig.DropData) wwwDirs.Add("images");
    if (AppConfig.DropData || AppConfig.SeedData)
    {
        foreach (var dir in wwwDirs)
        {
            var path = Path.Combine(app.Environment.WebRootPath, dir);
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive: true);
                Directory.CreateDirectory(path);
            }
        }
    }

    // GARANTIZAR QUE LAS IMAGENES ESENCIALES EXISTEN EN wwwroot/images/
    // sin-imagen.png: placeholder cuando una persona no tiene foto.
    // app-icon.png: icono de la aplicacion (favicon, navbar, acerca de).
    // Se restauran desde Resources/images si faltan (ej: tras limpieza con DropData).
    var imgDir = Path.Combine(app.Environment.WebRootPath, "images");
    Directory.CreateDirectory(imgDir);
    var resourcesDir = Path.Combine(AppContext.BaseDirectory, "Resources", "images");
    foreach (var img in new[] { "sin-imagen.png", "app-icon.png" })
    {
        var dest = Path.Combine(imgDir, img);
        if (!File.Exists(dest))
        {
            var src = Path.Combine(resourcesDir, img);
            if (File.Exists(src))
                File.Copy(src, dest, true);
        }
    }

    // MIDDLEWARE PIPELINE
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
    }

    // ANTIFORGERY: Protege contra ataques CSRF en formularios Blazor.
    // Debe ir ANTES de MapStaticAssets y MapRazorComponents.
    app.UseAntiforgery();

    // MAPEAR ACTIVOS ESTATICOS (MapStaticAssets en .NET 10)
    // Reemplaza a UseStaticFiles() y ademas sirve los assets del framework
    // (_framework/blazor.web.js) que son necesarios para que SignalR funcione.
    // Sin MapStaticAssets, blazor.web.js no se sirve y los botones no funcionan.
    // MapStaticAssets tambien configura automáticamente los Static Web Assets
    // en desarrollo mediante el SDK (Microsoft.NET.Sdk.Web).
    app.MapStaticAssets();

    // MAPEAR COMPONENTES RAZOR CON RENDER MODE INTERACTIVO
    // AddInteractiveServerRenderMode() habilita el renderizado interactivo
    // en servidor. En .NET 10, NO se debe poner @rendermode en <Routes> ni
    // <HeadOutlet> de App.razor; el render mode se configura exclusivamente aqui.
    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();

    Log.Information("Aplicacion Blazor lista");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicacion terminada inesperadamente");
}
finally
{
    Log.CloseAndFlush();
}
