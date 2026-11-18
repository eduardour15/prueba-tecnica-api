using PruebaTecnica.Api.Entities;

namespace PruebaTecnica.Api.Repositories.Interfaces;

public interface IPolizaRepository
{
    Task<IReadOnlyList<Poliza>> ObtenerTodasConDetalleAsync(CancellationToken cancellationToken = default);
    Task<Poliza?> ObtenerPorIdConDetalleAsync(int id, CancellationToken cancellationToken = default);
    Task<Poliza?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistePolizaActivaPorClienteYPlacaAsync(int clienteId, string placaNormalizada, CancellationToken cancellationToken = default);
    Task<int> ContarEmitidasEnRangoUtcAsync(DateTime inicioUtc, DateTime finUtc, CancellationToken cancellationToken = default);
    Task AgregarAsync(Poliza poliza, CancellationToken cancellationToken = default);
}