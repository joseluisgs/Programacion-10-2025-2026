using System.Windows;
using System.Windows.Input;
using GestionAcademica.ViewModels.Docentes;

namespace GestionAcademica.Views.Dialog;

public partial class DocenteEditWindow : Window
{
    public DocenteEditWindow()
    {
        InitializeComponent();
    }

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

    private void OnPhotoClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is DocenteEditViewModel vm)
        {
            vm.ChangeImageCommand.Execute(null);
        }
    }
}
