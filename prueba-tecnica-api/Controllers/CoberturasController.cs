using Microsoft.AspNetCore.Mvc;
using PruebaTecnica.Api.DTOs.Coberturas;
using PruebaTecnica.Api.Services.Interfaces;

namespace PruebaTecnica.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CoberturasController : ControllerBase
{
    private readonly ICoberturaService _coberturaService;

    public CoberturasController(ICoberturaService coberturaService)
    {
        _coberturaService = coberturaService;
    }

    /// <summary>
    /// Obtiene el catálogo de coberturas disponibles y sus tasas porcentuales.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CoberturaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CoberturaResponse>>> ObtenerTodas(
        CancellationToken cancellationToken)
    {
        IReadOnlyList<CoberturaResponse> coberturas = await _coberturaService.ObtenerTodasAsync(cancellationToken);
        return Ok(coberturas);
    }
}