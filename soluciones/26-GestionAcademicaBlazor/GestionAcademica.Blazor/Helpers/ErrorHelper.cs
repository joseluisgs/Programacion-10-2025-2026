using Serilog;

namespace GestionAcademica.Blazor.Services;

public static class ErrorHelper
{
    private static readonly Serilog.ILogger Logger = Log.ForContext(typeof(ErrorHelper));

    public static string LogYAmigable(this Exception ex, string contexto)
    {
        Logger.Error(ex, "Error al {Contexto}", contexto);
        return $"Error al {contexto}. Si el problema persiste, contacte con el administrador.";
    }
}
