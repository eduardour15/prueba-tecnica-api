namespace PruebaTecnica.Api.Services.Interfaces;

public interface IUsuarioActualService
{
    string ObtenerUsuarioActual();
    string? OptenerIpOrigen();
    string ObtenerCorrelationId();
}