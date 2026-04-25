using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Infrastructure.Entity;

// Configuración de EF Core para la tabla BaggageType
public sealed class BaggageTypeEntityConfiguration : IEntityTypeConfiguration<BaggageTypeEntity>
{
    public void Configure(EntityTypeBuilder<BaggageTypeEntity> builder)
    {
        builder.ToTable("BaggageType");

        // Clave primaria
        builder.HasKey(x => x.IdBaggageType);

        // Se genera automáticamente en la BD
        builder.Property(x => x.IdBaggageType)
            .HasColumnName("IdBaggageType")
            .ValueGeneratedOnAdd();

        // Nombre del tipo, obligatorio, máximo 50 caracteres
        builder.Property(x => x.TypeName)
            .HasColumnName("TypeName")
            .HasColumnType("varchar(50)")
            .IsRequired();

        builder.Property(x => x.WeightKg)
            .HasColumnName("WeightKg")
            .HasColumnType("decimal(6,2)")
            .IsRequired();

        builder.Property(x => x.BasePriceCop)
            .HasColumnName("BasePriceCop")
            .HasColumnType("decimal(12,2)")
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("Description")
            .HasColumnType("varchar(500)");

        builder.Property(x => x.IsActive)
            .HasColumnName("IsActive")
            .HasColumnType("tinyint(1)")
            .IsRequired();

        builder.HasData(
            new BaggageTypeEntity { IdBaggageType = 1, TypeName = "Equipaje de mano", WeightKg = 10m, BasePriceCop = 70_000m, Description = "Equipaje de mano (10 kg).", IsActive = true },
            new BaggageTypeEntity { IdBaggageType = 2, TypeName = "Equipaje de bodega", WeightKg = 23m, BasePriceCop = 70_000m, Description = "Equipaje de bodega (23 kg).", IsActive = true },
            new BaggageTypeEntity { IdBaggageType = 3, TypeName = "Equipaje especial", WeightKg = 0m, BasePriceCop = 0m, Description = "Equipaje especial.", IsActive = true },
            new BaggageTypeEntity { IdBaggageType = 7, TypeName = "Artículo personal (bolso)", WeightKg = 0m, BasePriceCop = 0m, Description = "Artículo personal (bolso).", IsActive = true }
        );
    }
}
