// FormularioRegistro.cs - Ejercicio 3: Formulario de Registro de Alumnos
// =======================================================================
// Este ejercicio introduce:
// - TextBox: cajas de texto para entrada de datos
// - ComboBox: lista desplegable para seleccionar cursos
// - Button: botones para acciones (guardar, limpiar)
// - Label: etiquetas informativas
// - MessageBox: diálogos de mensaje al usuario
// - Validación de datos antes de procesar

namespace IntroWinForms.Views.Formulario;

// FormularioRegistro hereda de Form (ventana de Windows Forms)
public class FormularioRegistro : Form
{
    // ============================================================
    // CAMPOS (FIELDS) - Controles de la ventana
    // ============================================================
    // ¿Por qué necesitamos campos?
    // Los controles se crean en el constructor, pero necesitamos
    // guardarlos para usarlos en los métodos de validación y acción.
    // Sin guardarlos, el Garbage Collector los eliminará.
    
    // _txtNombre: TextBox para introducir el nombre del alumno
    private TextBox _txtNombre = null!;
    
    // _txtEmail: TextBox para introducir el email del alumno
    private TextBox _txtEmail = null!;
    
    // _cmbCurso: ComboBox (lista desplegable) para seleccionar el curso
    private ComboBox _cmbCurso = null!;
    
    // _lblEstado: Label para mostrar mensajes de estado
    private Label _lblEstado = null!;

    // ============================================================
    // CONSTRUCTOR - Inicializa la ventana
    // ============================================================
    public FormularioRegistro()
    {
        // ---------------------------------------------
        // Configuración básica del formulario
        // ---------------------------------------------
        Text = "Registro de Alumnos";           // Título de la ventana
        Size = new Size(400, 320);              // Dimensiones (ancho, alto)
        StartPosition = FormStartPosition.CenterScreen;  // Centrar en pantalla

        // ---------------------------------------------
        // CREAR ETIQUETAS Y CONTROLES
        // ---------------------------------------------
        
        // Campo: Nombre
        // Label: etiqueta descriptiva
        var lblNom = new Label 
        { 
            Text = "Nombre:", 
            Location = new Point(20, 30),  // Posición (x=20, y=30)
            Width = 80                       // Ancho de la etiqueta
        };
        
        // TextBox: caja de texto donde el usuario escribe
        _txtNombre = new TextBox 
        { 
            Location = new Point(110, 30),   // A la derecha de la etiqueta
            Width = 250                      // Ancho del campo de texto
        };

        // Campo: Email
        var lblEmail = new Label 
        { 
            Text = "Email:", 
            Location = new Point(20, 70), 
            Width = 80 
        };
        _txtEmail = new TextBox 
        { 
            Location = new Point(110, 70), 
            Width = 250 
        };

        // Campo: Curso (ComboBox - lista desplegable)
        var lblCur = new Label 
        { 
            Text = "Curso:", 
            Location = new Point(20, 110), 
            Width = 80 
        };
        
        _cmbCurso = new ComboBox 
        { 
            Location = new Point(110, 110), 
            Width = 250, 
            // DropDownStyle = DropDownList: solo permite seleccionar de la lista
            // El usuario NO puede escribir un valor personalizado
            DropDownStyle = ComboBoxStyle.DropDownList 
        };
        
        // Añadir cursos disponibles a la lista
        _cmbCurso.Items.AddRange(["DAM1", "DAM2", "DAW1", "DAW2", "ASIR1", "ASIR2"]);
        
        // SelectedIndex = 0: seleccionar el primer curso por defecto
        _cmbCurso.SelectedIndex = 0;

        // ---------------------------------------------
        // BOTONES
        // ---------------------------------------------
        
        // Botón Guardar: registra al alumno
        var btnGuardar = new Button 
        { 
            Text = "Guardar", 
            Location = new Point(110, 170),  // Debajo de los campos
            Width = 120,                      // Ancho
            Height = 35,                       // Alto
            BackColor = Color.LightGreen      // Color de fondo verde
        };
        
        // Evento Click: al pulsar, ejecutar el método Guardar()
        btnGuardar.Click += (_, _) => Guardar();

        // Botón Limpiar: borra todos los campos
        var btnLimpiar = new Button 
        { 
            Text = "Limpiar", 
            Location = new Point(240, 170),   // A la derecha del botón Guardar
            Width = 120, 
            Height = 35, 
            BackColor = Color.LightCoral      // Color de fondo rojo/coral
        };
        
        // Evento Click: al pulsar, ejecutar el método Limpiar()
        btnLimpiar.Click += (_, _) => Limpiar();

        // ---------------------------------------------
        // ETIQUETA DE ESTADO
        // ---------------------------------------------
        _lblEstado = new Label 
        { 
            Text = "Estado: Listo", 
            Location = new Point(20, 230), 
            Size = new Size(340, 35),         // Tamaño (ancho, alto)
            BorderStyle = BorderStyle.FixedSingle  // Borde visible
        };

        // ---------------------------------------------
        // AÑADIR CONTROLES AL FORMULARIO
        // ---------------------------------------------
        Controls.AddRange([lblNom, _txtNombre, lblEmail, _txtEmail, lblCur, _cmbCurso, btnGuardar, btnLimpiar, _lblEstado]);
    }

    // ============================================================
    // MÉTODO Guardar - Valida y procesa el registro
    // ============================================================
    private void Guardar()
    {
        // ---------------------------------------------
        // VALIDACIÓN: Comprobar que el nombre no esté vacío
        // ---------------------------------------------
        // string.IsNullOrWhiteSpace(): devuelve true si el string es null, vacío o solo espacios
        if (string.IsNullOrWhiteSpace(_txtNombre.Text))
        {
            // MessageBox: mostrar diálogo de error
            // Parámetros: mensaje, título, botones, icono
            MessageBox.Show(
                "El nombre es obligatorio",  // Mensaje
                "Error",                       // Título
                MessageBoxButtons.OK,         // Solo botón Aceptar
                MessageBoxIcon.Warning        // Icono de advertencia
            );
            return;  // Salir sin guardar
        }

        // ---------------------------------------------
        // VALIDACIÓN: Comprobar que el email sea válido
        // ---------------------------------------------
        // Conditions:
        // 1. No esté vacío
        // 2. Contenga el carácter '@'
        if (string.IsNullOrWhiteSpace(_txtEmail.Text) || !_txtEmail.Text.Contains('@'))
        {
            MessageBox.Show(
                "Email inválido", 
                "Error", 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Warning
            );
            return;
        }

        // ---------------------------------------------
        // REGISTRO EXITOSO
        // ---------------------------------------------
        // Si llegamos aquí, todos los datos son válidos
        MessageBox.Show(
            $"Alumno {_txtNombre.Text} registrado",  // Mensaje con el nombre
            "Éxito",                                   // Título
            MessageBoxButtons.OK,                     // Botón Aceptar
            MessageBoxIcon.Information                 // Icono de información
        );
        
        // Actualizar etiqueta de estado
        _lblEstado.Text = "Estado: Registrado";
    }

    // ============================================================
    // MÉTODO Limpiar - Borra todos los campos
    // ============================================================
    private void Limpiar()
    {
        // Clear(): método de TextBox que borra todo el texto
        _txtNombre.Clear();
        _txtEmail.Clear();
        
        // SelectedIndex = 0: volver al primer curso
        _cmbCurso.SelectedIndex = 0;
        
        // Actualizar etiqueta de estado
        _lblEstado.Text = "Estado: Limpiado";
    }
}
