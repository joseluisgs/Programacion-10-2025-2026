namespace WpfBindingsReactividad.Models;

public class Persona
{
    public int Id { get; set; }
    public string Dni { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Apellidos { get; set; } = "";
    public string Email { get; set; } = "";
    public int Edad { get; set; }
    
    public string NombreCompleto => $"{Nombre} {Apellidos}".Trim();
}