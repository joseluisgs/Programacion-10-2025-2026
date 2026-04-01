using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using GestionAcademica.ViewModels.Docentes;

namespace GestionAcademica.Views.Docentes;

public partial class DocentesView : Page
{
    public DocentesView()
    {
        InitializeComponent();
        var vm = App.ServiceProvider.GetRequiredService<DocentesViewModel>();
        DataContext = vm;
    }
}