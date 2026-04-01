using System.Windows;
using LayoutsComponentes.Views.Ejercicio1_Grid;
using LayoutsComponentes.Views.Ejercicio2_StackDock;
using LayoutsComponentes.Views.Ejercicio3_WrapCanvas;
using LayoutsComponentes.Views.Ejercicio4_FormComponents;
using LayoutsComponentes.Views.Ejercicio5_Selection;
using LayoutsComponentes.Views.Ejercicio6_DataGrid;
using LayoutsComponentes.Views.Ejercicio7_TabsExpander;
using LayoutsComponentes.Views.Ejercicio8_Resources;

namespace LayoutsComponentes.Views.Main;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void BtnGrid_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new GridWindow();
        ventana.ShowDialog();
    }

    private void BtnStackDock_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new StackDockWindow();
        ventana.ShowDialog();
    }

    private void BtnWrapCanvas_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new WrapCanvasWindow();
        ventana.ShowDialog();
    }

    private void BtnFormComponents_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new FormComponentsWindow();
        ventana.ShowDialog();
    }

    private void BtnSelection_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new SelectionWindow();
        ventana.ShowDialog();
    }

    private void BtnDataGrid_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new DataGridWindow();
        ventana.ShowDialog();
    }

    private void BtnTabsExpander_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new TabsExpanderWindow();
        ventana.ShowDialog();
    }

    private void BtnResources_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new ResourcesWindow();
        ventana.ShowDialog();
    }

    private void BtnSalir_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}
