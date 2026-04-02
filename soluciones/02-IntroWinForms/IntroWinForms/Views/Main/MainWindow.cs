using System;
using System.Diagnostics;
using System.Windows.Forms;
using IntroWinForms.Views.HolaMundo;
using IntroWinForms.Views.Calculadora;
using IntroWinForms.Views.Formulario;
using IntroWinForms.Views.Layouts;

namespace IntroWinForms.Views.Main;

/// <summary>
/// Ventana principal de introducción a WinForms.
/// Actúa como lanzador de los diferentes ejercicios.
/// </summary>
public partial class MainWindow : Form
{
    private Button btnHolaMundo = null!;
    private Button btnCalculadora = null!;
    private Button btnFormulario = null!;
    private Button btnLayouts = null!;
    private Label lblTitulo = null!;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.btnHolaMundo = new Button();
        this.btnCalculadora = new Button();
        this.btnFormulario = new Button();
        this.btnLayouts = new Button();
        this.lblTitulo = new Label();

        this.SuspendLayout();

        // Titulo
        this.lblTitulo.AutoSize = true;
        this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
        this.lblTitulo.Location = new System.Drawing.Point(50, 20);
        this.lblTitulo.Text = "Intro a Windows Forms";

        // Boton Hola Mundo
        this.btnHolaMundo.Location = new System.Drawing.Point(100, 80);
        this.btnHolaMundo.Size = new System.Drawing.Size(200, 40);
        this.btnHolaMundo.Text = "1. Hola Mundo";
        this.btnHolaMundo.Click += (s, e) => new HolaMundoForm().ShowDialog();

        // Boton Calculadora
        this.btnCalculadora.Location = new System.Drawing.Point(100, 130);
        this.btnCalculadora.Size = new System.Drawing.Size(200, 40);
        this.btnCalculadora.Text = "2. Calculadora";
        this.btnCalculadora.Click += (s, e) => new CalculadoraForm().ShowDialog();

        // Boton Formulario
        this.btnFormulario.Location = new System.Drawing.Point(100, 180);
        this.btnFormulario.Size = new System.Drawing.Size(200, 40);
        this.btnFormulario.Text = "3. Formulario";
        this.btnFormulario.Click += (s, e) => new FormularioRegistro().ShowDialog();

        // Boton Layouts
        this.btnLayouts.Location = new System.Drawing.Point(100, 230);
        this.btnLayouts.Size = new System.Drawing.Size(200, 40);
        this.btnLayouts.Text = "4. Layouts";
        this.btnLayouts.Click += (s, e) => new FormLayouts().ShowDialog();

        // Form properties
        this.ClientSize = new System.Drawing.Size(400, 320);
        this.Controls.Add(this.lblTitulo);
        this.Controls.Add(this.btnHolaMundo);
        this.Controls.Add(this.btnCalculadora);
        this.Controls.Add(this.btnFormulario);
        this.Controls.Add(this.btnLayouts);
        this.Text = "Principal - WinForms";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;

        this.ResumeLayout(false);
        this.PerformLayout();
    }
}
