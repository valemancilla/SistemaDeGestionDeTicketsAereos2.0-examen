using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Infrastructure.Entity;

// Configuración de EF Core para mapear BookingCustomerEntity a la tabla BookingCustomer
public sealed class BookingCustomerEntityConfiguration : IEntityTypeConfiguration<BookingCustomerEntity>
{
    public void Configure(EntityTypeBuilder<BookingCustomerEntity> builder)
    {
        builder.ToTable("BookingCustomer");

        // Clave primaria
        builder.HasKey(x => x.IdBookingCustomer);

        // El ID se genera automáticamente
        builder.Property(x => x.IdBookingCustomer)
            .HasColumnName("IdBookingCustomer")
            .ValueGeneratedOnAdd();

        // FK a la reserva, obligatoria
        builder.Property(x => x.IdBooking)
            .HasColumnName("IdBooking")
            .IsRequired();

        // FK al usuario, obligatoria
        builder.Property(x => x.IdUser)
            .HasColumnName("IdUser")
            .IsRequired();

        // FK a la persona, obligatoria
        builder.Property(x => x.IdPerson)
            .HasColumnName("IdPerson")
            .IsRequired();

        // FK al asiento asignado, obligatoria
        builder.Property(x => x.IdSeat)
            .HasColumnName("IdSeat")
            .IsRequired();

        // Por defecto el primer cliente asociado es el titular principal
        builder.Property(x => x.IsPrimary)
            .HasColumnName("IsPrimary")
            .HasColumnType("tinyint(1)")
            .HasDefaultValue(true)
            .IsRequired();

        // La fecha de asociación se toma automáticamente al momento de registrar
        builder.Property(x => x.AssociationDate)
            .HasColumnName("AssociationDate")
            .HasColumnType("datetime")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        // Relación: un cliente-reserva pertenece a una reserva, una reserva puede tener varios clientes
        builder.HasOne(x => x.Booking)
            .WithMany(x => x.BookingCustomers)
            .HasForeignKey(x => x.IdBooking)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: un cliente-reserva está vinculado a una persona con sus datos
        builder.HasOne(x => x.Person)
            .WithMany(x => x.BookingCustomers)
            .HasForeignKey(x => x.IdPerson)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: un cliente-reserva está vinculado a un usuario del sistema
        builder.HasOne(x => x.User)
            .WithMany(x => x.BookingCustomers)
            .HasForeignKey(x => x.IdUser)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: un cliente-reserva tiene un asiento asignado, un asiento puede aparecer en varios registros
        builder.HasOne(x => x.Seat)
            .WithMany(x => x.BookingCustomers)
            .HasForeignKey(x => x.IdSeat)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
