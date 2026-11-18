using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PruebaTecnica.Api.Entities;
using PruebaTecnica.Api.Services.Interfaces;

namespace PruebaTecnica.Api.Data.Interceptors;

public class AuditoriaInterceptor : SaveChangesInterceptor
{
    private readonly IUsuarioActualService _usuarioActualService;

    public AuditoriaInterceptor(IUsuarioActualService usuarioActualService)
    {
        _usuarioActualService = usuarioActualService;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        AplicarAuditoria(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new())
    {
        AplicarAuditoria(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void AplicarAuditoria(DbContext? context)
    {
        if (context is null) return;
        var ahoraUtc = DateTime.UtcNow;
        var usuario = _usuarioActualService.ObtenerUsuarioActual();
        foreach (var entry in context.ChangeTracker.Entries<EntidadAuditable>())
            if (entry.State == EntityState.Added)
            {
                entry.Entity.FechaCreacion = ahoraUtc;
                entry.Entity.CreadoPor = usuario;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(e => e.FechaCreacion).IsModified = false;
                entry.Property(e => e.CreadoPor).IsModified = false;
                entry.Entity.FechaModificacion = ahoraUtc;
                entry.Entity.ModificadoPor = usuario;
            }
    }
}