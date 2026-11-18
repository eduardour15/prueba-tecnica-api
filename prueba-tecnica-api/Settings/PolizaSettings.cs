namespace PruebaTecnica.Api.Settings;

public class PolizaSettings
{
    public const string Seccion = "PolizaSettings";
    public int MaxAntiguedadVehiculoAnios { get; set; } = 20;
    public string PrefijoNumeroPoliza { get; set; } = "POL";
}