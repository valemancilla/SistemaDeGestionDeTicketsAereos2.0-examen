using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Infrastructure.Entity;

// Configuración de EF Core para mapear FlightStatusHistoryEntity a la tabla FlightStatusHistory
public sealed class FlightStatusHistoryEntityConfiguration : IEntityTypeConfiguration<FlightStatusHistoryEntity>
{
    public void Configure(EntityTypeBuilder<FlightStatusHistoryEntity> builder)
    {
        builder.ToTable("FlightStatusHistory");

        // Clave primaria
        builder.HasKey(x => x.IdHistory);

        // El ID se genera automáticamente
        builder.Property(x => x.IdHistory)
            .HasColumnName("IdHistory")
            .ValueGeneratedOnAdd();

        // FK al vuelo, obligatoria
        builder.Property(x => x.IdFlight)
            .HasColumnName("IdFlight")
            .IsRequired();

        // FK al estado, obligatoria
        builder.Property(x => x.IdStatus)
            .HasColumnName("IdStatus")
            .IsRequired();

        // Fecha del cambio, se registra automáticamente con la hora actual del servidor
        builder.Property(x => x.ChangeDate)
            .HasColumnName("ChangeDate")
            .HasColumnType("datetime")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        // FK al usuario que registró el cambio, obligatoria
        builder.Property(x => x.IdUser)
            .HasColumnName("IdUser")
            .IsRequired();

        // Observación opcional sobre el cambio, máximo 255 caracteres
        builder.Property(x => x.Observation)
            .HasColumnName("Observation")
            .HasColumnType("varchar(255)");

        // Relación: el historial pertenece a un vuelo, un vuelo puede tener varios cambios de estado
        builder.HasOne(x => x.Flight)
            .WithMany(x => x.FlightStatusHistories)
            .HasForeignKey(x => x.IdFlight)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: cada entrada tiene un estado asociado al momento del cambio
        builder.HasOne(x => x.Status)
            .WithMany(x => x.FlightStatusHistories)
            .HasForeignKey(x => x.IdStatus)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: cada cambio de estado lo hizo un usuario del sistema
        builder.HasOne(x => x.User)
            .WithMany(x => x.FlightStatusHistories)
            .HasForeignKey(x => x.IdUser)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
