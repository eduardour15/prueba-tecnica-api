namespace PruebaTecnica.Api.Entities;

public class Poliza : EntidadAuditable
{
    public int Id { get; set; }
    public string NumeroPoliza { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public int VehiculoId { get; set; }
    public DateTime FechaEmision { get; set; }
    public decimal SumaAsegurada { get; set; }
    public decimal PrimaTotal { get; set; }
    public bool Activa { get; set; }
    public Cliente Cliente { get; set; } = null!;
    public Vehiculo Vehiculo { get; set; } = null!;
    public ICollection<PolizaCobertura> PolizaCoberturas { get; set; } = new List<PolizaCobertura>();
}