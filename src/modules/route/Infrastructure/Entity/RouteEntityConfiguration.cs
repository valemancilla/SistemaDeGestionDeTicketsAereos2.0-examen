using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.route.Infrastructure.Entity;

// Configuración de EF Core para mapear RouteEntity a la tabla Route
public sealed class RouteEntityConfiguration : IEntityTypeConfiguration<RouteEntity>
{
    public void Configure(EntityTypeBuilder<RouteEntity> builder)
    {
        builder.ToTable("Route");

        // Clave primaria
        builder.HasKey(x => x.IdRoute);

        // El ID se genera automáticamente
        builder.Property(x => x.IdRoute)
            .HasColumnName("IdRoute")
            .ValueGeneratedOnAdd();

        // FK al aeropuerto de origen, obligatoria
        builder.Property(x => x.OriginAirport)
            .HasColumnName("OriginAirport")
            .IsRequired();

        // FK al aeropuerto de destino, obligatoria
        builder.Property(x => x.DestinationAirport)
            .HasColumnName("DestinationAirport")
            .IsRequired();

        // Distancia en kilómetros, hasta 10 dígitos con 2 decimales
        builder.Property(x => x.DistanceKm)
            .HasColumnName("DistanceKm")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        // Duración estimada del vuelo en formato de hora
        builder.Property(x => x.EstDuration)
            .HasColumnName("EstDuration")
            .IsRequired();

        // Active por defecto es true cuando se registra la ruta
        builder.Property(x => x.Active)
            .HasColumnName("Active")
            .HasColumnType("tinyint(1)")
            .HasDefaultValue(true)
            .IsRequired();

        // Relación: una ruta tiene un aeropuerto de origen
        // Se usa WithMany con la colección RoutesAsOrigin para distinguir las dos FK hacia Airport
        builder.HasOne(x => x.OriginAirportNavigation)
            .WithMany(x => x.RoutesAsOrigin)
            .HasForeignKey(x => x.OriginAirport)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: una ruta tiene un aeropuerto de destino
        // Se usa WithMany con la colección RoutesAsDestination para no confundir con el origen
        builder.HasOne(x => x.DestinationAirportNavigation)
            .WithMany(x => x.RoutesAsDestination)
            .HasForeignKey(x => x.DestinationAirport)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new RouteEntity
            {
                IdRoute = 1,
                OriginAirport = 1,
                DestinationAirport = 3,
                DistanceKm = 2440m,
                EstDuration = new TimeOnly(6, 15),
                Active = true
            },
            new RouteEntity
            {
                IdRoute = 2,
                OriginAirport = 1,
                DestinationAirport = 2,
                DistanceKm = 240m,
                EstDuration = new TimeOnly(0, 55),
                Active = true
            });
    }
}
