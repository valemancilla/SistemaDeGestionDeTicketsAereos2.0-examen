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

        builder.HasData(
            new BaggageTypeEntity { IdBaggageType = 1, TypeName = "Equipaje de mano" },
            new BaggageTypeEntity { IdBaggageType = 2, TypeName = "Equipaje de bodega" },
            new BaggageTypeEntity { IdBaggageType = 3, TypeName = "Equipaje especial" },
            // Coinciden con la tarifa Business/Client (Basic, Classic, Flex) al comprar; para listar «equipaje» con la opción elegida.
            new BaggageTypeEntity { IdBaggageType = 4, TypeName = "Tarifa Basic (elegida al comprar)" },
            new BaggageTypeEntity { IdBaggageType = 5, TypeName = "Tarifa Classic (elegida al comprar)" },
            new BaggageTypeEntity { IdBaggageType = 6, TypeName = "Tarifa Flex (elegida al comprar)" }
        );
    }
}
