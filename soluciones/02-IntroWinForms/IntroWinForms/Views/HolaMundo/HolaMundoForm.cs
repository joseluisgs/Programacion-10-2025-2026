// Ejercicio 1: Primera ventana WinForms
// Mostrar una etiqueta y un botón para cerrar
namespace IntroWinForms.Views.HolaMundo;

public class HolaMundoForm : Form
{
    public HolaMundoForm()
    {
        // Configurar la ventana
        Text = "Hola Mundo";                    // Título de la ventana
        Size = new Size(400, 200);              // Ancho x Alto
        StartPosition = FormStartPosition.CenterScreen;  // Centrar en pantalla
        
        // Crear una etiqueta (Label) para mostrar texto
        var label = new Label
        {
            Text = "¡Hola Mundo!",             // Texto que muestra
            Font = new Font("Segoe UI", 24, FontStyle.Bold),  // Fuente y tamaño
            ForeColor = Color.DarkBlue,         // Color del texto
            Location = new Point(100, 30),      // Posición (x, y)
            AutoSize = true                      // Ajustar tamaño al contenido
        };
        
        // Crear un botón
        var boton = new Button
        {
            Text = "Cerrar",                   // Texto del botón
            Location = new Point(150, 80),       // Posición
            Size = new Size(100, 35)            // Tamaño
        };
        // Asignar evento Click: cuando se pulse, cerrar la ventana
        boton.Click += (_, _) => Close();

        // Añadir los controles al formulario
        Controls.AddRange([label, boton]);
    }
}
