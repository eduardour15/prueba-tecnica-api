using PruebaTecnica.Api.Data;
using PruebaTecnica.Api.Repositories.Interfaces;

namespace PruebaTecnica.Api.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public Task<int> GuardarCambiosAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}