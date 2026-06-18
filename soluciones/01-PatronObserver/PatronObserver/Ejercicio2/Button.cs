// Button.cs - Ejercicio 2: Eventos con Delegates
// ==============================================
// Este ejercicio muestra cómo implementar eventos en C#
// usando delegates de forma explícita.
//
// ¿Qué es un Delegate?
// Es un tipo que representa una referencia a un método.
// Piensa en él como un "puntero a función" seguro.
//
// ¿Qué es un Evento?
// Es un mecanismo de comunicación donde un objeto (botón)
// notifica a otros (manejadores) que algo ha ocurrido (clic).

namespace PatronObserver.Ejercicio2;

// ============================================================
// DELEGATE ClickHandler
// ============================================================
// Define la firma (signature) de los manejadores de evento para clicks.
// Cualquier método que coincida con esta firma puede manejar el evento.
//
// Explicación de la firma:
// - void: el método no devuelve ningún valor
// - object? sender: referencia al objeto que lanzó el evento (el botón)
//   El ? indica que puede ser null
// - EventArgs e: argumentos del evento (en este caso vacío, pero puede contener datos)
public delegate void ClickHandler(object? sender, EventArgs e);

// ============================================================
// CLASE BUTTON
// ============================================================
// Simula un botón que puede ser pulsado y dispara eventos
public class Button
{
    // ============================================================
    // EVENTO OnClick
    // ============================================================
    // Un evento es como un campo especial con dos operaciones:
    // - Suscribir (+=): añadir un manejador
    // - Desuscribir (-=): quitar un manejador
    //
    // ? indica que puede ser null (nullable)
    public event ClickHandler? OnClick;

    // ============================================================
    // MÉTODO Click()
    // ============================================================
    // Simula que el usuario pulsa el botón
    public void Click()
    {
        // Imprimir mensaje indicando que se pulsó
        Console.WriteLine("🔘 Botón pulsado");
        
        // ============================================================
        // DISPARAR EL EVENTO
        // ============================================================
        // OnClick?.Invoke(): solo llama a los manejadores si hay alguien suscrito
        // El operador ?. (null-conditional) evita error si es null
        //
        // this: referencia al botón actual (será el sender)
        // EventArgs.Empty: argumentos vacíos del evento
        OnClick?.Invoke(this, EventArgs.Empty);
    }
}

// ============================================================
// CLASE ButtonClickHandler
// ============================================================
// Clase que contiene el método que maneja el evento click
public class ButtonClickHandler
{
    // ============================================================
    // MÉTODO HandleClick
    // ============================================================
    // Este método se ejecutará cuando se dispare el evento OnClick
    // Cumple la firma definida por el delegate ClickHandler
    public void HandleClick(object? sender, EventArgs e)
    {
        // Imprimir mensaje de que se ejecutó el manejador
        Console.WriteLine("🎉 ¡Manejador de click ejecutado!");
    }
}
