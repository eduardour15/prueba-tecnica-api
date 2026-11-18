namespace PruebaTecnica.Api.Entities;

public class Vehiculo : EntidadAuditable
{
    public int Id { get; set; }
    public string Placa { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int Anio { get; set; }
    public decimal ValorComercial { get; set; }
    public ICollection<Poliza> Polizas { get; set; } = new List<Poliza>();
}