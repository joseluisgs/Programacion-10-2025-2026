using GestionAcademica.Models;
using GestionAcademica.Models.Personas;
using GestionAcademica.Storage.Common;

namespace GestionAcademica.Storage.Json;

/// <summary>
/// Contrato para persistir y cargar personas en formato JSON.
/// Hereda de <see cref="IStorage{T}"/> con T = <see cref="Persona"/>.
/// </summary>
public interface IAcademiaJsonStorage : IStorage<Persona> { }
