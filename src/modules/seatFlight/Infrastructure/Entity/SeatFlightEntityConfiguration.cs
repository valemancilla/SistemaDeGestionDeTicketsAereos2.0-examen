using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Infrastructure.Entity;

// Configuración de EF Core para mapear SeatFlightEntity a la tabla SeatFlight
public sealed class SeatFlightEntityConfiguration : IEntityTypeConfiguration<SeatFlightEntity>
{
    public void Configure(EntityTypeBuilder<SeatFlightEntity> builder)
    {
        builder.ToTable("SeatFlight");

        // Clave primaria
        builder.HasKey(x => x.IdSeatFlight);

        // El ID se genera automáticamente
        builder.Property(x => x.IdSeatFlight)
            .HasColumnName("IdSeatFlight")
            .ValueGeneratedOnAdd();

        // FK al asiento, obligatoria
        builder.Property(x => x.IdSeat)
            .HasColumnName("IdSeat")
            .IsRequired();

        // FK al vuelo, obligatoria
        builder.Property(x => x.IdFlight)
            .HasColumnName("IdFlight")
            .IsRequired();

        // Por defecto el asiento está disponible al asignarse a un vuelo
        builder.Property(x => x.Available)
            .HasColumnName("Available")
            .HasColumnType("tinyint(1)")
            .HasDefaultValue(true)
            .IsRequired();

        // Un mismo asiento no puede repetirse dos veces en el mismo vuelo
        builder.HasIndex(x => new { x.IdSeat, x.IdFlight })
            .IsUnique()
            .HasDatabaseName("UQ_SeatFlight");

        // Relación: un registro SeatFlight pertenece a un asiento físico del avión
        builder.HasOne(x => x.Seat)
            .WithMany(x => x.SeatFlights)
            .HasForeignKey(x => x.IdSeat)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: un registro SeatFlight pertenece a un vuelo específico
        builder.HasOne(x => x.Flight)
            .WithMany(x => x.SeatFlights)
            .HasForeignKey(x => x.IdFlight)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new SeatFlightEntity { IdSeatFlight = 1, IdSeat = 1, IdFlight = 1, Available = true },
            new SeatFlightEntity { IdSeatFlight = 2, IdSeat = 2, IdFlight = 1, Available = true },
            new SeatFlightEntity { IdSeatFlight = 3, IdSeat = 3, IdFlight = 1, Available = true },
            new SeatFlightEntity { IdSeatFlight = 4, IdSeat = 4, IdFlight = 1, Available = true },
            new SeatFlightEntity { IdSeatFlight = 5, IdSeat = 5, IdFlight = 1, Available = true },
            new SeatFlightEntity { IdSeatFlight = 6, IdSeat = 6, IdFlight = 1, Available = true },
            new SeatFlightEntity { IdSeatFlight = 7, IdSeat = 7, IdFlight = 1, Available = true },
            new SeatFlightEntity { IdSeatFlight = 8, IdSeat = 8, IdFlight = 1, Available = true },
            new SeatFlightEntity { IdSeatFlight = 9, IdSeat = 9, IdFlight = 1, Available = true },
            new SeatFlightEntity { IdSeatFlight = 10, IdSeat = 10, IdFlight = 1, Available = true },
            new SeatFlightEntity { IdSeatFlight = 11, IdSeat = 11, IdFlight = 1, Available = true },
            new SeatFlightEntity { IdSeatFlight = 12, IdSeat = 12, IdFlight = 1, Available = true },
            new SeatFlightEntity { IdSeatFlight = 13, IdSeat = 13, IdFlight = 1, Available = true },
            new SeatFlightEntity { IdSeatFlight = 14, IdSeat = 14, IdFlight = 1, Available = true },
            new SeatFlightEntity { IdSeatFlight = 15, IdSeat = 15, IdFlight = 1, Available = true },
            new SeatFlightEntity { IdSeatFlight = 16, IdSeat = 16, IdFlight = 1, Available = true },
            new SeatFlightEntity { IdSeatFlight = 17, IdSeat = 17, IdFlight = 1, Available = true },
            new SeatFlightEntity { IdSeatFlight = 18, IdSeat = 18, IdFlight = 1, Available = true },
            new SeatFlightEntity { IdSeatFlight = 19, IdSeat = 19, IdFlight = 1, Available = true },
            new SeatFlightEntity { IdSeatFlight = 20, IdSeat = 20, IdFlight = 1, Available = true });
    }
}
