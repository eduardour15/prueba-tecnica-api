using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace PruebaTecnica.Api.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private const string HeaderCorrelationId = "X-Correlation-Id";
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var correlationId = httpContext.Items.TryGetValue(HeaderCorrelationId, out var valor)
                            && valor is string id
            ? id
            : httpContext.TraceIdentifier;

        var (statusCode, title, detail) = exception switch
        {
            ReglaNegocioException => (
                StatusCodes.Status400BadRequest,
                "Regla de negocio incumplida",
                exception.Message),

            NoEncontradoException => (
                StatusCodes.Status404NotFound,
                "Recurso no encontrado",
                exception.Message),

            ConflictoException => (
                StatusCodes.Status409Conflict,
                "Conflicto con el estado del recurso",
                exception.Message),
            DbUpdateException dbEx when dbEx.InnerException is SqlException sqlEx && sqlEx.Number is 2601 or 2627 => (
                StatusCodes.Status409Conflict,
                "Conflicto con el estado del recurso",
                "Ya existe un registro activo con los datos únicos proporcionados."),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Error interno del servidor",
                "Ocurrió un error inesperado al procesar la solicitud.")
        };

        if (statusCode >= StatusCodes.Status500InternalServerError)
            _logger.LogError(
                exception,
                "Error no controlado en {Path} con CorrelationId {CorrelationId}",
                httpContext.Request.Path,
                correlationId);
        else
            _logger.LogWarning(
                "Excepción de dominio ({StatusCode}) en {Path} con CorrelationId {CorrelationId}: {Mensaje}",
                statusCode,
                httpContext.Request.Path,
                correlationId,
                exception.Message);

        var problemDetails = new ProblemDetails
        {
            Type = $"https://httpstatuses.com/{statusCode}",
            Title = title,
            Status = statusCode,
            Detail = detail,
            Instance = httpContext.Request.Path
        };
        problemDetails.Extensions["correlationId"] = correlationId;

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}