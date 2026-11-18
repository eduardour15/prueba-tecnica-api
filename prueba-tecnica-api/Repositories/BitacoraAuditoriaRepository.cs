using PruebaTecnica.Api.Data;
using PruebaTecnica.Api.Entities;
using PruebaTecnica.Api.Repositories.Interfaces;

namespace PruebaTecnica.Api.Repositories;

public class BitacoraAuditoriaRepository : IBitacoraAuditoriaRepository
{
    private readonly AppDbContext _context;
    public BitacoraAuditoriaRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task RegistrarAsync(BitacoraAuditoria registro, CancellationToken cancellationToken = default)
    {
        await _context.BitacoraAuditorias.AddAsync(registro, cancellationToken);
    }
}