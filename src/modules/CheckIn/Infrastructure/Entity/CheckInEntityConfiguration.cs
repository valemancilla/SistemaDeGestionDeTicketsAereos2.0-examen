using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Infrastructure.Entity;

// Configuración de EF Core para mapear CheckInEntity a la tabla CheckIn
public sealed class CheckInEntityConfiguration : IEntityTypeConfiguration<CheckInEntity>
{
    public void Configure(EntityTypeBuilder<CheckInEntity> builder)
    {
        builder.ToTable("CheckIn");

        // Clave primaria
        builder.HasKey(x => x.IdCheckIn);

        // El ID se genera automáticamente
        builder.Property(x => x.IdCheckIn)
            .HasColumnName("IdCheckIn")
            .ValueGeneratedOnAdd();

        // FK al ticket, obligatoria
        builder.Property(x => x.IdTicket)
            .HasColumnName("IdTicket")
            .IsRequired();

        // Fecha del check-in, se captura automáticamente con la hora del servidor
        builder.Property(x => x.CheckInDate)
            .HasColumnName("CheckInDate")
            .HasColumnType("datetime")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        // FK al canal de check-in, obligatoria
        builder.Property(x => x.IdChannel)
            .HasColumnName("IdChannel")
            .IsRequired();

        // FK al asiento asignado, obligatoria
        builder.Property(x => x.IdSeat)
            .HasColumnName("IdSeat")
            .IsRequired();

        // FK al usuario que procesó el check-in, obligatoria
        builder.Property(x => x.IdUser)
            .HasColumnName("IdUser")
            .IsRequired();

        // FK al estado del check-in, obligatoria
        builder.Property(x => x.IdStatus)
            .HasColumnName("IdStatus")
            .IsRequired();

        // Relación: un check-in pertenece a un ticket, un ticket puede tener varios check-ins (ej: reasignaciones)
        builder.HasOne(x => x.Ticket)
            .WithMany(x => x.CheckIns)
            .HasForeignKey(x => x.IdTicket)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: un check-in se realizó por un canal específico
        builder.HasOne(x => x.Channel)
            .WithMany(x => x.CheckIns)
            .HasForeignKey(x => x.IdChannel)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: un check-in tiene un asiento asignado
        builder.HasOne(x => x.Seat)
            .WithMany(x => x.CheckIns)
            .HasForeignKey(x => x.IdSeat)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: un check-in lo procesó un usuario del sistema
        builder.HasOne(x => x.User)
            .WithMany(x => x.CheckIns)
            .HasForeignKey(x => x.IdUser)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: un check-in tiene un estado que indica si está completado, pendiente, etc.
        builder.HasOne(x => x.Status)
            .WithMany(x => x.CheckIns)
            .HasForeignKey(x => x.IdStatus)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
