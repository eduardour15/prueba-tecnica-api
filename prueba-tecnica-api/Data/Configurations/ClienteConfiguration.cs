using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PruebaTecnica.Api.Entities;

namespace PruebaTecnica.Api.Data.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nombre).IsRequired().HasMaxLength(150);
        builder.Property(c => c.Identificacion).IsRequired().HasMaxLength(30);
        builder.Property(c => c.Correo).IsRequired().HasMaxLength(150);

        builder.Property(c => c.CreadoPor).IsRequired().HasMaxLength(100);
        builder.Property(c => c.ModificadoPor).HasMaxLength(100);

        builder.HasIndex(c => c.Identificacion).IsUnique();

        var fechaSeed = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        builder.HasData(
            new Cliente
            {
                Id = 1,
                Nombre = "María Fernanda López",
                Identificacion = "001-150490-0012A",
                Correo = "maria.lopez@correo.com",
                FechaCreacion = fechaSeed,
                CreadoPor = "Seed"
            },
            new Cliente
            {
                Id = 2,
                Nombre = "Carlos Eduardo Martínez",
                Identificacion = "001-220885-0045B",
                Correo = "carlos.martinez@correo.com",
                FechaCreacion = fechaSeed,
                CreadoPor = "Seed"
            });
    }
}