using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PruebaTecnica.Api.Entities;

namespace PruebaTecnica.Api.Data.Configurations;

public class BitacoraAuditoriaConfiguration : IEntityTypeConfiguration<BitacoraAuditoria>
{
    public void Configure(EntityTypeBuilder<BitacoraAuditoria> builder)
    {
        builder.ToTable("BitacoraAuditoria");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Usuario).IsRequired().HasMaxLength(100);
        builder.Property(b => b.Accion).IsRequired().HasMaxLength(50);
        builder.Property(b => b.Entidad).IsRequired().HasMaxLength(50);
        builder.Property(b => b.EntidadId).IsRequired().HasMaxLength(50);
        builder.Property(b => b.Detalle).IsRequired().HasMaxLength(2000);
        builder.Property(b => b.CorrelationId).IsRequired().HasMaxLength(64);
        builder.Property(b => b.IpOrigen).HasMaxLength(45);

        builder.HasIndex(b => b.Fecha);
        builder.HasIndex(b => new { b.Entidad, b.EntidadId });
    }
}