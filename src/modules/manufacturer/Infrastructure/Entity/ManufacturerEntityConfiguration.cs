using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Infrastructure.Entity;

// Configuración de EF Core para mapear ManufacturerEntity a la tabla Manufacturer
public sealed class ManufacturerEntityConfiguration : IEntityTypeConfiguration<ManufacturerEntity>
{
    public void Configure(EntityTypeBuilder<ManufacturerEntity> builder)
    {
        builder.ToTable("Manufacturer");

        // Clave primaria
        builder.HasKey(x => x.IdManufacturer);

        // El ID se genera automáticamente
        builder.Property(x => x.IdManufacturer)
            .HasColumnName("IdManufacturer")
            .ValueGeneratedOnAdd();

        // Nombre del fabricante, obligatorio, máximo 100 caracteres
        builder.Property(x => x.Name)
            .HasColumnName("Name")
            .HasColumnType("varchar(100)")
            .IsRequired();

        builder.HasData(
            new ManufacturerEntity { IdManufacturer = 1, Name = "Boeing" },
            new ManufacturerEntity { IdManufacturer = 2, Name = "Airbus" });
    }
}
