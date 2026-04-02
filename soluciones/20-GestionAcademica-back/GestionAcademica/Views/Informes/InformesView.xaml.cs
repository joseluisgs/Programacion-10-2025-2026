using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using GestionAcademica.ViewModels.Informes;

namespace GestionAcademica.Views.Informes;

public partial class InformesView : Page
{
    public InformesView()
    {
        InitializeComponent();
        DataContext = App.ServiceProvider.GetRequiredService<InformesViewModel>();
    }
}