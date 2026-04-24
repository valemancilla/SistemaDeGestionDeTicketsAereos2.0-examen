using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.booking.Infrastructure.Entity;

// Configuración de EF Core para mapear BookingEntity a la tabla Booking
public sealed class BookingEntityConfiguration : IEntityTypeConfiguration<BookingEntity>
{
    public void Configure(EntityTypeBuilder<BookingEntity> builder)
    {
        builder.ToTable("Booking");

        // Clave primaria
        builder.HasKey(x => x.IdBooking);

        // El ID se genera automáticamente
        builder.Property(x => x.IdBooking)
            .HasColumnName("IdBooking")
            .ValueGeneratedOnAdd();

        // Código único de la reserva, máximo 20 caracteres
        builder.Property(x => x.BookingCode)
            .HasColumnName("BookingCode")
            .HasColumnType("varchar(20)")
            .IsRequired();

        // El código de reserva debe ser único en el sistema
        builder.HasIndex(x => x.BookingCode)
            .IsUnique()
            .HasDatabaseName("UQ_Booking_Code");

        // Fecha y hora del vuelo, se almacena como datetime completo
        builder.Property(x => x.FlightDate)
            .HasColumnName("FlightDate")
            .HasColumnType("datetime")
            .IsRequired();

        // FK al estado de la reserva, obligatoria
        builder.Property(x => x.IdStatus)
            .HasColumnName("IdStatus")
            .IsRequired();

        // FK al vuelo, obligatoria
        builder.Property(x => x.IdFlight)
            .HasColumnName("IdFlight")
            .IsRequired();

        // Cantidad de asientos, por defecto 1 cuando se crea la reserva
        builder.Property(x => x.SeatCount)
            .HasColumnName("SeatCount")
            .HasDefaultValue(1)
            .IsRequired();

        // Fecha de creación de la reserva, solo fecha sin hora
        builder.Property(x => x.CreationDate)
            .HasColumnName("CreationDate")
            .HasColumnType("date")
            .IsRequired();

        // Observaciones opcionales, máximo 255 caracteres
        builder.Property(x => x.Observations)
            .HasColumnName("Observations")
            .HasColumnType("varchar(255)");

        builder.Property(x => x.HolderEmail)
            .HasColumnName("HolderEmail")
            .HasColumnType("varchar(160)");

        builder.Property(x => x.HolderPhonePrefix)
            .HasColumnName("HolderPhonePrefix")
            .HasColumnType("varchar(8)");

        builder.Property(x => x.HolderPhone)
            .HasColumnName("HolderPhone")
            .HasColumnType("varchar(32)");

        builder.Property(x => x.ConsentDataProcessing)
            .HasColumnName("ConsentDataProcessing")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(x => x.ConsentMarketing)
            .HasColumnName("ConsentMarketing")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(x => x.IdHolderPerson)
            .HasColumnName("IdHolderPerson");

        builder.HasIndex(x => x.IdHolderPerson);

        builder.HasOne(x => x.HolderPerson)
            .WithMany()
            .HasForeignKey(x => x.IdHolderPerson)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: una reserva tiene un estado, un estado puede estar en muchas reservas
        // Restrict: no se puede borrar un estado si hay reservas con ese estado
        builder.HasOne(x => x.Status)
            .WithMany(x => x.Bookings)
            .HasForeignKey(x => x.IdStatus)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: una reserva pertenece a un vuelo, un vuelo puede tener muchas reservas
        // Restrict: no se puede borrar un vuelo si tiene reservas
        builder.HasOne(x => x.Flight)
            .WithMany(x => x.Bookings)
            .HasForeignKey(x => x.IdFlight)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
