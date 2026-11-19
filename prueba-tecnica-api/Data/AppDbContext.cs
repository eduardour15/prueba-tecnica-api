using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Api.Entities;

namespace PruebaTecnica.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Vehiculo> Vehiculos => Set<Vehiculo>();
    public DbSet<Cobertura> Coberturas => Set<Cobertura>();
    public DbSet<Poliza> Polizas => Set<Poliza>();
    public DbSet<PolizaCobertura> PolizaCoberturas => Set<PolizaCobertura>();
    public DbSet<BitacoraAuditoria> BitacoraAuditorias => Set<BitacoraAuditoria>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        modelBuilder.HasSequence<long>("SeqNumeroPoliza")
            .StartsAt(1)
            .IncrementsBy(1);
    }
}