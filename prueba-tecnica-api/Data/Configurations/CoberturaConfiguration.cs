using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PruebaTecnica.Api.Entities;

namespace PruebaTecnica.Api.Data.Configurations;

public class CoberturaConfiguration : IEntityTypeConfiguration<Cobertura>
{
    public void Configure(EntityTypeBuilder<Cobertura> builder)
    {
        builder.ToTable("Coberturas");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Tasa).HasPrecision(5, 2);

        builder.Property(c => c.CreadoPor).IsRequired().HasMaxLength(100);
        builder.Property(c => c.ModificadoPor).HasMaxLength(100);

        builder.HasIndex(c => c.Nombre).IsUnique();

        var fechaSeed = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        builder.HasData(
            new Cobertura
            {
                Id = 1,
                Nombre = "Robo",
                Tasa = 3.50m,
                FechaCreacion = fechaSeed,
                CreadoPor = "Seed"
            },
            new Cobertura
            {
                Id = 2,
                Nombre = "Choque",
                Tasa = 4.50m,
                FechaCreacion = fechaSeed,
                CreadoPor = "Seed"
            },
            new Cobertura
            {
                Id = 3,
                Nombre = "Responsabilidad Civil",
                Tasa = 2.00m,
                FechaCreacion = fechaSeed,
                CreadoPor = "Seed"
            });
    }
}