using System.Windows;
using System.Windows.Input;
using GestionAcademica.ViewModels.Docentes;

namespace GestionAcademica.Views.Docentes;

/// <summary>
/// Ventana modal para crear o editar un Docente.
/// </summary>
public partial class DocenteEditWindow : Window
{
    /// <summary>
    /// Inicializa la ventana de edición de docente.
    /// </summary>
    public DocenteEditWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Configura la acción de cierre del ViewModel cuando el contenido se renderiza.
    /// </summary>
    protected override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);
        if (DataContext is DocenteEditViewModel vm)
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
        if (DataContext is DocenteEditViewModel vm)
        {
            vm.ChangeImageCommand.Execute(null);
        }
    }
}
