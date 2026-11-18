namespace PruebaTecnica.Api.Services.Interfaces;

public interface IUsuarioActualService
{
    string ObtenerUsuarioActual();
    string? ObtenerIpOrigen();
    string ObtenerCorrelationId();
}