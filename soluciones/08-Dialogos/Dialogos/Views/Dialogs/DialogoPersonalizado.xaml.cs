// ============================================================
// DialogoPersonalizado.xaml.cs - Diálogo completo personalizado
// ============================================================
// Ejemplo avanzado de diálogo que:
// - Recibe configuración inicial en propiedades
// - Tiene múltiples campos de entrada
// - Devuelve todos los datos mediante propiedades

using System.Windows;

namespace Dialogos.Views.Dialogs;

public partial class DialogoPersonalizado : Window
{
    // ============================================================
    // PROPIEDADES DE ENTRADA (configuración inicial)
    // ============================================================
    
    /// <summary>
    /// Título inicial del diálogo.
    /// </summary>
    public string TituloInicial { get; set; } = "Configuración";

    // ============================================================
    // PROPIEDADES DE SALIDA (resultados)
    // ============================================================
    
    /// <summary>
    /// Título introducido por el usuario.
    /// </summary>
    public string TituloResultado { get; private set; } = "";
    
    /// <summary>
    /// Descripción introducida por el usuario.
    /// </summary>
    public string DescripcionResultado { get; private set; } = "";
    
    /// <summary>
    /// Si el usuario confirmó la operación.
    /// </summary>
    public bool Confirmado { get; private set; } = false;

    // ============================================================
    // CONSTRUCTOR
    // ============================================================
    
    /// <summary>
    /// Constructor del diálogo.
    /// </summary>
    /// <param name="tituloInicial">Título inicial del diálogo.</param>
    public DialogoPersonalizado(string tituloInicial = "Configuración")
    {
        InitializeComponent();
        
        // Guardar título inicial
        TituloInicial = tituloInicial;
        
        // Aplicar configuración inicial
        Title = tituloInicial;
        TxtTitulo.Text = tituloInicial;
        TxtTituloInput.Text = "Valor por defecto";
        TxtDescripcion.Text = "Descripción opcional...";
    }

    // ============================================================
    // EVENTOS
    // ============================================================
    
    /// <summary>
    /// Botón Confirmar.
    /// </summary>
    private void BtnConfirmar_Click(object sender, RoutedEventArgs e)
    {
        // Validar
        if (string.IsNullOrWhiteSpace(TxtTituloInput.Text))
        {
            MessageBox.Show("El título es obligatorio", "Error",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            TxtTituloInput.Focus();
            return;
        }
        
        // Guardar resultados
        TituloResultado = TxtTituloInput.Text.Trim();
        DescripcionResultado = TxtDescripcion.Text.Trim();
        Confirmado = ChkConfirmado.IsChecked == true;
        
        // Cerrar
        DialogResult = true;
    }

    /// <summary>
    /// Botón Cancelar.
    /// </summary>
    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}

// ============================================================
// RESUMEN: PATRONES PARA DIÁLOGOS PERSONALIZADOS
// ============================================================
//
// 1. PROPIEDADES DE ENTRADA:
//    - Se establecen ANTES de ShowDialog()
//    - Configuran el estado inicial del diálogo
//    - Ejemplo: TituloInicial, ValorInicial
//
// 2. PROPIEDADES DE SALIDA:
//    - Se leen DESPUÉS de ShowDialog() cuando retorna true
//    - Contienen los datos introducidos por el usuario
//    - Ejemplo: Nombre, Email, Numero
//
// 3. USO CORRECTO:
//
//    var dialogo = new DialogoUsuario();
//    dialogo.PropieadEntrada = valor;  // Opcional
//    dialogo.Owner = this;
//
//    if (dialogo.ShowDialog() == true)
//    {
//        var resultado = dialogo.PropieadSalida;
//    }
//
// 4. VENTANA MODAL:
//    - ShowDialog() bloquea la ventana padre
//    - DialogResult = true/false devuelve resultado
//    - Owner = this centra el diálogo en la padre
