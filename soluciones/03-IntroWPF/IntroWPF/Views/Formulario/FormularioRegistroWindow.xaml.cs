// FormularioRegistroWindow.xaml.cs - Ejercicio 3: Formulario de Registro
// =======================================================================
// Este ejercicio introduce:
// - TextBox: campos de entrada de texto
// - ComboBox: lista desplegable para seleccionar cursos
// - Button: botones con eventos Click
// - StackPanel: panel que apila controles horizontal o verticalmente
// - MessageBox: diálogos de mensaje
// - Validación de datos

using System.Windows;
using System.Windows.Controls;

namespace IntroWPF.Views.Formulario;

public partial class FormularioRegistroWindow : Window
{
    public FormularioRegistroWindow()
    {
        InitializeComponent();
    }

    // ============================================================
    // EVENTO Click del botón Guardar
    // ============================================================
    private void BtnGuardar_Click(object sender, RoutedEventArgs e)
    {
        // ---------------------------------------------
        // VALIDACIÓN: Nombre no vacío
        // ---------------------------------------------
        // string.IsNullOrWhiteSpace(): true si es null, vacío o solo espacios
        if (string.IsNullOrWhiteSpace(TxtNombre.Text))
        {
            MessageBox.Show(
                "El nombre es obligatorio",  // Mensaje
                "Error",                      // Título
                MessageBoxButton.OK,         // Botón Aceptar
                MessageBoxImage.Warning      // Icono de advertencia
            );
            return;
        }

        // ---------------------------------------------
        // VALIDACIÓN: Email válido
        // ---------------------------------------------
        // Dos condiciones:
        // 1. No esté vacío
        // 2. Contenga el carácter '@'
        if (string.IsNullOrWhiteSpace(TxtEmail.Text) || 
            !TxtEmail.Text.Contains('@'))
        {
            MessageBox.Show(
                "Email inválido", 
                "Error", 
                MessageBoxButton.OK, 
                MessageBoxImage.Warning
            );
            return;
        }

        // ---------------------------------------------
        // REGISTRO EXITOSO
        // ---------------------------------------------
        // Interpolación de strings: $"texto {variable}"
        MessageBox.Show(
            $"Alumno {TxtNombre.Text} registrado",  // Mensaje
            "Éxito",                                   // Título
            MessageBoxButton.OK,                      // Botón Aceptar
            MessageBoxImage.Information                // Icono de información
        );

        // Actualizar etiqueta de estado
        LblEstado.Text = "Estado: Registrado";
    }

    // ============================================================
    // EVENTO Click del botón Limpiar
    // ============================================================
    private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
    {
        // Clear(): método de TextBox que borra todo el texto
        TxtNombre.Clear();
        TxtEmail.Clear();
        
        // SelectedIndex = 0: seleccionar el primer curso
        CmbCurso.SelectedIndex = 0;
        
        // Actualizar etiqueta de estado
        LblEstado.Text = "Estado: Limpiado";
    }
}
