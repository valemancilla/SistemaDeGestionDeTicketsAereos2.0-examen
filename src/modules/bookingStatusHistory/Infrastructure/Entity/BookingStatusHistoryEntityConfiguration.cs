using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Infrastructure.Entity;

// Configuración de EF Core para mapear BookingStatusHistoryEntity a la tabla BookingStatusHistory
public sealed class BookingStatusHistoryEntityConfiguration : IEntityTypeConfiguration<BookingStatusHistoryEntity>
{
    public void Configure(EntityTypeBuilder<BookingStatusHistoryEntity> builder)
    {
        builder.ToTable("BookingStatusHistory");

        // Clave primaria
        builder.HasKey(x => x.IdHistory);

        // El ID se genera automáticamente
        builder.Property(x => x.IdHistory)
            .HasColumnName("IdHistory")
            .ValueGeneratedOnAdd();

        // FK a la reserva, obligatoria
        builder.Property(x => x.IdBooking)
            .HasColumnName("IdBooking")
            .IsRequired();

        // FK al estado, obligatoria
        builder.Property(x => x.IdStatus)
            .HasColumnName("IdStatus")
            .IsRequired();

        // Fecha del cambio, se registra automáticamente con la hora actual
        builder.Property(x => x.ChangeDate)
            .HasColumnName("ChangeDate")
            .HasColumnType("datetime")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        // FK al usuario que hizo el cambio, obligatoria
        builder.Property(x => x.IdUser)
            .HasColumnName("IdUser")
            .IsRequired();

        // Observación opcional, máximo 255 caracteres
        builder.Property(x => x.Observation)
            .HasColumnName("Observation")
            .HasColumnType("varchar(255)");

        // Relación: el historial pertenece a una reserva, una reserva puede tener varios registros históricos
        builder.HasOne(x => x.Booking)
            .WithMany(x => x.BookingStatusHistories)
            .HasForeignKey(x => x.IdBooking)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: cada entrada del historial tiene un estado asociado
        builder.HasOne(x => x.Status)
            .WithMany(x => x.BookingStatusHistories)
            .HasForeignKey(x => x.IdStatus)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: cada cambio fue hecho por un usuario del sistema
        builder.HasOne(x => x.User)
            .WithMany(x => x.BookingStatusHistories)
            .HasForeignKey(x => x.IdUser)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
