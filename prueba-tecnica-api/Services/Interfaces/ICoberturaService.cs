using PruebaTecnica.Api.DTOs.Coberturas;

namespace PruebaTecnica.Api.Services.Interfaces;

public interface ICoberturaService
{
    Task<IReadOnlyList<CoberturaResponse>> ObtenerTodasAsync(CancellationToken cancellationToken = default);
}