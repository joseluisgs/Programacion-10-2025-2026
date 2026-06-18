using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ListaCompra.Models;

public partial class Producto : ObservableObject
{
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private string _nombre = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Total))]
    private int _cantidad;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Total))]
    private decimal _precio;

    [ObservableProperty]
    private bool _estaComprado;

    [ObservableProperty]
    private DateTime _createdAt;

    [ObservableProperty]
    private DateTime _updatedAt;

    public decimal Total => Cantidad * Precio;

    public Producto() { }

    public Producto(int id, string nombre, int cantidad, decimal precio, bool estaComprado = false)
    {
        Id = id;
        Nombre = nombre;
        Cantidad = cantidad;
        Precio = precio;
        EstaComprado = estaComprado;
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
    }
}
