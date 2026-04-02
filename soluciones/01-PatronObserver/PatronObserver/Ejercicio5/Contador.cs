// Contador.cs - Ejercicio 5: Eventos con Action<T>
// ==================================================
// Este ejercicio muestra cómo usar Action<T> en lugar de delegates propios.
// Action<T> es un delegate predefinido de .NET que acepta un parámetro.
//
// ¿Qué es Action<T>?
// Es un delegate genérico predefinido en el namespace System.
// - Action: sin parámetros, no devuelve valor
// - Action<T>: 1 parámetro, no devuelve valor
// - Action<T1, T2>: 2 parámetros, no devuelve valor
// - ... hasta Action<T1, ..., T16>
//
// Ventajas sobre definir nuestros propios delegates:
// - No necesitamos definir la firma manualmente
// - Código más limpio y legible
// - Estándar de .NET, reconocido por todos los desarrolladores

namespace PatronObserver.Ejercicio5;

// ============================================================
// CLASE CONTADOR
// ============================================================
public class Contador
{
    // ============================================================
    // EVENTO: OnValueChanged
    // ============================================================
    // Usa Action<int> en lugar de un delegate personalizado.
    // Significa: "una función que acepta un int y no devuelve nada"
    //
    // ¿Por qué Action<int>? Porque queremos pasar el valor del contador
    // al manejador para que sepa qué valor tiene.
    public event Action<int>? OnValueChanged;

    // ============================================================
    // CAMPO: _value
    // ============================================================
    // Almacena el valor actual del contador
    private int _value;

    // ============================================================
    // PROPIEDAD: Value
    // ============================================================
    // Propiedad que controla el acceso al campo _value
    // Incluye la lógica de notificación cuando cambia el valor
    public int Value
    {
        get => _value;  // Devolver el valor actual
        
        set
        {
            // Solo notificar si el valor realmente cambió
            if (_value != value)
            {
                // Asignar el nuevo valor
                _value = value;
                
                // ============================================================
                // NOTIFICAR EL NUEVO VALOR
                // ============================================================
                // OnValueChanged?.Invoke(_value):
                // - ?: solo ejecuta si hay alguien suscrito
                // - _value: argumento que se pasa al manejador
                // El manejador recibirá este valor como parámetro
                OnValueChanged?.Invoke(_value);
            }
        }
    }

    // ============================================================
    // MÉTODO: Incrementar
    // ============================================================
    // Incrementa el valor en 1
    // Value++ es equivalente a: Value = Value + 1
    public void Incrementar() => Value++;

    // ============================================================
    // MÉTODO: Decrementar
    // ============================================================
    // Decrementa el valor en 1
    // Value-- es equivalente a: Value = Value - 1
    public void Decrementar() => Value--;
}
