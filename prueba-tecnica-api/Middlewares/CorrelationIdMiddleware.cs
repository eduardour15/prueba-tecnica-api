using System.Text.RegularExpressions;

namespace PruebaTecnica.Api.Middlewares;

public partial class CorrelationIdMiddleware
{
    private const string HeaderCorrelationId = "X-Correlation-Id";
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string correlationId = ObtenerOSanitizarCorrelationId(context);
        context.Items[HeaderCorrelationId] = correlationId;
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderCorrelationId] = correlationId;
            return Task.CompletedTask;
        });
        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["CorrelationId"] = correlationId
               }))
        {
            await _next(context);
        }
    }

    private static string ObtenerOSanitizarCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(HeaderCorrelationId, out var header) &&
            !string.IsNullOrWhiteSpace(header))
        {
            var candidato = header.ToString().Trim();
            if (candidato.Length <= 64 && CorrelationRegex().IsMatch(candidato)) return candidato;
        }

        return Guid.NewGuid().ToString("N");
    }

    [GeneratedRegex("^[a-zA-Z0-9_-]+$")]
    private static partial Regex CorrelationRegex();
}