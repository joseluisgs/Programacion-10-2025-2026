using CSharpFunctionalExtensions;
using GestionAcademica.Errors.Common;
using GestionAcademica.Models.Personas;
using GestionAcademica.Storage.Common;

namespace GestionAcademica.Storage.Binary;

public interface IAcademiaBinStorage : IStorage<Persona>
{
}
