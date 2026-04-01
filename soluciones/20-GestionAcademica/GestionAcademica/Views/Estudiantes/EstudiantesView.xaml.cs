using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using GestionAcademica.ViewModels.Estudiantes;

namespace GestionAcademica.Views.Estudiantes;

public partial class EstudiantesView : Page
{
    public EstudiantesView()
    {
        InitializeComponent();
        DataContext = App.ServiceProvider.GetRequiredService<EstudiantesViewModel>();
    }

    private void OnPhotoClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is EstudiantesViewModel vm && vm.SelectImageCommand.CanExecute(null))
        {
            vm.SelectImageCommand.Execute(null);
        }
    }
}