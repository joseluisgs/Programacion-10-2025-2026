// ============================================================
// MainWindow.xaml.cs - Formulario de Registro con Validación Natural
// ============================================================
// Este formulario demonstra la validación natural en WPF.
//
// CONCEPTOS IMPORTANTES:
//
// 1. VALIDACIÓN NATURAL:
//    - Validación mediante eventos de los controles
//    - Se validan los datos en tiempo real mientras el usuario escribe
//    - No se necesita una arquitectura compleja (servicios, repositorios)
//    - El código está en el code-behind (simplificado para el ejemplo)
//
// 2. EVENTOS USADOS PARA VALIDACIÓN:
//    - TextChanged: Se dispara al cambiar el texto (para TextBox)
//    - PreviewTextInput: Se dispara antes de insertar texto (para validar entrada)
//    - PasswordChanged: Se dispara al cambiar la contraseña (para PasswordBox)
//    - Checked/Unchecked: Se dispara al marcar/desmarcar CheckBox
//
// 3. VALIDACIONES IMPLEMENTADAS:
//    - Nombre: Obligatorio, mínimo 3 caracteres
//    - Email: Formato válido (contiene @ y dominio)
//    - Teléfono: Solo números, exactamente 9 dígitos
//    - Edad: Número entre 18 y 120 años
//    - Contraseña: Mínimo 6 caracteres
//    - Confirmar contraseña: Debe coincidir con la contraseña
//    - Términos: Debe estar marcado para enviar
//
// 4. ESTADO DEL BOTÓN:
//    - El botón se habilita solo cuando todos los campos son válidos
//    - Esto proporciona feedback visual inmediato al usuario
//
// 5. MENSAJES DE ERROR:
//    - Se muestran en Label debajo de cada campo
//    - Se cambia el estilo del TextBox (borde rojo) cuando hay error

using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FormularioValidacion.Views.Windows;

/// <summary>
/// Ventana principal del formulario de registro.
/// </summary>
public partial class MainWindow : Window
{
    // ============================================================
    // BANDERAS DE VALIDACIÓN
    // ============================================================
    // Cada campo tiene una bandera para saber si es válido.
    // El botón de enviar solo se habilita si todos son válidos.
    
    private bool _nombreValido = false;
    private bool _emailValido = false;
    private bool _telefonoValido = false;
    private bool _edadValida = false;
    private bool _passwordValido = false;
    private bool _confirmarValido = false;
    private bool _terminosAceptados = false;

    /// <summary>
    /// Constructor de la ventana.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
    }

    // ============================================================
    // VALIDAR NOMBRE
    // ============================================================
    
    /// <summary>
    /// Se ejecuta cuando cambia el texto del nombre.
    /// Valida que el nombre no esté vacío y tenga al menos 3 caracteres.
    /// </summary>
    private void TxtNombre_TextChanged(object sender, TextChangedEventArgs e)
    {
        var nombre = TxtNombre.Text.Trim();
        
        if (string.IsNullOrEmpty(nombre))
        {
            MostrarErrorNombre("El nombre es obligatorio");
            _nombreValido = false;
        }
        else if (nombre.Length < 3)
        {
            MostrarErrorNombre("El nombre debe tener al menos 3 caracteres");
            _nombreValido = false;
        }
        else
        {
            LimpiarErrorNombre();
            _nombreValido = true;
        }
        
        ActualizarBoton();
    }

    /// <summary>
    /// Muestra el mensaje de error en el Label y cambia el estilo del TextBox.
    /// </summary>
    private void MostrarErrorNombre(string mensaje)
    {
        LblErrorNombre.Content = mensaje;
        TxtNombre.Style = (Style)FindResource("CampoError");
    }

    /// <summary>
    /// Limpia el mensaje de error y restaura el estilo normal.
    /// </summary>
    private void LimpiarErrorNombre()
    {
        LblErrorNombre.Content = "";
        TxtNombre.Style = (Style)FindResource("CampoTexto");
    }

    // ============================================================
    // VALIDAR EMAIL
    // ============================================================
    
    /// <summary>
    /// Se ejecuta cuando cambia el texto del email.
    /// Valida el formato del email (contiene @ y dominio).
    /// </summary>
    private void TxtEmail_TextChanged(object sender, TextChangedEventArgs e)
    {
        var email = TxtEmail.Text.Trim();
        
        if (string.IsNullOrEmpty(email))
        {
            MostrarErrorEmail("El email es obligatorio");
            _emailValido = false;
        }
        else if (!EsEmailValido(email))
        {
            MostrarErrorEmail("El formato del email no es válido");
            _emailValido = false;
        }
        else
        {
            LimpiarErrorEmail();
            _emailValido = true;
        }
        
        ActualizarBoton();
    }

    /// <summary>
    /// Comprueba si el email tiene un formato válido.
    /// </summary>
    private bool EsEmailValido(string email)
    {
        // Expresión regular simple para validar email
        // Debe contener: texto@texto.dominio
        var patron = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, patron);
    }

    private void MostrarErrorEmail(string mensaje)
    {
        LblErrorEmail.Content = mensaje;
        TxtEmail.Style = (Style)FindResource("CampoError");
    }

    private void LimpiarErrorEmail()
    {
        LblErrorEmail.Content = "";
        TxtEmail.Style = (Style)FindResource("CampoTexto");
    }

    // ============================================================
    // VALIDAR TELÉFONO (solo números)
    // ============================================================
    
    /// <summary>
    /// Se ejecuta antes de insertar texto en el teléfono.
    /// Solo permite dígitos (0-9).
    /// </summary>
    private void TxtTelefono_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        // Solo permitir dígitos
        e.Handled = !Regex.IsMatch(e.Text, @"^\d+$");
    }

    /// <summary>
    /// Se ejecuta cuando cambia el texto del teléfono.
    /// Valida que tenga exactamente 9 dígitos.
    /// </summary>
    private void TxtTelefono_TextChanged(object sender, TextChangedEventArgs e)
    {
        var telefono = TxtTelefono.Text.Trim();
        
        if (string.IsNullOrEmpty(telefono))
        {
            MostrarErrorTelefono("El teléfono es obligatorio");
            _telefonoValido = false;
        }
        else if (telefono.Length != 9)
        {
            MostrarErrorTelefono("El teléfono debe tener 9 dígitos");
            _telefonoValido = false;
        }
        else
        {
            LimpiarErrorTelefono();
            _telefonoValido = true;
        }
        
        ActualizarBoton();
    }

    private void MostrarErrorTelefono(string mensaje)
    {
        LblErrorTelefono.Content = mensaje;
        TxtTelefono.Style = (Style)FindResource("CampoError");
    }

    private void LimpiarErrorTelefono()
    {
        LblErrorTelefono.Content = "";
        TxtTelefono.Style = (Style)FindResource("CampoTexto");
    }

    // ============================================================
    // VALIDAR EDAD (solo números)
    // ============================================================
    
    /// <summary>
    /// Se ejecuta antes de insertar texto en la edad.
    /// Solo permite dígitos.
    /// </summary>
    private void TxtEdad_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !Regex.IsMatch(e.Text, @"^\d+$");
    }

    /// <summary>
    /// Se ejecuta cuando cambia el texto de la edad.
    /// Valida que esté entre 18 y 120 años.
    /// </summary>
    private void TxtEdad_TextChanged(object sender, TextChangedEventArgs e)
    {
        var edadTexto = TxtEdad.Text.Trim();
        
        if (string.IsNullOrEmpty(edadTexto))
        {
            MostrarErrorEdad("La edad es obligatoria");
            _edadValida = false;
        }
        else if (!int.TryParse(edadTexto, out var edad))
        {
            MostrarErrorEdad("La edad debe ser un número");
            _edadValida = false;
        }
        else if (edad < 18 || edad > 120)
        {
            MostrarErrorEdad("La edad debe estar entre 18 y 120 años");
            _edadValida = false;
        }
        else
        {
            LimpiarErrorEdad();
            _edadValida = true;
        }
        
        ActualizarBoton();
    }

    private void MostrarErrorEdad(string mensaje)
    {
        LblErrorEdad.Content = mensaje;
        TxtEdad.Style = (Style)FindResource("CampoError");
    }

    private void LimpiarErrorEdad()
    {
        LblErrorEdad.Content = "";
        TxtEdad.Style = (Style)FindResource("CampoTexto");
    }

    // ============================================================
    // VALIDAR CONTRASEÑA
    // ============================================================
    
    /// <summary>
    /// Se ejecuta cuando cambia la contraseña.
    /// Valida que tenga al menos 6 caracteres.
    /// </summary>
    private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
    {
        var password = TxtPassword.Password;
        
        if (string.IsNullOrEmpty(password))
        {
            MostrarErrorPassword("La contraseña es obligatoria");
            _passwordValido = false;
        }
        else if (password.Length < 6)
        {
            MostrarErrorPassword("La contraseña debe tener al menos 6 caracteres");
            _passwordValido = false;
        }
        else
        {
            LimpiarErrorPassword();
            _passwordValido = true;
        }
        
        // También validar confirmar contraseña si ya tiene valor
        ValidarConfirmarPassword();
        
        ActualizarBoton();
    }

    private void MostrarErrorPassword(string mensaje)
    {
        LblErrorPassword.Content = mensaje;
    }

    private void LimpiarErrorPassword()
    {
        LblErrorPassword.Content = "";
    }

    // ============================================================
    // VALIDAR CONFIRMAR CONTRASEÑA
    // ============================================================
    
    /// <summary>
    /// Se ejecuta cuando cambia la confirmación de contraseña.
    /// Valida que coincida con la contraseña.
    /// </summary>
    private void TxtConfirmarPassword_PasswordChanged(object sender, RoutedEventArgs e)
    {
        ValidarConfirmarPassword();
        ActualizarBoton();
    }

    /// <summary>
    /// Valida que la contraseña de confirmación coincida con la contraseña.
    /// </summary>
    private void ValidarConfirmarPassword()
    {
        var confirmar = TxtConfirmarPassword.Password;
        
        if (string.IsNullOrEmpty(confirmar))
        {
            MostrarErrorConfirmar("Debe confirmar la contraseña");
            _confirmarValido = false;
        }
        else if (confirmar != TxtPassword.Password)
        {
            MostrarErrorConfirmar("Las contraseñas no coinciden");
            _confirmarValido = false;
        }
        else
        {
            LimpiarErrorConfirmar();
            _confirmarValido = true;
        }
    }

    private void MostrarErrorConfirmar(string mensaje)
    {
        LblErrorConfirmar.Content = mensaje;
    }

    private void LimpiarErrorConfirmar()
    {
        LblErrorConfirmar.Content = "";
    }

    // ============================================================
    // VALIDAR TÉRMINOS
    // ============================================================
    
    /// <summary>
    /// Se ejecuta cuando se marca/desmarca el CheckBox de términos.
    /// </summary>
    private void ChkTerminos_Changed(object sender, RoutedEventArgs e)
    {
        if (ChkTerminos.IsChecked == true)
        {
            LblErrorTerminos.Content = "";
            _terminosAceptados = true;
        }
        else
        {
            LblErrorTerminos.Content = "Debe aceptar los términos y condiciones";
            _terminosAceptados = false;
        }
        
        ActualizarBoton();
    }

    // ============================================================
    // ACTUALIZAR BOTÓN ENVIAR
    // ============================================================
    
    /// <summary>
    /// Habilita el botón de enviar solo si todos los campos son válidos.
    /// </summary>
    private void ActualizarBoton()
    {
        var todoValido = _nombreValido && _emailValido && _telefonoValido && 
                         _edadValida && _passwordValido && _confirmarValido && 
                         _terminosAceptados;
        
        BtnEnviar.IsEnabled = todoValido;
        
        // Cambiar estilo según estado
        if (todoValido)
        {
            BtnEnviar.Style = (Style)FindResource("BotonEnviar");
        }
        else
        {
            BtnEnviar.Style = (Style)FindResource("BotonDeshabilitado");
        }
    }

    // ============================================================
    // BOTÓN ENVIAR
    // ============================================================
    
    /// <summary>
    /// Se ejecuta al hacer clic en el botón de enviar.
    /// Muestra un mensaje de éxito si todos los datos son válidos.
    /// </summary>
    private void BtnEnviar_Click(object sender, RoutedEventArgs e)
    {
        // Doble verificación (por seguridad)
        if (_nombreValido && _emailValido && _telefonoValido && 
            _edadValida && _passwordValido && _confirmarValido && 
            _terminosAceptados)
        {
            MessageBox.Show(
                $"¡Registro completado!\n\n" +
                $"Nombre: {TxtNombre.Text}\n" +
                $"Email: {TxtEmail.Text}\n" +
                $"Teléfono: {TxtTelefono.Text}\n" +
                $"Edad: {TxtEdad.Text} años",
                "Registro Exitoso",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }
    }
}

// ============================================================
// RESUMEN: VALIDACIÓN NATURAL EN WPF
// ============================================================
//
// EVENTOS UTILIZADOS:
// - TextChanged: Para validar texto mientras se escribe
// - PreviewTextInput: Para filtrar qué caracteres se permiten
// - PasswordChanged: Para validar PasswordBox
// - Checked/Unchecked: Para validar CheckBox
//
// TÉCNICAS DE VALIDACIÓN:
// 1. VALIDACIÓN EN TIEMPO REAL:
//    - Se valida cada campo mientras el usuario escribe
//    - Feedback inmediato (no al final)
//
// 2. ESTILO VISUAL:
//    - TextBox cambia a rojo cuando hay error
//    - Label muestra el mensaje de error debajo
//
// 3. BOTÓN DINÁMICO:
//    - Solo se habilita cuando todos los campos son válidos
//    - Feedback claro de cuándo se puede enviar
//
// VENTAJAS DE LA VALIDACIÓN NATURAL:
// - Código simple y fácil de entender
// - No requiere arquitectura compleja
// - Ideal para formularios pequeños
// - Sin dependencias externas
//
// DESVENTAJAS:
// - No escalable para formularios grandes
// - Código duplicado si hay muchos campos
// - Difícil de mantener y testar
