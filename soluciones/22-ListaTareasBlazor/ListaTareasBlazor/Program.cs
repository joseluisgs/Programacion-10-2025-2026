using ListaTareasBlazor.Components;
using ListaTareasBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// ============================================================
// INYECCIÓN DE DEPENDENCIAS EN BLAZOR
// ============================================================
// Aquí registramos los servicios que serán inyectados en los componentes.
// AddScoped significa que se crea una nueva instancia por cada sesión de usuario.
// Esto es apropiado para servicios que necesitan mantener estado entre peticiones.

// Registro del servicio de tareas
// ITareaService es la interfaz (contrato)
// TareaService es la implementación concreta
builder.Services.AddScoped<ITareaService, TareaService>();

// Servicios de Razor Components (esto ya estaba)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
