using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Api.Data;
using PruebaTecnica.Api.Entities;
using PruebaTecnica.Api.Repositories.Interfaces;

namespace PruebaTecnica.Api.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly AppDbContext _context;

    public ClienteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Cliente>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Clientes
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Cliente?> ObtenerPorIdSoloLecturaAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Clientes
            .AsNoTracking()
            .OrderBy(c => c.Nombre)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Cliente?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public Task<bool> ExisteAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Clientes
            .AsNoTracking()
            .AnyAsync(c => c.Id == id, cancellationToken);
    }

    public Task<bool> ExistePorIdentificacionAsync(string identificacion, int? excluirId = null,
        CancellationToken cancellationToken = default)
    {
        return _context.Clientes
            .AsNoTracking()
            .AnyAsync(
                c => c.Identificacion == identificacion && (!excluirId.HasValue || c.Id != excluirId.Value),
                cancellationToken);
    }

    public Task<bool> TienePolizasAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Polizas
            .AsNoTracking()
            .AnyAsync(p => p.ClienteId == id, cancellationToken);
    }

    public async Task AgregarAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        await _context.Clientes.AddAsync(cliente, cancellationToken);
    }

    public void Actualizar(Cliente cliente)
    {
        _context.Clientes.Update(cliente);
    }

    public void Eliminar(Cliente cliente)
    {
        _context.Clientes.Remove(cliente);
    }
}