using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SistemaDeGestionDeTicketsAereos.src.modules.crew.Infrastructure.Entity;

// Configuración de EF Core para mapear CrewEntity a la tabla Crew
public sealed class CrewEntityConfiguration : IEntityTypeConfiguration<CrewEntity>
{
    public void Configure(EntityTypeBuilder<CrewEntity> builder)
    {
        builder.ToTable("Crew");

        // Clave primaria
        builder.HasKey(x => x.IdCrew);

        // El ID se genera automáticamente
        builder.Property(x => x.IdCrew)
            .HasColumnName("IdCrew")
            .ValueGeneratedOnAdd();

        // Nombre del grupo de tripulación, obligatorio, máximo 100 caracteres
        builder.Property(x => x.GroupName)
            .HasColumnName("GroupName")
            .HasColumnType("varchar(100)")
            .IsRequired();

        builder.HasData(
            new CrewEntity { IdCrew = 1, GroupName = "Tripulación demo - vuelo precargado" });
    }
}
