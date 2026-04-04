// ============================================================
// RoutesManager.cs - Gestor de rutas de la aplicación
// ============================================================
// Clase estática que centraliza la navegación entre ventanas.
// Basado en el patrón Router/RoutesManager de JavaFX.

using System;
using System.Windows;
using Enrutador.Views.Login;
using Enrutador.Views.Main;
using Enrutador.Views.Modal;
using Enrutador.Views.NoModal;

namespace Enrutador.Infrastructure;

/// <summary>
/// Gestiona la navegación y las rutas de la aplicación.
/// </summary>
public static class RoutesManager
{
    private static Window? _mainWindow;
    private static Window? _activeWindow;

    public enum View
    {
        Login,
        Main,
        Modal,
        NoModal
    }

    public static void InitMainStage(View view = View.Login)
    {
        var window = CreateView(view);
        _mainWindow = window;
        _activeWindow = window;
        _mainWindow.Show();
    }

    public static void NavigateTo(View view)
    {
        var oldWindow = _mainWindow;
        _mainWindow = CreateView(view);
        _activeWindow = _mainWindow;
        _mainWindow.Show();
        oldWindow?.Close();
    }

    public static void OpenWindow(View view)
    {
        var window = CreateView(view);
        _activeWindow = window;
        window.Show();
    }

    public static bool? ShowModal(View view)
    {
        var window = CreateView(view);
        window.Owner = _mainWindow;
        return window.ShowDialog();
    }

    private static Window CreateView(View view) => view switch
    {
        View.Login => new LoginView(),
        View.Main => new MainView(),
        View.Modal => new ModalView(),
        View.NoModal => new NoModalView(),
        _ => throw new ArgumentOutOfRangeException(nameof(view), view, "Vista no soportada")
    };

    public static void Exit()
    {
        Application.Current.Shutdown();
    }
}
