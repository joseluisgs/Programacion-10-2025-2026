using System.Windows;
using System.Windows.Input;
using GestionAcademica.ViewModels.Estudiantes;

namespace GestionAcademica.Views.Estudiantes;

/// <summary>
/// Ventana modal para crear o editar un Estudiante.
/// </summary>
public partial class EstudianteEditWindow : Window
{
    /// <summary>
    /// Inicializa la ventana de edición de estudiante.
    /// </summary>
    public EstudianteEditWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Configura la acción de cierre del ViewModel cuando el contenido se renderiza.
    /// </summary>
    protected override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);
        if (DataContext is EstudianteEditViewModel vm)
        {
            vm.CloseAction = result =>
            {
                DialogResult = result;
                Close();
            };
        }
    }

    /// <summary>
    /// Maneja el clic en la imagen para cambiarla.
    /// </summary>
    private void OnPhotoClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is EstudianteEditViewModel vm)
        {
            vm.ChangeImageCommand.Execute(null);
        }
    }
}
