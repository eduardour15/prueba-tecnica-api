using PruebaTecnica.Api.DTOs.Clientes;

namespace PruebaTecnica.Api.Services.Interfaces;

public interface IClienteService
{
    Task<IReadOnlyList<ClienteResponse>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<ClienteResponse> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ClienteResponse> CrearAsync(CrearClienteRequest request, CancellationToken cancellationToken = default);
    Task<ClienteResponse> ActualizarAsync(int id, ActualizarClienteRequest request, CancellationToken cancellationToken = default);
    Task EliminarAsync(int id, CancellationToken cancellationToken = default);
}