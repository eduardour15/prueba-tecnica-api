using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PruebaTecnica.Api.Data;
using PruebaTecnica.Api.Entities;
using PruebaTecnica.Api.Helpers;
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

    public Task<int?> ObtenerClienteIdDePolizaActivaPorPlacaAsync(string placaNormalizada,
        CancellationToken cancellationToken = default)
    {
        string placaCanonica = TextoValidacionHelper.NormalizarPlacaCanonica(placaNormalizada);
        return _context.Polizas
            .AsNoTracking()
            .Where(p => p.Activa && EF.Property<string>(p.Vehiculo, "PlacaCanonica") == placaCanonica)
            .Select(p => (int?)p.ClienteId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<long> ObtenerSiguienteCorrelativoAsync(CancellationToken cancellationToken = default)
    {
        DbConnection connection = _context.Database.GetDbConnection();
        bool cerrarAlTerminar = connection.State != ConnectionState.Open;
        if (cerrarAlTerminar)
            await connection.OpenAsync(cancellationToken);

        try
        {
            await using DbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT NEXT VALUE FOR [dbo].[SeqNumeroPoliza];";
            command.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction();
            object? resultado = await command.ExecuteScalarAsync(cancellationToken);
            return Convert.ToInt64(resultado);
        }
        finally
        {
            if (cerrarAlTerminar)
                await connection.CloseAsync();
        }
    }

    public async Task AgregarAsync(Poliza poliza, CancellationToken cancellationToken = default)
    {
        await _context.Polizas.AddAsync(poliza, cancellationToken);
    }
}