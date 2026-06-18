// ============================================================
// ICrudRepository.cs - Interfaz genérica para operaciones CRUD
// ============================================================
// Interfaz genérica para operaciones básicas de creación, lectura,
// actualización y eliminación de entidades.
//
// CONCEPTOS IMPORTANTES:
//
// 1. CRUD (Create, Read, Update, Delete):
//    - Operaciones básicas de acceso a datos
//    - Create: Insertar nueva entidad
//    - Read: Obtener entidad(es)
//    - Update: Modificar entidad existente
//    - Delete: Eliminar entidad
//
// 2. INTERFAZ GENÉRICA:
//    - ICrudRepository<TKey, TEntity>
//    - TKey: Tipo de la clave primaria (int, string, Guid, etc.)
//    - TEntity: Tipo de la entidad (Producto, Cliente, etc.)
//    - Permite reutilizar para diferentes entidades
//
// 3. MÉTODOS DEFINIDOS:
//    - GetAll(): Obtener todas las entidades
//    - GetById(TKey): Obtener una entidad por su clave
//    - Create(TEntity): Insertar nueva entidad
//    - Update(TKey, TEntity): Actualizar una entidad
//    - Delete(TKey, bool): Eliminar (lógico o físico)
//
// 4. IMPLEMENTACIONES ESPECÍFICAS:
//    - IProductoRepository hereda de ICrudRepository<int, Producto>
//    - Añade métodos específicos como GetByNombre()

using System.Collections.Generic;

namespace ListaCompra.Repositories;

/// <summary>
/// Interfaz genérica para operaciones CRUD (Create, Read, Update, Delete).
/// Define las operaciones básicas de acceso a datos.
/// </summary>
/// <typeparam name="TKey">Tipo de la clave primaria.</typeparam>
/// <typeparam name="TEntity">Tipo de la entidad.</typeparam>
public interface ICrudRepository<in TKey, TEntity> where TEntity : class
{
    /// <summary>
    /// Obtiene todas las entidades.
    /// </summary>
    /// <returns>Enumerable de todas las entidades.</returns>
    IEnumerable<TEntity> GetAll();

    /// <summary>
    /// Obtiene una entidad por su clave primaria.
    /// </summary>
    /// <param name="id">Clave primaria de la entidad.</param>
    /// <returns>Entidad encontrada o null si no existe.</returns>
    TEntity? GetById(TKey id);

    /// <summary>
    /// Crea una nueva entidad.
    /// </summary>
    /// <param name="entity">Entidad a crear.</param>
    /// <returns>Entidad creada (puede incluir ID generado).</returns>
    TEntity? Create(TEntity entity);

    /// <summary>
    /// Actualiza una entidad existente.
    /// </summary>
    /// <param name="id">Clave primaria de la entidad a actualizar.</param>
    /// <param name="entity">Nueva entidad.</param>
    /// <returns>Entidad actualizada o null si no existe.</returns>
    TEntity? Update(TKey id, TEntity entity);

    /// <summary>
    /// Elimina una entidad.
    /// </summary>
    /// <param name="id">Clave primaria de la entidad a eliminar.</param>
    /// <param name="logical">Si true, eliminación lógica; si false, eliminación física.</param>
    /// <returns>Entidad eliminada o null si no existe.</returns>
    TEntity? Delete(TKey id, bool logical = true);
}