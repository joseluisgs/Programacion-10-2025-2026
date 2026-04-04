// ============================================================
// DialogoUsuario.xaml.cs - Diálogo para ingresar datos de usuario
// ============================================================
// Ejemplo de diálogo personalizado que:
// - Usa propiedades públicas para devolver datos
// - Valida los datos antes de cerrar
// - Usa DialogResult para comunicar el resultado

using System.Windows;

namespace Dialogos.Views.Dialogs;

public partial class DialogoUsuario : Window
{
    // ============================================================
    // PROPIEDADES PÚBLICAS (para devolver datos)
    // ============================================================
    
    /// <summary>
    /// Nombre del usuario ingresado.
    /// </summary>
    public string Nombre { get; private set; } = "";
    
    /// <summary>
    /// Email del usuario ingresado.
    /// </summary>
    public string Email { get; private set; } = "";

    // ============================================================
    // CONSTRUCTOR
    // ============================================================
    
    /// <summary>
    /// Constructor del diálogo.
    /// </summary>
    public DialogoUsuario()
    {
        InitializeComponent();
        TxtNombre.Focus();
    }

    // ============================================================
    // EVENTOS
    // ============================================================
    
    /// <summary>
    /// Botón Aceptar: valida y cierra.
    /// </summary>
    private void BtnAceptar_Click(object sender, RoutedEventArgs e)
    {
        // Validar nombre
        if (string.IsNullOrWhiteSpace(TxtNombre.Text))
        {
            MessageBox.Show("El nombre es obligatorio", "Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            TxtNombre.Focus();
            return;
        }
        
        // Validar email (básico)
        if (string.IsNullOrWhiteSpace(TxtEmail.Text) || !TxtEmail.Text.Contains("@"))
        {
            MessageBox.Show("El email no es válido", "Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            TxtEmail.Focus();
            return;
        }
        
        // Guardar los datos
        Nombre = TxtNombre.Text.Trim();
        Email = TxtEmail.Text.Trim();
        
        // Cerrar devolviendo true
        DialogResult = true;
    }

    /// <summary>
    /// Botón Cancelar: cierra sin guardar.
    /// </summary>
    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
