// Persona.cs - Ejercicio 4: INotifyPropertyChanged
// ==================================================
// Este ejercicio muestra la interfaz estándar de .NET
// para implementar propiedades reactivas (data binding).
//
// ¿Qué es INotifyPropertyChanged?
// Es una interfaz de System.ComponentModel que permite a los objetos
// notificar a otros cuando una propiedad cambia.
//
// ¿Por qué es importante?
// Es la base del Data Binding en WPF, WinForms y Xamarin.
// Permite que la UI se actualice automáticamente cuando cambian los datos.
//
// Ejemplo real: En una aplicación de banca, cuando cambia el saldo,
// la interfaz debe mostrar el nuevo valor sin código adicional.
using System.ComponentModel;

namespace PatronObserver.Ejercicio4;

// ============================================================
// CLASE PERSONA
// ============================================================
// Implementa INotifyPropertyChanged para notificar cambios de propiedades
// Usamos primary constructor (C# 12+) con parámetro nombreInicial
public class Persona(string nombreInicial) : INotifyPropertyChanged
{
    // ============================================================
    // CAMPO: Almacena el valor real del nombre
    // ============================================================
    // _nombre es el campo privado que guarda el valor
    // No es accesible directamente desde fuera de la clase
    private string _nombre = nombreInicial;

    // ============================================================
    // EVENTO: PropertyChanged
    // ============================================================
    // Evento que se dispara cuando cualquier propiedad cambia.
    // Los suscriptores (como la UI) escuchan este evento para saber qué cambió.
    //
    // PropertyChangedEventHandler es el delegate estándar:
    // - object? sender: el objeto que cambió
    // - PropertyChangedEventArgs: contiene información del cambio (nombre de la propiedad)
    public event PropertyChangedEventHandler? PropertyChanged;

    // ============================================================
    // PROPIEDAD: Nombre
    // ============================================================
    // Las propiedades en C# tienen get (lectura) y set (escritura)
    // Aquí controlamos explícitamente cuándo se puede escribir
    
    public string Nombre
    {
        // get: devuelve el valor actual
        get => _nombre;
        
        // set: asigna un nuevo valor
        set
        {
            // ============================================================
            // IMPORTANTE: Solo notificar si el valor realmente cambió
            // ============================================================
            // Comparamos el valor nuevo con el actual
            if (_nombre != value)
            {
                // Asignar el nuevo valor
                _nombre = value;
                
                // ============================================================
                // DISPARAR EL EVENTO DE CAMBIO
                // ============================================================
                // Esto notifica a todos los suscriptores que la propiedad cambió
                //
                // PropertyChanged?.Invoke():
                // - ?: solo ejecuta si hay alguien suscrito
                // - this: referencia a este objeto (Persona)
                // - new PropertyChangedEventArgs(...): indica QUÉ propiedad cambió
                // - nameof(Nombre): expresión que da el nombre "Nombre" como string
                PropertyChanged?.Invoke(
                    this, 
                    new PropertyChangedEventArgs(nameof(Nombre))
                );
            }
            // Si el valor es igual, NO disparamos el evento (optimización)
        }
    }
}

// ============================================================
// CLASE PersonaView (Vista de Persona)
// ============================================================
// Simula una vista (como un control de UI) que muestra los cambios
public class PersonaView
{
    // Constructor que recibe la persona a observar
    public PersonaView(Persona persona)
    {
        // ============================================================
        // SUSCRIBIRSE AL EVENTO PropertyChanged
        // ============================================================
        // Usamos una lambda como manejador del evento
        // Cuando la propiedad cambie, esta lambda se ejecutará
        
        persona.PropertyChanged += (sender, e) =>
        {
            // sender: el objeto que cambió (cast a Persona para acceder a sus propiedades)
            // e.PropertyName: nombre de la propiedad que cambió
            if (sender is Persona p)
                Console.WriteLine($"🔄 La propiedad '{e.PropertyName}' cambió a: {p.Nombre}");
        };
    }
}
