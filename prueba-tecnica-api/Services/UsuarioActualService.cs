using PruebaTecnica.Api.Services.Interfaces;

namespace PruebaTecnica.Api.Services;

public class UsuarioActualService : IUsuarioActualService
{
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

    public string? ObtenerIpOrigen()
    {
        return _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
    }

    public string ObtenerCorrelationId()
    {
        HttpContext? context = _httpContextAccessor.HttpContext;
        if (context?.Items.TryGetValue(HeaderCorrelationId, out var valor) == true
            && valor is string correlationId)
            return correlationId;

        return context?.TraceIdentifier ?? Guid.NewGuid().ToString("N");
    }
}