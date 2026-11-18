namespace PruebaTecnica.Api.Entities;

public abstract class EntidadAuditable
{
    public DateTime FechaCreacion { get; set; }
    public string CreadoPor { get; set; } = string.Empty;
    public DateTime? FechaModificacion { get; set; }
    public string? ModificadoPor { get; set; }
}