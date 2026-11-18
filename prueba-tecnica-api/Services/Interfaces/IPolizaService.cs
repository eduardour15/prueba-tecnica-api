using PruebaTecnica.Api.DTOs.Polizas;

namespace PruebaTecnica.Api.Services.Interfaces;

public interface IPolizaService
{
    Task<PolizaResponse> EmitirPolizaAsync(EmitirPolizaRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PolizaResponse>> ObtenerHistorialAsync(CancellationToken cancellationToken = default);
    Task<PolizaResponse> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task AnularPolizaAsync(int id, CancellationToken cancellationToken = default);
}