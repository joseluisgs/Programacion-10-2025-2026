// ============================================================
// IStorage.cs - Interfaz genérica para almacenamiento
// ============================================================
// Interfaz para implementar sistemas de almacenamiento (JSON, CSV, etc.).
// Usa Result<T, Error> para manejo de errores.
//
// CONCEPTOS IMPORTANTES:
//
// 1. STORAGE:
//    - Interfaz genérica para guardar/cargar datos
//    - Salvar: Guarda colección en archivo
//    - Cargar: Lee colección desde archivo
//
// 2. ROP:
//    - Devuelve Result<bool, DomainError> para Salvar
//    - Devuelve Result<IEnumerable<T>, DomainError> para Cargar
//    - Manejo de errores centralizado

using System.Collections.Generic;
using CSharpFunctionalExtensions;
using ListaCompra.Errors;

namespace ListaCompra.Storage.Common;

public interface IStorage<T>
{
    Result<bool, DomainError> Salvar(IEnumerable<T> items, string path);
    Result<IEnumerable<T>, DomainError> Cargar(string path);
}
