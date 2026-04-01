using System.Windows;
using System.Windows.Input;
using GestionAcademica.ViewModels.Estudiantes;

namespace GestionAcademica.Views.Dialog;

public partial class EstudianteEditWindow : Window
{
    public EstudianteEditWindow()
    {
        InitializeComponent();
    }

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

    private void OnPhotoClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is EstudianteEditViewModel vm)
        {
            vm.ChangeImageCommand.Execute(null);
        }
    }
}
