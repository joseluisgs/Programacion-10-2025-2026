using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using GestionAcademica.ViewModels.Docentes;

namespace GestionAcademica.Views.Docentes;

public partial class DocentesView : Page
{
    public DocentesView()
    {
        InitializeComponent();
        DataContext = App.ServiceProvider.GetRequiredService<DocentesViewModel>();
    }

    private void OnSearchKeyDown(object sender, KeyEventArgs e)
    {
        if (DataContext is DocentesViewModel vm)
        {
            vm.SearchCommand.Execute(null);
        }
    }

    private void OnPhotoClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is DocentesViewModel vm && vm.SelectImageCommand.CanExecute(null))
        {
            vm.SelectImageCommand.Execute(null);
        }
    }
}