using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Api.Data;
using PruebaTecnica.Api.Entities;
using PruebaTecnica.Api.Repositories.Interfaces;

namespace PruebaTecnica.Api.Repositories;

public class PolizaRepository : IPolizaRepository
{
    private readonly AppDbContext _context;

    public PolizaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Poliza>> ObtenerTodasConDetalleAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Polizas
            .AsNoTracking()
            .Include(p => p.Cliente)
            .Include(p => p.Vehiculo)
            .Include(p => p.PolizaCoberturas)
            .ThenInclude(pc => pc.Cobertura)
            .OrderByDescending(p => p.FechaEmision)
            .ToListAsync(cancellationToken);
    }

    public Task<Poliza?> ObtenerPorIdConDetalleAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Polizas
            .AsNoTracking()
            .Include(p => p.Cliente)
            .Include(p => p.Vehiculo)
            .Include(p => p.PolizaCoberturas)
            .ThenInclude(pc => pc.Cobertura)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public Task<Poliza?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Polizas
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public Task<bool> ExistePolizaActivaPorClienteYPlacaAsync(int clienteId, string placaNormalizada,
        CancellationToken cancellationToken = default)
    {
        return _context.Polizas
            .AsNoTracking()
            .AnyAsync(
                p => p.ClienteId == clienteId && p.Activa && p.Vehiculo.Placa == placaNormalizada,
                cancellationToken);
    }

    public Task<int> ContarEmitidasEnRangoUtcAsync(DateTime inicioUtc, DateTime finUtc,
        CancellationToken cancellationToken = default)
    {
        return _context.Polizas
            .AsNoTracking()
            .CountAsync(
                p => p.FechaEmision >= inicioUtc && p.FechaEmision < finUtc,
                cancellationToken);
    }

    public async Task AgregarAsync(Poliza poliza, CancellationToken cancellationToken = default)
    {
        await _context.Polizas.AddAsync(poliza, cancellationToken);
    }
}