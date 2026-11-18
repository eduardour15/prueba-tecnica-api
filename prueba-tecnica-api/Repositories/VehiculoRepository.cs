using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Api.Data;
using PruebaTecnica.Api.Entities;
using PruebaTecnica.Api.Repositories.Interfaces;

namespace PruebaTecnica.Api.Repositories;

public class VehiculoRepository : IVehiculoRepository
{
    private readonly AppDbContext _context;

    public VehiculoRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Vehiculo?> ObtenerPorPlacaAsync(string placaNormalizada, CancellationToken cancellationToken = default)
    {
        return _context.Vehiculos.FirstOrDefaultAsync(v => v.Placa == placaNormalizada, cancellationToken);
    }

    public async Task AgregarAsync(Vehiculo vehiculo, CancellationToken cancellationToken = default)
    {
        await _context.Vehiculos.AddAsync(vehiculo, cancellationToken);
    }
}