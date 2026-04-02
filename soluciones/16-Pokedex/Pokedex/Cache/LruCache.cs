using System;
using System.Collections.Generic;
using Serilog;

namespace Pokedex.Cache;

/// <summary>
/// Interfaz genérica para caché
/// </summary>
public interface ICache<in TKey, TValue> where TKey : notnull 
{
    void Add(TKey key, TValue value);
    TValue? Get(TKey key);
    bool Remove(TKey key);
    void Clear();
    void DisplayStatus();
}

/// <summary>
/// Implementación de caché LRU (Least Recently Used)
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

    public void Clear()
    {
        _data.Clear();
        _usageOrder.Clear();
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
