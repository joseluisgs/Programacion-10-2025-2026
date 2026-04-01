using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using GestionAcademica.ViewModels.ImportExport;

namespace GestionAcademica.Views.ImportExport;

public partial class ImportExportView : Page
{
    public ImportExportView()
    {
        InitializeComponent();
        DataContext = App.ServiceProvider.GetRequiredService<ImportExportViewModel>();
    }
}