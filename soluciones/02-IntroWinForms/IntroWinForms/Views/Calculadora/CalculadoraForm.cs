// CalculadoraForm.cs - Ejercicio 2: Calculadora básica
// =====================================================
// Este ejercicio introduce:
// - TextBox: cajas de texto para introducir datos
// - ComboBox: lista desplegable para seleccionar operaciones
// - Label: etiquetas para mostrar resultados
// - Eventos: cómo responder a clics en botones
// - Conversión de texto a números: int.TryParse() / double.TryParse()

namespace IntroWinForms.Views.Calculadora;

// Declaramos una clase que hereda de Form (ventana)
// Form es la clase base de Windows Forms que proporciona toda la funcionalidad de una ventana
public class CalculadoraForm : Form
{
    // ============================================================
    // CAMPOS (FIELDS) - Variables de instancia
    // ============================================================
    // ¿Por qué necesitamos estos campos?
    // Los controles (TextBox, ComboBox, Label) se crean en el constructor,
    // pero necesitamos guardarlos en variables para usarlos después en otros métodos.
    // Si no los guardamos, el Garbage Collector los eliminará y no funcionará.

    // _txtNum1: TextBox para que el usuario introduzca el primer número
    // El modificador 'private' oculta el campo desde fuera de la clase
    // El tipo 'TextBox' es el control de Windows Forms para cajas de texto
    // '= null!' es necesario en C# con Nullable enabled: inicializamos a null pero prometemos asignar的值 antes de usar
    private TextBox _txtNum1 = null!;
    
    // _txtNum2: TextBox para el segundo número
    private TextBox _txtNum2 = null!;
    
    // _cmbOp: ComboBox (lista desplegable) para seleccionar la operación
    // ComboBox permite seleccionar un elemento de una lista o escribir el nuestro
    private ComboBox _cmbOp = null!;
    
    // _lblRes: Label donde mostraremos el resultado
    // Label es un control que solo muestra texto (no editable por el usuario)
    private Label _lblRes = null!;

    // ============================================================
    // CONSTRUCTOR - Inicializa la ventana y sus controles
    // ============================================================
    public CalculadoraForm()
    {
        // ---------------------------------------------
        // Configuración básica del formulario (ventana)
        // ---------------------------------------------
        
        // Text: título que aparece en la barra de título de la ventana
        Text = "Calculadora";
        
        // Size: dimensiones de la ventana (ancho, alto) en píxeles
        Size = new Size(350, 280);
        
        // StartPosition: dónde aparece la ventana al abrirse
        // CenterScreen la centra en el monitor actual
        StartPosition = FormStartPosition.CenterScreen;

        // ---------------------------------------------
        // CREACIÓN DE CONTROLES
        // ---------------------------------------------
        
        // Primer número - Etiqueta + TextBox
        // new Label(): creamos una nueva etiqueta
        // { ... }: inicializador de objetos - asignamos propiedades en una línea
        var lbl1 = new Label { Text = "Número 1:", Location = new Point(20, 30) };
        // Location: posición (x, y) en píxeles desde la esquina superior izquierda
        
        // TextBox: caja de texto donde el usuario escribe
        _txtNum1 = new TextBox { Location = new Point(120, 30), Width = 180 };
        // Width: ancho del control en píxeles

        // Segundo número
        var lbl2 = new Label { Text = "Número 2:", Location = new Point(20, 70) };
        _txtNum2 = new TextBox { Location = new Point(120, 70), Width = 180 };

        // Operación - Etiqueta + ComboBox
        var lblOp = new Label { Text = "Operación:", Location = new Point(20, 110) };
        
        // ComboBox: lista desplegable
        _cmbOp = new ComboBox 
        { 
            Location = new Point(120, 110), 
            Width = 180, 
            // DropDownStyle: define el comportamiento del ComboBox
            // DropDownList = solo permite seleccionar, no escribir
            DropDownStyle = ComboBoxStyle.DropDownList 
        };
        
        // Items: colección de elementos de la lista
        // AddRange(): añade varios elementos a la vez (array de strings)
        _cmbOp.Items.AddRange(["+", "-", "*", "/"]);
        
        // SelectedIndex: índice del elemento seleccionado (0 = primero)
        _cmbOp.SelectedIndex = 0;

        // Botón para calcular
        var btn = new Button 
        { 
            Text = "Calcular", 
            Location = new Point(120, 150), 
            Width = 180, 
            BackColor = Color.LightBlue  // Color de fondo del botón
        };
        
        // ---------------------------------------------
        // EVENTO Click - Qué hacer cuando se pulsa el botón
        // ---------------------------------------------
        // Sintaxis: control.Evento += manejador
        // El manejador es una lambda: (_, _) => método()
        // Primer parámetro = objeto que lanzó el evento (ignoramos con _)
        // Segundo parámetro = argumentos del evento (ignoramos con _)
        btn.Click += (_, _) => Calcular();

        // Etiqueta para mostrar el resultado
        _lblRes = new Label 
        { 
            Text = "Resultado:", 
            Location = new Point(20, 200), 
            Size = new Size(280, 40),    // Size = ancho y alto
            BorderStyle = BorderStyle.FixedSingle,  // borde visible
            BackColor = Color.LightYellow 
        };

        // ---------------------------------------------
        // AÑADIR CONTROLES AL FORMULARIO
        // ---------------------------------------------
        // Controls: colección que contiene todos los controles de la ventana
        // AddRange(): añade varios controles a la vez
        Controls.AddRange([lbl1, _txtNum1, lbl2, _txtNum2, lblOp, _cmbOp, btn, _lblRes]);
    }

    // ============================================================
    // MÉTODO Calcular - Realiza la operación matemática
    // ============================================================
    private void Calcular()
    {
        // ---------------------------------------------
        // Validación: ¿son válidos los números introducidos?
        // ---------------------------------------------
        // TryParse(): intenta convertir texto a número
        // Devuelve true si funciona y false si no
        // Si funciona, el número se guarda en la variable 'out var n1'
        if (!double.TryParse(_txtNum1.Text, out var n1) || !double.TryParse(_txtNum2.Text, out var n2))
        {
            // Si la conversión falla, mostrar mensaje de error
            _lblRes.Text = "Error: números inválidos";
            return;  // Salir del método sin continuar
        }

        // ---------------------------------------------
        // Expresión switch para realizar la operación
        // ---------------------------------------------
        // _cmbOp.SelectedItem?: accede al elemento seleccionado (puede ser null)
        // ?.ToString() convierte el objeto a string de forma segura
        // switch: evalúa el valor y devuelve según el caso
        var r = _cmbOp.SelectedItem?.ToString() switch
        {
            "+" => n1 + n2,      // Si es "+", sumar
            "-" => n1 - n2,      // Si es "-", restar
            "*" => n1 * n2,      // Si es "*", multiplicar
            // Si es "/", comprobar que no sea división por cero
            "/" => n2 != 0 ? n1 / n2 : double.NaN,
            // _: caso por defecto (cualquier otro valor)
            _ => 0.0
        };

        // Mostrar el resultado en la etiqueta
        _lblRes.Text = $"Resultado: {r}";
    }
}
