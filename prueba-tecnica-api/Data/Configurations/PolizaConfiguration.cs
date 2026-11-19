using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PruebaTecnica.Api.Entities;

namespace PruebaTecnica.Api.Data.Configurations;

public class PolizaConfiguration : IEntityTypeConfiguration<Poliza>
{
    public void Configure(EntityTypeBuilder<Poliza> builder)
    {
        builder.ToTable("Polizas");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.NumeroPoliza).IsRequired().HasMaxLength(30);
        builder.Property(p => p.SumaAsegurada).HasPrecision(18, 2);
        builder.Property(p => p.PrimaTotal).HasPrecision(18, 2);

        builder.Property(p => p.CreadoPor).IsRequired().HasMaxLength(100);
        builder.Property(p => p.ModificadoPor).HasMaxLength(100);

        builder.HasIndex(p => p.NumeroPoliza).IsUnique();

        // un cliente solo puede tener 1 póliza activa por vehículo
        builder.HasIndex(p => p.VehiculoId)
            .IsUnique()
            .HasFilter("[Activa] = 1");

        builder.HasOne(p => p.Cliente)
            .WithMany(c => c.Polizas)
            .HasForeignKey(p => p.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Vehiculo)
            .WithMany(v => v.Polizas)
            .HasForeignKey(p => p.VehiculoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}