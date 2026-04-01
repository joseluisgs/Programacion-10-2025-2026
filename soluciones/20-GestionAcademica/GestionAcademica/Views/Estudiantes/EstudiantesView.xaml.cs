using System.Windows.Controls;
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
}