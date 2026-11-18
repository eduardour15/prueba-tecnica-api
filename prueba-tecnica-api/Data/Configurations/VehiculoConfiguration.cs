using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PruebaTecnica.Api.Entities;

namespace PruebaTecnica.Api.Data.Configurations;

public class VehiculoConfiguration : IEntityTypeConfiguration<Vehiculo>
{
    public void Configure(EntityTypeBuilder<Vehiculo> builder)
    {
        builder.ToTable("Vehiculos");
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Placa).IsRequired().HasMaxLength(15);
        builder.Property(v => v.Marca).IsRequired().HasMaxLength(60);
        builder.Property(v => v.Modelo).IsRequired().HasMaxLength(60);
        builder.Property(v => v.ValorComercial).HasPrecision(18, 2);

        builder.Property(v => v.CreadoPor).IsRequired().HasMaxLength(100);
        builder.Property(v => v.ModificadoPor).HasMaxLength(100);

        builder.HasIndex(v => v.Placa).IsUnique();
    }
}