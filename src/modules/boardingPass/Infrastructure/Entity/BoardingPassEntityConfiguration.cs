using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Infrastructure.Entity;

public sealed class BoardingPassEntityConfiguration : IEntityTypeConfiguration<BoardingPassEntity>
{
    public void Configure(EntityTypeBuilder<BoardingPassEntity> builder)
    {
        builder.ToTable("BoardingPass");

        builder.HasKey(x => x.IdBoardingPass);

        builder.Property(x => x.IdBoardingPass)
            .HasColumnName("IdBoardingPass")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Code)
            .HasColumnName("Code")
            .HasColumnType("varchar(25)")
            .IsRequired();

        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasIndex(x => x.IdTicket).IsUnique();

        builder.Property(x => x.IdTicket).HasColumnName("IdTicket").IsRequired();
        builder.Property(x => x.IdSeat).HasColumnName("IdSeat").IsRequired();

        builder.Property(x => x.Gate)
            .HasColumnName("Gate")
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.Property(x => x.BoardingTime)
            .HasColumnName("BoardingTime")
            .HasColumnType("datetime")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        builder.Property(x => x.IdStatus).HasColumnName("IdStatus").IsRequired();

        builder.Property(x => x.PassengerFullName)
            .HasColumnName("PassengerFullName")
            .HasColumnType("varchar(200)")
            .IsRequired()
            .HasDefaultValue(string.Empty);

        builder.HasOne<TicketEntity>(x => x.Ticket)
            .WithMany()
            .HasForeignKey(x => x.IdTicket)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<SeatEntity>(x => x.Seat)
            .WithMany()
            .HasForeignKey(x => x.IdSeat)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<SystemStatusEntity>(x => x.Status)
            .WithMany()
            .HasForeignKey(x => x.IdStatus)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

