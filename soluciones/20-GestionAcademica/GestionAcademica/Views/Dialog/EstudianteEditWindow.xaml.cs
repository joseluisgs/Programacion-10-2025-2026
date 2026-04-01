using System.Windows;
using System.Windows.Input;
using GestionAcademica.ViewModels.Estudiantes;

namespace GestionAcademica.Views.Dialog;

public partial class EstudianteEditWindow : Window
{
    public EstudianteEditWindow(EstudianteEditViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();

        viewModel.CloseAction = result =>
        {
            DialogResult = result;
            Close();
        };
    }

    private void OnPhotoClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is EstudianteEditViewModel vm)
        {
            vm.ChangeImageCommand.Execute(null);
        }
    }
}
