// ============================================================
// MainWindow.xaml.cs - Ejemplos de diálogos en WPF
// ============================================================
// Este código muestra cómo usar los distintos diálogos en WPF.
//
// DIÁLOGOS PREDEFINIDOS:
// - MessageBox: mensajes simples
// - OpenFileDialog: abrir archivos
// - SaveFileDialog: guardar archivos
// - FolderBrowserDialog: seleccionar carpetas
//
// DIÁLOGOS PERSONALIZADOS:
// - Ventanas modales con ShowDialog()
// - Paso de datos por constructor
// - Devolución de resultados con DialogResult

using System;
using System.IO;
using System.Windows;
using Dialogos.Views.Dialogs;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;

namespace Dialogos.Views.Main;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    // ============================================================
    // 1. MESSAGEBOX - Diálogos simples
    // ============================================================
    
    /// <summary>
    /// MessageBox de información.
    /// </summary>
    private void BtnMessageBoxInfo_Click(object sender, RoutedEventArgs e)
    {
        System.Windows.MessageBox.Show(
            "Operación completada correctamente.\n\nEste es un MessageBox de información.",
            "Información",
            MessageBoxButton.OK,
            MessageBoxImage.Information
        );
        
        TxtResultado.Text = "MessageBox: Información mostrado";
    }

    /// <summary>
    /// MessageBox de advertencia.
    /// </summary>
    private void BtnMessageBoxWarning_Click(object sender, RoutedEventArgs e)
    {
        System.Windows.MessageBox.Show(
            "¡Atención! Los datos no se han guardado.\n¿Desea continuar?",
            "Advertencia",
            MessageBoxButton.OK,
            MessageBoxImage.Warning
        );
        
        TxtResultado.Text = "MessageBox: Advertencia mostrado";
    }

    /// <summary>
    /// MessageBox de error.
    /// </summary>
    private void BtnMessageBoxError_Click(object sender, RoutedEventArgs e)
    {
        System.Windows.MessageBox.Show(
            "Error al conectar con el servidor.\n\nPor favor, inténtelo más tarde.",
            "Error",
            MessageBoxButton.OK,
            MessageBoxImage.Error
        );
        
        TxtResultado.Text = "MessageBox: Error mostrado";
    }

    /// <summary>
    /// MessageBox de pregunta (con respuesta).
    /// </summary>
    private void BtnMessageBoxQuestion_Click(object sender, RoutedEventArgs e)
    {
        var resultado = System.Windows.MessageBox.Show(
            "¿Desea guardar los cambios antes de salir?",
            "Confirmar",
            MessageBoxButton.YesNoCancel,
            MessageBoxImage.Question
        );
        
        string respuesta = resultado switch
        {
            MessageBoxResult.Yes => "Sí",
            MessageBoxResult.No => "No",
            MessageBoxResult.Cancel => "Cancelar",
            _ => "Ninguna"
        };
        
        TxtResultado.Text = $"MessageBox: Respuesta del usuario = {respuesta}";
    }

    // ============================================================
    // 2. OPENFILEDIALOG - Abrir archivos
    // ============================================================
    
    /// <summary>
    /// Abrir un archivo.
    /// </summary>
    private void BtnOpenFile_Click(object sender, RoutedEventArgs e)
    {
        var dialogo = new Microsoft.Win32.OpenFileDialog
        {
            Title = "Seleccionar archivo",
            Filter = "Archivos de texto|*.txt|Todos los archivos|*.*",
            FilterIndex = 1,
            CheckFileExists = true,
            CheckPathExists = true
        };
        
        if (dialogo.ShowDialog() == true)
        {
            var archivo = dialogo.FileName;
            var contenido = File.ReadAllText(archivo);
            
            TxtResultado.Text = $"Archivo abierto: {archivo}\n\nContenido (primeros 100 chars):\n{contenido.Substring(0, Math.Min(100, contenido.Length))}...";
        }
        else
        {
            TxtResultado.Text = "OpenFileDialog: Cancelado por el usuario";
        }
    }

    /// <summary>
    /// Abrir múltiples archivos.
    /// </summary>
    private void BtnOpenMultipleFiles_Click(object sender, RoutedEventArgs e)
    {
        var dialogo = new Microsoft.Win32.OpenFileDialog
        {
            Title = "Seleccionar múltiples archivos",
            Filter = "Imágenes|*.jpg;*.jpeg;*.png;*.gif|Todos los archivos|*.*",
            Multiselect = true
        };
        
        if (dialogo.ShowDialog() == true)
        {
            var archivos = dialogo.FileNames;
            var total = archivos.Length;
            
            TxtResultado.Text = $"Archivos seleccionados: {total}\n\n";
            foreach (var archivo in archivos)
            {
                TxtResultado.Text += $"• {Path.GetFileName(archivo)}\n";
            }
        }
        else
        {
            TxtResultado.Text = "OpenFileDialog (múltiple): Cancelado";
        }
    }

    /// <summary>
    /// Abrir una imagen.
    /// </summary>
    private void BtnOpenImage_Click(object sender, RoutedEventArgs e)
    {
        var dialogo = new Microsoft.Win32.OpenFileDialog
        {
            Title = "Seleccionar imagen",
            Filter = "Imágenes|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Todos los archivos|*.*",
            CheckFileExists = true
        };
        
        if (dialogo.ShowDialog() == true)
        {
            var archivo = dialogo.FileName;
            var info = new FileInfo(archivo);
            
            TxtResultado.Text = $"Imagen seleccionada:\n" +
                               $"Nombre: {info.Name}\n" +
                               $"Tamaño: {info.Length / 1024.0:F2} KB\n" +
                               $"Ruta: {archivo}";
        }
        else
        {
            TxtResultado.Text = "OpenFileDialog (imagen): Cancelado";
        }
    }

    // ============================================================
    // 3. SAVEFILEDIALOG - Guardar archivos
    // ============================================================
    
    /// <summary>
    /// Guardar archivo (sugiere nombre).
    /// </summary>
    private void BtnSaveFile_Click(object sender, RoutedEventArgs e)
    {
        var dialogo = new Microsoft.Win32.SaveFileDialog
        {
            Title = "Guardar archivo",
            Filter = "Archivos de texto|*.txt|Todos los archivos|*.*",
            FilterIndex = 1,
            DefaultExt = "txt",
            FileName = "documento"
        };
        
        if (dialogo.ShowDialog() == true)
        {
            File.WriteAllText(dialogo.FileName, "Contenido guardado desde WPF");
            
            TxtResultado.Text = $"Archivo guardado:\n{dialogo.FileName}";
        }
        else
        {
            TxtResultado.Text = "SaveFileDialog: Cancelado";
        }
    }

    /// <summary>
    /// Guardar como (siempre pregunta).
    /// </summary>
    private void BtnSaveFileAs_Click(object sender, RoutedEventArgs e)
    {
        var dialogo = new Microsoft.Win32.SaveFileDialog
        {
            Title = "Guardar como...",
            Filter = "Documento PDF|*.pdf|Word|*.docx|Todos los archivos|*.*",
            FilterIndex = 2,
            DefaultExt = "docx",
            FileName = "nuevo_documento"
        };
        
        if (dialogo.ShowDialog() == true)
        {
            TxtResultado.Text = $"Guardar como:\n{dialogo.FileName}\n\n(Filtro seleccionado: {dialogo.FilterIndex})";
        }
        else
        {
            TxtResultado.Text = "SaveFileDialog (Guardar como): Cancelado";
        }
    }

    // ============================================================
    // 4. FOLDERBROWSERDIALOG - Seleccionar carpetas
    // ============================================================
    
    /// <summary>
    /// Seleccionar carpeta.
    /// </summary>
    private void BtnSelectFolder_Click(object sender, RoutedEventArgs e)
    {
        var dialogo = new FolderBrowserDialog
        {
            Description = "Seleccionar carpeta de destino",
            UseDescriptionForTitle = true,
            ShowNewFolderButton = true
        };
        
        if (dialogo.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            var carpeta = dialogo.SelectedPath;
            var archivos = Directory.GetFiles(carpeta).Length;
            var carpetas = Directory.GetDirectories(carpeta).Length;
            
            TxtResultado.Text = $"Carpeta seleccionada:\n{carpeta}\n\n" +
                               $"Archivos: {archivos}\n" +
                               $"Subcarpetas: {carpetas}";
        }
        else
        {
            TxtResultado.Text = "FolderBrowserDialog: Cancelado";
        }
    }

    // ============================================================
    // 5. DIÁLOGOS PERSONALIZADOS
    // ============================================================
    
    /// <summary>
    /// Diálogo de usuario (personalizado).
    /// </summary>
    private void BtnDialogoUsuario_Click(object sender, RoutedEventArgs e)
    {
        var dialogo = new DialogoUsuario();
        dialogo.Owner = this;
        
        if (dialogo.ShowDialog() == true)
        {
            var nombre = dialogo.Nombre;
            var email = dialogo.Email;
            
            TxtResultado.Text = $"Nuevo usuario:\n" +
                               $"Nombre: {nombre}\n" +
                               $"Email: {email}";
        }
        else
        {
            TxtResultado.Text = "Diálogo de usuario: Cancelado";
        }
    }

    /// <summary>
    /// Diálogo de número.
    /// </summary>
    private void BtnDialogoNumero_Click(object sender, RoutedEventArgs e)
    {
        var dialogo = new DialogoNumero(50, 0, 100);
        dialogo.Owner = this;
        
        if (dialogo.ShowDialog() == true)
        {
            var numero = dialogo.Numero;
            
            TxtResultado.Text = $"Número seleccionado: {numero}";
        }
        else
        {
            TxtResultado.Text = "Diálogo de número: Cancelado";
        }
    }

    /// <summary>
    /// Diálogo personalizado completo.
    /// </summary>
    private void BtnDialogoPersonalizado_Click(object sender, RoutedEventArgs e)
    {
        var dialogo = new DialogoPersonalizado("Mi Título");
        dialogo.Owner = this;
        
        if (dialogo.ShowDialog() == true)
        {
            TxtResultado.Text = $"Datos del diálogo:\n" +
                               $"Título: {dialogo.TituloResultado}\n" +
                               $"Descripción: {dialogo.DescripcionResultado}\n" +
                               $"Confirmado: {dialogo.Confirmado}";
        }
        else
        {
            TxtResultado.Text = "Diálogo personalizado: Cancelado";
        }
    }
}

// ============================================================
// RESUMEN: TIPOS DE DIÁLOGOS EN WPF
// ============================================================
//
// MESSAGEBOX:
// - MessageBox.Show(mensaje, titulo, botones, icono)
// - Botones: OK, OKCancel, YesNo, YesNoCancel, RetryCancel, AbortRetryIgnore
// - Icono: Information, Warning, Error, Question, None
// - Retorna: MessageBoxResult (Yes, No, OK, Cancel, etc.)
//
// OPENFILEDIALOG:
// - OpenFileDialog.ShowDialog() retorna bool?
// - FileName: ruta del archivo seleccionado
// - FileNames: array de archivos (si Multiselect=true)
// - Filter: filtro de archivos ("Texto|*.txt|Imágenes|*.jpg")
// - FilterIndex: índice del filtro por defecto
//
// SAVEFILEDIALOG:
// - Similar a OpenFileDialog
// - DefaultExt: extensión por defecto
// - FileName: nombre de archivo sugerido
//
// FOLDERBROWSERDIALOG:
// - Requiere referencia a System.Windows.Forms
// - ShowDialog() retorna DialogResult
// - SelectedPath: ruta de la carpeta seleccionada
// - Description: texto de descripción
// - ShowNewFolderButton: mostrar botón "Nueva carpeta"
//
// DIÁLOGOS PERSONALIZADOS:
// - Window con WindowStyle="ToolWindow" o normales
// - ShowDialog() para abrir como modal
// - DialogResult = true/false para devolver resultado
// - Propiedades públicas para get/set de datos
