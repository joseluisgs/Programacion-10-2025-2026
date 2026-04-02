// ============================================================
// LruCache.cs - Implementación de caché LRU
// ============================================================
// Implementación de caché basada en el algoritmo LRU (Least Recently Used).
// Cuando la capacidad se agota, elimina el elemento que lleva más tiempo sin usarse.
//
// CONCEPTOS IMPORTANTES:
//
// 1. ALGORITMO LRU:
//    - Least Recently Used (Menos Recientemente Usado)
//    - Elimina primero los elementos más antiguos
//    - Mantiene los datos más accedidos en memoria
//
// 2. ESTRUCTURAS DE DATOS:
//    - Dictionary<TKey, TValue>: Acceso O(1) por clave
//    - LinkedList<TKey>: Gestión de orden de uso O(1)
//    - Combinación óptima para caché LRU
//
// 3. OPERACIONES:
//    - Add: Añade o actualiza un elemento
//    - Get: Obtiene y marca como usado
//    - Remove: Elimina un elemento específico
//    - Eviction: Elimina el más antiguo si está llena
//
// 4. USO EN LA APLICACIÓN:
//    - Se registra en DependenciesProvider
//    - Se inyecta en el Servicio
//    - Tamaño configurable en appsettings.json

using System;
using System.Collections.Generic;
using Serilog;

namespace ListaCompra.Cache;

/// <summary>
/// Implementación de caché LRU.
/// </summary>
public class LruCache<TKey, TValue> : ICache<TKey, TValue>
    where TKey : notnull 
{
    private readonly int _capacity;
    private readonly Dictionary<TKey, TValue> _data = new();
    private readonly ILogger _logger = Log.ForContext<LruCache<TKey, TValue>>();
    private readonly LinkedList<TKey> _usageOrder = new();

    public LruCache(int capacity) 
    {
        if (capacity <= 0)
            throw new ArgumentException("La capacidad debe ser mayor que 0.", nameof(capacity));
        _capacity = capacity;
    }

    public void Add(TKey key, TValue value) 
    {
        _logger.Debug("[LRU-ADD] Añadiendo clave: {Key}", key);

        if (_data.TryGetValue(key, out var existingValue)) 
        {
            _data[key] = value;
            RefreshUsage(key);
            return;
        }

        if (_data.Count >= _capacity) 
        {
            var oldestKey = _usageOrder.First!.Value;
            _usageOrder.RemoveFirst();
            _data.Remove(oldestKey);
        }

        _data.Add(key, value);
        _usageOrder.AddLast(key);
    }

    public TValue? Get(TKey key) 
    {
        if (!_data.TryGetValue(key, out var value)) 
        {
            return default;
        }

        RefreshUsage(key);
        return value;
    }

    public bool Remove(TKey key) 
    {
        if (!_data.Remove(key)) 
        {
            return false;
        }

        _usageOrder.Remove(key);
        return true;
    }

    public void DisplayStatus() 
    {
        _logger.Information("[LRU-STATUS] Capacidad: {Used}/{Total}", _data.Count, _capacity);
    }

    private void RefreshUsage(TKey key) 
    {
        _usageOrder.Remove(key);
        _usageOrder.AddLast(key);
    }
}
