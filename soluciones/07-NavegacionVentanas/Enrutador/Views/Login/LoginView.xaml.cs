using System.Windows;
using Enrutador.Infrastructure;

namespace Enrutador.Views.Login;

public partial class LoginView : Window
{
    public LoginView()
    {
        InitializeComponent();
    }

    private void BtnLogin_Click(object sender, RoutedEventArgs e)
    {
        // Simulamos validación
        if (TxtUsuario.Text == "admin" && TxtPassword.Password == "123456")
        {
            // ¡Usamos el Enrutador para navegar!
            RoutesManager.NavigateTo(RoutesManager.View.Main);
        }
        else
        {
            MessageBox.Show("Credenciales incorrectas (admin/123456)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
