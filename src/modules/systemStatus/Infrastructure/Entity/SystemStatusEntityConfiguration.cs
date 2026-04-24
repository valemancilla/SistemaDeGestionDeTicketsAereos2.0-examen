using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Infrastructure.Entity;

// Configuración de EF Core para la tabla SystemStatus
public sealed class SystemStatusEntityConfiguration : IEntityTypeConfiguration<SystemStatusEntity>
{
    public void Configure(EntityTypeBuilder<SystemStatusEntity> builder)
    {
        builder.ToTable("SystemStatus");

        // Clave primaria
        builder.HasKey(x => x.IdStatus);

        // El ID se genera automáticamente en la BD
        builder.Property(x => x.IdStatus)
            .HasColumnName("IdStatus")
            .ValueGeneratedOnAdd();

        // Nombre del estado, obligatorio, máximo 50 caracteres
        builder.Property(x => x.StatusName)
            .HasColumnName("StatusName")
            .HasColumnType("varchar(50)")
            .IsRequired();

        // Indica a qué entidad aplica este estado (Vuelo, Reserva, etc.), máximo 20 caracteres
        builder.Property(x => x.EntityType)
            .HasColumnName("EntityType")
            .HasColumnType("varchar(20)")
            .IsRequired();

        builder.HasData(
            new SystemStatusEntity { IdStatus =  1, StatusName = "Programado",   EntityType = "Flight" },
            new SystemStatusEntity { IdStatus =  2, StatusName = "En vuelo",     EntityType = "Flight" },
            new SystemStatusEntity { IdStatus =  3, StatusName = "Aterrizado",   EntityType = "Flight" },
            new SystemStatusEntity { IdStatus =  4, StatusName = "Cancelado",    EntityType = "Flight" },
            new SystemStatusEntity { IdStatus =  5, StatusName = "Demorado",     EntityType = "Flight" },
            new SystemStatusEntity { IdStatus =  6, StatusName = "Confirmada",   EntityType = "Booking" },
            new SystemStatusEntity { IdStatus =  7, StatusName = "Pendiente",    EntityType = "Booking" },
            new SystemStatusEntity { IdStatus =  8, StatusName = "Cancelada",    EntityType = "Booking" },
            new SystemStatusEntity { IdStatus =  9, StatusName = "Activo",       EntityType = "Ticket" },
            new SystemStatusEntity { IdStatus = 10, StatusName = "Usado",        EntityType = "Ticket" },
            new SystemStatusEntity { IdStatus = 11, StatusName = "Cancelado",    EntityType = "Ticket" },
            new SystemStatusEntity { IdStatus = 12, StatusName = "Completado",   EntityType = "CheckIn" },
            new SystemStatusEntity { IdStatus = 13, StatusName = "Pendiente",    EntityType = "CheckIn" },
            new SystemStatusEntity { IdStatus = 14, StatusName = "Registrado",   EntityType = "Baggage" },
            new SystemStatusEntity { IdStatus = 15, StatusName = "En tránsito",  EntityType = "Baggage" },
            new SystemStatusEntity { IdStatus = 16, StatusName = "Entregado",    EntityType = "Baggage" },
            new SystemStatusEntity { IdStatus = 17, StatusName = "Aprobado",     EntityType = "Payment" },
            new SystemStatusEntity { IdStatus = 18, StatusName = "Pendiente",    EntityType = "Payment" },
            new SystemStatusEntity { IdStatus = 19, StatusName = "Rechazado",    EntityType = "Payment" },
            new SystemStatusEntity { IdStatus = 20, StatusName = "Pagada",       EntityType = "Booking" }
        );
    }
}
