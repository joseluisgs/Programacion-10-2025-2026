using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using GestionAcademica.ViewModels.ImportExport;

namespace GestionAcademica.Views.ImportExport;

/// <summary>
/// Página de importación y exportación de datos.
/// </summary>
public partial class ImportExportView : Page
{
    /// <summary>
    /// Inicializa la vista de importación/exportación y configura el ViewModel correspondiente.
    /// </summary>
    public ImportExportView()
    {
        InitializeComponent();
        DataContext = App.ServiceProvider.GetRequiredService<ImportExportViewModel>();
    }
}