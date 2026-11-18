namespace PruebaTecnica.Api.Entities;

public class Cliente : EntidadAuditable
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Identificacion { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public ICollection<Poliza> Polizas { get; set; } = new List<Poliza>();
}