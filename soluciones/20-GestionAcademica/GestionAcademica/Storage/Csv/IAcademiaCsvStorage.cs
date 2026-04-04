using GestionAcademica.Models;
using GestionAcademica.Models.Personas;
using GestionAcademica.Storage.Common;

namespace GestionAcademica.Storage.Csv;

/// <summary>
/// Contrato para persistir y cargar personas en formato CSV.
/// Hereda de <see cref="IStorage{T}"/> con T = <see cref="Persona"/>.
/// </summary>
public interface IAcademiaCsvStorage : IStorage<Persona> { }