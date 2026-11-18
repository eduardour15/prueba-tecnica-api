namespace PruebaTecnica.Api.Repositories.Interfaces;

public interface IUnitOfWork
{
    Task<int> GuardarCambiosAsync(CancellationToken cancellationToken = default);
}