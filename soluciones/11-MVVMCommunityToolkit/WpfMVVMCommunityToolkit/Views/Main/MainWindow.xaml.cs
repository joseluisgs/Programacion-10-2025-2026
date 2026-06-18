// ============================================================
// MainWindow.xaml.cs - Code-behind
// ============================================================
// Este archivo es identico al del ejercicio 10.
// La unica diferencia es el tipo de ViewModel.
//
// En MVVM, el code-behind debe tener minima logica.
// Solo necesitamos establecer el DataContext.

using System.Windows;
using WpfMVVMCommunityToolkit.ViewModels;

namespace WpfMVVMCommunityToolkit.Views.Main;

/// <summary>
/// Ventana principal de la aplicacion.
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        // Establecer el ViewModel como DataContext
        // A partir de ahora, todos los bindings buscan en ContadorViewModel
        DataContext = new ContadorViewModel();
    }
}

// ============================================================
// NOTA IMPORTANTE
// ============================================================
//
// El code-behind es EXACTAMENTE IGUAL en ambos ejercicios.
// La diferencia esta en el ViewModel:
//
// Ejercicio 10 (manual):
//   - ViewModel implementa INotifyPropertyChanged a mano
//   - RelayCommand se crea a mano
//
// Ejercicio 11 (CommunityToolkit):
//   - ViewModel hereda de ObservableObject
//   - [ObservableProperty] genera propiedades
//   - [RelayCommand] genera comandos
//
// La vista (XAML y code-behind) no necesita cambios.
// Esto demuestra que MVVM separa correctamente la UI de la logica.
//
// ============================================================
