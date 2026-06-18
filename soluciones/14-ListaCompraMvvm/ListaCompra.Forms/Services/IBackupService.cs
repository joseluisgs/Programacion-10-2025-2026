// ============================================================
// IBackupService.cs - Interfaz para servicio de backup
// ============================================================
// Interfaz para importar/exportar productos usando diferentes formatos.
//
// CONCEPTOS IMPORTANTES:
//
// 1. BACKUP SERVICE:
//    - Importar: Carga productos desde archivo
//    - Exportar: Guarda productos en archivo
//    -自动 detecta formato por extensión
//
// 2. FORMATOS SOPORTADOS:
//    - .json: Usa ProductoJsonStorage
//    - .csv: Usa ProductoCsvStorage
//
// 3. ROP:
//    - Devuelve Result<T, DomainError>
//    - Manejo de errores centralizado

using System.Collections.Generic;
using CSharpFunctionalExtensions;
using ListaCompra.Errors;
using ListaCompra.Models;

namespace ListaCompra.Services;

public interface IBackupService
{
    Result<bool, DomainError> Exportar(IEnumerable<Producto> productos, string path);
    Result<IEnumerable<Producto>, DomainError> Importar(string path);
}
