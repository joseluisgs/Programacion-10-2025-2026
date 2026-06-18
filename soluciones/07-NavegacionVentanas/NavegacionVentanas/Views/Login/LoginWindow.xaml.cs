// ============================================================
// LoginWindow.xaml.cs - Ventana de inicio de sesión
// ============================================================
// Simula una ventana de login.
//
// CONCEPTO:
// - Esta ventana podría ser la ventana inicial de la aplicación
// - Después de login exitoso, se abre la ventana principal
// - Se cierra el login para no volver atrás
//
// FLUJO COMPLETO:
// 1. App.xaml define StartupUri="Windows/LoginWindow.xaml"
// 2. LoginWindow valida credenciales
// 3. Si exitoso: abrir MainWindow, cerrar LoginWindow
// 4. La aplicación sigue corriendo con MainWindow como principal

using System.Windows;

namespace NavegacionVentanas.Views.Login;

/// <summary>
/// Ventana de inicio de sesión.
/// </summary>
public partial class LoginWindow : Window
{
    /// <summary>
    /// Constructor de la ventana de login.
    /// </summary>
    public LoginWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Valida las credenciales e inicia sesión.
    /// </summary>
    private void BtnLogin_Click(object sender, RoutedEventArgs e)
    {
        // Validar credenciales (demo: usuario=admin, password=123456)
        var usuario = TxtUsuario.Text.Trim();
        var password = TxtPassword.Password;
        
        if (usuario == "admin" && password == "123456")
        {
            // Login exitoso
            DialogResult = true;
            Close();
        }
        else
        {
            // Mostrar error
            TxtError.Text = "Credenciales incorrectas. Usa admin / 123456";
            TxtError.Visibility = Visibility.Visible;
        }
    }

    /// <summary>
    /// Cancela el login.
    /// </summary>
    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
