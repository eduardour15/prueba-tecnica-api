using PruebaTecnica.Api.Entities;

namespace PruebaTecnica.Api.Repositories.Interfaces;

public interface IPolizaRepository
{
    Task<IReadOnlyList<Poliza>> ObtenerTodasConDetalleAsync(CancellationToken cancellationToken = default);
    Task<Poliza?> ObtenerPorIdConDetalleAsync(int id, CancellationToken cancellationToken = default);
    Task<Poliza?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int?> ObtenerClienteIdDePolizaActivaPorPlacaAsync(string placaNormalizada, CancellationToken cancellationToken = default);
    Task<long> ObtenerSiguienteCorrelativoAsync(CancellationToken cancellationToken = default);
    Task AgregarAsync(Poliza poliza, CancellationToken cancellationToken = default);
}