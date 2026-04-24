using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.ticket.Infrastructure.Entity;

// Configuración de EF Core para mapear TicketEntity a la tabla Ticket
public sealed class TicketEntityConfiguration : IEntityTypeConfiguration<TicketEntity>
{
    public void Configure(EntityTypeBuilder<TicketEntity> builder)
    {
        builder.ToTable("Ticket");

        // Clave primaria
        builder.HasKey(x => x.IdTicket);

        // El ID se genera automáticamente
        builder.Property(x => x.IdTicket)
            .HasColumnName("IdTicket")
            .ValueGeneratedOnAdd();

        // Código único del ticket, obligatorio, máximo 20 caracteres
        builder.Property(x => x.TicketCode)
            .HasColumnName("TicketCode")
            .HasColumnType("varchar(20)")
            .IsRequired();

        // El código del ticket debe ser único en todo el sistema
        builder.HasIndex(x => x.TicketCode)
            .IsUnique()
            .HasDatabaseName("UQ_Ticket_Code");

        // FK a la reserva, obligatoria
        builder.Property(x => x.IdBooking)
            .HasColumnName("IdBooking")
            .IsRequired();

        // FK a la tarifa, obligatoria
        builder.Property(x => x.IdFare)
            .HasColumnName("IdFare")
            .IsRequired();

        // FK al estado, obligatoria
        builder.Property(x => x.IdStatus)
            .HasColumnName("IdStatus")
            .IsRequired();

        // Fecha de emisión del ticket, se registra automáticamente al crear el registro
        builder.Property(x => x.IssueDate)
            .HasColumnName("IssueDate")
            .HasColumnType("datetime")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        // Relación: un ticket pertenece a una reserva, una reserva puede tener varios tickets
        builder.HasOne(x => x.Booking)
            .WithMany(x => x.Tickets)
            .HasForeignKey(x => x.IdBooking)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: un ticket tiene una tarifa aplicada
        builder.HasOne(x => x.Fare)
            .WithMany(x => x.Tickets)
            .HasForeignKey(x => x.IdFare)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: un ticket tiene un estado que puede cambiar durante su ciclo de vida
        builder.HasOne(x => x.Status)
            .WithMany(x => x.Tickets)
            .HasForeignKey(x => x.IdStatus)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
