using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Infrastructure.Entity;

// Configuración de EF Core para la tabla CheckInChannel
public sealed class CheckInChannelEntityConfiguration : IEntityTypeConfiguration<CheckInChannelEntity>
{
    public void Configure(EntityTypeBuilder<CheckInChannelEntity> builder)
    {
        builder.ToTable("CheckInChannel");

        // Clave primaria
        builder.HasKey(x => x.IdChannel);

        // Se genera automáticamente
        builder.Property(x => x.IdChannel)
            .HasColumnName("IdChannel")
            .ValueGeneratedOnAdd();

        // Nombre del canal, obligatorio, máximo 50 caracteres
        builder.Property(x => x.ChannelName)
            .HasColumnName("ChannelName")
            .HasColumnType("varchar(50)")
            .IsRequired();

        builder.HasData(
            new CheckInChannelEntity { IdChannel = 1, ChannelName = "Web" },
            new CheckInChannelEntity { IdChannel = 2, ChannelName = "Móvil" },
            new CheckInChannelEntity { IdChannel = 3, ChannelName = "Kiosco aeropuerto" },
            new CheckInChannelEntity { IdChannel = 4, ChannelName = "Mostrador" }
        );
    }
}
