using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using GestionAcademica.ViewModels.Graficos;

namespace GestionAcademica.Views.Graficos;

public partial class GraficosView : Page
{
    private readonly GraficosViewModel _viewModel;

    public GraficosView()
    {
        InitializeComponent();
        _viewModel = App.ServiceProvider.GetRequiredService<GraficosViewModel>();
        DataContext = _viewModel;
    }
}