using PruebaTecnica.Api.Services.Interfaces;

namespace PruebaTecnica.Api.Services;

public class UsuarioActualService : IUsuarioActualService
{
    private const string HeaderEjecutivo = "X-Ejecutivo-Id";
    private const string HeaderCorrelationId = "X-Correlation-Id";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsuarioActualService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string ObtenerUsuarioActual()
    {
        return "EjecutivoDemo";
    }

    public string? OptenerIpOrigen()
    {
        return _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
    }

    public string ObtenerCorrelationId()
    {
        return Guid.NewGuid().ToString("N");
    }
}