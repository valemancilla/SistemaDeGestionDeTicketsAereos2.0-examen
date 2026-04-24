using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Infrastructure.Entity;

// Configuración de EF Core para mapear AirportTimeZoneEntity a la tabla AirportTimeZone
public sealed class AirportTimeZoneEntityConfiguration : IEntityTypeConfiguration<AirportTimeZoneEntity>
{
    public void Configure(EntityTypeBuilder<AirportTimeZoneEntity> builder)
    {
        builder.ToTable("AirportTimeZone");

        // Clave primaria compuesta: la combinación de aeropuerto y zona horaria es única
        builder.HasKey(x => new { x.IdAirport, x.IdTimeZone });

        // FK al aeropuerto, obligatoria
        builder.Property(x => x.IdAirport)
            .HasColumnName("IdAirport")
            .IsRequired();

        // FK a la zona horaria, obligatoria
        builder.Property(x => x.IdTimeZone)
            .HasColumnName("IdTimeZone")
            .IsRequired();

        // Relación: un aeropuerto puede tener muchas zonas horarias asociadas
        // Restrict: no se puede borrar un aeropuerto si tiene zonas horarias vinculadas
        builder.HasOne(x => x.Airport)
            .WithMany(x => x.AirportTimeZones)
            .HasForeignKey(x => x.IdAirport)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: una zona horaria puede estar en varios aeropuertos
        // Restrict: no se puede borrar una zona horaria si está asignada a algún aeropuerto
        builder.HasOne(x => x.TimeZone)
            .WithMany(x => x.AirportTimeZones)
            .HasForeignKey(x => x.IdTimeZone)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new AirportTimeZoneEntity { IdAirport = 1, IdTimeZone = 1 },
            new AirportTimeZoneEntity { IdAirport = 2, IdTimeZone = 1 },
            new AirportTimeZoneEntity { IdAirport = 3, IdTimeZone = 4 });
    }
}
