using PruebaTecnica.Api.Entities;

namespace PruebaTecnica.Api.Repositories.Interfaces;

public interface IVehiculoRepository
{
    Task<Vehiculo?> ObtenerPorPlacaAsync(string placaNormalizada, CancellationToken cancellationToken = default);
    Task AgregarAsync(Vehiculo vehiculo, CancellationToken cancellationToken = default);
}