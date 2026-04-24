using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.fare.Infrastructure.Entity;

// Configuración de EF Core para mapear FareEntity a la tabla Fare
public sealed class FareEntityConfiguration : IEntityTypeConfiguration<FareEntity>
{
    public void Configure(EntityTypeBuilder<FareEntity> builder)
    {
        builder.ToTable("Fare");

        // Clave primaria
        builder.HasKey(x => x.IdFare);

        // El ID se genera automáticamente
        builder.Property(x => x.IdFare)
            .HasColumnName("IdFare")
            .ValueGeneratedOnAdd();

        // Nombre de la tarifa, obligatorio, máximo 100 caracteres
        builder.Property(x => x.FareName)
            .HasColumnName("FareName")
            .HasColumnType("varchar(100)")
            .IsRequired();

        // Precio base, hasta 12 dígitos con 2 decimales (ej: 1500000.00)
        builder.Property(x => x.BasePrice)
            .HasColumnName("BasePrice")
            .HasColumnType("decimal(12,2)")
            .IsRequired();

        // FK a la aerolínea, obligatoria
        builder.Property(x => x.IdAirline)
            .HasColumnName("IdAirline")
            .IsRequired();

        // Fecha de inicio de vigencia, solo fecha sin hora
        builder.Property(x => x.ValidFrom)
            .HasColumnName("ValidFrom")
            .HasColumnType("date")
            .IsRequired();

        // Fecha de fin de vigencia, solo fecha sin hora
        builder.Property(x => x.ValidTo)
            .HasColumnName("ValidTo")
            .HasColumnType("date")
            .IsRequired();

        // Active por defecto es true cuando se crea la tarifa
        builder.Property(x => x.Active)
            .HasColumnName("Active")
            .HasColumnType("tinyint(1)")
            .HasDefaultValue(true)
            .IsRequired();

        // Fecha de vencimiento opcional, si no se pone es null
        builder.Property(x => x.ExpirationDate)
            .HasColumnName("ExpirationDate")
            .HasColumnType("date");

        // Relación: una tarifa pertenece a una aerolínea, una aerolínea puede tener muchas tarifas
        // Restrict: no se puede borrar una aerolínea si tiene tarifas registradas
        builder.HasOne(x => x.Airline)
            .WithMany(x => x.Fares)
            .HasForeignKey(x => x.IdAirline)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new FareEntity
            {
                IdFare = 1,
                FareName = "Tarifa económica demo",
                BasePrice = 450_000m,
                IdAirline = 1,
                ValidFrom = new DateOnly(2024, 1, 1),
                ValidTo = new DateOnly(2035, 12, 31),
                Active = true,
                ExpirationDate = null
            });
    }
}
