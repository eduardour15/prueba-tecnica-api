using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Api.Data;
using PruebaTecnica.Api.Entities;
using PruebaTecnica.Api.Repositories.Interfaces;

namespace PruebaTecnica.Api.Repositories;

public class CoberturaRepository : ICoberturaRepository
{
    private readonly AppDbContext _context;

    public CoberturaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Cobertura>> ObtenerTodasAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Coberturas
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Cobertura>> ObtenerPorIdsAsync(IReadOnlyCollection<int> ids,
        CancellationToken cancellationToken = default)
    {
        return await _context.Coberturas
            .AsNoTracking()
            .Where(c => ids.Contains(c.Id))
            .ToListAsync(cancellationToken);
    }
}