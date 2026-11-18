using PruebaTecnica.Api.DTOs.Coberturas;
using PruebaTecnica.Api.Entities;
using PruebaTecnica.Api.Repositories.Interfaces;
using PruebaTecnica.Api.Services.Interfaces;

namespace PruebaTecnica.Api.Services;

public class CoberturaService : ICoberturaService
{
    private readonly ICoberturaRepository _coberturaRepository;

    public CoberturaService(ICoberturaRepository coberturaRepository)
    {
        _coberturaRepository = coberturaRepository;
    }

    public async Task<IReadOnlyList<CoberturaResponse>> ObtenerTodasAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Cobertura> coberturas = await _coberturaRepository.ObtenerTodasAsync(cancellationToken);

        return coberturas
            .Select(c => new CoberturaResponse(c.Id, c.Nombre, c.Tasa))
            .ToList();
    }
}