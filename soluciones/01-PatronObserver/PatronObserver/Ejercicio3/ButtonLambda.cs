// ButtonLambda.cs - Ejercicio 3: Eventos con Lambdas
// ====================================================
// Este ejercicio muestra cómo usar funciones lambda
// como manejadores de eventos.
//
// ¿Qué son las Lambdas?
// Son funciones anónimas (sin nombre) que se pueden definir inline.
// Sintaxis: (parámetros) => expresión o bloque de código
//
// Ventajas:
// - Más concisas que crear una clase/método completo
// - Ideales para callbacks y eventos simples

namespace PatronObserver.Ejercicio3;

// ============================================================
// CLASE ButtonLambda
// ============================================================
public class ButtonLambda
{
    // ============================================================
    // CAMPO: Action - Delegate predefinido de .NET
    // ============================================================
    // Action es un delegate predefinido que NO devuelve valor.
    // Puede tener entre 0 y 16 parámetros.
    //
    // Action? significa: función sin parámetros que no devuelve nada
    // El ? indica que puede ser null (no hay manejador asignado)
    private Action? _onClickHandler;

    // ============================================================
    // MÉTODO: Asignar manejador
    // ============================================================
    // Permite asignar una función (puede ser lambda) que se ejecutará al hacer click
    public void SetOnClickHandler(Action handler)
    {
        // Guardar la función en el campo privado
        _onClickHandler = handler;
    }

    // ============================================================
    // MÉTODO: Click
    // ============================================================
    // Simula que el usuario pulsa el botón
    public void Click()
    {
        Console.WriteLine("🔘 Lambda Button pulsado");
        
        // ============================================================
        // EJECUTAR EL MANEJADOR (si existe)
        // ============================================================
        // _onClickHandler?.Invoke(): solo ejecuta si no es null
        // Como Action no tiene parámetros, solo se llama sin argumentos
        _onClickHandler?.Invoke();
    }
}
