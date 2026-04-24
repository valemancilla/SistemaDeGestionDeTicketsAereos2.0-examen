using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.crew.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.route.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.flight.Infrastructure.Entity;

// Configuración de EF Core para mapear FlightEntity a la tabla Flight
public sealed class FlightEntityConfiguration : IEntityTypeConfiguration<FlightEntity>
{
    public void Configure(EntityTypeBuilder<FlightEntity> builder)
    {
        builder.ToTable("Flight");

        // Clave primaria
        builder.HasKey(x => x.IdFlight);

        // El ID se genera automáticamente
        builder.Property(x => x.IdFlight)
            .HasColumnName("IdFlight")
            .ValueGeneratedOnAdd();

        // FK a la ruta, obligatoria
        builder.Property(x => x.IdRoute)
            .HasColumnName("IdRoute")
            .IsRequired();

        // FK al avión, obligatoria
        builder.Property(x => x.IdAircraft)
            .HasColumnName("IdAircraft")
            .IsRequired();

        // Número de vuelo, obligatorio, máximo 10 caracteres (ej: AV101)
        builder.Property(x => x.FlightNumber)
            .HasColumnName("FlightNumber")
            .HasColumnType("varchar(10)")
            .IsRequired();

        // Fecha del vuelo, solo fecha sin hora
        builder.Property(x => x.Date)
            .HasColumnName("Date")
            .HasColumnType("date")
            .IsRequired();

        // Hora de salida del vuelo
        builder.Property(x => x.DepartureTime)
            .HasColumnName("DepartureTime")
            .IsRequired();

        // Hora de llegada estimada
        builder.Property(x => x.ArrivalTime)
            .HasColumnName("ArrivalTime")
            .IsRequired();

        // Capacidad total del vuelo, obligatoria
        builder.Property(x => x.TotalCapacity)
            .HasColumnName("TotalCapacity")
            .IsRequired();

        // Asientos disponibles, se va actualizando conforme se hacen reservas
        builder.Property(x => x.AvailableSeats)
            .HasColumnName("AvailableSeats")
            .IsRequired();

        // FK al estado del vuelo, obligatoria
        builder.Property(x => x.IdStatus)
            .HasColumnName("IdStatus")
            .IsRequired();

        // FK a la tripulación asignada, obligatoria
        builder.Property(x => x.IdCrew)
            .HasColumnName("IdCrew")
            .IsRequired();

        // FK a la tarifa asignada al vuelo (precio). Null = no asignada (compatibilidad con datos antiguos).
        builder.Property(x => x.IdFare)
            .HasColumnName("IdFare")
            .IsRequired(false);

        // Relación: un vuelo sigue una ruta específica entre origen y destino
        builder.HasOne(x => x.Route)
            .WithMany(x => x.Flights)
            .HasForeignKey(x => x.IdRoute)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: un vuelo opera con un avión específico
        builder.HasOne(x => x.Aircraft)
            .WithMany(x => x.Flights)
            .HasForeignKey(x => x.IdAircraft)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: un vuelo tiene un estado que puede cambiar durante su ciclo de vida
        builder.HasOne(x => x.Status)
            .WithMany(x => x.Flights)
            .HasForeignKey(x => x.IdStatus)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: un vuelo tiene una tripulación asignada
        builder.HasOne(x => x.Crew)
            .WithMany(x => x.Flights)
            .HasForeignKey(x => x.IdCrew)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: un vuelo puede tener una tarifa asignada (precio por vuelo).
        builder.HasOne(x => x.Fare)
            .WithMany()
            .HasForeignKey(x => x.IdFare)
            .OnDelete(DeleteBehavior.Restrict);

        // Vuelo demo: BOG → MIA (ruta 1), avión y tripulación precargados, estado Programado.
        builder.HasData(
            new FlightEntity
            {
                IdFlight = 1,
                IdRoute = 1,
                IdAircraft = 1,
                FlightNumber = "DM101",
                Date = new DateOnly(2026, 12, 15),
                DepartureTime = new TimeOnly(8, 30),
                ArrivalTime = new TimeOnly(14, 45),
                TotalCapacity = 20,
                AvailableSeats = 20,
                IdStatus = 1,
                IdCrew = 1,
                IdFare = 1
            });
    }
}
