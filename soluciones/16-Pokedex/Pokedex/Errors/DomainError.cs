namespace Pokedex.Errors;

/// <summary>
/// Clase base para errores de dominio usando ROP (Railway Oriented Programming)
/// </summary>
public abstract record DomainError(string Message);
