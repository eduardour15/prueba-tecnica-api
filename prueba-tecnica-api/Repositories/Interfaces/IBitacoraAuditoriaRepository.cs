using PruebaTecnica.Api.Entities;

namespace PruebaTecnica.Api.Repositories.Interfaces;

public interface IBitacoraAuditoriaRepository
{
    Task RegistrarAsync(BitacoraAuditoria registro, CancellationToken cancellationToken = default);
}