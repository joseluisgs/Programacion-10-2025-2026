using CSharpFunctionalExtensions;
using GestionAcademica.Errors.Common;
using GestionAcademica.Models.Personas;
using GestionAcademica.Storage.Common;

namespace GestionAcademica.Storage.Binary;

/// <summary>
/// Contrato para persistir y cargar personas en formato binario.
/// Hereda de <see cref="IStorage{T}"/> con T = <see cref="Persona"/>.
/// </summary>
public interface IAcademiaBinStorage : IStorage<Persona>
{
}
