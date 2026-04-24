using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seat.Infrastructure.Entity;

// Configuración de EF Core para mapear SeatEntity a la tabla Seat
public sealed class SeatEntityConfiguration : IEntityTypeConfiguration<SeatEntity>
{
    public void Configure(EntityTypeBuilder<SeatEntity> builder)
    {
        builder.ToTable("Seat");

        // Clave primaria
        builder.HasKey(x => x.IdSeat);

        // El ID se genera automáticamente
        builder.Property(x => x.IdSeat)
            .HasColumnName("IdSeat")
            .ValueGeneratedOnAdd();

        // FK al avión, obligatoria
        builder.Property(x => x.IdAircraft)
            .HasColumnName("IdAircraft")
            .IsRequired();

        // Número del asiento, obligatorio, máximo 5 caracteres (ej: 12A, 100B)
        builder.Property(x => x.Number)
            .HasColumnName("Number")
            .HasColumnType("varchar(5)")
            .IsRequired();

        // FK a la clase del asiento, obligatoria
        builder.Property(x => x.IdClase)
            .HasColumnName("IdClase")
            .IsRequired();

        // Relación: un asiento pertenece a un avión, un avión tiene muchos asientos
        builder.HasOne(x => x.Aircraft)
            .WithMany(x => x.Seats)
            .HasForeignKey(x => x.IdAircraft)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: un asiento tiene una clase (económica, business, etc.)
        builder.HasOne(x => x.SeatClass)
            .WithMany(x => x.Seats)
            .HasForeignKey(x => x.IdClase)
            .OnDelete(DeleteBehavior.Restrict);

        // Asientos del avión precargado (IdAircraft = 1); clase económica.
        builder.HasData(
            new SeatEntity { IdSeat = 1, IdAircraft = 1, Number = "1A", IdClase = 1 },
            new SeatEntity { IdSeat = 2, IdAircraft = 1, Number = "2A", IdClase = 1 },
            new SeatEntity { IdSeat = 3, IdAircraft = 1, Number = "3A", IdClase = 1 },
            new SeatEntity { IdSeat = 4, IdAircraft = 1, Number = "4A", IdClase = 1 },
            new SeatEntity { IdSeat = 5, IdAircraft = 1, Number = "5A", IdClase = 1 },
            new SeatEntity { IdSeat = 6, IdAircraft = 1, Number = "6A", IdClase = 1 },
            new SeatEntity { IdSeat = 7, IdAircraft = 1, Number = "7A", IdClase = 1 },
            new SeatEntity { IdSeat = 8, IdAircraft = 1, Number = "8A", IdClase = 1 },
            new SeatEntity { IdSeat = 9, IdAircraft = 1, Number = "9A", IdClase = 1 },
            new SeatEntity { IdSeat = 10, IdAircraft = 1, Number = "10A", IdClase = 1 },
            new SeatEntity { IdSeat = 11, IdAircraft = 1, Number = "11A", IdClase = 1 },
            new SeatEntity { IdSeat = 12, IdAircraft = 1, Number = "12A", IdClase = 1 },
            new SeatEntity { IdSeat = 13, IdAircraft = 1, Number = "13A", IdClase = 1 },
            new SeatEntity { IdSeat = 14, IdAircraft = 1, Number = "14A", IdClase = 1 },
            new SeatEntity { IdSeat = 15, IdAircraft = 1, Number = "15A", IdClase = 1 },
            new SeatEntity { IdSeat = 16, IdAircraft = 1, Number = "16A", IdClase = 1 },
            new SeatEntity { IdSeat = 17, IdAircraft = 1, Number = "17A", IdClase = 1 },
            new SeatEntity { IdSeat = 18, IdAircraft = 1, Number = "18A", IdClase = 1 },
            new SeatEntity { IdSeat = 19, IdAircraft = 1, Number = "19A", IdClase = 1 },
            new SeatEntity { IdSeat = 20, IdAircraft = 1, Number = "20A", IdClase = 1 });
    }
}
