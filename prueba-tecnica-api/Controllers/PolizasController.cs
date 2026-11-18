using Microsoft.AspNetCore.Mvc;
using PruebaTecnica.Api.DTOs.Polizas;
using PruebaTecnica.Api.Services.Interfaces;

namespace PruebaTecnica.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PolizasController : ControllerBase
{
    private readonly IPolizaService _polizaService;

    public PolizasController(IPolizaService polizaService)
    {
        _polizaService = polizaService;
    }

    /// <summary>
    /// Emite una nueva póliza de automóvil calculando la prima total en el servidor.
    /// </summary>
    [HttpPost("emitir")]
    [ProducesResponseType(typeof(PolizaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PolizaResponse>> Emitir(
        [FromBody] EmitirPolizaRequest request,
        CancellationToken cancellationToken)
    {
        PolizaResponse polizaEmitida = await _polizaService.EmitirPolizaAsync(request, cancellationToken);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = polizaEmitida.Id }, polizaEmitida);
    }

    /// <summary>
    /// Lista el historial completo de pólizas emitidas ordenadas de la más reciente a la más antigua.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PolizaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PolizaResponse>>> ObtenerHistorial(
        CancellationToken cancellationToken)
    {
        IReadOnlyList<PolizaResponse> polizas = await _polizaService.ObtenerHistorialAsync(cancellationToken);
        return Ok(polizas);
    }

    /// <summary>
    /// Obtiene el detalle completo de una póliza emitida por su identificador.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PolizaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PolizaResponse>> ObtenerPorId(
        int id,
        CancellationToken cancellationToken)
    {
        PolizaResponse poliza = await _polizaService.ObtenerPorIdAsync(id, cancellationToken);
        return Ok(poliza);
    }

    /// <summary>
    /// Anula lógicamente una póliza activa cambiando su estado a inactiva.
    /// </summary>
    [HttpPatch("{id:int}/anular")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Anular(
        int id,
        CancellationToken cancellationToken)
    {
        await _polizaService.AnularPolizaAsync(id, cancellationToken);
        return NoContent();
    }
}