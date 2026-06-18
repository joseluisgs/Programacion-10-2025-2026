// IObservable.cs - Ejercicio 1: Patrón Observer con Interfaces
// =============================================================
// Este ejercicio muestra la implementación básica del Patrón Observer
// de forma manual, sin usar eventos de C#.
//
// ¿Qué es el Patrón Observer?
// Es un patrón de diseño donde un objeto (publicador/sujeto) mantiene
// una lista de dependientes (observadores/suscriptores) y los notifica
// automáticamente cuando ocurre un cambio.
//
// Ejemplo real: YouTube - cuando te suscribes a un canal, recibes notificaciones
// cuando publican un nuevo vídeo.

namespace PatronObserver.Ejercicio1;

// ============================================================
// INTERFAZ IObservable (Sujeto/Publicador)
// ============================================================
// Interfaz que define las operaciones del publicador
// Cualquier clase que implemente esta interfaz puede tener suscriptores
public interface IObservable
{
    // Añadir un observador a la lista de suscriptores
    void AddObserver(IObserver observer);
    
    // Eliminar un observador de la lista
    void RemoveObserver(IObserver observer);
    
    // Notificar a todos los observadores con un mensaje
    void NotifyObservers(string message);
}

// ============================================================
// INTERFAZ IObserver (Suscriptor/Observador)
// ============================================================
// Interfaz que deben implementar los observadores
// Define el método que será llamado cuando haya cambios
public interface IObserver
{
    // Método que será llamado cuando el publicador envíe una notificación
    // string message: el mensaje que envía el publicador
    void Update(string message);
}

// ============================================================
// CLASE CAFETERÍA (Publicador)
// ============================================================
// Implementa IObservable - es el objeto que tiene suscriptores
// En este caso, la cafetería informa a los clientes cuando el café está listo
public class Cafeteria : IObservable
{
    // ============================================================
    // CAMPO: Lista de observadores
    // ============================================================
    // Lista que guarda todos los observadores suscritos
    // readonly: no se puede asignar otra lista, pero sí modificar su contenido
    // List<IObserver>: colección tipada de observadores
    // [] inicializador de colección vacío (C# 12+)
    private readonly List<IObserver> _observers = [];

    // ============================================================
    // MÉTODO: Añadir observador
    // ============================================================
    // Añade un cliente a la lista de suscriptores
    public void AddObserver(IObserver observer) => _observers.Add(observer);

    // ============================================================
    // MÉTODO: Eliminar observador
    // ============================================================
    // Quita un cliente de la lista de suscriptores
    public void RemoveObserver(IObserver observer) => _observers.Remove(observer);

    // ============================================================
    // MÉTODO: Notificar a todos los observadores
    // ============================================================
    // Recorre todos los observadores y llama a su método Update()
    public void NotifyObservers(string message)
    {
        // foreach: iterar sobre cada observador en la lista
        foreach (var observer in _observers)
        {
            // Llamar al método Update del observador con el mensaje
            observer.Update(message);
        }
    }

    // ============================================================
    // MÉTODO: Acción del publicador
    // ============================================================
    // Simula que se prepara un café
    public void PrepararCafe()
    {
        // Imprimir mensaje de que se está preparando
        Console.WriteLine("☕ Preparando café...");
        
        // Thread.Sleep(): pausa la ejecución por 1000 milisegundos (1 segundo)
        // Simula el tiempo de preparación del café
        Thread.Sleep(1000);
        
        Console.WriteLine("✅ Café listo!");
        
        // ============================================================
        // IMPORTANTE: Notificar a los observadores
        // ============================================================
        // Cuando ocurre algo importante, notify a todos los suscriptores
        NotifyObservers("¡El café está listo!");
    }
}

// ============================================================
// CLASE CLIENTE (Observador)
// ============================================================
// Implementa IObserver - recibe notificaciones de la cafetería
// Usamos primary constructor (C# 12+) para inicializar el nombre
public class Cliente(string nombre) : IObserver
{
    // ============================================================
    // PROPIEDAD: Nombre del cliente
    // ============================================================
    // get: solo lectura, no se puede modificar después de creado
    public string Nombre { get; } = nombre;

    // ============================================================
    // MÉTODO: Update - manejar la notificación
    // ============================================================
    // Este método es llamado por el publicador cuando hay cambios
    public void Update(string message)
    {
        // Imprimir el mensaje recibido
        Console.WriteLine($"📢 {Nombre} recibe: {message}");
    }
}
