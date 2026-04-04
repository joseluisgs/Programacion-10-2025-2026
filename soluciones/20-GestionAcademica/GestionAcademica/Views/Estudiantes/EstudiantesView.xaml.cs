using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using GestionAcademica.ViewModels.Estudiantes;

namespace GestionAcademica.Views.Estudiantes;

/// <summary>
/// Página de visualización y gestión de Estudiantes.
/// </summary>
public partial class EstudiantesView : Page
{
    /// <summary>
    /// Inicializa la vista de estudiantes y configura el ViewModel correspondiente.
    /// </summary>
    public EstudiantesView()
    {
        InitializeComponent();
        var vm = App.ServiceProvider.GetRequiredService<EstudiantesViewModel>();
        DataContext = vm;
    }

    /// <summary>
    /// Maneja el evento de doble clic sobre un estudiante para mostrar sus detalles.
    /// </summary>
    private void OnEstudianteDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is EstudiantesViewModel vm && vm.ViewCommand.CanExecute(null))
        {
            vm.ViewCommand.Execute(null);
        }
    }
}