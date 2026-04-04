using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using GestionAcademica.ViewModels.Docentes;

namespace GestionAcademica.Views.Docentes;

/// <summary>
/// Página de visualización y gestión de Docentes.
/// </summary>
public partial class DocentesView : Page
{
    /// <summary>
    /// Inicializa la vista de docentes y configura el ViewModel correspondiente.
    /// </summary>
    public DocentesView()
    {
        InitializeComponent();
        var vm = App.ServiceProvider.GetRequiredService<DocentesViewModel>();
        DataContext = vm;
    }

    /// <summary>
    /// Maneja el evento de doble clic sobre un docente para mostrar sus detalles.
    /// </summary>
    private void OnDocenteDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is DocentesViewModel vm && vm.ViewCommand.CanExecute(null))
        {
            vm.ViewCommand.Execute(null);
        }
    }
}