namespace GestionAcademica.Cache;

/// <summary>
/// Contrato para una caché con algoritmo LRU (Least Recently Used).
/// </summary>
/// <typeparam name="TKey">Tipo de la clave.</typeparam>
/// <typeparam name="TValue">Tipo del valor.</typeparam>
public interface ICache<in TKey, TValue> where TKey : notnull {
    /// <summary>
    /// Agrega un elemento a la caché. Si está llena, elimina el menos usado.
    /// </summary>
    /// <param name="key">Clave del elemento.</param>
    /// <param name="value">Valor del elemento.</param>
    void Add(TKey key, TValue value);

    /// <summary>
    /// Obtiene un elemento de la caché.
    /// </summary>
    /// <param name="key">Clave del elemento.</param>
    /// <returns>El valor si existe, null en caso contrario.</returns>
    TValue? Get(TKey key);

    /// <summary>
    /// Elimina un elemento de la caché.
    /// </summary>
    /// <param name="key">Clave del elemento a eliminar.</param>
    /// <returns>True si se eliminó, False si no existía.</returns>
    bool Remove(TKey key);
}