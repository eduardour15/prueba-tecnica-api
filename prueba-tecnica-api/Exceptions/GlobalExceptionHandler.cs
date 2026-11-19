using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Api.Helpers;

namespace PruebaTecnica.Api.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
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
        var correlationId = CorrelationIdHelper.ObtenerDesdeHttpContext(httpContext);

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
            DbUpdateConcurrencyException => (
                StatusCodes.Status409Conflict,
                "Conflicto de concurrencia",
                "El recurso fue modificado o eliminado concurrentemente por otra operación."),
            DbUpdateException dbEx when SqlExceptionHelper.EsViolacionUnica(dbEx) => (
                StatusCodes.Status409Conflict,
                "Conflicto con el estado del recurso",
                "Ya existe un registro activo con los datos únicos proporcionados."),
            DbUpdateException dbEx when SqlExceptionHelper.EsViolacionClaveForanea(dbEx) => (
                StatusCodes.Status409Conflict,
                "Conflicto con el estado del recurso",
                "La operación no puede completarse debido a una restricción de integridad referencial."),
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