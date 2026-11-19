using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Api.Data;
using PruebaTecnica.Api.Entities;
using PruebaTecnica.Api.Helpers;
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
        string placaCanonica = TextoValidacionHelper.NormalizarPlacaCanonica(placaNormalizada);
        return _context.Vehiculos.FirstOrDefaultAsync(
            v => EF.Property<string>(v, "PlacaCanonica") == placaCanonica,
            cancellationToken);
    }

    public async Task AgregarAsync(Vehiculo vehiculo, CancellationToken cancellationToken = default)
    {
        await _context.Vehiculos.AddAsync(vehiculo, cancellationToken);
    }
}