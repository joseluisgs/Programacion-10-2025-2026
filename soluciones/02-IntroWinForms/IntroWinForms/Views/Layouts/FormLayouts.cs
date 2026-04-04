// FormLayouts.cs - Ejercicio 4: Layouts con Anchor y Dock
// =========================================================
// Este ejercicio introduce:
// - Panel: control contenedor para organizar otros controles
// - Dock: sistema de anclaje a los bordes del formulario
// - Anchor: sistema de anclaje a los bordes manteniendo distancia relativa
// - ListBox: lista de elementos seleccionables
//
// ¿Qué son los Layouts?
// Los layouts son mecanismos para posicionar los controles en la ventana.
// WinForms ofrece varios: Dock, Anchor, TableLayoutPanel, FlowLayoutPanel.

namespace IntroWinForms.Views.Layouts;

// FormLayouts hereda de Form
public class FormLayouts : Form
{
    // Constructor
    public FormLayouts()
    {
        // Configuración básica
        Text = "Layouts - Anchor y Dock";
        Size = new Size(500, 400);
        StartPosition = FormStartPosition.CenterScreen;

        // ============================================================
        // PANEL SUPERIOR (DockStyle.Top)
        // ============================================================
        // Dock: hace que el panel se "pegue" a un borde y ocupe todo el ancho
        // DockStyle.Top: pega el panel arriba y ajusta su altura
        var panelSup = new Panel 
        { 
            Dock = DockStyle.Top,    // Anclar arriba
            Height = 50,             // Altura fija de 50 píxeles
            BackColor = Color.LightBlue  // Color de fondo azul claro
        };
        
        // Añadir una etiqueta al panel superior
        // Dock = DockStyle.Fill: ocupa todo el espacio disponible del panel
        panelSup.Controls.Add(
            new Label 
            { 
                Text = "🎓 Gestión", 
                Dock = DockStyle.Fill, 
                TextAlign = ContentAlignment.MiddleCenter,  // Centrar texto
                Font = new Font("Segoe UI", 14, FontStyle.Bold)  // Fuente grande
            }
        );

        // ============================================================
        // PANEL INFERIOR (DockStyle.Bottom)
        // ============================================================
        var panelInf = new Panel 
        { 
            Dock = DockStyle.Bottom,  // Anclar abajo
            Height = 50, 
            BackColor = Color.LightGray 
        };
        
        // Añadir botones al panel inferior
        // Location: posición dentro del panel (no del formulario)
        panelInf.Controls.Add(
            new Button 
            { 
                Text = "Aceptar", 
                Location = new Point(280, 10), 
                Width = 100 
            }
        );
        panelInf.Controls.Add(
            new Button 
            { 
                Text = "Cancelar", 
                Location = new Point(390, 10), 
                Width = 100 
            }
        );

        // ============================================================
        // PANEL IZQUIERDO (DockStyle.Left)
        // ============================================================
        var panelIzq = new Panel 
        { 
            Dock = DockStyle.Left,   // Anclar a la izquierda
            Width = 100,              // Ancho fijo
            BackColor = Color.LightYellow 
        };
        
        // Añadir etiqueta con texto de menú
        // Padding: espacio interior alrededor del contenido
        panelIzq.Controls.Add(
            new Label 
            { 
                Text = "Menú\n\n📁 Inicio\n📁 Datos", 
                Dock = DockStyle.Fill, 
                Padding = new Padding(10)  // 10 píxeles de relleno
            }
        );

        // ============================================================
        // PANEL CENTRAL (Anchor - sin Dock)
        // ============================================================
        // Este panel NO usa Dock, usa Anchor para posicionamiento relativo
        
        var panelCentro = new Panel 
        { 
            BackColor = Color.White  // Fondo blanco (zona principal)
        };

        // TextBox con Anchor
        // AnchorStyles es una enumeración con los bordes: Top, Bottom, Left, Right
        // AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        // Significa: mantener distancia al borde superior, izquierdo y derecho
        var txtBus = new TextBox 
        { 
            Text = "Buscar...", 
            Location = new Point(20, 20), 
            Width = 250, 
            // El control se ajusta cuando la ventana cambia de tamaño:
            // - Mantiene 20px del borde izquierdo (Left)
            // - Mantiene 20px del borde superior (Top)
            // - Mantiene distancia al borde derecho (Right)
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right 
        };

        // ListBox con Anchor completo
        // Anchored a los 4 bordes: se expande en ambas direcciones
        var lst = new ListBox 
        { 
            Location = new Point(20, 60), 
            Size = new Size(290, 150), 
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right 
        };
        
        // Añadir elementos a la lista
        lst.Items.AddRange(["Juan - DAM1", "María - DAW1", "Luis - ASIR1"]);

        // Añadir controles al panel central
        panelCentro.Controls.AddRange([txtBus, lst]);

        // ============================================================
        // AÑADIR TODOS LOS PANELES AL FORMULARIO
        // ============================================================
        // El orden importa: el primer panel añadido queda al fondo
        // Los paneles con Dock tienen prioridad sobre los que no la tienen
        Controls.AddRange([panelCentro, panelIzq, panelInf, panelSup]);
    }
}

// ============================================================
// RESUMEN: Dock vs Anchor
// ============================================================
// DOCK (DockStyle):
// - Ocupa todo el espacio disponible en un borde
// - Ejemplo: barra de herramientas siempre pegada arriba
// - El tamaño se ajusta automáticamente al formulario
//
// ANCHOR (AnchorStyles):
// - Mantiene distancia constante a los bordes especificados
// - Ejemplo: botón en esquina que siempre está a 10px del borde derecho
// - El control cambia de tamaño pero mantiene la distancia relativa

// ============================================================
// NOTA IMPORTANTE
// ============================================================
// El panel central (sin Dock) se coloca donde queda espacio.
// Al añadir los paneles con Dock primero, el panel central ocupa
// automáticamente el espacio restante en el centro del formulario.
