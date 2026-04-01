using System.Windows;
using System.Windows.Input;
using GestionAcademica.ViewModels.Docentes;

namespace GestionAcademica.Views.Dialog;

public partial class DocenteEditWindow : Window
{
    public DocenteEditWindow(DocenteEditViewModel viewModel)
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
        if (DataContext is DocenteEditViewModel vm)
        {
            vm.ChangeImageCommand.Execute(null);
        }
    }
}
