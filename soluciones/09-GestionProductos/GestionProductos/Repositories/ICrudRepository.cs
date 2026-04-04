// ============================================================
// ICrudRepository.cs - Interfaz genérica CRUD
// ============================================================
// Patrón repositorio genérico para operaciones CRUD.
//
// MÉTODOS:
// - GetAll(): Obtener todos
// - GetById(id): Obtener por ID
// - Create(entity): Crear
// - Update(id, entity): Actualizar
// - Delete(id): Eliminar

using System.Collections.Generic;

namespace GestionProductos.Repositories;

/// <summary>
/// Interfaz genérica para operaciones CRUD.
/// </summary>
public interface ICrudRepository<in TKey, TEntity> where TEntity : class
{
    IEnumerable<TEntity> GetAll();
    TEntity? GetById(TKey id);
    TEntity? Create(TEntity entity);
    TEntity? Update(TKey id, TEntity entity);
    TEntity? Delete(TKey id);
}