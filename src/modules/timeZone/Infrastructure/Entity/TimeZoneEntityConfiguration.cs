using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Infrastructure.Entity;

// Configuración de EF Core para la tabla TimeZone
public sealed class TimeZoneEntityConfiguration : IEntityTypeConfiguration<TimeZoneEntity>
{
    public void Configure(EntityTypeBuilder<TimeZoneEntity> builder)
    {
        builder.ToTable("TimeZone");

        // Clave primaria
        builder.HasKey(x => x.IdTimeZone);

        // Se genera automáticamente
        builder.Property(x => x.IdTimeZone)
            .HasColumnName("IdTimeZone")
            .ValueGeneratedOnAdd();

        // Nombre de la zona horaria, obligatorio, máximo 100 caracteres
        builder.Property(x => x.Name)
            .HasColumnName("Name")
            .HasColumnType("varchar(100)")
            .IsRequired();

        // Offset UTC (ej: -05:00), máximo 10 caracteres
        builder.Property(x => x.UTCOffset)
            .HasColumnName("UTCOffset")
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.HasData(
            new TimeZoneEntity { IdTimeZone = 1, Name = "Bogotá / Lima / Quito", UTCOffset = "UTC-5" },
            new TimeZoneEntity { IdTimeZone = 2, Name = "Buenos Aires / Santiago", UTCOffset = "UTC-3" },
            new TimeZoneEntity { IdTimeZone = 3, Name = "Ciudad de México", UTCOffset = "UTC-6" },
            new TimeZoneEntity { IdTimeZone = 4, Name = "Nueva York / Miami", UTCOffset = "UTC-5" },
            new TimeZoneEntity { IdTimeZone = 5, Name = "Madrid / París", UTCOffset = "UTC+1" },
            new TimeZoneEntity { IdTimeZone = 6, Name = "Londres", UTCOffset = "UTC+0" },
            new TimeZoneEntity { IdTimeZone = 7, Name = "Caracas", UTCOffset = "UTC-4" }
        );
    }
}
