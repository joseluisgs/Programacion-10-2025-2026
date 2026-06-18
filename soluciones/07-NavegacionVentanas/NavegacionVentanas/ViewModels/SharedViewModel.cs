// ============================================================
// SharedViewModel.cs - ViewModel compartido entre ventanas
// ============================================================
// Este ViewModel se usa para comunicar datos entre ventanas.
// Ambas ventanas comparten la misma instancia del ViewModel.
//
// CONCEPTO:
// - En MVVM, el ViewModel es el intermediario entre la Vista y el Modelo
// - Cuando dos vistas comparten un ViewModel, pueden comunicarse a través de él
// - Cualquier cambio en el ViewModel se refleja en todas las vistas bindeadas

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NavegacionVentanas.ViewModels;

/// <summary>
/// ViewModel compartido entre la ventana principal y secundaria.
/// Permite demostrar la comunicación entre ventanas.
/// </summary>
public class SharedViewModel : INotifyPropertyChanged
{
    // ============================================================
    // PROPIEDADES
    // ============================================================
    
    private string _mensaje = "Hola desde el ViewModel compartido";
    
    /// <summary>
    /// Mensaje que se comparte entre ventanas.
    /// </summary>
    public string Mensaje
    {
        get => _mensaje;
        set
        {
            if (_mensaje != value)
            {
                _mensaje = value;
                OnPropertyChanged();
            }
        }
    }

    private string _datoCompartido = "";
    
    /// <summary>
    /// Dato que una ventana puede escribir y otra leer.
    /// </summary>
    public string DatoCompartido
    {
        get => _datoCompartido;
        set
        {
            if (_datoCompartido != value)
            {
                _datoCompartido = value;
                OnPropertyChanged();
            }
        }
    }

    private int _contador = 0;
    
    /// <summary>
    /// Contador que se puede incrementar desde cualquier ventana.
    /// </summary>
    public int Contador
    {
        get => _contador;
        set
        {
            if (_contador != value)
            {
                _contador = value;
                OnPropertyChanged();
            }
        }
    }

    // ============================================================
    // MÉTODOS
    // ============================================================
    
    /// <summary>
    /// Incrementa el contador en 1.
    /// </summary>
    public void IncrementarContador()
    {
        Contador++;
    }

    /// <summary>
    /// Reinicia el contador a 0.
    /// </summary>
    public void ReiniciarContador()
    {
        Contador = 0;
    }

    // ============================================================
    // INotifyPropertyChanged
    // ============================================================
    
    /// <summary>
    /// Evento que se dispara cuando una propiedad cambia.
    /// Necesario para que el binding funcione correctamente.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Método que se llama cuando una propiedad cambia.
    /// </summary>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}