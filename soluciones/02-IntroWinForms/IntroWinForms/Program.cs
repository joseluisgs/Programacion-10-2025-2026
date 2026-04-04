using System;
using System.Windows.Forms;
using IntroWinForms.Views.Main;

namespace IntroWinForms;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        
        // Iniciamos con el MainWindow dentro del nuevo namespace de Views
        Application.Run(new MainWindow());
    }
}
