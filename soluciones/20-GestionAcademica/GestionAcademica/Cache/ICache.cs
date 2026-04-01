namespace GestionAcademica.Cache;

/// <summary>
///     Cache
/// </summary>
/// <typeparam name="TKey">Clave</typeparam>
/// <typeparam name="TValue">Valor</typeparam>
public interface ICache<in TKey, TValue> where TKey : notnull {
    /// <summary>
    ///     Agregar un elemento al cache
    /// </summary>
    void Add(TKey key, TValue value);

    /// <summary>
    ///     Obtener un elemento del cache
    /// </summary>
    TValue? Get(TKey key);

    /// <summary>
    ///     Eliminar un elemento del cache
    /// </summary>
    bool Remove(TKey key);

    // void DisplayStatus();
}