using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Infrastructure.Entity;

// Configuración de EF Core para mapear BookingCancellationEntity a la tabla BookingCancellation
public sealed class BookingCancellationEntityConfiguration : IEntityTypeConfiguration<BookingCancellationEntity>
{
    public void Configure(EntityTypeBuilder<BookingCancellationEntity> builder)
    {
        builder.ToTable("BookingCancellation");

        // Clave primaria
        builder.HasKey(x => x.IdCancellation);

        // El ID se genera automáticamente
        builder.Property(x => x.IdCancellation)
            .HasColumnName("IdCancellation")
            .ValueGeneratedOnAdd();

        // FK a la reserva cancelada, obligatoria
        builder.Property(x => x.IdBooking)
            .HasColumnName("IdBooking")
            .IsRequired();

        // Motivo de la cancelación, obligatorio, máximo 255 caracteres
        builder.Property(x => x.CancellationReason)
            .HasColumnName("CancellationReason")
            .HasColumnType("varchar(255)")
            .IsRequired();

        // Penalización en dinero, por defecto 0 si no hay cobro adicional
        builder.Property(x => x.PenaltyAmount)
            .HasColumnName("PenaltyAmount")
            .HasColumnType("decimal(10,2)")
            .HasDefaultValue(0.00m)
            .IsRequired();

        // Fecha de cancelación, se toma automáticamente al momento de registrar
        builder.Property(x => x.CancellationDate)
            .HasColumnName("CancellationDate")
            .HasColumnType("datetime")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        // FK al usuario que procesó la cancelación, obligatoria
        builder.Property(x => x.IdUser)
            .HasColumnName("IdUser")
            .IsRequired();

        // Relación: una cancelación pertenece a una reserva, una reserva puede tener varias cancelaciones
        builder.HasOne(x => x.Booking)
            .WithMany(x => x.BookingCancellations)
            .HasForeignKey(x => x.IdBooking)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: una cancelación la hace un usuario, un usuario puede cancelar varias reservas
        builder.HasOne(x => x.User)
            .WithMany(x => x.BookingCancellations)
            .HasForeignKey(x => x.IdUser)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
