using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using GestionAcademica.ViewModels.Informes;

namespace GestionAcademica.Views.Informes;

/// <summary>
/// Página de generación de informes.
/// </summary>
public partial class InformesView : Page
{
    /// <summary>
    /// Inicializa la vista de informes y configura el ViewModel correspondiente.
    /// </summary>
    public InformesView()
    {
        InitializeComponent();
        DataContext = App.ServiceProvider.GetRequiredService<InformesViewModel>();
    }
}