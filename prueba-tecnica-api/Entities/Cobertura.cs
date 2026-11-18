namespace PruebaTecnica.Api.Entities;

public class Cobertura : EntidadAuditable
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Tasa { get; set; }
    public ICollection<PolizaCobertura> PolizaCoberturas { get; set; } = new List<PolizaCobertura>();
}