using Microsoft.AspNetCore.Mvc;
using PruebaTecnica.Api.DTOs.Clientes;
using PruebaTecnica.Api.Services.Interfaces;

namespace PruebaTecnica.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    /// <summary>
    /// Obtiene el catálogo de clientes registrados.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ClienteResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ClienteResponse>>> ObtenerTodos(CancellationToken cancellationToken)
    {
        var clientes = await _clienteService.ObtenerTodosAsync(cancellationToken);
        return Ok(clientes);
    }

    /// <summary>
    /// Obtiene el detalle de un cliente por su identificador.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteResponse>> ObtenerPorId(
        int id,
        CancellationToken cancellationToken)
    {
        var cliente = await _clienteService.ObtenerPorIdAsync(id, cancellationToken);
        return Ok(cliente);
    }

    /// <summary>
    /// Registra un nuevo cliente en el sistema.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ClienteResponse>> Crear(
        [FromBody] CrearClienteRequest request,
        CancellationToken cancellationToken)
    {
        var creado = await _clienteService.CrearAsync(request, cancellationToken);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
    }

    /// <summary>
    /// Actualiza los datos de un cliente existente.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ClienteResponse>> Actualizar(
        int id,
        [FromBody] ActualizarClienteRequest request,
        CancellationToken cancellationToken)
    {
        var actualizado = await _clienteService.ActualizarAsync(id, request, cancellationToken);
        return Ok(actualizado);
    }

    /// <summary>
    /// Elimina un cliente siempre que no tenga pólizas asociadas.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Eliminar(
        int id,
        CancellationToken cancellationToken)
    {
        await _clienteService.EliminarAsync(id, cancellationToken);
        return NoContent();
    }
}