using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Infrastructure.Entity;

// Configuración de EF Core para mapear TicketStatusHistoryEntity a la tabla TicketStatusHistory
public sealed class TicketStatusHistoryEntityConfiguration : IEntityTypeConfiguration<TicketStatusHistoryEntity>
{
    public void Configure(EntityTypeBuilder<TicketStatusHistoryEntity> builder)
    {
        builder.ToTable("TicketStatusHistory");

        // Clave primaria
        builder.HasKey(x => x.IdHistory);

        // El ID se genera automáticamente
        builder.Property(x => x.IdHistory)
            .HasColumnName("IdHistory")
            .ValueGeneratedOnAdd();

        // FK al ticket, obligatoria
        builder.Property(x => x.IdTicket)
            .HasColumnName("IdTicket")
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

        // Relación: el historial pertenece a un ticket, un ticket puede tener varios cambios de estado
        builder.HasOne(x => x.Ticket)
            .WithMany(x => x.TicketStatusHistories)
            .HasForeignKey(x => x.IdTicket)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: cada entrada tiene el estado al que cambió el ticket
        builder.HasOne(x => x.Status)
            .WithMany(x => x.TicketStatusHistories)
            .HasForeignKey(x => x.IdStatus)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: cada cambio fue hecho por un usuario del sistema
        builder.HasOne(x => x.User)
            .WithMany(x => x.TicketStatusHistories)
            .HasForeignKey(x => x.IdUser)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
