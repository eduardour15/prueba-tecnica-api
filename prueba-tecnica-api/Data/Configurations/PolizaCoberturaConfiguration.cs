using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PruebaTecnica.Api.Entities;

namespace PruebaTecnica.Api.Data.Configurations;

public class PolizaCoberturaConfiguration : IEntityTypeConfiguration<PolizaCobertura>
{
    public void Configure(EntityTypeBuilder<PolizaCobertura> builder)
    {
        builder.ToTable("PolizaCoberturas");
        builder.HasKey(pc => new { pc.PolizaId, pc.CoberturaId });

        builder.Property(pc => pc.TasaAplicada).HasPrecision(5, 2);

        builder.HasOne(pc => pc.Poliza)
            .WithMany(p => p.PolizaCoberturas)
            .HasForeignKey(pc => pc.PolizaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pc => pc.Cobertura)
            .WithMany(c => c.PolizaCoberturas)
            .HasForeignKey(pc => pc.CoberturaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}