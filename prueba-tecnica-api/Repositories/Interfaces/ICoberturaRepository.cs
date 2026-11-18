using PruebaTecnica.Api.Entities;

namespace PruebaTecnica.Api.Repositories.Interfaces;

public interface ICoberturaRepository
{
    Task<IReadOnlyList<Cobertura>> ObtenerTodasAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Cobertura>> ObtenerPorIdsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken = default);
}