using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using GestionAcademica.ViewModels.Backup;

namespace GestionAcademica.Views.Backup;

/// <summary>
/// Página de gestión de copias de seguridad.
/// </summary>
public partial class BackupView : Page
{
    /// <summary>
    /// Inicializa la vista de backups y configura el ViewModel correspondiente.
    /// </summary>
    public BackupView()
    {
        InitializeComponent();
        var vm = App.ServiceProvider.GetRequiredService<BackupViewModel>();
        DataContext = vm;
    }
}