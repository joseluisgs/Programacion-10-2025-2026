using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using GestionAcademica.ViewModels.Backup;

namespace GestionAcademica.Views.Backup;

public partial class BackupView : Page
{
    public BackupView()
    {
        InitializeComponent();
        var vm = App.ServiceProvider.GetRequiredService<BackupViewModel>();
        vm.Initialize();
        DataContext = vm;
    }
}