// ============================================================
// ICache.cs - Interfaz para la caché de la aplicación
// ============================================================
// Interfaz genérica para implementar patrones de caché.
//
// CONCEPTOS IMPORTANTES:
//
// 1. CACHÉ:
//    - Almacenamiento temporal de datos en memoria
//    - Evita accesos repetidos a la base de datos
//    - Mejora el rendimiento de la aplicación
//
// 2. INTERFAZ GENÉRICA:
//    - ICache<TKey, TValue>: Cualquier tipo de clave-valor
//    - TKey: Tipo de la clave (ej: int, string)
//    - TValue: Tipo del valor a almacenar
//
// 3. OPERACIONES BÁSICAS:
//    - Add: Añadir elemento a la caché
//    - Get: Obtener elemento por clave
//    - Remove: Eliminar elemento

namespace ListaCompra.Cache;

/// <summary>
/// Interfaz genérica para implementar sistemas de caché.
/// </summary>
/// <typeparam name="TKey">Tipo de la clave.</typeparam>
/// <typeparam name="TValue">Tipo del valor.</typeparam>
public interface ICache<in TKey, TValue> where TKey : notnull 
{
    void Add(TKey key, TValue value);
    TValue? Get(TKey key);
    bool Remove(TKey key);
    void DisplayStatus();
}
