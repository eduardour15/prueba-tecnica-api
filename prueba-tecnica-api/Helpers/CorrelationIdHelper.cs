namespace PruebaTecnica.Api.Helpers;

internal static class CorrelationIdHelper
{
    public const string HeaderCorrelationId = "X-Correlation-Id";

    public static string ObtenerDesdeHttpContext(HttpContext context)
    {
        return context.Items.TryGetValue(HeaderCorrelationId, out object? valor) && valor is string id
            ? id
            : context.TraceIdentifier;
    }
}
