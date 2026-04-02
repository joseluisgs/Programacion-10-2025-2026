using System.Windows;
using GestionProductos.Models;

namespace GestionProductos.Views.Dialog;

public partial class ProductoDialog : Window
{
    public string Nombre => TxtNombre.Text.Trim();
    public string Descripcion => TxtDescripcion.Text.Trim();
    public string Categoria => CmbCategoria.Text.Trim();
    public decimal Precio => decimal.TryParse(TxtPrecio.Text, out var p) ? p : 0;
    public int Stock => int.TryParse(TxtStock.Text, out var s) ? s : 0;

    public ProductoDialog()
    {
        InitializeComponent();
        Title = "Añadir Producto";
        TxtNombre.Focus();
    }

    public ProductoDialog(Producto producto) : this()
    {
        Title = "Editar Producto";
        TxtNombre.Text = producto.Nombre;
        TxtDescripcion.Text = producto.Descripcion;
        CmbCategoria.Text = producto.Categoria;
        TxtPrecio.Text = producto.Precio.ToString();
        TxtStock.Text = producto.Stock.ToString();
    }

    private void BtnAceptar_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Nombre))
        {
            MessageBox.Show("El nombre es obligatorio", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            TxtNombre.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(Categoria))
        {
            MessageBox.Show("La categoría es obligatoria", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            CmbCategoria.Focus();
            return;
        }

        if (Precio < 0)
        {
            MessageBox.Show("El precio no puede ser negativo", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            TxtPrecio.Focus();
            return;
        }

        if (Stock < 0)
        {
            MessageBox.Show("El stock no puede ser negativo", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            TxtStock.Focus();
            return;
        }

        DialogResult = true;
        Close();
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
