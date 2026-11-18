namespace PruebaTecnica.Api.Entities;

public class BitacoraAuditoria
{
    public long Id { get; set; }
    public DateTime Fecha { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public string Accion { get; set; } = string.Empty;
    public string Entidad { get; set; } = string.Empty;
    public string EntidadId { get; set; } = string.Empty;
    public string Detalle { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public string? IpOrigen { get; set; }
}