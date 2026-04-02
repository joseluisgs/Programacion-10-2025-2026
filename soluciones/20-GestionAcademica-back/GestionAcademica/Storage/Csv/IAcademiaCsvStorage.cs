using GestionAcademica.Models;
using GestionAcademica.Models.Personas;
using GestionAcademica.Storage.Common;

namespace GestionAcademica.Storage.Csv;

public interface IAcademiaCsvStorage : IStorage<Persona> { }